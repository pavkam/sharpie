/*
Copyright (c) 2022, Alexandru Ciobanu
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
using Nito.AsyncEx;

/// <summary>
///     This class allows the developer to interact with the terminal and its settings. This is the main
///     class that is used in a Curses-based application.
/// </summary>
[PublicAPI]
public sealed class Terminal: ITerminal, IDisposable
{
    private static bool _terminalInstanceActive;
    private static object _stopSignal = new();
    
    private BlockingCollection<object>? _delegateQueue;
    private ManualResetEventSlim _runCompletedEvent = new(true);
    private ColorManager _colorManager;
    private EventPump _eventPump;
    private int? _initialCaretMode;
    private int? _initialMouseClickDelay;
    private int? _initialMouseMask;
    private Screen _screen;
    private Window? _header;
    private Window? _footer;
    private SoftLabelKeyManager _softLabelKeyManager;

    /// <summary>
    ///     Creates a new instance
    ///     of the terminal.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="options">The terminal options.</param>
    /// <exception cref="ArgumentOutOfRangeException">Some of the options are invalid.</exception>
    /// <exception cref="InvalidOperationException">Another terminal instance is active.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="curses" /> is <c>null</c>.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Terminal(ICursesProvider curses, TerminalOptions options)
    {
        Options = options;
        Curses = curses ?? throw new ArgumentNullException(nameof(curses));

        if (options.MouseClickInterval is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.MouseClickInterval));
        }

        if (_terminalInstanceActive)
        {
            throw new InvalidOperationException(
                "Another terminal instance is active. Only one instance can be active at one time.");
        }

        // Set unicode locale.
        curses.set_unicode_locale();

        // Pre-screen creation.
        curses.use_env(Options.UseEnvironmentOverrides);

        // Screen setup.
        _softLabelKeyManager = new(curses, Options.SoftLabelKeyMode);
        if (Options.AllocateHeader)
        {
            curses.ripoffline(1, (handle, _) =>
            {
                _header = handle != IntPtr.Zero ? new Window(curses, null, handle) : null;
                return 0;
            })
                  .Check(nameof(curses.ripoffline), "Failed to allocate header line.");
        }

        if (Options.AllocateFooter)
        {
            curses.ripoffline(-1, (handle, _) =>
                  {
                      _footer = handle != IntPtr.Zero ? new Window(curses, null, handle) : null;
                      return 0;
                  })
                  .Check(nameof(curses.ripoffline), "Failed to allocate footer line.");
        }

        _screen = new(curses, this, curses.initscr()
                                          .Check(nameof(curses.initscr), "Failed to create the screen window."));

        if (Options.AllocateHeader && _header == null)
        {
            throw new CursesOperationException(nameof(curses.ripoffline), "Failed to allocated header line.");
        }

        if (Options.AllocateFooter && _footer == null)
        {
            throw new CursesOperationException(nameof(curses.ripoffline), "Failed to allocated footer line.");
        }

        _eventPump = new(curses, _screen);
        _colorManager = new(curses, Options.UseColors);

        // After screen creation.
        curses.intrflush(IntPtr.Zero, Options.ManualFlush)
              .Check(nameof(curses.intrflush), "Failed to initialize manual flush.");

        if (Options.ManualFlush)
        {
            curses.noqiflush();
        } else
        {
            curses.qiflush();
        }

        if (Options.EchoInput)
        {
            curses.echo()
                  .Check(nameof(curses.echo), "Failed to setup terminal's echo mode.");
        } else
        {
            curses.noecho()
                  .Check(nameof(curses.noecho), "Failed to setup terminal's no-echo mode.");
        }

        if (Options.UseInputBuffering)
        {
            curses.noraw()
                  .Check(nameof(curses.noraw), "Failed to setup terminal's no-raw mode.");

            curses.nocbreak()
                  .Check(nameof(curses.nocbreak), "Failed to setup terminal buffered mode.");
        } else if (Options.SuppressControlKeys)
        {
            curses.raw()
                  .Check(nameof(curses.raw), "Failed to setup terminal's raw mode.");
        } else
        {
            curses.cbreak()
                  .Check(nameof(curses.cbreak), "Failed to setup terminal's non-buffered mode.");
        }

        curses.nonl()
              .Check(nameof(curses.nonl), "Failed to disable new line translation.");

        // Caret configuration
        _initialCaretMode = Curses.curs_set((int) Options.CaretMode)
                                  .Check(nameof(Curses.curs_set), "Failed to change the caret mode.");

        _screen.IgnoreHardwareCaret = Options.CaretMode == CaretMode.Invisible;

        // Mouse configuration
        if (Options.UseMouse)
        {
            _initialMouseClickDelay = Curses.mouseinterval(Options.MouseClickInterval ?? 0)
                                            .Check(nameof(Curses.mouseinterval), //TODO manual click
                                                "Failed to set the mouse click interval.");

            Curses.mousemask((int) CursesMouseEvent.EventType.ReportPosition | (int) CursesMouseEvent.EventType.All,
                      out var initialMouseMask)
                  .Check(nameof(Curses.mousemask), "Failed to enable the mouse.");

            _eventPump.UseInternalMouseEventResolver = Options.MouseClickInterval == null;
            _initialMouseMask = initialMouseMask;
        } else
        {
            Curses.mousemask(0, out var initialMouseMask)
                  .Check(nameof(Curses.mousemask), "Failed to enable the mouse.");

            _initialMouseMask = initialMouseMask;
            _initialMouseClickDelay = Curses.mouseinterval(-1)
                                            .Check(nameof(Curses.mouseinterval),
                                                "Failed to set the mouse click interval.");
        }

        // Disable meta interpretation and ignore the result.
        curses.meta(IntPtr.Zero, false);
        _terminalInstanceActive = true;

        if (options.UseStandardKeySequenceResolvers)
        {
            // Register standard key sequence resolvers.
            _eventPump.Use(KeySequenceResolver.SpecialCharacterResolver);
            _eventPump.Use(KeySequenceResolver.ControlKeyResolver);
            _eventPump.Use(KeySequenceResolver.AltKeyResolver);
            _eventPump.Use(KeySequenceResolver.KeyPadModifiersResolver);
        }
    }

    /// <inheritdoc cref="ITerminal.BaudRate"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public int BaudRate =>
        Curses.baudrate()
              .Check(nameof(Curses.baudrate), "Failed to obtain the baud rate of the terminal.");

    /// <inheritdoc cref="ITerminal.Colors"/>
    public IColorManager Colors
    {
        get
        {
            AssertAlive();
            return _colorManager;
        }
    }

    /// <inheritdoc cref="ITerminal.SoftLabelKeys"/>
    public ISoftLabelKeyManager SoftLabelKeys
    {
        get
        {
            AssertAlive();
            return _softLabelKeyManager;
        }
    }

    /// <summary>
    ///     Gets the options that are used by this terminal instance.
    /// </summary>
    public TerminalOptions Options { get; }

    /// <inheritdoc cref="ITerminal.Name"/>
    public string? Name => Curses.termname();

    /// <inheritdoc cref="ITerminal.Description"/>
    public string? Description => Curses.longname();

    /// <inheritdoc cref="ITerminal.CursesVersion"/>
    public string? CursesVersion => Curses.curses_version();

    /// <inheritdoc cref="ITerminal.SupportedAttributes"/>
    public VideoAttribute SupportedAttributes => (VideoAttribute) Curses.term_attrs();

    /// <inheritdoc cref="ITerminal.Screen"/>
    public IScreen Screen
    {
        get
        {
            AssertAlive();

            return _screen;
        }
    }
    
    /// <inheritdoc cref="ITerminal.Header"/>
    public IWindow? Header  
    {
        get
        {
            AssertAlive();

            return _header;
        }
    }
    
    /// <inheritdoc cref="ITerminal.Footer"/>
    public IWindow? Footer 
    {
        get
        {
            AssertAlive();

            return _footer;
        }
    }

    /// <inheritdoc cref="ITerminal.Events"/>
    public IEventPump Events
    {
        get
        {
            AssertAlive();
            return _eventPump;
        }
    }

    /// <inheritdoc cref="ITerminal.HasHardwareLineEditor"/>
    public bool HasHardwareLineEditor => Curses.has_il();

    /// <inheritdoc cref="ITerminal.HasHardwareCharEditor"/>
    public bool HasHardwareCharEditor => Curses.has_ic();

    /// <inheritdoc cref="ITerminal.CurrentKillChar"/>
    public Rune? CurrentKillChar =>
        Curses.killwchar(out var @char)
              .Failed()
            ? null
            : new(@char);

    /// <inheritdoc cref="ITerminal.CurrentEraseChar"/>
    public Rune? CurrentEraseChar =>
        Curses.erasewchar(out var @char)
              .Failed()
            ? null
            : new(@char);

    /// <summary>
    ///     The Curses backend.
    /// </summary>
    internal ICursesProvider Curses { get; }

    /// <summary>
    ///     Checks whether the terminal has been disposed of and is no longer usable.
    /// </summary>
    public bool Disposed { get; private set; }

    /// <summary>
    ///     Disposes the current terminal instance.
    /// </summary>
    public void Dispose()
    {
        if (Disposed)
        {
            return;
        }
        
        _delegateQueue?.Add(_stopSignal);
        _runCompletedEvent.Wait();
        _runCompletedEvent.Dispose();
        
        if (_screen != null!)
        {
            _screen.Dispose();
        }

        if (_initialMouseMask != null)
        {
            Curses.mousemask(_initialMouseMask.Value, out var _);
        }

        if (_initialMouseClickDelay != null)
        {
            Curses.mouseinterval(_initialMouseClickDelay.Value);
        }

        if (_initialCaretMode != null)
        {
            Curses.curs_set(_initialCaretMode.Value);
        }

        _terminalInstanceActive = false;
        Disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="ITerminal.SetTitle"/>
    /// <param name="title">The title of the terminal.</param>
    public void SetTitle(string title)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        Curses.set_title(title);
    }

    /// <inheritdoc cref="ITerminal.Alert"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Alert(bool silent)
    {
        if (silent)
        {
            Curses.flash()
                  .Check(nameof(Curses.flash), "Failed to flash the terminal.");
        } else
        {
            Curses.beep()
                  .Check(nameof(Curses.beep), "Failed to beep the terminal.");
        }
    }

    /// <summary>
    ///     Validates that the terminal is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed of and is no longer usable.</exception>
    private void AssertAlive()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException("The terminal has been disposed and no further operations are allowed.");
        }
    }

    private Interval NewInterval<TState>(Func<Terminal, TState?, Task> action, int dueMillis, int periodMillis,
        TState? initialState)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var interval = new Interval();

        interval.Timer = new(i =>
        {
            Debug.Assert(i is Interval);
            if (((Interval) i).Stopped || Disposed)
            {
                ((Interval) i).Timer?.Dispose();
            } else
            {
                Delegate(() => action(this, initialState));
            }
        }, interval, dueMillis, periodMillis);

        return interval;
    }
    
    private Task PumpEventsAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_delegateQueue != null);
        
        return Task.Run(() =>
        {
            foreach (var @event in Events.Listen(cancellationToken))
            {
                _delegateQueue.Add(@event, CancellationToken.None);
            }
        }, cancellationToken);
    }

    private Task PumpMessagesAsync(Func<Event, Task> action, CancellationTokenSource cancellationTokenSource,
        bool stopOnCtrlC)
    {
        Debug.Assert(_delegateQueue != null);
        Debug.Assert(action != null);
        Debug.Assert(cancellationTokenSource != null);

        return Task.Run(() =>
        {
            AsyncContext.Run(async () =>
            {
                foreach (var message in _delegateQueue.GetConsumingEnumerable())
                {
                    switch (message)
                    {
                        case Func<Task> @delegate:
                            await @delegate();
                            break;
                        case not null when message.Equals(_stopSignal):
                        case KeyEvent { Char.Value: 'C', Key: Key.Character, Modifiers: ModifierKey.Ctrl }
                            when stopOnCtrlC:
                            cancellationTokenSource.Cancel();
                            break;
                        case Event @event:
                            await action(@event);
                            break;
                    }
                    
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });
        });
    }

    /// <inheritdoc cref="ITerminal.RunAsync"/>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventAction" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if another <see cref="RunAsync"/> is already running.</exception>
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task RunAsync(Func<Event, Task> eventAction, bool stopOnCtrlC = true)
    {
        if (eventAction == null)
        {
            throw new ArgumentNullException(nameof(eventAction));
        }

        if (_delegateQueue != null)
        {
            throw new InvalidOperationException("Event processing already running.");
        }

        AssertAlive();
        
        _delegateQueue = new();
        _runCompletedEvent.Reset();
        
        var cts = new CancellationTokenSource();
        var eventTask = PumpEventsAsync(cts.Token);
        var messageTask = PumpMessagesAsync(eventAction, cts, stopOnCtrlC);

        await Task.WhenAll(eventTask, messageTask);

        _delegateQueue.Dispose();
        _delegateQueue = null;
        _runCompletedEvent.Set();
    }

    /// <inheritdoc cref="ITerminal.Delegate"/>
    public void Delegate(Func<Task> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        AssertAlive();
        _delegateQueue?.Add(action);
    }

    /// <inheritdoc cref="ITerminal.Delay"/>
    public IInterval Delay<TState>(Func<Terminal, TState?, Task> action, int delayMillis, TState? state)
    {
        if (delayMillis < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMillis));
        }

        return NewInterval(action, delayMillis, Timeout.Infinite, state);
    }

    /// <inheritdoc cref="ITerminal.Delay"/>
    public IInterval Delay(Func<Terminal, Task> action, int delayMillis)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return Delay((t, _) => action(t), delayMillis, DBNull.Value);
    }

    /// <inheritdoc cref="ITerminal.Repeat"/>
    public IInterval Repeat<TState>(Func<Terminal, TState?, Task> action, int intervalMillis, bool immediate = false,
        TState? state = default)
    {
        if (intervalMillis < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalMillis));
        }

        return NewInterval(action, immediate ? 0 : intervalMillis, intervalMillis, state);
    }

    /// <inheritdoc cref="ITerminal.Repeat"/>
    public IInterval Repeat(Func<Terminal, Task> action, int intervalMillis, bool immediate = false)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return Repeat<DBNull>((t, _) => action(t), intervalMillis, immediate);
    }

    /// <inheritdoc cref="ITerminal.Stop"/>
    /// <param name="wait">If <c>true</c>, waits until running completes.</param>
    public void Stop(bool wait = false)
    {
        AssertAlive();

        _delegateQueue?.Add(_stopSignal);
        if (wait)
        {
            _runCompletedEvent.Wait();
        }
    }

    /// <summary>
    ///     The destructor. Calls <see cref="Dispose" /> method.
    /// </summary>
    ~Terminal() { Dispose(); }

    private sealed class Interval: IInterval
    {
        public bool Stopped;
        public Timer? Timer;
        public void Stop() { Stopped = true; }
    }
}
