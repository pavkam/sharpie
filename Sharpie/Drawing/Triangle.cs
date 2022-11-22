namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain triangle drawing characters.
/// </summary>
[PublicAPI]
public static class Triangle
{
    /// <summary>
    ///     Describes the direction of triangle.
    /// </summary>
    [PublicAPI]
    public enum Direction
    {
        /// <summary>
        ///     Towards up.
        /// </summary>
        Up,

        /// <summary>
        ///     Towards down.
        /// </summary>
        Down,

        /// <summary>
        ///     Towards left.
        /// </summary>
        Left,

        /// <summary>
        ///     Towards right.
        /// </summary>
        Right
    }

    /// <summary>
    ///     Describes the fill of the triangle.
    /// </summary>
    [PublicAPI]
    public enum Fill
    {
        /// <summary>
        ///     Normal fill.
        /// </summary>
        Normal,

        /// <summary>
        ///     Black fill.
        /// </summary>
        Black
    }

    /// <summary>
    ///     Describes the size of the triangle.
    /// </summary>
    [PublicAPI]
    public enum Size
    {
        /// <summary>
        ///     Normal size.
        /// </summary>
        Normal,

        /// <summary>
        ///     Small size.
        /// </summary>
        Small
    }

    private static readonly Rune[] Triangles = "▲△▴▵▼▽▾▿◀◁◂◃▶▷▸▹".Select(c => new Rune(c))
                                                                 .ToArray();

    /// <summary>
    ///     Gets the rectangle given the supplied style.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <param name="size">The size.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="direction" />, <paramref name="size" /> or
    ///     <paramref name="fill" /> have invalid values.
    /// </exception>
    public static Rune Get(Direction direction, Size size, Fill fill)
    {
        var index = (int) direction * 4 + (int) size * 2 + (int) fill;
        if (index < 0 || index >= Triangles.Length)
        {
            throw new ArgumentException("Invalid parameter combination");
        }

        return Triangles[(int) direction * 4 + (int) size * 2 + (int) fill];
    }
}
