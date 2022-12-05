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

namespace Sharpie;

/// <summary>
///     Defines the traits that a <see cref="Screen"/> implements.
/// </summary>
[PublicAPI]
public interface IScreen: IWindow
{
    /// <summary>
    ///     The terminal this screen belongs to.
    /// </summary>
    ITerminal Terminal { get; }

    /// <summary>
    ///     Creates a new window in the screen.
    /// </summary>
    /// <param name="area">The area for the new window.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the screen bounds.</exception>
    IWindow CreateWindow(Rectangle area);

    /// <summary>
    ///     Creates a new sub-window in the parent window.
    /// </summary>
    /// <param name="window">The parent window.</param>
    /// <param name="area">The area of the window to put the sub-window in.</param>
    /// <remarks>
    /// </remarks>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="window" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the bounds of the parent.</exception>
    IWindow CreateSubWindow(IWindow window, Rectangle area);

    /// <summary>
    ///     Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="InvalidOperationException">Trying to duplicate the screen window.</exception>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="window" /> is <c>null</c>.</exception>
    IWindow DuplicateWindow(IWindow window);

    /// <summary>
    ///     Creates a new pad.
    /// </summary>
    /// <param name="size">The pad size.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="size" /> is invalid.</exception>
    IPad CreatePad(Size size);

    /// <summary>
    ///     Creates a new sub-pad.
    /// </summary>
    /// <param name="pad">The parent pad.</param>
    /// <param name="area">The are of the pad to use.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="area" /> is outside the pad's bounds.</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="pad" /> is <c>null</c>.</exception>
    IPad CreateSubPad(IPad pad, Rectangle area);

    /// <summary>
    ///     Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    void ApplyPendingRefreshes();

    /// <summary>
    ///     This method invalidates the screen in its entirety and redraws if from scratch.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    void ForceInvalidateAndRefresh()
    {
        Invalidate();
        foreach (var child in Children)
        {
            child.Invalidate();
        }

        Refresh(false, true);
    }
}