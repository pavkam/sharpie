namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain characters that act as gradients fills.
/// </summary>
[PublicAPI]
public static class Gradient
{
    /// <summary>
    ///     The gradient style.
    /// </summary>
    [PublicAPI]
    public enum Style
    {
        /// <summary>
        ///     Bottom-to-top fill.
        /// </summary>
        BottomToTop,

        /// <summary>
        ///     Left-to-right fill.
        /// </summary>
        LeftToRight
    }

    private static readonly Rune[] HorizontalBlocks = " ▁▂▃▄▅▆▇█".Select(c => new Rune(c))
                                                                 .ToArray();

    private static readonly Rune[] VerticalBlocks = " ▏▎▍▌▋▊▉█".Select(c => new Rune(c))
                                                               .ToArray();

    /// <summary>
    ///     Obtains a line filled with characters selected from <paramref name="options" /> based on the
    ///     <paramref name="percent" /> of them being filled.
    /// </summary>
    /// <param name="cells">The count of cells to fill.</param>
    /// <param name="options">The character options to fill the cells.</param>
    /// <param name="percent">The percent of the cell to fill.</param>
    /// <returns>The characters that make up the gradient.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="percent" /> is negative or greater than
    ///     <c>1</c> or <paramref name="cells" /> is less than <c>1</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options" /> is <c>null</c>.</exception>
    internal static IReadOnlyList<Rune> Fill(int cells, IReadOnlyList<Rune> options, double percent)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var (fullCellCount, charIndex) = Helpers.CalculateOptionDistribution(cells, options.Count, percent);
        var result = new Rune[cells];

        Array.Fill(result, options.Last(), 0, fullCellCount);
        if (fullCellCount < cells - 1)
        {
            Array.Fill(result, options.First(), fullCellCount + 1, cells - fullCellCount);
        }

        result[fullCellCount] = VerticalBlocks[charIndex];
        return result;
    }

    /// <summary>
    ///     Obtains a gradient line that fills the cells from the start to the finish with a given <paramref name="percent" />.
    /// </summary>
    /// <param name="cells">The count of cells to fill.</param>
    /// <param name="style">The gradient style.</param>
    /// <param name="percent">The percent of gradient that is filled.</param>
    /// <returns>The characters that make up the gradient.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="percent" /> is negative or greater than
    ///     <c>1</c> or <paramref name="cells" /> is less than <c>1</c>.
    /// </exception>
    public static IReadOnlyList<Rune> Get(int cells, Style style, double percent) =>
        Fill(cells, style == Style.LeftToRight ? VerticalBlocks : HorizontalBlocks, percent);
}
