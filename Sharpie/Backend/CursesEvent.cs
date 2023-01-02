namespace Sharpie.Backend;

public abstract record CursesEvent;

public sealed record CursesCharEvent(char @char): CursesEvent;

public sealed record CursesKeyEvent(Key key, ModifierKey modifiers): CursesEvent;

public sealed record CursesMouseEvent(int x, int y, MouseButton button, MouseButtonState state, ModifierKey modifiers): CursesEvent;

public sealed record CursesResizeEvent: CursesEvent;
