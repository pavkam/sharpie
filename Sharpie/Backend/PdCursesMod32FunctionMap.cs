/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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
///     Function map for PDCurses library.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "InconsistentNaming")]
internal abstract class PdCursesMod32FunctionMap: BaseCursesFunctionMap
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int endwin_w32_4302();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getcchar(ref uint @char, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, out uint attrs,
        out short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool is_immedok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool is_scrollok(IntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int nc_getmouse(out CursesMouseState state);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int setcchar(out uint @char, [MarshalAs(UnmanagedType.LPWStr)] string text, uint attrs,
        short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_attr();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_attr_off(uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_attr_on(uint attrs, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_attr_set(uint attrs, short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_clear();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_color(short colorPair);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_init(int format);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_noutrefresh();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_refresh();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_restore();

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate int slk_set(int labelIndex, string title, int fmt);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int slk_touch();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wadd_wch(IntPtr window, ref uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wbkgrnd(IntPtr window, ref uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wborder_set(IntPtr window, ref uint leftSide, ref uint rightSide, ref uint topSide,
        ref uint bottomSide, ref uint topLeftCorner, ref uint topRightCorner, ref uint bottomLeftCorner,
        ref uint bottomRightCorner);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wgetbkgrnd(IntPtr window, out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int whline_set(IntPtr window, ref uint @char, int count);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int win_wch(IntPtr window, out uint @char);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wvline_set(IntPtr window, ref uint @char, int count);
}
