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
///     Describes the traits specific to the <see cref="Pad" />.
/// </summary>
[PublicAPI]
public interface IPad: ISurface
{
    /// <summary>
    ///     The parent screen of this pad.
    /// </summary>
    IScreen Screen { get; }

    /// <summary>
    ///     Gets the sub-pads of this pad.
    /// </summary>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IEnumerable<ISubPad> SubPads { get; }

    /// <summary>
    ///     Gets or sets the size of the window.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    new Size Size { get; set; }

    /// <summary>
    ///     Redraws an area of the screen with the contents of the pad.
    /// </summary>
    /// <param name="srcArea">The rectangle of the pad to place onto the screen.</param>
    /// <param name="destLocation">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="srcArea" /> or <paramref name="destLocation" /> are
    ///     out of bounds.
    /// </exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Refresh(Rectangle srcArea, Point destLocation);

    /// <summary>
    ///     Redraws an area of the screen with the contents of the pad.
    /// </summary>
    /// <param name="destLocation">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="destLocation" /> is out of bounds.
    /// </exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Refresh(Point destLocation);

    /// <summary>
    ///     Creates a new sub-pad in the parent pad.
    /// </summary>
    /// <param name="area">The area of the pad to put the sub-pad in.</param>
    /// <remarks>
    /// </remarks>
    /// <returns>A new pad object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the bounds of the parent.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ISubPad SubPad(Rectangle area);

    /// <summary>
    ///     Duplicates and existing pad, including its attributes.
    /// </summary>
    /// <returns>A new pad object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IPad Duplicate();
}
