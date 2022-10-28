namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
///     Defines the possible modifier keys.
/// </summary>
[PublicAPI, Flags]
public enum ModifierKey
{
    /// <summary>
    ///     No modifier key was pressed.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Shift key.
    /// </summary>
    Shift = 1,

    /// <summary>
    ///     Alt key.
    /// </summary>
    Alt = Shift << 1,

    /// <summary>
    ///     Ctrl key.
    /// </summary>
    Ctrl = Alt << 1,

    /// <summary>
    ///     Caps-lock is on.
    /// </summary>
    CapsLock = Ctrl << 1,

    /// <summary>
    ///     Num-lock is on.
    /// </summary>
    NumLock = CapsLock << 1,

    /// <summary>
    ///     Scroll-lock is on.
    /// </summary>
    ScrollLock = NumLock << 1
}
