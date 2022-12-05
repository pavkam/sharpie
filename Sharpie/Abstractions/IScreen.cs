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
    ///     Creates a new pad.
    /// </summary>
    /// <param name="size">The pad size.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="size" /> is invalid.</exception>
    IPad Pad(Size size);
    
    /// <summary>
    ///     Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    void ApplyPendingRefreshes();

    /// <summary>
    ///     This method invalidates the screen in its entirety and redraws if from scratch.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Screen or some children are no longer usable.</exception>
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