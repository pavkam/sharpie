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

[TestClass]
public class NCursesBackendTests
{
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;
    private NCursesBackend _backend = null!;

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
    public void is_cleared_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_cleared, bool>(
            s => s(h), ret);

        _backend.is_cleared(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_idcok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_idcok, bool>(
            s => s(h), ret);

        _backend.is_idcok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_idlok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_idlok, bool>(
            s => s(h), ret);

        _backend.is_idlok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_immedok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_immedok, bool>(
            s => s(h), ret);

        _backend.is_immedok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_keypad_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_keypad, bool>(
            s => s(h), ret);

        _backend.is_keypad(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_leaveok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_leaveok, bool>(
            s => s(h), ret);

        _backend.is_leaveok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_nodelay_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_nodelay, bool>(
            s => s(h), ret);

        _backend.is_nodelay(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_notimeout_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_notimeout, bool>(
            s => s(h), ret);

        _backend.is_notimeout(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_scrollok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_scrollok, bool>(
            s => s(h), ret);

        _backend.is_scrollok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_syncok_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_syncok, bool>(
            s => s(h), ret);

        _backend.is_syncok(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_wintouched_IsRelayedToLibrary(bool ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_wintouched, bool>(
            s => s(h), ret);

        _backend.is_wintouched(h).ShouldBe(ret);
    }
    
    [TestMethod]
    public void filter_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.filter>()
                                 .Setup(s => s())
                                 .Callback(() =>
                                 {
                                     called = true;
                                 });

        _backend.filter();
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void noqiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noqiflush>()
                                 .Setup(s => s())
                                 .Callback(() =>
                                 {
                                     called = true;
                                 });

        _backend.noqiflush();
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void qiflush_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.qiflush>()
                                 .Setup(s => s())
                                 .Callback(() =>
                                 {
                                     called = true;
                                 });

        _backend.qiflush();
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void nofilter_IsRelayedToLibrary()
    {
        var called = true;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nofilter>()
                                 .Setup(s => s())
                                 .Callback(() =>
                                 {
                                     called = true;
                                 });

        _backend.nofilter();
        
        called.ShouldBeTrue();
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void baudrate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.baudrate, int>(
            s => s(), ret);

        _backend.baudrate().ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void beep_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.beep, int>(
            s => s(), ret);

        _backend.beep().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void cbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.cbreak, int>(
            s => s(), ret);

        _backend.cbreak().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void def_prog_mode_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.def_prog_mode, int>(
            s => s(), ret);

        _backend.def_prog_mode().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void def_shell_mode_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.def_shell_mode, int>(
            s => s(), ret);

        _backend.def_shell_mode().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void doupdate_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.doupdate, int>(
            s => s(), ret);

        _backend.doupdate().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void echo_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.echo, int>(
            s => s(), ret);

        _backend.echo().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void endwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.endwin, int>(
            s => s(), ret);

        _backend.endwin().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void flushinp_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.flushinp, int>(
            s => s(), ret);

        _backend.flushinp().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void flash_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.flash, int>(
            s => s(), ret);

        _backend.flash().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void nl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nl, int>(
            s => s(), ret);

        _backend.nl().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void nocbreak_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nocbreak, int>(
            s => s(), ret);

        _backend.nocbreak().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void noecho_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noecho, int>(
            s => s(), ret);

        _backend.noecho().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void nonl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nonl, int>(
            s => s(), ret);

        _backend.nonl().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void noraw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.noraw, int>(
            s => s(), ret);

        _backend.noraw().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void raw_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.raw, int>(
            s => s(), ret);

        _backend.raw().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void resetty_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.resetty, int>(
            s => s(), ret);

        _backend.resetty().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void reset_prog_mode_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.reset_prog_mode, int>(
            s => s(), ret);

        _backend.reset_prog_mode().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void reset_shell_mode_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.reset_shell_mode, int>(
            s => s(), ret);

        _backend.reset_shell_mode().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void savetty_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.savetty, int>(
            s => s(), ret);

        _backend.savetty().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_clear_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_clear, int>(
            s => s(), ret);

        _backend.slk_clear().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_noutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_noutrefresh, int>(
            s => s(), ret);

        _backend.slk_noutrefresh().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_refresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_refresh, int>(
            s => s(), ret);

        _backend.slk_refresh().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_restore_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_restore, int>(
            s => s(), ret);

        _backend.slk_restore().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_touch_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_touch, int>(
            s => s(), ret);

        _backend.slk_touch().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void start_color_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.start_color, int>(
            s => s(), ret);

        _backend.start_color().ShouldBe(ret);
    }
    [TestMethod, DataRow(0), DataRow(-1)]
    public void use_default_colors_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.use_default_colors, int>(
            s => s(), ret);

        _backend.use_default_colors().ShouldBe(ret);
    }
    
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void has_colors_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.has_colors, bool>(
            s => s(), ret);

        _backend.has_colors().ShouldBe(ret);
    }
    [TestMethod, DataRow(true), DataRow(false)]
    public void has_ic_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.has_ic, bool>(
            s => s(), ret);

        _backend.has_ic().ShouldBe(ret);
    }
    [TestMethod, DataRow(true), DataRow(false)]
    public void has_il_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.has_il, bool>(
            s => s(), ret);

        _backend.has_il().ShouldBe(ret);
    }
    [TestMethod, DataRow(true), DataRow(false)]
    public void can_change_color_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.can_change_color, bool>(
            s => s(), ret);

        _backend.can_change_color().ShouldBe(ret);
    }   
    [TestMethod, DataRow(true), DataRow(false)]
    public void isendwin_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.isendwin, bool>(
            s => s(), ret);

        _backend.isendwin().ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(999)]
    public void initscr_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.initscr, IntPtr>(
            s => s(), new(ret));

        _backend.initscr().ShouldBe(new(ret));
    }
    
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void longname_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.longname, IntPtr>(
            s => s(), h);

        _backend.longname().ShouldBe(ret);
    }
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void termname_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.termname, IntPtr>(
            s => s(), h);

        _backend.termname().ShouldBe(ret);
    }
    
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void curses_version_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.curses_version, IntPtr>(
            s => s(), h);

        _backend.curses_version().ShouldBe(ret);
    }

        
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void clearok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.clearok, int>(
            s => s(h, yes), ret);

        _backend.clearok(h, yes).ShouldBe(ret);
    }
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void idlok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.idlok, int>(
            s => s(h, yes), ret);

        _backend.idlok(h, yes).ShouldBe(ret);
    }
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void intrflush_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.intrflush, int>(
            s => s(h, yes), ret);

        _backend.intrflush(h, yes).ShouldBe(ret);
    }
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void keypad_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.keypad, int>(
            s => s(h, yes), ret);

        _backend.keypad(h, yes).ShouldBe(ret);
    }
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void leaveok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.leaveok, int>(
            s => s(h, yes), ret);

        _backend.leaveok(h, yes).ShouldBe(ret);
    }
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void meta_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.meta, int>(
            s => s(h, yes), ret);

        _backend.meta(h, yes).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void nodelay_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.nodelay, int>(
            s => s(h, yes), ret);

        _backend.nodelay(h, yes).ShouldBe(ret);
    }
    
    
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void notimeout_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.notimeout, int>(
            s => s(h, yes), ret);

        _backend.notimeout(h, yes).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void scrollok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.scrollok, int>(
            s => s(h, yes), ret);

        _backend.scrollok(h, yes).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true, 0), DataRow(false, -1)]
    public void syncok_IsRelayedToLibrary(bool yes, int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.syncok, int>(
            s => s(h, yes), ret);

        _backend.syncok(h, yes).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void idcok_IsRelayedToLibrary(bool yes)
    {
        var h = new IntPtr(999);
        var called = false;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.idcok>().Setup(s => s(h, yes)).Callback((IntPtr _,bool _) =>
        {
            called = true;
        });

        _backend.idcok(h, yes);
        
        called.ShouldBeTrue();
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void immedok_IsRelayedToLibrary(bool yes)
    {
        var h = new IntPtr(999);
        var called = false;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.immedok>().Setup(s => s(h, yes)).Callback((IntPtr _,bool _) =>
        {
            called = true;
        });

        _backend.immedok(h, yes);
        
        called.ShouldBeTrue();
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void use_env_IsRelayedToLibrary(bool yes)
    {
        var called = false;
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.use_env>().Setup(s => s(yes)).Callback((IntPtr _,bool _) =>
        {
            called = true;
        });

        _backend.use_env(yes);
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void wcursyncup_IsRelayedToLibrary()
    {
        var h = new IntPtr(999);
        var called = false;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wcursyncup>().Setup(s => s(h)).Callback((IntPtr _) =>
        {
            called = true;
        });

        _backend.wcursyncup(h);
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void wsyncdown_IsRelayedToLibrary()
    {
        var h = new IntPtr(999);
        var called = false;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wsyncdown>().Setup(s => s(h)).Callback((IntPtr _) =>
        {
            called = true;
        });

        _backend.wsyncdown(h);
        
        called.ShouldBeTrue();
    }
    
    [TestMethod]
    public void wsyncup_IsRelayedToLibrary()
    {
        var h = new IntPtr(999);
        var called = false;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wsyncup>().Setup(s => s(h)).Callback((IntPtr _) =>
        {
            called = true;
        });

        _backend.wsyncup(h);
        
        called.ShouldBeTrue();
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclear_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wclear, int>(
            s => s(h), ret);

        _backend.wclear(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtobot_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wclrtobot, int>(
            s => s(h), ret);

        _backend.wclrtobot(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wclrtoeol_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wclrtoeol, int>(
            s => s(h), ret);

        _backend.wclrtoeol(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wdelch_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wdelch, int>(
            s => s(h), ret);

        _backend.wdelch(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void werase_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.werase, int>(
            s => s(h), ret);

        _backend.werase(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wgetch_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wgetch, int>(
            s => s(h), ret);

        _backend.wgetch(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcury_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcury, int>(
            s => s(h), ret);

        _backend.getcury(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getcurx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getcurx, int>(
            s => s(h), ret);

        _backend.getcurx(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getbegx, int>(
            s => s(h), ret);

        _backend.getbegx(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getbegy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getbegy, int>(
            s => s(h), ret);

        _backend.getbegy(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmaxx, int>(
            s => s(h), ret);

        _backend.getmaxx(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getmaxy_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getmaxy, int>(
            s => s(h), ret);

        _backend.getmaxy(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getparx_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getparx, int>(
            s => s(h), ret);

        _backend.getparx(h).ShouldBe(ret);
    }
    
      
    [TestMethod, DataRow(0), DataRow(-1)]
    public void getpary_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.getpary, int>(
            s => s(h), ret);

        _backend.getpary(h).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attroff_IsRelayedToLibrary(int ret)
    {
        const uint i = 999U;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attroff, int>(
            s => s(i), ret);

        _backend.slk_attroff(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attron_IsRelayedToLibrary(int ret)
    {
        const uint i = 999U;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attron, int>(
            s => s(i), ret);

        _backend.slk_attron(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attrset_IsRelayedToLibrary(int ret)
    {
        const uint i = 999U;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attrset, int>(
            s => s(i), ret);

        _backend.slk_attrset(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0u), DataRow(-1u)]
    public void COLOR_PAIR_IsRelayedToLibrary(uint ret)
    {
        const uint i = 999U;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.COLOR_PAIR, uint>(
            s => s(i), ret);

        _backend.COLOR_PAIR(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0u), DataRow(-1u)]
    public void PAIR_NUMBER_IsRelayedToLibrary(uint ret)
    {
        const uint i = 999U;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.PAIR_NUMBER, uint>(
            s => s(i), ret);

        _backend.PAIR_NUMBER(i).ShouldBe(ret);
    }
    
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void halfdelay_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.halfdelay, int>(
            s => s(i), ret);

        _backend.halfdelay(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void curs_set_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.curs_set, int>(
            s => s(i), ret);

        _backend.curs_set(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void delay_output_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.delay_output, int>(
            s => s(i), ret);

        _backend.delay_output(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_init_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_init, int>(
            s => s(i), ret);

        _backend.slk_init(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void mouseinterval_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mouseinterval, int>(
            s => s(i), ret);

        _backend.mouseinterval(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void set_tabsize_IsRelayedToLibrary(int ret)
    {
        const int i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.set_tabsize, int>(
            s => s(i), ret);

        _backend.set_tabsize(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void slk_label_IsRelayedToLibrary(string ret)
    {
        const int i = 999;
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_label, IntPtr>(
            s => s(i), h);

        _backend.slk_label(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_color_IsRelayedToLibrary(int ret)
    {
        const short i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_color, int>(
            s => s(i), ret);

        _backend.slk_color(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void ungetch_IsRelayedToLibrary(int ret)
    {
        const uint i = 999;
        
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.ungetch, int>(
            s => s(i), ret);

        _backend.ungetch(i).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr, int>(
            s => s(), ret);

        _backend.slk_attr().ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_term_resized_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_term_resized, bool>(
            s => s(10, 20), ret);

        _backend.is_term_resized(10, 20).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void resize_term_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.resize_term, int>(
            s => s(10, 20), ret);

        _backend.resize_term(10, 20).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void resizeterm_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.resizeterm, int>(
            s => s(10, 20), ret);

        _backend.resizeterm(10, 20).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(99), DataRow(0)]
    public void newpad_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.newpad, IntPtr>(
            s => s(10, 20), new(ret));

        _backend.newpad(10, 20).ShouldBe(new(ret));
    }
    
    [TestMethod, DataRow(99), DataRow(0)]
    public void newwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.newwin, IntPtr>(
            s => s(10, 20, 30, 40), new(ret));

        _backend.newwin(10, 20, 30, 40).ShouldBe(new(ret));
    }
      
    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mvwin, int>(
            s => s(h, 30, 40), ret);

        _backend.mvwin(h, 30, 40).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(-1), DataRow(0)]
    public void mvderwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.mvderwin, int>(
            s => s(h, 30, 40), ret);

        _backend.mvderwin(h, 30, 40).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(99), DataRow(0)]
    public void derwin_IsRelayedToLibrary(int ret)
    {
        var h = new IntPtr(999);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.derwin, IntPtr>(
            s => s(h, 10, 20, 30, 40), new(ret));

        _backend.derwin(h, 10, 20, 30, 40).ShouldBe(new(ret));
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void copywin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.copywin, int>(
            s => s(new(1), new(2), 20, 30, 40, 
                50, 60, 70, 80), ret);

        _backend.copywin(new(1), new(2), 20, 30, 40, 
            50, 60, 70, 80).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void whline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.whline, int>(
            s => s(new(1), 'A', 10), ret);

        _backend.whline(new(1), 'A', 10).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wvline_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wvline, int>(
            s => s(new(1), 'A', 10), ret);

        _backend.wvline(new(1), 'A', 10).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wsetscrreg_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wsetscrreg, int>(
            s => s(new(1), 1, 10), ret);

        _backend.wsetscrreg(new(1), 1, 10).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wredrawln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wredrawln, int>(
            s => s(new(1), 1, 10), ret);

        _backend.wredrawln(new(1), 1, 10).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wtouchln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wtouchln, int>(
            s => s(new(1), 2, 10, 1), ret);

        _backend.wtouchln(new(1), 2, 10, 1).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void wenclose_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wenclose, bool>(
            s => s(new(1), 1, 2), ret);

        _backend.wenclose(new(1), 1, 2).ShouldBe(ret);
    }
    
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void keybound_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.keybound, IntPtr>(
            s => s('A', 2), h);

        _backend.keybound('A', 2).ShouldBe(ret);
    }
    
    [TestMethod, DataRow((string?)null), DataRow("hello")]
    public void keyname_IsRelayedToLibrary(string ret)
    {
        var h = Marshal.StringToHGlobalAnsi(ret);
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.keyname, IntPtr>(
            s => s('A'), h);

        _backend.keyname('A').ShouldBe(ret);
    }

    [TestMethod, DataRow(99), DataRow(0)]
    public void dupwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.dupwin, IntPtr>(
            s => s(new(1)), new(ret));

        _backend.dupwin(new(1)).ShouldBe(new(ret));
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void is_linetouched_IsRelayedToLibrary(bool ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.is_linetouched, bool>(
            s => s(new(1), 2), ret);

        _backend.is_linetouched(new(1), 2).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void delwin_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.delwin, int>(
            s => s(new(1)), ret);

        _backend.delwin(new(1)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void overlay_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.overlay, int>(
            s => s(new(1), new(2)), ret);

        _backend.overlay(new(1), new(2)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void overwrite_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.overwrite, int>(
            s => s(new(1), new(2)), ret);

        _backend.overwrite(new(1), new(2)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void pechochar_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.pechochar, int>(
            s => s(new(1), 'A'), ret);

        _backend.pechochar(new(1), 'A').ShouldBe(ret);
    }
     
    [TestMethod, DataRow(0), DataRow(-1)]
    public void winsch_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.winsch, int>(
            s => s(new(1), 'A'), ret);

        _backend.winsch(new(1), 'A').ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_on_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_on, int>(
            s => s(new(1), 1, new(2)), ret);

        _backend.wattr_on(new(1), 1, new(2)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wattr_off_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wattr_off, int>(
            s => s(new(1), 1, new(2)), ret);

        _backend.wattr_off(new(1), 1, new(2)).ShouldBe(ret);
    }
    
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void slk_attr_set_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.slk_attr_set, int>(
            s => s(1, 2, new(2)), ret);

        _backend.slk_attr_set(1, 2, new(2)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void waddch_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.waddch, int>(
            s => s(new(1), 'A'), ret);

        _backend.waddch(new(1), 'A').ShouldBe(ret);
    }
     
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wbkgd_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wbkgd, int>(
            s => s(new(1), 'A'), ret);

        _backend.wbkgd(new(1), 'A').ShouldBe(ret);
    }
    
    [TestMethod, DataRow(0), DataRow(-1)]
    public void wechochar_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wechochar, int>(
            s => s(new(1), 'A'), ret);

        _backend.wechochar(new(1), 'A').ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void winch_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.winch, int>(
            s => s(new(1)), ret);

        _backend.winch(new(1)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void winsdelln_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.winsdelln, int>(
            s => s(new(1), 1), ret);

        _backend.winsdelln(new(1), 1).ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wmove_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wmove, int>(
            s => s(new(1), 1, 2), ret);

        _backend.wmove(new(1), 1, 2).ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wnoutrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wnoutrefresh, int>(
            s => s(new(1)), ret);

        _backend.wnoutrefresh(new(1)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wrefresh_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wrefresh, int>(
            s => s(new(1)), ret);

        _backend.wrefresh(new(1)).ShouldBe(ret);
    }
    
    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wscrl_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wscrl, int>(
            s => s(new(1), 1), ret);

        _backend.wscrl(new(1), 1).ShouldBe(ret);
    }

    [TestMethod, DataRow('A'), DataRow(-1)]
    public void wresize_IsRelayedToLibrary(int ret)
    {
        _nativeSymbolResolverMock.MockResolve<NCursesFunctionMap.wresize, int>(
            s => s(new(1), 1, 2), ret);

        _backend.wresize(new(1), 1, 2).ShouldBe(ret);
    }
}
