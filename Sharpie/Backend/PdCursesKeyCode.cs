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
    Yes = 0x100,       /* If get_wch() gives a key code */
    Break = 0x101,     /* Not on PC KBD */
    Down = 0x102,      /* Down arrow key */
    Up = 0x103,        /* Up arrow key */
    Left = 0x104,      /* Left arrow key */
    Right = 0x105,     /* Right arrow key */
    Home = 0x106,      /* home key */
    Backspace = 0x107, /* not on pc */
    Delete = 0x14a,
    Insert = 0x14b,
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

    ScrollDown = 0x150,
    ScrollUp = 0x151,
    PageDown = 0x152,
    PageUp = 0x153,
    BackTab = 0x15f,    /* Back tab key */
    End = 0x166,        /* end key */
    ShiftEnd = 0x180,   /* shifted end key */
    ShiftHome = 0x184,  /* shifted home key */
    ShiftLeft = 0x187,  /* shifted left arrow key */
    ShiftRight = 0x190, /* shifted right arrow */
    Alt0 = 0x197,
    Alt1 = 0x198,
    Alt2 = 0x199,
    Alt3 = 0x19a,
    Alt4 = 0x19b,
    Alt5 = 0x19c,
    Alt6 = 0x19d,
    Alt7 = 0x19e,
    Alt8 = 0x19f,
    Alt9 = 0x1a0,
    AltA = 0x1a1,
    AltB = 0x1a2,
    AltC = 0x1a3,
    AltD = 0x1a4,
    AltE = 0x1a5,
    AltF = 0x1a6,
    AltG = 0x1a7,
    AltH = 0x1a8,
    AltI = 0x1a9,
    AltJ = 0x1aa,
    AltK = 0x1ab,
    AltL = 0x1ac,
    AltM = 0x1ad,
    AltN = 0x1ae,
    AltO = 0x1af,
    AltP = 0x1b0,
    AltQ = 0x1b1,
    AltR = 0x1b2,
    AltS = 0x1b3,
    AltT = 0x1b4,
    AltU = 0x1b5,
    AltV = 0x1b6,
    AltW = 0x1b7,
    AltX = 0x1b8,
    AltY = 0x1b9,
    AltZ = 0x1ba,

    CtrlLeft = 0x1bb, /* Control-Left-Arrow */
    CtrlRight = 0x1bc,
    CtrlPageUp = 0x1bd,
    CtrlPageDown = 0x1be,
    CtrlHome = 0x1bf,
    CtrlEnd = 0x1c0,

    KeypadSlash = 0x1ca,        /* slash on keypad */
    KeypadEnter = 0x1cb,        /* enter on keypad */
    KeypadCtrlEnter = 0x1cc,    /* ctl-enter on keypad */
    KeypadAltEnter = 0x1cd,     /* alt-enter on keypad */
    KeypadStop = 0x1ce,         /* stop on keypad */
    KeypadAsterisk = 0x1cf,     /* star on keypad */
    KeypadMinus = 0x1d0,        /* minus on keypad */
    KeypadPlus = 0x1d1,         /* plus on keypad */
    KeypadCtrlStop = 0x1d2,     /* ctl-stop on keypad */
    KeypadCtrlMiddle = 0x1d3,   /* ctl-enter on keypad */
    KeypadCtrlPlus = 0x1d4,     /* ctl-plus on keypad */
    KeypadCtrlMinus = 0x1d5,    /* ctl-minus on keypad */
    KeypadCtrlSlash = 0x1d6,    /* ctl-slash on keypad */
    KeypadCtrlAsterisk = 0x1d7, /* ctl-star on keypad */
    KeypadAltPlus = 0x1d8,      /* alt-plus on keypad */
    KeypadAltMinus = 0x1d9,     /* alt-minus on keypad */
    KeypadAltSlash = 0x1da,     /* alt-slash on keypad */
    KeypadAltAsterisk = 0x1db,      /* alt-star on keypad */
    KeypadAltStop = 0x1dc,      /* alt-stop on keypad */
    CtrlInsert = 0x1dd,         /* ctl-insert */
    AltDelete = 0x1de,          /* alt-delete */
    AltInsert = 0x1df,          /* alt-insert */
    CtrlUp = 0x1e0,             /* ctl-up arrow */
    CtrlDown = 0x1e1,           /* ctl-down arrow */
    CtrlTab = 0x1e2,            /* ctl-tab */
    AltTab = 0x1e3,
    AltMinus = 0x1e4,
    AltEqual = 0x1e5,
    AltHome = 0x1e6,
    AltPageUp = 0x1e7,
    AltPageDown = 0x1e8,
    AltEnd = 0x1e9,
    AltUp = 0x1ea,           /* alt-up arrow */
    AltDown = 0x1eb,         /* alt-down arrow */
    AltRight = 0x1ec,        /* alt-right arrow */
    AltLeft = 0x1ed,         /* alt-left arrow */
    AltEnter = 0x1ee,        /* alt-enter */
    AltEscape = 0x1ef,       /* alt-escape */
    AltBackQuote = 0x1f0,    /* alt-back quote */
    AltLeftBracket = 0x1f1,  /* alt-left bracket */
    AltRightBracket = 0x1f2, /* alt-right bracket */
    AltSemicolon = 0x1f3,    /* alt-semi-colon */
    AltForwardQuote = 0x1f4, /* alt-forward quote */
    AltComma = 0x1f5,        /* alt-comma */
    AltStop = 0x1f6,         /* alt-stop */
    AltForwardSlash = 0x1f7, /* alt-forward slash */
    AltBackspace = 0x1f8,    /* alt-backspace */
    CtrlBackspace = 0x1f9,   /* ctl-backspace */

    KeyPad0 = 0x1fa, /* keypad 0 */

    KeyPadCtrl0 = 0x1fb, /* ctl-keypad 0 */
    KeyPadCtrl1 = 0x1fc,
    KeyPadCtrl2 = 0x1fd,
    KeyPadCtrl3 = 0x1fe,
    KeyPadCtrl4 = 0x1ff,
    KeyPadCtrl5 = 0x200,
    KeyPadCtrl6 = 0x201,
    KeyPadCtrl7 = 0x202,
    KeyPadCtrl8 = 0x203,
    KeyPadCtrl9 = 0x204,

    KeyPadAlt0 = 0x205, /* alt-keypad 0 */
    KeyPadAlt1 = 0x206,
    KeyPadAlt2 = 0x207,
    KeyPadAlt3 = 0x208,
    KeyPadAlt4 = 0x209,
    KeyPadAlt5 = 0x20a,
    KeyPadAlt6 = 0x20b,
    KeyPadAlt7 = 0x20c,
    KeyPadAlt8 = 0x20d,
    KeyPadAlt9 = 0x20e,

    CtrlDelete = 0x20f,   /* clt-delete */
    AltBackSlash = 0x210, /* alt-back slash */
    CtrlEnter = 0x211,    /* ctl-enter */

    KeyPadShiftEnter = 0x212,    /* shift-enter on keypad */
    KeyPadShiftSlash = 0x213,    /* shift-slash on keypad */
    KeyPadShiftAsterisk = 0x214, /* shift-star  on keypad */
    KeyPadShiftPlus = 0x215,     /* shift-plus  on keypad */
    KeyPadShiftMinus = 0x216,    /* shift-minus on keypad */
    KeyPadShiftUp = 0x217,       /* shift-up on keypad */
    KeyPadShiftDown = 0x218,     /* shift-down on keypad */
    KeyPadShiftInsert = 0x219,   /* shift-insert on keypad */
    KeyPadShiftDelete = 0x21a,   /* shift-delete on keypad */

    Mouse = 0x21b,      /* "mouse" key */
    LeftShift = 0x21c,  /* Left-shift */
    RightShift = 0x21d, /* Right-shift */
    LeftCtrl = 0x21e,   /* Left-control */
    RightCtrl = 0x21f,  /* Right-control */
    LeftAlt = 0x220,    /* Left-alt */
    RightAlt = 0x221,   /* Right-alt */
    Resize = 0x222,     /* Window resize */
    ShiftUp = 0x223,    /* Shifted up arrow */
    ShiftDown = 0x224,  /* Shifted down arrow */
}
