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
internal abstract class PdCursesFunctionMap: BaseCursesFunctionMap
{
#pragma warning disable IDE1006 // Naming Styles -- these are native names
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int endwin();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getcchar(ref uint @char, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dest, out uint attrs,
        out short colorPair, IntPtr reserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int getmouse(out CursesMouseState state);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int setcchar(out uint @char, [MarshalAs(UnmanagedType.LPWStr)] string text, uint attrs,
        short colorPair, IntPtr reserved);

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

#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning restore IDE1006 // Naming Styles
}
