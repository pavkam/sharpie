namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the alignment of labels in a <see cref="SoftKeyLabelManager"/>.
/// </summary>
[PublicAPI]
public enum SoftKeyLabelAlignment
{
    /// <summary>
    /// The label is left-aligned.
    /// </summary>
    Left = 0,

    /// <summary>
    /// The label is center-aligned.
    /// </summary>
    Center = 1,

    /// <summary>
    /// The label is right-aligned.
    /// </summary>
    Right = 2,
}
