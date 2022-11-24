namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain check characters (circles and etc).
/// </summary>
[PublicAPI]
public static class Check
{
    /// <summary>
    ///     The types fills.
    /// </summary>
    [PublicAPI]
    public enum Fill
    {
        /// <summary>
        ///     Normal, empty.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Filled.
        /// </summary>
        Black
    }

    /// <summary>
    ///     The possible shapes used by the check.
    /// </summary>
    [PublicAPI]
    public enum Style
    {
        /// <summary>
        ///     A circle.
        /// </summary>
        Circle = 0,

        /// <summary>
        ///     A diamond.
        /// </summary>
        Diamond,

        /// <summary>
        ///     A square.
        /// </summary>
        Square
    }

    private static readonly Rune[] Checks = "◯●◇◆□■".Select(c => new Rune(c))
                                                    .ToArray();

    /// <summary>
    ///     Gets a character that represents the desired check style and fill.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="style" /> or <paramref name="fill" /> are invalid.</exception>
    public static Rune Get(Style style, Fill fill)
    {
        var index = (int) style * 2 + (int) fill;
        if (index < 0 || index >= Checks.Length)
        {
            throw new ArgumentException("Invalid style and fill combination.");
        }

        return Checks[index];
    }
}
