namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain quarter-block drawing characters.
/// </summary>
[PublicAPI]
public static class Block
{
    /// <summary>
    ///     Defines a fill quadrant that can be combined together to define the final character.
    /// </summary>
    [PublicAPI, Flags]
    public enum Quadrant
    {
        /// <summary>
        ///     Nothing filled.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        TopLeft = 1 << 0,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        TopRight = 1 << 1,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        BottomLeft = 1 << 2,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        BottomRight = 1 << 3
    }

    private static readonly Dictionary<Quadrant, Rune> QuadrantChars = new()
    {
        { Quadrant.None, new(' ') },
        { Quadrant.TopLeft, new('▘') },
        { Quadrant.TopRight, new('▝') },
        { Quadrant.BottomLeft, new('▖') },
        { Quadrant.BottomRight, new('▗') },
        { Quadrant.TopLeft | Quadrant.TopRight, new('▀') },
        { Quadrant.BottomLeft | Quadrant.BottomRight, new('▄') },
        { Quadrant.TopLeft | Quadrant.BottomLeft, new('▌') },
        { Quadrant.TopRight | Quadrant.BottomRight, new('▐') },
        { Quadrant.TopLeft | Quadrant.BottomRight, new('▚') },
        { Quadrant.TopRight | Quadrant.BottomLeft, new('▞') },
        { Quadrant.TopLeft | Quadrant.BottomLeft | Quadrant.TopRight, new('▛') },
        { Quadrant.TopLeft | Quadrant.TopRight | Quadrant.BottomRight, new('▜') },
        { Quadrant.TopLeft | Quadrant.BottomLeft | Quadrant.BottomRight, new('▙') },
        { Quadrant.TopRight | Quadrant.BottomLeft | Quadrant.BottomRight, new('▟') }
    };

    /// <summary>
    ///     Gets the character representing the block with the selected filled <paramref name="quadrants" />.
    /// </summary>
    /// <param name="quadrants">The filled quadrants.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="quadrants" /> contains an invalid value.</exception>
    public static Rune Get(Quadrant quadrants)
    {
        if (QuadrantChars.TryGetValue(quadrants, out var r))
        {
            return r;
        }

        throw new ArgumentException("Invalid quadrant combination.", nameof(quadrants));
    }
}
