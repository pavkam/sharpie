namespace Sharpie;

/// <summary>
/// Defines the possible strategies of clearing contents of a window.
/// </summary>
public enum SoftKeyLabelMode
{
    /// <summary>
    /// No soft key labels will be used.
    /// </summary>
    Disabled = -1,

    /// <summary>
    /// Indicates a 3-2-3 arrangement of the labels.
    /// </summary>
    ThreeTwoThree = 0,

    /// <summary>
    /// Indicates a 4-4 arrangement of the labels.
    /// </summary>
    FourFour = 1,

    /// <summary>
    /// Indicates a 4-4-4 arrangement of the labels.
    /// </summary>
    FourFourFour = 2,

    /// <summary>
    /// Indicates a 4-4-4 arrangement of the labels with an index line.
    /// </summary>
    FourFourFourWithIndex = 3,
}
