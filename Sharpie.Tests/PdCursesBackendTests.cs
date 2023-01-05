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

namespace Sharpie.Tests;

[TestClass, SuppressMessage("ReSharper", "IdentifierTypo")]
public class PdCursesBackendTests
{
    private PdCursesBackend _backend = null!;
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;

    private static (ComplexChar, uint) MakeTestComplexChar(uint x = 1) => (new(x), x);

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();
        _nativeSymbolResolverMock = new();

        _backend = new(_dotNetSystemAdapterMock.Object, _nativeSymbolResolverMock.Object, null);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void endwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.endwin, int>(s => s(), ret);

        _backend.endwin()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmouse_IsRelayedToLibrary(int ret)
    {
        var exp = new CursesMouseState { id = 199 };
        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.getmouse>()
                                 .Setup(s => s(out It.Ref<CursesMouseState>.IsAny))
                                 .Returns((out CursesMouseState o) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });


        _backend.getmouse(out var x)
                .ShouldBe(ret);

        x.ShouldBe(exp);
    }

    [TestMethod]
    public void slk_clear_ReturnsError()
    {
        _backend.slk_clear()
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_noutrefresh_ReturnsError()
    {
        _backend.slk_noutrefresh()
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_refresh_ReturnsError()
    {
        _backend.slk_refresh()
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_restore_ReturnsError()
    {
        _backend.slk_restore()
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_touch_ReturnsError()
    {
        _backend.slk_touch()
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_init_ReturnsError()
    {
        _backend.slk_init(1)
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_color_ReturnsError()
    {
        _backend.slk_color(1)
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_attr_ReturnsError()
    {
        _backend.slk_attr(out var _, out var _)
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_attr_set_ReturnsError()
    {
        _backend.slk_attr_set(VideoAttribute.Blink, 2, new(2))
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_set_ReturnsError()
    {
        _backend.slk_set(3, "title", 90)
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_attr_on_ReturnsError()
    {
        _backend.slk_attr_on(VideoAttribute.Blink, new(2))
                .ShouldBe(-1);
    }

    [TestMethod]
    public void slk_attr_off_ReturnsError()
    {
        _backend.slk_attr_off(VideoAttribute.Blink, new(2))
                .ShouldBe(-1);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wadd_wch_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.wadd_wch, int>(s => s(new(1), ref nc), ret);

        _backend.wadd_wch(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wbkgrnd_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.wbkgrnd, int>(s => s(new(1), ref nc), ret);

        _backend.wbkgrnd(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_set_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.wvline_set, int>(s => s(new(1), ref nc, 4), ret);

        _backend.wvline_set(new(1), ch, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_set_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.whline_set, int>(s => s(new(1), ref nc, 4), ret);

        _backend.whline_set(new(1), ch, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wborder_set_IsRelayedToLibrary(int ret)
    {
        var (ch1, nc1) = MakeTestComplexChar();
        var (ch2, nc2) = MakeTestComplexChar(2);
        var (ch3, nc3) = MakeTestComplexChar(3);
        var (ch4, nc4) = MakeTestComplexChar(4);
        var (ch5, nc5) = MakeTestComplexChar(5);
        var (ch6, nc6) = MakeTestComplexChar(6);
        var (ch7, nc7) = MakeTestComplexChar(7);
        var (ch8, nc8) = MakeTestComplexChar(8);

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.wborder_set, int>(
            s => s(new(1), ref nc1, ref nc2, ref nc3, ref nc4,
                ref nc5, ref nc6, ref nc7, ref nc8), ret);

        _backend.wborder_set(new(1), ch1, ch2, ch3, ch4,
                    ch5, ch6, ch7, ch8)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wgetbkgrnd_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.wgetbkgrnd>()
                                 .Setup(s => s(new(1), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint o) =>
                                 {
                                     o = nc;
                                     return ret;
                                 });


        _backend.wgetbkgrnd(new(1), out var x)
                .ShouldBe(ret);

        x.ShouldBe(ch);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void win_wch_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.win_wch>()
                                 .Setup(s => s(new(1), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint o) =>
                                 {
                                     o = nc;
                                     return ret;
                                 });


        _backend.win_wch(new(1), out var x)
                .ShouldBe(ret);

        x.ShouldBe(ch);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void setcchar_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.setcchar>()
                                 .Setup(s => s(out It.Ref<uint>.IsAny, "text", 0x00400000, 20, new(2)))
                                 .Returns((out uint o, string _, uint _, short _,
                                     IntPtr _) =>
                                 {
                                     o = nc;
                                     return ret;
                                 });

        _backend.setcchar(out var c, "text", VideoAttribute.Blink, 20, new(2))
                .ShouldBe(ret);

        c.ShouldBe(ch);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcchar_IsRelayedToLibrary(int ret)
    {
        var sb = new StringBuilder();

        var (ch, nc) = MakeTestComplexChar();

        _nativeSymbolResolverMock.MockResolve<PdCursesFunctionMap.getcchar>()
                                 .Setup(s => s(ref nc, sb, out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, new(2)))
                                 .Returns((ref uint _, StringBuilder _, out uint a, out short cp,
                                     IntPtr _) =>
                                 {
                                     a = 0x00400000;
                                     cp = 2;
                                     return ret;
                                 });

        _backend.getcchar(ch, sb, out var attrs, out var pair, new(2))
                .ShouldBe(ret);

        attrs.ShouldBe(VideoAttribute.Blink);
        ((int) pair).ShouldBe(2);
    }

    [TestMethod]
    public void CursesMouseEventParser_ReturnsMouseParserAbi2()
    {
        _backend.CursesMouseEventParser.ShouldBe(CursesMouseEventParser.Get(2));
    }

    [TestMethod, DataRow(VideoAttribute.None, 0), DataRow(VideoAttribute.StandOut, 0x00A00000),
     DataRow(VideoAttribute.Underline, 0x00100000), DataRow(VideoAttribute.Reverse, 0x00200000),
     DataRow(VideoAttribute.Blink, 0x00400000), DataRow(VideoAttribute.Dim, 0),
     DataRow(VideoAttribute.Bold, 0x00800000), DataRow(VideoAttribute.AltCharset, 0x00010000),
     DataRow(VideoAttribute.Invisible, 0), DataRow(VideoAttribute.Protect, 0),
     DataRow(VideoAttribute.HorizontalHighlight, 0), DataRow(VideoAttribute.LeftHighlight, 0x00040000),
     DataRow(VideoAttribute.LowHighlight, 0), DataRow(VideoAttribute.LowHighlight, 0),
     DataRow(VideoAttribute.RightHighlight, 0x00020000), DataRow(VideoAttribute.TopHighlight, 0),
     DataRow(VideoAttribute.VerticalHighlight, 0), DataRow(VideoAttribute.Italic, 0x00080000),
     DataRow(VideoAttribute.AltCharset | VideoAttribute.Bold, 0x00810000)]
    public void EncodeCursesAttribute_WorksAsExpected(VideoAttribute attr, int exp)
    {
        _backend.EncodeCursesAttribute(attr, 15)
                .ShouldBe((uint) exp | (15 << 24));
    }

    [TestMethod, DataRow(VideoAttribute.None, 0), DataRow(VideoAttribute.StandOut, 0x00A00000),
     DataRow(VideoAttribute.Underline, 0x00100000), DataRow(VideoAttribute.Reverse, 0x00200000),
     DataRow(VideoAttribute.Blink, 0x00400000), DataRow(VideoAttribute.Bold, 0x00800000),
     DataRow(VideoAttribute.AltCharset, 0x00010000), DataRow(VideoAttribute.LeftHighlight, 0x00040000),
     DataRow(VideoAttribute.RightHighlight, 0x00020000), DataRow(VideoAttribute.Italic, 0x00080000),
     DataRow(VideoAttribute.AltCharset | VideoAttribute.Bold, 0x00810000)]
    public void DecodeCursesAttributes_WorksAsExpected(VideoAttribute exp, int attr)
    {
        _backend.DecodeCursesAttributes((uint) attr | (15 << 24))
                .ShouldBe((exp, (short) 15));
    }

    [TestMethod, DataRow(-1, 10u, (int) CursesKeyCodeType.Unknown), DataRow(0, 32u, (int) CursesKeyCodeType.Character),
     DataRow(100, 32u, (int) CursesKeyCodeType.Character),
     DataRow((int) PdCursesKeyCode.Yes, (uint) PdCursesKeyCode.F3, (int) CursesKeyCodeType.Key),
     DataRow((int) PdCursesKeyCode.Yes, (uint) PdCursesKeyCode.Resize, (int) CursesKeyCodeType.Resize),
     DataRow((int) PdCursesKeyCode.Yes, (uint) PdCursesKeyCode.Mouse, (int) CursesKeyCodeType.Mouse)]
    public void DecodeKeyCodeType_DecodesProperly(int res, uint code, int exp)
    {
        _backend.DecodeKeyCodeType(res, code)
                .ShouldBe((CursesKeyCodeType) exp);
    }

    [TestMethod, DataRow(PdCursesKeyCode.F1, ControlCharacter.Null, Key.F1, ModifierKey.None),
     DataRow(PdCursesKeyCode.F2, ControlCharacter.Null, Key.F2, ModifierKey.None),
     DataRow(PdCursesKeyCode.F3, ControlCharacter.Null, Key.F3, ModifierKey.None),
     DataRow(PdCursesKeyCode.F4, ControlCharacter.Null, Key.F4, ModifierKey.None),
     DataRow(PdCursesKeyCode.F5, ControlCharacter.Null, Key.F5, ModifierKey.None),
     DataRow(PdCursesKeyCode.F6, ControlCharacter.Null, Key.F6, ModifierKey.None),
     DataRow(PdCursesKeyCode.F7, ControlCharacter.Null, Key.F7, ModifierKey.None),
     DataRow(PdCursesKeyCode.F8, ControlCharacter.Null, Key.F8, ModifierKey.None),
     DataRow(PdCursesKeyCode.F9, ControlCharacter.Null, Key.F9, ModifierKey.None),
     DataRow(PdCursesKeyCode.F10, ControlCharacter.Null, Key.F10, ModifierKey.None),
     DataRow(PdCursesKeyCode.F11, ControlCharacter.Null, Key.F11, ModifierKey.None),
     DataRow(PdCursesKeyCode.F12, ControlCharacter.Null, Key.F12, ModifierKey.None),
     DataRow(PdCursesKeyCode.ShiftF1, ControlCharacter.Null, Key.F1, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF2, ControlCharacter.Null, Key.F2, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF3, ControlCharacter.Null, Key.F3, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF4, ControlCharacter.Null, Key.F4, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF5, ControlCharacter.Null, Key.F5, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF6, ControlCharacter.Null, Key.F6, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF7, ControlCharacter.Null, Key.F7, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF8, ControlCharacter.Null, Key.F8, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF9, ControlCharacter.Null, Key.F9, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF10, ControlCharacter.Null, Key.F10, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF11, ControlCharacter.Null, Key.F11, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftF12, ControlCharacter.Null, Key.F12, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.CtrlF1, ControlCharacter.Null, Key.F1, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF2, ControlCharacter.Null, Key.F2, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF3, ControlCharacter.Null, Key.F3, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF4, ControlCharacter.Null, Key.F4, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF5, ControlCharacter.Null, Key.F5, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF6, ControlCharacter.Null, Key.F6, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF7, ControlCharacter.Null, Key.F7, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF8, ControlCharacter.Null, Key.F8, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF9, ControlCharacter.Null, Key.F9, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF10, ControlCharacter.Null, Key.F10, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF11, ControlCharacter.Null, Key.F11, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlF12, ControlCharacter.Null, Key.F12, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.AltF1, ControlCharacter.Null, Key.F1, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF2, ControlCharacter.Null, Key.F2, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF3, ControlCharacter.Null, Key.F3, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF4, ControlCharacter.Null, Key.F4, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF5, ControlCharacter.Null, Key.F5, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF6, ControlCharacter.Null, Key.F6, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF7, ControlCharacter.Null, Key.F7, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF8, ControlCharacter.Null, Key.F8, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF9, ControlCharacter.Null, Key.F9, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF10, ControlCharacter.Null, Key.F10, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF11, ControlCharacter.Null, Key.F11, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF12, ControlCharacter.Null, Key.F12, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.ShiftAltF1, ControlCharacter.Null, Key.F1, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF2, ControlCharacter.Null, Key.F2, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF3, ControlCharacter.Null, Key.F3, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF4, ControlCharacter.Null, Key.F4, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF5, ControlCharacter.Null, Key.F5, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF6, ControlCharacter.Null, Key.F6, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF7, ControlCharacter.Null, Key.F7, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF8, ControlCharacter.Null, Key.F8, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF9, ControlCharacter.Null, Key.F9, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF10, ControlCharacter.Null, Key.F10, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF11, ControlCharacter.Null, Key.F11, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftAltF12, ControlCharacter.Null, Key.F12, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(PdCursesKeyCode.Up, ControlCharacter.Null, Key.KeypadUp, ModifierKey.None),
     DataRow(PdCursesKeyCode.Down, ControlCharacter.Null, Key.KeypadDown, ModifierKey.None),
     DataRow(PdCursesKeyCode.Left, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.None),
     DataRow(PdCursesKeyCode.Right, ControlCharacter.Null, Key.KeypadRight, ModifierKey.None),
     DataRow(PdCursesKeyCode.Home, ControlCharacter.Null, Key.KeypadHome, ModifierKey.None),
     DataRow(PdCursesKeyCode.End, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.None),
     DataRow(PdCursesKeyCode.PageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.None),
     DataRow(PdCursesKeyCode.PageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.None),
     DataRow(PdCursesKeyCode.Delete, ControlCharacter.Null, Key.Delete, ModifierKey.None),
     DataRow(PdCursesKeyCode.Insert, ControlCharacter.Null, Key.Insert, ModifierKey.None),
     DataRow(PdCursesKeyCode.BackTab, ControlCharacter.Null, Key.Tab, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.Backspace, ControlCharacter.Null, Key.Backspace, ModifierKey.None),
     DataRow(PdCursesKeyCode.ShiftUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.ShiftEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.AltUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlPageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.CtrlPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.ScrollDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.None),
     DataRow(PdCursesKeyCode.ScrollUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.None),
     DataRow(PdCursesKeyCode.Alt0, '0', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt1, '1', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt2, '2', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt3, '3', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt4, '4', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt5, '5', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt6, '6', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt7, '7', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt8, '8', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.Alt9, '9', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltA, 'A', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltB, 'B', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltC, 'C', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltD, 'D', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltE, 'E', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltF, 'F', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltG, 'G', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltH, 'H', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltI, 'I', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltJ, 'J', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltK, 'K', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltL, 'L', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltM, 'M', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltN, 'N', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltO, 'O', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltP, 'P', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltQ, 'Q', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltR, 'R', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltS, 'S', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltT, 'T', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltU, 'U', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltV, 'V', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltW, 'W', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltX, 'X', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltY, 'Y', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltZ, 'Z', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadSlash, '/', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadEnter, '\n', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadCtrlEnter, '\n', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadAltEnter, '\n', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadStop, ControlCharacter.Null, Key.KeypadHome, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadAsterisk, '*', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadMinus, '-', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadPlus, '+', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeypadCtrlStop, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadCtrlMiddle, '5', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadCtrlPlus, '+', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadCtrlMinus, '-', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadCtrlSlash, '/', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadCtrlAsterisk, '*', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeypadAltPlus, '+', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadAltMinus, '-', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadAltSlash, '/', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadAltAsterisk, '*', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeypadAltStop, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlInsert, ControlCharacter.Null, Key.Insert, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.AltDelete, ControlCharacter.Null, Key.Delete, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltInsert, ControlCharacter.Null, Key.Insert, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlTab, ControlCharacter.Null, Key.Tab, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.AltTab, ControlCharacter.Null, Key.Tab, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltMinus, '-', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltEqual, '=', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltPageDown, '+', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltEnter, '\n', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltEscape, ControlCharacter.Null, Key.Escape, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltBackQuote, '\'', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltLeftBracket, '(', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltRightBracket, ')', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltSemicolon, ';', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltForwardQuote, '`', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltComma, ',', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltStop, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltForwardSlash, '\\', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.AltBackspace, ControlCharacter.Null, Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlBackspace, ControlCharacter.Null, Key.Backspace, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPad0, '0', Key.Character, ModifierKey.None),
     DataRow(PdCursesKeyCode.KeyPadCtrl0, '0', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl1, '1', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl2, '2', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl3, '3', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl4, '4', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl5, '5', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl6, '6', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl7, '7', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl8, '8', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadCtrl9, '9', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadAlt0, '0', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt1, '1', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt2, '2', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt3, '3', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt4, '4', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt5, '5', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt6, '6', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt7, '7', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt8, '8', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.KeyPadAlt9, '9', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlDelete, ControlCharacter.Null, Key.Delete, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.AltBackSlash, '\\', Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.CtrlEnter, '\n', Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.KeyPadShiftEnter, '\n', Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftPlus, '+', Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftMinus, '-', Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftSlash, '/', Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftAsterisk, '*', Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftInsert, ControlCharacter.Null, Key.Insert, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.KeyPadShiftDelete, ControlCharacter.Null, Key.Delete, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.LeftShift, ControlCharacter.Null, Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.RightShift, ControlCharacter.Null, Key.Character, ModifierKey.Shift),
     DataRow(PdCursesKeyCode.LeftCtrl, ControlCharacter.Null, Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.RightCtrl, ControlCharacter.Null, Key.Character, ModifierKey.Ctrl),
     DataRow(PdCursesKeyCode.LeftAlt, ControlCharacter.Null, Key.Character, ModifierKey.Alt),
     DataRow(PdCursesKeyCode.RightAlt, ControlCharacter.Null, Key.Character, ModifierKey.Alt),
     DataRow((uint) 9999, ControlCharacter.Null, Key.Unknown, ModifierKey.None)]
    public void DecodeRawKey_DecodesProperly(uint rawKey, char chr, Key expKey, ModifierKey expMod)
    {
        _backend.DecodeRawKey(rawKey)
                .ShouldBe((expKey, chr, expMod));
    }

    [TestMethod]
    public void wadd_wch_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.wadd_wch(new(1), null!));
    }

    [TestMethod]
    public void wadd_wch_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.wadd_wch(new(1), new("bad")));
    }

    [TestMethod]
    public void wbkgrnd_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.wbkgrnd(new(1), null!));
    }

    [TestMethod]
    public void wbkgrnd_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.wbkgrnd(new(1), new("bad")));
    }

    [TestMethod]
    public void wvline_set_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.wvline_set(new(1), null!, 4));
    }

    [TestMethod]
    public void wvline_set_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.wvline_set(new(1), new("bad"), 4));
    }

    [TestMethod]
    public void whline_set_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.whline_set(new(1), null!, 4));
    }

    [TestMethod]
    public void whline_set_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.whline_set(new(1), new("bad"), 4));
    }

    [TestMethod]
    public void wgetbkgrnd_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.whline_set(new(1), null!, 4));
    }

    [TestMethod]
    public void wgetbkgrnd_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.whline_set(new(1), new("bad"), 4));
    }

    [TestMethod]
    public void getcchar_Throws_IfCharIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _backend.getcchar(null!, new(), out var _, out var _, new(2)));
    }

    [TestMethod]
    public void getcchar_Throws_IfCharIsIncompatible()
    {
        Should.Throw<ArgumentException>(() => _backend.getcchar(new("bad"), new(), out var _, out var _, new(2)));
    }

    [TestMethod, DataRow(0), DataRow(1), DataRow(2), DataRow(3), DataRow(4), DataRow(5), DataRow(6), DataRow(7)]
    public void wborder_set_Throws_IfCharIsNull(int bad)
    {
        var chs = new ComplexChar[8];
        for (var x = 0; x < chs.Length; x++)
        {
            if (x != bad)
            {
                (chs[x], _) = MakeTestComplexChar();
            }
        }

        Should.Throw<ArgumentNullException>(() => _backend.wborder_set(new(1), chs[0], chs[1], chs[2], chs[3],
            chs[4], chs[5], chs[6], chs[7]));
    }

    [TestMethod, DataRow(0), DataRow(1), DataRow(2), DataRow(3), DataRow(4), DataRow(5), DataRow(6), DataRow(7)]
    public void wborder_set_Throws_IfCharIsIncompatible(int bad)
    {
        var chs = new ComplexChar[8];
        for (var x = 0; x < chs.Length; x++)
        {
            if (x != bad)
            {
                (chs[x], _) = MakeTestComplexChar();
            } else
            {
                chs[x] = new("bad");
            }
        }

        Should.Throw<ArgumentException>(() => _backend.wborder_set(new(1), chs[0], chs[1], chs[2], chs[3],
            chs[4], chs[5], chs[6], chs[7]));
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void scrollok_RetainsValueInLocalCache(bool yes, int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.scrollok, int>(s => s(new(1), yes), ret);

        _backend.scrollok(new(1), yes)
                .ShouldBe(ret);

        _backend.is_scrollok(new(1))
                .ShouldBe(ret != -1 && yes);

        _backend.is_immedok(new(1))
                .ShouldBe(false);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void immedok_RetainsValueInLocalCache(bool yes)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.immedok>()
                                 .Setup(s => s(new(1), yes));

        _backend.immedok(new(1), yes);

        _backend.is_immedok(new(1))
                .ShouldBe(yes);

        _backend.is_scrollok(new(1))
                .ShouldBe(false);
    }

    [TestMethod]
    public void scrollok_And_immedok_DoeNotTouchEachOthersValues()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.scrollok, int>(
            s => s(It.IsAny<IntPtr>(), It.IsAny<bool>()), 0);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.immedok>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), It.IsAny<bool>()));

        _backend.scrollok(new(1), true);
        _backend.immedok(new(1), true);

        _backend.is_scrollok(new(1))
                .ShouldBe(true);

        _backend.is_immedok(new(1))
                .ShouldBe(true);

        _backend.scrollok(new(1), false);
        _backend.immedok(new(1), true);

        _backend.is_immedok(new(1))
                .ShouldBe(true);

        _backend.scrollok(new(1), true);
        _backend.immedok(new(1), false);

        _backend.is_scrollok(new(1))
                .ShouldBe(true);
    }
}
