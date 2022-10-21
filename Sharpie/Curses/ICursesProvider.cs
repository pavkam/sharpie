namespace Sharpie.Curses;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using CursesWindow = IntPtr;
using CursesRune = IntPtr;

/// <summary>
/// Interface provides access to the Curses functionality. Use the <see cref="NativeCursesProvider"/> property to access the actual
/// implementation.
/// </summary>
[PublicAPI, SuppressMessage("ReSharper", "InconsistentNaming")]

// ReSharper disable IdentifierTypo
public interface ICursesProvider
{
    int baudrate(); //DONE

    int beep(); //DONE

    bool can_change_color(); //DONE

    int cbreak(); //DONE

    int clearok(CursesWindow window, bool set);

    int color_content(ushort color, out ushort red, out ushort green, out ushort blue); //DONE

    int copywin(CursesWindow fromWindow, CursesWindow toWindow, int srcMinLine, int srcMinCol,
        int destMinLine, int destMinCol, int destMaxLine, int destMaxCol,
        int overlay);

    int curs_set(int level); //done

    int def_prog_mode(); //NO

    int def_shell_mode(); //NO

    int delay_output(int delayMillis);

    int delwin(CursesWindow window);

    CursesWindow derwin(CursesWindow window, int lines, int cols, int beginLine,
        int beginCol);

    int doupdate();

    CursesWindow dupwin(CursesWindow window);

    int echo(); //DONE

    int endwin(); //DONE

    char erasechar();

    void filter(); //NO

    int flash(); //DONE

    int flushinp();

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

    bool is_linetouched(CursesWindow window, int line);

    bool is_wintouched(CursesWindow window);

    string keyname(int keyCode);

    int keypad(CursesWindow window, bool set); //DONE

    char killchar();

    int leaveok(CursesWindow window, bool set);

    string longname();

    int meta(CursesWindow window, bool set); //DONE

    int mvderwin(CursesWindow window, int parentLine, int parentCol);

    int mvwin(CursesWindow window, int toLine, int toCol);

    CursesWindow newpad(int lines, int cols);

    CursesWindow newwin(int lines, int cols, int atLine, int atCol); //DONE

    int nl(); //DONE

    int nocbreak(); //DONE

    int nodelay(CursesWindow window, bool set);

    int noecho(); //DONE

    int nonl(); //DONE

    void noqiflush();

    int noraw();

    int notimeout(CursesWindow window, bool set);

    int overlay(CursesWindow srcWindow, CursesWindow destWindow);

    int overwrite(CursesWindow srcWindow, CursesWindow destWindow);

    int pair_content(ushort colorPair, out ushort fgColor, out ushort bgColor);

    int COLOR_PAIR(int colorPair);

    int PAIR_NUMBER(int attrOrChar);

    int pechochar(CursesWindow pad, char @char);

    int pnoutrefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    int prefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    void qiflush();

    int raw();

    int resetty(); //NO

    int reset_prog_mode(); //NO

    int reset_shell_mode(); //NO

    public delegate bool ripoffline_callback(CursesWindow window, int columns);

    int ripoffline(int lines, ripoffline_callback callback);

    int savetty(); //NO

    int scrollok(CursesWindow window, bool set);

    int slk_attroff(uint attrs);

    int slk_attron(uint attrs);

    int slk_attrset(uint attrs);

    char slk_attr();

    int slk_attr_set(uint attrs, ushort colorPair, IntPtr reserved);

    int slk_clear();

    int slk_color(ushort colorPair);

    int slk_init(int format);

    string slk_label(int labelIndex);

    int slk_noutrefresh();

    int slk_refresh();

    int slk_restore();

    int slk_touch();

    int start_color(); //DONE

    CursesWindow subpad(CursesWindow pad, int lines, int cols, int atRow,
        int atCol);

    CursesWindow subwin(CursesWindow window, int lines, int cols, int atLine,
        int atCol);

    int syncok(CursesWindow window, bool set);

    string termname(); //DONE

    int ungetch(int @char);

    void use_env(bool set); //DONE

    int waddch(CursesWindow window, char @char);

    int waddchnstr(CursesWindow window, string text, int length);

    int wattr_get(CursesWindow window, out uint attrs, out ushort colorPair, IntPtr reserved); //DONE

    int wattr_set(CursesWindow window, uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int wattr_on(CursesWindow window, uint attrs, IntPtr reserved); //DONE

    int wattr_off(CursesWindow window, uint attrs, IntPtr reserved); //DONE

    int wbkgd(CursesWindow window, uint charAndAttrs);

    void wbkgdset(CursesWindow window, uint charAndAttrs);

    int wborder(CursesWindow window, CursesRune leftSide, CursesRune rightSide, CursesRune topSide,
        CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner, CursesRune bottomLeftCorner,
        CursesRune bottomRightCorner);

    int wchgat(CursesWindow window, int count, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int wclear(CursesWindow window);

    int wclrtobot(CursesWindow window);

    int wclrtoeol(CursesWindow window);

    int wcolor_set(CursesWindow window, ushort pair, IntPtr reserved); //DONE

    void wcursyncup(CursesWindow window);

    int wdelch(CursesWindow window);

    int wechochar(CursesWindow window, char @char);

    int werase(CursesWindow window);

    int wgetch(CursesWindow window);

    int wgetnstr(CursesWindow window, StringBuilder dest, int length);

    int whline(CursesWindow window, char @char, int count);

    char winch(CursesWindow window);

    int winchnstr(CursesWindow window, StringBuilder dest, int length);

    int winsch(CursesWindow window, char @char);

    int winsdelln(CursesWindow window, int count);

    int wmove(CursesWindow window, int newLine, int newCol); //DONE

    int wnoutrefresh(CursesWindow window);

    int wredrawln(CursesWindow window, int startLine, int lineCount);

    int wrefresh(CursesWindow window);

    int wscrl(CursesWindow window, int count);

    int wsetscrreg(CursesWindow window, int top, int bottom);

    void wsyncdown(CursesWindow window); //NO

    void wsyncup(CursesWindow window);

    void wtimeout(CursesWindow window, int delay);

    int wtouchln(CursesWindow window, int line, int count, int changed);

    int wvline(CursesWindow window, char @char, int count);

    bool is_term_resized(int lines, int cols);

    int resize_term(int lines, int cols); //NO

    int resizeterm(int lines, int cols); //NO

    string keybound(int keyCode, int count);

    string curses_version(); //DONE

    int assume_default_colors(int fgColor, int bgColor); //DONE

    int define_key(string keyName, int keyCode);

    int get_escdelay();

    int key_defined(string keyName);

    int keyok(int keyCode, bool set);

    int set_escdelay(int tenths);

    int set_tabsize(int size);

    int use_default_colors(); //DONE

    int wresize(CursesWindow window, int lines, int columns);

    void nofilter(); //NO

    int getcchar(CursesRune rune, StringBuilder dest, ref uint attrs, ref ushort colorPair,
        IntPtr reserved);

    string key_name(char @char);

    int killwchar(string text);

    int pecho_wchar(CursesWindow window, CursesRune rune);

    int setcchar(CursesRune rune, string text, uint attrs, ushort colorPair,
        IntPtr reserved);

    int slk_wset(int labelIndex, string title, int just);

    uint term_attrs();

    int unget_wch(char @char);

    int wadd_wch(CursesWindow window, CursesRune rune);

    int wadd_wchnstr(CursesWindow window, CursesRune rune, int count);

    int waddnwstr(CursesWindow window, string text, int length);

    int wbkgrnd(CursesWindow window, CursesRune rune);

    void wbkgrndset(CursesWindow window, CursesRune rune);

    int wborder_set(CursesWindow window, CursesRune leftSide, CursesRune rightSide, CursesRune topSide,
        CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner, CursesRune bottomLeftCorner,
        CursesRune bottomRightCorner);

    int wecho_wchar(CursesWindow window, CursesRune rune);

    int wget_wch(CursesWindow window, StringBuilder dest);

    int wgetbkgrnd(CursesWindow window, CursesRune rune);

    int wgetn_wstr(CursesWindow window, StringBuilder dest, int length);

    int whline_set(CursesWindow window, CursesRune rune, int count);

    int win_wch(CursesWindow window, CursesRune rune);

    int win_wchnstr(CursesWindow window, CursesRune rune, int length);

    int winnwstr(CursesWindow window, string text, int length);

    int wins_nwstr(CursesWindow window, string text, int length);

    int wins_wch(CursesWindow window, CursesRune rune);

    int winwstr(CursesWindow window, string text);

    string wunctrl(CursesRune rune);

    int wvline_set(CursesWindow window, CursesRune rune, int count);
}
