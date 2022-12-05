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
///     Describes the traits specific to the <see cref="Pad"/>.
/// </summary>
[PublicAPI]
public interface IPad: IWindow
{
    /// <summary>
    ///     Refreshes the pad by synchronizing it to the terminal screen.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <param name="entireScreen">If <c>true</c>, when this refresh happens, the entire screen is redrawn.</param>
    /// <param name="rect">The rectangle of the pad to place onto the screen.</param>
    /// <param name="screenPos">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="rect"/> or <paramref name="screenPos"/> are out of bounds.</exception>
    void Refresh(bool batch, bool entireScreen, Rectangle rect, Point screenPos);

    /// <summary>
    ///     Refreshes the pad by synchronizing it to the terminal screen with immediate redraw.
    /// </summary>
    /// <param name="rect">The rectangle of the pad to place onto the screen.</param>
    /// <param name="screenPos">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="rect"/> or <paramref name="screenPos"/> are out of bounds.</exception>
    void Refresh(Rectangle rect, Point screenPos) => Refresh(false, false, rect, screenPos);
}
