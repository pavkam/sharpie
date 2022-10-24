namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Represents the style of the text (attributes and colors).
/// </summary>
[PublicAPI]
public struct Style
{
    /// <summary>
    /// The default terminal style.
    /// </summary>
    public static Style Default = new() { Attributes = VideoAttribute.None, ColorMixture = ColorMixture.Default, };

    /// <summary>
    /// The attributes of the text.
    /// </summary>
    public VideoAttribute Attributes { get; init; }

    /// <summary>
    /// The color mixture.
    /// </summary>
    public ColorMixture ColorMixture { get; init; }
}
