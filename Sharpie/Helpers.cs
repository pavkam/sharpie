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

/// <summary>
///     Internal helper routines.
/// </summary>
public static class Helpers
{
    private const int CursesErrorResult = -1;

    /// <summary>
    ///     Checks if a given code shows a failure.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The result of the failure check.</returns>
    public static bool Failed(this int code) => code == CursesErrorResult;

    /// <summary>
    ///     Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="code">The return code.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="code" /> value.</returns>
    /// <exception cref="CursesOperationException">Thrown if <paramref name="code" /> indicates an error.</exception>
    internal static int Check(this int code, string operation, string message)
    {
        if (code == CursesErrorResult)
        {
            throw new CursesOperationException(operation, message);
        }

        return code;
    }

    /// <summary>
    ///     Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="ptr">The return pointer.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="ptr" /> value.</returns>
    /// <exception cref="CursesOperationException">Thrown if <paramref name="ptr" /> is zero.</exception>
    internal static IntPtr Check(this IntPtr ptr, string operation, string message)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new CursesOperationException(operation, message);
        }

        return ptr;
    }

    /// <summary>
    ///     Converts millis to tenths of a second by rounding up.
    /// </summary>
    /// <param name="value">The millis.</param>
    /// <returns>The value in 100s of millis.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="value" /> is less than zero.</exception>
    internal static int ConvertMillisToTenths(int value)
    {
        switch (value)
        {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(value));
            case 0:
                return 0;
            default:
            {
                var hundreds = value / 100 + (value % 100 > 0 ? 1 : 0);
                return Math.Min(255, hundreds);
            }
        }
    }

    /// <summary>
    ///     Checks that a given Curses backend is valid.
    /// </summary>
    /// <param name="curses">The Curses backend.</param>
    /// <returns>Returns <paramref name="curses" /> if it's valid. <c>null</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="curses"/> is <c>null</c>.</exception>
    internal static ICursesProvider? ValidOrNull(this ICursesProvider curses)
    {
        if (curses == null)
        {
            throw new ArgumentNullException(nameof(curses));
        }

        try
        {
            curses.termname();
        } catch (Exception e)
        {
            if (e is DllNotFoundException or EntryPointNotFoundException)
            {
                return null;
            }

            throw;
        }

        return curses;
    }

    /// <summary>
    ///     Converts a given rune to a complex character.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="rune">The rune to convert.</param>
    /// <param name="style">The style to apply.</param>
    /// <returns>The complex character.</returns>
    /// <exception cref="CursesOperationException">Thrown if <paramref name="rune" /> failed to convert to a complex char.</exception>
    public static CursesComplexChar ToComplexChar(this ICursesProvider curses, Rune rune, Style style)
    {
        // Convert the special characters into Unicode.
        if (rune.IsAscii &&
            rune.Value != ControlCharacter.NewLine &&
            rune.Value != '\b' &&
            rune.Value != ControlCharacter.Tab &&
            rune.Value <= 0x1F ||
            rune.Value is >= 0X7F and <= 0x9F)
        {
            rune = new(rune.Value + 0x2400);
        }

        // Use Curses to encode the characters.
        curses.setcchar(out var @char, rune.ToString(), (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(curses.setcchar), "Failed to convert string to complex character.");

        return @char;
    }

    /// <summary>
    ///     Converts a complex char into a rune and its style.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="char">The char to breakdown.</param>
    /// <returns>The rune and the style.</returns>
    public static (Rune rune, Style style) FromComplexChar(this ICursesProvider curses, CursesComplexChar @char)
    {
        // Use Curses to decode the characters. Assume 10 characters is enough in the string.

        var builder = new StringBuilder(10);
        curses.getcchar(@char, builder, out var attrs, out var colorPair, IntPtr.Zero)
              .Check(nameof(curses.getcchar), "Failed to deconstruct the complex character.");

        return (Rune.GetRuneAt(builder.ToString(), 0),
            new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } });
    }

    /// <summary>
    ///     Converts a key code for a key to a known key and modifiers.
    /// </summary>
    /// <param name="keyCode">The key code.</param>
    /// <returns>The key and modifiers combination.</returns>
    internal static (Key key, ModifierKey modifierKey) ConvertKeyPressEvent(uint keyCode)
    {
        return (CursesKey) keyCode switch
        {
            CursesKey.F1 => (Key.F1, ModifierKey.None),
            CursesKey.F2 => (Key.F2, ModifierKey.None),
            CursesKey.F3 => (Key.F3, ModifierKey.None),
            CursesKey.F4 => (Key.F4, ModifierKey.None),
            CursesKey.F5 => (Key.F5, ModifierKey.None),
            CursesKey.F6 => (Key.F6, ModifierKey.None),
            CursesKey.F7 => (Key.F7, ModifierKey.None),
            CursesKey.F8 => (Key.F8, ModifierKey.None),
            CursesKey.F9 => (Key.F9, ModifierKey.None),
            CursesKey.F10 => (Key.F10, ModifierKey.None),
            CursesKey.F11 => (Key.F11, ModifierKey.None),
            CursesKey.F12 => (Key.F12, ModifierKey.None),
            CursesKey.ShiftF1 => (Key.F1, ModifierKey.Shift),
            CursesKey.ShiftF2 => (Key.F2, ModifierKey.Shift),
            CursesKey.ShiftF3 => (Key.F3, ModifierKey.Shift),
            CursesKey.ShiftF4 => (Key.F4, ModifierKey.Shift),
            CursesKey.ShiftF5 => (Key.F5, ModifierKey.Shift),
            CursesKey.ShiftF6 => (Key.F6, ModifierKey.Shift),
            CursesKey.ShiftF7 => (Key.F7, ModifierKey.Shift),
            CursesKey.ShiftF8 => (Key.F8, ModifierKey.Shift),
            CursesKey.ShiftF9 => (Key.F9, ModifierKey.Shift),
            CursesKey.ShiftF10 => (Key.F10, ModifierKey.Shift),
            CursesKey.ShiftF11 => (Key.F11, ModifierKey.Shift),
            CursesKey.ShiftF12 => (Key.F12, ModifierKey.Shift),
            CursesKey.CtrlF1 => (Key.F1, ModifierKey.Ctrl),
            CursesKey.CtrlF2 => (Key.F2, ModifierKey.Ctrl),
            CursesKey.CtrlF3 => (Key.F3, ModifierKey.Ctrl),
            CursesKey.CtrlF4 => (Key.F4, ModifierKey.Ctrl),
            CursesKey.CtrlF5 => (Key.F5, ModifierKey.Ctrl),
            CursesKey.CtrlF6 => (Key.F6, ModifierKey.Ctrl),
            CursesKey.CtrlF7 => (Key.F7, ModifierKey.Ctrl),
            CursesKey.CtrlF8 => (Key.F8, ModifierKey.Ctrl),
            CursesKey.CtrlF9 => (Key.F9, ModifierKey.Ctrl),
            CursesKey.CtrlF10 => (Key.F10, ModifierKey.Ctrl),
            CursesKey.CtrlF11 => (Key.F11, ModifierKey.Ctrl),
            CursesKey.CtrlF12 => (Key.F12, ModifierKey.Ctrl),
            CursesKey.AltF1 => (Key.F1, ModifierKey.Alt),
            CursesKey.AltF2 => (Key.F2, ModifierKey.Alt),
            CursesKey.AltF3 => (Key.F3, ModifierKey.Alt),
            CursesKey.AltF4 => (Key.F4, ModifierKey.Alt),
            CursesKey.AltF5 => (Key.F5, ModifierKey.Alt),
            CursesKey.AltF6 => (Key.F6, ModifierKey.Alt),
            CursesKey.AltF7 => (Key.F7, ModifierKey.Alt),
            CursesKey.AltF8 => (Key.F8, ModifierKey.Alt),
            CursesKey.AltF9 => (Key.F9, ModifierKey.Alt),
            CursesKey.AltF10 => (Key.F10, ModifierKey.Alt),
            CursesKey.AltF11 => (Key.F11, ModifierKey.Alt),
            CursesKey.AltF12 => (Key.F12, ModifierKey.Alt),
            CursesKey.ShiftAltF1 => (Key.F1, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF2 => (Key.F2, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF3 => (Key.F3, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF4 => (Key.F4, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF5 => (Key.F5, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF6 => (Key.F6, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF7 => (Key.F7, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF8 => (Key.F8, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF9 => (Key.F9, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF10 => (Key.F10, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF11 => (Key.F11, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.ShiftAltF12 => (Key.F12, ModifierKey.Alt | ModifierKey.Shift),
            CursesKey.Up => (Key.KeypadUp, ModifierKey.None),
            CursesKey.Down => (Key.KeypadDown, ModifierKey.None),
            CursesKey.Left => (Key.KeypadLeft, ModifierKey.None),
            CursesKey.Right => (Key.KeypadRight, ModifierKey.None),
            CursesKey.Home => (Key.KeypadHome, ModifierKey.None),
            CursesKey.End => (Key.KeypadEnd, ModifierKey.None),
            CursesKey.PageDown => (Key.KeypadPageDown, ModifierKey.None),
            CursesKey.PageUp => (Key.KeypadPageUp, ModifierKey.None),
            CursesKey.DeleteChar => (Key.DeleteChar, ModifierKey.None),
            CursesKey.InsertChar => (Key.InsertChar, ModifierKey.None),
            CursesKey.Tab => (Key.Tab, ModifierKey.None),
            CursesKey.BackTab => (Key.Tab, ModifierKey.Shift),
            CursesKey.Backspace => (Key.Backspace, ModifierKey.None),
            CursesKey.ShiftUp => (Key.KeypadUp, ModifierKey.Shift),
            CursesKey.ShiftDown => (Key.KeypadDown, ModifierKey.Shift),
            CursesKey.ShiftLeft => (Key.KeypadLeft, ModifierKey.Shift),
            CursesKey.ShiftRight => (Key.KeypadRight, ModifierKey.Shift),
            CursesKey.ShiftHome => (Key.KeypadHome, ModifierKey.Shift),
            CursesKey.ShiftEnd => (Key.KeypadEnd, ModifierKey.Shift),
            CursesKey.ShiftPageDown => (Key.KeypadPageDown, ModifierKey.Shift),
            CursesKey.ShiftPageUp => (Key.KeypadPageUp, ModifierKey.Shift),
            CursesKey.AltUp => (Key.KeypadUp, ModifierKey.Alt),
            CursesKey.AltDown => (Key.KeypadDown, ModifierKey.Alt),
            CursesKey.AltLeft => (Key.KeypadLeft, ModifierKey.Alt),
            CursesKey.AltRight => (Key.KeypadRight, ModifierKey.Alt),
            CursesKey.AltHome => (Key.KeypadHome, ModifierKey.Alt),
            CursesKey.AltEnd => (Key.KeypadEnd, ModifierKey.Alt),
            CursesKey.AltPageDown => (Key.KeypadPageDown, ModifierKey.Alt),
            CursesKey.AltPageUp => (Key.KeypadPageUp, ModifierKey.Alt),
            CursesKey.CtrlUp => (Key.KeypadUp, ModifierKey.Ctrl),
            CursesKey.CtrlDown => (Key.KeypadDown, ModifierKey.Ctrl),
            CursesKey.CtrlLeft => (Key.KeypadLeft, ModifierKey.Ctrl),
            CursesKey.CtrlRight => (Key.KeypadRight, ModifierKey.Ctrl),
            CursesKey.CtrlHome => (Key.KeypadHome, ModifierKey.Ctrl),
            CursesKey.CtrlEnd => (Key.KeypadEnd, ModifierKey.Ctrl),
            CursesKey.CtrlPageDown => (Key.KeypadPageDown, ModifierKey.Ctrl),
            CursesKey.CtrlPageUp => (Key.KeypadPageUp, ModifierKey.Ctrl),
            CursesKey.ShiftCtrlUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Ctrl),
            CursesKey.ShiftAltUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.ShiftAltEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
            CursesKey.AltCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Alt | ModifierKey.Ctrl),
            CursesKey.AltCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
            CursesKey.AltCtrlHome => (Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
            CursesKey.AltCtrlEnd => (Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
            var _ => (Key.Unknown, ModifierKey.None)
        };
    }

    /// <summary>
    ///     Converts a Curses mouse action into proper format.
    /// </summary>
    /// <param name="type">The Curses mouse event type.</param>
    /// <returns>The mouse action attributes.</returns>
    internal static (MouseButton button, MouseButtonState state, ModifierKey modifierKey) ConvertMouseActionEvent(
        CursesMouseEvent.EventType type)
    {
        var modifierKey = ModifierKey.None;
        var button = (MouseButton) 0;
        var state = (MouseButtonState) 0;

        if (type.HasFlag(CursesMouseEvent.EventType.Alt))
        {
            modifierKey |= ModifierKey.Alt;
        }

        if (type.HasFlag(CursesMouseEvent.EventType.Ctrl))
        {
            modifierKey |= ModifierKey.Ctrl;
        }

        if (type.HasFlag(CursesMouseEvent.EventType.Shift))
        {
            modifierKey |= ModifierKey.Shift;
        }

        if (type.HasFlag(CursesMouseEvent.EventType.Button1Released))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button1Pressed))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button1Clicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button1DoubleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button1TripleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button2Released))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button2Pressed))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button2Clicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button2DoubleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button2TripleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button3Released))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button3Pressed))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button3Clicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button3DoubleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button3TripleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button4Released))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button4Pressed))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button4Clicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button4DoubleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(CursesMouseEvent.EventType.Button4TripleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.TripleClicked;
        }

        return (button, state, modifierKey);
    }
}
