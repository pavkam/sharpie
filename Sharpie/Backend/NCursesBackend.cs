#pragma warning disable CS1591

namespace Sharpie.Backend;

[PublicAPI]
internal class NCursesBackend: ICursesBackend
{
    public IDotNetSystemAdapter DotNetSystemAdapter { get; }
    private readonly INativeSymbolResolver _nCursesSymbolResolver;

    internal NCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver nCursesSymbolResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(nCursesSymbolResolver != null);

        DotNetSystemAdapter = dotNetSystemAdapter;
        _nCursesSymbolResolver = nCursesSymbolResolver;
    }

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public bool is_cleared(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_cleared>()(window);

    public bool is_idcok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_idcok>()(window);

    public bool is_idlok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_idlok>()(window);

    public bool is_immedok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_immedok>()(window);

    public bool is_keypad(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_keypad>()(window);

    public bool is_leaveok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_leaveok>()(window);

    public bool is_nodelay(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_nodelay>()(window);

    public bool is_notimeout(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_notimeout>()(window);

    public bool is_scrollok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_scrollok>()(window);

    public bool is_syncok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_syncok>()(window);

    public IntPtr wgetparent(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetparent>()(window);

    public int wgetscrreg(IntPtr window, out int top, out int bottom) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetscrreg>()(window, out top, out bottom);

    public int baudrate() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.baudrate>()();

    public int beep() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.beep>()();

    public bool can_change_color() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.can_change_color>()();

    public int cbreak() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.cbreak>()();

    public int clearok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.clearok>()(window, set);

    public int color_content(short color, out short red, out short green, out short blue) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.color_content>()(color, out red, out green, out blue);

    public int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.copywin>()(fromWindow, toWindow, srcStartLine, srcStartCol,
            destStartLine, destStartCol, destEndLine, destEndCol, overlay);

    public int curs_set(int level) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.curs_set>()(level);

    public int def_prog_mode() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.def_prog_mode>()();

    public int def_shell_mode() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.def_shell_mode>()();

    public int delay_output(int delayMillis) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.delay_output>()(delayMillis);

    public int delwin(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.delwin>()(window);

    public IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.derwin>()(window, lines, cols, beginLine, beginCol);

    public int doupdate() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.doupdate>()();

    public IntPtr dupwin(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.dupwin>()(window);

    public int echo() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.echo>()();

    public int endwin() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.endwin>()();

    public int erasewchar(out uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.erasewchar>()(out @char);

    public void filter() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.filter>()();

    public int flash() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.flash>()();

    public int flushinp() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.flushinp>()();

    public  int getattrs(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getattrs>()(window);

    public int getcurx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcurx>()(window);

    public int getcury(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcury>()(window);

    public int getbegx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getbegx>()(window);

    public int getbegy(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getbegy>()(window);

    public int getmaxx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmaxx>()(window);

    public int getmaxy(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmaxy>()(window);

    public int getparx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getparx>()(window);

    public int getpary(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getpary>()(window);

    public int halfdelay(int tenthsOfSec) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.halfdelay>()(tenthsOfSec);

    public bool has_colors() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.has_colors>()();

    public bool has_ic() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.has_ic>()();

    public bool has_il() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.has_il>()();

    public void idcok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.idcok>()(window, set);

    public int idlok(IntPtr window, bool set) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.idlok>()(window, set);

    public void immedok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.immedok>()(window, set);

    public IntPtr initscr() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.initscr>()();

    public int init_color(short color, short red, short green, short blue) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.init_color>()(color, red, green, blue);

    public int init_pair(short colorPair, short fgColor, short bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.init_pair>()(colorPair, fgColor, bgColor);

    public int intrflush(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.intrflush>()(window, set);

    public bool isendwin() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.isendwin>()();

    public bool is_linetouched(IntPtr window, int line) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_linetouched>()(window, line);

    public bool is_wintouched(IntPtr window) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_wintouched>()(window);

    public string? keyname(uint keyCode) =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.keyname>()(keyCode));

    public int keypad(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.keypad>()(window, set);

    public int leaveok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.leaveok>()(window, set);

    public string? longname() =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.longname>()());

    public int meta(IntPtr window, bool set) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.meta>()(window, set);

    public int mvderwin(IntPtr window, int parentLine, int parentCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mvderwin>()(window, parentLine, parentCol);

    public int mvwin(IntPtr window, int toLine, int toCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mvwin>()(window, toLine, toCol);

    public IntPtr newpad(int lines, int cols) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.newpad>()(lines, cols);

    public IntPtr newwin(int lines, int cols, int atLine, int atCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.newwin>()(lines, cols, atLine, atCol);

    public int nl() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nl>()();

    public int nocbreak() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nocbreak>()();

    public int nodelay(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nodelay>()(window, set);

    public int noecho() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noecho>()();

    public int nonl() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nonl>()();

    public void noqiflush() { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noqiflush>()(); }

    public int noraw() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noraw>()();

    public int notimeout(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.notimeout>()(window, set);

    public int overlay(IntPtr srcWindow, IntPtr destWindow) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.overlay>()(srcWindow, destWindow);

    public int overwrite(IntPtr srcWindow, IntPtr destWindow) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.overwrite>()(srcWindow, destWindow);

    public int pair_content(short colorPair, out short fgColor, out short bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pair_content>()(colorPair, out fgColor, out bgColor);

    public uint COLOR_PAIR(uint attrs) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.COLOR_PAIR>()(attrs);

    public uint PAIR_NUMBER(uint colorPair) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.PAIR_NUMBER>()(colorPair);

    public int pechochar(IntPtr pad, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pechochar>()(pad, charAndAttrs);

    public int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pnoutrefresh>()(pad, padMinLine, padMinCol, scrMinLine,
            scrMinCol, scrMaxLine, scrMaxCol);

    public int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.prefresh>()(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    public void qiflush() { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.qiflush>()(); }

    public int raw() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.raw>()();

    public int resetty() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.resetty>()();

    public int reset_prog_mode() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.reset_prog_mode>()();

    public int reset_shell_mode() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.reset_shell_mode>()();

    public int ripoffline(int lines, ICursesBackend.ripoffline_callback callback) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.ripoffline>()(lines, callback);

    public int savetty() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.savetty>()();

    public int scrollok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.scrollok>()(window, set);

    public int slk_attroff(uint attrs) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attroff>()(attrs);

    public int slk_attr_off(uint attrs, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_off>()(attrs, reserved);

    public int slk_attron(uint attrs) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attron>()(attrs);

    public int slk_attr_on(uint attrs, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_on>()(attrs, reserved);

    public int slk_attrset(uint attrs) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attrset>()(attrs);

    public int slk_attr() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr>()();

    public int slk_attr_set(uint attrs, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_set>()(attrs, colorPair, reserved);

    public int slk_clear() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_clear>()();

    public int slk_color(short colorPair) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_color>()(colorPair);

    public int slk_init(int format) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_init>()(format);

    public string? slk_label(int labelIndex) =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_label>()(labelIndex));

    public int slk_noutrefresh() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_noutrefresh>()();

    public int slk_refresh() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_refresh>()();

    public int slk_restore() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_restore>()();

    public int slk_touch() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_touch>()();

    public int start_color() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.start_color>()();

    public IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.subpad>()(pad, lines, cols, atLine, atCol);

    public IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.subwin>()(window, lines, cols, atLine, atCol);

    public int syncok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.syncok>()(window, set);

    public string? termname() =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.termname>()());

    public int ungetch(uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.ungetch>()(@char);

    public void use_env(bool set) { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.use_env>()(set); }

    public int waddch(IntPtr window, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.waddch>()(window, charAndAttrs);

    public int waddchnstr(IntPtr window, uint[] charsAndAttrs, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.waddchnstr>()(window, charsAndAttrs, length);

    public int wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_get>()(window, out attrs, out colorPair, reserved);

    public int wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_set>()(window, attrs, colorPair, reserved);

    public int wattr_on(IntPtr window, uint attrs, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_on>()(window, attrs, reserved);

    public int wattr_off(IntPtr window, uint attrs, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_off>()(window, attrs, reserved);

    public int wbkgd(IntPtr window, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgd>()(window, charAndAttrs);

    public void wbkgdset(IntPtr window, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgdset>()(window, charAndAttrs);

    public int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wborder>()(window, leftSide, rightSide, topSide, bottomSide,
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);

    public int wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wchgat>()(window, count, attrs, colorPair, reserved);

    public int wclear(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wclear>()(window);

    public int wclrtobot(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wclrtobot>()(window);

    public int wclrtoeol(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wclrtoeol>()(window);

    public int wcolor_set(IntPtr window, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wcolor_set>()(window, colorPair, reserved);

    public void wcursyncup(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wcursyncup>()(window);

    public int wdelch(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wdelch>()(window);

    public int wechochar(IntPtr window, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wechochar>()(window, charAndAttrs);

    public int werase(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.werase>()(window);

    public int wgetch(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetch>()(window);

    public int wgetnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetnstr>()(window, dest, length);

    public int whline(IntPtr window, uint @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.whline>()(window, @char, count);

    public int winch(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winch>()(window);

    public int winchnstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winchnstr>()(window, dest, length);

    public int winsch(IntPtr window, uint charAndAttrs) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winsch>()(window, charAndAttrs);

    public int winsdelln(IntPtr window, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winsdelln>()(window, count);

    public int wmove(IntPtr window, int newLine, int newCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wmove>()(window, newLine, newCol);

    public int wnoutrefresh(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wnoutrefresh>()(window);

    public int wredrawln(IntPtr window, int startLine, int lineCount) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wredrawln>()(window, startLine, lineCount);

    public int wrefresh(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wrefresh>()(window);

    public int wscrl(IntPtr window, int lines) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wscrl>()(window, lines);

    public int wsetscrreg(IntPtr window, int top, int bottom) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wsetscrreg>()(window, top, bottom);

    public void wsyncdown(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wsyncdown>()(window);

    public void wsyncup(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wsyncup>()(window);

    public void wtimeout(IntPtr window, int delay) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wtimeout>()(window, delay);

    public int wtouchln(IntPtr window, int line, int count, int changed) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wtouchln>()(window, line, count, changed);

    public int wvline(IntPtr window, uint @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wvline>()(window, @char, count);

    public bool is_term_resized(int lines, int cols) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_term_resized>()(lines, cols);

    public int resize_term(int lines, int cols) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.resize_term>()(lines, cols);

    public int resizeterm(int lines, int cols) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.resizeterm>()(lines, cols);

    public string? keybound(uint keyCode, int count) =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.keybound>()(keyCode, count));

    public string? curses_version() =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.curses_version>()());

    public int assume_default_colors(int fgColor, int bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.assume_default_colors>()(fgColor, bgColor);

    public int define_key(string keyName, int keyCode) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.define_key>()(keyName, keyCode);

    public int key_defined(string keyName) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.key_defined>()(keyName);

    public int keyok(int keyCode, bool set) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.keyok>()(keyCode, set);

    public int set_tabsize(int size) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.set_tabsize>()(size);

    public int use_default_colors() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.use_default_colors>()();

    public int wresize(IntPtr window, int lines, int columns) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wresize>()(window, lines, columns);

    public void nofilter() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nofilter>()();

    public int getcchar(CursesComplexChar @char, StringBuilder dest, out uint attrs, out short colorPair,
        IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcchar>()(ref @char, dest, out attrs, out colorPair,
            reserved);

    public string? key_name(uint @char) =>
        Marshal.PtrToStringAnsi(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.key_name>()(@char));

    public int killwchar(out uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.killwchar>()(out @char);

    public int pecho_wchar(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pecho_wchar>()(window, ref @char);

    public int setcchar(out CursesComplexChar @char, string text, uint attrs, short colorPair,
        IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.setcchar>()(out @char, text, attrs, colorPair, reserved);

    public int slk_set(int labelIndex, string title, int align) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_set>()(labelIndex, title, align);

    public int term_attrs() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.term_attrs>()();

    public int unget_wch(uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.unget_wch>()(@char);

    public int wadd_wch(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wadd_wch>()(window, ref @char);

    public int wadd_wchnstr(IntPtr window, CursesComplexChar[] str, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wadd_wchnstr>()(window, str, count);

    public int waddnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] string text, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.waddnwstr>()(window, text, length);

    public int wbkgrnd(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgrnd>()(window, ref @char);

    public void wbkgrndset(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgrndset>()(window, ref @char);

    public int wborder_set(IntPtr window, CursesComplexChar leftSide, CursesComplexChar rightSide,
        CursesComplexChar topSide, CursesComplexChar bottomSide, CursesComplexChar topLeftCorner,
        CursesComplexChar topRightCorner, CursesComplexChar bottomLeftCorner, CursesComplexChar bottomRightCorner) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wborder_set>()(window, ref leftSide, ref rightSide,
            ref topSide, ref bottomSide, ref topLeftCorner, ref topRightCorner, ref bottomLeftCorner,
            ref bottomRightCorner);

    public int wecho_wchar(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wecho_wchar>()(window, ref @char);

    public int wget_wch(IntPtr window, out uint @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wget_wch>()(window, out @char);

    public int wgetbkgrnd(IntPtr window, out CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetbkgrnd>()(window, out @char);

    public int wgetn_wstr(IntPtr window, StringBuilder dest, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetn_wstr>()(window, dest, length);

    public int whline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.whline_set>()(window, ref @char, count);

    public int win_wch(IntPtr window, out CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.win_wch>()(window, out @char);

    public int win_wchnstr(IntPtr window, CursesComplexChar[] dest, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.win_wchnstr>()(window, dest, length);

    public int winnwstr(IntPtr window, StringBuilder dest, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winnwstr>()(window, dest, length);

    public int wins_nwstr(IntPtr window, string text, int length) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wins_nwstr>()(window, text, length);

    public int wins_wch(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wins_wch>()(window, ref @char);

    public string? wunctrl(CursesComplexChar @char) =>
        Marshal.PtrToStringUni(_nCursesSymbolResolver.Resolve<NCursesFunctionMap.wunctrl>()(ref @char));

    public int wvline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wvline_set>()(window, ref @char, count);

    public int getmouse(out CursesMouseEvent @event) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmouse>()(out @event);

    public int ungetmouse(CursesMouseEvent @event) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.ungetmouse>()(ref @event);

    public virtual int mousemask(int newMask, out int oldMask) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mousemask>()(newMask, out oldMask);

    public bool wenclose(IntPtr window, int line, int col) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wenclose>()(window, line, col);

    public int mouseinterval(int millis) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mouseinterval>()(millis);

    public bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wmouse_trafo>()(window, ref line, ref col, toScreen);

    public void set_title(string title) { DotNetSystemAdapter.SetConsoleTitle(title); }

    public virtual void set_unicode_locale() { }

    public virtual bool monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        handle = null;
        return false;
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
