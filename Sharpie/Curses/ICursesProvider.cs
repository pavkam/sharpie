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

namespace Sharpie.Curses;

/// <summary>
///     Interface provides access to the Curses functionality. Use the <see cref="NativeCursesProvider" /> property to
///     access the actual
///     implementation.
/// </summary>
[PublicAPI, SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "IdentifierTypo")]
public interface ICursesProvider
{
    public delegate bool ripoffline_callback(IntPtr window, int columns); //TODO

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
        int beginCol); //DONE

    int doupdate(); //DONE

    IntPtr dupwin(IntPtr window); //DONE

    int echo(); //DONE

    int endwin(); //DONE

    int erasewchar(out uint @char); //DONE

    void filter(); //NO

    int flash(); //DONE

    int flushinp(); //NO

    uint getattrs(IntPtr window); //NO

    int getcurx(IntPtr window); //DONE

    int getcury(IntPtr window); //DONE

    int getbegx(IntPtr window); //DONE

    int getbegy(IntPtr window); //DONE

    int getmaxx(IntPtr window); //DONE

    int getmaxy(IntPtr window); //DONE

    int getparx(IntPtr window); //DONE

    int getpary(IntPtr window); //DONE

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

    bool is_keypad(IntPtr window); //NO

    bool is_leaveok(IntPtr window); //DONE

    bool is_nodelay(IntPtr window); //NO

    bool is_notimeout(IntPtr window); //NO

    bool is_pad(IntPtr window); //NO

    bool is_scrollok(IntPtr window); //DONE

    bool is_subwin(IntPtr window); //DONE

    bool is_syncok(IntPtr window); //NO

    IntPtr wgetparent(IntPtr window); //NO

    int wgetdelay(IntPtr window); //NO

    int wgetscrreg(IntPtr window, out int top, out int bottom); //NO

    string? keyname(uint keyCode); //NO

    int keypad(IntPtr window, bool set); //DONE

    int leaveok(IntPtr window, bool set); //DONE

    string? longname(); //DONE

    int meta(IntPtr window, bool set); //DONE

    int mvderwin(IntPtr window, int parentLine, int parentCol); //DONE

    int mvwin(IntPtr window, int toLine, int toCol); //DONE

    IntPtr newpad(int lines, int cols); //DONE

    IntPtr newwin(int lines, int cols, int atLine, int atCol); //DONE

    int nl(); //DONE

    int nocbreak(); //DONE

    int nodelay(IntPtr window, bool set); //DONE

    int noecho(); //DONE

    int nonl(); //DONE

    void noqiflush(); //DONE

    int noraw(); //DONE

    int notimeout(IntPtr window, bool set); //DONE

    int overlay(IntPtr srcWindow, IntPtr destWindow); //DONE

    int overwrite(IntPtr srcWindow, IntPtr destWindow); //DONE

    int pair_content(ushort colorPair, out ushort fgColor, out ushort bgColor); //DONE

    uint COLOR_PAIR(uint attrs); //DONE

    uint PAIR_NUMBER(uint colorPair); //DONE

    int pechochar(IntPtr pad, uint charAndAttrs); //NO

    int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol); //DONE

    void qiflush(); //DONE

    int raw(); //DONE

    int resetty(); //NO

    int reset_prog_mode(); //NO

    int reset_shell_mode(); //NO

    int ripoffline(int lines, ripoffline_callback callback); //TODO

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
        int atCol); //DONE

    IntPtr subwin(IntPtr window, int lines, int cols, int atLine,
        int atCol); //NO

    int syncok(IntPtr window, bool set); //DONE

    string? termname(); //DONE

    int ungetch(uint @char); //NO

    void use_env(bool set); //DONE

    int waddch(IntPtr window, uint charAndAttrs); //NO

    int waddchnstr(IntPtr window, string text, int length); //NO

    int wattr_get(IntPtr window, out uint attrs, out ushort colorPair, IntPtr reserved); //DONE

    int wattr_set(IntPtr window, uint attrs, ushort colorPair, IntPtr reserved); //DONE

    int wattr_on(IntPtr window, uint attrs, IntPtr reserved); //DONE

    int wattr_off(IntPtr window, uint attrs, IntPtr reserved); //DONE

    int wbkgd(IntPtr window, uint charAndAttrs); //NO

    void wbkgdset(IntPtr window, uint charAndAttrs); //NO

    int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner); //DONE

    int wchgat(IntPtr window, int count, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int wclear(IntPtr window); //NO

    int wclrtobot(IntPtr window); //DONE

    int wclrtoeol(IntPtr window); //DONE

    int wcolor_set(IntPtr window, ushort pair, IntPtr reserved); //DONE

    void wcursyncup(IntPtr window); //NO

    int wdelch(IntPtr window); //DONE

    int wechochar(IntPtr window, uint charAndAttrs); //NO

    int werase(IntPtr window); //DONE

    int wgetch(IntPtr window); //NO

    int wgetnstr(IntPtr window, StringBuilder dest, int length); //NO

    int whline(IntPtr window, uint @char, int count); //DONE

    uint winch(IntPtr window); //NO

    int winchnstr(IntPtr window, StringBuilder dest, int length); //NO

    int winsch(IntPtr window, uint @char); //NO

    int winsdelln(IntPtr window, int count); //DONE

    int wmove(IntPtr window, int newLine, int newCol); //DONE

    int wnoutrefresh(IntPtr window); //DONE

    int wredrawln(IntPtr window, int startLine, int lineCount); //DONE

    int wrefresh(IntPtr window); //DONE

    int wscrl(IntPtr window, int count); //DONE

    int wsetscrreg(IntPtr window, int top, int bottom); //NO

    void wsyncdown(IntPtr window); //NO

    void wsyncup(IntPtr window); // NO

    void wtimeout(IntPtr window, int delay); //DONE

    int wtouchln(IntPtr window, int line, int count, int changed); //DONE

    int wvline(IntPtr window, uint @char, int count); //DONE

    bool is_term_resized(int lines, int cols); //NO

    int resize_term(int lines, int cols); //NO

    int resizeterm(int lines, int cols); //NO

    string? keybound(uint keyCode, int count); //TODO

    string? curses_version(); //DONE

    int assume_default_colors(int fgColor, int bgColor); //DONE

    int define_key(string keyName, int keyCode); //TODO

    int key_defined(string keyName); //TODO

    int keyok(int keyCode, bool set); //TODO

    int set_tabsize(int size); //NO

    int use_default_colors(); //DONE

    int wresize(IntPtr window, int lines, int columns); //DONE

    void nofilter(); //NO

    int getcchar(ComplexChar @char, StringBuilder dest, out uint attrs, out ushort colorPair,
        IntPtr reserved); //DONE

    string? key_name(uint @char); //DONE

    int killwchar(out uint @char); //DONE

    int pecho_wchar(IntPtr window, ComplexChar @char); //NO

    int setcchar(out ComplexChar @char, string text, uint attrs, ushort colorPair,
        IntPtr reserved); //DONE

    int slk_set(int labelIndex, string title, int align); //DONE

    uint term_attrs(); //DONE

    int unget_wch(uint @char); //NO

    int wadd_wch(IntPtr window, ComplexChar @char); //DONE

    int wadd_wchnstr(IntPtr window, ComplexChar[] str, int count); //DONE

    int waddnwstr(IntPtr window, string text, int length); //NO

    int wbkgrnd(IntPtr window, ComplexChar @char); //DONE

    void wbkgrndset(IntPtr window, ComplexChar @char); //NO

    int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide, ComplexChar topSide,
        ComplexChar bottomSide, ComplexChar topLeftCorner, ComplexChar topRightCorner, ComplexChar bottomLeftCorner,
        ComplexChar bottomRightCorner); //DONE

    int wecho_wchar(IntPtr window, ComplexChar @char); //NO

    int wget_wch(IntPtr window, out uint @char); //DONE

    int wgetbkgrnd(IntPtr window, out ComplexChar @char); //DONE

    int wgetn_wstr(IntPtr window, StringBuilder dest, int length); //NO

    int whline_set(IntPtr window, ComplexChar @char, int count); //DONE

    int win_wch(IntPtr window, out ComplexChar @char); //NO

    int win_wchnstr(IntPtr window, ComplexChar[] @char, int length); //DONE

    int winnwstr(IntPtr window, StringBuilder dest, int length); //NO

    int wins_nwstr(IntPtr window, string text, int length); //NO

    int wins_wch(IntPtr window, ComplexChar @char); //NO

    string? wunctrl(ComplexChar @char); //NO

    int wvline_set(IntPtr window, ComplexChar @char, int count); //DONE

    int getmouse(out RawMouseEvent @event); //DONE

    int ungetmouse(RawMouseEvent @event); //NO

    int mousemask(ulong newMask, out ulong oldMask); //DONE

    bool wenclose(IntPtr window, int line, int col); //DONE

    int mouseinterval(int millis); //DONE

    bool wmouse_trafo(IntPtr window, ref int line, ref int col, bool toScreen); //TODO
}
