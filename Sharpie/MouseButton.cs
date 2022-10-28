namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
///     Defines the possible mouse buttons.
/// </summary>
[PublicAPI]
public enum MouseButton: uint
{
    /// <summary>
    ///     Unknown button.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The button 1.
    /// </summary>
    Button1,

    /// <summary>
    ///     The button 2.
    /// </summary>
    Button2,

    /// <summary>
    ///     The button 3.
    /// </summary>
    Button3,

    /// <summary>
    ///     The button 4.
    /// </summary>
    Button4,

    /// <summary>
    ///     The button 5.
    /// </summary>
    Button5
}
