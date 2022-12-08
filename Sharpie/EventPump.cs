namespace Sharpie;

/// <summary>
///     The event "pump" class listens to all events from Curses, processes them and passes them along to
///     consumers.
/// </summary>
[PublicAPI]
public sealed class EventPump: IEventPump
{
    private readonly ITerminal _terminal;
    private readonly IList<ResolveEscapeSequenceFunc> _keySequenceResolvers = new List<ResolveEscapeSequenceFunc>();
    private MouseEventResolver? _mouseEventResolver;

    /// <summary>
    ///     Creates a new instances of this class.
    /// </summary>
    /// <param name="terminal">The parent terminal.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="terminal" /> is <c>null</c>.
    /// </exception>
    internal EventPump(ITerminal terminal) => _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

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
        _terminal.Curses.wtimeout(windowHandle, quickWait ? 10 : 100);
        var result = _terminal.Curses.wget_wch(windowHandle, out var keyCode);

        return (result, keyCode);
    }

    private Event? ReadNextEvent(IntPtr windowHandle, bool quickWait)
    {
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
                    return new TerminalResizeEvent(_terminal.Screen.Size);
                case (uint) CursesKey.Mouse:
                    if (_terminal.Curses.getmouse(out var mouseEvent)
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
                    return new KeyEvent(key, new(ControlCharacter.Null), _terminal.Curses.key_name(keyCode), keyMod);
            }
        }

        return new KeyEvent(Key.Character, new(keyCode), _terminal.Curses.key_name(keyCode), ModifierKey.None);
    }

    /// <inheritdoc cref="IEventPump.Listen(Sharpie.Abstractions.ISurface,System.Threading.CancellationToken)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen(ISurface surface, CancellationToken cancellationToken)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        var hasPendingResize = false;
        var monitorsResizes = _terminal.Curses.monitor_pending_resize(() =>
        {
            hasPendingResize = true;
        }, out var monitorHandle);
        
        var escapeSequence = new List<KeyEvent>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var @event = ReadNextEvent(surface.Handle, escapeSequence.Count > 0);
            if (!monitorsResizes && @event == null || hasPendingResize)
            {
                if (hasPendingResize)
                {
                    @event = new TerminalAboutToResizeEvent();
                }
                
                _terminal.Update();
                hasPendingResize = false;
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
            } else
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
                if (@event.Type == EventType.TerminalResize && !monitorsResizes)
                {
                    yield return new TerminalAboutToResizeEvent();
                }
                
                yield return @event;

                if (@event.Type == EventType.TerminalResize)
                {
                    _terminal.Screen.FullRefresh();
                }
            }
        }
        
        monitorHandle?.Dispose();
    }

    /// <inheritdoc cref="IEventPump.Listen(System.Threading.CancellationToken)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IEnumerable<Event> Listen(CancellationToken cancellationToken) => Listen(_terminal.Screen, cancellationToken);

    /// <inheritdoc cref="IEventPump.Use"/>
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

    /// <inheritdoc cref="IEventPump.Uses"/>
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
            var (resKey, resCount) = resolver(sequence, _terminal.Curses.key_name);

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
