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
///     Defines the traits implemented by the <see cref="Window" /> class.
/// </summary>
[PublicAPI]
public interface IWindow: ITerminalSurface
{
    /// <summary>
    ///     The parent screen of this window.
    /// </summary>
    IScreen Screen { get; }

    /// <summary>
    ///     Lists of children of this object.
    /// </summary>
    IEnumerable<ISubWindow> SubWindows { get; }

    /// <summary>
    ///     Enables or disables the use of hardware line insert/delete handling fpr this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Terminal.HasHardwareLineEditor" />
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool UseHardwareLineEdit { get; set; }

    /// <summary>
    ///     Enables or disables the use of hardware character insert/delete handling for this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Terminal.HasHardwareCharEditor" />
    ///     Default is <c>true</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool UseHardwareCharEdit { get; set; }

    /// <summary>
    ///     Gets or sets the value indicating if the window is visible.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool Visible { get; set; }
    
    /// <summary>
    ///     Gets or sets the location of the window within its parent.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the parent's bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    Point Location { get; set; }

    /// <summary>
    ///     Gets or sets the size of the window.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    new Size Size { get; set; }

    /// <summary>
    ///     Send the window to the back of the Z-order.
    /// </summary>
    void SendToBack();

    /// <summary>
    ///     Send the window to the front of the Z-order.
    /// </summary>
    void BringToFront();

    /// <summary>
    ///     Creates a new sub-window in the parent window.
    /// </summary>
    /// <param name="area">The area of the window to put the sub-window in.</param>
    /// <remarks>
    /// </remarks>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the bounds of the parent.</exception>
    ISubWindow SubWindow(Rectangle area);

    /// <summary>
    ///     Duplicates and existing window, including its attributes.
    /// </summary>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">Screen is no longer usable.</exception>
    IWindow Duplicate();
}
