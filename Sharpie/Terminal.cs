namespace Sharpie;

using Curses;
using JetBrains.Annotations;

/// <summary>
/// This class allows the developer to interact with the terminal and its settings. This is the main
/// class that is used
/// </summary>
[PublicAPI]
public sealed class Terminal: IDisposable
{
    private bool _enableLineBuffering;
    private int _readTimeoutMillis;
    private bool _enableInputEchoing;
    private bool _enableReturnToNewLineTranslation;
    private bool _enableForceInterruptingFlush;
    private bool _enableProcessingKeypadKeys;
    private bool _enableColors;
    private CaretMode _hardwareCursorMode;
    private ICursesProvider _cursesProvider;
    private Screen _screen;
    private ColorManager _colorManager;

    internal Terminal(ICursesProvider cursesProvider, bool enableLineBuffering, bool enableReturnToNewLineTranslation,
        int readTimeoutMillis, bool enableInputEchoing, bool enableForceInterruptingFlush,
        bool enableProcessingKeypadKeys, bool enableColors, CaretMode hardwareCursorMode)
    {
        _cursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));

        /* Set configuration. */
        EnableLineBuffering = enableLineBuffering;
        EnableInputEchoing = enableInputEchoing;
        ReadTimeoutMillis = readTimeoutMillis;
        EnableReturnToNewLineTranslation = enableReturnToNewLineTranslation;
        EnableForceInterruptingFlush = enableForceInterruptingFlush;
        CaretMode = hardwareCursorMode;

        /* Other configuration */
        _cursesProvider.meta(IntPtr.Zero, true);

        /* Initialize the screen and other objects */
        _colorManager = new(_cursesProvider, enableColors);
        _screen = new(_cursesProvider, _cursesProvider.initscr())
        {
            EnableProcessingKeypadKeys = enableProcessingKeypadKeys
        };
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
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.baudrate()
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
            _cursesProvider.AssertNotDisposed();

            return _colorManager;
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
            _cursesProvider.AssertNotDisposed();

            return _enableLineBuffering;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            if (value)
            {
                _cursesProvider.nocbreak()
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
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.termname();
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
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.curses_version();
        }
    }

    private void UpdateNonLineBufferedMode()
    {
        if (_readTimeoutMillis == Timeout.Infinite)
        {
            _cursesProvider.cbreak().TreatError();
        } else
        {
            _cursesProvider.halfdelay(Helpers.ConvertMillisToTenths(_readTimeoutMillis)).TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the timeout used when reading characters in non-buffered mode.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public int ReadTimeoutMillis
    {
        get
        {
            _cursesProvider.AssertNotDisposed();

            return _readTimeoutMillis;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

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
            _cursesProvider.AssertNotDisposed();

            return _enableInputEchoing;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            if (value)
            {
                _cursesProvider.echo().TreatError();
            } else
            {
                _cursesProvider.noecho().TreatError();
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
            _cursesProvider.AssertNotDisposed();

            return _enableReturnToNewLineTranslation;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            if (value)
            {
                _cursesProvider.nl().TreatError();
            } else
            {
                _cursesProvider.nonl().TreatError();
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
            _cursesProvider.AssertNotDisposed();

            return _enableForceInterruptingFlush;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.intrflush(IntPtr.Zero, value).TreatError();
            _enableForceInterruptingFlush = value;
        }
    }

    /// <summary>
    /// Enables or disables the processing of keypad keys.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool EnableProcessingKeypadKeys
    {
        get => Screen.EnableProcessingKeypadKeys;
        set => Screen.EnableProcessingKeypadKeys = value;
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
            _cursesProvider.AssertNotDisposed();

            return _hardwareCursorMode;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.curs_set((int)value).TreatError();
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
            _cursesProvider.AssertNotDisposed();

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
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.has_il();
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
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.has_ic();
        }
    }

    /// <summary>
    /// Specifies whether the terminal has been disposed of and is no longer usable.
    /// </summary>
    public bool IsDisposed => _cursesProvider.isendwin();

    /// <summary>
    /// Attempts to notify the user with audio or flashing alert.
    /// </summary>
    /// <remarks>The actual notification depends on terminal support.</remarks>
    /// <param name="silent">The alert mode.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Alert(bool silent)
    {
        _cursesProvider.AssertNotDisposed();

        if (silent)
        {
            _cursesProvider.flash().TreatError();
        } else
        {
            _cursesProvider.beep().TreatError();
        }
    }

    public void Dispose()
    {
        if (!_cursesProvider.isendwin())
        {
            _cursesProvider.endwin();
        }
    }
}
