namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the possible mouse button states.
/// </summary>
[PublicAPI]
public enum MouseButtonState: uint
{
    /// <summary>
    /// Unknown state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The button has been released.
    /// </summary>
    Released,

    /// <summary>
    /// The button has been pressed.
    /// </summary>
    Pressed,

    /// <summary>
    /// The button has been clicked.
    /// </summary>
    Clicked,

    /// <summary>
    /// The button has been double clicked.
    /// </summary>
    DoubleClicked,

    /// <summary>
    /// The button has been triple clicked.
    /// </summary>
    TripleClicked,
}
