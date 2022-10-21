namespace Sharpie;

internal static class MouseCrap
{
    public const int NCURSES_MOUSE_VERSION = 2;
    public const int NCURSES_MOUSE_SHIFT = NCURSES_MOUSE_VERSION > 1 ? 5 : 6;
}

public enum MouseButton: uint
{
    BUTTON_1 = 1,
    BUTTON_2 = 2,
    BUTTON_3 = 3,
    BUTTON_4 = 4,
    BUTTON_5 = 5,
    MODIFIERS = MouseCrap.NCURSES_MOUSE_VERSION > 1 ? BUTTON_5 : 6,
}

[Flags]
public enum MouseButtonEvent: uint
{
    NCURSES_BUTTON_RELEASED = 1,
    NCURSES_BUTTON_PRESSED = NCURSES_BUTTON_RELEASED << 1,
    NCURSES_BUTTON_CLICKED = NCURSES_BUTTON_PRESSED << 1,
    NCURSES_DOUBLE_CLICKED = NCURSES_BUTTON_CLICKED << 2,
    NCURSES_TRIPLE_CLICKED = NCURSES_DOUBLE_CLICKED << 1,
    NCURSES_RESERVED_EVENT = NCURSES_TRIPLE_CLICKED << 1,
}

[Flags]
public enum MouseModifier: uint
{
    BUTTON_CTRL = 1U << (((int) MouseButton.MODIFIERS - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON_SHIFT = 2U << (((int) MouseButton.MODIFIERS - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON_ALT = 4U << (((int) MouseButton.MODIFIERS - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    REPORT_MOUSE_POSITION = 10U << (((int) MouseButton.MODIFIERS - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
}

[Flags]
public enum MouseEventType: ulong
{
    BUTTON1_RELEASED = MouseButtonEvent.NCURSES_BUTTON_RELEASED << (((int)MouseButton.BUTTON_1 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON1_PRESSED = MouseButtonEvent.NCURSES_BUTTON_PRESSED << (((int)MouseButton.BUTTON_1 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON1_CLICKED = MouseButtonEvent.NCURSES_BUTTON_CLICKED << (((int)MouseButton.BUTTON_1 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON1_DOUBLE_CLICKED = MouseButtonEvent.NCURSES_DOUBLE_CLICKED << (((int)MouseButton.BUTTON_1 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON1_TRIPLE_CLICKED = MouseButtonEvent.NCURSES_TRIPLE_CLICKED << (((int)MouseButton.BUTTON_1 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),

    BUTTON2_RELEASED = MouseButtonEvent.NCURSES_BUTTON_RELEASED << (((int)MouseButton.BUTTON_2 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON2_PRESSED = MouseButtonEvent.NCURSES_BUTTON_PRESSED << (((int)MouseButton.BUTTON_2 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON2_CLICKED = MouseButtonEvent.NCURSES_BUTTON_CLICKED << (((int)MouseButton.BUTTON_2 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON2_DOUBLE_CLICKED = MouseButtonEvent.NCURSES_DOUBLE_CLICKED << (((int)MouseButton.BUTTON_2 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON2_TRIPLE_CLICKED = MouseButtonEvent.NCURSES_TRIPLE_CLICKED << (((int)MouseButton.BUTTON_2 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),

    BUTTON3_RELEASED = MouseButtonEvent.NCURSES_BUTTON_RELEASED << (((int)MouseButton.BUTTON_3 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON3_PRESSED = MouseButtonEvent.NCURSES_BUTTON_PRESSED << (((int)MouseButton.BUTTON_3 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON3_CLICKED = MouseButtonEvent.NCURSES_BUTTON_CLICKED << (((int)MouseButton.BUTTON_3 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON3_DOUBLE_CLICKED = MouseButtonEvent.NCURSES_DOUBLE_CLICKED << (((int)MouseButton.BUTTON_3 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON3_TRIPLE_CLICKED = MouseButtonEvent.NCURSES_TRIPLE_CLICKED << (((int)MouseButton.BUTTON_3 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),

    BUTTON4_RELEASED = MouseButtonEvent.NCURSES_BUTTON_RELEASED << (((int)MouseButton.BUTTON_4 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON4_PRESSED = MouseButtonEvent.NCURSES_BUTTON_PRESSED << (((int)MouseButton.BUTTON_4 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON4_CLICKED = MouseButtonEvent.NCURSES_BUTTON_CLICKED << (((int)MouseButton.BUTTON_4 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON4_DOUBLE_CLICKED = MouseButtonEvent.NCURSES_DOUBLE_CLICKED << (((int)MouseButton.BUTTON_4 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON4_TRIPLE_CLICKED = MouseButtonEvent.NCURSES_TRIPLE_CLICKED << (((int)MouseButton.BUTTON_4 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),

    BUTTON5_RELEASED = MouseButtonEvent.NCURSES_BUTTON_RELEASED << (((int)MouseButton.BUTTON_5 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON5_PRESSED = MouseButtonEvent.NCURSES_BUTTON_PRESSED << (((int)MouseButton.BUTTON_5 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON5_CLICKED = MouseButtonEvent.NCURSES_BUTTON_CLICKED << (((int)MouseButton.BUTTON_5 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON5_DOUBLE_CLICKED = MouseButtonEvent.NCURSES_DOUBLE_CLICKED << (((int)MouseButton.BUTTON_5 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
    BUTTON5_TRIPLE_CLICKED = MouseButtonEvent.NCURSES_TRIPLE_CLICKED << (((int)MouseButton.BUTTON_5 - 1) * MouseCrap.NCURSES_MOUSE_SHIFT),
}
