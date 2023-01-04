namespace Sharpie.Backend;

/// <summary>
///     The base type for all back-end independent Curses events.
/// </summary>
public abstract record CursesEvent;

/// <summary>
///     Character input event.
/// </summary>
/// <param name="Char">The character that was read from the terminal.</param>
/// <param name="Name">The description of the character (backend-specific).</param>
/// <param name="Modifiers">The key modifiers.</param>
public sealed record CursesCharEvent(string? Name, char Char, ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Key input event.
/// </summary>
/// <param name="Key">The key that was read from the terminal.</param>
/// <param name="Name">The description of the character (backend-specific).</param>
/// <param name="Modifiers">The key modifiers.</param>
public sealed record CursesKeyEvent(string? Name, Key Key, ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Mouse input event.
/// </summary>
/// <param name="X">The mouse X coordinate.</param>
/// <param name="Y">The mouse Y coordinate.</param>
/// <param name="Button">The mouse button.</param>
/// <param name="State">The mouse button state.</param>
/// <param name="Modifiers">Key modifiers.</param>
public sealed record CursesMouseEvent(int X, int Y, MouseButton Button, MouseButtonState State,
    ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Terminal resize event.
/// </summary>
public sealed record CursesResizeEvent: CursesEvent;
