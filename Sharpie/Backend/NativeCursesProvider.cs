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

#pragma warning disable CS1591
namespace Sharpie.Backend;

/// <summary>
///     Interface provides access to the Curses functionality. Use the <see cref="System" /> property to access the actual
///     implementation.
/// </summary>
[PublicAPI, SuppressMessage("ReSharper", "IdentifierTypo"), ExcludeFromCodeCoverage]
public sealed class NativeCursesProvider: ICursesProvider, IDisposable
{
    private const string CursesLibraryName = "ncurses";
    private const string LibCLibraryName = "libc";

    private static readonly ICursesProvider? LazyInstance = new NativeCursesProvider().ValidOrNull();

    private PosixSignalRegistration? _signalRegistration;

    private NativeCursesProvider() { }

    /// <summary>
    ///     Returns the instance of the Curses backend.
    /// </summary>
    /// <exception cref="CursesInitializationException">Thrown if Curses library not available.</exception>
    public static ICursesProvider Instance => LazyInstance ?? throw new CursesInitializationException();

    /// <summary>
    ///     Checks if the Curses library is available.
    /// </summary>
    public static bool IsAvailable => LazyInstance != null;

    bool ICursesProvider.is_cleared(IntPtr window) => is_cleared(window);

    bool ICursesProvider.is_idcok(IntPtr window) => is_idcok(window);

    bool ICursesProvider.is_idlok(IntPtr window) => is_idlok(window);

    bool ICursesProvider.is_immedok(IntPtr window) => is_immedok(window);

    bool ICursesProvider.is_keypad(IntPtr window) => is_keypad(window);

    bool ICursesProvider.is_leaveok(IntPtr window) => is_leaveok(window);

    bool ICursesProvider.is_nodelay(IntPtr window) => is_nodelay(window);

    bool ICursesProvider.is_notimeout(IntPtr window) => is_notimeout(window);

    bool ICursesProvider.is_scrollok(IntPtr window) => is_scrollok(window);

    bool ICursesProvider.is_syncok(IntPtr window) => is_syncok(window);

    IntPtr ICursesProvider.wgetparent(IntPtr window) => wgetparent(window);

    int ICursesProvider.wgetdelay(IntPtr window) => wgetdelay(window);

    int ICursesProvider.wgetscrreg(IntPtr window, out int top, out int bottom) =>
        wgetscrreg(window, out top, out bottom);

    int ICursesProvider.baudrate() => baudrate();

    int ICursesProvider.beep() => beep();

    bool ICursesProvider.can_change_color() => can_change_color();

    int ICursesProvider.cbreak() => cbreak();

    int ICursesProvider.clearok(IntPtr window, bool set) => clearok(window, set);

    int ICursesProvider.color_content(short color, out short red, out short green, out short blue) =>
        color_content(color, out red, out green, out blue);

    int ICursesProvider.copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay) =>
        copywin(fromWindow, toWindow, srcStartLine, srcStartCol, destStartLine,
            destStartCol, destEndLine, destEndCol, overlay);

    int ICursesProvider.curs_set(int level) => curs_set(level);

    int ICursesProvider.def_prog_mode() => def_prog_mode();

    int ICursesProvider.def_shell_mode() => def_shell_mode();

    int ICursesProvider.delay_output(int delayMillis) => delay_output(delayMillis);

    int ICursesProvider.delwin(IntPtr window) => delwin(window);

    IntPtr ICursesProvider.derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol) =>
        derwin(window, lines, cols, beginLine, beginCol);

    int ICursesProvider.doupdate() => doupdate();

    IntPtr ICursesProvider.dupwin(IntPtr window) => dupwin(window);

    int ICursesProvider.echo() => echo();

    int ICursesProvider.endwin() => endwin();

    int ICursesProvider.erasewchar(out uint @char) => erasewchar(out @char);

    void ICursesProvider.filter() { filter(); }

    int ICursesProvider.flash() => flash();

    int ICursesProvider.flushinp() => flushinp();

    uint ICursesProvider.getattrs(IntPtr window) => getattrs(window);

    int ICursesProvider.getcurx(IntPtr window) => getcurx(window);

    int ICursesProvider.getcury(IntPtr window) => getcury(window);

    int ICursesProvider.getbegx(IntPtr window) => getbegx(window);

    int ICursesProvider.getbegy(IntPtr window) => getbegy(window);

    int ICursesProvider.getmaxx(IntPtr window) => getmaxx(window);

    int ICursesProvider.getmaxy(IntPtr window) => getmaxy(window);

    int ICursesProvider.getparx(IntPtr window) => getparx(window);

    int ICursesProvider.getpary(IntPtr window) => getpary(window);

    int ICursesProvider.halfdelay(int tenthsOfSec) => halfdelay(tenthsOfSec);

    bool ICursesProvider.has_colors() => has_colors();

    bool ICursesProvider.has_ic() => has_ic();

    bool ICursesProvider.has_il() => has_il();

    void ICursesProvider.idcok(IntPtr window, bool set) => idcok(window, set);

    int ICursesProvider.idlok(IntPtr window, bool set) => idlok(window, set);

    void ICursesProvider.immedok(IntPtr window, bool set) => immedok(window, set);

    IntPtr ICursesProvider.initscr() => initscr();

    int ICursesProvider.init_color(short color, short red, short green, short blue) =>
        init_color(color, red, green, blue);

    int ICursesProvider.init_pair(short colorPair, short fgColor, short bgColor) =>
        init_pair(colorPair, fgColor, bgColor);

    int ICursesProvider.intrflush(IntPtr window, bool set) => intrflush(window, set);

    bool ICursesProvider.isendwin() => isendwin();

    bool ICursesProvider.is_linetouched(IntPtr window, int line) => is_linetouched(window, line);

    bool ICursesProvider.is_wintouched(IntPtr window) => is_wintouched(window);

    string? ICursesProvider.keyname(uint keyCode) => Marshal.PtrToStringAnsi(keyname(keyCode));

    int ICursesProvider.keypad(IntPtr window, bool set) => keypad(window, set);

    int ICursesProvider.leaveok(IntPtr window, bool set) => leaveok(window, set);

    string? ICursesProvider.longname() => Marshal.PtrToStringAnsi(longname());

    int ICursesProvider.meta(IntPtr window, bool set) => meta(window, set);

    int ICursesProvider.mvderwin(IntPtr window, int parentLine, int parentCol) =>
        mvderwin(window, parentLine, parentCol);

    int ICursesProvider.mvwin(IntPtr window, int toLine, int toCol) => mvwin(window, toLine, toCol);

    IntPtr ICursesProvider.newpad(int lines, int cols) => newpad(lines, cols);

    IntPtr ICursesProvider.newwin(int lines, int cols, int atLine, int atCol) => newwin(lines, cols, atLine, atCol);

    int ICursesProvider.nl() => nl();

    int ICursesProvider.nocbreak() => nocbreak();

    int ICursesProvider.nodelay(IntPtr window, bool set) => nodelay(window, set);

    int ICursesProvider.noecho() => noecho();

    int ICursesProvider.nonl() => nonl();

    void ICursesProvider.noqiflush() { noqiflush(); }

    int ICursesProvider.noraw() => noraw();

    int ICursesProvider.notimeout(IntPtr window, bool set) => notimeout(window, set);

    int ICursesProvider.overlay(IntPtr srcWindow, IntPtr destWindow) => overlay(srcWindow, destWindow);

    int ICursesProvider.overwrite(IntPtr srcWindow, IntPtr destWindow) => overwrite(srcWindow, destWindow);

    int ICursesProvider.pair_content(short colorPair, out short fgColor, out short bgColor) =>
        pair_content(colorPair, out fgColor, out bgColor);

    uint ICursesProvider.COLOR_PAIR(uint attrs) => COLOR_PAIR(attrs);

    uint ICursesProvider.PAIR_NUMBER(uint colorPair) => PAIR_NUMBER(colorPair);

    int ICursesProvider.pechochar(IntPtr pad, uint @char) => pechochar(pad, @char);

    int ICursesProvider.pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        pnoutrefresh(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    int ICursesProvider.prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        prefresh(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    void ICursesProvider.qiflush() { qiflush(); }

    int ICursesProvider.raw() => raw();

    int ICursesProvider.resetty() => resetty();

    int ICursesProvider.reset_prog_mode() => reset_prog_mode();

    int ICursesProvider.reset_shell_mode() => reset_shell_mode();

    int ICursesProvider.ripoffline(int lines, ICursesProvider.ripoffline_callback callback) =>
        ripoffline(lines, callback);

    int ICursesProvider.savetty() => savetty();

    int ICursesProvider.scrollok(IntPtr window, bool set) => scrollok(window, set);

    int ICursesProvider.slk_attroff(uint attrs) => slk_attroff(attrs);

    int ICursesProvider.slk_attr_off(uint attrs, IntPtr reserved) => slk_attr_off(attrs, reserved);

    int ICursesProvider.slk_attron(uint attrs) => slk_attron(attrs);

    int ICursesProvider.slk_attr_on(uint attrs, IntPtr reserved) => slk_attr_on(attrs, reserved);

    int ICursesProvider.slk_attrset(uint attrs) => slk_attrset(attrs);

    int ICursesProvider.slk_attr() => slk_attr();

    int ICursesProvider.slk_attr_set(uint attrs, short colorPair, IntPtr reserved) =>
        slk_attr_set(attrs, colorPair, reserved);

    int ICursesProvider.slk_clear() => slk_clear();

    int ICursesProvider.slk_color(short colorPair) => slk_color(colorPair);

    int ICursesProvider.slk_init(int format) => slk_init(format);

    string? ICursesProvider.slk_label(int labelIndex) => Marshal.PtrToStringAnsi(slk_label(labelIndex));

    int ICursesProvider.slk_noutrefresh() => slk_noutrefresh();

    int ICursesProvider.slk_refresh() => slk_refresh();

    int ICursesProvider.slk_restore() => slk_restore();

    int ICursesProvider.slk_touch() => slk_touch();

    int ICursesProvider.start_color() => start_color();

    IntPtr ICursesProvider.subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol) =>
        subpad(pad, lines, cols, atLine, atCol);

    IntPtr ICursesProvider.subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol) =>
        subwin(window, lines, cols, atLine, atCol);

    int ICursesProvider.syncok(IntPtr window, bool set) => syncok(window, set);

    string? ICursesProvider.termname() => Marshal.PtrToStringAnsi(termname());

    int ICursesProvider.ungetch(uint @char) => ungetch(@char);

    void ICursesProvider.use_env(bool set) { use_env(set); }

    int ICursesProvider.waddch(IntPtr window, uint charAndAttrs) => waddch(window, charAndAttrs);

    int ICursesProvider.waddchnstr(IntPtr window, uint[] charsAndAttrs, int length) =>
        waddchnstr(window, charsAndAttrs, length);

    int ICursesProvider.wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved) =>
        wattr_get(window, out attrs, out colorPair, reserved);

    int ICursesProvider.wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved) =>
        wattr_set(window, attrs, colorPair, reserved);

    int ICursesProvider.wattr_on(IntPtr window, uint attrs, IntPtr reserved) => wattr_on(window, attrs, reserved);

    int ICursesProvider.wattr_off(IntPtr window, uint attrs, IntPtr reserved) => wattr_off(window, attrs, reserved);

    int ICursesProvider.wbkgd(IntPtr window, uint charAndAttrs) => wbkgd(window, charAndAttrs);

    void ICursesProvider.wbkgdset(IntPtr window, uint charAndAttrs) => wbkgdset(window, charAndAttrs);

    int ICursesProvider.wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner) =>
        wborder(window, leftSide, rightSide, topSide, bottomSide,
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);

    int ICursesProvider.wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved) =>
        wchgat(window, count, attrs, colorPair, reserved);

    int ICursesProvider.wclear(IntPtr window) => wclear(window);

    int ICursesProvider.wclrtobot(IntPtr window) => wclrtobot(window);

    int ICursesProvider.wclrtoeol(IntPtr window) => wclrtoeol(window);

    int ICursesProvider.wcolor_set(IntPtr window, short colorPair, IntPtr reserved) =>
        wcolor_set(window, colorPair, reserved);

    void ICursesProvider.wcursyncup(IntPtr window) => wcursyncup(window);

    int ICursesProvider.wdelch(IntPtr window) => wdelch(window);

    int ICursesProvider.wechochar(IntPtr window, uint charAndAttrs) => wechochar(window, charAndAttrs);

    int ICursesProvider.werase(IntPtr window) => werase(window);

    int ICursesProvider.wgetch(IntPtr window) => wgetch(window);

    int ICursesProvider.wgetnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        wgetnstr(window, dest, length);

    int ICursesProvider.whline(IntPtr window, uint @char, int count) => whline(window, @char, count);

    uint ICursesProvider.winch(IntPtr window) => winch(window);

    int ICursesProvider.winchnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        winchnstr(window, dest, length);

    int ICursesProvider.winsch(IntPtr window, uint charAndAttrs) => winsch(window, charAndAttrs);

    int ICursesProvider.winsdelln(IntPtr window, int count) => winsdelln(window, count);

    int ICursesProvider.wmove(IntPtr window, int newLine, int newCol) => wmove(window, newLine, newCol);

    int ICursesProvider.wnoutrefresh(IntPtr window) => wnoutrefresh(window);

    int ICursesProvider.wredrawln(IntPtr window, int startLine, int lineCount) =>
        wredrawln(window, startLine, lineCount);

    int ICursesProvider.wrefresh(IntPtr window) => wrefresh(window);

    int ICursesProvider.wscrl(IntPtr window, int lines) => wscrl(window, lines);

    int ICursesProvider.wsetscrreg(IntPtr window, int top, int bottom) => wsetscrreg(window, top, bottom);

    void ICursesProvider.wsyncdown(IntPtr window) => wsyncdown(window);

    void ICursesProvider.wsyncup(IntPtr window) => wsyncup(window);

    void ICursesProvider.wtimeout(IntPtr window, int delay) => wtimeout(window, delay);

    int ICursesProvider.wtouchln(IntPtr window, int line, int count, int changed) =>
        wtouchln(window, line, count, changed);

    int ICursesProvider.wvline(IntPtr window, uint @char, int count) => wvline(window, @char, count);

    bool ICursesProvider.is_term_resized(int lines, int cols) => is_term_resized(lines, cols);

    int ICursesProvider.resize_term(int lines, int cols) => resize_term(lines, cols);

    int ICursesProvider.resizeterm(int lines, int cols) => resizeterm(lines, cols);

    string? ICursesProvider.keybound(uint keyCode, int count) => Marshal.PtrToStringAnsi(keybound(keyCode, count));

    string? ICursesProvider.curses_version() => Marshal.PtrToStringAnsi(curses_version());

    int ICursesProvider.assume_default_colors(int fgColor, int bgColor) => assume_default_colors(fgColor, bgColor);

    int ICursesProvider.define_key(string keyName, int keyCode) => define_key(keyName, keyCode);

    int ICursesProvider.key_defined(string keyName) => key_defined(keyName);

    int ICursesProvider.keyok(int keyCode, bool set) => keyok(keyCode, set);

    int ICursesProvider.set_tabsize(int size) => set_tabsize(size);

    int ICursesProvider.use_default_colors() => use_default_colors();

    int ICursesProvider.wresize(IntPtr window, int lines, int columns) => wresize(window, lines, columns);

    void ICursesProvider.nofilter() => nofilter();

    int ICursesProvider.getcchar(CursesComplexChar @char, StringBuilder dest, out uint attrs, out short colorPair,
        IntPtr reserved) =>
        getcchar(ref @char, dest, out attrs, out colorPair, reserved);

    string? ICursesProvider.key_name(uint @char) => Marshal.PtrToStringAnsi(key_name(@char));

    int ICursesProvider.killwchar(out uint @char) => killwchar(out @char);

    int ICursesProvider.pecho_wchar(IntPtr window, CursesComplexChar @char) => pecho_wchar(window, ref @char);

    int ICursesProvider.setcchar(out CursesComplexChar @char, string text, uint attrs, short colorPair,
        IntPtr reserved) =>
        setcchar(out @char, text, attrs, colorPair, reserved);

    int ICursesProvider.slk_set(int labelIndex, string title, int align) => slk_set(labelIndex, title, align);

    uint ICursesProvider.term_attrs() => term_attrs();

    int ICursesProvider.unget_wch(uint @char) => unget_wch(@char);

    int ICursesProvider.wadd_wch(IntPtr window, CursesComplexChar @char) => wadd_wch(window, ref @char);

    int ICursesProvider.wadd_wchnstr(IntPtr window, CursesComplexChar[] str, int count) =>
        wadd_wchnstr(window, str, count);

    int ICursesProvider.waddnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] string text, int length) =>
        waddnwstr(window, text, length);

    int ICursesProvider.wbkgrnd(IntPtr window, CursesComplexChar @char) => wbkgrnd(window, ref @char);

    void ICursesProvider.wbkgrndset(IntPtr window, CursesComplexChar @char) => wbkgrndset(window, ref @char);

    int ICursesProvider.wborder_set(IntPtr window, CursesComplexChar leftSide, CursesComplexChar rightSide,
        CursesComplexChar topSide, CursesComplexChar bottomSide, CursesComplexChar topLeftCorner,
        CursesComplexChar topRightCorner, CursesComplexChar bottomLeftCorner, CursesComplexChar bottomRightCorner) =>
        wborder_set(window, ref leftSide, ref rightSide, ref topSide, ref bottomSide,
            ref topLeftCorner, ref topRightCorner, ref bottomLeftCorner, ref bottomRightCorner);

    int ICursesProvider.wecho_wchar(IntPtr window, CursesComplexChar @char) => wecho_wchar(window, ref @char);

    int ICursesProvider.wget_wch(IntPtr window, out uint @char) => wget_wch(window, out @char);

    int ICursesProvider.wgetbkgrnd(IntPtr window, out CursesComplexChar @char) => wgetbkgrnd(window, out @char);

    int ICursesProvider.wgetn_wstr(IntPtr window, StringBuilder dest, int length) => wgetn_wstr(window, dest, length);

    int ICursesProvider.whline_set(IntPtr window, CursesComplexChar @char, int count) =>
        whline_set(window, ref @char, count);

    int ICursesProvider.win_wch(IntPtr window, out CursesComplexChar @char) => win_wch(window, out @char);

    int ICursesProvider.win_wchnstr(IntPtr window, CursesComplexChar[] dest, int length) =>
        win_wchnstr(window, dest, length);

    int ICursesProvider.winnwstr(IntPtr window, StringBuilder dest, int length) => winnwstr(window, dest, length);

    int ICursesProvider.wins_nwstr(IntPtr window, string text, int length) => wins_nwstr(window, text, length);

    int ICursesProvider.wins_wch(IntPtr window, CursesComplexChar @char) => wins_wch(window, ref @char);

    string? ICursesProvider.wunctrl(CursesComplexChar @char) => Marshal.PtrToStringUni(wunctrl(ref @char));

    int ICursesProvider.wvline_set(IntPtr window, CursesComplexChar @char, int count) =>
        wvline_set(window, ref @char, count);

    int ICursesProvider.getmouse(out CursesMouseEvent @event) => getmouse(out @event);

    int ICursesProvider.ungetmouse(CursesMouseEvent @event) => ungetmouse(ref @event);

    int ICursesProvider.mousemask(int newMask, out int oldMask)
    {
        var result = mousemask(newMask, out oldMask);
        if (!result.Failed())
        {
            var csi = "\x1b[?1003l";
            if ((newMask & (int) CursesMouseEvent.EventType.ReportPosition) != 0)
            {
                csi = "\x1b[?1003h";
            } else if ((newMask & (int) CursesMouseEvent.EventType.All) != 0)
            {
                csi = "\x1b[?1000h";
            }

            // Force enable mouse reporting. Curses doesn't always want to do that.
            Console.Out.Write(csi);
            Console.Out.Flush();
        }

        return result;
    }

    bool ICursesProvider.wenclose(IntPtr window, int line, int col) => wenclose(window, line, col);

    int ICursesProvider.mouseinterval(int millis) => mouseinterval(millis);

    bool ICursesProvider.wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen) =>
        wmouse_trafo(window, ref line, ref col, toScreen);

    void ICursesProvider.set_title(string title) { Console.Title = title; }

    void ICursesProvider.set_unicode_locale()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            setlocale(6, "");
        } else if (OperatingSystem.IsMacOS())
        {
            setlocale(0, "");
        }
    }

    bool ICursesProvider.monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
        {
            handle = PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ => { action(); });

            return true;
        }

        handle = null;
        return false;
    }

    public void Dispose()
    {
        if (_signalRegistration != null)
        {
            _signalRegistration?.Dispose();
            _signalRegistration = null;
        }

        GC.SuppressFinalize(this);
    }

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int baudrate();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int beep();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool can_change_color();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int cbreak();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int clearok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int color_content(short color, out short red, out short green, out short blue);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int copywin(IntPtr fromWindow, IntPtr toWindow, int srcMinLine, int srcMinCol,
        int destMinLine, int destMinCol, int destMaxLine, int destMaxCol,
        int overlay);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int curs_set(int level);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int def_prog_mode();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int def_shell_mode();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int delay_output(int delayMillis);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int delwin(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int doupdate();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr dupwin(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int echo();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int endwin();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern char erasechar();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void filter();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int flash();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int flushinp();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int halfdelay(int tenthsOfSec);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool has_colors();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool has_ic();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool has_il();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void idcok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int idlok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void immedok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr initscr();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int init_color(short color, short red, short green, short blue);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int init_pair(short colorPair, short fgColor, short bgColor);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int intrflush(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool isendwin();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_linetouched(IntPtr window, int line);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_wintouched(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr keyname(uint keyCode);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int keypad(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern char killchar();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int leaveok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr longname();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int meta(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int mvderwin(IntPtr window, int parentLine, int parentCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int mvwin(IntPtr window, int toLine, int toCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr newpad(int lines, int cols);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr newwin(int lines, int cols, int atLine, int atCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int nl();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int nocbreak();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int nodelay(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int noecho();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int nonl();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void noqiflush();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int noraw();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int notimeout(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int overlay(IntPtr srcWindow, IntPtr destWindow);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int overwrite(IntPtr srcWindow, IntPtr destWindow);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int pair_content(short colorPair, out short fgColor, out short bgColor);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint COLOR_PAIR(uint attrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint PAIR_NUMBER(uint colorPair);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int pechochar(IntPtr pad, uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void qiflush();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int raw();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int resetty();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int reset_prog_mode();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int reset_shell_mode();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ripoffline(int lines, ICursesProvider.ripoffline_callback callback);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int savetty();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int scrollok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attroff(uint attrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attron(uint attrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attrset(uint attrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern char slk_attr();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attr_set(uint attrs, short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_clear();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_color(short colorPair);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_init(int format);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr slk_label(int labelIndex);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_noutrefresh();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_refresh();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_restore();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_touch();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int start_color();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr subpad(IntPtr pad, int lines, int cols, int atRow,
        int atCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int syncok(IntPtr window, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint termattrs();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr termname();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ungetch(uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void use_env(bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int waddch(IntPtr window, uint charAndAttrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int waddchnstr(IntPtr window, uint[] charsAndAttrs, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wattr_on(IntPtr window, uint attrs, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wattr_off(IntPtr window, uint attrs, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wbkgd(IntPtr window, uint charAndAttrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wbkgdset(IntPtr window, uint charAndAttrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wclear(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wclrtobot(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wclrtoeol(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wcolor_set(IntPtr window, short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wcursyncup(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wdelch(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wechochar(IntPtr window, uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int werase(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wgetch(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int wgetnstr(IntPtr window, StringBuilder dest, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int whline(IntPtr window, uint @char, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint winch(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int winchnstr(IntPtr window, StringBuilder dest, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int winsch(IntPtr window, uint charAndAttrs);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int winsdelln(IntPtr window, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wmove(IntPtr window, int newLine, int newCol);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wnoutrefresh(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wredrawln(IntPtr window, int startLine, int lineCount);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wrefresh(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wscrl(IntPtr window, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wsetscrreg(IntPtr window, int top, int bottom);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wsyncdown(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wsyncup(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wtimeout(IntPtr window, int delay);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wtouchln(IntPtr window, int line, int count, int changed);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wvline(IntPtr window, uint @char, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_term_resized(int lines, int cols);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int resize_term(int lines, int cols);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int resizeterm(int lines, int cols);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr keybound(uint keyCode, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr curses_version();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int assume_default_colors(int fgColor, int bgColor);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int define_key(string keyName, int keyCode);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int key_defined(string keyName);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int keyok(int keyCode, bool set);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int set_tabsize(int size);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int use_default_colors();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wresize(IntPtr window, int lines, int columns);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void nofilter();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getcchar(ref CursesComplexChar @char,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, out uint attrs, out short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr key_name(uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int killwchar(out uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int pecho_wchar(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int setcchar(out CursesComplexChar @char, [MarshalAs(UnmanagedType.LPWStr)] string text,
        uint attrs, short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int slk_set(int labelIndex, string title, int fmt);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint term_attrs();

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int unget_wch(uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wadd_wch(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wadd_wchnstr(IntPtr window, CursesComplexChar[] @char, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int waddnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] string text, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wbkgrnd(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void wbkgrndset(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wborder_set(IntPtr window, ref CursesComplexChar leftSide,
        ref CursesComplexChar rightSide, ref CursesComplexChar topSide, ref CursesComplexChar bottomSide,
        ref CursesComplexChar topLeftCorner, ref CursesComplexChar topRightCorner,
        ref CursesComplexChar bottomLeftCorner, ref CursesComplexChar bottomRightCorner);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wecho_wchar(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wget_wch(IntPtr window, out uint dest);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wgetbkgrnd(IntPtr window, out CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wgetn_wstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest,
        int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int whline_set(IntPtr window, ref CursesComplexChar @char, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int win_wch(IntPtr window, out CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int win_wchnstr(IntPtr window, CursesComplexChar[] @char, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int winnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder text, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wins_nwstr(IntPtr window, string text, int length);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wins_wch(IntPtr window, ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int winwstr(IntPtr window, string text);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr wunctrl(ref CursesComplexChar @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wvline_set(IntPtr window, ref CursesComplexChar @char, int count);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int erasewchar(out uint @char);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint getattrs(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getcurx(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getcury(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getbegx(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getbegy(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getmaxx(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getmaxy(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getparx(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getpary(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attr_off(uint attrs, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int slk_attr_on(uint attrs, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_cleared(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_idcok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_idlok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_immedok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_keypad(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_leaveok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_nodelay(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_notimeout(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_scrollok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_syncok(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr wgetparent(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wgetdelay(IntPtr window);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wgetscrreg(IntPtr window, out int top, out int bottom);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getmouse(out CursesMouseEvent @event);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ungetmouse(ref CursesMouseEvent @event);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int mousemask(int newMask, out int oldMask);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool wenclose(IntPtr window, int line, int col);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int mouseinterval(int millis);

    [DllImport(CursesLibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen);

    [DllImport(LibCLibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int setlocale(int cate, string locale);

    ~NativeCursesProvider() { Dispose(); }
}
