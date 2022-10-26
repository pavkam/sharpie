namespace Sharpie;

/// <summary>
/// Defines the possible event types.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Undefined event.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The terminal has been resized.
    /// </summary>
    ResizeTerminal,

    /// <summary>
    /// A key has been pressed.
    /// </summary>
    KeyPress,

    /// <summary>
    /// The mouse has moved.
    /// </summary>
    MouseMove,

    /// <summary>
    /// The mouse buttons have been used.
    /// </summary>
    MouseClick,
}
