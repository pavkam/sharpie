namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Represents the style of the text (attributes and colors).
/// </summary>
[PublicAPI]
public struct Style
{
    /// <summary>
    /// The attributes of the text.
    /// </summary>
    public VideoAttribute Attributes { get; init; }

    /// <summary>
    /// The color pair of the attribute.
    /// </summary>
    public ColorPair ColorPair { get; init; }
}
