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

using System.Runtime.InteropServices;

[TestClass, SuppressMessage("ReSharper", "IdentifierTypo")]
public class BaseCursesBackendTests
{
    private BaseCursesBackend _backend = null!;
    private Mock<BaseCursesBackend> _backendMock = null!;
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();
        _nativeSymbolResolverMock = new();

        var isLinux = TestContext.TestName!.Contains("_WhenLinux_");
        var isFreeBsd = TestContext.TestName!.Contains("_WhenFreeBsd_");
        var isMacOs = TestContext.TestName!.Contains("_WhenMacOs_");
        var isWindows = TestContext.TestName!.Contains("_WhenWindows_");
        var isUnix = TestContext.TestName!.Contains("_WhenUnix_") || isLinux || isFreeBsd || isMacOs;

        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(isUnix);

        _dotNetSystemAdapterMock.Setup(s => s.IsLinux)
                                .Returns(isLinux);

        _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(isFreeBsd);

        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(isMacOs);

        _dotNetSystemAdapterMock.Setup(s => s.IsWindows)
                                .Returns(isWindows);

        _backendMock = new(_dotNetSystemAdapterMock.Object, _nativeSymbolResolverMock.Object,
            isWindows ? null : _nativeSymbolResolverMock.Object);

        _backendMock.Setup(s => s.EncodeCursesAttribute(It.IsAny<VideoAttribute>(), It.IsAny<short>()))
                    .Returns((VideoAttribute attributes, short colorPair) =>
                        ((uint) attributes << 16) | (((uint) colorPair & 0xFF) << 8));

        _backendMock.Setup(s => s.DecodeCursesAttributes(It.IsAny<uint>()))
                    .Returns((uint attrs) => ((VideoAttribute) (attrs >> 16), (short) ((attrs >> 8) & 0xFF)));

        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        _backend = _backendMock.Object;
    }

    [TestMethod]
    public void DecodeRawMouseButtonState_ParsesUsingMouseParser()
    {
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(CursesMouseEventParser.Get(2));

        _backendMock.Setup(s => s.DecodeRawMouseButtonState(It.IsAny<uint>()))
                    .CallBase();

        _backend.DecodeRawMouseButtonState(8u << ((5 - 1) * 5))
                .ShouldBe((MouseButton.Button5, MouseButtonState.DoubleClicked, ModifierKey.None));

        _backendMock.Verify(v => v.CursesMouseEventParser, Times.Once);
    }

    [TestMethod]
    public void DecodeRawMouseButtonState_ReturnsDefault_IfNotParsed()
    {
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(CursesMouseEventParser.Get(2));

        _backendMock.Setup(s => s.DecodeRawMouseButtonState(It.IsAny<uint>()))
                    .CallBase();

        _backend.DecodeRawMouseButtonState(0)
                .ShouldBe((MouseButton.Unknown, MouseButtonState.None, ModifierKey.None));

        _backendMock.Verify(v => v.CursesMouseEventParser, Times.Once);
    }

    [TestMethod]
    public void set_title_CallsTheDotNetSystemAdapter()
    {
        _backend.set_title("hello");

        _dotNetSystemAdapterMock.Verify(v => v.SetConsoleTitle("hello"), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_immedok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.is_immedok, bool>(s => s(h), ret);

        _backend.is_immedok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_leaveok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.is_leaveok, bool>(s => s(h), ret);

        _backend.is_leaveok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_scrollok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.is_scrollok, bool>(s => s(h), ret);

        _backend.is_scrollok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_wintouched_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.is_wintouched, bool>(s => s(h), ret);

        _backend.is_wintouched(h)
                .ShouldBe(ret);
    }

    [TestMethod]
    public void noqiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.noqiflush>()
                                 .Setup(s => s())
                                 .Callback(() => { called = true; });

        _backend.noqiflush();

        called.ShouldBeTrue();
    }

    [TestMethod]
    public void qiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.qiflush>()
                                 .Setup(s => s())
                                 .Callback(() => { called = true; });

        _backend.qiflush();

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void baudrate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.baudrate, int>(s => s(), ret);

        _backend.baudrate()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void beep_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.beep, int>(s => s(), ret);

        _backend.beep()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void cbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.cbreak, int>(s => s(), ret);

        _backend.cbreak()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void doupdate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.doupdate, int>(s => s(), ret);

        _backend.doupdate()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void echo_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.echo, int>(s => s(), ret);

        _backend.echo()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void flash_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.flash, int>(s => s(), ret);

        _backend.flash()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.nl, int>(s => s(), ret);

        _backend.nl()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nocbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.nocbreak, int>(s => s(), ret);

        _backend.nocbreak()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void noecho_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.noecho, int>(s => s(), ret);

        _backend.noecho()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nonl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.nonl, int>(s => s(), ret);

        _backend.nonl()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void noraw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.noraw, int>(s => s(), ret);

        _backend.noraw()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void raw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.raw, int>(s => s(), ret);

        _backend.raw()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void start_color_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.start_color, int>(s => s(), ret);

        _backend.start_color()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void use_default_colors_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.use_default_colors, int>(s => s(), ret);

        _backend.use_default_colors()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void has_colors_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.has_colors, bool>(s => s(), ret);

        _backend.has_colors()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void can_change_color_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.can_change_color, bool>(s => s(), ret);

        _backend.can_change_color()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(999)]
    public void initscr_IsRelayedToLibrary(int ret)
    {
        _backendMock.Setup(s => s.initscr())
                    .CallBase();

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.initscr, IntPtr>(s => s(), new(ret));

        _backend.initscr()
                .ShouldBe(new(ret));
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void initscr_WhenLinux_CallsLibc_ToSetupUnicode()
    {
        var m = _nativeSymbolResolverMock.MockResolve<LibCFunctionMap.setlocale>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.initscr>();

        _backendMock.Setup(s => s.initscr())
                    .CallBase();

        _backend.initscr();

        m.Verify(v => v(6, ""));
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void initscr_WhenFreeBsd_CallsLibc_ToSetupUnicode()
    {
        var m = _nativeSymbolResolverMock.MockResolve<LibCFunctionMap.setlocale>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.initscr>();

        _backendMock.Setup(s => s.initscr())
                    .CallBase();

        _backend.initscr();

        m.Verify(v => v(6, ""));
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void initscr_WhenMacOs_CallsLibc_ToSetupUnicode()
    {
        var m = _nativeSymbolResolverMock.MockResolve<LibCFunctionMap.setlocale>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.initscr>();

        _backendMock.Setup(s => s.initscr())
                    .CallBase();

        _backend.initscr();

        m.Verify(v => v(0, ""));
    }

    [TestMethod, DataRow(null), DataRow("hello")]
    public void longname_IsRelayedToLibrary(string ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.longname, IntPtr>(s => s(), Marshal.StringToHGlobalAnsi(ret));

        _backend.longname()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(null), DataRow("hello")]
    public void termname_IsRelayedToLibrary(string ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.termname, IntPtr>(s => s(), Marshal.StringToHGlobalAnsi(ret));

        _backend.termname()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(null), DataRow("hello")]
    public void curses_version_IsRelayedToLibrary(string ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version, IntPtr>(s => s(), Marshal.StringToHGlobalAnsi(ret));

        _backend.curses_version()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void intrflush_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.intrflush, int>(s => s(h, yes), ret);

        _backend.intrflush(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void keypad_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.keypad, int>(s => s(h, yes), ret);

        _backend.keypad(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void leaveok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.leaveok, int>(s => s(h, yes), ret);

        _backend.leaveok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void meta_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.meta, int>(s => s(h, yes), ret);

        _backend.meta(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void nodelay_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.nodelay, int>(s => s(h, yes), ret);

        _backend.nodelay(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void notimeout_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.notimeout, int>(s => s(h, yes), ret);

        _backend.notimeout(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void scrollok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.scrollok, int>(s => s(h, yes), ret);

        _backend.scrollok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void syncok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.syncok, int>(s => s(h, yes), ret);

        _backend.syncok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void immedok_IsRelayedToLibrary(bool yes)
    {
        var h = new IntPtr(999);
        var called = false;

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.immedok>()
                                 .Setup(s => s(h, yes))
                                 .Callback((IntPtr _, bool _) => { called = true; });

        _backend.immedok(h, yes);

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void use_env_IsRelayedToLibrary(bool yes)
    {
        var called = false;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.use_env>()
                                 .Setup(s => s(yes))
                                 .Callback((bool _) => { called = true; });

        _backend.use_env(yes);

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtobot_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wclrtobot, int>(s => s(h), ret);

        _backend.wclrtobot(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtoeol_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wclrtoeol, int>(s => s(h), ret);

        _backend.wclrtoeol(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wdelch_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wdelch, int>(s => s(h), ret);

        _backend.wdelch(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void werase_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.werase, int>(s => s(h), ret);

        _backend.werase(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcury_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getcury, int>(s => s(h), ret);

        _backend.getcury(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcurx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getcurx, int>(s => s(h), ret);

        _backend.getcurx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getbegx, int>(s => s(h), ret);

        _backend.getbegx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getbegy, int>(s => s(h), ret);

        _backend.getbegy(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getmaxx, int>(s => s(h), ret);

        _backend.getmaxx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getmaxy, int>(s => s(h), ret);

        _backend.getmaxy(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getparx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getparx, int>(s => s(h), ret);

        _backend.getparx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getpary_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.getpary, int>(s => s(h), ret);

        _backend.getpary(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void curs_set_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curs_set, int>(s => s(i), ret);

        _backend.curs_set(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void mouseinterval_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mouseinterval, int>(s => s(i), ret);

        _backend.mouseinterval(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void newpad_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.newpad, IntPtr>(s => s(10, 20), new(ret));

        _backend.newpad(10, 20)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void newwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.newwin, IntPtr>(s => s(10, 20, 30, 40), new(ret));

        _backend.newwin(10, 20, 30, 40)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mvwin, int>(s => s(h, 30, 40), ret);

        _backend.mvwin(h, 30, 40)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvderwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mvderwin, int>(s => s(h, 30, 40), ret);

        _backend.mvderwin(h, 30, 40)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void derwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.derwin, IntPtr>(s => s(h, 10, 20, 30, 40),
            new(ret));

        _backend.derwin(h, 10, 20, 30, 40)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void copywin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.copywin, int>(s => s(new(1), new(2), 20, 30, 40,
            50, 60, 70, 80), ret);

        _backend.copywin(new(1), new(2), 20, 30, 40,
                    50, 60, 70, 80)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.whline, int>(s => s(new(1), 'A', 10), ret);

        _backend.whline(new(1), 'A', 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wvline, int>(s => s(new(1), 'A', 10), ret);

        _backend.wvline(new(1), 'A', 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wredrawln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wredrawln, int>(s => s(new(1), 1, 10), ret);

        _backend.wredrawln(new(1), 1, 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wtouchln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtouchln, int>(s => s(new(1), 2, 10, 1), ret);

        _backend.wtouchln(new(1), 2, 10, 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(null), DataRow("hello")]
    public void key_name_IsRelayedToLibrary(string ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.key_name, IntPtr>(s => s('A'), Marshal.StringToHGlobalAnsi(ret));

        _backend.key_name('A')
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void dupwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.dupwin, IntPtr>(s => s(new(1)), new(ret));

        _backend.dupwin(new(1))
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_linetouched_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.is_linetouched, bool>(s => s(new(1), 2), ret);

        _backend.is_linetouched(new(1), 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void delwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.delwin, int>(s => s(new(1)), ret);

        _backend.delwin(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void overlay_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.overlay, int>(s => s(new(1), new(2)), ret);

        _backend.overlay(new(1), new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void overwrite_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.overwrite, int>(s => s(new(1), new(2)), ret);

        _backend.overwrite(new(1), new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_on_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wattr_on, int>(
            s => s(new(1), (uint) VideoAttribute.Blink << 16, new(2)), ret);

        _backend.wattr_on(new(1), VideoAttribute.Blink, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_off_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wattr_off, int>(
            s => s(new(1), (uint) VideoAttribute.Blink << 16, new(2)), ret);

        _backend.wattr_off(new(1), VideoAttribute.Blink, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void winsdelln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.winsdelln, int>(s => s(new(1), 1), ret);

        _backend.winsdelln(new(1), 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wmove_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wmove, int>(s => s(new(1), 1, 2), ret);

        _backend.wmove(new(1), 1, 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wnoutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wnoutrefresh, int>(s => s(new(1)), ret);

        _backend.wnoutrefresh(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wrefresh, int>(s => s(new(1)), ret);

        _backend.wrefresh(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wscrl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wscrl, int>(s => s(new(1), 1), ret);

        _backend.wscrl(new(1), 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wresize_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wresize, int>(s => s(new(1), 1, 2), ret);

        _backend.wresize(new(1), 1, 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void pnoutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.pnoutrefresh, int>(s => s(new(1), 1, 2, 3, 4,
            5, 6), ret);

        _backend.pnoutrefresh(new(1), 1, 2, 3, 4,
                    5, 6)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void prefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.prefresh, int>(s => s(new(1), 1, 2, 3, 4,
            5, 6), ret);

        _backend.prefresh(new(1), 1, 2, 3, 4,
                    5, 6)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void subpad_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.subpad, IntPtr>(s => s(new(1), 1, 2, 3, 4),
            new(ret));

        _backend.subpad(new(1), 1, 2, 3, 4)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wborder_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wborder, int>(s => s(new(1), 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H'), ret);

        _backend.wborder(new(1), 'A', 'B', 'C', 'D',
                    'E', 'F', 'G', 'H')
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wchgat_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wchgat, int>(
            s => s(new(1), 1, (uint) VideoAttribute.Blink << 16, 44, new(999)), ret);

        _backend.wchgat(new(1), 1, VideoAttribute.Blink, 44, new(999))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wcolor_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wcolor_set, int>(s => s(new(1), 1, new(999)), ret);

        _backend.wcolor_set(new(1), 1, new(999))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void assume_default_colors_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.assume_default_colors, int>(s => s(1, 2), ret);

        _backend.assume_default_colors(1, 2)
                .ShouldBe(ret);
    }

    [TestMethod]
    public void term_attrs_IsRelayedToLibrary_1()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.term_attrs, int>(s => s(),
            (int) VideoAttribute.Blink << 16);

        _backend.term_attrs(out var x)
                .ShouldBe(0);

        x.ShouldBe(VideoAttribute.Blink);
    }

    [TestMethod]
    public void term_attrs_IsRelayedToLibrary_2()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.term_attrs, int>(s => s(), -1);

        _backend.term_attrs(out var _)
                .ShouldBe(-1);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wattr_set, int>(
            s => s(new(1), (uint) VideoAttribute.Blink << 16, 3, new(4)), ret);

        _backend.wattr_set(new(1), VideoAttribute.Blink, 3, new(4))
                .ShouldBe(ret);
    }

    [TestMethod]
    public void wtimeout_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>()
                                 .Setup(s => s(new(1), 2))
                                 .Callback(() => { called = true; });

        _backend.wtimeout(new(1), 2);

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void init_color_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.init_color, int>(s => s(1, 2, 3, 4), ret);

        _backend.init_color(1, 2, 3, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void init_pair_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.init_pair, int>(s => s(1, 2, 3), ret);

        _backend.init_pair(1, 2, 3)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void ripoffline_IsRelayedToLibrary(int ret)
    {
        var rf = new ICursesBackend.ripoffline_callback((_, _) => 0);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.ripoffline, int>(s => s(1, rf), ret);

        _backend.ripoffline(1, rf)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void erasewchar_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.erasewchar>()
                                 .Setup(s => s(out It.Ref<uint>.IsAny))
                                 .Returns((out uint o) =>
                                 {
                                     o = 'A';
                                     return ret;
                                 });

        _backend.erasewchar(out var ch)
                .ShouldBe(ret);

        ch.ShouldBe('A');
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void killwchar_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.killwchar>()
                                 .Setup(s => s(out It.Ref<uint>.IsAny))
                                 .Returns((out uint o) =>
                                 {
                                     o = 'A';
                                     return ret;
                                 });

        _backend.killwchar(out var ch)
                .ShouldBe(ret);

        ch.ShouldBe('A');
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wget_wch_IsRelayedToLibrary(int ret)
    {
        const uint exp = 199u;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(new(1), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint o) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });


        _backend.wget_wch(new(1), out var x)
                .ShouldBe(ret);

        x.ShouldBe(exp);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void pair_content_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.pair_content>()
                                 .Setup(s => s(1, out It.Ref<short>.IsAny, out It.Ref<short>.IsAny))
                                 .Returns((short _, out short t, out short b) =>
                                 {
                                     t = 99;
                                     b = 299;
                                     return ret;
                                 });


        _backend.pair_content(1, out var fg, out var bg)
                .ShouldBe(ret);

        ((int) fg).ShouldBe(99);
        ((int) bg).ShouldBe(299);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void color_content_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.color_content>()
                                 .Setup(s => s(1, out It.Ref<short>.IsAny, out It.Ref<short>.IsAny,
                                     out It.Ref<short>.IsAny))
                                 .Returns((short _, out short r, out short g, out short b) =>
                                 {
                                     r = 11;
                                     g = 22;
                                     b = 33;
                                     return ret;
                                 });


        _backend.color_content(1, out var red, out var green, out var blue)
                .ShouldBe(ret);

        ((int) red).ShouldBe(11);
        ((int) green).ShouldBe(22);
        ((int) blue).ShouldBe(33);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void mousemask_IsRelayedToLibrary(int ret)
    {
        var p = CursesMouseEventParser.Get(2);
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(p);

        var expNm = 0u;
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask>()
                                 .Setup(s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny))
                                 .Returns((uint nm, out uint om) =>
                                 {
                                     expNm = nm;
                                     om = 11;
                                     return ret;
                                 });

        _backend.mousemask(0xffffffff, out var old)
                .ShouldBe(ret);

        expNm.ShouldBe(p.All | p.ReportPosition);

        old.ShouldBe(11u);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void mousemask_WhenUnix_CallsCursesButNotConsole_IfCursesFails()
    {
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(CursesMouseEventParser.Get(1));

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask>()
                                 .Setup(s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny))
                                 .Returns((uint _, out uint o) =>
                                 {
                                     o = 999;
                                     return -1;
                                 });

        _backend.mousemask(100, out var old)
                .ShouldBe(-1);

        old.ShouldBe(999u);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush(It.IsAny<string>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo"), DataRow(1), DataRow(2)]
    public void mousemask_WhenUnix_OutsToConsole_WhenReportingPosition(int abi)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        var parser = CursesMouseEventParser.Get(abi);
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(parser);

        _backend.mousemask(parser.ReportPosition, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1003h"), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo"), DataRow(1), DataRow(2)]
    public void mousemask_WhenUnix_OutsToConsole_WhenAll(int abi)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        var parser = CursesMouseEventParser.Get(abi);
        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(parser);

        _backend.mousemask(parser.All, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1000h"), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void mousemask_WhenUnix_OutsToConsole_WhenNothing()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        _backendMock.Setup(s => s.CursesMouseEventParser)
                    .Returns(CursesMouseEventParser.Get(1));

        _backend.mousemask(0, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1003l"), Times.Once);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_get_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wattr_get>()
                                 .Setup(s => s(new(1), out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, new(2)))
                                 .Returns((IntPtr _, out uint a, out short cp, IntPtr _) =>
                                 {
                                     a = (uint) VideoAttribute.Blink << 16;
                                     cp = 22;
                                     return ret;
                                 });


        _backend.wattr_get(new(1), out var attrs, out var colorPair, new(2))
                .ShouldBe(ret);

        attrs.ShouldBe(VideoAttribute.Blink);
        ((int) colorPair).ShouldBe(22);
    }

    [TestMethod]
    public void set_unicode_locale_DoesNothing()
    {
        _backendMock.Setup(s => s.set_unicode_locale())
                    .CallBase();

        _backend.set_unicode_locale();

        _dotNetSystemAdapterMock.VerifyNoOtherCalls();
        _nativeSymbolResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void wget_event_SetsTimeoutAndReadsCharacter()
    {
        var wt = _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 999;
                                     return 88;
                                 });

        _backend.wget_event(new(1), 10, out var _)
                .ShouldBe(88);

        wt.Verify(v => v(new(1), 10), Times.Once);
    }

    [TestMethod]
    public void wget_event_ReturnsNullEvent_IfKeyCodeTypeIsUnknown()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 999;
                                     return -1;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(-1);

        e.ShouldBeNull();
    }

    [TestMethod]
    public void wget_event_ReturnsCharEvent_IfKeyCodeTypeIsCharacter()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(0, 'A'))
                    .Returns(CursesKeyCodeType.Character);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.key_name>()
                                 .Setup(s => s('A'))
                                 .Returns(Marshal.StringToHGlobalAnsi("name"));
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 'A';
                                     return 0;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(0);

        e.ShouldBe(new CursesCharEvent("name", 'A', ModifierKey.None));
    }

    [TestMethod]
    public void wget_event_ReturnsResizeEvent_IfKeyCodeTypeIsResize()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(22, 11))
                    .Returns(CursesKeyCodeType.Resize);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 11;
                                     return 22;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(22);

        e.ShouldBe(new CursesResizeEvent());
    }

    [TestMethod]
    public void wget_event_ReturnsKeyEvent_IfKeyCodeTypeIsKey()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(11, 'A'))
                    .Returns(CursesKeyCodeType.Key);

        _backendMock.Setup(s => s.DecodeRawKey('A'))
                    .Returns((Key.Backspace, ControlCharacter.Null, ModifierKey.Ctrl));

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.key_name>()
                                 .Setup(s => s('A'))
                                 .Returns(Marshal.StringToHGlobalAnsi("name"));
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 'A';
                                     return 11;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(11);

        e.ShouldBe(new CursesKeyEvent("name", Key.Backspace, ModifierKey.Ctrl));
    }

    [TestMethod]
    public void wget_event_ReturnsCharEvent_IfDecodedKeyCodeIsCharIndeed()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(11, 'A'))
                    .Returns(CursesKeyCodeType.Key);

        _backendMock.Setup(s => s.DecodeRawKey('A'))
                    .Returns((Key.Character, 'Z', ModifierKey.Ctrl));
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.key_name>()
                                 .Setup(s => s('A'))
                                 .Returns(Marshal.StringToHGlobalAnsi("name"));
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 'A';
                                     return 11;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(11);

        e.ShouldBe(new CursesCharEvent("name", 'Z', ModifierKey.Ctrl));
    }

    [TestMethod]
    public void wget_event_ReturnsMouseEvent_IfKeyCodeTypeIsMouse()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(11, 'A'))
                    .Returns(CursesKeyCodeType.Mouse);

        _backendMock.Setup(s => s.getmouse(out It.Ref<CursesMouseState>.IsAny))
                    .Returns((out CursesMouseState ms) =>
                    {
                        ms = new() { x = 10, y = 20, buttonState = 999 };
                        return 0;
                    });

        _backendMock.Setup(s => s.DecodeRawMouseButtonState(999))
                    .Returns((MouseButton.Button2, MouseButtonState.TripleClicked, ModifierKey.Ctrl));

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 'A';
                                     return 11;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(11);

        e.ShouldBe(new CursesMouseEvent(10, 20, MouseButton.Button2, MouseButtonState.TripleClicked, ModifierKey.Ctrl));
    }

    [TestMethod]
    public void wget_event_ReturnsNull_IfKeyCodeTypeIsMouse_AndGetMouseFails()
    {
        _backendMock.Setup(s => s.DecodeKeyCodeType(11, 'A'))
                    .Returns(CursesKeyCodeType.Mouse);

        _backendMock.Setup(s => s.getmouse(out It.Ref<CursesMouseState>.IsAny))
                    .Returns((out CursesMouseState ms) =>
                    {
                        ms = new() { x = 10, y = 20, buttonState = 999 };
                        return -1;
                    });

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wtimeout>();
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.wget_wch>()
                                 .Setup(s => s(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                                 .Returns((IntPtr _, out uint kc) =>
                                 {
                                     kc = 'A';
                                     return 11;
                                 });

        _backend.wget_event(new(1), 10, out var e)
                .ShouldBe(11);

        e.ShouldBeNull();
    }
}
