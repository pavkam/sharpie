#pragma warning disable CS1591

namespace Sharpie.Backend;

[PublicAPI]
public class NativeCursesProvider: ICursesProvider
{
    private readonly INativeSymbolResolver _cursesLibraryResolver;

    public static ICursesProvider Load(Func<string, IEnumerable<string>> libPathResolver)
    {
        if (libPathResolver == null)
        {
            throw new ArgumentNullException(nameof(libPathResolver));
        }

        var cw = NativeLibraryWrapper<CursesFunctionMap>.TryLoad(libPathResolver("ncurses"));
        if (cw == null)
        {
            throw new CursesInitializationException();
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() || OperatingSystem.IsMacOS())
        {
            var lw = NativeLibraryWrapper<LibCFunctionMap>.TryLoad(libPathResolver("libc"));
            if (lw == null)
            {
                throw new CursesInitializationException();
            }
            
            return new UnixNativeCursesProvider(cw, lw);
        }

        return new NativeCursesProvider(cw);
    }

    public static ICursesProvider Load()
    {
        return Load(s =>
        {
            return new[] { s };
        });
    }

    internal NativeCursesProvider(INativeSymbolResolver cursesLibraryResolver) =>
        _cursesLibraryResolver =
            cursesLibraryResolver ?? throw new ArgumentNullException(nameof(cursesLibraryResolver));

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public bool is_cleared(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_cleared>()(window);

    public bool is_idcok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_idcok>()(window);

    public bool is_idlok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_idlok>()(window);

    public bool is_immedok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_immedok>()(window);

    public bool is_keypad(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_keypad>()(window);

    public bool is_leaveok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_leaveok>()(window);

    public bool is_nodelay(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_nodelay>()(window);

    public bool is_notimeout(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_notimeout>()(window);

    public bool is_scrollok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_scrollok>()(window);

    public bool is_syncok(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.is_syncok>()(window);

    public IntPtr wgetparent(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetparent>()(window);

    public int wgetscrreg(IntPtr window, out int top, out int bottom) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetscrreg>()(window, out top, out bottom);

    public int baudrate() => _cursesLibraryResolver.Resolve<CursesFunctionMap.baudrate>()();

    public int beep() => _cursesLibraryResolver.Resolve<CursesFunctionMap.beep>()();

    public bool can_change_color() => _cursesLibraryResolver.Resolve<CursesFunctionMap.can_change_color>()();

    public int cbreak() => _cursesLibraryResolver.Resolve<CursesFunctionMap.cbreak>()();

    public int clearok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.clearok>()(window, set);

    public int color_content(short color, out short red, out short green, out short blue) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.color_content>()(color, out red, out green, out blue);

    public int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.copywin>()(fromWindow, toWindow, srcStartLine, srcStartCol,
            destStartLine, destStartCol, destEndLine, destEndCol, overlay);

    public int curs_set(int level) => _cursesLibraryResolver.Resolve<CursesFunctionMap.curs_set>()(level);

    public int def_prog_mode() => _cursesLibraryResolver.Resolve<CursesFunctionMap.def_prog_mode>()();

    public int def_shell_mode() => _cursesLibraryResolver.Resolve<CursesFunctionMap.def_shell_mode>()();

    public int delay_output(int delayMillis) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.delay_output>()(delayMillis);

    public int delwin(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.delwin>()(window);

    public IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.derwin>()(window, lines, cols, beginLine, beginCol);

    public int doupdate() => _cursesLibraryResolver.Resolve<CursesFunctionMap.doupdate>()();

    public IntPtr dupwin(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.dupwin>()(window);

    public int echo() => _cursesLibraryResolver.Resolve<CursesFunctionMap.echo>()();

    public int endwin() => _cursesLibraryResolver.Resolve<CursesFunctionMap.endwin>()();

    public int erasewchar(out uint @char) => _cursesLibraryResolver.Resolve<CursesFunctionMap.erasewchar>()(out @char);

    public void filter() => _cursesLibraryResolver.Resolve<CursesFunctionMap.filter>()();

    public int flash() => _cursesLibraryResolver.Resolve<CursesFunctionMap.flash>()();

    public int flushinp() => _cursesLibraryResolver.Resolve<CursesFunctionMap.flushinp>()();

    public uint getattrs(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getattrs>()(window);

    public int getcurx(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getcurx>()(window);

    public int getcury(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getcury>()(window);

    public int getbegx(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getbegx>()(window);

    public int getbegy(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getbegy>()(window);

    public int getmaxx(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getmaxx>()(window);

    public int getmaxy(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getmaxy>()(window);

    public int getparx(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getparx>()(window);

    public int getpary(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.getpary>()(window);

    public int halfdelay(int tenthsOfSec) => _cursesLibraryResolver.Resolve<CursesFunctionMap.halfdelay>()(tenthsOfSec);

    public bool has_colors() => _cursesLibraryResolver.Resolve<CursesFunctionMap.has_colors>()();

    public bool has_ic() => _cursesLibraryResolver.Resolve<CursesFunctionMap.has_ic>()();

    public bool has_il() => _cursesLibraryResolver.Resolve<CursesFunctionMap.has_il>()();

    public void idcok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.idcok>()(window, set);

    public int idlok(IntPtr window, bool set) => _cursesLibraryResolver.Resolve<CursesFunctionMap.idlok>()(window, set);

    public void immedok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.immedok>()(window, set);

    public IntPtr initscr() => _cursesLibraryResolver.Resolve<CursesFunctionMap.initscr>()();

    public int init_color(short color, short red, short green, short blue) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.init_color>()(color, red, green, blue);

    public int init_pair(short colorPair, short fgColor, short bgColor) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.init_pair>()(colorPair, fgColor, bgColor);

    public int intrflush(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.intrflush>()(window, set);

    public bool isendwin() => _cursesLibraryResolver.Resolve<CursesFunctionMap.isendwin>()();

    public bool is_linetouched(IntPtr window, int line) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.is_linetouched>()(window, line);

    public bool is_wintouched(IntPtr window) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.is_wintouched>()(window);

    string? ICursesProvider.keyname(uint keyCode) =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.keyname>()(keyCode));

    public int keypad(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.keypad>()(window, set);

    public int leaveok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.leaveok>()(window, set);

    string? ICursesProvider.longname() =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.longname>()());

    public int meta(IntPtr window, bool set) => _cursesLibraryResolver.Resolve<CursesFunctionMap.meta>()(window, set);

    public int mvderwin(IntPtr window, int parentLine, int parentCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.mvderwin>()(window, parentLine, parentCol);

    public int mvwin(IntPtr window, int toLine, int toCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.mvwin>()(window, toLine, toCol);

    public IntPtr newpad(int lines, int cols) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.newpad>()(lines, cols);

    public IntPtr newwin(int lines, int cols, int atLine, int atCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.newwin>()(lines, cols, atLine, atCol);

    public int nl() => _cursesLibraryResolver.Resolve<CursesFunctionMap.nl>()();

    public int nocbreak() => _cursesLibraryResolver.Resolve<CursesFunctionMap.nocbreak>()();

    public int nodelay(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.nodelay>()(window, set);

    public int noecho() => _cursesLibraryResolver.Resolve<CursesFunctionMap.noecho>()();

    public int nonl() => _cursesLibraryResolver.Resolve<CursesFunctionMap.nonl>()();

    public void noqiflush() { _cursesLibraryResolver.Resolve<CursesFunctionMap.noqiflush>()(); }

    public int noraw() => _cursesLibraryResolver.Resolve<CursesFunctionMap.noraw>()();

    public int notimeout(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.notimeout>()(window, set);

    public int overlay(IntPtr srcWindow, IntPtr destWindow) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.overlay>()(srcWindow, destWindow);

    public int overwrite(IntPtr srcWindow, IntPtr destWindow) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.overwrite>()(srcWindow, destWindow);

    public int pair_content(short colorPair, out short fgColor, out short bgColor) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.pair_content>()(colorPair, out fgColor, out bgColor);

    public uint COLOR_PAIR(uint attrs) => _cursesLibraryResolver.Resolve<CursesFunctionMap.COLOR_PAIR>()(attrs);

    public uint PAIR_NUMBER(uint colorPair) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.PAIR_NUMBER>()(colorPair);

    public int pechochar(IntPtr pad, uint @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.pechochar>()(pad, @char);

    public int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.pnoutrefresh>()(pad, padMinLine, padMinCol, scrMinLine,
            scrMinCol, scrMaxLine, scrMaxCol);

    public int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.prefresh>()(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    public void qiflush() { _cursesLibraryResolver.Resolve<CursesFunctionMap.qiflush>()(); }

    public int raw() => _cursesLibraryResolver.Resolve<CursesFunctionMap.raw>()();

    public int resetty() => _cursesLibraryResolver.Resolve<CursesFunctionMap.resetty>()();

    public int reset_prog_mode() => _cursesLibraryResolver.Resolve<CursesFunctionMap.reset_prog_mode>()();

    public int reset_shell_mode() => _cursesLibraryResolver.Resolve<CursesFunctionMap.reset_shell_mode>()();

    public int ripoffline(int lines, ICursesProvider.ripoffline_callback callback) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.ripoffline>()(lines, callback);

    public int savetty() => _cursesLibraryResolver.Resolve<CursesFunctionMap.savetty>()();

    public int scrollok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.scrollok>()(window, set);

    public int slk_attroff(uint attrs) => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attroff>()(attrs);

    public int slk_attr_off(uint attrs, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attr_off>()(attrs, reserved);

    public int slk_attron(uint attrs) => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attron>()(attrs);

    public int slk_attr_on(uint attrs, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attr_on>()(attrs, reserved);

    public int slk_attrset(uint attrs) => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attrset>()(attrs);

    public int slk_attr() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attr>()();

    public int slk_attr_set(uint attrs, short colorPair, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_attr_set>()(attrs, colorPair, reserved);

    public int slk_clear() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_clear>()();

    public int slk_color(short colorPair) => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_color>()(colorPair);

    public int slk_init(int format) => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_init>()(format);

    public string? slk_label(int labelIndex) =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.slk_label>()(labelIndex));

    public int slk_noutrefresh() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_noutrefresh>()();

    public int slk_refresh() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_refresh>()();

    public int slk_restore() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_restore>()();

    public int slk_touch() => _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_touch>()();

    public int start_color() => _cursesLibraryResolver.Resolve<CursesFunctionMap.start_color>()();

    public IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.subpad>()(pad, lines, cols, atLine, atCol);

    public IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.subwin>()(window, lines, cols, atLine, atCol);

    public int syncok(IntPtr window, bool set) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.syncok>()(window, set);

    string? ICursesProvider.termname() =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.termname>()());

    public int ungetch(uint @char) => _cursesLibraryResolver.Resolve<CursesFunctionMap.ungetch>()(@char);

    public void use_env(bool set) { _cursesLibraryResolver.Resolve<CursesFunctionMap.use_env>()(set); }

    public int waddch(IntPtr window, uint charAndAttrs) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.waddch>()(window, charAndAttrs);

    public int waddchnstr(IntPtr window, uint[] charsAndAttrs, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.waddchnstr>()(window, charsAndAttrs, length);

    public int wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wattr_get>()(window, out attrs, out colorPair, reserved);

    public int wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wattr_set>()(window, attrs, colorPair, reserved);

    public int wattr_on(IntPtr window, uint attrs, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wattr_on>()(window, attrs, reserved);

    public int wattr_off(IntPtr window, uint attrs, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wattr_off>()(window, attrs, reserved);

    public int wbkgd(IntPtr window, uint charAndAttrs) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wbkgd>()(window, charAndAttrs);

    public void wbkgdset(IntPtr window, uint charAndAttrs) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wbkgdset>()(window, charAndAttrs);

    public int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wborder>()(window, leftSide, rightSide, topSide, bottomSide,
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);

    public int wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wchgat>()(window, count, attrs, colorPair, reserved);

    public int wclear(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wclear>()(window);

    public int wclrtobot(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wclrtobot>()(window);

    public int wclrtoeol(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wclrtoeol>()(window);

    public int wcolor_set(IntPtr window, short colorPair, IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wcolor_set>()(window, colorPair, reserved);

    public void wcursyncup(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wcursyncup>()(window);

    public int wdelch(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wdelch>()(window);

    public int wechochar(IntPtr window, uint charAndAttrs) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wechochar>()(window, charAndAttrs);

    public int werase(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.werase>()(window);

    public int wgetch(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetch>()(window);

    public int wgetnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetnstr>()(window, dest, length);

    public int whline(IntPtr window, uint @char, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.whline>()(window, @char, count);

    public uint winch(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.winch>()(window);

    public int winchnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.winchnstr>()(window, dest, length);

    public int winsch(IntPtr window, uint charAndAttrs) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.winsch>()(window, charAndAttrs);

    public int winsdelln(IntPtr window, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.winsdelln>()(window, count);

    public int wmove(IntPtr window, int newLine, int newCol) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wmove>()(window, newLine, newCol);

    public int wnoutrefresh(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wnoutrefresh>()(window);

    public int wredrawln(IntPtr window, int startLine, int lineCount) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wredrawln>()(window, startLine, lineCount);

    public int wrefresh(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wrefresh>()(window);

    public int wscrl(IntPtr window, int lines) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wscrl>()(window, lines);

    public int wsetscrreg(IntPtr window, int top, int bottom) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wsetscrreg>()(window, top, bottom);

    public void wsyncdown(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wsyncdown>()(window);

    public void wsyncup(IntPtr window) => _cursesLibraryResolver.Resolve<CursesFunctionMap.wsyncup>()(window);

    public void wtimeout(IntPtr window, int delay) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wtimeout>()(window, delay);

    public int wtouchln(IntPtr window, int line, int count, int changed) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wtouchln>()(window, line, count, changed);

    public int wvline(IntPtr window, uint @char, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wvline>()(window, @char, count);

    public bool is_term_resized(int lines, int cols) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.is_term_resized>()(lines, cols);

    public int resize_term(int lines, int cols) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.resize_term>()(lines, cols);

    public int resizeterm(int lines, int cols) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.resizeterm>()(lines, cols);

    public string? keybound(uint keyCode, int count) =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.keybound>()(keyCode, count));

    public string? curses_version() =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.curses_version>()());

    public int assume_default_colors(int fgColor, int bgColor) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.assume_default_colors>()(fgColor, bgColor);

    public int define_key(string keyName, int keyCode) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.define_key>()(keyName, keyCode);

    public int key_defined(string keyName) => _cursesLibraryResolver.Resolve<CursesFunctionMap.key_defined>()(keyName);

    public int keyok(int keyCode, bool set) => _cursesLibraryResolver.Resolve<CursesFunctionMap.keyok>()(keyCode, set);

    public int set_tabsize(int size) => _cursesLibraryResolver.Resolve<CursesFunctionMap.set_tabsize>()(size);

    public int use_default_colors() => _cursesLibraryResolver.Resolve<CursesFunctionMap.use_default_colors>()();

    public int wresize(IntPtr window, int lines, int columns) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wresize>()(window, lines, columns);

    public void nofilter() => _cursesLibraryResolver.Resolve<CursesFunctionMap.nofilter>()();

    public int getcchar(CursesComplexChar @char, StringBuilder dest, out uint attrs, out short colorPair,
        IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.getcchar>()(ref @char, dest, out attrs, out colorPair,
            reserved);

    public string? key_name(uint @char) =>
        Marshal.PtrToStringAnsi(_cursesLibraryResolver.Resolve<CursesFunctionMap.key_name>()(@char));

    public int killwchar(out uint @char) => _cursesLibraryResolver.Resolve<CursesFunctionMap.killwchar>()(out @char);

    public int pecho_wchar(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.pecho_wchar>()(window, ref @char);

    public int setcchar(out CursesComplexChar @char, string text, uint attrs, short colorPair,
        IntPtr reserved) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.setcchar>()(out @char, text, attrs, colorPair, reserved);

    public int slk_set(int labelIndex, string title, int align) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.slk_set>()(labelIndex, title, align);

    public uint term_attrs() => _cursesLibraryResolver.Resolve<CursesFunctionMap.term_attrs>()();

    public int unget_wch(uint @char) => _cursesLibraryResolver.Resolve<CursesFunctionMap.unget_wch>()(@char);

    public int wadd_wch(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wadd_wch>()(window, ref @char);

    public int wadd_wchnstr(IntPtr window, CursesComplexChar[] str, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wadd_wchnstr>()(window, str, count);

    public int waddnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] string text, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.waddnwstr>()(window, text, length);

    public int wbkgrnd(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wbkgrnd>()(window, ref @char);

    public void wbkgrndset(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wbkgrndset>()(window, ref @char);

    public int wborder_set(IntPtr window, CursesComplexChar leftSide, CursesComplexChar rightSide,
        CursesComplexChar topSide, CursesComplexChar bottomSide, CursesComplexChar topLeftCorner,
        CursesComplexChar topRightCorner, CursesComplexChar bottomLeftCorner, CursesComplexChar bottomRightCorner) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wborder_set>()(window, ref leftSide, ref rightSide,
            ref topSide, ref bottomSide, ref topLeftCorner, ref topRightCorner, ref bottomLeftCorner,
            ref bottomRightCorner);

    public int wecho_wchar(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wecho_wchar>()(window, ref @char);

    public int wget_wch(IntPtr window, out uint @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wget_wch>()(window, out @char);

    public int wgetbkgrnd(IntPtr window, out CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetbkgrnd>()(window, out @char);

    public int wgetn_wstr(IntPtr window, StringBuilder dest, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wgetn_wstr>()(window, dest, length);

    public int whline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.whline_set>()(window, ref @char, count);

    public int win_wch(IntPtr window, out CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.win_wch>()(window, out @char);

    public int win_wchnstr(IntPtr window, CursesComplexChar[] dest, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.win_wchnstr>()(window, dest, length);

    public int winnwstr(IntPtr window, StringBuilder dest, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.winnwstr>()(window, dest, length);

    public int wins_nwstr(IntPtr window, string text, int length) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wins_nwstr>()(window, text, length);

    public int wins_wch(IntPtr window, CursesComplexChar @char) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wins_wch>()(window, ref @char);

    public string? wunctrl(CursesComplexChar @char) =>
        Marshal.PtrToStringUni(_cursesLibraryResolver.Resolve<CursesFunctionMap.wunctrl>()(ref @char));

    public int wvline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wvline_set>()(window, ref @char, count);

    public int getmouse(out CursesMouseEvent @event) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.getmouse>()(out @event);

    public int ungetmouse(CursesMouseEvent @event) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.ungetmouse>()(ref @event);

    public virtual int mousemask(int newMask, out int oldMask) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.mousemask>()(newMask, out oldMask);

    public bool wenclose(IntPtr window, int line, int col) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wenclose>()(window, line, col);

    public int mouseinterval(int millis) => _cursesLibraryResolver.Resolve<CursesFunctionMap.mouseinterval>()(millis);

    public bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen) =>
        _cursesLibraryResolver.Resolve<CursesFunctionMap.wmouse_trafo>()(window, ref line, ref col, toScreen);

    public void set_title(string title) { Console.Title = title; }

    public virtual void set_unicode_locale() { }

    public virtual bool monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        handle = null;
        return false;
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
