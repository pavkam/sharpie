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

namespace Sharpie;

/// <summary>
///     Stores the terminal options.
/// </summary>
/// <param name="UseColors">Toggle the use of colors. Default is <c>true</c>.</param>
/// <param name="EchoInput">Toggle the input echoing. Default is <c>false</c>.</param>
/// <param name="UseInputBuffering">Toggles buffering input. Default is <c>false</c>.</param>
/// <param name="UseMouse">Toggles the use of mouse. Default is <c>true</c>.</param>
/// <param name="MouseClickInterval">
///     The mouse click delay. Default is <c>null</c> which disables Curses handling of
///     clicks.
/// </param>
/// <param name="SuppressControlKeys">Toggles the suppression of control keys such as CTRL+C. Default is <c>true</c>.</param>
/// <param name="CaretMode">Specifies the caret mode. Default is <see cref="Sharpie.CaretMode.Visible" />.</param>
/// <param name="ManualFlush">Toggle the ability to manually flush the terminal. Default is <c>false</c>.</param>
/// <param name="ManagedWindows">
///     Specifies whether the <see cref="Screen" /> manages overlapping windows. Default is
///     <c>false</c>.
/// </param>
/// <param name="SoftLabelKeyMode">Specifies the SLK mode. Default is <see cref="Sharpie.SoftLabelKeyMode.Disabled" />.</param>
/// <param name="AllocateHeader">If <c>true</c>, allocates one line at the top as a header.</param>
/// <param name="AllocateFooter">If <c>true</c>, allocates one line at the bottom as a footer.</param>
/// <param name="UseEnvironmentOverrides">Toggles the use of environment LINE/COL overrides. Default is <c>true</c>.</param>
/// <param name="UseStandardKeySequenceResolvers">
///     Registers the standard key sequence resolvers defined in
///     <see cref="KeySequenceResolver" />.
/// </param>
[PublicAPI]
public record TerminalOptions(bool UseColors = true, bool EchoInput = false, bool UseInputBuffering = false,
    bool UseMouse = true, int? MouseClickInterval = null, bool SuppressControlKeys = true,
    CaretMode CaretMode = CaretMode.Visible, bool ManualFlush = false, bool ManagedWindows = false,
    SoftLabelKeyMode SoftLabelKeyMode = SoftLabelKeyMode.Disabled, bool AllocateHeader = false,
    bool AllocateFooter = false, bool UseEnvironmentOverrides = true, bool UseStandardKeySequenceResolvers = true);
