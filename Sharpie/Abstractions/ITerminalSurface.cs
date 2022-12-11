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

namespace Sharpie.Abstractions;

/// <summary>
///     Defines the base traits required for <see cref="TerminalSurface" /> class.
/// </summary>
[PublicAPI]
public interface ITerminalSurface: ISurface
{
    /// <summary>
    ///     The terminal this surface belongs to.
    /// </summary>
    ITerminal Terminal { get; }

    /// <summary>
    ///     Set or get the immediate refresh capability of the surface.
    /// </summary>
    /// <remarks>
    ///     Immediate refresh will make the surface redraw affected areas on each change.
    ///     This might be very slow for most use cases so the default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    bool ImmediateRefresh { get; set; }

    /// <summary>
    ///     Set or get the flag indicating that the surface is critical and, thus, should
    ///     trigger the entire terminal to update on refresh.
    /// </summary>
    /// <remarks>
    ///     This setting might result in excessive refreshes so the default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    bool Critical { get; set; }

    /// <summary>
    ///     Redraws all the invalidated lines of the surface to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    void Refresh();

    /// <summary>
    ///     Redraws the given lines of the window to the terminal.
    /// </summary>
    /// <param name="y">The starting line to refresh.</param>
    /// <param name="count">The number of lines to refresh.</param>
    /// <exception cref="ArgumentOutOfRangeException">The combination of lines and count exceed the window boundary.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Refresh(int y, int count);
}
