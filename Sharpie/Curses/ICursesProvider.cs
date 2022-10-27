namespace Sharpie.Curses;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;

/// <summary>
/// Interface provides access to the Curses functionality. Use the <see cref="NativeCursesProvider"/> property to access the actual
/// implementation.
/// </summary>
[PublicAPI, SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "IdentifierTypo")]
public interface ICursesProvider
{
    int baudrate(); //DONE

    int beep(); //DONE

    bool can_change_color(); //DONE

    int cbreak(); //DONE

    int clearok(IntPtr window, bool set); //DONE

    int color_content(ushort color, out ushort red, out ushort green, out ushort blue); //DONE

    int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay); //DONE

    int curs_set(int level); //DONE

    int def_prog_mode(); //NO

    int def_shell_mode(); //NO

    int delay_output(int delayMillis); //NO

    int delwin(IntPtr window); //DONE

    IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol);

    int doupdate(); //DONE

    IntPtr dupwin(IntPtr window); //DONE

    int echo(); //DONE

    int endwin(); //DONE

    int erasewchar(out uint @char); //DONE

    void filter(); //NO

    int flash(); //DONE

    int flushinp();

    uint getattrs(IntPtr window); //NO

    int getcurx(IntPtr window); //DONE

    int getcury(IntPtr window); //DONE

    int getbegx(IntPtr window); //DONE

    int getbegy(IntPtr window); //DONE

    int getmaxx(IntPtr window); //DONE

    int getmaxy(IntPtr window); //DONE

    int getparx(IntPtr window); //NO

    int getpary(IntPtr window); //NO

    int halfdelay(int tenthsOfSec); //DONE

    bool has_colors(); //DONE

    bool has_ic(); //DONE

    bool has_il(); //DONE

    void idcok(IntPtr window, bool set); //DONE

    int idlok(IntPtr window, bool set); //DONE

    void immedok(IntPtr window, bool set); //DONE

    IntPtr initscr(); //DONE

    int init_color(ushort color, ushort red, ushort green, ushort blue); //DONE

    int init_pair(ushort colorPair, ushort fgColor, ushort bgColor); //DONE

    int intrflush(IntPtr window, bool set); //DONE

    bool isendwin(); //DONE

    bool is_linetouched(IntPtr window, int line); //DONE

    bool is_wintouched(IntPtr window); //DONE

    bool is_cleared(IntPtr window);

    bool is_idcok(IntPtr window); //DONE

    bool is_idlok(IntPtr window); //DONE

    bool is_immedok(IntPtr window); //DONE

    bool is_keypad(IntPtr window);

    bool is_leaveok(IntPtr window);

    bool is_nodelay(IntPtr window);

    bool is_notimeout(IntPtr window);

    bool is_pad(IntPtr window); //NO

    bool is_scrollok(IntPtr window); //DONE

    bool is_subwin(IntPtr window); //NO

    bool is_syncok(IntPtr window);

    IntPtr wgetparent(IntPtr window); //NO

    int wgetdelay(IntPtr window);

    int wgetscrreg(IntPtr window, out int top, out int bottom);

    string? keyname(uint keyCode);

    int keypad(IntPtr window, bool set); //DONE

    int leaveok(IntPtr window, bool set);

    string? longname(); //DONE

    int meta(IntPtr window, bool set); //DONE

    int mvderwin(IntPtr window, int parentLine, int parentCol);

    int mvwin(IntPtr window, int toLine, int toCol); //DONE

    IntPtr newpad(int lines, int cols); //DONE

    IntPtr newwin(int lines, int cols, int atLine, int atCol); //DONE

    int nl(); //DONE

    int nocbreak(); //DONE

    int nodelay(IntPtr window, bool set); //DONE

    int noecho(); //DONE

    int nonl(); //DONE

    void noqiflush();

    int noraw();

    int notimeout(IntPtr window, bool set); //DONE

    int overlay(IntPtr srcWindow, IntPtr destWindow); //DONE

    int overwrite(IntPtr srcWindow, IntPtr destWindow); //DONE

    int pair_content(ushort colorPair, out ushort fgColor, out ushort bgColor);

    uint COLOR_PAIR(uint attrs); //DONE

    uint PAIR_NUMBER(uint colorPair); //DONE

    int pechochar(IntPtr pad, uint charAndAttrs); //NO

    int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    void qiflush();

    int raw();

    int resetty(); //NO

    int reset_prog_mode(); //NO

    int reset_shell_mode(); //NO

    public delegate bool ripoffline_callback(IntPtr window, int columns);

    int ripoffline(int lines, ripoffline_callback callback);

    int savetty(); //NO

    int scrollok(IntPtr window, bool set); //DONE

    int slk_attroff(uint attrs); //NO

    int slk_attr_off(uint attrs, IntPtr reserved); //DONE

    int slk_attron(uint attrs); //NO

    int slk_attr_on(uint attrs, IntPtr reserved); //DONE

    int slk_attrset(uint attrs); //NO

    int slk_attr(); //DONE

    int slk_attr_set(uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int slk_clear(); //DONE

    int slk_color(ushort colorPair); //DONE

    int slk_init(int format); //DONE

    string? slk_label(int labelIndex); //NO

    int slk_noutrefresh(); //DONE

    int slk_refresh(); //DONE

    int slk_restore(); //DONE

    int slk_touch(); //DONE

    int start_color(); //DONE

    IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol);

    IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol);

    int syncok(IntPtr window, bool set);

    string? termname(); //DONE

    int ungetch(uint @char); //NO

    void use_env(bool set); //DONE

    int waddch(IntPtr window, uint charAndAttrs); //NO

    int waddchnstr(IntPtr window, string text, int length); //NO

    int wattr_get(IntPtr window, out uint attrs, out ushort colorPair, IntPtr reserved); //DONE

    int wattr_set(IntPtr window, uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int wattr_on(IntPtr window, uint attrs, IntPtr reserved); //DONE

    int wattr_off(IntPtr window, uint attrs, IntPtr reserved); //DONE

    int wbkgd(IntPtr window, uint charAndAttrs);

    void wbkgdset(IntPtr window, uint charAndAttrs);

    int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner);

    int wchgat(IntPtr window, int count, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int wclear(IntPtr window); //NO

    int wclrtobot(IntPtr window); //DONE

    int wclrtoeol(IntPtr window); //DONE

    int wcolor_set(IntPtr window, ushort pair, IntPtr reserved); //DONE

    void wcursyncup(IntPtr window);

    int wdelch(IntPtr window); //DONE

    int wechochar(IntPtr window, uint charAndAttrs); //NO

    int werase(IntPtr window); //DONE

    int wgetch(IntPtr window); //NO

    int wgetnstr(IntPtr window, StringBuilder dest, int length); //NO

    int whline(IntPtr window, uint @char, int count); //DONE

    uint winch(IntPtr window); //NO

    int winchnstr(IntPtr window, StringBuilder dest, int length); //NO

    int winsch(IntPtr window, uint @char); //NO

    int winsdelln(IntPtr window, int count);

    int wmove(IntPtr window, int newLine, int newCol); //DONE

    int wnoutrefresh(IntPtr window); //DONE

    int wredrawln(IntPtr window, int startLine, int lineCount);

    int wrefresh(IntPtr window); //DONE

    int wscrl(IntPtr window, int count);

    int wsetscrreg(IntPtr window, int top, int bottom);

    void wsyncdown(IntPtr window); //NO

    void wsyncup(IntPtr window);

    void wtimeout(IntPtr window, int delay);

    int wtouchln(IntPtr window, int line, int count, int changed); //DONE

    int wvline(IntPtr window, uint @char, int count); //DONE

    bool is_term_resized(int lines, int cols); //NO

    int resize_term(int lines, int cols); //NO

    int resizeterm(int lines, int cols); //NO

    string? keybound(uint keyCode, int count);

    string? curses_version(); //DONE

    int assume_default_colors(int fgColor, int bgColor); //DONE

    int define_key(string keyName, int keyCode);

    int key_defined(string keyName);

    int keyok(int keyCode, bool set);

    int set_tabsize(int size);

    int use_default_colors(); //DONE

    int wresize(IntPtr window, int lines, int columns); //DONE

    void nofilter(); //NO

    int getcchar(CChar @char, StringBuilder dest, out uint attrs, out ushort colorPair,
        IntPtr reserved); //DONE

    string? key_name(uint @char);

    int killwchar(out uint @char); //DONE

    int pecho_wchar(IntPtr window, CChar @char); //NO

    int setcchar(out CChar @char, string text, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int slk_set(int labelIndex, string title, int align); //DONE

    uint term_attrs(); //DONE

    int unget_wch(uint @char); //NO

    int wadd_wch(IntPtr window, CChar @char); //DONE

    int wadd_wchnstr(IntPtr window, CChar[] str, int count); //DONE

    int waddnwstr(IntPtr window, string text, int length); //NO

    int wbkgrnd(IntPtr window, CChar @char); //DONE

    void wbkgrndset(IntPtr window, CChar @char); //NO

    int wborder_set(IntPtr window, CChar leftSide, CChar rightSide, CChar topSide,
        CChar bottomSide, CChar topLeftCorner, CChar topRightCorner, CChar bottomLeftCorner,
        CChar bottomRightCorner); //DONE

    int wecho_wchar(IntPtr window, CChar @char); //NO

    int wget_wch(IntPtr window, out uint @char); //DONE

    int wgetbkgrnd(IntPtr window, out CChar @char); //DONE

    int wgetn_wstr(IntPtr window, StringBuilder dest, int length); //NO

    int whline_set(IntPtr window, CChar @char, int count); //DONE

    int win_wch(IntPtr window, out CChar @char);

    int win_wchnstr(IntPtr window, CChar[] @char, int length);

    int winnwstr(IntPtr window, StringBuilder dest, int length); //NO

    int wins_nwstr(IntPtr window, string text, int length); //NO

    int wins_wch(IntPtr window, CChar @char); //NO

    string? wunctrl(CChar @char);

    int wvline_set(IntPtr window, CChar @char, int count); //DONE

    int getmouse(out RawMouseEvent @event); //DONE

    int ungetmouse(RawMouseEvent @event); //NO

    int mousemask(ulong newMask, out ulong oldMask); //DONE

    bool wenclose(IntPtr window, int line, int col);

    int mouseinterval(int millis); //DONE

    bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen);
}
