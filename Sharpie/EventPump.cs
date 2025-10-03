/*
Copyright (c) 2022-2025, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Sharpie;

using System.Collections.Concurrent;

/// <summary>
///     The event "pump" class listens to all events from Curses, processes them and passes them along to
///     consumers.
/// </summary>
[PublicAPI]
public sealed class EventPump: IEventPump
{
    private readonly IList<ResolveEscapeSequenceFunc> _keySequenceResolvers = [];
    private readonly ConcurrentQueue<object> _delegatedObjects = new();
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
    public Terminal Terminal
    {
        get;
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

    /// <inheritdoc cref="IColorManager.Terminal" />
    ITerminal IEventPump.Terminal => Terminal;

    /// <inheritdoc cref="IEventPump.Listen(ISurface,CancellationToken)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public IEnumerable<Event> Listen(ISurface surface, CancellationToken cancellationToken) => surface == null ? throw new ArgumentNullException(nameof(surface)) : Listen(surface.Handle, cancellationToken);

    /// <inheritdoc cref="IEventPump.Listen(ISurface)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public IEnumerable<Event> Listen(ISurface surface) => Listen(surface, CancellationToken.None);

    /// <inheritdoc cref="IEventPump.Listen(CancellationToken)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public IEnumerable<Event> Listen(CancellationToken cancellationToken)
    {
        var padHandle = Terminal.Curses.newpad(1, 1)
                                .Check(nameof(Terminal.Curses.newpad), "Failed to create dummy listen pad.");

        _ = Terminal.Curses.keypad(padHandle, true)
                .Check(nameof(Terminal.Curses.keypad), "Failed to configure dummy listen pad.");

        try
        {
            foreach (var e in Listen(padHandle, cancellationToken))
            {
                yield return e;
            }
        }
        finally
        {
            _ = Terminal.Curses.delwin(padHandle)
                    .Check(nameof(Terminal.Curses.delwin), "Failed to remove the dummy listen pad.");
        }
    }

    /// <inheritdoc cref="IEventPump.Listen()" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
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

    private void AssertSynchronized() => Terminal.AssertSynchronized();

    private IEnumerable<Event> Listen(IntPtr handle, CancellationToken cancellationToken)
    {
        Debug.Assert(handle != IntPtr.Zero);

        AssertSynchronized();

        var escapeSequence = new List<KeyEvent>();
        yield return new StartEvent();

        while (!cancellationToken.IsCancellationRequested)
        {
            var @event = ReadNextEvent(handle, escapeSequence.Count > 0);
            switch (@event)
            {
                case null:
                    _ = Terminal.TryUpdate();
                    break;
                case KeyEvent ke:
                    {
                        escapeSequence.Add(ke);
                        var count = TryResolveKeySequence(escapeSequence, false, out var resolved);
                        if (resolved != null)
                        {
                            escapeSequence.RemoveRange(0, count);
                        }

                        @event = resolved;
                        break;
                    }
                default:
                    {
                        if (@event.Type != EventType.Delegate)
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

                                default:
                                    break;
                            }
                        }

                        break;
                    }
            }

            if (@event is not null)
            {
                var isResize = @event.Type == EventType.TerminalResize;

                if (isResize)
                {
                    Terminal.Screen.AdjustChildrenToExplicitArea();
                }

                yield return @event;

                if (isResize && Terminal.Screen.ManagedWindows)
                {
                    using (Terminal.AtomicRefresh())
                    {
                        Terminal.Screen.MarkDirty();
                        Terminal.Screen.Refresh();
                    }
                }
            }
        }

        yield return new StopEvent();
    }

    private Event? ReadNextEvent(IntPtr windowHandle, bool quickWait)
    {
        if (_delegatedObjects.TryDequeue(out var @object))
        {
            return new DelegateEvent(@object);
        }

        _ = Terminal.Curses.wget_event(windowHandle, quickWait ? 10 : 50, out var raw);
        return raw switch
        {
            CursesResizeEvent => new TerminalResizeEvent(Terminal.Screen.Size),
            CursesMouseEvent { Button: MouseButton.Unknown } cme => new MouseMoveEvent(new(cme.X, cme.Y)),
            CursesMouseEvent { Button: not MouseButton.Unknown } cme => new MouseActionEvent(new(cme.X, cme.Y),
                cme.Button, cme.State, cme.Modifiers),
            CursesKeyEvent { Name: var name, Key: var key, Modifiers: var mod } => new KeyEvent(key,
                new(ControlCharacter.Null), name, mod),
            CursesCharEvent { Name: var name, Char: var ch, Modifiers: var mod } => new KeyEvent(Key.Character, new(ch),
                name, mod),
            var _ => null
        };
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
            var (resKey, resCount) = resolver(sequence);

            if (resCount >= max && resKey != null && best)
            {
                max = resCount;
                resolved = resKey;
            }
            else if (resCount > max && resKey != null)
            {
                max = resCount;
                resolved = resKey;
            }
            else if (resCount >= max && resKey == null && !best)
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
