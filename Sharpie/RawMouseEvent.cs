/*
Copyright (c) 2022, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#pragma warning disable CS1591
namespace Sharpie;

using System.Runtime.InteropServices;

/// <summary>
///     Internal Curses mouse event.
/// </summary>
[PublicAPI, StructLayout(LayoutKind.Sequential), ExcludeFromCodeCoverage]
public struct RawMouseEvent
{
    private const int CursesMouseShift = 6;

    [PublicAPI]
    public enum Button: ulong
    {
        None = 0UL,
        Button1 = 1UL,
        Button2 = 2UL,
        Button3 = 3UL,
        Button4 = 4UL,
        Modifiers = 5UL
    }

    [PublicAPI, Flags]
    public enum Action: ulong
    {
        ButtonReleased = 1UL,
        ButtonPressed = ButtonReleased << 1,
        ButtonClicked = ButtonPressed << 1,
        DoubleClicked = ButtonClicked << 1,
        TripleClicked = DoubleClicked << 1,
        Reserved = TripleClicked << 1
    }

    [Flags, PublicAPI]
    public enum EventType: ulong
    {
        Button1Released = Action.ButtonReleased << (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1Pressed = Action.ButtonPressed << (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1Clicked = Action.ButtonClicked << (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1DoubleClicked = Action.DoubleClicked << (((int) Button.Button1 - 1) * CursesMouseShift),

        Button1TripleClicked = Action.TripleClicked << (((int) Button.Button1 - 1) * CursesMouseShift),

        Button2Released = Action.ButtonReleased << (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2Pressed = Action.ButtonPressed << (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2Clicked = Action.ButtonClicked << (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2DoubleClicked = Action.DoubleClicked << (((int) Button.Button2 - 1) * CursesMouseShift),

        Button2TripleClicked = Action.TripleClicked << (((int) Button.Button2 - 1) * CursesMouseShift),

        Button3Released = Action.ButtonReleased << (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3Pressed = Action.ButtonPressed << (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3Clicked = Action.ButtonClicked << (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3DoubleClicked = Action.DoubleClicked << (((int) Button.Button3 - 1) * CursesMouseShift),

        Button3TripleClicked = Action.TripleClicked << (((int) Button.Button3 - 1) * CursesMouseShift),

        Button4Released = Action.ButtonReleased << (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4Pressed = Action.ButtonPressed << (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4Clicked = Action.ButtonClicked << (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4DoubleClicked = Action.DoubleClicked << (((int) Button.Button4 - 1) * CursesMouseShift),

        Button4TripleClicked = Action.TripleClicked << (((int) Button.Button4 - 1) * CursesMouseShift),

        Ctrl = 1UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        Shift = 2UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        Alt = 4UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        ReportPosition = 8UL << (((int) Button.Modifiers - 1) * CursesMouseShift),
        All = ReportPosition - 1
    }

    [MarshalAs(UnmanagedType.I2)] public short id;
    [MarshalAs(UnmanagedType.I4)] public int x;
    [MarshalAs(UnmanagedType.I4)] public int y;
    [MarshalAs(UnmanagedType.I4)] public int z;
    [MarshalAs(UnmanagedType.I8)] public ulong buttonState;
}
