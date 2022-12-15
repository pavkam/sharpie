namespace Sharpie;

using System.Collections.Concurrent;

/// <summary>
///     The event "pump" class listens to all events from Curses, processes them and passes them along to
///     consumers.
/// </summary>
[PublicAPI]
public sealed class EventPump: IEventPump
{
    private readonly IList<ResolveEscapeSequenceFunc> _keySequenceResolvers = new List<ResolveEscapeSequenceFunc>();
    private ConcurrentQueue<object> _delegatedObjects = new();
    private MouseEventResolver? _mouseEventResolver;

    /// <summary>
    ///     Creates a new instances of this class.
    /// </summary>
    /// <param name="parent">The parent terminal.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="parent" /> is <c>null</c>.
    /// </exception>
    /// <remarks>This method is not thread-safe.</remarks>
    internal EventPump(Terminal parent) => Terminal = parent ?? throw new ArgumentNullException(nameof(parent));

    /// <inheritdoc cref="IColorManager.Terminal" />
    public Terminal Terminal { get; }

    /// <summary>
    ///     Gets or sets the flag indicating whether the internal mouse resolver should be used.
    ///     This is an internal property and initialized by the terminal.
    /// </summary>
    internal bool UseInternalMouseEventResolver
    {
        get => _mouseEventResolver != null;
        set
        {
            _mouseEventResolver = value switch
            {
                true when _mouseEventResolver == null => new(),
                false when _mouseEventResolver != null => null,
                var _ => _mouseEventResolver
            };
        }
    }

    /// <inheritdoc cref="IColorManager.Terminal" />
    ITerminal IEventPump.Terminal => Terminal;

    /// <inheritdoc cref="IEventPump.Listen(Sharpie.Abstractions.ISurface,System.Threading.CancellationToken)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen(ISurface surface, CancellationToken cancellationToken)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        return Listen(surface.Handle, cancellationToken);
    }

    /// <inheritdoc cref="IEventPump.Listen(Sharpie.Abstractions.ISurface)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen(ISurface surface) => Listen(surface, CancellationToken.None);

    /// <inheritdoc cref="IEventPump.Listen(System.Threading.CancellationToken)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen(CancellationToken cancellationToken)
    {
        var padHandle = Terminal.Curses.newpad(1, 1)
                                .Check(nameof(Terminal.Curses.newpad), "Failed to create dummy listen pad.");

        Terminal.Curses.keypad(padHandle, true)
                .Check(nameof(Terminal.Curses.keypad), "Failed to configure dummy listen pad.");

        try
        {
            foreach (var e in Listen(padHandle, cancellationToken))
            {
                yield return e;
            }
        } finally
        {
            Terminal.Curses.delwin(padHandle)
                    .Check(nameof(Terminal.Curses.delwin), "Failed to remove the dummy listen pad.");
        }
    }

    /// <inheritdoc cref="IEventPump.Listen()" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen() => Listen(CancellationToken.None);

    /// <inheritdoc cref="IEventPump.Use" />
    public void Use(ResolveEscapeSequenceFunc resolver)
    {
        if (resolver == null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        AssertSynchronized();

        if (!Uses(resolver))
        {
            _keySequenceResolvers.Add(resolver);
        }
    }

    /// <inheritdoc cref="IEventPump.Uses" />
    public bool Uses(ResolveEscapeSequenceFunc resolver)
    {
        if (resolver == null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        AssertSynchronized();

        return _keySequenceResolvers.Contains(resolver);
    }

    private void AssertSynchronized() { Terminal.AssertSynchronized(); }

    private IEnumerable<Event> Listen(IntPtr handle, CancellationToken cancellationToken)
    {
        Debug.Assert(handle != IntPtr.Zero);

        AssertSynchronized();

        var hasPendingResize = false;
        var monitorsResizes =
            Terminal.Curses.monitor_pending_resize(() => { hasPendingResize = true; }, out var monitorHandle);

        var escapeSequence = new List<KeyEvent>();

        try
        {
            yield return new StartEvent();

            while (!cancellationToken.IsCancellationRequested)
            {
                var @event = ReadNextEvent(handle, escapeSequence.Count > 0);
                if (!monitorsResizes && @event == null || hasPendingResize)
                {
                    if (hasPendingResize)
                    {
                        @event = new TerminalAboutToResizeEvent();
                    }

                    if (Terminal.TryUpdate())
                    {
                        hasPendingResize = false;
                    }
                }

                if (@event is KeyEvent ke)
                {
                    escapeSequence.Add(ke);
                    var count = TryResolveKeySequence(escapeSequence, false, out var resolved);
                    if (resolved != null)
                    {
                        escapeSequence.RemoveRange(0, count);
                    }

                    @event = resolved;
                } else if (@event?.Type != EventType.Delegate)
                {
                    while (escapeSequence.Count > 0)
                    {
                        var count = TryResolveKeySequence(escapeSequence, true, out var resolved);
                        Debug.Assert(count > 0);
                        Debug.Assert(resolved != null);

                        escapeSequence.RemoveRange(0, count);
                        yield return resolved;
                    }

                    // Process/resolve mouse events.
                    switch (@event)
                    {
                        case MouseMoveEvent mme:
                        {
                            if (TryResolveMouseEvent(mme, out var l))
                            {
                                @event = null;
                                foreach (var oe in l)
                                {
                                    yield return oe;
                                }
                            }

                            break;
                        }
                        case MouseActionEvent mae:
                        {
                            if (TryResolveMouseEvent(mae, out var l))
                            {
                                @event = null;
                                foreach (var oe in l)
                                {
                                    yield return oe;
                                }
                            }

                            break;
                        }
                    }
                }

                if (@event is not null)
                {
                    var isResize = @event.Type == EventType.TerminalResize;

                    if (isResize)
                    {
                        if (!monitorsResizes)
                        {
                            yield return new TerminalAboutToResizeEvent();
                        }

                        Terminal.Screen.AdjustChildrenToExplicitArea();
                    }

                    yield return @event;

                    if (isResize)
                    {
                        Terminal.Screen.MarkDirty();
                        Terminal.Screen.Refresh();
                    }
                }
            }

            yield return new StopEvent();
        } finally
        {
            monitorHandle?.Dispose();
        }
    }

    private (int result, uint keyCode) ReadNext(IntPtr windowHandle, bool quickWait)
    {
        Terminal.Curses.wtimeout(windowHandle, quickWait ? 10 : 50);
        var result = Terminal.Curses.wget_wch(windowHandle, out var keyCode);

        return (result, keyCode);
    }

    private Event? ReadNextEvent(IntPtr windowHandle, bool quickWait)
    {
        if (_delegatedObjects.TryDequeue(out var @object))
        {
            return new DelegateEvent(@object);
        }

        var (result, keyCode) = ReadNext(windowHandle, quickWait);
        if (result.Failed())
        {
            return null;
        }

        if (result == (int) CursesKey.Yes)
        {
            switch (keyCode)
            {
                case (uint) CursesKey.Resize:
                    return new TerminalResizeEvent(Terminal.Screen.Size);
                case (uint) CursesKey.Mouse:
                    if (Terminal.Curses.getmouse(out var mouseEvent)
                                .Failed())
                    {
                        return null;
                    }

                    if (mouseEvent.buttonState == (uint) CursesMouseEvent.EventType.ReportPosition)
                    {
                        return new MouseMoveEvent(new(mouseEvent.x, mouseEvent.y));
                    }

                    var (button, state, mouseMod) =
                        Helpers.ConvertMouseActionEvent((CursesMouseEvent.EventType) mouseEvent.buttonState);

                    return button == 0
                        ? null
                        : new MouseActionEvent(new(mouseEvent.x, mouseEvent.y), button, state, mouseMod);
                default:
                    var (key, keyMod) = Helpers.ConvertKeyPressEvent(keyCode);
                    return new KeyEvent(key, new(ControlCharacter.Null), Terminal.Curses.key_name(keyCode), keyMod);
            }
        }

        return new KeyEvent(Key.Character, new(keyCode), Terminal.Curses.key_name(keyCode), ModifierKey.None);
    }

    /// <summary>
    ///     Tries to resolve a sequence of keys suing the registered resolvers.
    /// </summary>
    /// <param name="sequence">The sequence that contains the collected keys.</param>
    /// <param name="best">Force the return of the best match.</param>
    /// <param name="resolved">The resolved key (if any).</param>
    /// <returns>The number of matching keys. A zero value indicates no matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sequence" /> is <c>null</c>.</exception>
    internal int TryResolveKeySequence(IReadOnlyList<KeyEvent> sequence, bool best, out KeyEvent? resolved)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        resolved = null;
        var max = 0;
        if (sequence.Count == 0)
        {
            return max;
        }

        foreach (var resolver in _keySequenceResolvers)
        {
            var (resKey, resCount) = resolver(sequence, Terminal.Curses.key_name);

            if (resCount >= max && resKey != null && best)
            {
                max = resCount;
                resolved = resKey;
            } else if (resCount > max && resKey != null)
            {
                max = resCount;
                resolved = resKey;
            } else if (resCount >= max && resKey == null && !best)
            {
                max = resCount;
                resolved = resKey;
            }
        }

        if (max == 0)
        {
            resolved = sequence[0];
            max = 1;
        }

        return max;
    }

    /// <summary>
    ///     Tries to resolve a given mouse event into a sequence of different events.
    /// </summary>
    /// <param name="event">The mouse event to try and process.</param>
    /// <param name="resolved">The resolved events (if any).</param>
    /// <returns>The number of matching keys. A zero value indicates no matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="event" /> is <c>null</c>.</exception>
    internal bool TryResolveMouseEvent(MouseMoveEvent @event, [NotNullWhen(true)] out IEnumerable<Event>? resolved)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (_mouseEventResolver == null)
        {
            resolved = null;
            return false;
        }

        resolved = _mouseEventResolver.Process(@event);
        return true;
    }

    /// <summary>
    ///     Tries to resolve a given mouse event into a sequence of different events.
    /// </summary>
    /// <param name="event">The mouse event to try and process.</param>
    /// <param name="resolved">The resolved events (if any).</param>
    /// <returns>The number of matching keys. A zero value indicates no matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="event" /> is <c>null</c>.</exception>
    internal bool TryResolveMouseEvent(MouseActionEvent @event, [NotNullWhen(true)] out IEnumerable<Event>? resolved)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (_mouseEventResolver == null)
        {
            resolved = null;
            return false;
        }

        resolved = _mouseEventResolver.Process(@event);
        return true;
    }

    /// <summary>
    ///     Enqueues a delegated object to the queue.
    /// </summary>
    /// <param name="object">The object to delegate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="object" /> is <c>null</c>.</exception>
    internal void Delegate(object @object)
    {
        if (@object == null)
        {
            throw new ArgumentNullException(nameof(@object));
        }

        _delegatedObjects.Enqueue(@object);
    }
}
