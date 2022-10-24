namespace Sharpie;

/// <summary>
/// Defines a color pair (foreground and background colors).
/// </summary>
public struct ColorMixture
{
    /// <summary>
    /// The default color mixture of the terminal. Use it to reset to default colors.
    /// </summary>
    public static readonly ColorMixture Default = new() { Handle = 0 };

    /// <summary>
    /// The handle of the color pair.
    /// </summary>
    internal ushort Handle { get; init; }
}
