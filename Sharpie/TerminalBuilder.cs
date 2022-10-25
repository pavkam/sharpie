namespace Sharpie;

using Curses;
using JetBrains.Annotations;

/// <summary>
/// This class allows building instances of the <see cref="Terminal"/> class.
/// </summary>
[PublicAPI]
public sealed class TerminalBuilder
{
    private readonly ICursesProvider _cursesProvider;
    private bool _enableLineBuffering;
    private bool _enableInputEchoing;
    private int _readTimeoutMillis = Timeout.Infinite;
    private bool _enableForceInterruptingFlush;
    private bool _enableColors = true;
    private bool _enableReturnToNewLineTranslation;
    private bool _useEnvironmentOverrides = true;
    private CaretMode _hardwareCursorMode = CaretMode.Visible;
    private int _escapeDelayMillis = 1000;
    private SoftKeyLabelMode _softKeyLabelMode = SoftKeyLabelMode.Disabled;

    /// <summary>
    /// Creates a new instance of the terminal builder using a given Curses provider.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="cursesProvider"/> is <c>null</c>.</exception>
    internal TerminalBuilder(ICursesProvider cursesProvider) =>
        _cursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));

    /// <summary>
    /// Toggles line buffering on or off.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c> as that is used in most applications. The <paramref name="readTimeoutMillis"/> is used in
    /// non-buffered mode, and represents the time to wait until any read operation is interrupted if not input has been
    /// supplied by the user.
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <param name="readTimeoutMillis">If the <paramref name="enabled"/> is <c>false</c>, the read timeout.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithLineBuffering(bool enabled, int readTimeoutMillis = Timeout.Infinite)
    {
        _enableLineBuffering = enabled;
        _readTimeoutMillis = readTimeoutMillis;
        return this;
    }

    /// <summary>
    /// Toggles new line translation on or off.
    /// </summary>
    /// <remarks>
    /// If enabled, the return keys are translated into new line characters.
    /// The default is <c>false</c>.
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithReturnToNewLineTranslation(bool enabled)
    {
        _enableReturnToNewLineTranslation = enabled;
        return this;
    }

    /// <summary>
    /// Toggles input echoing on or off.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c> as that is used in most applications TUI applications that
    /// want to deal with input display by themselves.
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithInputEchoing(bool enabled)
    {
        _enableInputEchoing = enabled;
        return this;
    }

    /// <summary>
    /// Toggle flush interrupting on or off.
    /// </summary>
    /// <remarks>
    /// If set, the console flush is discarded mid-way when an application interrupt occurs.
    /// Default is <c>false</c>
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithForceInterruptingFlush(bool enabled)
    {
        _enableForceInterruptingFlush = enabled;
        return this;
    }

    /// <summary>
    /// Toggle the use of colors in the terminal (if supported).
    /// </summary>
    /// <remarks>
    /// Default is <c>true</c>.
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithColors(bool enabled)
    {
        _enableColors = enabled;
        return this;
    }

    /// <summary>
    /// Toggle the use of environment overrides for lines and columns.
    /// </summary>
    /// <remarks>
    /// Default is <c>true</c>
    /// </remarks>
    /// <param name="use"><c>true</c> if the environment overrides are to be used.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithEnvironmentOverrides(bool use)
    {
        _useEnvironmentOverrides = use;
        return this;
    }

    /// <summary>
    /// Toggle the use of the hardware caret.
    /// </summary>
    /// <remarks>
    /// Default is <see cref="CaretMode.Visible"/>
    /// </remarks>
    /// <param name="mode">The caret mode.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithCaret(CaretMode mode)
    {
        _hardwareCursorMode = mode;
        return this;
    }

    /// <summary>
    /// Sets the escape sequence wait timeout.
    /// </summary>
    /// <remarks>
    /// Default is 1 second.
    /// </remarks>
    /// <param name="delayMillis">The delay in milliseconds.</param>
    /// <returns>The same builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="delayMillis"/> is negative.</exception>
    public TerminalBuilder WithEscapeSequenceWaitDelay(int delayMillis)
    {
        if (delayMillis < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMillis));
        }

        _escapeDelayMillis = delayMillis;
        return this;
    }

    /// <summary>
    /// Sets the soft key labels functionality mode.
    /// </summary>
    /// <remarks>
    /// Default is <see cref="SoftKeyLabelMode.Disabled"/>.
    /// </remarks>
    /// <param name="mode">The mode of the soft label keys used.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithSoftKeyLabels(SoftKeyLabelMode mode)
    {
        _softKeyLabelMode = mode;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Terminal"/> class.
    /// </summary>
    /// <returns>A new terminal object.</returns>
    public Terminal Create() =>
        new(_cursesProvider, _enableLineBuffering, _enableReturnToNewLineTranslation, _readTimeoutMillis,
            _enableInputEchoing, _enableForceInterruptingFlush, _enableColors,
            _hardwareCursorMode, _useEnvironmentOverrides, _escapeDelayMillis, _softKeyLabelMode);
}
