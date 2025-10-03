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
///     Opaque Curses character with attributes and color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct NCursesComplexChar
{
    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _attrAndColorPair;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char0;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char1;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char2;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char3;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char4;

    /// <inheritdoc cref="object.ToString" />
    public override readonly string ToString() =>
        $"{_attrAndColorPair:X8}-{_char0:X8}:{_char1:X8}:{_char2:X8}:{_char3:X8}:{_char4:X8}";
}
