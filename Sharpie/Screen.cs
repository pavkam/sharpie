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
///     The main Curses screen class. Implements screen-related functionality.
///     Only one instance of this class can be active at one time.
/// </summary>
[PublicAPI]
public sealed class Screen: Window
{
    /// <summary>
    ///     Initializes the screen using a window handle. The <paramref name="windowHandle" /> should be
    ///     a screen and not a regular window.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="terminal">The owner terminal.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(ICursesProvider curses, Terminal terminal, IntPtr windowHandle): base(curses, null, windowHandle) =>
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

    /// <summary>
    ///     The terminal this screen belongs to.
    /// </summary>
    public Terminal Terminal { get; }

    /// <inheritdoc cref="Window.Location" />
    /// <remarks>
    ///     The setter will always throw in this implementation as moving the main window is not allowed.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws on the setter.</exception>
    public override Point Location
    {
        get => base.Location;
        set => throw new NotSupportedException("Cannot move the screen window.");
    }

    /// <inheritdoc cref="Window.Location" />
    /// <remarks>
    ///     The setter will always throw in this implementation changing the size of the main window is not allowed.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws on the setter.</exception>
    public override Size Size
    {
        get => base.Size;
        set => throw new NotSupportedException("Cannot resize the screen window.");
    }

    /// <summary>
    ///     Created a new window in the screen.
    /// </summary>
    /// <param name="area">The area for the new window.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the screen bounds.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Window CreateWindow(Rectangle area)
    {
        if (!IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.newwin(area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Terminal.Curses.newwin), "Failed to create a new window.");

        return new(Curses, this, handle);
    }

    /// <summary>
    ///     Created a new sub-window in the parent window.
    /// </summary>
    /// <param name="window">The parent window.</param>
    /// <param name="area">The area of the window to put the sub-window in.</param>
    /// <remarks>
    /// </remarks>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The window has been disposed and can no longer be used.</exception>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="window" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the bounds of the parent.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Window CreateSubWindow(Window window, Rectangle area)
    {
        switch (window)
        {
            case null:
                throw new ArgumentNullException(nameof(window));
            case Screen:
                return CreateWindow(area);
        }

        if (!window.IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        if (window is Pad)
        {
            throw new InvalidOperationException("Cannot create a sub-window in a pad.");
        }

        var handle = Curses.derwin(window.Handle, area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Terminal.Curses.derwin), "Failed to create a new sub-window.");

        return new(Curses, window, handle);
    }

    /// <summary>
    ///     Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The window has been disposed and can no longer be used.</exception>
    /// <exception cref="InvalidOperationException">Trying to duplicate the screen window.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="window" /> is <c>null</c>.</exception>
    public Window DuplicateWindow(Window window)
    {
        switch (window)
        {
            case null:
                throw new ArgumentNullException(nameof(window));
            case Screen:
                throw new InvalidOperationException("Cannot duplicate the screen window.");
            default:
            {
                var handle = Curses.dupwin(window.Handle)
                                   .Check(nameof(Terminal.Curses.dupwin), "Failed to duplicate an existing window.");

                return window is Pad
                    ? new Pad(Curses, window.Parent ?? this, handle)
                    : new Window(Curses, window.Parent, handle);
            }
        }
    }

    /// <summary>
    ///     Created a new pad.
    /// </summary>
    /// <param name="size">The pad size.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="size" /> is invalid.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Pad CreatePad(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        AssertAlive();
        var handle = Curses.newpad(size.Height, size.Width)
                           .Check(nameof(Terminal.Curses.newpad), "Failed to create a new pad.");

        return new(Curses, this, handle);
    }

    /// <summary>
    ///     Created a new sub-pad.
    /// </summary>
    /// <param name="pad">The parent pad.</param>
    /// <param name="area">The are of the pad to use.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The pad have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="area" /> is outside the pad's bounds.</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="pad" /> is <c>null</c>.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Pad CreateSubPad(Pad pad, Rectangle area)
    {
        if (pad == null)
        {
            throw new ArgumentNullException(nameof(pad));
        }

        if (!pad.IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.subpad(pad.Handle, area.Height, area.Width, area.Top, area.Right)
                           .Check(nameof(Terminal.Curses.subpad), "Failed to create a new sub-pad.");

        return new(Curses, pad, handle);
    }

    /// <summary>
    ///     Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The screen has been disposed and can no longer be used.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        AssertAlive();
        Curses.doupdate()
              .Check(nameof(Terminal.Curses.doupdate), "Failed to update the main screen.");
    }

    /// <summary>
    ///     This method invalidates the screen in its entirety and redraws if from scratch.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The screen has been disposed and can no longer be used.</exception>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ForceInvalidateAndRefresh()
    {
        Invalidate();
        foreach (var child in Children)
        {
            child.Invalidate();
        }

        Refresh(false, true);
    }

    /// <summary>
    ///     Deletes the screen window and ends the terminal session.
    /// </summary>
    protected override void Delete() { Curses.endwin(); }
}
