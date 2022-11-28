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

/// <summary>
///     This class allows the developer to interact with the terminal and its settings. This is the main
///     class that is used in a Curses-based application.
/// </summary>
[PublicAPI]
public sealed class Terminal: IDisposable
{
    private static bool _terminalInstanceActive;
    private ColorManager _colorManager;
    private int? _initialCaretMode;
    private int? _initialMouseClickDelay;
    private uint? _initialMouseMask;
    private Screen _screen;
    private SoftLabelKeyManager _softLabelKeyManager;
    private readonly IList<ResolveEscapeSequenceFunc> _keySequenceResolvers = new List<ResolveEscapeSequenceFunc>();
    private MouseEventResolver? _mouseEventResolver;

    
    /// <summary>
    ///     Creates a new instance of the terminal.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="options">The terminal options.</param>
    /// <exception cref="ArgumentOutOfRangeException">Some of the options are invalid.</exception>
    /// <exception cref="InvalidOperationException">Another terminal instance is active.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="curses" /> is <c>null</c>.</exception>
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
        _screen = new(curses, this, curses.initscr()
                                    .Check(nameof(curses.initscr), "Failed to create the screen window."));

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

            Curses.mousemask((uint) CursesMouseEvent.EventType.ReportPosition | (uint) CursesMouseEvent.EventType.All,
                      out var initialMouseMask)
                  .Check(nameof(Curses.mousemask), "Failed to enable the mouse.");

            UseInternalMouseEventResolver = Options.MouseClickInterval == null;
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
            Use(KeySequenceResolver.SpecialCharacterResolver);
            Use(KeySequenceResolver.ControlKeyResolver);
            Use(KeySequenceResolver.AltKeyResolver);
            Use(KeySequenceResolver.KeyPadModifiersResolver);
        }
    }

    /// <summary>
    ///     Gets the terminal's baud rate.
    /// </summary>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public int BaudRate =>
        Curses.baudrate()
              .Check(nameof(Curses.baudrate), "Failed to obtain the baud rate of the terminal.");

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public ColorManager Colors
    {
        get
        {
            AssertAlive();
            return _colorManager;
        }
    }

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public SoftLabelKeyManager SoftLabelKeys
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

    /// <summary>
    ///     Returns the name of the terminal.
    /// </summary>
    public string? Name => Curses.termname();

    /// <summary>
    ///     Returns the long description of the terminal.
    /// </summary>
    public string? Description => Curses.longname();

    /// <summary>
    ///     Returns the version of the Curses library in use.
    /// </summary>
    public string? CursesVersion => Curses.curses_version();

    /// <summary>
    ///     Gets the combination of supported terminal attributes.
    /// </summary>
    public VideoAttribute SupportedAttributes => (VideoAttribute) Curses.term_attrs();

    /// <summary>
    ///     The screen instance. Use this property to access the entire screen functionality.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public Screen Screen
    {
        get
        {
            AssertAlive();

            return _screen;
        }
    }

    /// <summary>
    ///     Specifies whether the terminal supports hardware line insert and delete.
    /// </summary>
    public bool HasHardwareLineEditor => Curses.has_il();

    /// <summary>
    ///     Specifies whether the terminal supports hardware character insert and delete.
    /// </summary>
    public bool HasHardwareCharEditor => Curses.has_ic();

    /// <summary>
    ///     Gets the currently defined kill character. \0 is returned if none is defined.
    /// </summary>
    public Rune? CurrentKillChar =>
        Curses.killwchar(out var @char)
              .Failed()
            ? null
            : new(@char);

    /// <summary>
    ///     Gets the currently defined erase character. \0 is returned if none is defined.
    /// </summary>
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
    public bool IsDisposed => _screen.Disposed;

    /// <summary>
    ///     Disposes the current terminal instance.
    /// </summary>
    public void Dispose()
    {
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
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Sets the terminal title.
    /// </summary>
    /// <param name="title">The title of the terminal.</param>
    public void SetTitle(string title)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        Curses.set_title(title);
    }

    /// <summary>
    ///     Attempts to notify the user with audio or flashing alert.
    /// </summary>
    /// <remarks>The actual notification depends on terminal support.</remarks>
    /// <param name="silent">The alert mode.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
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
            var (resKey, resCount) = resolver(sequence, Curses.key_name);

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
    ///     Validates that the terminal is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed of and is no longer usable.</exception>
    private void AssertAlive()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("The terminal has been disposed and no further operations are allowed.");
        }
    }

    /// <summary>
    ///     The destructor. Calls <see cref="Dispose" /> method.
    /// </summary>
    ~Terminal() { Dispose(); }
}
