#pragma warning disable CS1591

namespace Sharpie.Backend;

/// <summary>
///     PDCurses-specific backend implementation.
/// </summary>
[PublicAPI]
internal class PdCursesBackend: BaseCursesBackend
{
    [Flags]
    private enum PdVideoAttribute: uint
    {
        Normal = 0,
        AltCharset = 0x00010000,
        RightHighlight = 0x00020000,
        LeftHighlight = 0x00040000,
        Italic = 0x00080000,
        Underline = 0x00100000,
        Reverse = 0x00200000,
        Blink = 0x00400000,
        Bold = 0x00800000,
        StandOut = Reverse | Bold,
    }

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system adapter.</param>
    /// <param name="pdCursesSymbolResolver">The PDCurses library symbol resolver.</param>
    /// <param name="libCSymbolResolver">The LibC symbol resolver.</param>
    internal PdCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, 
        INativeSymbolResolver pdCursesSymbolResolver, INativeSymbolResolver? libCSymbolResolver):
        base(dotNetSystemAdapter, pdCursesSymbolResolver, libCSymbolResolver) =>
        CursesMouseEventParser = CursesMouseEventParser.Get(2);

    /// <inheritdoc cref="BaseCursesBackend.CursesMouseEventParser" />
    protected internal override CursesMouseEventParser CursesMouseEventParser { get; }

    /// <inheritdoc cref="BaseCursesBackend.EncodeCursesAttribute" />
    protected internal override uint EncodeCursesAttribute(VideoAttribute attributes, short colorPair)
    {
        var pdc = PdVideoAttribute.Normal;
        if (attributes.HasFlag(VideoAttribute.StandOut))
        {
            pdc |= PdVideoAttribute.StandOut;
        }
        if (attributes.HasFlag(VideoAttribute.Underline))
        {
            pdc |= PdVideoAttribute.Underline;
        }
        if (attributes.HasFlag(VideoAttribute.Reverse))
        {
            pdc |= PdVideoAttribute.Reverse;
        }
        if (attributes.HasFlag(VideoAttribute.Blink))
        {
            pdc |= PdVideoAttribute.Blink;
        }
        if (attributes.HasFlag(VideoAttribute.Bold))
        {
            pdc |= PdVideoAttribute.Bold;
        }
        if (attributes.HasFlag(VideoAttribute.AltCharset))
        {
            pdc |= PdVideoAttribute.AltCharset;
        }
        if (attributes.HasFlag(VideoAttribute.LeftHighlight))
        {
            pdc |= PdVideoAttribute.LeftHighlight;
        }
        if (attributes.HasFlag(VideoAttribute.RightHighlight))
        {
            pdc |= PdVideoAttribute.RightHighlight;
        }
        if (attributes.HasFlag(VideoAttribute.Italic))
        {
            pdc |= PdVideoAttribute.Italic;
        }
        
        return (uint) pdc | (((uint) colorPair & 0xFF) << 24);
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeCursesAttributes" />
    protected internal override (VideoAttribute attributes, short colorPair) DecodeCursesAttributes(uint attrs)
    {
        var pdc = (PdVideoAttribute)(attrs & 0x00FF0000);
        var at = VideoAttribute.None;
        
        if (pdc.HasFlag(PdVideoAttribute.StandOut))
        {
            at |= VideoAttribute.StandOut;
        }
        if (pdc.HasFlag(PdVideoAttribute.Underline))
        {
            at |= VideoAttribute.Underline;
        }
        if (pdc.HasFlag(PdVideoAttribute.Reverse) && !pdc.HasFlag(PdVideoAttribute.Bold))
        {
            at |= VideoAttribute.Reverse;
        }
        if (pdc.HasFlag(PdVideoAttribute.Blink))
        {
            at |= VideoAttribute.Blink;
        }
        if (pdc.HasFlag(PdVideoAttribute.Bold) && !pdc.HasFlag(PdVideoAttribute.Reverse))
        {
            at |= VideoAttribute.Bold;
        }
        if (pdc.HasFlag(PdVideoAttribute.AltCharset))
        {
            at |= VideoAttribute.AltCharset;
        }
        if (pdc.HasFlag(PdVideoAttribute.LeftHighlight))
        {
            at |= VideoAttribute.LeftHighlight;
        }
        if (pdc.HasFlag(PdVideoAttribute.RightHighlight))
        {
            at |= VideoAttribute.RightHighlight;
        }
        if (pdc.HasFlag(PdVideoAttribute.Italic))
        {
            at |= VideoAttribute.Italic;
        }
        
        return (at, (short) (attrs >> 24));
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeKeyCodeType" />
    protected internal override CursesKeyCodeType DecodeKeyCodeType(int result, uint keyCode)
    {
        return (result, keyCode) switch
        {
            (< 0, var _) => CursesKeyCodeType.Unknown,
            ((int) PdCursesKeyCode.Yes, (uint) PdCursesKeyCode.Resize) => CursesKeyCodeType.Resize,
            ((int) PdCursesKeyCode.Yes, (uint) PdCursesKeyCode.Mouse) => CursesKeyCodeType.Mouse,
            ((int) PdCursesKeyCode.Yes, var _) => CursesKeyCodeType.Key,
            (>= 0, var _) => CursesKeyCodeType.Character
        };
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeRawKey" />
    protected internal override (Key key, char @char, ModifierKey modifierKey) DecodeRawKey(uint keyCode)
    {
        return (PdCursesKeyCode)keyCode switch
        {
            PdCursesKeyCode.F1 => (Key.F1, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F2 => (Key.F2, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F3 => (Key.F3, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F4 => (Key.F4, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F5 => (Key.F5, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F6 => (Key.F6, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F7 => (Key.F7, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F8 => (Key.F8, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F9 => (Key.F9, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F10 => (Key.F10, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F11 => (Key.F11, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.F12 => (Key.F12, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.ShiftF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.CtrlF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.AltF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.ShiftAltF1 => (Key.F1, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF2 => (Key.F2, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF3 => (Key.F3, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF4 => (Key.F4, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF5 => (Key.F5, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF6 => (Key.F6, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF7 => (Key.F7, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF8 => (Key.F8, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF9 => (Key.F9, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF10 => (Key.F10, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF11 => (Key.F11, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.ShiftAltF12 => (Key.F12, ControlCharacter.Null, ModifierKey.Alt | ModifierKey.Shift),
            PdCursesKeyCode.Up => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Down => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Left => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Right => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Home => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.End => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.PageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.PageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Delete => (Key.Delete, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Insert => (Key.Insert, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.BackTab => (Key.Tab, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.Backspace => (Key.Backspace, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.ShiftUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.ShiftEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.AltUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltPageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.CtrlUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlLeft => (Key.KeypadLeft, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlRight => (Key.KeypadRight, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlHome => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlEnd => (Key.KeypadEnd, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlPageDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.CtrlPageUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.ScrollDown => (Key.KeypadPageDown, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.ScrollUp => (Key.KeypadPageUp, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.Alt0 => (Key.Character, '0', ModifierKey.Alt),
            PdCursesKeyCode.Alt1 => (Key.Character, '1', ModifierKey.Alt),
            PdCursesKeyCode.Alt2 => (Key.Character, '2', ModifierKey.Alt),
            PdCursesKeyCode.Alt3 => (Key.Character, '3', ModifierKey.Alt),
            PdCursesKeyCode.Alt4 => (Key.Character, '4', ModifierKey.Alt),
            PdCursesKeyCode.Alt5 => (Key.Character, '5', ModifierKey.Alt),
            PdCursesKeyCode.Alt6 => (Key.Character, '6', ModifierKey.Alt),
            PdCursesKeyCode.Alt7 => (Key.Character, '7', ModifierKey.Alt),
            PdCursesKeyCode.Alt8 => (Key.Character, '8', ModifierKey.Alt),
            PdCursesKeyCode.Alt9 => (Key.Character, '9', ModifierKey.Alt),
            PdCursesKeyCode.AltA => (Key.Character, 'A', ModifierKey.Alt),
            PdCursesKeyCode.AltB => (Key.Character, 'B', ModifierKey.Alt),
            PdCursesKeyCode.AltC => (Key.Character, 'C', ModifierKey.Alt),
            PdCursesKeyCode.AltD => (Key.Character, 'D', ModifierKey.Alt),
            PdCursesKeyCode.AltE => (Key.Character, 'E', ModifierKey.Alt),
            PdCursesKeyCode.AltF => (Key.Character, 'F', ModifierKey.Alt),
            PdCursesKeyCode.AltG => (Key.Character, 'G', ModifierKey.Alt),
            PdCursesKeyCode.AltH => (Key.Character, 'H', ModifierKey.Alt),
            PdCursesKeyCode.AltI => (Key.Character, 'I', ModifierKey.Alt),
            PdCursesKeyCode.AltJ => (Key.Character, 'J', ModifierKey.Alt),
            PdCursesKeyCode.AltK => (Key.Character, 'K', ModifierKey.Alt),
            PdCursesKeyCode.AltL => (Key.Character, 'L', ModifierKey.Alt),
            PdCursesKeyCode.AltM => (Key.Character, 'M', ModifierKey.Alt),
            PdCursesKeyCode.AltN => (Key.Character, 'N', ModifierKey.Alt),
            PdCursesKeyCode.AltO => (Key.Character, 'O', ModifierKey.Alt),
            PdCursesKeyCode.AltP => (Key.Character, 'P', ModifierKey.Alt),
            PdCursesKeyCode.AltQ => (Key.Character, 'Q', ModifierKey.Alt),
            PdCursesKeyCode.AltR => (Key.Character, 'R', ModifierKey.Alt),
            PdCursesKeyCode.AltS => (Key.Character, 'S', ModifierKey.Alt),
            PdCursesKeyCode.AltT => (Key.Character, 'T', ModifierKey.Alt),
            PdCursesKeyCode.AltU => (Key.Character, 'U', ModifierKey.Alt),
            PdCursesKeyCode.AltV => (Key.Character, 'V', ModifierKey.Alt),
            PdCursesKeyCode.AltW => (Key.Character, 'W', ModifierKey.Alt),
            PdCursesKeyCode.AltX => (Key.Character, 'X', ModifierKey.Alt),
            PdCursesKeyCode.AltY => (Key.Character, 'Y', ModifierKey.Alt),
            PdCursesKeyCode.AltZ => (Key.Character, 'Z', ModifierKey.Alt),
            PdCursesKeyCode.KeypadSlash => (Key.Character, '/', ModifierKey.None),
            PdCursesKeyCode.KeypadEnter => (Key.Character, '\n', ModifierKey.None),
            PdCursesKeyCode.KeypadCtrlEnter => (Key.Character, '\n', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadAltEnter => (Key.Character, '\n', ModifierKey.Alt),
            PdCursesKeyCode.KeypadStop => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.None),
            PdCursesKeyCode.KeypadAsterisk => (Key.Character, '*', ModifierKey.None),
            PdCursesKeyCode.KeypadMinus => (Key.Character, '-', ModifierKey.None),
            PdCursesKeyCode.KeypadPlus => (Key.Character, '+', ModifierKey.None),
            PdCursesKeyCode.KeypadCtrlStop => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadCtrlMiddle => (Key.Character, '5', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadCtrlPlus => (Key.Character, '+', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadCtrlMinus => (Key.Character, '-', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadCtrlSlash => (Key.Character, '/', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadCtrlAsterisk => (Key.Character, '*', ModifierKey.Ctrl),
            PdCursesKeyCode.KeypadAltPlus => (Key.Character, '+', ModifierKey.Alt),
            PdCursesKeyCode.KeypadAltMinus => (Key.Character, '-', ModifierKey.Alt),
            PdCursesKeyCode.KeypadAltSlash => (Key.Character, '/', ModifierKey.Alt),
            PdCursesKeyCode.KeypadAltAsterisk => (Key.Character, '*', ModifierKey.Alt),
            PdCursesKeyCode.KeypadAltStop => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.CtrlInsert => (Key.Insert, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.AltDelete => (Key.Delete, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltInsert => (Key.Insert, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.CtrlTab => (Key.Tab, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.AltTab => (Key.Tab, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltMinus => (Key.Character, '-', ModifierKey.Alt),
            PdCursesKeyCode.AltEqual => (Key.Character, '=', ModifierKey.Alt),
            PdCursesKeyCode.AltPageDown => (Key.Character, '+', ModifierKey.Alt),
            PdCursesKeyCode.AltEnter => (Key.Character, '\n', ModifierKey.Alt),
            PdCursesKeyCode.AltEscape => (Key.Escape, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltBackQuote => (Key.Character, '\'', ModifierKey.Alt),
            PdCursesKeyCode.AltLeftBracket => (Key.Character, '(', ModifierKey.Alt),
            PdCursesKeyCode.AltRightBracket => (Key.Character, ')', ModifierKey.Alt),
            PdCursesKeyCode.AltSemicolon => (Key.Character, ';', ModifierKey.Alt),
            PdCursesKeyCode.AltForwardQuote => (Key.Character, '`', ModifierKey.Alt),
            PdCursesKeyCode.AltComma => (Key.Character, ',', ModifierKey.Alt),
            PdCursesKeyCode.AltStop => (Key.KeypadHome, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.AltForwardSlash => (Key.Character, '\\', ModifierKey.Alt),
            PdCursesKeyCode.AltBackspace => (Key.Character, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.CtrlBackspace => (Key.Backspace, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPad0 => (Key.Character, '0', ModifierKey.None),
            PdCursesKeyCode.KeyPadCtrl0 => (Key.Character, '0', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl1 => (Key.Character, '1', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl2 => (Key.Character, '2', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl3 => (Key.Character, '3', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl4 => (Key.Character, '4', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl5 => (Key.Character, '5', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl6 => (Key.Character, '6', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl7 => (Key.Character, '7', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl8 => (Key.Character, '8', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadCtrl9 => (Key.Character, '9', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadAlt0 => (Key.Character, '0', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt1 => (Key.Character, '1', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt2 => (Key.Character, '2', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt3 => (Key.Character, '3', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt4 => (Key.Character, '4', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt5 => (Key.Character, '5', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt6 => (Key.Character, '6', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt7 => (Key.Character, '7', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt8 => (Key.Character, '8', ModifierKey.Alt),
            PdCursesKeyCode.KeyPadAlt9 => (Key.Character, '9', ModifierKey.Alt),
            PdCursesKeyCode.CtrlDelete => (Key.Delete, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.AltBackSlash => (Key.Character, '\\', ModifierKey.Alt),
            PdCursesKeyCode.CtrlEnter => (Key.Character, '\n', ModifierKey.Ctrl),
            PdCursesKeyCode.KeyPadShiftEnter => (Key.Character, '\n', ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftPlus => (Key.Character, '+', ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftMinus => (Key.Character, '-', ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftSlash => (Key.Character, '/', ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftAsterisk => (Key.Character, '*', ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftUp => (Key.KeypadUp, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftDown => (Key.KeypadDown, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftInsert => (Key.Insert, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.KeyPadShiftDelete => (Key.Delete, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.LeftShift => (Key.Character, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.RightShift => (Key.Character, ControlCharacter.Null, ModifierKey.Shift),
            PdCursesKeyCode.LeftCtrl => (Key.Character, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.RightCtrl => (Key.Character, ControlCharacter.Null, ModifierKey.Ctrl),
            PdCursesKeyCode.LeftAlt => (Key.Character, ControlCharacter.Null, ModifierKey.Alt),
            PdCursesKeyCode.RightAlt => (Key.Character, ControlCharacter.Null, ModifierKey.Alt),

            var _ => (Key.Unknown, ControlCharacter.Null, ModifierKey.None)
        };
    }

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public override int endwin() => CursesSymbolResolver.Resolve<PdCursesFunctionMap.endwin>()();

    public override int getmouse(out CursesMouseState state) =>
        CursesSymbolResolver.Resolve<PdCursesFunctionMap.getmouse>()(out state);

    public override int slk_attr_off(VideoAttribute attributes, IntPtr reserved) => Helpers.CursesErrorResult;

    public override int slk_attr_on(VideoAttribute attributes, IntPtr reserved) => Helpers.CursesErrorResult;

    public override int slk_attr(out VideoAttribute attributes, out short colorPair)
    {
        attributes = VideoAttribute.None;
        colorPair = 0;
        
        return Helpers.CursesErrorResult;
    }

    public override int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved) => Helpers.CursesErrorResult;

    public override int slk_clear() => Helpers.CursesErrorResult;

    public override int slk_color(short colorPair) => Helpers.CursesErrorResult;

    public override int slk_set(int labelIndex, string title, int align) => Helpers.CursesErrorResult;

    public override int slk_init(int format) => Helpers.CursesErrorResult;

    public override int slk_noutrefresh() => Helpers.CursesErrorResult;

    public override int slk_refresh() => Helpers.CursesErrorResult;

    public override int slk_restore() => Helpers.CursesErrorResult;

    public override int slk_touch() => Helpers.CursesErrorResult;

    public override int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes,
        out short colorPair, IntPtr reserved)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        var ret = CursesSymbolResolver.Resolve<PdCursesFunctionMap.getcchar>()(ref c[0], dest, out var attrs,
            out colorPair, reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public override int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesFunctionMap.setcchar>()(out c[0], text,
            EncodeCursesAttribute(attributes, 0), colorPair, reserved);

        @char = new(c[0]);
        return ret;
    }

    public override int wadd_wch(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesFunctionMap.wadd_wch>()(window, ref c[0]);
    }

    public override int wbkgrnd(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesFunctionMap.wbkgrnd>()(window, ref c[0]);
    }

    public override int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide, ComplexChar topSide,
        ComplexChar bottomSide, ComplexChar topLeftCorner, ComplexChar topRightCorner, ComplexChar bottomLeftCorner,
        ComplexChar bottomRightCorner)
    {
        var c = new[]
        {
            leftSide.GetRawValue<uint>(),
            rightSide.GetRawValue<uint>(),
            topSide.GetRawValue<uint>(),
            bottomSide.GetRawValue<uint>(),
            topLeftCorner.GetRawValue<uint>(),
            topRightCorner.GetRawValue<uint>(),
            bottomLeftCorner.GetRawValue<uint>(),
            bottomRightCorner.GetRawValue<uint>()
        };

        return CursesSymbolResolver.Resolve<PdCursesFunctionMap.wborder_set>()(window, ref c[0], ref c[1], ref c[2],
            ref c[3], ref c[4], ref c[5], ref c[6], ref c[7]);
    }

    public override int wgetbkgrnd(IntPtr window, out ComplexChar @char)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesFunctionMap.wgetbkgrnd>()(window, out c[0]);
        @char = new(c[0]);

        return ret;
    }

    public override int whline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesFunctionMap.whline_set>()(window, ref c[0], count);
    }

    public override int win_wch(IntPtr window, out ComplexChar @char)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesFunctionMap.win_wch>()(window, out c[0]);
        @char = new(c[0]);
        return ret;
    }

    public override int wvline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesFunctionMap.wvline_set>()(window, ref c[0], count);
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
