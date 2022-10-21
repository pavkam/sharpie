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
    /// The default is <c>false</c> as that is used in most applications.
    /// </remarks>
    /// <param name="enabled">The value of the flag.</param>
    /// <returns>The same builder instance.</returns>
    public TerminalBuilder WithLineBuffering(bool enabled)
    {
        _enableLineBuffering = enabled;
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
    /// Creates a new instance of the <see cref="Terminal"/> class.
    /// </summary>
    /// <returns>A new terminal object.</returns>
    public Terminal Create() { return new(_cursesProvider, _enableLineBuffering, _enableInputEchoing); }
}
