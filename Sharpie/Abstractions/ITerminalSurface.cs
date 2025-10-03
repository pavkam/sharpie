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
    ITerminal Terminal
    {
        get;
    }

    /// <summary>
    ///     Set or get the immediate refresh capability of the surface.
    /// </summary>
    /// <remarks>
    ///     Immediate refresh will make the surface redraw affected areas on each change.
    ///     This might be very slow for most use cases so the default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool ImmediateRefresh
    {
        get; set;
    }

    /// <summary>
    ///     Redraws all the dirty lines of the surface to the terminal. If <see cref="ITerminal.AtomicRefresh" /> is active,
    ///     all refreshes are batched together until the lock is released.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Refresh();

    /// <summary>
    ///     Redraws the given lines of the window to the terminal.
    /// </summary>
    /// <param name="y">The starting line to refresh.</param>
    /// <param name="count">The number of lines to refresh.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="y" /> or <paramref name="count" /> are
    ///     negative.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if an atomic refresh is in progress.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Refresh(int y, int count);
}
