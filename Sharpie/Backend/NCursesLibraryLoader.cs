#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Reflection;

[SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "InconsistentNaming")]
public class NCursesLibraryLoader
{
    private readonly NativeLibrary _library;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int baudrate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int beep();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool can_change_color();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int cbreak();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int clearok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int color_content(short color, out short red, out short green, out short blue);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int copywin(IntPtr fromWindow, IntPtr toWindow, int srcMinLine, int srcMinCol,
        int destMinLine, int destMinCol, int destMaxLine, int destMaxCol,
        int overlay);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int curs_set(int level);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int def_prog_mode();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int def_shell_mode();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int delay_output(int delayMillis);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int delwin(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int doupdate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr dupwin(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int echo();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int endwin();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate char erasechar();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void filter();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int flash();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int flushinp();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int halfdelay(int tenthsOfSec);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool has_colors();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool has_ic();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool has_il();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void idcok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int idlok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void immedok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr initscr();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int init_color(short color, short red, short green, short blue);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int init_pair(short colorPair, short fgColor, short bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int intrflush(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool isendwin();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_linetouched(IntPtr window, int line);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_wintouched(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr keyname(uint keyCode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int keypad(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate char killchar();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int leaveok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr longname();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int meta(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int mvderwin(IntPtr window, int parentLine, int parentCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int mvwin(IntPtr window, int toLine, int toCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr newpad(int lines, int cols);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr newwin(int lines, int cols, int atLine, int atCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int nl();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int nocbreak();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int nodelay(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int noecho();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int nonl();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void noqiflush();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int noraw();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int notimeout(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int overlay(IntPtr srcWindow, IntPtr destWindow);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int overwrite(IntPtr srcWindow, IntPtr destWindow);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int pair_content(short colorPair, out short fgColor, out short bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint COLOR_PAIR(uint attrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint PAIR_NUMBER(uint colorPair);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int pechochar(IntPtr pad, uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void qiflush();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int raw();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int resetty();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int reset_prog_mode();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int reset_shell_mode();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ripoffline(int lines, ICursesProvider.ripoffline_callback callback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int savetty();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int scrollok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attroff(uint attrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attron(uint attrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attrset(uint attrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate char slk_attr();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attr_set(uint attrs, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_clear();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_color(short colorPair);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_init(int format);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr slk_label(int labelIndex);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_noutrefresh();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_refresh();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_restore();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_touch();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int start_color();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr subpad(IntPtr pad, int lines, int cols, int atRow,
        int atCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int syncok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint termattrs();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr termname();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ungetch(uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void use_env(bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int waddch(IntPtr window, uint charAndAttrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int waddchnstr(IntPtr window, uint[] charsAndAttrs, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wattr_on(IntPtr window, uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wattr_off(IntPtr window, uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wbkgd(IntPtr window, uint charAndAttrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wbkgdset(IntPtr window, uint charAndAttrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wclear(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wclrtobot(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wclrtoeol(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wcolor_set(IntPtr window, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wcursyncup(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wdelch(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wechochar(IntPtr window, uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int werase(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wgetch(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int wgetnstr(IntPtr window, StringBuilder dest, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int whline(IntPtr window, uint @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint winch(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int winchnstr(IntPtr window, StringBuilder dest, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int winsch(IntPtr window, uint charAndAttrs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int winsdelln(IntPtr window, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wmove(IntPtr window, int newLine, int newCol);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wnoutrefresh(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wredrawln(IntPtr window, int startLine, int lineCount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wrefresh(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wscrl(IntPtr window, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wsetscrreg(IntPtr window, int top, int bottom);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wsyncdown(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wsyncup(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wtimeout(IntPtr window, int delay);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wtouchln(IntPtr window, int line, int count, int changed);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wvline(IntPtr window, uint @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_term_resized(int lines, int cols);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int resize_term(int lines, int cols);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int resizeterm(int lines, int cols);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr keybound(uint keyCode, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr curses_version();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int assume_default_colors(int fgColor, int bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int define_key(string keyName, int keyCode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int key_defined(string keyName);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int keyok(int keyCode, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int set_tabsize(int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int use_default_colors();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wresize(IntPtr window, int lines, int columns);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void nofilter();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getcchar(ref CursesComplexChar @char, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest,
        out uint attrs, out short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr key_name(uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int killwchar(out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int pecho_wchar(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int setcchar(out CursesComplexChar @char, [MarshalAs(UnmanagedType.LPWStr)] string text, uint attrs,
        short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int slk_set(int labelIndex, string title, int fmt);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint term_attrs();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int unget_wch(uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wadd_wch(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wadd_wchnstr(IntPtr window, CursesComplexChar[] @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int waddnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] string text, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wbkgrnd(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wbkgrndset(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wborder_set(IntPtr window, ref CursesComplexChar leftSide, ref CursesComplexChar rightSide,
        ref CursesComplexChar topSide, ref CursesComplexChar bottomSide, ref CursesComplexChar topLeftCorner,
        ref CursesComplexChar topRightCorner, ref CursesComplexChar bottomLeftCorner,
        ref CursesComplexChar bottomRightCorner);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wecho_wchar(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wget_wch(IntPtr window, out uint dest);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wgetbkgrnd(IntPtr window, out CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wgetn_wstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int whline_set(IntPtr window, ref CursesComplexChar @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int win_wch(IntPtr window, out CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int win_wchnstr(IntPtr window, CursesComplexChar[] @char, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int winnwstr(IntPtr window, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder text, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wins_nwstr(IntPtr window, string text, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wins_wch(IntPtr window, ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int winwstr(IntPtr window, string text);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr wunctrl(ref CursesComplexChar @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wvline_set(IntPtr window, ref CursesComplexChar @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int erasewchar(out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint getattrs(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getcurx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getcury(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getbegx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getbegy(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getmaxx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getmaxy(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getparx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getpary(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attr_off(uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int slk_attr_on(uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_cleared(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_idcok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_idlok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_immedok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_keypad(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_leaveok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_nodelay(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_notimeout(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_scrollok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool is_syncok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr wgetparent(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int wgetscrreg(IntPtr window, out int top, out int bottom);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int getmouse(out CursesMouseEvent @event);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ungetmouse(ref CursesMouseEvent @event);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int mousemask(int newMask, out int oldMask);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool wenclose(IntPtr window, int line, int col);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int mouseinterval(int millis);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int setlocale(int cate, [MarshalAs(UnmanagedType.LPStr)] string locale);
    
    public NCursesLibraryLoader(NativeLibrary library) => _library = library ?? throw new ArgumentNullException(nameof(library));

    protected abstract Delegate GetExport(string name, Type type);
    
    public IReadOnlyDictionary<Type, Delegate> MethodTable
    {
        get
        {
            var table = new Dictionary<Type, Delegate>();

            if (NativeLibrary.TryLoad("ncurses", Assembly.GetCallingAssembly(), null, out var libHandle))
            {
                var t = GetType()
                    .GetTypeInfo();

                var ds = t.DeclaredMembers.Where(m => m.MemberType == MemberTypes.NestedType)
                          .Select(s => (TypeInfo) s)
                          .Where(t => !t.IsGenericType && t.BaseType == typeof(MulticastDelegate))
                          .ToArray();

                foreach (var import in ds)
                {
                    var fh = NativeLibrary.GetExport(libHandle, import.Name);
                    var k = Marshal.GetDelegateForFunctionPointer(fh, import);
                    table.Add(import, k);
                }
            }
        }
    }
}
