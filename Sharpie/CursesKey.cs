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

namespace Sharpie;
#pragma warning disable CS1591

/// <summary>
///     Internal Curses key constants.
/// </summary>
public enum CursesKey: uint
{
    Yes = 0x100,
    Backspace = 0x107,
    Up = 0x103,
    Down = 0x102,
    Left = 0x104,
    Right = 0x105,
    PageDown = 0x152,
    PageUp = 0x153,
    Home = 0x106,
    Mouse = 0x199,
    End = 0x168,
    DeleteChar = 0x14a,
    InsertChar = 0x14b,
    Tab = 0x009,
    BackTab = 0x161,

    F1 = 0x109,
    F2 = F1 + 1,
    F3 = F2 + 1,
    F4 = F3 + 1,
    F5 = F4 + 1,
    F6 = F5 + 1,
    F7 = F6 + 1,
    F8 = F7 + 1,
    F9 = F8 + 1,
    F10 = F9 + 1,
    F11 = F10 + 1,
    F12 = F11 + 1,

    ShiftF1 = F1 + 12,
    ShiftF2 = ShiftF1 + 1,
    ShiftF3 = ShiftF2 + 1,
    ShiftF4 = ShiftF3 + 1,
    ShiftF5 = ShiftF4 + 1,
    ShiftF6 = ShiftF5 + 1,
    ShiftF7 = ShiftF6 + 1,
    ShiftF8 = ShiftF7 + 1,
    ShiftF9 = ShiftF8 + 1,
    ShiftF10 = ShiftF9 + 1,
    ShiftF11 = ShiftF10 + 1,
    ShiftF12 = ShiftF11 + 1,
    CtrlF1 = ShiftF1 + 12,
    CtrlF2 = CtrlF1 + 1,
    CtrlF3 = CtrlF2 + 1,
    CtrlF4 = CtrlF3 + 1,
    CtrlF5 = CtrlF4 + 1,
    CtrlF6 = CtrlF5 + 1,
    CtrlF7 = CtrlF6 + 1,
    CtrlF8 = CtrlF7 + 1,
    CtrlF9 = CtrlF8 + 1,
    CtrlF10 = CtrlF9 + 1,
    CtrlF11 = CtrlF10 + 1,
    CtrlF12 = CtrlF11 + 1,
    AltF1 = CtrlF1 + 12,
    AltF2 = AltF1 + 1,
    AltF3 = AltF2 + 1,
    AltF4 = AltF3 + 1,
    AltF5 = AltF4 + 1,
    AltF6 = AltF5 + 1,
    AltF7 = AltF6 + 1,
    AltF8 = AltF7 + 1,
    AltF9 = AltF8 + 1,
    AltF10 = AltF9 + 1,
    AltF11 = AltF10 + 1,
    AltF12 = AltF11 + 1,
    ShiftAltF1 = AltF1 + 12,
    ShiftAltF2 = ShiftAltF1 + 1,
    ShiftAltF3 = ShiftAltF2 + 1,
    ShiftAltF4 = ShiftAltF3 + 1,
    ShiftAltF5 = ShiftAltF4 + 1,
    ShiftAltF6 = ShiftAltF5 + 1,
    ShiftAltF7 = ShiftAltF6 + 1,
    ShiftAltF8 = ShiftAltF7 + 1,
    ShiftAltF9 = ShiftAltF8 + 1,
    ShiftAltF10 = ShiftAltF9 + 1,
    ShiftAltF11 = ShiftAltF10 + 1,
    ShiftAltF12 = ShiftAltF11 + 1,

    Resize = 0x19a,
    ShiftUp = 0x151,
    ShiftDown = 0x150,
    ShiftLeft = 0x189,
    ShiftRight = 0x192,
    ShiftPageDown = 0x18c,
    ShiftPageUp = 0x18e,
    ShiftHome = 0x187,
    ShiftEnd = 0x182,
    AltUp = 0x234,
    AltDown = 0x20b,
    AltLeft = 0x21f,
    AltRight = 0x22e,
    AltPageDown = 0x224,
    AltPageUp = 0x229,
    AltHome = 0x215,
    AltEnd = 0x210,
    CtrlUp = 0x236,
    CtrlDown = 0x20d,
    CtrlLeft = 0x221,
    CtrlRight = 0x230,
    CtrlPageDown = 0x226,
    CtrlPageUp = 0x22b,
    CtrlHome = 0x217,
    CtrlEnd = 0x212,
    ShiftCtrlUp = 0x237,
    ShiftCtrlDown = 0x20e,
    ShiftCtrlLeft = 0x222,
    ShiftCtrlRight = 0x231,
    ShiftCtrlPageDown = 0x227,
    ShiftCtrlPageUp = 0x22c,
    ShiftCtrlHome = 0x218,
    ShiftCtrlEnd = 0x213,
    ShiftAltUp = 0x235,
    ShiftAltDown = 0x20c,
    ShiftAltLeft = 0x220,
    ShiftAltRight = 0x22f,
    ShiftAltPageDown = 0x225,
    ShiftAltPageUp = 0x22a,
    ShiftAltHome = 0x216,
    ShiftAltEnd = 0x211,
    AltCtrlPageDown = 0x228,
    AltCtrlPageUp = 0x22d,
    AltCtrlHome = 0x219,
    AltCtrlEnd = 0x214
}
