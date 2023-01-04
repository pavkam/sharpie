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

namespace Sharpie.Backend;
#pragma warning disable CS1591

/// <summary>
///     PDCurses specific key codes.
/// </summary>
public enum PdCursesKeyCode: uint
{
    Yes = 0x100,
    Down = Yes + 0x02,
    Up = Yes + 0x03,
    Left = Yes + 0x04,
    Right = Yes + 0x05,
    Home = Yes + 0x06,
    Backspace = Yes + 0x07,
    Delete = Yes + 0x4a,
    Insert = Yes + 0x4b,
    F1 = Yes + 0x09,
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
    ScrollDown = Yes + 0x50,
    ScrollUp = Yes + 0x51,
    PageDown = Yes + 0x52,
    PageUp = Yes + 0x53,
    BackTab = Yes + 0x5f,
    End = Yes + 0x66,
    ShiftEnd = Yes + 0x80,
    ShiftHome = Yes + 0x84,
    ShiftLeft = Yes + 0x87,
    ShiftRight = Yes + 0x90,
    Alt0 = Yes + 0x97,
    Alt1 = Yes + 0x98,
    Alt2 = Yes + 0x99,
    Alt3 = Yes + 0x9a,
    Alt4 = Yes + 0x9b,
    Alt5 = Yes + 0x9c,
    Alt6 = Yes + 0x9d,
    Alt7 = Yes + 0x9e,
    Alt8 = Yes + 0x9f,
    Alt9 = Yes + 0xa0,
    AltA = Yes + 0xa1,
    AltB = Yes + 0xa2,
    AltC = Yes + 0xa3,
    AltD = Yes + 0xa4,
    AltE = Yes + 0xa5,
    AltF = Yes + 0xa6,
    AltG = Yes + 0xa7,
    AltH = Yes + 0xa8,
    AltI = Yes + 0xa9,
    AltJ = Yes + 0xaa,
    AltK = Yes + 0xab,
    AltL = Yes + 0xac,
    AltM = Yes + 0xad,
    AltN = Yes + 0xae,
    AltO = Yes + 0xaf,
    AltP = Yes + 0xb0,
    AltQ = Yes + 0xb1,
    AltR = Yes + 0xb2,
    AltS = Yes + 0xb3,
    AltT = Yes + 0xb4,
    AltU = Yes + 0xb5,
    AltV = Yes + 0xb6,
    AltW = Yes + 0xb7,
    AltX = Yes + 0xb8,
    AltY = Yes + 0xb9,
    AltZ = Yes + 0xba,
    CtrlLeft = Yes + 0xbb,
    CtrlRight = Yes + 0xbc,
    CtrlPageUp = Yes + 0xbd,
    CtrlPageDown = Yes + 0xbe,
    CtrlHome = Yes + 0xbf,
    CtrlEnd = Yes + 0xc0,
    KeypadSlash = Yes + 0xca,
    KeypadEnter = Yes + 0xcb,
    KeypadCtrlEnter = Yes + 0xcc,
    KeypadAltEnter = Yes + 0xcd,
    KeypadStop = Yes + 0xce,
    KeypadAsterisk = Yes + 0xcf,
    KeypadMinus = Yes + 0xd0,
    KeypadPlus = Yes + 0xd1,
    KeypadCtrlStop = Yes + 0xd2,
    KeypadCtrlMiddle = Yes + 0xd3,
    KeypadCtrlPlus = Yes + 0xd4,
    KeypadCtrlMinus = Yes + 0xd5,
    KeypadCtrlSlash = Yes + 0xd6,
    KeypadCtrlAsterisk = Yes + 0xd7,
    KeypadAltPlus = Yes + 0xd8,
    KeypadAltMinus = Yes + 0xd9,
    KeypadAltSlash = Yes + 0xda,
    KeypadAltAsterisk = Yes + 0xdb,
    KeypadAltStop = Yes + 0xdc,
    CtrlInsert = Yes + 0xdd,
    AltDelete = Yes + 0xde,
    AltInsert = Yes + 0xdf,
    CtrlUp = Yes + 0xe0,
    CtrlDown = Yes + 0xe1,
    CtrlTab = Yes + 0xe2,
    AltTab = Yes + 0xe3,
    AltMinus = Yes + 0xe4,
    AltEqual = Yes + 0xe5,
    AltHome = Yes + 0xe6,
    AltPageUp = Yes + 0xe7,
    AltPageDown = Yes + 0xe8,
    AltEnd = Yes + 0xe9,
    AltUp = Yes + 0xea,
    AltDown = Yes + 0xeb,
    AltRight = Yes + 0xec,
    AltLeft = Yes + 0xed,
    AltEnter = Yes + 0xee,
    AltEscape = Yes + 0xef,
    AltBackQuote = Yes + 0xf0,
    AltLeftBracket = Yes + 0xf1,
    AltRightBracket = Yes + 0xf2,
    AltSemicolon = Yes + 0xf3,
    AltForwardQuote = Yes + 0xf4,
    AltComma = Yes + 0xf5,
    AltStop = Yes + 0xf6,
    AltForwardSlash = Yes + 0xf7,
    AltBackspace = Yes + 0xf8,
    CtrlBackspace = Yes + 0xf9,
    KeyPad0 = Yes + 0xfa,
    KeyPadCtrl0 = Yes + 0xfb,
    KeyPadCtrl1 = Yes + 0xfc,
    KeyPadCtrl2 = Yes + 0xfd,
    KeyPadCtrl3 = Yes + 0xfe,
    KeyPadCtrl4 = Yes + 0xff,
    KeyPadCtrl5 = Yes + 0x100,
    KeyPadCtrl6 = Yes + 0x101,
    KeyPadCtrl7 = Yes + 0x102,
    KeyPadCtrl8 = Yes + 0x103,
    KeyPadCtrl9 = Yes + 0x104,
    KeyPadAlt0 = Yes + 0x105,
    KeyPadAlt1 = Yes + 0x106,
    KeyPadAlt2 = Yes + 0x107,
    KeyPadAlt3 = Yes + 0x108,
    KeyPadAlt4 = Yes + 0x109,
    KeyPadAlt5 = Yes + 0x10a,
    KeyPadAlt6 = Yes + 0x10b,
    KeyPadAlt7 = Yes + 0x10c,
    KeyPadAlt8 = Yes + 0x10d,
    KeyPadAlt9 = Yes + 0x10e,
    CtrlDelete = Yes + 0x10f,
    AltBackSlash = Yes + 0x110,
    CtrlEnter = Yes + 0x111,
    KeyPadShiftEnter = Yes + 0x112,
    KeyPadShiftSlash = Yes + 0x113,
    KeyPadShiftAsterisk = Yes + 0x114,
    KeyPadShiftPlus = Yes + 0x115,
    KeyPadShiftMinus = Yes + 0x116,
    KeyPadShiftUp = Yes + 0x117,
    KeyPadShiftDown = Yes + 0x118,
    KeyPadShiftInsert = Yes + 0x119,
    KeyPadShiftDelete = Yes + 0x11a,
    Mouse = Yes + 0x11b,
    LeftShift = Yes + 0x11c,
    RightShift = Yes + 0x11d,
    LeftCtrl = Yes + 0x11e,
    RightCtrl = Yes + 0x11f,
    LeftAlt = Yes + 0x120,
    RightAlt = Yes + 0x121,
    Resize = Yes + 0x122,
    ShiftUp = Yes + 0x123,
    ShiftDown = Yes + 0x124,
}
