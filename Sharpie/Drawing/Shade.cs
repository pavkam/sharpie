namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain characters used when drawing shades.
/// </summary>
[PublicAPI]
public static class Shade
{
    /// <summary>
    ///     The types of possible shades.
    /// </summary>
    [PublicAPI]
    public enum Style
    {
        /// <summary>
        ///     No shade.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Light shade.
        /// </summary>
        Light,

        /// <summary>
        ///     Medium shade.
        /// </summary>
        Medium,

        /// <summary>
        ///     Dark shade.
        /// </summary>
        Dark
    }

    private static readonly Rune[] ShadeBlocks = " ░▒▓".Select(c => new Rune(c))
                                                       .ToArray();

    /// <summary>
    ///     Obtains a shade character for a given <paramref name="style" />.
    /// </summary>
    /// <param name="style">the shade style.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="style" /> is invalid.</exception>
    public static Rune Get(Style style)
    {
        if (style < 0 || (int) style > ShadeBlocks.Length)
        {
            throw new ArgumentException("Invalid style value.", nameof(style));
        }

        return ShadeBlocks[(int) style];
    }
}
