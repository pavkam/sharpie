namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the attributes used by Curses library.
/// </summary>
[PublicAPI]
public enum VideoAttributea: uint
{
    A_NORMAL = 0U,
    WA_NORMAL = A_NORMAL,
    A_ATTRIBUTES = ~0U << 8,
    WA_ATTRIBUTES = A_ATTRIBUTES,
    A_CHARTEXT = (1U << 8) - 1,
    A_COLOR = ((1U << 8) - 1) << 8,

    A_STANDOUT = 1U << (8 + 8),
    WA_STANDOUT = A_STANDOUT,
    A_UNDERLINE = A_STANDOUT << 1,
    WA_UNDERLINE = A_UNDERLINE,
    A_REVERSE = A_UNDERLINE << 1,
    WA_REVERSE = A_REVERSE,
    A_BLINK = A_REVERSE << 1,
    WA_BLINK = A_BLINK,
    A_DIM = A_BLINK << 1,
    WA_DIM = A_DIM,
    A_BOLD = A_DIM << 1,
    WA_BOLD = A_BOLD,
    A_ALTCHARSET = A_BOLD << 1,
    WA_ALTCHARSET = A_ALTCHARSET,
    A_INVIS = A_ALTCHARSET << 1,
    WA_INVIS = A_INVIS,
    A_PROTECT = A_INVIS << 1,
    WA_PROTECT = A_PROTECT,
    A_HORIZONTAL = A_PROTECT << 1,
    WA_HORIZONTAL = A_HORIZONTAL,
    A_LEFT = A_HORIZONTAL << 1,
    WA_LEFT = A_LEFT,
    A_LOW = A_LEFT << 1,
    WA_LOW = A_LOW,
    A_RIGHT = A_LOW << 1,
    WA_RIGHT = A_RIGHT,
    A_TOP = A_RIGHT << 1,
    WA_TOP = A_TOP,
    A_VERTICAL = A_TOP << 1,
    WA_VERTICAL = A_VERTICAL,
    A_ITALIC = A_VERTICAL << 1,
    WA_ITALIC = A_ITALIC
}

