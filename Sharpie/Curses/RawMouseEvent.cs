namespace Sharpie.Curses;

using System.Runtime.InteropServices;
using JetBrains.Annotations;

/// <summary>
/// Internal Curses mouse event.
/// </summary>
[PublicAPI, StructLayout(LayoutKind.Sequential)]
public struct RawMouseEvent
{
    private const int CursesMouseShift =  6;

    [PublicAPI]
    public enum Button: ulong
    {
        None = 0UL,
        Button1 = 1UL,
        Button2 = 2UL,
        Button3 = 3UL,
        Button4 = 4UL,
        Button5 = 5UL,
        Modifiers = 6UL,
    }

    [PublicAPI, Flags]
    public enum Action: ulong
    {
        ButtonReleased = 1UL,
        ButtonPressed = ButtonReleased << 1,
        ButtonClicked = ButtonPressed << 1,
        DoubleClicked = ButtonClicked << 1,
        TripleClicked = DoubleClicked << 1,
        Reserved = TripleClicked << 1,
    }

    [Flags, PublicAPI]
    public enum EventType: ulong
    {
        Button1Released = Action.ButtonReleased <<
            (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1Pressed = Action.ButtonPressed <<
            (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1Clicked = Action.ButtonClicked <<
            (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1DoubleClicked = Action.DoubleClicked <<
            (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1TripleClicked = Action.TripleClicked <<
            (((int) Button.Button1 - 1) * CursesMouseShift),

        Button2Released = Action.ButtonReleased <<
            (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2Pressed = Action.ButtonPressed <<
            (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2Clicked = Action.ButtonClicked <<
            (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2DoubleClicked = Action.DoubleClicked <<
            (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2TripleClicked = Action.TripleClicked <<
            (((int) Button.Button2 - 1) * CursesMouseShift),

        Button3Released = Action.ButtonReleased <<
            (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3Pressed = Action.ButtonPressed <<
            (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3Clicked = Action.ButtonClicked <<
            (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3DoubleClicked = Action.DoubleClicked <<
            (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3TripleClicked = Action.TripleClicked <<
            (((int) Button.Button3 - 1) * CursesMouseShift),

        Button4Released = Action.ButtonReleased <<
            (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4Pressed = Action.ButtonPressed <<
            (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4Clicked = Action.ButtonClicked <<
            (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4DoubleClicked = Action.DoubleClicked <<
            (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4TripleClicked = Action.TripleClicked <<
            (((int) Button.Button4 - 1) * CursesMouseShift),

        Button5Released = Action.ButtonReleased <<
            (((int) Button.Button5 - 1) * CursesMouseShift),

        Button5Pressed = Action.ButtonPressed <<
            (((int) Button.Button5 - 1) * CursesMouseShift),

        Button5Clicked = Action.ButtonClicked <<
            (((int) Button.Button5 - 1) * CursesMouseShift),

        Button5DoubleClicked = Action.DoubleClicked <<
            (((int) Button.Button5 - 1) * CursesMouseShift),

        Button5TripleClicked = Action.TripleClicked <<
            (((int) Button.Button5 - 1) * CursesMouseShift),

        Ctrl = 1UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        Shift = 2UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        Alt = 4UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        ReportPosition = 8UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        All = ReportPosition - 1
    }


    [MarshalAs(UnmanagedType.I2)] public readonly short id;
    [MarshalAs(UnmanagedType.I4)] public readonly int x;
    [MarshalAs(UnmanagedType.I4)] public readonly int y;
    [MarshalAs(UnmanagedType.I4)] public readonly int z;
    [MarshalAs(UnmanagedType.I8)] public readonly ulong buttonState;
}
