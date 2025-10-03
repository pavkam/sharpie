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

namespace Sharpie.Backend;

/// <summary>
///     Lists the supported backend Curses libraries' flavors.
/// </summary>
[PublicAPI]
public enum CursesBackendFlavor
{
    /// <summary>
    ///     The NCurses back-end.
    ///     Available by default on most POSIX-compliant operating systems such as
    ///     Linux, FreeBSD an MacOS. Not available by default on Windows.
    ///     Install <c>Curses-Libs-NCurses</c> package to obtain pre-built versions
    ///     that includes Windows.
    ///     This is version uses whatever available protocol is built into the found library.
    /// </summary>
    NCurses = 1,

    /// <summary>
    ///     The PDCurses back-end. Not available by default on operating systems.
    ///     For some Linuxes and FreeBSD a prebuilt library can be installed from package
    ///     managers.
    ///     Install <c>Curses-Libs-PdCurses</c> package to obtain pre-built versions
    ///     that add Windows Console support.
    ///     Does not support <see cref="SoftLabelKeyManager" />.
    ///     Not recommended for use. <see cref="PdCursesMod" /> for a better alternative.
    ///     This is version uses whatever available protocol is built into the found library.
    /// </summary>
    PdCurses,

    /// <summary>
    ///     The PDCurses back-end. Not available by default on operating systems.
    ///     For some Linuxes and FreeBSD a prebuilt library can be installed from package
    ///     managers.
    ///     Install <c>Curses-Libs-PdCurses</c> package to obtain pre-built versions
    ///     that add Windows Console support.
    ///     Does not support <see cref="SoftLabelKeyManager" />.
    ///     Not recommended for use. <see cref="PdCursesModWindowsConsole" /> for a better alternative.
    ///     This is version uses Windows Console protocol.
    /// </summary>
    [SupportedOSPlatform("windows")]
    PdCursesWindowsConsole,

    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses" /> with more
    ///     platform availability and support. Not available by default on any operating systems.
    ///     Install <c>Curses-Libs-PdCursesMod</c> package to obtain pre-built versions
    ///     for Windows.
    ///     This is version uses whatever available protocol is built into the found library.
    /// </summary>
    PdCursesMod,

    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses" /> with more
    ///     platform availability and support. Not available by default on any operating systems.
    ///     Install <c>Curses-Libs-PdCursesMod</c> package to obtain pre-built versions
    ///     for Windows.
    ///     This is version uses Windows Console protocol.
    /// </summary>
    [SupportedOSPlatform("windows")]
    PdCursesModWindowsConsole,

    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses" /> with more
    ///     platform availability and support. Not available by default on any operating systems.
    ///     Install <c>Curses-Libs-PdCursesMod</c> package to obtain pre-built versions
    ///     for Windows, Linux and MacOS.
    ///     This is version uses Virtual Terminal (xt) protocol.
    /// </summary>
    PdCursesModVirtualTerminal,

    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses" /> with more
    ///     platform availability and support. Not available by default on any operating systems.
    ///     Install <c>Curses-Libs-PdCursesMod</c> package to obtain pre-built versions
    ///     for Windows, Linux and MacOS.
    ///     This is version uses Windows GUI, or SDL1/SDL2 protocols. Requires `sdl/sdl2` and `sdl_ttf` to be installed
    ///     if this protocol is used.
    /// </summary>
    PdCursesModGui,

    /// <summary>
    ///     Any back-end that uses Windows Console protocol. Will select the best option available on the platform.
    ///     Install <c>Curses-Libs-PdCursesMod</c>, <c>Curses-Libs-NCurses</c> or <c>Curses-Libs-PdCurses</c> packages
    ///     to maximize the number of supported platform combinations.
    /// </summary>
    [SupportedOSPlatform("windows")]
    AnyWindowsConsole,

    /// <summary>
    ///     Any back-end that uses Virtual Terminal (xt) protocol. Will select the best option available on the platform.
    ///     Install <c>Curses-Libs-PdCursesMod</c>, <c>Curses-Libs-NCurses</c> or <c>Curses-Libs-PdCurses</c> packages
    ///     to maximize the number of supported platform combinations.
    /// </summary>
    AnyVirtualTerminal,

    /// <summary>
    ///     Any back-end that uses Windows GUI, or SDL1/SDL2 protocols. Will select the best option available on the platform.
    ///     Install <c>Curses-Libs-PdCursesMod</c>, <c>Curses-Libs-NCurses</c> or <c>Curses-Libs-PdCurses</c> packages
    ///     to maximize the number of supported platform combinations.
    /// </summary>
    AnyGui,

    /// <summary>
    ///     Any available back-end. Will select the best option available on the platform.
    ///     Install <c>Curses-Libs-PdCursesMod</c>, <c>Curses-Libs-NCurses</c> or <c>Curses-Libs-PdCurses</c> packages
    ///     to maximize the number of supported platform combinations.
    /// </summary>
    Any
}
