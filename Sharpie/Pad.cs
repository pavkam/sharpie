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
///     Represents pad which is a special type of window.
/// </summary>
[PublicAPI]
public sealed class Pad: Window
{
    /// <summary>
    ///     Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="terminal">The curses functionality provider.</param>
    /// <param name="parent">The parent window or pad.</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Pad(Terminal terminal, Window parent, IntPtr windowHandle): base(terminal, parent, windowHandle) { }

    /// <inheritdoc cref="Window.ImmediateRefresh" />
    /// <remarks>
    ///     This functionality is disabled in the pads. Any call to set the value to <c>true</c> will
    ///     fail with error.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws on write.</exception>
    public override bool ImmediateRefresh
    {
        get => base.ImmediateRefresh;
        set => throw new NotSupportedException("Pads cannot have immediate refresh enabled.");
    }

    /// <inheritdoc cref="Window.Refresh" />
    /// <remarks>
    ///     This functionality is disabled in the pads. Use the overloaded version of this method.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws.</exception>
    public override void Refresh(bool batch, bool entireScreen)
    {
        throw new NotSupportedException("Pads cannot be refreshed in this way.");
    }

    /// <summary>
    ///     Refreshes the pad by synchronizing it to the terminal screen.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <param name="entireScreen">If <c>true</c>, when this refresh happens, the entire screen is redrawn.</param>
    /// <param name="rect">The rectangle of the pad to place onto the screen.</param>
    /// <param name="screenPos">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Refresh(bool batch, bool entireScreen, Rectangle rect, Point screenPos)
    {
        if (!IsRectangleWithin(rect))
        {
            throw new ArgumentOutOfRangeException(nameof(rect));
        }

        var destRect = new Rectangle(screenPos, new(rect.Bottom - rect.Top, rect.Right - rect.Left));
        if (!Terminal.Screen.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(screenPos));
        }

        Terminal.Curses.clearok(Handle, entireScreen)
                .Check(nameof(Terminal.Curses.clearok), "Failed to configure pad refresh.");

        if (batch)
        {
            Terminal.Curses.pnoutrefresh(Handle, rect.Top, rect.Left, destRect.Top, destRect.Left,
                        destRect.Bottom, destRect.Right)
                    .Check(nameof(Terminal.Curses.pnoutrefresh), "Failed to queue pad refresh.");
        } else
        {
            Terminal.Curses.prefresh(Handle, rect.Top, rect.Left, destRect.Top, destRect.Left,
                        destRect.Bottom, destRect.Right)
                    .Check(nameof(Terminal.Curses.prefresh), "Failed to perform pad refresh.");
        }
    }
}
