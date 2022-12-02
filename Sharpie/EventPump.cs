namespace Sharpie;

/// <summary>
///     The event "pump" class listens to all events from Curses, processes them and passes them along to
///     consumers.
/// </summary>
[PublicAPI]
public sealed class EventPump
{
    private readonly ICursesProvider _curses;
    private readonly IList<ResolveEscapeSequenceFunc> _keySequenceResolvers = new List<ResolveEscapeSequenceFunc>();
    private readonly Screen _screen;
    private MouseEventResolver? _mouseEventResolver;

    /// <summary>
    ///     Creates a new instances of this class.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="screen">The screen object.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="curses" /> or <paramref name="screen" /> are
    ///     <c>null</c>.
    /// </exception>
    internal EventPump(ICursesProvider curses, Screen screen)
    {
        _curses = curses ?? throw new ArgumentNullException(nameof(curses));
        _screen = screen ?? throw new ArgumentNullException(nameof(screen));
    }

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

    private (int result, uint keyCode) ReadNext(IntPtr windowHandle, bool quickWait)
    {
        _curses.wtimeout(windowHandle, quickWait ? 10 : 100);
        var result = _curses.wget_wch(windowHandle, out var keyCode);

        return (result, keyCode);
    }

    private Event? ReadNextEvent(IntPtr windowHandle, bool quickWait)
    {
        var (result, keyCode) = ReadNext(windowHandle, quickWait);
        if (result.Failed())
        {
            _screen.ApplyPendingRefreshes();
            return null;
        }

        if (result == (int) CursesKey.Yes)
        {
            switch (keyCode)
            {
                case (uint) CursesKey.Resize:
                    return new TerminalResizeEvent(_screen.Size);
                case (uint) CursesKey.Mouse:
                    if (_curses.getmouse(out var mouseEvent)
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
                    return new KeyEvent(key, new(ControlCharacter.Null), _curses.key_name(keyCode), keyMod);
            }
        }

        return new KeyEvent(Key.Character, new(keyCode), _curses.key_name(keyCode), ModifierKey.None);
    }

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <remarks>
    ///     The enumerable returned by this method only stops waiting when cancellation is requested.
    /// </remarks>
    /// <param name="window">The window to refresh during event processing.</param>
    /// <param name="cancellationToken">Cancellation token used to interrupt the process.</param>
    /// <returns>The event listening enumerable.</returns>
    public IEnumerable<Event> Listen(Window window, CancellationToken cancellationToken)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        var escapeSequence = new List<KeyEvent>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var @event = ReadNextEvent(window.Handle, escapeSequence.Count > 0);
            if (@event is KeyEvent ke)
            {
                escapeSequence.Add(ke);
                var count = TryResolveKeySequence(escapeSequence, false, out var resolved);
                if (resolved != null)
                {
                    escapeSequence.RemoveRange(0, count);
                }

                @event = resolved;
            } else if (@event is not DelegateEvent)
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

            // Flush the event if anything in there.
            if (@event is not null)
            {
                yield return @event;

                if (@event.Type == EventType.TerminalResize)
                {
                    _screen.ForceInvalidateAndRefresh();
                }
            }
        }
    }

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <param name="window">The window to refresh during event processing.</param>
    /// <returns>The event listening enumerable.</returns>
    public IEnumerable<Event> Listen(Window window) => Listen(window, CancellationToken.None);

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <remarks>
    ///     The enumerable returned by this method only stops waiting when cancellation is requested.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token used to interrupt the process.</param>
    /// <returns>The event listening enumerable.</returns>
    public IEnumerable<Event> Listen(CancellationToken cancellationToken) => Listen(_screen, cancellationToken);

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <returns>The event listening enumerable.</returns>
    public IEnumerable<Event> Listen() => Listen(CancellationToken.None);

    /// <summary>
    ///     Registers a key sequence resolver into the input pipeline.
    /// </summary>
    /// <param name="resolver">The resolver to register.</param>
    /// <exception cref="ArgumentNullException">Thrown is <paramref name="resolver" /> is <c>null</c>.</exception>
    public void Use(ResolveEscapeSequenceFunc resolver)
    {
        if (resolver == null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        if (!Uses(resolver))
        {
            _keySequenceResolvers.Add(resolver);
        }
    }

    /// <summary>
    ///     Checks if the screen has a given key sequence resolver registered.
    /// </summary>
    /// <param name="resolver">The resolver to check.</param>
    /// <returns><c>true</c> if the resolver is registered; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown is <paramref name="resolver" /> is <c>null</c>.</exception>
    public bool Uses(ResolveEscapeSequenceFunc resolver)
    {
        if (resolver == null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        return _keySequenceResolvers.Contains(resolver);
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
            var (resKey, resCount) = resolver(sequence, _curses.key_name);

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
}
