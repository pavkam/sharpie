namespace Sharpie;

using System.Globalization;
using Curses;

/// <summary>
/// Contains a formatted text that can be outputted to the console.
/// </summary>
public sealed class FormattedText
{
    internal CChar[] Characters { get; }

    /// <summary>
    /// Creates a new instanceof this class.
    /// </summary>
    /// <param name="characters">The characters provided by the builder.</param>
    /// <exception cref="ArgumentNullException">If the <see cref="characters"/> is <c>null</c>.</exception>
    internal FormattedText(CChar[] characters) => Characters = characters ?? throw new ArgumentNullException(nameof(characters));
}
