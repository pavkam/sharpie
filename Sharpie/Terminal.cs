namespace Sharpie;

using System.Diagnostics;
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
    private bool _enableLineBuffering;
    private int _readTimeoutMillis;
    private bool _enableInputEchoing;
    private bool _enableReturnToNewLineTranslation;
    private bool _enableForceInterruptingFlush;
    private bool _enableProcessingKeypadKeys;
    private bool _enableColors;
    private bool _useEnvironmentOverrides;
    private CaretMode _hardwareCursorMode;
    private Screen _screen;
    private ColorManager _colorManager;
    private IList<Window> _windows;
    private SoftKeyLabelManager _softKeyLabelManager;

    internal Terminal(ICursesProvider cursesProvider, bool enableLineBuffering, bool enableReturnToNewLineTranslation,
        int readTimeoutMillis, bool enableInputEchoing, bool enableForceInterruptingFlush,
        bool enableColors, CaretMode hardwareCursorMode, bool useEnvironmentOverrides,
        int escapeDelayMillis, SoftKeyLabelMode softKeyLabelMode)
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
        EnableInputEchoing = enableInputEchoing;
        _readTimeoutMillis = readTimeoutMillis;
        EnableLineBuffering = enableLineBuffering;
        EnableReturnToNewLineTranslation = enableReturnToNewLineTranslation;
        EnableForceInterruptingFlush = enableForceInterruptingFlush;
        CaretMode = hardwareCursorMode;
        EscapeSequenceWaitDelay = escapeDelayMillis;

        /* Other configuration */
        Curses.meta(IntPtr.Zero, true);


        _windows = new List<Window> { _screen };
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
                                  .TreatError();
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
    public bool EnableLineBuffering
    {
        get
        {
            AssertNotDisposed();

            return _enableLineBuffering;
        }
        set
        {
            AssertNotDisposed();

            if (value)
            {
                Curses.nocbreak()
                               .TreatError();
            } else
            {
                UpdateNonLineBufferedMode();
            }

            _enableLineBuffering = value;
        }
    }

    /// <summary>
    /// Returns the name of the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public string Name
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
    public string Description
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
    public string CursesVersion
    {
        get
        {
            AssertNotDisposed();

            return Curses.curses_version();
        }
    }

    private void UpdateNonLineBufferedMode()
    {
        if (_readTimeoutMillis == Timeout.Infinite)
        {
            Curses.cbreak().TreatError();
        } else
        {
            Curses.halfdelay(Helpers.ConvertMillisToTenths(_readTimeoutMillis)).TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the timeout used when reading characters in non-buffered mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is less than one.</exception>
    public int ReadTimeoutMillis
    {
        get
        {
            AssertNotDisposed();

            return _readTimeoutMillis;
        }
        set
        {
            AssertNotDisposed();

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (!EnableLineBuffering)
            {
                UpdateNonLineBufferedMode();
            }

            _readTimeoutMillis = value;
        }
    }

    /// <summary>
    /// Enables or disables the character echo mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableInputEchoing
    {
        get
        {
            AssertNotDisposed();

            return _enableInputEchoing;
        }
        set
        {
            AssertNotDisposed();

            if (value)
            {
                Curses.echo().TreatError();
            } else
            {
                Curses.noecho().TreatError();
            }

            _enableInputEchoing = value;
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

            return _enableReturnToNewLineTranslation;
        }
        set
        {
            AssertNotDisposed();

            if (value)
            {
                Curses.nl().TreatError();
            } else
            {
                Curses.nonl().TreatError();
            }

            _enableReturnToNewLineTranslation = value;
        }
    }

    /// <summary>
    /// Enables or disables flush interruption during application breaks.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableForceInterruptingFlush
    {
        get
        {
            AssertNotDisposed();

            return _enableForceInterruptingFlush;
        }
        set
        {
            AssertNotDisposed();

            Curses.intrflush(IntPtr.Zero, value).TreatError();
            _enableForceInterruptingFlush = value;
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

            Curses.curs_set((int)value).TreatError();
            _hardwareCursorMode = value;
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

            return
                new(
                    Curses.killwchar(out var @char) != Helpers.CursesErrorResult ? @char : '\0');
        }
    }

    /// <summary>
    /// Gets or sets the escape sequence wait delay.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is negative.</exception>
    public int EscapeSequenceWaitDelay
    {
        get
        {
            AssertNotDisposed();

            return Curses.get_escdelay();
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            AssertNotDisposed();
            Curses.set_escdelay(value)
                  .TreatError();
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

            return new(
                Curses.erasewchar(out var @char) != Helpers.CursesErrorResult ? @char : '\0');
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
            Curses.flash().TreatError();
        } else
        {
            Curses.beep().TreatError();
        }
    }

    /// <summary>
    /// Lists the active windows in this terminal.
    /// </summary>
    public IEnumerable<Window> Windows => _windows;

    /// <summary>
    /// Utility method that removes a window from the managed windows list. Only used internally.
    /// </summary>
    /// <param name="window">The window to remove.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="window"/> is <c>null</c>.</exception>
    internal void RemoveWindow(Window window)
    {
        var removed = _windows.Remove(window);
        Debug.Assert(removed);
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
            Curses.endwin(); // Ignore potential error.
        }

        _terminalInstanceActive = false;
    }

    /// <summary>
    /// Disposes the current terminal instance.
    /// </summary>
    public void Dispose()
    {
        // Dispose of all the windows
        var windows = _windows.ToArray();
        foreach (var window in windows)
        {
            window.Dispose();
        }

        // Kill off the terminal.
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The destructor. Calls <see cref="ReleaseUnmanagedResources"/>
    /// </summary>
    ~Terminal() {
        ReleaseUnmanagedResources();
    }
}
