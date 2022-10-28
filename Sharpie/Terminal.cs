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
///     class that is used
/// </summary>
[PublicAPI]
public sealed class Terminal: IDisposable
{
    private static bool _terminalInstanceActive;
    private ColorManager _colorManager;
    private bool _enableMouse;
    private CaretMode _hardwareCursorMode;
    private int? _initialHardwareCursorMode;
    private bool _inputEchoing;
    private bool _lineBuffering;
    private bool _manualFlush;
    private bool _newLineTranslation;
    private ulong? _oldMouseMask;
    private bool _rawMode;
    private int _readTimeoutMillis;
    private Screen _screen;

    private SoftLabelKeyManager _softLabelKeyManager;
    private bool _useEnvironmentOverrides;

    internal Terminal(ICursesProvider curses, bool enableLineBuffering, bool enableRawMode,
        bool enableReturnToNewLineTranslation, int readTimeoutMillis, bool enableInputEchoing, bool manualFlush,
        bool enableColors, CaretMode hardwareCursorMode, bool useEnvironmentOverrides,
        SoftLabelKeyMode softLabelKeyMode, bool enableMouse)
    {
        if (_terminalInstanceActive)
        {
            throw new InvalidOperationException(
                "Another terminal instance is active. Only one instance can be active at one time.");
        }

        Curses = curses ?? throw new ArgumentNullException(nameof(curses));

        // Pre-screen creation.
        _useEnvironmentOverrides = useEnvironmentOverrides;
        if (_useEnvironmentOverrides)
        {
            curses.use_env(true);
        }

        _softLabelKeyManager = new(curses, softLabelKeyMode);

        // Screen setup.
        _screen = new(curses, curses.initscr());
        _colorManager = new(curses, enableColors);

        // After screen creation.
        _inputEchoing = enableInputEchoing;
        _readTimeoutMillis = readTimeoutMillis;
        _rawMode = enableRawMode;
        _lineBuffering = enableLineBuffering;
        _manualFlush = manualFlush;
        _newLineTranslation = enableReturnToNewLineTranslation;

        curses.intrflush(IntPtr.Zero, _manualFlush);
        if (_manualFlush)
        {
            curses.noqiflush();
        } else
        {
            curses.qiflush();
        }

        if (_inputEchoing)
        {
            curses.echo()
                  .Check(nameof(curses.echo), "Failed to setup terminal's echo mode.");
        } else
        {
            curses.noecho()
                  .Check(nameof(curses.noecho), "Failed to setup terminal's no-echo mode.");
        }

        if (!_lineBuffering)
        {
            if (_rawMode)
            {
                curses.raw()
                      .Check(nameof(curses.raw), "Failed to setup terminal's raw mode.");
            } else
            {
                curses.noraw()
                      .Check(nameof(curses.noraw), "Failed to setup terminal's no-raw mode.");
            }

            if (_readTimeoutMillis != Timeout.Infinite)
            {
                curses.halfdelay(Helpers.ConvertMillisToTenths(_readTimeoutMillis))
                      .Check(nameof(curses.halfdelay), "Failed to setup terminal's half-delay non-buffered mode.");
            } else if (!_rawMode)
            {
                curses.cbreak()
                      .Check(nameof(curses.cbreak), "Failed to setup terminal's non-buffered mode.");
            }
        } else
        {
            curses.nocbreak()
                  .Check(nameof(curses.nocbreak), "Failed to setup terminal buffered mode.");
        }

        if (_newLineTranslation)
        {
            curses.nl()
                  .Check(nameof(curses.nl), "Failed to enable new line translation.");
        } else
        {
            curses.nonl()
                  .Check(nameof(curses.nonl), "Failed to disable new line translation.");
        }

        CaretMode = hardwareCursorMode;
        EnableMouse = enableMouse;

        /* Other configuration */
        curses.meta(IntPtr.Zero, true);

        _terminalInstanceActive = true;
    }

    /// <summary>
    ///     Gets the terminal's baud rate.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public int BaudRate
    {
        get
        {
            AssertNotDisposed();

            return Curses.baudrate()
                         .Check(nameof(Curses.baudrate), "Failed to obtain the baud rate of the terminal.");
        }
    }

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public ColorManager Colors
    {
        get
        {
            AssertNotDisposed();

            return _colorManager;
        }
    }

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public SoftLabelKeyManager SoftLabelKey
    {
        get
        {
            AssertNotDisposed();

            return _softLabelKeyManager;
        }
    }

    /// <summary>
    ///     Enables or disables the line buffering mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableMouse
    {
        get
        {
            AssertNotDisposed();

            return _enableMouse;
        }
        set
        {
            AssertNotDisposed();

            var newMask = value
                ? (ulong) RawMouseEvent.EventType.ReportPosition | (ulong) RawMouseEvent.EventType.All
                : 0;

            Curses.mousemask(newMask, out var oldMask)
                  .Check(nameof(Curses.mousemask), "Failed to enable the mouse.");

            _oldMouseMask ??= oldMask;
            _enableMouse = value;
        }
    }

    /// <summary>
    ///     Gets or sets the interval used to treat pressed/released as clicks.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is negative.</exception>
    public int MouseClickInterval
    {
        get
        {
            AssertNotDisposed();

            return Curses.mouseinterval(-1)
                         .Check(nameof(Curses.mouseinterval), "Failed to get the mouse click interval.");
        }
        set
        {
            AssertNotDisposed();

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.mouseinterval(value)
                  .Check(nameof(Curses.mouseinterval), "Failed to set the mouse click interval.");
        }
    }

    /// <summary>
    ///     Checks if the terminal is in line buffering mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool LineBuffering
    {
        get
        {
            AssertNotDisposed();
            return _lineBuffering;
        }
    }

    /// <summary>
    ///     Check if the control keys are silenced (e.g. CTRL+C).
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool ControlKeysSilenced
    {
        get
        {
            AssertNotDisposed();

            return _rawMode;
        }
    }

    /// <summary>
    ///     Gets timeout used when reading characters in non-buffered mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public int ReadTimeout
    {
        get
        {
            AssertNotDisposed();

            return _readTimeoutMillis;
        }
    }

    /// <summary>
    ///     Returns the name of the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public string? Name
    {
        get
        {
            AssertNotDisposed();

            return Curses.termname();
        }
    }

    /// <summary>
    ///     Returns the long description of the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public string? Description
    {
        get
        {
            AssertNotDisposed();

            return Curses.longname();
        }
    }

    /// <summary>
    ///     Returns the version of the Curses library in use.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public string? CursesVersion
    {
        get
        {
            AssertNotDisposed();

            return Curses.curses_version();
        }
    }

    /// <summary>
    ///     Checks if the input echoing is enabled in this terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool InputEchoing
    {
        get
        {
            AssertNotDisposed();

            return _inputEchoing;
        }
    }

    /// <summary>
    ///     Enables or disables return to new line character translation.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool ReturnKeyTranslatesToNewLine
    {
        get
        {
            AssertNotDisposed();

            return _newLineTranslation;
        }
    }

    /// <summary>
    ///     Checks if the application flushes the terminal buffers when control keys are read by the application.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool ManualFlush
    {
        get
        {
            AssertNotDisposed();

            return _manualFlush;
        }
    }

    /// <summary>
    ///     Gets or sets the current caret mode of the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public CaretMode CaretMode
    {
        get
        {
            AssertNotDisposed();

            return _hardwareCursorMode;
        }
        set
        {
            AssertNotDisposed();

            var prevMode = Curses.curs_set((int) value)
                                 .Check(nameof(Curses.curs_set), "Failed to change the caret mode.");

            _initialHardwareCursorMode ??= prevMode;
            _hardwareCursorMode = value;

            _screen.IgnoreHardwareCaret = value == CaretMode.Invisible;
        }
    }

    /// <summary>
    ///     Gets the combination of supported terminal attributes.
    /// </summary>
    public VideoAttribute SupportedAttributes
    {
        get
        {
            AssertNotDisposed();

            return (VideoAttribute) Curses.term_attrs();
        }
    }

    /// <summary>
    ///     The screen instance. Use this property to access the entire screen functionality.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public Screen Screen
    {
        get
        {
            AssertNotDisposed();

            return _screen;
        }
    }

    /// <summary>
    ///     Specifies whether the terminal supports hardware line insert and delete.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool SupportsHardwareLineInsertAndDelete
    {
        get
        {
            AssertNotDisposed();

            return Curses.has_il();
        }
    }

    /// <summary>
    ///     Specifies whether the terminal supports hardware character insert and delete.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool SupportsHardwareCharacterInsertAndDelete
    {
        get
        {
            AssertNotDisposed();

            return Curses.has_ic();
        }
    }

    /// <summary>
    ///     Specifies whether the environment variables are used to setup the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool UsesEnvironmentOverrides
    {
        get
        {
            AssertNotDisposed();

            return _useEnvironmentOverrides;
        }
    }

    /// <summary>
    ///     Gets the currently defined kill character. \0 is returned if none is defined.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public Rune CurrentKillChar
    {
        get
        {
            AssertNotDisposed();

            return new(Curses.killwchar(out var @char)
                             .Failed()
                ? '\0'
                : @char);
        }
    }

    /// <summary>
    ///     Gets the currently defined erase character. \0 is returned if none is defined.
    /// </summary>
    public Rune CurrentEraseChar
    {
        get
        {
            AssertNotDisposed();

            return new(Curses.erasewchar(out var @char)
                             .Failed()
                ? '\0'
                : @char);
        }
    }

    /// <summary>
    ///     The Curses functionality provider.
    /// </summary>
    public ICursesProvider Curses { get; }

    /// <summary>
    ///     Specifies whether the terminal has been disposed of and is no longer usable.
    /// </summary>
    public bool IsDisposed => Curses.isendwin();

    /// <summary>
    ///     Disposes the current terminal instance.
    /// </summary>
    public void Dispose()
    {
        _screen.Dispose();

        // Kill off the terminal.
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Starts the building process of the terminal.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <returns>A new terminal builder.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="cursesProvider" /> is <c>null</c>.</exception>
    public static TerminalBuilder UsingCurses(ICursesProvider cursesProvider)
    {
        if (cursesProvider == null)
        {
            throw new ArgumentNullException(nameof(cursesProvider));
        }

        return new(cursesProvider);
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
        AssertNotDisposed();

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
    public void AssertNotDisposed()
    {
        if (Curses.isendwin())
        {
            throw new ObjectDisposedException("The terminal has been disposed and no further operations are allowed.");
        }
    }

    /// <summary>
    ///     Disposes of any unmanaged resources.
    /// </summary>
    private void ReleaseUnmanagedResources()
    {
        if (!IsDisposed)
        {
            if (_oldMouseMask != null)
            {
                Curses.mousemask(_oldMouseMask.Value, out var _);
            }

            if (_initialHardwareCursorMode != null)
            {
                Curses.curs_set(_initialHardwareCursorMode.Value);
            }

            Curses.endwin();
        }

        _terminalInstanceActive = false;
    }

    /// <summary>
    ///     The destructor. Calls <see cref="ReleaseUnmanagedResources" />
    /// </summary>
    ~Terminal() { ReleaseUnmanagedResources(); }
}
