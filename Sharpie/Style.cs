namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
///     Represents the style of the text (attributes and colors).
/// </summary>
[PublicAPI]
public struct Style
{
    /// <summary>
    ///     The default terminal style.
    /// </summary>
    public static Style Default { get; } =
        new() { Attributes = VideoAttribute.None, ColorMixture = ColorMixture.Default };

    /// <summary>
    ///     The attributes of the text.
    /// </summary>
    public VideoAttribute Attributes { get; init; }

    /// <summary>
    ///     The color mixture.
    /// </summary>
    public ColorMixture ColorMixture { get; init; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() => $"Style [{Attributes}; {ColorMixture}]";

    /// <summary>
    ///     The equality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator ==(Style left, Style right) => left.Equals(right);

    /// <summary>
    ///     The inequality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator !=(Style left, Style right) => !(left == right);

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is Style s && s.Attributes == Attributes && s.ColorMixture == ColorMixture;

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Attributes, ColorMixture);
}
