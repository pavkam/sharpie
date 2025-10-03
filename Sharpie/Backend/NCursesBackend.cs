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

using System.Text.RegularExpressions;

/// <summary>
///     NCurses-specific backend implementation.
/// </summary>
[PublicAPI]
internal class NCursesBackend: BaseCursesBackend
{
    private CursesMouseEventParser? _cursesMouseEventParser;

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system adapter.</param>
    /// <param name="nCursesSymbolResolver">The NCurses library symbol resolver.</param>
    /// <param name="libCSymbolResolver">The LibC symbol resolver.</param>
    internal NCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver nCursesSymbolResolver,
        INativeSymbolResolver? libCSymbolResolver) : base(dotNetSystemAdapter, nCursesSymbolResolver, libCSymbolResolver)
    {
    }

    /// <inheritdoc cref="BaseCursesBackend.CursesMouseEventParser" />
    protected internal override CursesMouseEventParser CursesMouseEventParser
    {
        get
        {
            if (_cursesMouseEventParser == null)
            {
                var ver = curses_version();
                var abi = CursesAbiVersion.NCurses5;
                if (ver != null)
                {
                    var versionParser = new Regex(@".*(\d+)\.(\d+)\.(\d+)");
                    var match = versionParser.Match(ver);
                    if (match.Success)
                    {
                        var major = int.Parse(match.Groups[1]
                                                   .Value);

                        abi = major switch
                        {
                            >= 6 => CursesAbiVersion.NCurses6,
                            5 => CursesAbiVersion.NCurses5,
                            var _ => abi
                        };
                    }
                }

                _cursesMouseEventParser = CursesMouseEventParser.Get(abi);
            }

            return _cursesMouseEventParser;
        }
    }

    /// <inheritdoc cref="BaseCursesBackend.EncodeCursesAttribute" />
    protected internal override uint EncodeCursesAttribute(VideoAttribute attributes, short colorPair) =>
        ((uint) attributes << 16) | (((uint) colorPair & 0xFF) << 8);

    /// <inheritdoc cref="BaseCursesBackend.DecodeCursesAttributes" />
    protected internal override (VideoAttribute attributes, short colorPair) DecodeCursesAttributes(uint attrs) =>
        ((VideoAttribute) (attrs >> 16), (short) ((attrs >> 8) & 0xFF));

    /// <inheritdoc cref="BaseCursesBackend.DecodeKeyCodeType" />
    protected internal override CursesKeyCodeType DecodeKeyCodeType(int result, uint keyCode)
    {
        return (result, keyCode) switch
        {
            ( < 0, var _) => CursesKeyCodeType.Unknown,
            ((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Resize) => CursesKeyCodeType.Resize,
            ((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Mouse) => CursesKeyCodeType.Mouse,
            ((int) NCursesKeyCode.Yes, var _) => CursesKeyCodeType.Key,
            ( >= 0, var _) => CursesKeyCodeType.Character,
            _ => throw new NotImplementedException()
        };
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeRawKey" />
    protected internal override (Key key, char @char, ModifierKey modifierKey) DecodeRawKey(uint keyCode)
    {
        return (NCursesKeyCode) keyCode switch
        {
            NCursesKeyCode.F1 => (Key.F1, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F2 => (Key.F2, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F3 => (Key.F3, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F4 => (Key.F4, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F5 => (Key.F5, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F6 => (Key.F6, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F7 => (Key.F7, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F8 => (Key.F8, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F9 => (Key.F9, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F10 => (Key.F10, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F11 => (Key.F11, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.F12 => (Key.F12, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.ShiftF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.CtrlF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.AltF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.ShiftAltF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.ShiftAltF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            NCursesKeyCode.Up => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Down => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Left => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Right => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Home => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.End => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.PageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.PageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Delete => (Key.Delete, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.Insert => (Key.Insert, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.BackTab => (Key.Tab, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.Backspace => (Key.Backspace, ControlCharacter.Null, ModifierKey.None),
            NCursesKeyCode.ShiftUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftPageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.ShiftPageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.Shift),
            NCursesKeyCode.AltUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltPageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.AltPageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.Alt),
            NCursesKeyCode.CtrlUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlPageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.CtrlPageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlDown => (Key.KeypadDown, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlLeft => (Key.KeypadLeft, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlRight => (Key.KeypadRight, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlHome => (Key.KeypadHome, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlPageDown => (Key.KeypadPageDown, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftCtrlPageUp => (Key.KeypadPageUp, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Ctrl),
            NCursesKeyCode.ShiftAltUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltRight => (Key.KeypadRight, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltPageDown => (Key.KeypadPageDown, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltPageUp => (Key.KeypadPageUp, ControlCharacter.Null,
                ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.ShiftAltEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Shift | ModifierKey.Alt),
            NCursesKeyCode.AltCtrlPageDown => (Key.KeypadPageDown, ControlCharacter.Null,
                ModifierKey.Alt | ModifierKey.Ctrl),
            NCursesKeyCode.AltCtrlPageUp => (Key.KeypadPageUp, ControlCharacter.Null,
                ModifierKey.Alt | ModifierKey.Ctrl),
            NCursesKeyCode.AltCtrlHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Ctrl),
            NCursesKeyCode.AltCtrlEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Ctrl),
            NCursesKeyCode.Yes => throw new NotImplementedException(),
            NCursesKeyCode.Mouse => throw new NotImplementedException(),
            NCursesKeyCode.Resize => throw new NotImplementedException(),
            var _ => (Key.Unknown, ControlCharacter.Null, ModifierKey.None)
        };
    }

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public override bool is_immedok(IntPtr window) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.is_immedok>()(window);

    public override bool is_scrollok(IntPtr window) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.is_scrollok>()(window);

    public override int endwin() => CursesSymbolResolver.Resolve<NCursesFunctionMap.endwin>()();

    public override int getmouse(out CursesMouseState state) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.getmouse>()(out state);

    public override int slk_attr_off(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_off>()(EncodeCursesAttribute(attributes, 0), reserved);

    public override int slk_attr_on(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_on>()(EncodeCursesAttribute(attributes, 0), reserved);

    public override int slk_attr(out VideoAttribute attributes, out short colorPair)
    {
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            colorPair = 0;

            return ret;
        }

        (attributes, colorPair) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public override int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_set>()(EncodeCursesAttribute(attributes, 0), colorPair,
            reserved);

    public override int slk_clear() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_clear>()();

    public override int slk_color(short colorPair) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_color>()(colorPair);

    public override int slk_set(int labelIndex, string title, int align) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_set>()(labelIndex, title, align);

    public override int slk_init(int format) => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_init>()(format);

    public override int slk_noutrefresh() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_noutrefresh>()();

    public override int slk_refresh() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_refresh>()();

    public override int slk_restore() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_restore>()();

    public override int slk_touch() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_touch>()();

    public override int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes,
        out short colorPair, IntPtr reserved)
    {
        var c = new[] { @char.GetRawValue<NCursesComplexChar>() };

        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.getcchar>()(ref c[0], dest, out var attrs,
            out colorPair, reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public override int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved)
    {
        var c = new NCursesComplexChar[1];

        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.setcchar>()(out c[0], text,
            EncodeCursesAttribute(attributes, 0), colorPair, reserved);

        @char = new(c[0]);
        return ret;
    }

    public override int wadd_wch(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<NCursesComplexChar>() };

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wadd_wch>()(window, ref c[0]);
    }

    public override int wbkgrnd(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<NCursesComplexChar>() };

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgrnd>()(window, ref c[0]);
    }

    public override int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide, ComplexChar topSide,
        ComplexChar bottomSide, ComplexChar topLeftCorner, ComplexChar topRightCorner, ComplexChar bottomLeftCorner,
        ComplexChar bottomRightCorner)
    {
        var c = new[]
        {
            leftSide.GetRawValue<NCursesComplexChar>(),
            rightSide.GetRawValue<NCursesComplexChar>(),
            topSide.GetRawValue<NCursesComplexChar>(),
            bottomSide.GetRawValue<NCursesComplexChar>(),
            topLeftCorner.GetRawValue<NCursesComplexChar>(),
            topRightCorner.GetRawValue<NCursesComplexChar>(),
            bottomLeftCorner.GetRawValue<NCursesComplexChar>(),
            bottomRightCorner.GetRawValue<NCursesComplexChar>()
        };

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wborder_set>()(window, ref c[0], ref c[1], ref c[2],
            ref c[3], ref c[4], ref c[5], ref c[6], ref c[7]);
    }

    public override int wgetbkgrnd(IntPtr window, out ComplexChar @char)
    {
        var c = new NCursesComplexChar[1];

        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.wgetbkgrnd>()(window, out c[0]);
        @char = new(c[0]);

        return ret;
    }

    public override int whline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<NCursesComplexChar>() };

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.whline_set>()(window, ref c[0], count);
    }

    public override int win_wch(IntPtr window, out ComplexChar @char)
    {
        var c = new NCursesComplexChar[1];

        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.win_wch>()(window, out c[0]);
        @char = new(c[0]);
        return ret;
    }

    public override int wvline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<NCursesComplexChar>() };

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wvline_set>()(window, ref c[0], count);
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
