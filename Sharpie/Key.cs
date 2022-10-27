namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the possible key types.
/// </summary>
[PublicAPI]
public enum Key: uint
{
    /// <summary>
    /// A simple character key.
    /// </summary>
    Character,

    /// <summary>
    /// Unknown key.
    /// </summary>
    Unknown,

    /// <summary>
    /// The delete key.
    /// </summary>
    Delete,

    /// <summary>
    /// Backspace key.
    /// </summary>
    Backspace,

    /// <summary>
    /// Arrow up key.
    /// </summary>
    KeypadUp,

    /// <summary>
    /// Arrow down key.
    /// </summary>
    KeypadDown,

    /// <summary>
    /// Arrow left key.
    /// </summary>
    KeypadLeft,

    /// <summary>
    /// Arrow right key.
    /// </summary>
    KeypadRight,

    /// <summary>
    /// Page up key.
    /// </summary>
    KeypadPageUp,

    /// <summary>
    /// Page down key.
    /// </summary>
    KeypadPageDown,

    /// <summary>
    /// Home key.
    /// </summary>
    KeypadHome,

    /// <summary>
    /// End key.
    /// </summary>
    KeypadEnd,

    /// <summary>
    /// Delete character key.
    /// </summary>
    DeleteChar,

    /// <summary>
    /// Insert character key.
    /// </summary>
    InsertChar,

    /// <summary>
    /// Tab key.
    /// </summary>
    Tab,

    /// <summary>
    /// F1 key.
    /// </summary>
    F1,

    /// <summary>
    /// F2 key.
    /// </summary>
    F2,

    /// <summary>
    /// F3 key.
    /// </summary>
    F3,

    /// <summary>
    /// F4 key.
    /// </summary>
    F4,

    /// <summary>
    /// F5 key.
    /// </summary>
    ///
    F5,

    /// <summary>
    /// F6 key.
    /// </summary>
    F6,

    /// <summary>
    /// F7 key.
    /// </summary>
    F7,

    /// <summary>
    /// F8 key.
    /// </summary>
    F8,

    /// <summary>
    /// F9 key.
    /// </summary>
    F9,

    /// <summary>
    /// F10 key.
    /// </summary>
    F10,

    /// <summary>
    /// F11 key.
    /// </summary>
    F11,

    /// <summary>
    /// F12 key.
    /// </summary>
    F12,
}
