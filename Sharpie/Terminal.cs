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
    private bool _enableInputEchoing;
    private ICursesProvider _cursesProvider;

    internal Terminal(
        ICursesProvider cursesProvider,
        bool enableLineBuffering,
        bool enableInputEchoing)
    {
        _cursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));

        /* Set configuration. */
        EnableLineBuffering = enableLineBuffering;
        EnableInputEchoing = enableInputEchoing;

        /* Initialize the screen */
        Screen = new Screen(_cursesProvider.initscr());
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

        return new TerminalBuilder(cursesProvider);
    }

    /// <summary>
    /// Gets the terminal's baud rate.
    /// </summary>
    public int BaudRate => _cursesProvider.baudrate();

    /// <summary>
    /// Specifies whether the terminal supports redefining colors.
    /// </summary>
    public bool SupportCustomColors => _cursesProvider.can_change_color();

    /// <summary>
    /// Toggles line buffering on or off.
    /// </summary>
    public bool EnableLineBuffering
    {
        get => _enableLineBuffering;
        set
        {
            if (value)
            {
                _cursesProvider.nocbreak();
            }
            else
            {
                _cursesProvider.cbreak();
            }

            _enableLineBuffering = value;
        }
    }

    /// <summary>
    /// Toggles input echoing on or off.
    /// </summary>
    public bool EnableInputEchoing
    {
        get => _enableInputEchoing;
        set
        {
            if (value)
            {
                _cursesProvider.echo();
            }
            else
            {
                _cursesProvider.noecho();
            }

            _enableInputEchoing = value;
        }
    }

    /// <summary>
    /// The screen instance. Use this property to access the entire screen functionality.
    /// </summary>
    public Screen Screen { get; }

    /// <summary>
    /// Attempts to notify the user with audio or flashing alert.
    /// </summary>
    /// <remarks>The actual notification depends on terminal support.</remarks>
    /// <param name="silent">The alert mode.</param>
    public void Alert(bool silent)
    {
        if (silent)
        {
            _cursesProvider.flash();
        } else
        {
            _cursesProvider.beep();
        }
    }

    public void Dispose()
    {
        EnableLineBuffering = true;
    }
}
