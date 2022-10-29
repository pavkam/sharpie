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

using Curses;

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
    private ulong? _initialMouseMask;
    private Screen _screen;
    private SoftLabelKeyManager _softLabelKeyManager;

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

        if (options.MouseClickDelay < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.MouseClickDelay));
        }

        if (_terminalInstanceActive)
        {
            throw new InvalidOperationException(
                "Another terminal instance is active. Only one instance can be active at one time.");
        }


        // Pre-screen creation.
        curses.use_env(Options.UseEnvironmentOverrides);

        // Screen setup.
        _softLabelKeyManager = new(curses, Options.SoftLabelKeyMode);
        _screen = new(curses, curses.initscr()
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

        if (Options.TranslateReturnToNewLineChar)
        {
            curses.nl()
                  .Check(nameof(curses.nl), "Failed to enable new line translation.");
        } else
        {
            curses.nonl()
                  .Check(nameof(curses.nonl), "Failed to disable new line translation.");
        }

        // Caret configuration
        _initialCaretMode = Curses.curs_set((int) Options.CaretMode)
                                  .Check(nameof(Curses.curs_set), "Failed to change the caret mode.");

        _screen.IgnoreHardwareCaret = Options.CaretMode == CaretMode.Invisible;

        // Mouse configuration
        if (Options.UseMouse)
        {
            _initialMouseClickDelay = Curses.mouseinterval(Options.MouseClickDelay)
                                            .Check(nameof(Curses.mouseinterval),
                                                "Failed to set the mouse click interval.");

            Curses.mousemask((ulong) RawMouseEvent.EventType.ReportPosition | (ulong) RawMouseEvent.EventType.All,
                      out var initialMouseMask)
                  .Check(nameof(Curses.mousemask), "Failed to enable the mouse.");

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

        // Other configuration
        curses.meta(IntPtr.Zero, true);
        _terminalInstanceActive = true;
    }

    /// <summary>
    ///     Gets the terminal's baud rate.
    /// </summary>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
    ///     Attempts to notify the user with audio or flashing alert.
    /// </summary>
    /// <remarks>The actual notification depends on terminal support.</remarks>
    /// <param name="silent">The alert mode.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
