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

namespace Sharpie.Tests;

using System.Runtime.InteropServices;

[TestClass, SuppressMessage("ReSharper", "IdentifierTypo")]
public class NCursesBackendTests
{
    private NCursesBackend _backend = null!;
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;

    private static (ComplexChar, NCursesComplexChar) MakeTestComplexChar(uint x = 1)
    {
        var nc = new NCursesComplexChar { _attrAndColorPair = x };

        return (new(nc), nc);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();
        _nativeSymbolResolverMock = new();

        _backend = new(_dotNetSystemAdapterMock.Object, _nativeSymbolResolverMock.Object, null);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_scrollok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_scrollok, bool>(s => s(h), ret);

        _backend.is_scrollok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_immedok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_immedok, bool>(s => s(h), ret);

        _backend.is_immedok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void endwin_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.endwin, int>(s => s(), ret);

        _backend.endwin()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmouse_IsRelayedToLibrary(int ret)
    {
        var exp = new CursesMouseState { id = 199 };
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmouse>()
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

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_clear_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_clear, int>(s => s(), ret);

        _backend.slk_clear()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_noutrefresh_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_noutrefresh, int>(s => s(), ret);

        _backend.slk_noutrefresh()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_refresh_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_refresh, int>(s => s(), ret);

        _backend.slk_refresh()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_restore_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_restore, int>(s => s(), ret);

        _backend.slk_restore()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_touch_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_touch, int>(s => s(), ret);

        _backend.slk_touch()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_init_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_init, int>(s => s(i), ret);

        _backend.slk_init(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_color_IsRelayedToLibrary(int ret)
    {
        const short i = 999;

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_color, int>(s => s(i), ret);

        _backend.slk_color(i)
                .ShouldBe(ret);
    }

    [TestMethod]
    public void slk_attr_IsRelayedToLibrary_1()
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr, int>(s => s(),
            ((int) VideoAttribute.Blink << 16) + (99 << 8));

        _backend.slk_attr(out var a, out var cp)
                .ShouldBe(0);

        a.ShouldBe(VideoAttribute.Blink);
        ((int) cp).ShouldBe(99);
    }

    [TestMethod]
    public void slk_attr_IsRelayedToLibrary_2()
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr, int>(s => s(), -1);

        _backend.slk_attr(out _, out _)
                .ShouldBe(-1);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_set_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_set, int>(
            s => s((uint) VideoAttribute.Blink << 16, 2, new(2)), ret);

        _backend.slk_attr_set(VideoAttribute.Blink, 2, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_set_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_set, int>(s => s(3, "title", 90), ret);

        _backend.slk_set(3, "title", 90)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_on_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_on, int>(
            s => s((uint) VideoAttribute.Blink << 16, new(2)), ret);

        _backend.slk_attr_on(VideoAttribute.Blink, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_off_IsRelayedToLibrary(int ret)
    {
        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_off, int>(
            s => s((uint) VideoAttribute.Blink << 16, new(2)), ret);

        _backend.slk_attr_off(VideoAttribute.Blink, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wadd_wch_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wadd_wch, int>(s => s(new(1), ref nc), ret);

        _backend.wadd_wch(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wbkgrnd_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wbkgrnd, int>(s => s(new(1), ref nc), ret);

        _backend.wbkgrnd(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_set_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wvline_set, int>(s => s(new(1), ref nc, 4), ret);

        _backend.wvline_set(new(1), ch, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_set_IsRelayedToLibrary(int ret)
    {
        var (ch, nc) = MakeTestComplexChar();

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.whline_set, int>(s => s(new(1), ref nc, 4), ret);

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

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wborder_set, int>(
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

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wgetbkgrnd>()
                                 .Setup(s => s(new(1), out It.Ref<NCursesComplexChar>.IsAny))
                                 .Returns((IntPtr _, out NCursesComplexChar o) =>
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

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.win_wch>()
                                 .Setup(s => s(new(1), out It.Ref<NCursesComplexChar>.IsAny))
                                 .Returns((IntPtr _, out NCursesComplexChar o) =>
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

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.setcchar>()
                                 .Setup(s => s(out It.Ref<NCursesComplexChar>.IsAny, "text",
                                     (uint) VideoAttribute.Blink << 16, 20, new(2)))
                                 .Returns((out NCursesComplexChar o, string _, uint _, short _,
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

        _ = _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcchar>()
                                 .Setup(s => s(ref nc, sb, out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, new(2)))
                                 .Returns((ref NCursesComplexChar _, StringBuilder _, out uint a, out short cp,
                                     IntPtr _) =>
                                 {
                                     a = (uint) VideoAttribute.Blink << 16;
                                     cp = 2;
                                     return ret;
                                 });

        _backend.getcchar(ch, sb, out var attrs, out var pair, new(2))
                .ShouldBe(ret);

        attrs.ShouldBe(VideoAttribute.Blink);
        ((int) pair).ShouldBe(2);
    }

    [TestMethod, DataRow("something", -1), DataRow("6.2.3", 2), DataRow("something6.2.3", 2),
     DataRow("something 5.7", -1), DataRow("something 4.7.5", -1), DataRow("something 5.7.12312", 1)]
    public void CursesMouseEventParser_ReturnsMouseParserBasedOnAbi(string ver, int m)
    {
        var h = Marshal.StringToHGlobalAnsi(ver);
        _ = _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();

        _ = _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version>()
                                 .Setup(s => s())
                                 .Returns(h);

        _backend.CursesMouseEventParser.ShouldBe(m == 2
            ? CursesMouseEventParser.Get(CursesAbiVersion.NCurses6)
            : CursesMouseEventParser.Get(CursesAbiVersion.NCurses5));
    }

    [TestMethod]
    public void EncodeCursesAttribute_WorksAsExpected()
    {
        _backend.EncodeCursesAttribute(VideoAttribute.Blink, 15)
                .ShouldBe(((uint) VideoAttribute.Blink << 16) | (15 << 8));
    }

    [TestMethod]
    public void DecodeCursesAttributes_WorksAsExpected()
    {
        _backend.DecodeCursesAttributes(((uint) VideoAttribute.Blink << 16) | (15 << 8))
                .ShouldBe((VideoAttribute.Blink, (short) 15));
    }

    [TestMethod, DataRow(-1, 10u, (int) CursesKeyCodeType.Unknown), DataRow(0, 32u, (int) CursesKeyCodeType.Character),
     DataRow(100, 32u, (int) CursesKeyCodeType.Character),
     DataRow((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.F3, (int) CursesKeyCodeType.Key),
     DataRow((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Resize, (int) CursesKeyCodeType.Resize),
     DataRow((int) NCursesKeyCode.Yes, (uint) NCursesKeyCode.Mouse, (int) CursesKeyCodeType.Mouse)]
    public void DecodeKeyCodeType_DecodesProperly(int res, uint code, int exp)
    {
        _backend.DecodeKeyCodeType(res, code)
                .ShouldBe((CursesKeyCodeType) exp);
    }

    [TestMethod, DataRow(NCursesKeyCode.F1, ControlCharacter.Null, Key.F1, ModifierKey.None),
     DataRow(NCursesKeyCode.F2, ControlCharacter.Null, Key.F2, ModifierKey.None),
     DataRow(NCursesKeyCode.F3, ControlCharacter.Null, Key.F3, ModifierKey.None),
     DataRow(NCursesKeyCode.F4, ControlCharacter.Null, Key.F4, ModifierKey.None),
     DataRow(NCursesKeyCode.F5, ControlCharacter.Null, Key.F5, ModifierKey.None),
     DataRow(NCursesKeyCode.F6, ControlCharacter.Null, Key.F6, ModifierKey.None),
     DataRow(NCursesKeyCode.F7, ControlCharacter.Null, Key.F7, ModifierKey.None),
     DataRow(NCursesKeyCode.F8, ControlCharacter.Null, Key.F8, ModifierKey.None),
     DataRow(NCursesKeyCode.F9, ControlCharacter.Null, Key.F9, ModifierKey.None),
     DataRow(NCursesKeyCode.F10, ControlCharacter.Null, Key.F10, ModifierKey.None),
     DataRow(NCursesKeyCode.F11, ControlCharacter.Null, Key.F11, ModifierKey.None),
     DataRow(NCursesKeyCode.F12, ControlCharacter.Null, Key.F12, ModifierKey.None),
     DataRow(NCursesKeyCode.ShiftF1, ControlCharacter.Null, Key.F1, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF2, ControlCharacter.Null, Key.F2, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF3, ControlCharacter.Null, Key.F3, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF4, ControlCharacter.Null, Key.F4, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF5, ControlCharacter.Null, Key.F5, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF6, ControlCharacter.Null, Key.F6, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF7, ControlCharacter.Null, Key.F7, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF8, ControlCharacter.Null, Key.F8, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF9, ControlCharacter.Null, Key.F9, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF10, ControlCharacter.Null, Key.F10, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF11, ControlCharacter.Null, Key.F11, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftF12, ControlCharacter.Null, Key.F12, ModifierKey.Shift),
     DataRow(NCursesKeyCode.CtrlF1, ControlCharacter.Null, Key.F1, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF2, ControlCharacter.Null, Key.F2, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF3, ControlCharacter.Null, Key.F3, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF4, ControlCharacter.Null, Key.F4, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF5, ControlCharacter.Null, Key.F5, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF6, ControlCharacter.Null, Key.F6, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF7, ControlCharacter.Null, Key.F7, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF8, ControlCharacter.Null, Key.F8, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF9, ControlCharacter.Null, Key.F9, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF10, ControlCharacter.Null, Key.F10, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF11, ControlCharacter.Null, Key.F11, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlF12, ControlCharacter.Null, Key.F12, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.AltF1, ControlCharacter.Null, Key.F1, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF2, ControlCharacter.Null, Key.F2, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF3, ControlCharacter.Null, Key.F3, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF4, ControlCharacter.Null, Key.F4, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF5, ControlCharacter.Null, Key.F5, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF6, ControlCharacter.Null, Key.F6, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF7, ControlCharacter.Null, Key.F7, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF8, ControlCharacter.Null, Key.F8, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF9, ControlCharacter.Null, Key.F9, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF10, ControlCharacter.Null, Key.F10, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF11, ControlCharacter.Null, Key.F11, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltF12, ControlCharacter.Null, Key.F12, ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltF1, ControlCharacter.Null, Key.F1, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF2, ControlCharacter.Null, Key.F2, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF3, ControlCharacter.Null, Key.F3, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF4, ControlCharacter.Null, Key.F4, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF5, ControlCharacter.Null, Key.F5, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF6, ControlCharacter.Null, Key.F6, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF7, ControlCharacter.Null, Key.F7, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF8, ControlCharacter.Null, Key.F8, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF9, ControlCharacter.Null, Key.F9, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF10, ControlCharacter.Null, Key.F10, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF11, ControlCharacter.Null, Key.F11, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftAltF12, ControlCharacter.Null, Key.F12, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(NCursesKeyCode.Up, ControlCharacter.Null, Key.KeypadUp, ModifierKey.None),
     DataRow(NCursesKeyCode.Down, ControlCharacter.Null, Key.KeypadDown, ModifierKey.None),
     DataRow(NCursesKeyCode.Left, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.None),
     DataRow(NCursesKeyCode.Right, ControlCharacter.Null, Key.KeypadRight, ModifierKey.None),
     DataRow(NCursesKeyCode.Home, ControlCharacter.Null, Key.KeypadHome, ModifierKey.None),
     DataRow(NCursesKeyCode.End, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.None),
     DataRow(NCursesKeyCode.PageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.None),
     DataRow(NCursesKeyCode.PageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.None),
     DataRow(NCursesKeyCode.Delete, ControlCharacter.Null, Key.Delete, ModifierKey.None),
     DataRow(NCursesKeyCode.Insert, ControlCharacter.Null, Key.Insert, ModifierKey.None),
     DataRow(NCursesKeyCode.BackTab, ControlCharacter.Null, Key.Tab, ModifierKey.Shift),
     DataRow(NCursesKeyCode.Backspace, ControlCharacter.Null, Key.Backspace, ModifierKey.None),
     DataRow(NCursesKeyCode.ShiftUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftPageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.Shift),
     DataRow(NCursesKeyCode.ShiftPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Shift),
     DataRow(NCursesKeyCode.AltUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltPageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Alt),
     DataRow(NCursesKeyCode.CtrlUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlPageDown, ControlCharacter.Null, Key.KeypadPageDown, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.CtrlPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlRight, ControlCharacter.Null, Key.KeypadRight,
         ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlPageDown, ControlCharacter.Null, Key.KeypadPageDown,
         ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftCtrlPageUp, ControlCharacter.Null, Key.KeypadPageUp,
         ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.ShiftAltUp, ControlCharacter.Null, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltDown, ControlCharacter.Null, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltLeft, ControlCharacter.Null, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltRight, ControlCharacter.Null, Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltPageDown, ControlCharacter.Null, Key.KeypadPageDown,
         ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltPageUp, ControlCharacter.Null, Key.KeypadPageUp,
         ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.ShiftAltEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(NCursesKeyCode.AltCtrlPageDown, ControlCharacter.Null, Key.KeypadPageDown,
         ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.AltCtrlPageUp, ControlCharacter.Null, Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.AltCtrlHome, ControlCharacter.Null, Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(NCursesKeyCode.AltCtrlEnd, ControlCharacter.Null, Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((uint) 9999, ControlCharacter.Null, Key.Unknown, ModifierKey.None)]
    public void DecodeRawKey_DecodesProperly(uint rawKey, char chr, Key expKey, ModifierKey expMod)
    {
        _backend.DecodeRawKey(rawKey)
                .ShouldBe((expKey, chr, expMod));
    }

    [TestMethod]
    public void wadd_wch_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.wadd_wch(new(1), null!));

    [TestMethod]
    public void wadd_wch_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.wadd_wch(new(1), new("bad")));

    [TestMethod]
    public void wbkgrnd_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.wbkgrnd(new(1), null!));

    [TestMethod]
    public void wbkgrnd_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.wbkgrnd(new(1), new("bad")));

    [TestMethod]
    public void wvline_set_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.wvline_set(new(1), null!, 4));

    [TestMethod]
    public void wvline_set_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.wvline_set(new(1), new("bad"), 4));

    [TestMethod]
    public void whline_set_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.whline_set(new(1), null!, 4));

    [TestMethod]
    public void whline_set_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.whline_set(new(1), new("bad"), 4));

    [TestMethod]
    public void wgetbkgrnd_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.whline_set(new(1), null!, 4));

    [TestMethod]
    public void wgetbkgrnd_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.whline_set(new(1), new("bad"), 4));

    [TestMethod]
    public void getcchar_Throws_IfCharIsNull() => Should.Throw<ArgumentNullException>(() => _backend.getcchar(null!, new(), out var _, out var _, new(2)));

    [TestMethod]
    public void getcchar_Throws_IfCharIsIncompatible() => Should.Throw<ArgumentException>(() => _backend.getcchar(new("bad"), new(), out var _, out var _, new(2)));

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

        _ = Should.Throw<ArgumentNullException>(() => _backend.wborder_set(new(1), chs[0], chs[1], chs[2], chs[3],
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
            }
            else
            {
                chs[x] = new("bad");
            }
        }

        _ = Should.Throw<ArgumentException>(() => _backend.wborder_set(new(1), chs[0], chs[1], chs[2], chs[3],
            chs[4], chs[5], chs[6], chs[7]));
    }
}
