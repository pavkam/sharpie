namespace Sharpie.Abstractions;

/// <summary>
///     Defines the trains required for an object to be drawable onto a <see cref="IDrawSurface" />.
/// </summary>
[PublicAPI]
public interface IDrawable
{
    /// <summary>
    ///     The size of the drawable.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    ///     Draws the drawing onto a given surface.
    /// </summary>
    /// <param name="destination">The surface to draw on.</param>
    /// <param name="srcArea">The source area to draw.</param>
    /// <param name="destLocation">The destination location.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="srcArea" /> or <paramref name="destLocation" />
    ///     are invalid.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="destination" /> is <c>null</c>.</exception>
    void DrawTo(IDrawSurface destination, Rectangle srcArea, Point destLocation);
}
