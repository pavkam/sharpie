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
///     Lists the supported backend Curses libraries.
/// </summary>
[PublicAPI]
public enum CursesBackendType
{
    /// <summary>
    ///     The NCurses back-end.
    ///     Available by default on most POSIX-compliant operating systems such as
    ///     Linux, FreeBSD an MacOS. Not available by default on Windows.
    /// </summary>
    NCurses = 1,

    /// <summary>
    ///     The PDCurses back-end. Not available by default on operating systems.
    ///     For some Linuxes and FreeBSD a prebuilt library can be installed from package
    ///     managers.
    ///     Does not support <see cref="SoftLabelKeyManager" />.
    ///     Not recommended for use. <see cref="PdCursesMod" /> for a better alternative.
    /// </summary>
    PdCurses,

    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses" /> with more
    ///     platform availability and support. Not available by default on any operating systems.
    /// </summary>
    PdCursesMod
}
