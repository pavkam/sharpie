namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines a color pair (foreground and background colors).
/// </summary>
[PublicAPI]
public struct ColorMixture
{
    /// <summary>
    /// The default color mixture of the terminal. Use it to reset to default colors.
    /// </summary>
    public static ColorMixture Default { get; } = new() { Handle = 0 };

    /// <summary>
    /// The handle of the color pair.
    /// </summary>
    internal ushort Handle { get; init; }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"Mixture [{Handle}]";

    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? obj) => obj is ColorMixture cm && cm.Handle == Handle;

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => Handle.GetHashCode();

    /// <summary>
    /// The equality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator ==(ColorMixture left, ColorMixture right) => left.Equals(right);

    /// <summary>
    /// The inequality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator !=(ColorMixture left, ColorMixture right) => !(left == right);
}
