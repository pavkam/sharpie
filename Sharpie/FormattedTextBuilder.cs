namespace Sharpie;

using System.Globalization;
using Curses;

/// <summary>
/// Allows building formatted text instances. The <see cref="FormattedText"/> is the core way of outputting
/// text in teh terminal.
/// </summary>
public sealed class FormattedTextBuilder
{
    private readonly ICursesProvider _curses;
    private readonly List<CChar> _characters = new();

    /// <summary>ß
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="curses">The curses provider.</param>
    /// <exception cref="ArgumentNullException">If the <paramref name="curses"/> is <c>null</c>.</exception>
    public FormattedTextBuilder(ICursesProvider curses) =>
        _curses = curses ?? throw new ArgumentNullException(nameof(curses));

    /// <summary>
    /// Adds a new string with the given colors and attributes.
    /// </summary>
    /// <param name="value">The string to add.</param>
    /// <param name="style">The style to use.</param>
    /// <returns>The same builder instance.</returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="value"/> is <c>null</c>.</exception>
    public FormattedTextBuilder Add(string value, Style style)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var enumerator = StringInfo.GetTextElementEnumerator(value);
        while (enumerator.MoveNext())
        {
            _curses.setcchar(out var @char, enumerator.GetTextElement(), (uint) style.Attributes,
                       style.ColorMixture.Handle, IntPtr.Zero)
                   .TreatError();

            _characters.Add(@char);
        }

        return this;
    }

    /// <summary>
    /// Adds a new string with the given attributes and default colors.
    /// </summary>
    /// <param name="value">The string to add.</param>
    /// <param name="mixture">The color mixture.</param>
    /// <returns>The same builder instance.</returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="value"/> is <c>null</c>.</exception>
    public FormattedTextBuilder Add(string value, ColorMixture mixture) =>
        Add(value, new Style { ColorMixture = mixture, Attributes = VideoAttribute.None });

    /// <summary>
    /// Adds a new string with the given colors and not attributes.
    /// </summary>
    /// <param name="value">The string to add.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>The same builder instance.</returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="value"/> is <c>null</c>.</exception>
    public FormattedTextBuilder Add(string value, VideoAttribute attributes) =>
        Add(value, new Style { ColorMixture = ColorMixture.Default, Attributes = attributes });

    /// <summary>
    /// Adds a new string with default colors and no attributes.
    /// </summary>
    /// <param name="value">The string to add.</param>
    /// <returns>The same builder instance.</returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="value"/> is <c>null</c>.</exception>
    public FormattedTextBuilder Add(string value) =>
        Add(value, new Style { ColorMixture = ColorMixture.Default, Attributes = VideoAttribute.None });

    /// <summary>
    /// Builds a new <see cref="FormattedText"/> instance.
    /// </summary>
    /// <returns>A new formatted text.</returns>
    public FormattedText Build() => new(_characters.ToArray());
}
