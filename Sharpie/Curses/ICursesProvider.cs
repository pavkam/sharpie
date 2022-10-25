namespace Sharpie.Curses;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;

using CursesWindow = IntPtr;

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

    int clearok(CursesWindow window, bool set); //DONE

    int color_content(ushort color, out ushort red, out ushort green, out ushort blue); //DONE

    int copywin(CursesWindow fromWindow, CursesWindow toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay); //DONE

    int curs_set(int level); //DONE

    int def_prog_mode(); //NO

    int def_shell_mode(); //NO

    int delay_output(int delayMillis);

    int delwin(CursesWindow window); //DONE

    CursesWindow derwin(CursesWindow window, int lines, int cols, int beginLine,
        int beginCol);

    int doupdate(); //DONE

    CursesWindow dupwin(CursesWindow window); //DONE

    int echo(); //DONE

    int endwin(); //DONE

    int erasewchar(out char @char); //DONE

    void filter(); //NO

    int flash(); //DONE

    int flushinp();

    uint getattrs(CursesWindow window); //NO

    int getcurx(CursesWindow window); //DONE

    int getcury(CursesWindow window); //DONE

    int getbegx(CursesWindow window); //DONE

    int getbegy(CursesWindow window); //DONE

    int getmaxx(CursesWindow window); //DONE

    int getmaxy(CursesWindow window); //DONE

    int getparx(CursesWindow window); //NO

    int getpary(CursesWindow window); //NO

    int halfdelay(int tenthsOfSec); //DONE

    bool has_colors(); //DONE

    bool has_ic(); //DONE

    bool has_il(); //DONE

    void idcok(CursesWindow window, bool set); //DONE

    int idlok(CursesWindow window, bool set); //DONE

    void immedok(CursesWindow window, bool set); //DONE

    CursesWindow initscr(); //DONE

    int init_color(ushort color, ushort red, ushort green, ushort blue); //DONE

    int init_pair(ushort colorPair, ushort fgColor, ushort bgColor); //DONE

    int intrflush(CursesWindow window, bool set); //DONE

    bool isendwin(); //DONE

    bool is_linetouched(CursesWindow window, int line); //DONE

    bool is_wintouched(CursesWindow window); //DONE

    bool is_cleared(IntPtr window);

    bool is_idcok(IntPtr window); //DONE

    bool is_idlok(IntPtr window); //DONE

    bool is_immedok(IntPtr window); //DONE

    bool is_keypad(IntPtr window);

    bool is_leaveok(IntPtr window);

    bool is_nodelay(IntPtr window);

    bool is_notimeout(IntPtr window);

    bool is_pad(IntPtr window); //NO

    bool is_scrollok(IntPtr window);

    bool is_subwin(IntPtr window); //NO

    bool is_syncok(IntPtr window);

    IntPtr wgetparent(IntPtr window); //NO

    int wgetdelay(IntPtr window);

    int wgetscrreg(IntPtr window, out int top, out int bottom);

    string keyname(int keyCode);

    int keypad(CursesWindow window, bool set); //DONE

    int leaveok(CursesWindow window, bool set);

    string longname(); //DONE

    int meta(CursesWindow window, bool set); //DONE

    int mvderwin(CursesWindow window, int parentLine, int parentCol);

    int mvwin(CursesWindow window, int toLine, int toCol); //DONE

    CursesWindow newpad(int lines, int cols); //DONE

    CursesWindow newwin(int lines, int cols, int atLine, int atCol); //DONE

    int nl(); //DONE

    int nocbreak(); //DONE

    int nodelay(CursesWindow window, bool set);

    int noecho(); //DONE

    int nonl(); //DONE

    void noqiflush();

    int noraw();

    int notimeout(CursesWindow window, bool set);

    int overlay(CursesWindow srcWindow, CursesWindow destWindow); //DONE

    int overwrite(CursesWindow srcWindow, CursesWindow destWindow); //DONE

    int pair_content(ushort colorPair, out ushort fgColor, out ushort bgColor);

    uint COLOR_PAIR(uint attrs); //DONE

    uint PAIR_NUMBER(uint colorPair); //DONE

    int pechochar(CursesWindow pad, char @char); //NO

    int pnoutrefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    int prefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    void qiflush();

    int raw();

    int resetty(); //NO

    int reset_prog_mode(); //NO

    int reset_shell_mode(); //NO

    public delegate bool ripoffline_callback(CursesWindow window, int columns);

    int ripoffline(int lines, ripoffline_callback callback);

    int savetty(); //NO

    int scrollok(CursesWindow window, bool set);

    int slk_attroff(char attrs); //NO

    int slk_attr_off(uint attrs, IntPtr reserved); //DONE

    int slk_attron(char attrs); //NO

    int slk_attr_on(uint attrs, IntPtr reserved); //DONE

    int slk_attrset(char attrs); //NO

    int slk_attr(); //DONE

    int slk_attr_set(uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int slk_clear(); //DONE

    int slk_color(ushort colorPair); //DONE

    int slk_init(int format); //DONE

    string slk_label(int labelIndex); //NO

    int slk_noutrefresh(); //DONE

    int slk_refresh(); //DONE

    int slk_restore(); //DONE

    int slk_touch(); //DONE

    int start_color(); //DONE

    CursesWindow subpad(CursesWindow pad, int lines, int cols, int atLine,
        int atCol);

    CursesWindow subwin(CursesWindow window, int lines, int cols, int atLine,
        int atCol);

    int syncok(CursesWindow window, bool set);

    string termname(); //DONE

    int ungetch(int @char);

    void use_env(bool set); //DONE

    int waddch(CursesWindow window, uint charAndAttrs); //NO

    int waddchnstr(CursesWindow window, string text, int length); //NO

    int wattr_get(CursesWindow window, out uint attrs, out ushort colorPair, IntPtr reserved); //DONE

    int wattr_set(CursesWindow window, uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int wattr_on(CursesWindow window, uint attrs, IntPtr reserved); //DONE

    int wattr_off(CursesWindow window, uint attrs, IntPtr reserved); //DONE

    int wbkgd(CursesWindow window, uint charAndAttrs);

    void wbkgdset(CursesWindow window, uint charAndAttrs);

    int wborder(CursesWindow window, CChar leftSide, CChar rightSide, CChar topSide,
        CChar bottomSide, CChar topLeftCorner, CChar topRightCorner, CChar bottomLeftCorner,
        CChar bottomRightCorner);

    int wchgat(CursesWindow window, int count, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int wclear(CursesWindow window); //NO

    int wclrtobot(CursesWindow window); //DONE

    int wclrtoeol(CursesWindow window); //DONE

    int wcolor_set(CursesWindow window, ushort pair, IntPtr reserved); //DONE

    void wcursyncup(CursesWindow window);

    int wdelch(CursesWindow window); //DONE

    int wechochar(CursesWindow window, uint charAndAttrs); //NO

    int werase(CursesWindow window); //DONE

    int wgetch(CursesWindow window);

    int wgetnstr(CursesWindow window, StringBuilder dest, int length);

    int whline(CursesWindow window, char @char, int count);

    char winch(CursesWindow window);

    int winchnstr(CursesWindow window, StringBuilder dest, int length);

    int winsch(CursesWindow window, char @char); //NO

    int winsdelln(CursesWindow window, int count);

    int wmove(CursesWindow window, int newLine, int newCol); //DONE

    int wnoutrefresh(CursesWindow window); //DONE

    int wredrawln(CursesWindow window, int startLine, int lineCount);

    int wrefresh(CursesWindow window); //DONE

    int wscrl(CursesWindow window, int count);

    int wsetscrreg(CursesWindow window, int top, int bottom);

    void wsyncdown(CursesWindow window); //NO

    void wsyncup(CursesWindow window);

    void wtimeout(CursesWindow window, int delay);

    int wtouchln(CursesWindow window, int line, int count, int changed); //DONE

    int wvline(CursesWindow window, char @char, int count);

    bool is_term_resized(int lines, int cols);

    int resize_term(int lines, int cols); //NO

    int resizeterm(int lines, int cols); //NO

    string keybound(int keyCode, int count);

    string curses_version(); //DONE

    int assume_default_colors(int fgColor, int bgColor); //DONE

    int define_key(string keyName, int keyCode);

    int get_escdelay(); //DONE

    int key_defined(string keyName);

    int keyok(int keyCode, bool set);

    int set_escdelay(int millis); //DONE

    int set_tabsize(int size);

    int use_default_colors(); //DONE

    int wresize(CursesWindow window, int lines, int columns); //DONE

    void nofilter(); //NO

    int getcchar(CChar @char, StringBuilder dest, out uint attrs, out ushort colorPair,
        IntPtr reserved);

    string key_name(char @char);

    int killwchar(out char @char); //DONE

    int pecho_wchar(CursesWindow window, CChar @char); //NO

    int setcchar(out CChar @char, string text, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int slk_wset(int labelIndex, string title, int align); //DONE

    uint term_attrs();

    int unget_wch(char @char);

    int wadd_wch(CursesWindow window, CChar @char); //DONE

    int wadd_wchnstr(CursesWindow window, CChar[] str, int count); //DONE

    int waddnwstr(CursesWindow window, string text, int length); //NO

    int wbkgrnd(CursesWindow window, CChar @char);

    void wbkgrndset(CursesWindow window, CChar @char);

    int wborder_set(CursesWindow window, CChar leftSide, CChar rightSide, CChar topSide,
        CChar bottomSide, CChar topLeftCorner, CChar topRightCorner, CChar bottomLeftCorner,
        CChar bottomRightCorner);

    int wecho_wchar(CursesWindow window, CChar @char); //NO

    int wget_wch(CursesWindow window, out uint @char);

    int wgetbkgrnd(CursesWindow window, CChar @char);

    int wgetn_wstr(CursesWindow window, StringBuilder dest, int length);

    int whline_set(CursesWindow window, CChar @char, int count);

    int win_wch(CursesWindow window, out CChar @char);

    int win_wchnstr(CursesWindow window, CChar[] @char, int length);

    int winnwstr(CursesWindow window, StringBuilder dest, int length);

    int wins_nwstr(CursesWindow window, string text, int length); //NO

    int wins_wch(CursesWindow window, CChar @char); //DONE

    string wunctrl(CChar @char); //FISHY

    int wvline_set(CursesWindow window, CChar @char, int count);
}
