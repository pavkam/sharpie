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
public class NCursesBackendTests
{
    private NCursesBackend _backend = null!;
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;

    private static CursesComplexChar MakeTestComplexChar(uint x) => new() { _attrAndColorPair = x };

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();
        _nativeSymbolResolverMock = new();

        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(true);

        _backend = new(_dotNetSystemAdapterMock.Object, _nativeSymbolResolverMock.Object);
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
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_immedok, bool>(s => s(h), ret);

        _backend.is_immedok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_leaveok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_leaveok, bool>(s => s(h), ret);

        _backend.is_leaveok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_scrollok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_scrollok, bool>(s => s(h), ret);

        _backend.is_scrollok(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_wintouched_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_wintouched, bool>(s => s(h), ret);

        _backend.is_wintouched(h)
                .ShouldBe(ret);
    }

    [TestMethod]
    public void noqiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noqiflush>()
                                 .Setup(s => s())
                                 .Callback(() => { called = true; });

        _backend.noqiflush();

        called.ShouldBeTrue();
    }

    [TestMethod]
    public void qiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.qiflush>()
                                 .Setup(s => s())
                                 .Callback(() => { called = true; });

        _backend.qiflush();

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void baudrate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.baudrate, int>(s => s(), ret);

        _backend.baudrate()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void beep_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.beep, int>(s => s(), ret);

        _backend.beep()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void cbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.cbreak, int>(s => s(), ret);

        _backend.cbreak()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void doupdate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.doupdate, int>(s => s(), ret);

        _backend.doupdate()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void echo_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.echo, int>(s => s(), ret);

        _backend.echo()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void endwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.endwin, int>(s => s(), ret);

        _backend.endwin()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void flash_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.flash, int>(s => s(), ret);

        _backend.flash()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nl, int>(s => s(), ret);

        _backend.nl()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nocbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nocbreak, int>(s => s(), ret);

        _backend.nocbreak()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void noecho_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noecho, int>(s => s(), ret);

        _backend.noecho()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void nonl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nonl, int>(s => s(), ret);

        _backend.nonl()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void noraw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noraw, int>(s => s(), ret);

        _backend.noraw()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void raw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.raw, int>(s => s(), ret);

        _backend.raw()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_clear_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_clear, int>(s => s(), ret);

        _backend.slk_clear()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_noutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_noutrefresh, int>(s => s(), ret);

        _backend.slk_noutrefresh()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_refresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_refresh, int>(s => s(), ret);

        _backend.slk_refresh()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_restore_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_restore, int>(s => s(), ret);

        _backend.slk_restore()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_touch_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_touch, int>(s => s(), ret);

        _backend.slk_touch()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void start_color_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.start_color, int>(s => s(), ret);

        _backend.start_color()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void use_default_colors_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.use_default_colors, int>(s => s(), ret);

        _backend.use_default_colors()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void has_colors_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.has_colors, bool>(s => s(), ret);

        _backend.has_colors()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void can_change_color_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.can_change_color, bool>(s => s(), ret);

        _backend.can_change_color()
                .ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(999)]
    public void initscr_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.initscr, IntPtr>(s => s(), new(ret));

        _backend.initscr()
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow((string?) null), DataRow("hello")]
    public void longname_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.longname, IntPtr>(s => s(), h);

        _backend.longname()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow((string?) null), DataRow("hello")]
    public void termname_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.termname, IntPtr>(s => s(), h);

        _backend.termname()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow((string?) null), DataRow("hello")]
    public void curses_version_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.curses_version, IntPtr>(s => s(), h);

        _backend.curses_version()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void intrflush_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.intrflush, int>(s => s(h, yes), ret);

        _backend.intrflush(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void keypad_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.keypad, int>(s => s(h, yes), ret);

        _backend.keypad(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void leaveok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.leaveok, int>(s => s(h, yes), ret);

        _backend.leaveok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void meta_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.meta, int>(s => s(h, yes), ret);

        _backend.meta(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void nodelay_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nodelay, int>(s => s(h, yes), ret);

        _backend.nodelay(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void notimeout_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.notimeout, int>(s => s(h, yes), ret);

        _backend.notimeout(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void scrollok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.scrollok, int>(s => s(h, yes), ret);

        _backend.scrollok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void syncok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.syncok, int>(s => s(h, yes), ret);

        _backend.syncok(h, yes)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void immedok_IsRelayedToLibrary(bool yes)
    {
        var h = new IntPtr(999);
        var called = false;

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.immedok>()
                                 .Setup(s => s(h, yes))
                                 .Callback((IntPtr _, bool _) => { called = true; });

        _backend.immedok(h, yes);

        called.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void use_env_IsRelayedToLibrary(bool yes)
    {
        var called = false;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.use_env>()
                                 .Setup(s => s(yes))
                                 .Callback((bool _) => { called = true; });

        _backend.use_env(yes);

        called.ShouldBeTrue();
    }
 
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtobot_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wclrtobot, int>(s => s(h), ret);

        _backend.wclrtobot(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtoeol_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wclrtoeol, int>(s => s(h), ret);

        _backend.wclrtoeol(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wdelch_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wdelch, int>(s => s(h), ret);

        _backend.wdelch(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void werase_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.werase, int>(s => s(h), ret);

        _backend.werase(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcury_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcury, int>(s => s(h), ret);

        _backend.getcury(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcurx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcurx, int>(s => s(h), ret);

        _backend.getcurx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getbegx, int>(s => s(h), ret);

        _backend.getbegx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getbegy, int>(s => s(h), ret);

        _backend.getbegy(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmaxx, int>(s => s(h), ret);

        _backend.getmaxx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmaxy, int>(s => s(h), ret);

        _backend.getmaxy(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getparx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getparx, int>(s => s(h), ret);

        _backend.getparx(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getpary_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getpary, int>(s => s(h), ret);

        _backend.getpary(h)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void curs_set_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.curs_set, int>(s => s(i), ret);

        _backend.curs_set(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_init_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_init, int>(s => s(i), ret);

        _backend.slk_init(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void mouseinterval_IsRelayedToLibrary(int ret)
    {
        const int i = 999;

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mouseinterval, int>(s => s(i), ret);

        _backend.mouseinterval(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_color_IsRelayedToLibrary(int ret)
    {
        const short i = 999;

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_color, int>(s => s(i), ret);

        _backend.slk_color(i)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr, int>(s => s(), ret);

        _backend.slk_attr()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void newpad_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.newpad, IntPtr>(s => s(10, 20), new(ret));

        _backend.newpad(10, 20)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void newwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.newwin, IntPtr>(s => s(10, 20, 30, 40), new(ret));

        _backend.newwin(10, 20, 30, 40)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mvwin, int>(s => s(h, 30, 40), ret);

        _backend.mvwin(h, 30, 40)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvderwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mvderwin, int>(s => s(h, 30, 40), ret);

        _backend.mvderwin(h, 30, 40)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void derwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.derwin, IntPtr>(s => s(h, 10, 20, 30, 40), new(ret));

        _backend.derwin(h, 10, 20, 30, 40)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void copywin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.copywin, int>(s => s(new(1), new(2), 20, 30, 40,
            50, 60, 70, 80), ret);

        _backend.copywin(new(1), new(2), 20, 30, 40,
                    50, 60, 70, 80)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.whline, int>(s => s(new(1), 'A', 10), ret);

        _backend.whline(new(1), 'A', 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wvline, int>(s => s(new(1), 'A', 10), ret);

        _backend.wvline(new(1), 'A', 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wredrawln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wredrawln, int>(s => s(new(1), 1, 10), ret);

        _backend.wredrawln(new(1), 1, 10)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wtouchln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wtouchln, int>(s => s(new(1), 2, 10, 1), ret);

        _backend.wtouchln(new(1), 2, 10, 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow((string?) null), DataRow("hello")]
    public void key_name_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.key_name, IntPtr>(s => s('A'), h);

        _backend.key_name('A')
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void dupwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.dupwin, IntPtr>(s => s(new(1)), new(ret));

        _backend.dupwin(new(1))
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void is_linetouched_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_linetouched, bool>(s => s(new(1), 2), ret);

        _backend.is_linetouched(new(1), 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void delwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.delwin, int>(s => s(new(1)), ret);

        _backend.delwin(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void overlay_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.overlay, int>(s => s(new(1), new(2)), ret);

        _backend.overlay(new(1), new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void overwrite_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.overwrite, int>(s => s(new(1), new(2)), ret);

        _backend.overwrite(new(1), new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_on_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_on, int>(s => s(new(1), 1, new(2)), ret);

        _backend.wattr_on(new(1), 1, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_off_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_off, int>(s => s(new(1), 1, new(2)), ret);

        _backend.wattr_off(new(1), 1, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_set, int>(s => s(1, 2, new(2)), ret);

        _backend.slk_attr_set(1, 2, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void winsdelln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.winsdelln, int>(s => s(new(1), 1), ret);

        _backend.winsdelln(new(1), 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wmove_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wmove, int>(s => s(new(1), 1, 2), ret);

        _backend.wmove(new(1), 1, 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wnoutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wnoutrefresh, int>(s => s(new(1)), ret);

        _backend.wnoutrefresh(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wrefresh, int>(s => s(new(1)), ret);

        _backend.wrefresh(new(1))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wscrl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wscrl, int>(s => s(new(1), 1), ret);

        _backend.wscrl(new(1), 1)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wresize_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wresize, int>(s => s(new(1), 1, 2), ret);

        _backend.wresize(new(1), 1, 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void pnoutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.pnoutrefresh, int>(s => s(new(1), 1, 2, 3, 4,
            5, 6), ret);

        _backend.pnoutrefresh(new(1), 1, 2, 3, 4,
                    5, 6)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void prefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.prefresh, int>(s => s(new(1), 1, 2, 3, 4,
            5, 6), ret);

        _backend.prefresh(new(1), 1, 2, 3, 4,
                    5, 6)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void subpad_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.subpad, IntPtr>(s => s(new(1), 1, 2, 3, 4), new(ret));

        _backend.subpad(new(1), 1, 2, 3, 4)
                .ShouldBe(new(ret));
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wborder_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wborder, int>(s => s(new(1), 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H'), ret);

        _backend.wborder(new(1), 'A', 'B', 'C', 'D',
                    'E', 'F', 'G', 'H')
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wchgat_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wchgat, int>(s => s(new(1), 1, 99, 44, new(999)), ret);

        _backend.wchgat(new(1), 1, 99, 44, new(999))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wcolor_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wcolor_set, int>(s => s(new(1), 1, new(999)), ret);

        _backend.wcolor_set(new(1), 1, new(999))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void assume_default_colors_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.assume_default_colors, int>(s => s(1, 2), ret);

        _backend.assume_default_colors(1, 2)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_set, int>(s => s(3, "title", 90), ret);

        _backend.slk_set(3, "title", 90)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void term_attrs_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.term_attrs, int>(s => s(), ret);

        _backend.term_attrs()
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_set, int>(s => s(new(1), 2, 3, new(4)), ret);

        _backend.wattr_set(new(1), 2, 3, new(4))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_on_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_on, int>(s => s(1, new(2)), ret);

        _backend.slk_attr_on(1, new(2))
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_off_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_off, int>(s => s(1, new(2)), ret);

        _backend.slk_attr_off(1, new(2))
                .ShouldBe(ret);
    }

    [TestMethod]
    public void wtimeout_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wtimeout>()
                                 .Setup(s => s(new(1), 2))
                                 .Callback(() => { called = true; });

        _backend.wtimeout(new(1), 2);

        called.ShouldBeTrue();
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void init_color_IsRelayedToLibrary(int ret)
    {
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.init_color, int>(s => s(1, 2, 3, 4), ret);

        _backend.init_color(1, 2, 3, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void init_pair_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.init_pair, int>(s => s(1, 2, 3), ret);

        _backend.init_pair(1, 2, 3)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wadd_wch_IsRelayedToLibrary(int ret)
    {
        var ch = new CursesComplexChar();

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wadd_wch, int>(s => s(new(1), ref ch), ret);

        _backend.wadd_wch(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wbkgrnd_IsRelayedToLibrary(int ret)
    {
        var ch = new CursesComplexChar();

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wbkgrnd, int>(s => s(new(1), ref ch), ret);

        _backend.wbkgrnd(new(1), ch)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_set_IsRelayedToLibrary(int ret)
    {
        var ch = new CursesComplexChar();

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wvline_set, int>(s => s(new(1), ref ch, 4), ret);

        _backend.wvline_set(new(1), ch, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_set_IsRelayedToLibrary(int ret)
    {
        var ch = new CursesComplexChar();

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.whline_set, int>(s => s(new(1), ref ch, 4), ret);

        _backend.whline_set(new(1), ch, 4)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void ripoffline_IsRelayedToLibrary(int ret)
    {
        var rf = new ICursesBackend.ripoffline_callback((_, _) => 0);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.ripoffline, int>(s => s(1, rf), ret);

        _backend.ripoffline(1, rf)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wborder_set_IsRelayedToLibrary(int ret)
    {
        var ch1 = MakeTestComplexChar(1);
        var ch2 = MakeTestComplexChar(2);
        var ch3 = MakeTestComplexChar(3);
        var ch4 = MakeTestComplexChar(4);
        var ch5 = MakeTestComplexChar(5);
        var ch6 = MakeTestComplexChar(6);
        var ch7 = MakeTestComplexChar(7);
        var ch8 = MakeTestComplexChar(8);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wborder_set, int>(
            s => s(new(1), ref ch1, ref ch2, ref ch3, ref ch4,
                ref ch5, ref ch6, ref ch7, ref ch8), ret);

        _backend.wborder_set(new(1), ch1, ch2, ch3, ch4,
                    ch5, ch6, ch7, ch8)
                .ShouldBe(ret);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void erasewchar_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.erasewchar>()
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
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.killwchar>()
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
    public void getmouse_IsRelayedToLibrary(int ret)
    {
        var exp = new CursesMouseEvent { id = 199 };
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmouse>()
                                 .Setup(s => s(out It.Ref<CursesMouseEvent>.IsAny))
                                 .Returns((out CursesMouseEvent o) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });


        _backend.getmouse(out var x)
                .ShouldBe(ret);

        x.ShouldBe(exp);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void wget_wch_IsRelayedToLibrary(int ret)
    {
        const uint exp = 199u;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wget_wch>()
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
    public void wgetbkgrnd_IsRelayedToLibrary(int ret)
    {
        var exp = MakeTestComplexChar(1);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wgetbkgrnd>()
                                 .Setup(s => s(new(1), out It.Ref<CursesComplexChar>.IsAny))
                                 .Returns((IntPtr _, out CursesComplexChar o) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });


        _backend.wgetbkgrnd(new(1), out var x)
                .ShouldBe(ret);

        x.ShouldBe(exp);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void win_wch_IsRelayedToLibrary(int ret)
    {
        var exp = MakeTestComplexChar(1);

        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.win_wch>()
                                 .Setup(s => s(new(1), out It.Ref<CursesComplexChar>.IsAny))
                                 .Returns((IntPtr _, out CursesComplexChar o) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });


        _backend.win_wch(new(1), out var x)
                .ShouldBe(ret);

        x.ShouldBe(exp);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void pair_content_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.pair_content>()
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
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.color_content>()
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

    [TestMethod, DataRow("something", -1), DataRow("6.2.3", 2), DataRow("something6.2.3", 2),
     DataRow("something 5.7", -1),  DataRow("something 4.7.5", -1), DataRow("something 5.7.12312", 1)]
    public void mouse_version_ParsesCursesVersion(string ver, int m)
    {
        var h = Marshal.StringToHGlobalAnsi(ver);
        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.curses_version>()
                                 .Setup(s => s())
                                 .Returns(h);

        _backend.mouse_version()
                .ShouldBe(m);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void mousemask_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mousemask>()
                                 .Setup(s => s(1, out It.Ref<uint>.IsAny))
                                 .Returns((uint _, out uint om) =>
                                 {
                                     om = 11;
                                     return ret;
                                 });


        _backend.mousemask(1, out var old)
                .ShouldBe(ret);

        old.ShouldBe(11u);
    }


    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_get_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_get>()
                                 .Setup(s => s(new(1), out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, new(2)))
                                 .Returns((IntPtr _, out uint a, out short cp, IntPtr _) =>
                                 {
                                     a = 11;
                                     cp = 22;
                                     return ret;
                                 });


        _backend.wattr_get(new(1), out var attrs, out var colorPair, new(2))
                .ShouldBe(ret);

        attrs.ShouldBe(11u);
        ((int) colorPair).ShouldBe(22);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void setcchar_IsRelayedToLibrary(int ret)
    {
        var exp = MakeTestComplexChar(1);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.setcchar>()
                                 .Setup(s => s(out It.Ref<CursesComplexChar>.IsAny, "text", 10, 20, new(2)))
                                 .Returns((out CursesComplexChar o, string _, uint _, short _,
                                     IntPtr _) =>
                                 {
                                     o = exp;
                                     return ret;
                                 });

        _backend.setcchar(out var ch, "text", 10, 20, new(2))
                .ShouldBe(ret);

        ch.ShouldBe(exp);
    }

    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcchar_IsRelayedToLibrary(int ret)
    {
        var sb = new StringBuilder();
        var ch = MakeTestComplexChar(1);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcchar>()
                                 .Setup(s => s(ref ch, sb, out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, new(2)))
                                 .Returns((ref CursesComplexChar _, StringBuilder _, out uint a, out short cp,
                                     IntPtr _) =>
                                 {
                                     a = 1;
                                     cp = 2;
                                     return ret;
                                 });

        _backend.getcchar(ch, sb, out var attrs, out var pair, new(2))
                .ShouldBe(ret);

        attrs.ShouldBe(1u);
        ((int) pair).ShouldBe(2);
    }

    [TestMethod]
    public void set_unicode_locale_DoesNothing()
    {
        _backend.set_unicode_locale();

        _dotNetSystemAdapterMock.VerifyNoOtherCalls();
        _nativeSymbolResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void monitor_pending_resize_DoesNothing()
    {
        _backend.monitor_pending_resize(() => { }, out var d)
                .ShouldBe(false);

        d.ShouldBeNull();

        _dotNetSystemAdapterMock.VerifyNoOtherCalls();
        _nativeSymbolResolverMock.VerifyNoOtherCalls();
    }
}