#pragma warning disable CS1591

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
    internal NCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver nCursesSymbolResolver):
        base(dotNetSystemAdapter, nCursesSymbolResolver)
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
                var abi = -1;
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
                            >= 6 => 2,
                            5 => 1,
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
    protected internal override (VideoAttribute attributtes, short colorPair) DecodeCursesAttributes(uint attrs) =>
        ((VideoAttribute) (attrs >> 16), (short) ((attrs >> 8) & 0xFF));

    /// <inheritdoc cref="BaseCursesBackend.DecodeKeyCodeType" />
    protected internal override CursesKeyCodeType DecodeKeyCodeType(int result, uint keyCode)
    {
        return (result, keyCode) switch
        {
            (< 0, var _) => CursesKeyCodeType.Unknown,
            ((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Resize) => CursesKeyCodeType.Resize,
            ((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Mouse) => CursesKeyCodeType.Mouse,
            ((int) NCursesKeyCode.Yes, var _) => CursesKeyCodeType.Key,
            (>= 0, var _) => CursesKeyCodeType.Character
        };
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeRawKey" />
    protected internal override (Key key, ModifierKey modifierKey) DecodeRawKey(uint keyCode)
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

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

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
