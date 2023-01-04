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

namespace Sharpie.Backend;

/// <summary>
///     Base function map for Curses-like libraries.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "InconsistentNaming")]
internal abstract class BaseCursesFunctionMap
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int assume_default_colors(int fgColor, int bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int baudrate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int beep();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool can_change_color();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int cbreak();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int color_content(short color, out short red, out short green, out short blue);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int copywin(IntPtr fromWindow, IntPtr toWindow, int srcMinLine, int srcMinCol,
        int destMinLine, int destMinCol, int destMaxLine, int destMaxCol,
        int overlay);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int curs_set(int level);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate IntPtr curses_version();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int delwin(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int doupdate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr dupwin(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int echo();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int erasewchar(out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int flash();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getbegx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getbegy(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getcurx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getcury(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getmaxx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getmaxy(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getparx(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getpary(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool has_colors();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void immedok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int init_color(short color, short red, short green, short blue);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int init_pair(short colorPair, short fgColor, short bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr initscr();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int intrflush(IntPtr window, bool set);


    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool is_leaveok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool is_linetouched(IntPtr window, int line);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool is_wintouched(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate IntPtr key_name(uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int keypad(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int killwchar(out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int leaveok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate IntPtr longname();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int meta(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int mouseinterval(int millis);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int mousemask(uint newMask, out uint oldMask);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int mvderwin(IntPtr window, int parentLine, int parentCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int mvwin(IntPtr window, int toLine, int toCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr newpad(int lines, int cols);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr newwin(int lines, int cols, int atLine, int atCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int nl();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int nocbreak();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int nodelay(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int noecho();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int nonl();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void noqiflush();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int noraw();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int notimeout(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int overlay(IntPtr srcWindow, IntPtr destWindow);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int overwrite(IntPtr srcWindow, IntPtr destWindow);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int pair_content(short colorPair, out short fgColor, out short bgColor);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void qiflush();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int raw();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int ripoffline(int lines, ICursesBackend.ripoffline_callback callback);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int scrollok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int start_color();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr subpad(IntPtr pad, int lines, int cols, int atRow,
        int atCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int syncok(IntPtr window, bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int term_attrs();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr termname();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int use_default_colors();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void use_env(bool set);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wattr_get(IntPtr window, out uint attrs, out short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wattr_off(IntPtr window, uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wattr_on(IntPtr window, uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wattr_set(IntPtr window, uint attrs, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wchgat(IntPtr window, int count, uint attrs, short colorPair,
        IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wclrtobot(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wclrtoeol(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wcolor_set(IntPtr window, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wdelch(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int werase(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wget_wch(IntPtr window, out uint dest);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int whline(IntPtr window, uint @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int winsdelln(IntPtr window, int count);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wmove(IntPtr window, int newLine, int newCol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wnoutrefresh(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wredrawln(IntPtr window, int startLine, int lineCount);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wrefresh(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wresize(IntPtr window, int lines, int columns);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wscrl(IntPtr window, int count);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void wtimeout(IntPtr window, int delay);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wtouchln(IntPtr window, int line, int count, int changed);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wvline(IntPtr window, uint @char, int count);
}
