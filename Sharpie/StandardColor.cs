namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
///     Defines the well-known standard video colors.
/// </summary>
[PublicAPI]
public enum StandardColor: short
{
    /// <summary>
    ///     The default color (fg or bg).
    /// </summary>
    Default = -1,

    /// <summary>
    ///     The black color.
    /// </summary>
    Black = 0,

    /// <summary>
    ///     The red color.
    /// </summary>
    Red = 1,

    /// <summary>
    ///     The green color.
    /// </summary>
    Green = 2,

    /// <summary>
    ///     The yellow color.
    /// </summary>
    Yellow = 3,

    /// <summary>
    ///     The blue color.
    /// </summary>
    Blue = 4,

    /// <summary>
    ///     The magenta color.
    /// </summary>
    Magenta = 5,

    /// <summary>
    ///     The cyan color.
    /// </summary>
    Cyan = 6,

    /// <summary>
    ///     The white color.
    /// </summary>
    White = 7
}
