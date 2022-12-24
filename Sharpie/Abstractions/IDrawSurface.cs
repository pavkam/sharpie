namespace Sharpie.Abstractions;

/// <summary>
///     Defines the traits needed by an object to draw a <see cref="IDrawable" /> onto.
/// </summary>
public interface IDrawSurface
{
    /// <summary>
    ///     The total size of the draw surface.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    ///     Draws a <paramref name="rune" /> at a <paramref name="location" /> using the given style.
    /// </summary>
    /// <param name="location">The location to draw to.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="style">The cell style.</param>
    void DrawCell(Point location, Rune rune, Style style);
}

