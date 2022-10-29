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
///     Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window: IDisposable
{
    private IntPtr _handle;
    private IList<Window> _windows = new List<Window>();

    /// <summary>
    ///     Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="parent">The parent window (if any).</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Window(ICursesProvider curses, Window? parent, IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            throw new ArgumentException("The window handle has an invalid value.");
        }

        Curses = curses ?? throw new ArgumentNullException(nameof(curses));
        _handle = windowHandle;
        Parent = parent;

        parent?._windows.Add(this);

        EnableScrolling = true;

        Curses.keypad(Handle, true)
              .Check(nameof(Curses.keypad), "Failed to enable the keypad resolution mode.");

        Curses.notimeout(Handle, false)
              .Check(nameof(Curses.notimeout), "Failed to enable no-read-timeout mode.");

        Curses.nodelay(Handle, false)
              .Check(nameof(Curses.nodelay), "Failed to enable read-delay mode.");

        Curses.syncok(Handle, true)
              .Check(nameof(Curses.syncok), "Failed to enable auto-sync mode.");
    }

    /// <summary>
    ///     The curses backend.
    /// </summary>
    protected internal ICursesProvider Curses { get; }

    /// <summary>
    ///     The parent of this window.
    /// </summary>
    protected internal Window? Parent { get; }

    /// <summary>
    ///     The Curses handle for the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public IntPtr Handle
    {
        get
        {
            AssertAlive();
            return _handle;
        }
    }

    /// <summary>
    ///     Lists the active windows in this terminal.
    /// </summary>
    public IEnumerable<Window> Children => _windows;

    /// <summary>
    ///     Gets or sets the ability of the window to scroll its contents when writing
    ///     needs a new line.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableScrolling
    {
        get => Curses.is_scrollok(Handle);
        set =>
            Curses.scrollok(Handle, value)
                  .Check(nameof(Curses.scrollok), "Failed to change the scrolling mode.");
    }

    /// <summary>
    ///     Checks if the window is not disposed.
    /// </summary>
    public bool Disposed => _handle == IntPtr.Zero;

    /// <summary>
    ///     Enables or disables the use of hardware line insert/delete handling fpr this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Terminal.HasHardwareLineEditor" />
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public bool UseHardwareLineEdit
    {
        get => Curses.is_idlok(Handle);
        set
        {
            if (Curses.has_il())
            {
                Curses.idlok(Handle, value)
                      .Check(nameof(Curses.idlok), "Failed to change the hardware line mode.");
            }
        }
    }

    /// <summary>
    ///     Enables or disables the use of hardware character insert/delete handling for this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Terminal.HasHardwareCharEditor" />
    ///     Default is <c>true</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public bool UseHardwareCharEdit
    {
        get => Curses.is_idcok(Handle);
        set
        {
            if (Curses.has_ic())
            {
                Curses.idcok(Handle, value);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Style Style
    {
        get
        {
            Curses.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero)
                  .Check(nameof(Curses.wattr_get), "Failed to get the window style.");

            return new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } };
        }
        set =>
            Curses.wattr_set(Handle, (uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                  .Check(nameof(Curses.wattr_set), "Failed to set the window style.");
    }

    /// <summary>
    ///     Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set =>
            Curses.wcolor_set(Handle, value.Handle, IntPtr.Zero)
                  .Check(nameof(Curses.wcolor_set), "Failed to set the window color mixture.");
    }

    /// <summary>
    ///     Gets or sets the window background.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (Rune @char, Style style) Background
    {
        get
        {
            Curses.wgetbkgrnd(Handle, out var @char)
                  .Check(nameof(Curses.wgetbkgrnd), "Failed to get the window background.");

            return Curses.FromComplexChar(@char);
        }
        set =>
            Curses.wbkgrnd(Handle, Curses.ToComplexChar(value.@char, value.style))
                  .Check(nameof(Curses.wbkgrnd), "Failed to set the window background.");
    }

    /// <summary>
    ///     Gets or sets the location of the window within its parent.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the parent's bounds.</exception>
    public virtual Point Location
    {
        get
        {
            if (Curses.is_subwin(Handle))
            {
                return new(Curses.getparx(Handle)
                                 .Check(nameof(Curses.getparx), "Failed to get window X coordinate."), Curses
                    .getpary(Handle)
                    .Check(nameof(Curses.getpary), "Failed to get window Y coordinate."));
            }

            return new(Curses.getbegx(Handle)
                             .Check(nameof(Curses.getbegx), "Failed to get window X coordinate."), Curses
                .getbegy(Handle)
                .Check(nameof(Curses.getbegy), "Failed to get window Y coordinate."));
        }
        set
        {
            if (Parent != null)
            {
                var size = Size;
                var newRect = new Rectangle(value.X, value.Y, value.X + size.Width - 1, value.Y + size.Height - 1);
                if (!Parent.IsRectangleWithin(newRect))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            if (Curses.is_subwin(Handle))
            {
                Curses.mvderwin(Handle, value.Y, value.X)
                      .Check(nameof(Curses.mvderwin), "Failed to move window to new coordinates.");
            } else
            {
                Curses.mvwin(Handle, value.Y, value.X)
                      .Check(nameof(Curses.mvwin), "Failed to move window to new coordinates.");
            }
        }
    }

    /// <summary>
    ///     Gets or sets the size of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the bounds.</exception>
    public virtual Size Size
    {
        get =>
            new(Curses.getmaxx(Handle)
                      .Check(nameof(Curses.getmaxx), "Failed to get window width."), Curses.getmaxy(Handle)
                .Check(nameof(Curses.getmaxy), "Failed to get window height."));
        set
        {
            if (Parent != null)
            {
                var loc = Location;
                var newRect = new Rectangle(loc.X, loc.Y, loc.X + value.Width - 1, loc.Y + value.Height - 1);
                if (!Parent.IsRectangleWithin(newRect))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            Curses.wresize(Handle, value.Height, value.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the window.");
        }
    }

    /// <summary>
    ///     Gets or sets the current position of the caret within the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the window bounds.</exception>
    public Point CaretPosition
    {
        get =>
            new(Curses.getcurx(Handle)
                      .Check(nameof(Curses.getcurx), "Failed to get caret X position."), Curses.getcury(Handle)
                .Check(nameof(Curses.getcury), "Failed to get caret Y position."));
        set
        {
            if (!IsPointWithin(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wmove(Handle, value.Y, value.X)
                  .Check(nameof(Curses.wmove), "Failed to move the caret.");
        }
    }

    /// <summary>
    ///     Specifies whether the window has some "dirty" parts that need to be synchronized
    ///     to the console.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public bool IsDirty => Curses.is_wintouched(Handle);

    /// <summary>
    ///     Set or get the immediate refresh capability of the window.
    /// </summary>
    /// <remarks>
    ///     Immediate refresh will redraw the window on each change.
    ///     This might be very slow for most use cases.
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public virtual bool ImmediateRefresh
    {
        get => Curses.is_immedok(Handle);
        set => Curses.immedok(Handle, value);
    }

    /// <summary>
    ///     Gets or sets the window's management of the hardware cursor. This is an internal property and should not
    ///     be called by developers.
    /// </summary>
    internal bool IgnoreHardwareCaret
    {
        get => Curses.is_leaveok(Handle);
        set
        {
            foreach (var window in _windows)
            {
                window.IgnoreHardwareCaret = value;
            }

            Curses.leaveok(Handle, value);
        }
    }

    /// <summary>
    ///     Disposes the current instance.
    /// </summary>
    public void Dispose()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Asserts that the window is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    protected internal void AssertAlive()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException("The window has been disposed and is no longer usable.");
        }
    }

    /// <summary>
    ///     Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        Curses.wattr_on(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_on), "Failed to enable window attributes.");
    }

    /// <summary>
    ///     Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        Curses.wattr_off(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_off), "Failed to disable window attributes.");
    }

    /// <summary>
    ///     Tries to move the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    public bool TryMoveCaretTo(int x, int y) =>
        IsPointWithin(new(x, y)) && !Curses.wmove(Handle, y, x)
                                           .Failed();

    /// <summary>
    ///     Moves the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="ArgumentException">The given coordinates are outside the window.</exception>
    public void MoveCaretTo(int x, int y)
    {
        if (!TryMoveCaretTo(x, y))
        {
            throw new ArgumentException("The coordinates are outside the window.");
        }
    }

    /// <summary>
    ///     Scrolls the contents of the window <paramref name="lines" /> up. Only works for scrollable windows.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="NotSupportedException">The <see cref="EnableScrolling" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ScrollUp(int lines)
    {
        if (lines <= 0 || lines > Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        if (!EnableScrolling)
        {
            throw new NotSupportedException("The window is not scroll-enabled.");
        }

        Curses.wscrl(Handle, lines)
              .Check(nameof(Curses.wscrl), "Failed to scroll the contents of the window up.");
    }

    /// <summary>
    ///     Scrolls the contents of the window <paramref name="lines" /> down. Only works for scrollable windows.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="NotSupportedException">The <see cref="EnableScrolling" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ScrollDown(int lines)
    {
        if (lines <= 0 || lines > Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        if (!EnableScrolling)
        {
            throw new NotSupportedException("The window is not scroll-enabled.");
        }

        Curses.wscrl(Handle, -lines)
              .Check(nameof(Curses.wscrl), "Failed to scroll the contents of the window down.");
    }

    /// <summary>
    ///     Inserts <paramref name="lines" /> empty lines at the current caret position.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void InsertEmptyLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Curses.winsdelln(Handle, lines)
              .Check(nameof(Curses.winsdelln), "Failed to insert blank lines into the window.");
    }

    /// <summary>
    ///     Deletes <paramref name="lines" /> lines starting with the current caret position. All lines below move up.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DeleteLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Curses.winsdelln(Handle, -lines)
              .Check(nameof(Curses.winsdelln), "Failed to delete lines from the window.");
    }

    /// <summary>
    ///     Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="width">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="ArgumentException">The <paramref name="width" /> is less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ChangeTextStyle(int width, Style style)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The length should be greater than zero.");
        }

        Curses.wchgat(Handle, width, (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(Curses.wchgat), "Failed to change style of characters in the window.");
    }

    /// <summary>
    ///     Writes a text at the caret position at the current window and optionally, advance the caret.
    /// </summary>
    /// <param name="str">The text to write.</param>
    /// <param name="style">The style of the text.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="str" /> is <c>null</c>.</exception>
    public void WriteText(string str, Style style)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        foreach (var rune in str.EnumerateRunes())
        {
            Curses.wadd_wch(Handle, Curses.ToComplexChar(rune, style));
        }
    }

    /// <summary>
    ///     Draws a vertical line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="style">The style to use.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawVerticalLine(Rune @char, Style style, int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.wvline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.wvline_set), "Failed to draw a vertical line.");
    }

    /// <summary>
    ///     Draws a vertical line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawVerticalLine(int length)
    {
        Curses.wvline(Handle, 0, length)
              .Check(nameof(Curses.wvline), "Failed to draw a vertical line.");
    }

    /// <summary>
    ///     Draws a vertical line from the current caret position downwards.
    /// </summary>
    /// <param name="bottomRightCornerChar">The bottom-right corner character.</param>
    /// <param name="leftSideChar">The left-side character.</param>
    /// <param name="rightSideChar">The right-side character.</param>
    /// <param name="topLeftCornerChar">The top-left corner character.</param>
    /// <param name="topRightCornerChar">The top-right corner character.</param>
    /// <param name="topSideChar">The top-side character.</param>
    /// <param name="bottomLeftCornerChar">The bottom-left corner character.</param>
    /// <param name="bottomSideChar">The bottom-side character.</param>
    /// <param name="style">The style to use.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DrawBorder(Rune leftSideChar, Rune rightSideChar, Rune topSideChar, Rune bottomSideChar,
        Rune topLeftCornerChar, Rune topRightCornerChar, Rune bottomLeftCornerChar, Rune bottomRightCornerChar,
        Style style)
    {
        var leftSide = Curses.ToComplexChar(leftSideChar, style);
        var rightSide = Curses.ToComplexChar(rightSideChar, style);
        var topSide = Curses.ToComplexChar(topSideChar, style);
        var bottomSide = Curses.ToComplexChar(bottomSideChar, style);
        var topLeftCorner = Curses.ToComplexChar(topLeftCornerChar, style);
        var topRightCorner = Curses.ToComplexChar(topRightCornerChar, style);
        var bottomLeftCorner = Curses.ToComplexChar(bottomLeftCornerChar, style);
        var bottomRightCorner = Curses.ToComplexChar(bottomRightCornerChar, style);

        Curses.wborder_set(Handle, leftSide, rightSide, topSide, bottomSide,
                  topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner)
              .Check(nameof(Curses.wborder_set), "Failed to draw a window border.");
    }

    /// <summary>
    ///     Draws a border around the window's edges using standard characters.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DrawBorder()
    {
        Curses.wborder(Handle, 0, 0, 0, 0,
                  0, 0, 0, 0)
              .Check(nameof(Curses.wborder), "Failed to draw a window border.");
    }

    /// <summary>
    ///     Draws a horizontal line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="style">The style to use.</param>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawHorizontalLine(Rune @char, Style style, int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.whline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.whline_set), "Failed to draw a horizontal line.");
    }

    /// <summary>
    ///     Draws a horizontal line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawHorizontalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.whline(Handle, 0, length)
              .Check(nameof(Curses.whline), "Failed to draw a horizontal line.");
    }

    /// <summary>
    ///     Removes the text under the caret and moves the contents of the line to the left.
    /// </summary>
    /// <param name="count">The number of characters to remove.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    public void RemoveText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        while (count > 0)
        {
            if (Curses.wdelch(Handle)
                      .Failed())
            {
                break;
            }

            count--;
        }
    }

    /// <summary>
    ///     Gets the text from the window at the caret position to the right.
    /// </summary>
    /// <param name="count">The number of characters to get.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    public (Rune @char, Style style)[] GetText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        count = Math.Min(count, Size.Width - CaretPosition.X);
        var arr = new ComplexChar[count];

        Curses.win_wchnstr(Handle, arr, count)
              .Check(nameof(Curses.win_wchnstr), "Failed to get the text from the window.");

        return arr.Select(ch => Curses.FromComplexChar(ch))
                  .ToArray();
    }

    /// <summary>
    ///     Clears the contents of the row/window.
    /// </summary>
    /// <param name="strategy">The strategy to use.</param>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Clear(ClearStrategy strategy)
    {
        switch (strategy)
        {
            case ClearStrategy.Full:
                Curses.werase(Handle)
                      .Check(nameof(Curses.werase), "Failed to queue a window erase.");

                break;
            case ClearStrategy.LineFromCaret:
                if (CaretPosition.X < Size.Width - 1)
                {
                    Curses.wclrtoeol(Handle)
                          .Check(nameof(Curses.wclrtoeol), "Failed to clear the line from the caret.");
                }

                break;
            case ClearStrategy.FullFromCaret:
                Curses.wclrtobot(Handle)
                      .Check(nameof(Curses.wclrtobot), "Failed to clear the window from the caret.");

                break;
        }
    }

    /// <summary>
    ///     Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is null.</exception>
    public void Replace(Window window, ReplaceStrategy strategy)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        switch (strategy)
        {
            case ReplaceStrategy.Overlay:
                Curses.overlay(Handle, window.Handle)
                      .Check(nameof(Curses.overlay), "Failed to overlay window.");

                break;
            case ReplaceStrategy.Overwrite:
                Curses.overwrite(Handle, window.Handle)
                      .Check(nameof(Curses.overwrite), "Failed to overwrite window.");

                break;
        }
    }

    /// <summary>
    ///     Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <param name="srcRect">The source rectangle to copy.</param>
    /// <param name="destPos">The destination position.</param>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is null.</exception>
    public void Replace(Window window, Rectangle srcRect, Point destPos, ReplaceStrategy strategy)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        var destRect = new Rectangle(destPos, new(srcRect.Bottom - srcRect.Top, srcRect.Right - srcRect.Left));
        if (!window.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(srcRect));
        }

        Curses.copywin(Handle, window.Handle, srcRect.Top, srcRect.Left, destRect.Top,
                  destRect.Left, destRect.Bottom, destRect.Right, Convert.ToInt32(strategy == ReplaceStrategy.Overlay))
              .Check(nameof(Curses.copywin), "Failed to copy the window contents.");
    }

    /// <summary>
    ///     Invalidates a number of lines within the window.
    /// </summary>
    /// <param name="line">The window to copy contents to.</param>
    /// <param name="count">The used strategy.</param>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="line" /> and <paramref name="count" /> combination is
    ///     out of bounds.
    /// </exception>
    public void Invalidate(int line, int count)
    {
        if (line < 0 || line >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        if (count <= 0 || line + count >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Curses.wtouchln(Handle, line, count, 1)
              .Check(nameof(Curses.wtouchln), "Failed to mark lines as dirty.");
    }

    /// <summary>
    ///     Invalidates the contents of the window thus forcing a redraw at the next <see cref="Refresh" />.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Invalidate() { Invalidate(0, Size.Height); }

    /// <summary>
    ///     Checks if a given point fits within the current window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>The result of the check.</returns>
    public bool IsPointWithin(Point point) => Curses.wenclose(Handle, point.Y, point.X);

    /// <summary>
    ///     Checks if a given rectangle fits within the current window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>The result of the check.</returns>
    public bool IsRectangleWithin(Rectangle rect) =>
        IsPointWithin(new(rect.Left, rect.Top)) && IsPointWithin(new(rect.Right - 1, rect.Bottom - 1));

    /// <summary>
    ///     Checks if the row at <paramref name="y" /> is dirty.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current window has been disposed and is no longer usable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="y" /> is outside the bounds.</exception>
    public bool IsRowDirty(int y)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Curses.is_linetouched(Handle, y);
    }

    /// <summary>
    ///     Refreshes the window by synchronizing it to the terminal.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <param name="entireScreen">If <c>true</c>, when this refresh happens, the entire screen is redrawn.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public virtual void Refresh(bool batch, bool entireScreen)
    {
        Curses.clearok(Handle, entireScreen)
              .Check(nameof(Curses.clearok), "Failed to configure the refresh mode.");

        if (batch)
        {
            Curses.wnoutrefresh(Handle)
                  .Check(nameof(Curses.wnoutrefresh), "Failed to queue window refresh.");
        } else
        {
            Curses.wrefresh(Handle)
                  .Check(nameof(Curses.wrefresh), "Failed to perform window refresh.");
        }
    }

    /// <summary>
    ///     Refreshes a number of lines within the window.
    /// </summary>
    /// <param name="line">The starting line to refresh.</param>
    /// <param name="count">The number of lines to refresh.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The combination of lines and count exceed the window boundary.</exception>
    public virtual void RefreshLines(int line, int count)
    {
        if (line < 0 || line >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        if (count < 1 || line + count - 1 >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Curses.wredrawln(Handle, line, count)
              .Check(nameof(Curses.wredrawln), "Failed to perform line refresh.");
    }

    /// <summary>
    ///     Removes the window form the parent, destroys all children and itself.
    /// </summary>
    public void Destroy()
    {
        if (!Disposed)
        {
            // Dispose of all the windows
            var windows = _windows.ToArray();
            foreach (var window in windows)
            {
                window.Destroy();
            }

            Parent?._windows.Remove(this);

            Delete();

            _handle = IntPtr.Zero;
        }
    }

    /// <summary>
    ///     Deletes the window from the curses backend.
    /// </summary>
    protected virtual void Delete()
    {
        Debug.Assert(_handle != IntPtr.Zero);
        Curses.delwin(_handle);
    }

    /// <summary>
    ///     The destructor. Calls <see cref="Destroy" />.
    /// </summary>
    ~Window() { Destroy(); }
}
