namespace Sharpie.Curses;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using CursesWindow = IntPtr;
using CursesRune = IntPtr;

/// <summary>
/// Interface provides access to the Curses functionality. Use the <see cref="System"/> property to access the actual
/// implementation.
/// </summary>
[PublicAPI,SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable IdentifierTypo
public sealed class NativeCursesProvider: ICursesProvider
{
    int baudrate();

    void beep();

    bool can_change_color();

    void cbreak();

    void clearok(CursesWindow window, bool set);

    void color_content(short color, out short red, out short green, out short blue);

    void copywin(CursesWindow fromWindow, CursesWindow toWindow, int srcMinLine, int srcMinCol,
        int destMinLine, int destMinCol, int destMaxLine, int destMaxCol,
        int overlay);

    void curs_set(int level);

    void def_prog_mode();

    void def_shell_mode();

    void delay_output(int delayMillis);

    void delwin(CursesWindow window);

    CursesWindow derwin(CursesWindow window, int lines, int cols, int beginLine,
        int beginCol);

    void doupdate();

    CursesWindow dupwin(CursesWindow window);

    void echo();

    void endwin();

    char erasechar();

    void filter();

    void flash();

    void flushinp();

    void halfdelay(int tenthsOfSec);

    bool has_colors();

    bool has_ic();

    bool has_il();

    void idcok(CursesWindow window, bool set);

    void idlok(CursesWindow window, bool set);

    void immedok(CursesWindow window, bool set);

    CursesWindow initscr();

    int init_color(short color, short red, short green, short blue);

    int init_pair(short colorPair, short fgColor, short bgColor);

    void intrflush(CursesWindow window, bool set);

    bool isendwin();

    bool is_linetouched(CursesWindow window, int line);

    bool is_wintouched(CursesWindow window);

    string keyname(int keyCode);

    void keypad(CursesWindow window, bool set);

    char killchar();

    void leaveok(CursesWindow window, bool set);

    string longname();

    void meta(CursesWindow window, bool set);

    void mvderwin(CursesWindow window, int parentLine, int parentCol);

    void mvwin(CursesWindow window, int toLine, int toCol);

    CursesWindow newpad(int lines, int cols);

    CursesWindow newwin(int lines, int cols, int atLine, int atCol);

    void nl();

    void nocbreak();

    void nodelay(CursesWindow window, bool set);

    void noecho();

    void nonl();

    void noqiflush();

    void noraw();

    void notimeout(CursesWindow window, bool set);

    void overlay(CursesWindow srcWindow, CursesWindow destWindow);

    void overwrite(CursesWindow srcWindow, CursesWindow destWindow);

    void pair_content(short colorPair, out short fgColor, out short bgColor);

    int COLOR_PAIR(int colorPair);

    int PAIR_NUMBER(int attrOrChar);

    void pechochar(CursesWindow pad, char @char);

    void pnoutrefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    void prefresh(CursesWindow pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    void qiflush();

    void raw();

    void resetty();

    void reset_prog_mode();

    void reset_shell_mode();

    void ripoffline(int lines, ICursesProvider.ripoffline_callback callback);

    void savetty();

    void scrollok(CursesWindow window, bool set);

    void slk_attroff(char attrs);

    void slk_attron(char attrs);

    void slk_attrset(char attrs);

    char slk_attr();

    void slk_attr_set(char attrs, short colorPair, IntPtr reserved);

    void slk_clear();

    void slk_color(short colorPair);

    void slk_init(int format);

    string slk_label(int labelIndex);

    void slk_noutrefresh();

    void slk_refresh();

    void slk_restore();

    void slk_touch();

    void start_color();

    CursesWindow subpad(CursesWindow pad, int lines, int cols, int atRow,
        int atCol);

    CursesWindow subwin(CursesWindow window, int lines, int cols, int atLine,
        int atCol);

    void syncok(CursesWindow window, bool set);

    char termattrs();

    string termname();

    void ungetch(int @char);

    void use_env(bool set);

    void waddch(CursesWindow window, char @char);

    void waddchnstr(CursesWindow window, string text, int length);

    void wattr_on(CursesWindow window, char attrs, IntPtr reserved);

    void wattr_off(CursesWindow window, char attrs, IntPtr reserved);

    void wbkgd(CursesWindow window, char charAndAttrs);

    void wbkgdset(CursesWindow window, char charAndAttrs);

    void wborder(CursesWindow window, CursesRune leftSide, CursesRune rightSide, CursesRune topSide,
        CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner, CursesRune bottomLeftCorner,
        CursesRune bottomRightCorner);

    void wchgat(CursesWindow window, int count, ushort attrs, short colorPair,
        IntPtr reserved);

    void wclear(CursesWindow window);

    void wclrtobot(CursesWindow window);

    void wclrtoeol(CursesWindow window);

    void wcolor_set(CursesWindow window, short pair, IntPtr reserved);

    void wcursyncup(CursesWindow window);

    void wdelch(CursesWindow window);

    void wechochar(CursesWindow window, char @char);

    void werase(CursesWindow window);

    int wgetch(CursesWindow window);

    void wgetnstr(CursesWindow window, StringBuilder dest, int length);

    void whline(CursesWindow window, char @char, int count);

    char winch(CursesWindow window);

    void winchnstr(CursesWindow window, StringBuilder dest, int length);

    void winsch(CursesWindow window, char @char);

    void winsdelln(CursesWindow window, int count);

    void wmove(CursesWindow window, int newLine, int newCol);

    void wnoutrefresh(CursesWindow window);

    void wredrawln(CursesWindow window, int startLine, int lineCount);

    void wrefresh(CursesWindow window);

    void wscrl(CursesWindow window, int count);

    void wsetscrreg(CursesWindow window, int top, int bottom);

    void wsyncdown(CursesWindow window);

    void wsyncup(CursesWindow window);

    void wtimeout(CursesWindow window, int delay);

    void wtouchln(CursesWindow window, int line, int count, int changed);

    void wvline(CursesWindow window, char @char, int count);

    bool is_term_resized(int lines, int cols);

    void resize_term(int lines, int cols);

    void resizeterm(int lines, int cols);

    string keybound(int keyCode, int count);

    string curses_version();

    void assume_default_colors(int fgColor, int bgColor);

    void define_key(string keyName, int keyCode);

    int get_escdelay();

    int key_defined(string keyName);

    void keyok(int keyCode, bool set);

    void set_escdelay(int tenths);

    void set_tabsize(int size);

    void use_default_colors();

    void wresize(CursesWindow window, int lines, int columns);

    void nofilter();

    void getcchar(CursesRune rune, StringBuilder dest, ref ushort attrs, ref short colorPair,
        IntPtr reserved);

    string key_name(char @char);

    void killwchar(string text);

    void pecho_wchar(CursesWindow window, CursesRune rune);

    void setcchar(CursesRune rune, string text, ushort attrs, short colorPair,
        IntPtr reserved);

    void slk_wset(int labelIndex, string title, int just);

    ushort term_attrs();

    void unget_wch(char @char);

    void wadd_wch(CursesWindow window, CursesRune rune);

    void wadd_wchnstr(CursesWindow window, CursesRune rune, int count);

    void waddnwstr(CursesWindow window, string text, int length);

    void wbkgrnd(CursesWindow window, CursesRune rune);

    void wbkgrndset(CursesWindow window, CursesRune rune);

    void wborder_set(CursesWindow window, CursesRune leftSide, CursesRune rightSide,
        CursesRune topSide, CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner,
        CursesRune bottomLeftCorner, CursesRune bottomRightCorner);

    void wecho_wchar(CursesWindow window, CursesRune rune);

    void wget_wch(CursesWindow window, StringBuilder dest);

    void wgetbkgrnd(CursesWindow window, CursesRune rune);

    void wgetn_wstr(CursesWindow window, StringBuilder dest, int length);

    void whline_set(CursesWindow window, CursesRune rune, int count);

    void win_wch(CursesWindow window, CursesRune rune);

    void win_wchnstr(CursesWindow window, CursesRune rune, int length);

    void winnwstr(CursesWindow window, string text, int length);

    void wins_nwstr(CursesWindow window, string text, int length);

    void wins_wch(CursesWindow window, CursesRune rune);

    void winwstr(CursesWindow window, string text);

    string wunctrl(CursesRune rune);

    void wvline_set(CursesWindow window, CursesRune rune, int count);
}
