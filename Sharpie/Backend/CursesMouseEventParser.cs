/*
Copyright (c) 2022-2025, Alexandru Ciobanu, Jordan Hemming
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

/// <summary>
///     Provides functionality for parsing Curses mouse events.
/// </summary>
internal abstract class CursesMouseEventParser
{
    private static readonly CursesMouseEventParser _nCurses5 = new CursesMouseV1EventParser();
    private static readonly CursesMouseEventParser _nCurses6 = new CursesMouseV2EventParser();
    private static readonly CursesMouseEventParser _pdCurses = new PdCursesModMouseEventParser();

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected CursesMouseEventParser()
    {
        Button1Released = CalculateButtonState(Action.Released, Button.Button1);
        Button1Pressed = CalculateButtonState(Action.Pressed, Button.Button1);
        Button1Clicked = CalculateButtonState(Action.Clicked, Button.Button1);
        Button1DoubleClicked = CalculateButtonState(Action.DoubleClicked, Button.Button1);
        Button1TripleClicked = CalculateButtonState(Action.TripleClicked, Button.Button1);

        Button2Released = CalculateButtonState(Action.Released, Button.Button2);
        Button2Pressed = CalculateButtonState(Action.Pressed, Button.Button2);
        Button2Clicked = CalculateButtonState(Action.Clicked, Button.Button2);
        Button2DoubleClicked = CalculateButtonState(Action.DoubleClicked, Button.Button2);
        Button2TripleClicked = CalculateButtonState(Action.TripleClicked, Button.Button2);

        Button3Released = CalculateButtonState(Action.Released, Button.Button3);
        Button3Pressed = CalculateButtonState(Action.Pressed, Button.Button3);
        Button3Clicked = CalculateButtonState(Action.Clicked, Button.Button3);
        Button3DoubleClicked = CalculateButtonState(Action.DoubleClicked, Button.Button3);
        Button3TripleClicked = CalculateButtonState(Action.TripleClicked, Button.Button3);

        Button4Released = CalculateButtonState(Action.Released, Button.Button4);
        Button4Pressed = CalculateButtonState(Action.Pressed, Button.Button4);
        Button4Clicked = CalculateButtonState(Action.Clicked, Button.Button4);
        Button4DoubleClicked = CalculateButtonState(Action.DoubleClicked, Button.Button4);
        Button4TripleClicked = CalculateButtonState(Action.TripleClicked, Button.Button4);

        Button5Released = CalculateButtonState(Action.Released, Button.Button5);
        Button5Pressed = CalculateButtonState(Action.Pressed, Button.Button5);
        Button5Clicked = CalculateButtonState(Action.Clicked, Button.Button5);
        Button5DoubleClicked = CalculateButtonState(Action.DoubleClicked, Button.Button5);
        Button5TripleClicked = CalculateButtonState(Action.TripleClicked, Button.Button5);

        Ctrl = CalculateModifierState(Modifier.Ctrl);
        Shift = CalculateModifierState(Modifier.Shift);
        Alt = CalculateModifierState(Modifier.Alt);
        ReportPosition = CalculateModifierState(Modifier.ReportPosition);

        All = ReportPosition - 1;
    }

    private uint Button1Released
    {
        get;
    }

    private uint Button1Pressed
    {
        get;
    }

    private uint Button1Clicked
    {
        get;
    }

    private uint Button1DoubleClicked
    {
        get;
    }
    private uint Button1TripleClicked
    {
        get;
    }

    private uint Button2Released
    {
        get;
    }

    private uint Button2Pressed
    {
        get;
    }

    private uint Button2Clicked
    {
        get;
    }

    private uint Button2DoubleClicked
    {
        get;
    }

    private uint Button2TripleClicked
    {
        get;
    }

    private uint Button3Released
    {
        get;
    }

    private uint Button3Pressed
    {
        get;
    }

    private uint Button3Clicked
    {
        get;
    }

    private uint Button3DoubleClicked
    {
        get;
    }

    private uint Button3TripleClicked
    {
        get;
    }

    private uint Button4Released
    {
        get;
    }

    private uint Button4Pressed
    {
        get;
    }

    private uint Button4Clicked
    {
        get;
    }

    private uint Button4DoubleClicked
    {
        get;
    }

    private uint Button4TripleClicked
    {
        get;
    }

    private uint Button5Released
    {
        get;
    }

    private uint Button5Pressed
    {
        get;
    }

    private uint Button5Clicked
    {
        get;
    }

    private uint Button5DoubleClicked
    {
        get;
    }

    private uint Button5TripleClicked
    {
        get;
    }

    private uint Ctrl
    {
        get;
    }

    private uint Shift
    {
        get;
    }

    private uint Alt
    {
        get;
    }

    public uint ReportPosition
    {
        get;
    }

    public uint All
    {
        get;
    }

    protected abstract uint CalculateButtonState(Action action, Button button);

    protected abstract uint CalculateModifierState(Modifier modifier);

    /// <summary>
    ///     Converts a Curses mouse action into proper format.
    /// </summary>
    /// <param name="flags">The raw Curses mouse event flags.</param>
    /// <returns>The mouse action attributes.</returns>
    public (MouseButton button, MouseButtonState state, ModifierKey modifierKey)? Parse(uint flags)
    {
        var modifierKey = ModifierKey.None;
        var button = (MouseButton) 0;
        var state = (MouseButtonState) 0;

        bool has(uint flag) => (flags & flag) == flag;

        void mapMod(uint flag, ModifierKey mod)
        {
            if (has(flag))
            {
                modifierKey |= mod;
            }
        }

        bool mapButton(uint flag, MouseButton b, MouseButtonState s)
        {
            var h = has(flag);
            if (h)
            {
                button = b;
                state = s;
            }

            return h;
        }

        mapMod(Alt, ModifierKey.Alt);
        mapMod(Ctrl, ModifierKey.Ctrl);
        mapMod(Shift, ModifierKey.Shift);

        return
            mapButton(Button1Released, MouseButton.Button1, MouseButtonState.Released) ||
            mapButton(Button1Pressed, MouseButton.Button1, MouseButtonState.Pressed) ||
            mapButton(Button1Clicked, MouseButton.Button1, MouseButtonState.Clicked) ||
            mapButton(Button1DoubleClicked, MouseButton.Button1, MouseButtonState.DoubleClicked) ||
            mapButton(Button1TripleClicked, MouseButton.Button1, MouseButtonState.TripleClicked) ||
            mapButton(Button2Released, MouseButton.Button2, MouseButtonState.Released) ||
            mapButton(Button2Pressed, MouseButton.Button2, MouseButtonState.Pressed) ||
            mapButton(Button2Clicked, MouseButton.Button2, MouseButtonState.Clicked) ||
            mapButton(Button2DoubleClicked, MouseButton.Button2, MouseButtonState.DoubleClicked) ||
            mapButton(Button2TripleClicked, MouseButton.Button2, MouseButtonState.TripleClicked) ||
            mapButton(Button3Released, MouseButton.Button3, MouseButtonState.Released) ||
            mapButton(Button3Pressed, MouseButton.Button3, MouseButtonState.Pressed) ||
            mapButton(Button3Clicked, MouseButton.Button3, MouseButtonState.Clicked) ||
            mapButton(Button3DoubleClicked, MouseButton.Button3, MouseButtonState.DoubleClicked) ||
            mapButton(Button3TripleClicked, MouseButton.Button3, MouseButtonState.TripleClicked) ||
            mapButton(Button4Released, MouseButton.Button4, MouseButtonState.Released) ||
            mapButton(Button4Pressed, MouseButton.Button4, MouseButtonState.Pressed) ||
            mapButton(Button4Clicked, MouseButton.Button4, MouseButtonState.Clicked) ||
            mapButton(Button4DoubleClicked, MouseButton.Button4, MouseButtonState.DoubleClicked) ||
            mapButton(Button4TripleClicked, MouseButton.Button4, MouseButtonState.TripleClicked) ||
            mapButton(Button5Released, MouseButton.Button5, MouseButtonState.Released) ||
            mapButton(Button5Pressed, MouseButton.Button5, MouseButtonState.Pressed) ||
            mapButton(Button5Clicked, MouseButton.Button5, MouseButtonState.Clicked) ||
            mapButton(Button5DoubleClicked, MouseButton.Button5, MouseButtonState.DoubleClicked) ||
            mapButton(Button5TripleClicked, MouseButton.Button5, MouseButtonState.TripleClicked)
            ? (button, state, modifierKey)
            : null;
    }

    /// <summary>
    ///     Gets the mouse event parser based on the provided ABI version.
    /// </summary>
    /// <param name="abiVersion">The ABI version.</param>
    /// <returns>The mouse event parser.</returns>
    [Obsolete("Use the overload that takes CursesAbiVersion instead.")]
    public static CursesMouseEventParser Get(int abiVersion) =>
        abiVersion switch
        {
            3 => _pdCurses,
            2 => _nCurses6,
            var _ => _nCurses5
        };

    /// <summary>
    ///     Gets the mouse event parser based on the provided ABI version.
    /// </summary>
    /// <param name="abiVersion">The ABI version.</param>
    /// <returns>The mouse event parser.</returns>

    public static CursesMouseEventParser Get(CursesAbiVersion abiVersion) =>
#pragma warning disable IDE0072 // Add missing cases -- this is intentional
        abiVersion switch
        {
            CursesAbiVersion.PdCurses => _pdCurses,
            CursesAbiVersion.NCurses6 => _nCurses6,
            var _ => _nCurses5
        };
#pragma warning restore IDE0072 // Add missing cases

    private sealed class CursesMouseV1EventParser: CursesMouseEventParser
    {
        protected override uint CalculateButtonState(Action action, Button button) =>
#pragma warning disable IDE0072 // Add missing cases -- this is intentional
            button switch
            {
                Button.Button5 => 1u << 31,
                var _ => (uint) action << (((int) button - 1) * 6)
            };
#pragma warning restore IDE0072 // Add missing cases

        protected override uint CalculateModifierState(Modifier modifier) => (uint) modifier << 24;
    }

    private sealed class CursesMouseV2EventParser: CursesMouseEventParser
    {
        protected override uint CalculateButtonState(Action action, Button button) =>
            (uint) action << (((int) button - 1) * 5);

        protected override uint CalculateModifierState(Modifier modifier) => (uint) modifier << 25;
    }

    private sealed class PdCursesModMouseEventParser: CursesMouseEventParser
    {
        protected override uint CalculateButtonState(Action action, Button button) =>
            (uint) action << (((int) button - 1) * 5);

        protected override uint CalculateModifierState(Modifier modifier)
        {
            return modifier switch
            {
                Modifier.Shift => 1 << 26,
                Modifier.Ctrl => 1 << 27,
                Modifier.Alt => 1 << 28,
                Modifier.ReportPosition => 1 << 29,
                _ => 0
            };
        }
    }

    protected enum Action
    {
        Released = 1,
        Pressed = Released << 1,
        Clicked = Pressed << 1,
        DoubleClicked = Clicked << 1,
        TripleClicked = DoubleClicked << 1
    }

    protected enum Modifier
    {
        Ctrl = 1,
        Shift = Ctrl << 1,
        Alt = Shift << 1,
        ReportPosition = Alt << 1
    }

    protected enum Button
    {
        Button1 = 1,
        Button2 = 2,
        Button3 = 3,
        Button4 = 4,
        Button5 = 5
    }
}
