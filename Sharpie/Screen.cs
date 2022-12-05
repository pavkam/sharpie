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
public sealed class Screen: Window, IScreen
{
    /// <summary>
    ///     Initializes the screen using a window handle. The <paramref name="windowHandle" /> should be
    ///     a screen and not a regular window.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="terminal">The owner terminal.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(ICursesProvider curses, ITerminal terminal, IntPtr windowHandle): base(curses, null, windowHandle) =>
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

    /// <inheritdoc cref="IScreen.Terminal"/>
    public ITerminal Terminal { get; }

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

    /// <inheritdoc cref="IScreen.CreateWindow"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow CreateWindow(Rectangle area)
    {
        if (!((IWindow)this).IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.newwin(area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Curses.newwin), "Failed to create a new window.");

        return new Window(Curses, this, handle);
    }

    /// <inheritdoc cref="IScreen.CreateSubWindow"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow CreateSubWindow(IWindow window, Rectangle area)
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
                           .Check(nameof(Curses.derwin), "Failed to create a new sub-window.");

        return new Window(Curses, window, handle);
    }

    /// <inheritdoc cref="IScreen.DuplicateWindow"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow DuplicateWindow(IWindow window)
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
                                   .Check(nameof(Curses.dupwin), "Failed to duplicate an existing window.");

                return window is Pad
                    ? new Pad(Curses, window.Parent ?? this, handle)
                    : new Window(Curses, window.Parent, handle);
            }
        }
    }

    /// <inheritdoc cref="IScreen.CreatePad"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IPad CreatePad(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        AssertAlive();
        var handle = Curses.newpad(size.Height, size.Width)
                           .Check(nameof(Curses.newpad), "Failed to create a new pad.");

        return new Pad(Curses, this, handle);
    }

    /// <inheritdoc cref="IScreen.CreateSubPad"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IPad CreateSubPad(IPad pad, Rectangle area)
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
                           .Check(nameof(Curses.subpad), "Failed to create a new sub-pad.");

        return new Pad(Curses, pad, handle);
    }

    /// <inheritdoc cref="IScreen.ApplyPendingRefreshes"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        AssertAlive();
        Curses.doupdate()
              .Check(nameof(Curses.doupdate), "Failed to update the main screen.");
    }

    /// <inheritdoc cref="Window.Delete"/>
    /// <summary>
    ///     Deletes the screen window and ends the terminal session.
    /// </summary>
    protected override void Delete() { Curses.endwin(); }
}
