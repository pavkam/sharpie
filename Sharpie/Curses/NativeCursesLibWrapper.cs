namespace Sharpie;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Curses;
using CursesMouseMask = UInt16;
using CursesAttr = UInt16;
using CursesPair = Int16;
using CursesWindow = IntPtr;
using CursesRune = IntPtr;

internal static class NativeCursesLibWrapper
 {
     private int ERR = -1;

     private static void Wrap(Func<int> call, [CallerMemberName] string caller = "")
     {
         var result = call();
         if (result == ERR)
         {
             throw new CursesException(caller);
         }
     }


     private const string libraryName = "ncurses";

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int baudrate();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int beep();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool can_change_color();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int cbreak();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int clearok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int color_content(short color, out short red, out short green, out short blue);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int copywin(CursesWindow fromWindow, CursesWindow toWindow,
         int srcMinLine, int srcMinCol, int destMinLine, int destMinCol, int destMaxLine,
         int destMaxCol, int overlay);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int curs_set(int level);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int def_prog_mode();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int def_shell_mode();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int delay_output(int delayMillis);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int delwin(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow derwin(CursesWindow window, int lines, int cols,
         int beginLine, int beginCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int doupdate();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow dupwin(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int echo();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int endwin();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern char erasechar();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void filter();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int flash();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int flushinp();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int halfdelay(int tenthsOfSec);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool has_colors();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool has_ic();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool has_il();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void idcok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int idlok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void immedok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow initscr();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int init_color(short color, short red, short green, short blue);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int init_pair(short colorPair, short fgColor, short bgColor);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int intrflush(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool isendwin();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool is_linetouched(CursesWindow window, int line);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool is_wintouched(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string keyname(int keyCode);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int keypad(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern char killchar();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int leaveok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string longname();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int meta(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int mvderwin(CursesWindow window, int parentLine, int parentCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int mvwin(CursesWindow window, int toLine, int toCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow newpad(int lines, int cols);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow newwin(int lines, int cols, int atLine, int atCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int nl();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int nocbreak();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int nodelay(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int noecho();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int nonl();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void noqiflush();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int noraw();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int notimeout(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int overlay(CursesWindow srcWindow, CursesWindow destWindow);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int overwrite(CursesWindow srcWindow, CursesWindow destWindow);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int pair_content(short colorPair, out short fgColor, out short bgColor);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int COLOR_PAIR(int colorPair);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int PAIR_NUMBER(int attrOrChar);


     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int pechochar(CursesWindow pad, char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int pnoutrefresh(CursesWindow pad, int padMinLine, int padMinCol,
         int scrMinLine, int scrMinCol, int scrMaxLine, int scrMaxCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int prefresh(CursesWindow pad, int padMinLine, int padMinCol,
         int scrMinLine, int scrMinCol, int scrMaxLine, int scrMaxCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void qiflush();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int raw();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int resetty();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int reset_prog_mode();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int reset_shell_mode();

     public delegate bool ripoffline_callback(CursesWindow window, int columns);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int ripoffline(int lines, ripoffline_callback callback);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int savetty();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int scrollok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_attroff(char attrs);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_attron(char attrs);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_attrset(char attrs);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern char slk_attr();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_attr_set(char attrs, short colorPair, IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_clear();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_color(short colorPair);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_init(int format);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string slk_label(int labelIndex);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_noutrefresh();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_refresh();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_restore();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_touch();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int start_color();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow subpad(CursesWindow pad, int lines, int cols,
         int atRow, int atCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesWindow subwin(CursesWindow window, int lines, int cols, int atLine, int atCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int syncok(CursesWindow window, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern char termattrs();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string termname();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int ungetch(int @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void use_env(bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int waddch(CursesWindow window, char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int waddchnstr(CursesWindow window, string text, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wattr_on(CursesWindow window, char attrs, IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wattr_off(CursesWindow window, char attrs, IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wbkgd(CursesWindow window, char charAndAttrs);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wbkgdset(CursesWindow window, char charAndAttrs);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wborder(CursesWindow window, CursesRune leftSide, CursesRune rightSide, CursesRune topSide,
         CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner, CursesRune bottomLeftCorner,
         CursesRune bottomRightCorner);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wchgat(CursesWindow window, int count, ushort attrs, short colorPair,
         IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wclear(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wclrtobot(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wclrtoeol(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wcolor_set(CursesWindow window, short pair, IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wcursyncup(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wdelch(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wechochar(CursesWindow window, char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int werase(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wgetch(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern int wgetnstr(CursesWindow window, StringBuilder dest, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int whline(CursesWindow window, char @char, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern char winch(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int winchnstr(CursesWindow window, StringBuilder dest, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int winsch(CursesWindow window, char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int winsdelln(CursesWindow window, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wmove(CursesWindow window, int newLine, int newCol);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wnoutrefresh(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wredrawln(CursesWindow window, int startLine, int lineCount);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wrefresh(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wscrl(CursesWindow window, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wsetscrreg(CursesWindow window, int top, int bottom);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wsyncdown(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wsyncup(CursesWindow window);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wtimeout(CursesWindow window, int delay);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wtouchln(CursesWindow window, int line, int count, int changed);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wvline(CursesWindow window, char @char, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern bool is_term_resized(int lines, int cols);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int resize_term(int lines, int cols);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int resizeterm(int lines, int cols);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string keybound(int keyCode, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string curses_version();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int assume_default_colors(int fgColor, int bgColor);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int define_key(string keyName, int keyCode);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int get_escdelay();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern int key_defined(string keyName);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int keyok(int keyCode, bool set);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int set_escdelay(int tenths);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int set_tabsize(int size);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int use_default_colors();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wresize(CursesWindow window, int lines, int columns);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void nofilter();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int getcchar(CursesRune rune, StringBuilder dest, ref CursesAttr attrs, ref short colorPair,
         IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
     public static extern string key_name([MarshalAs(UnmanagedType.U2)] char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int killwchar(string text);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int pecho_wchar(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int setcchar(CursesRune rune, string text, CursesAttr attrs, short colorPair,
         IntPtr reserved);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int slk_wset(int labelIndex, string title, int just);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern CursesAttr term_attrs();

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int unget_wch(char @char);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wadd_wch(CursesWindow window, CursesRune rune);

     public static extern int wadd_wchnstr(CursesWindow window, CursesRune rune, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int waddnwstr(CursesWindow window, string text, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wbkgrnd(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern void wbkgrndset(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wborder_set(CursesWindow window, CursesRune leftSide, CursesRune rightSide, CursesRune topSide,
         CursesRune bottomSide, CursesRune topLeftCorner, CursesRune topRightCorner, CursesRune bottomLeftCorner,
         CursesRune bottomRightCorner);

     public static extern int wecho_wchar(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wget_wch(CursesWindow window, StringBuilder dest);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wgetbkgrnd(CursesWindow window, CursesRune rune); /* generated:WIDEC */

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wgetn_wstr(CursesWindow window, StringBuilder dest, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int whline_set(CursesWindow window, CursesRune rune, int count);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int win_wch(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int win_wchnstr(CursesWindow window, CursesRune rune, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int winnwstr(CursesWindow window, string text, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wins_nwstr(CursesWindow window, string text, int length);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wins_wch(CursesWindow window, CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int winwstr(CursesWindow window, string text);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern string wunctrl(CursesRune rune);

     [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
     public static extern int wvline_set(CursesWindow window, CursesRune rune, int count);
 }
