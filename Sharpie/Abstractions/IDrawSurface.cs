namespace Sharpie.Abstractions;

/// <summary>
///     Defines the traits needed by an object to draw a <see cref="Drawing" /> onto.
/// </summary>
public interface IDrawSurface
{
    /// <summary>
    ///     Draws a <paramref name="rune" /> at a <paramref name="location" /> using the given style.
    /// </summary>
    /// <param name="location">The location to draw to.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="textStyle">The text style.</param>
    void DrawCell(Point location, Rune rune, Style textStyle);

    /// <summary>
    ///     Checks if a given <paramref name="area" /> is within the drawing surface.
    /// </summary>
    /// <param name="area">The area to check.</param>
    /// <returns>The result of the check.</returns>
    bool CoversArea(Rectangle area);
}
