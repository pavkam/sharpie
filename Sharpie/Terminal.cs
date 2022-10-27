namespace Sharpie;

using System.Text;
using Curses;
using JetBrains.Annotations;

/// <summary>
/// This class allows the developer to interact with the terminal and its settings. This is the main
/// class that is used
/// </summary>
[PublicAPI]
public sealed class Terminal: IDisposable
{
    private static bool _terminalInstanceActive;
    private bool _lineBuffering;
    private bool _rawMode;
    private int _readTimeoutMillis;
    private bool _inputEchoing;
    private bool _newLineTranslation;
    private bool _manualFlush;
    private bool _useEnvironmentOverrides;
    private bool _enableMouse;
    private ulong? _oldMouseMask;
    private CaretMode _hardwareCursorMode;
    private int? _initialHardwareCursorMode;
    private Screen _screen;
    private ColorManager _colorManager;

    private SoftKeyLabelManager _softKeyLabelManager;

    internal Terminal(ICursesProvider cursesProvider, bool enableLineBuffering, bool enableRawMode,
        bool enableReturnToNewLineTranslation, int readTimeoutMillis, bool enableInputEchoing, bool manualFlush,
        bool enableColors, CaretMode hardwareCursorMode, bool useEnvironmentOverrides,
        SoftKeyLabelMode softKeyLabelMode, bool enableMouse)
    {
        if (_terminalInstanceActive)
        {
            throw new InvalidOperationException(
                "Another terminal instance is active. Only one instance can be active at one time.");
        }

        Curses = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));

        // Pre-screen creation.
        _useEnvironmentOverrides = useEnvironmentOverrides;
        if (_useEnvironmentOverrides)
        {
            Curses.use_env(true);
        }

        _softKeyLabelManager = new(this, softKeyLabelMode);

        // Screen setup.
        _screen = new(this, Curses.initscr());
        _colorManager = new(this, enableColors);

        // After screen creation.
        _inputEchoing = enableInputEchoing;
        _readTimeoutMillis = readTimeoutMillis;
        _rawMode = enableRawMode;
        _lineBuffering = enableLineBuffering;
        _manualFlush = manualFlush;

        Curses.intrflush(IntPtr.Zero, _manualFlush);
        if (_manualFlush)
        {
            Curses.noqiflush();
        } else
        {
            Curses.qiflush();
        }

        if (_inputEchoing)
        {
            Curses.echo()
                  .Check(nameof(Curses.echo));
        } else
        {
            Curses.noecho()
                  .Check(nameof(Curses.noecho));
        }

        if (!_lineBuffering)
        {
            if (_rawMode)
            {
                Curses.raw()
                      .Check(nameof(Curses.raw));
            } else
            {
                Curses.noraw()
                      .Check(nameof(Curses.noraw));
            }

            if (_readTimeoutMillis != Timeout.Infinite)
            {
                Curses.halfdelay(Helpers.ConvertMillisToTenths(_readTimeoutMillis))
                      .Check(nameof(Curses.halfdelay));
            } else
            {
                Curses.cbreak()
                      .Check(nameof(Curses.cbreak));
            }
        } else
        {
            Curses.nocbreak()
                  .Check(nameof(Curses.nocbreak));
        }

        EnableReturnToNewLineTranslation = enableReturnToNewLineTranslation;
        CaretMode = hardwareCursorMode;
        EnableMouse = enableMouse;

        /* Other configuration */
        Curses.meta(IntPtr.Zero, true);

        _terminalInstanceActive = true;
    }

    /// <summary>
    /// Starts the building process of the terminal.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <returns>A new terminal builder.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="cursesProvider"/> is <c>null</c>.</exception>
    public static TerminalBuilder UsingCurses(ICursesProvider cursesProvider)
    {
        if (cursesProvider == null)
        {
            throw new ArgumentNullException(nameof(cursesProvider));
        }

        return new(cursesProvider);
    }

    /// <summary>
    /// Gets the terminal's baud rate.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public int BaudRate
    {
        get
        {
            AssertNotDisposed();

            return Curses.baudrate()
                         .Check(nameof(Curses.baudrate));
        }
    }

    /// <summary>
    /// Provides access to the terminal's color management.
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
    /// Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public SoftKeyLabelManager SoftKeyLabels
    {
        get
        {
            AssertNotDisposed();

            return _softKeyLabelManager;
        }
    }

    /// <summary>
    /// Enables or disables the line buffering mode.
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
                  .Check(nameof(Curses.mousemask));

            _oldMouseMask ??= oldMask;
        }
    }

    /// <summary>
    /// Gets or sets the interval used to treat pressed/released as clicks.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is negative.</exception>
    public int MouseClickInterval
    {
        get
        {
            AssertNotDisposed();

            return Curses.mouseinterval(-1)
                         .Check(nameof(Curses.mouseinterval));
        }
        set
        {
            AssertNotDisposed();

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.mouseinterval(value)
                  .Check(nameof(Curses.mouseinterval));
        }
    }

    /// <summary>
    /// Checks if the terminal is in line buffering mode.
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
    /// Check if the control keys are silenced (e.g. CTRL+C).
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
    /// Gets timeout used when reading characters in non-buffered mode.
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
    /// Returns the name of the terminal.
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
    /// Returns the long description of the terminal.
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
    /// Returns the version of the Curses library in use.
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
    /// Checks if the input echoing is enabled in this terminal.
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
    /// Enables or disables return to new line character translation.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableReturnToNewLineTranslation
    {
        get
        {
            AssertNotDisposed();

            return _newLineTranslation;
        }
        set
        {
            AssertNotDisposed();

            if (value)
            {
                Curses.nl()
                      .Check(nameof(Curses.nl));
            } else
            {
                Curses.nonl()
                      .Check(nameof(Curses.nonl));
            }

            _newLineTranslation = value;
        }
    }

    /// <summary>
    /// Checks if the application flushes the terminal buffers when control keys are read by the application.
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
    /// Gets or sets the current caret mode of the terminal.
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
                                 .Check(nameof(Curses.curs_set));

            _initialHardwareCursorMode ??= prevMode;
            _hardwareCursorMode = value;

            _screen.IgnoreHardwareCaret = value == CaretMode.Invisible;
        }
    }

    /// <summary>
    /// Gets the combination of supported terminal attributes.
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
    /// The screen instance. Use this property to access the entire screen functionality.
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
    /// Specifies whether the terminal supports hardware line insert and delete.
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
    /// Specifies whether the terminal supports hardware character insert and delete.
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
    /// Specifies whether the environment variables are used to setup the terminal.
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
    /// Gets the currently defined kill character. \0 is returned if none is defined.
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
    /// Gets the currently defined erase character. \0 is returned if none is defined.
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
    /// Attempts to notify the user with audio or flashing alert.
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
                  .Check(nameof(Curses.flash));
        } else
        {
            Curses.beep()
                  .Check(nameof(Curses.beep));
        }
    }

    /// <summary>
    /// The Curses functionality provider.
    /// </summary>
    public ICursesProvider Curses { get; }

    /// <summary>
    /// Validates that the terminal is not disposed.
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
    /// Specifies whether the terminal has been disposed of and is no longer usable.
    /// </summary>
    public bool IsDisposed => Curses.isendwin();

    /// <summary>
    /// Disposes of any unmanaged resources.
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
    /// Disposes the current terminal instance.
    /// </summary>
    public void Dispose()
    {
        _screen.Dispose();

        // Kill off the terminal.
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The destructor. Calls <see cref="ReleaseUnmanagedResources"/>
    /// </summary>
    ~Terminal() { ReleaseUnmanagedResources(); }
}
