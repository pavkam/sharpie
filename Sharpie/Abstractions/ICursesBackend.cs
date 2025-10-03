/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
namespace Sharpie.Abstractions;

/// <summary>
///     Interface provides access to the Curses functionality. Use the <see cref="NCursesBackend" /> property to
///     access the actual
///     implementation.
/// </summary>
[PublicAPI, SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "IdentifierTypo")]
public interface ICursesBackend
{
#pragma warning disable IDE1006 // Naming Styles -- these are native names
    public delegate int ripoffline_callback(IntPtr window, int columns);

    int baudrate();

    int beep();

    bool can_change_color();

    int cbreak();

    int color_content(short color, out short red, out short green, out short blue);

    int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay);

    int curs_set(int level);

    int delwin(IntPtr window);

    IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol);

    int doupdate();

    IntPtr dupwin(IntPtr window);

    int echo();

    int endwin();

    int erasewchar(out uint @char);

    int flash();

    int getcurx(IntPtr window);

    int getcury(IntPtr window);

    int getbegx(IntPtr window);

    int getbegy(IntPtr window);

    int getmaxx(IntPtr window);

    int getmaxy(IntPtr window);

    int getparx(IntPtr window);

    int getpary(IntPtr window);

    bool has_colors();

    void immedok(IntPtr window, bool set);

    IntPtr initscr();

    int init_color(short color, short red, short green, short blue);

    int init_pair(short colorPair, short fgColor, short bgColor);

    int intrflush(IntPtr window, bool set);

    bool is_linetouched(IntPtr window, int line);

    bool is_wintouched(IntPtr window);

    bool is_immedok(IntPtr window);

    bool is_leaveok(IntPtr window);

    bool is_scrollok(IntPtr window);

    int keypad(IntPtr window, bool set);

    int leaveok(IntPtr window, bool set);

    string? longname();

    int meta(IntPtr window, bool set);

    int mvderwin(IntPtr window, int parentLine, int parentCol);

    int mvwin(IntPtr window, int toLine, int toCol);

    IntPtr newpad(int lines, int cols);

    IntPtr newwin(int lines, int cols, int atLine, int atCol);

    int nl();

    int nocbreak();

    int nodelay(IntPtr window, bool set);

    int noecho();

    int nonl();

    void noqiflush();

    int noraw();

    int notimeout(IntPtr window, bool set);

    int overlay(IntPtr srcWindow, IntPtr destWindow);

    int overwrite(IntPtr srcWindow, IntPtr destWindow);

    int pair_content(short colorPair, out short fgColor, out short bgColor);

    int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    void qiflush();

    int raw();

    int ripoffline(int lines, ripoffline_callback callback);

    int scrollok(IntPtr window, bool set);

    int slk_attr_off(VideoAttribute attributes, IntPtr reserved);

    int slk_attr_on(VideoAttribute attributes, IntPtr reserved);

    int slk_attr(out VideoAttribute attributes, out short colorPair);

    int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved);

    int slk_clear();

    int slk_color(short colorPair);

    int slk_init(int format);

    int slk_noutrefresh();

    int slk_refresh();

    int slk_restore();

    int slk_touch();

    int start_color();

    IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol);

    int syncok(IntPtr window, bool set);

    string? termname();

    void use_env(bool set);

    int wattr_get(IntPtr window, out VideoAttribute attributes, out short colorPair, IntPtr reserved);

    int wattr_set(IntPtr window, VideoAttribute attributes, short colorPair, IntPtr reserved);

    int wattr_on(IntPtr window, VideoAttribute attributes, IntPtr reserved);

    int wattr_off(IntPtr window, VideoAttribute attributes, IntPtr reserved);

    int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner);

    int wchgat(IntPtr window, int count, VideoAttribute attributes, short colorPair,
        IntPtr reserved);

    int wclrtobot(IntPtr window);

    int wclrtoeol(IntPtr window);

    int wcolor_set(IntPtr window, short colorPair, IntPtr reserved);

    int wdelch(IntPtr window);

    int werase(IntPtr window);

    int whline(IntPtr window, uint @char, int count);

    int winsdelln(IntPtr window, int count);

    int wmove(IntPtr window, int newLine, int newCol);

    int wnoutrefresh(IntPtr window);

    int wredrawln(IntPtr window, int startLine, int lineCount);

    int wrefresh(IntPtr window);

    int wscrl(IntPtr window, int count);

    void wtimeout(IntPtr window, int delay);

    int wtouchln(IntPtr window, int line, int count, int changed);

    int wvline(IntPtr window, uint @char, int count);

    string? curses_version();

    int assume_default_colors(int fgColor, int bgColor);

    int use_default_colors();

    int wresize(IntPtr window, int lines, int columns);

    int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes, out short colorPair,
        IntPtr reserved);

    string? key_name(uint @char);

    int killwchar(out uint @char);

    int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved);

    int slk_set(int labelIndex, string title, int align);

    int term_attrs(out VideoAttribute attributes);

    int wadd_wch(IntPtr window, ComplexChar @char);

    int wbkgrnd(IntPtr window, ComplexChar @char);

    int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide, ComplexChar topSide,
        ComplexChar bottomSide, ComplexChar topLeftCorner, ComplexChar topRightCorner, ComplexChar bottomLeftCorner,
        ComplexChar bottomRightCorner);

    int wget_wch(IntPtr window, out uint @char);

    int wget_event(IntPtr window, int delay, out CursesEvent? @event);

    int wgetbkgrnd(IntPtr window, out ComplexChar @char);

    int whline_set(IntPtr window, ComplexChar @char, int count);

    int win_wch(IntPtr window, out ComplexChar @char);

    int wvline_set(IntPtr window, ComplexChar @char, int count);

    int getmouse(out CursesMouseState state);

    int mousemask(uint newMask, out uint oldMask);

    int mouseinterval(int millis);

    void set_title(string title);
#pragma warning restore IDE1006 // Naming Styles
}
