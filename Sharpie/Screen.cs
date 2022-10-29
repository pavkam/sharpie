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

using Curses;

/// <summary>
///     The core curses screen class. Implements screen-related functionality.
/// </summary>
[PublicAPI]
public sealed class Screen: Window
{
    /// <summary>
    ///     Initializes the screen using a window handle. The <paramref name="windowHandle" /> should be
    ///     a screen and not a regular window.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(ICursesProvider curses, IntPtr windowHandle): base(curses, null, windowHandle) { }

    /// <inheritdoc cref="Window.Location" />
    /// <remarks>
    ///     The setter will always throw in this implementation as moving the main window is not allowed.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateWindow(Rectangle area)
    {
        if (!IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        AssertAlive();
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
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area" /> is outside the bounds of the parent.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateSubWindow(Window window, Rectangle area)
    {
        if (!window.IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        AssertAlive();
        var handle = Curses.derwin(window.Handle, area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Terminal.Curses.derwin), "Failed to create a new sub-window.");

        return new(Curses, this, handle);
    }

    /// <summary>
    ///     Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="InvalidOperationException">Trying to duplicate the screen window.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window DuplicateWindow(Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        if (window == this)
        {
            throw new InvalidOperationException("Cannot duplicate the screen window.");
        }

        return new(Curses, this, Curses.dupwin(window.Handle)
                                       .Check(nameof(Terminal.Curses.dupwin),
                                           "Failed to duplicate an existing window."));
    }

    /// <summary>
    ///     Created a new pad.
    /// </summary>
    /// <param name="size">The pad size.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="size" /> is invalid.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
    /// <exception cref="ObjectDisposedException">The terminal or the pad have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="area" /> is outside the pad's bounds.</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="pad" /> is <c>null</c>.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
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
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        AssertAlive();
        Curses.doupdate()
              .Check(nameof(Terminal.Curses.doupdate), "Failed to update the main screen.");
    }
    
    /// <summary>
    ///     Tries to read an event from the terminal.
    /// </summary>
    /// <param name="timeoutMillis">The timeout to wait for the event.</param>
    /// <param name="event">The event that was read.</param>
    /// <returns><c>true</c> if there was an event; <c>false</c> if the timeout expired.</returns>
    public bool TryReadEvent(int timeoutMillis, [NotNullWhen(true)] out Event? @event)
    {
        /*

        Curses.define_key("\x001bO2C", 1024); //shift right
        Curses.define_key("\x001bO2B", 1024); //shift down
        Curses.define_key("\x001bO2A", 1024); //shift up
        Curses.define_key("\x001bO2D", 1024); //shift left
        Curses.define_key("\x001bO2H", 1024); //shift home
        Curses.define_key("\x001bO2F", 1024); //shift end of line

        Curses.define_key("\x001b06C", 1024); //shift ctrl right
        Curses.define_key("\x001b06B", 1024); //shift ctrl down
        Curses.define_key("\x001b06A", 1024); //shift ctrl up
        Curses.define_key("\x001b06D", 1024); //shift ctrl left
        Curses.define_key("\x001b06H", 1024); //shift ctrl home
        Curses.define_key("\x001b06F", 1024); //shift ctrl end of line

        Curses.define_key("\x001bb", 1024); //alt right
        Curses.define_key("\x001bf", 1024); //alt left

        Curses.define_key("\x001b" + (char)258, 1024); //alt up

         */
        Curses.wtimeout(Handle, timeoutMillis);

        @event = null;
        var result = Curses.wget_wch(Handle, out var keyCode);
        if (result == (uint) RawKey.Yes)
        {
            switch (keyCode)
            {
                case (uint) RawKey.Resize:
                    @event = new TerminalResizeEvent(Size);
                    break;
                case (uint) RawKey.Mouse:
                    if (Curses.getmouse(out var mouseEvent)
                              .Failed())
                    {
                        return false;
                    }

                    var (button, state, mouseMod) = Helpers.ConvertMouseActionEvent((RawMouseEvent.EventType) mouseEvent.buttonState);
                    if (button == 0)
                    {
                        @event = new MouseMoveEvent(new(mouseEvent.x, mouseEvent.y));
                    } else
                    {
                        @event = new MouseActionEvent(new(mouseEvent.x, mouseEvent.y), button, state, mouseMod);
                    }

                    break;
                default:
                    var (key, keyMod) = Helpers.ConvertKeyPressEvent(keyCode);
                    @event = new KeyEvent(key, new('\0'), Curses.key_name(keyCode), keyMod);
                    break;
            }

            return true;
        }

        if (!result.Failed())
        {
            if (keyCode == 3)
            {
                @event = new KeyEvent(Key.Interrupt, new('\0'), null, ModifierKey.None);
            } else
            {
                var keyName = Curses.key_name(keyCode);
                @event = new KeyEvent(Key.Character, new(keyCode), keyName, ModifierKey.None);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Deletes the screen window and ends the terminal session.
    /// </summary>
    protected override void Delete() { Curses.endwin(); }
}
