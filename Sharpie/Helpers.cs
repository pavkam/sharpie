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

using Curses;

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
    /// <exception cref="CursesException">Thrown if <paramref name="code" /> indicates an error.</exception>
    internal static int Check(this int code, string operation, string message)
    {
        if (code == CursesErrorResult)
        {
            throw new CursesException(operation, message);
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
    /// <exception cref="CursesException">Thrown if <paramref name="ptr" /> is zero.</exception>
    internal static IntPtr Check(this IntPtr ptr, string operation, string message)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new CursesException(operation, message);
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
    ///     Converts a given rune to a complex character.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="rune">The rune to convert.</param>
    /// <param name="style">The style to apply.</param>
    /// <returns>The complex character.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="rune" /> failed to convert to a complex char.</exception>
    public static ComplexChar ToComplexChar(this ICursesProvider curses, Rune rune, Style style)
    {
        // Convert the special characters into Unicode.
        if (rune.IsAscii && rune.Value != '\n' && rune.Value != '\b' && rune.Value != '\t' && rune.Value <= 0x1F ||
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
    public static (Rune rune, Style style) FromComplexChar(this ICursesProvider curses, ComplexChar @char)
    {
        // Use Curses to decode the characters. Assume 10 characters is enough in the string.

        var builder = new StringBuilder(10);
        curses.getcchar(@char, builder, out var attrs, out var colorPair, IntPtr.Zero)
              .Check(nameof(curses.getcchar), "Failed to deconstruct the complex character.");

        return (Rune.GetRuneAt(builder.ToString(), 0),
            new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } });
    }
    
    /// <summary>
    /// Converts a key code for a key to a known key and modifiers.
    /// </summary>
    /// <param name="keyCode">The key code.</param>
    /// <returns>The key and modifiers combination.</returns>
    internal static (Key key, ModifierKey modifierKey) ConvertKeyPressEvent(uint keyCode)
    {
        return (RawKey) keyCode switch
        {
            RawKey.F1 => (Key.F1, ModifierKey.None),
            RawKey.F2 => (Key.F2, ModifierKey.None),
            RawKey.F3 => (Key.F3, ModifierKey.None),
            RawKey.F4 => (Key.F4, ModifierKey.None),
            RawKey.F5 => (Key.F5, ModifierKey.None),
            RawKey.F6 => (Key.F6, ModifierKey.None),
            RawKey.F7 => (Key.F7, ModifierKey.None),
            RawKey.F8 => (Key.F8, ModifierKey.None),
            RawKey.F9 => (Key.F9, ModifierKey.None),
            RawKey.F10 => (Key.F10, ModifierKey.None),
            RawKey.F11 => (Key.F11, ModifierKey.None),
            RawKey.F12 => (Key.F12, ModifierKey.None),
            RawKey.ShiftF1 => (Key.F1, ModifierKey.Shift),
            RawKey.ShiftF2 => (Key.F2, ModifierKey.Shift),
            RawKey.ShiftF3 => (Key.F3, ModifierKey.Shift),
            RawKey.ShiftF4 => (Key.F4, ModifierKey.Shift),
            RawKey.ShiftF5 => (Key.F5, ModifierKey.Shift),
            RawKey.ShiftF6 => (Key.F6, ModifierKey.Shift),
            RawKey.ShiftF7 => (Key.F7, ModifierKey.Shift),
            RawKey.ShiftF8 => (Key.F8, ModifierKey.Shift),
            RawKey.ShiftF9 => (Key.F9, ModifierKey.Shift),
            RawKey.ShiftF10 => (Key.F10, ModifierKey.Shift),
            RawKey.ShiftF11 => (Key.F11, ModifierKey.Shift),
            RawKey.ShiftF12 => (Key.F12, ModifierKey.Shift),
            RawKey.CtrlF1 => (Key.F1, ModifierKey.Ctrl),
            RawKey.CtrlF2 => (Key.F2, ModifierKey.Ctrl),
            RawKey.CtrlF3 => (Key.F3, ModifierKey.Ctrl),
            RawKey.CtrlF4 => (Key.F4, ModifierKey.Ctrl),
            RawKey.CtrlF5 => (Key.F5, ModifierKey.Ctrl),
            RawKey.CtrlF6 => (Key.F6, ModifierKey.Ctrl),
            RawKey.CtrlF7 => (Key.F7, ModifierKey.Ctrl),
            RawKey.CtrlF8 => (Key.F8, ModifierKey.Ctrl),
            RawKey.CtrlF9 => (Key.F9, ModifierKey.Ctrl),
            RawKey.CtrlF10 => (Key.F10, ModifierKey.Ctrl),
            RawKey.CtrlF11 => (Key.F11, ModifierKey.Ctrl),
            RawKey.CtrlF12 => (Key.F12, ModifierKey.Ctrl),
            RawKey.AltF1 => (Key.F1, ModifierKey.Alt),
            RawKey.AltF2 => (Key.F2, ModifierKey.Alt),
            RawKey.AltF3 => (Key.F3, ModifierKey.Alt),
            RawKey.AltF4 => (Key.F4, ModifierKey.Alt),
            RawKey.AltF5 => (Key.F5, ModifierKey.Alt),
            RawKey.AltF6 => (Key.F6, ModifierKey.Alt),
            RawKey.AltF7 => (Key.F7, ModifierKey.Alt),
            RawKey.AltF8 => (Key.F8, ModifierKey.Alt),
            RawKey.AltF9 => (Key.F9, ModifierKey.Alt),
            RawKey.AltF10 => (Key.F10, ModifierKey.Alt),
            RawKey.AltF11 => (Key.F11, ModifierKey.Alt),
            RawKey.AltF12 => (Key.F12, ModifierKey.Alt),
            RawKey.ShiftAltF1 => (Key.F1, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF2 => (Key.F2, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF3 => (Key.F3, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF4 => (Key.F4, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF5 => (Key.F5, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF6 => (Key.F6, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF7 => (Key.F7, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF8 => (Key.F8, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF9 => (Key.F9, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF10 => (Key.F10, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF11 => (Key.F11, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF12 => (Key.F12, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.Up => (Key.KeypadUp, ModifierKey.None),
            RawKey.Down => (Key.KeypadDown, ModifierKey.None),
            RawKey.Left => (Key.KeypadLeft, ModifierKey.None),
            RawKey.Right => (Key.KeypadRight, ModifierKey.None),
            RawKey.Home => (Key.KeypadHome, ModifierKey.None),
            RawKey.End => (Key.KeypadEnd, ModifierKey.None),
            RawKey.PageDown => (Key.KeypadPageDown, ModifierKey.None),
            RawKey.PageUp => (Key.KeypadPageUp, ModifierKey.None),
            RawKey.DeleteChar => (Key.DeleteChar, ModifierKey.None),
            RawKey.InsertChar => (Key.InsertChar, ModifierKey.None),
            RawKey.Tab => (Key.Tab, ModifierKey.None),
            RawKey.BackTab => (Key.Tab, ModifierKey.Shift),
            RawKey.Backspace => (Key.Backspace, ModifierKey.None),
            RawKey.ShiftUp => (Key.KeypadUp, ModifierKey.Shift),
            RawKey.ShiftDown => (Key.KeypadDown, ModifierKey.Shift),
            RawKey.ShiftLeft => (Key.KeypadLeft, ModifierKey.Shift),
            RawKey.ShiftRight => (Key.KeypadRight, ModifierKey.Shift),
            RawKey.ShiftHome => (Key.KeypadHome, ModifierKey.Shift),
            RawKey.ShiftEnd => (Key.KeypadEnd, ModifierKey.Shift),
            RawKey.ShiftPageDown => (Key.KeypadPageDown, ModifierKey.Shift),
            RawKey.ShiftPageUp => (Key.KeypadPageUp, ModifierKey.Shift),
            RawKey.AltUp => (Key.KeypadUp, ModifierKey.Alt),
            RawKey.AltDown => (Key.KeypadDown, ModifierKey.Alt),
            RawKey.AltLeft => (Key.KeypadLeft, ModifierKey.Alt),
            RawKey.AltRight => (Key.KeypadRight, ModifierKey.Alt),
            RawKey.AltHome => (Key.KeypadHome, ModifierKey.Alt),
            RawKey.AltEnd => (Key.KeypadEnd, ModifierKey.Alt),
            RawKey.AltPageDown => (Key.KeypadPageDown, ModifierKey.Alt),
            RawKey.AltPageUp => (Key.KeypadPageUp, ModifierKey.Alt),
            RawKey.CtrlUp => (Key.KeypadUp, ModifierKey.Ctrl),
            RawKey.CtrlDown => (Key.KeypadDown, ModifierKey.Ctrl),
            RawKey.CtrlLeft => (Key.KeypadLeft, ModifierKey.Ctrl),
            RawKey.CtrlRight => (Key.KeypadRight, ModifierKey.Ctrl),
            RawKey.CtrlHome => (Key.KeypadHome, ModifierKey.Ctrl),
            RawKey.CtrlEnd => (Key.KeypadEnd, ModifierKey.Ctrl),
            RawKey.CtrlPageDown => (Key.KeypadPageDown, ModifierKey.Ctrl),
            RawKey.CtrlPageUp => (Key.KeypadPageUp, ModifierKey.Ctrl),
            RawKey.ShiftCtrlUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftAltUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.AltCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlHome => (Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlEnd => (Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
            var _ => (Key.Unknown, ModifierKey.None)
        };
    }

    /// <summary>
    /// Converts a Curses mouse action into proper format.
    /// </summary>
    /// <param name="type">The Curses mouse event type.</param>
    /// <returns>The mouse action attributes.</returns>
    internal static (MouseButton button, MouseButtonState state, ModifierKey modifierKey) ConvertMouseActionEvent(
        RawMouseEvent.EventType type)
    {
        var modifierKey = ModifierKey.None;
        var button = MouseButton.Unknown;
        var state = MouseButtonState.Unknown;

        if (type.HasFlag(RawMouseEvent.EventType.Alt))
        {
            modifierKey |= ModifierKey.Alt;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Ctrl))
        {
            modifierKey |= ModifierKey.Ctrl;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Shift))
        {
            modifierKey |= ModifierKey.Shift;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Button1Released))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1Pressed))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1Clicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1DoubleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1TripleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Released))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Pressed))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Clicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2DoubleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2TripleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Released))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Pressed))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Clicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3DoubleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3TripleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Released))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Pressed))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Clicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4DoubleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4TripleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Released))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Pressed))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Clicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5DoubleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5TripleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.TripleClicked;
        }

        return (button, state, modifierKey);
    }
}
