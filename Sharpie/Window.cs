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

using System.Globalization;
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
    /// <param name="terminal">The curses functionality provider.</param>
    /// <param name="parent">The parent window (if any).</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Window(Terminal terminal, Window? parent, IntPtr windowHandle)
    {
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        _handle = windowHandle;
        Parent = parent;

        parent?._windows.Add(this);

        EnableScrolling = true;

        Terminal.Curses.keypad(Handle, true)
                .Check(nameof(Terminal.Curses.keypad), "Failed to enable the keypad resolution mode.");

        Terminal.Curses.notimeout(Handle, false)
                .Check(nameof(Terminal.Curses.notimeout), "Failed to enable no-read-timeout mode.");

        Terminal.Curses.nodelay(Handle, false)
                .Check(nameof(Terminal.Curses.nodelay), "Failed to enable read-delay mode.");

        Terminal.Curses.syncok(Handle, true)
                .Check(nameof(Terminal.Curses.syncok), "Failed to enable auto-sync mode.");
    }

    /// <summary>
    ///     The terminal to which this window belongs.
    /// </summary>
    protected Terminal Terminal { get; }

    /// <summary>
    ///     The parent of this window.
    /// </summary>
    protected Window? Parent { get; }

    /// <summary>
    ///     The Curses handle for the window.
    /// </summary>
    public IntPtr Handle
    {
        get
        {
            if (_handle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("The window has been disposed and is no longer usable.");
            }

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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableScrolling
    {
        get
        {
            Terminal.AssertNotDisposed();
            return Terminal.Curses.is_scrollok(Handle);
        }
        set
        {
            Terminal.AssertNotDisposed();
            Terminal.Curses.scrollok(Handle, value)
                    .Check(nameof(Terminal.Curses.scrollok), "Failed to change the scrolling mode.");
        }
    }

    /// <summary>
    ///     Checks if the window is not disposed.
    /// </summary>
    public bool IsDisposed => Terminal.IsDisposed || Handle == IntPtr.Zero;

    /// <summary>
    ///     Enables or disables the use of hardware line insert/delete handling fpr this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Sharpie.Terminal.SupportsHardwareLineInsertAndDelete" />
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool UseHardwareLineInsertAndDelete
    {
        get => Terminal.Curses.is_idlok(Handle);
        set
        {
            if (Terminal.Curses.has_il())
            {
                Terminal.Curses.idlok(Handle, value)
                        .Check(nameof(Terminal.Curses.idlok), "Failed to change the hardware line mode.");
            }
        }
    }

    /// <summary>
    ///     Enables or disables the use of hardware character insert/delete handling for this window.
    /// </summary>
    /// <remarks>
    ///     This functionality only works if hardware has support for it. Consult
    ///     <see cref="Sharpie.Terminal.SupportsHardwareCharacterInsertAndDelete" />
    ///     Default is <c>true</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool UseHardwareCharacterInsertAndDelete
    {
        get => Terminal.Curses.is_idcok(Handle);
        set
        {
            if (Terminal.Curses.has_ic())
            {
                Terminal.Curses.idcok(Handle, value);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Style Style
    {
        get
        {
            Terminal.Curses.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero)
                    .Check(nameof(Terminal.Curses.wattr_get), "Failed to get the window style.");

            return new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } };
        }
        set =>
            Terminal.Curses.wattr_set(Handle, (uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                    .Check(nameof(Terminal.Curses.wattr_set), "Failed to set the window style.");
    }

    /// <summary>
    ///     Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set =>
            Terminal.Curses.wcolor_set(Handle, value.Handle, IntPtr.Zero)
                    .Check(nameof(Terminal.Curses.wcolor_set), "Failed to set the window color mixture.");
    }

    /// <summary>
    ///     Gets or sets the window background.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (Rune @char, Style style) Background
    {
        get
        {
            Terminal.Curses.wgetbkgrnd(Handle, out var @char)
                    .Check(nameof(Terminal.Curses.wgetbkgrnd), "Failed to get the window background.");

            return BreakComplexChar(@char);
        }
        set =>
            Terminal.Curses.wbkgrnd(Handle, MakeComplexChar(value.@char, value.style))
                    .Check(nameof(Terminal.Curses.wbkgrnd), "Failed to set the window background.");
    }

    /// <summary>
    ///     Gets or sets the location of the window within its parent.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the parent's bounds.</exception>
    public virtual Point Location
    {
        get
        {
            if (Terminal.Curses.is_subwin(Handle))
            {
                return new(Terminal.Curses.getparx(Handle)
                                   .Check(nameof(Terminal.Curses.getparx), "Failed to get window X coordinate."),
                    Terminal.Curses.getpary(Handle)
                            .Check(nameof(Terminal.Curses.getpary), "Failed to get window Y coordinate."));
            }

            return new(Terminal.Curses.getbegx(Handle)
                               .Check(nameof(Terminal.Curses.getbegx), "Failed to get window X coordinate."), Terminal
                .Curses.getbegy(Handle)
                .Check(nameof(Terminal.Curses.getbegy), "Failed to get window Y coordinate."));
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

            if (Terminal.Curses.is_subwin(Handle))
            {
                Terminal.Curses.mvderwin(Handle, value.Y, value.X)
                        .Check(nameof(Terminal.Curses.mvderwin), "Failed to move window to new coordinates.");
            } else
            {
                Terminal.Curses.mvwin(Handle, value.Y, value.X)
                        .Check(nameof(Terminal.Curses.mvwin), "Failed to move window to new coordinates.");
            }
        }
    }

    /// <summary>
    ///     Gets or sets the size of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the bounds.</exception>
    public virtual Size Size
    {
        get =>
            new(Terminal.Curses.getmaxx(Handle)
                        .Check(nameof(Terminal.Curses.getmaxx), "Failed to get window width."), Terminal.Curses
                .getmaxy(Handle)
                .Check(nameof(Terminal.Curses.getmaxy), "Failed to get window height."));
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

            Terminal.Curses.wresize(Handle, value.Height, value.Width)
                    .Check(nameof(Terminal.Curses.wresize), "Failed to resize the window.");
        }
    }

    /// <summary>
    ///     Gets or sets the current position of the caret within the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the window bounds.</exception>
    public Point CaretPosition
    {
        get =>
            new(Terminal.Curses.getcurx(Handle)
                        .Check(nameof(Terminal.Curses.getcurx), "Failed to get caret X position."), Terminal.Curses
                .getcury(Handle)
                .Check(nameof(Terminal.Curses.getcury), "Failed to get caret Y position."));
        set
        {
            if (!IsPointWithin(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Terminal.Curses.wmove(Handle, value.Y, value.X)
                    .Check(nameof(Terminal.Curses.wmove), "Failed to move the caret.");
        }
    }

    /// <summary>
    ///     Specifies whether the window has some "dirty" parts that need to be synchronized
    ///     to the console.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool IsDirty => Terminal.Curses.is_wintouched(Handle);

    /// <summary>
    ///     Set or get the immediate refresh capability of the window.
    /// </summary>
    /// <remarks>
    ///     Immediate refresh will redraw the window on each change.
    ///     This might be very slow for most use cases.
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public virtual bool ImmediateRefresh
    {
        get => Terminal.Curses.is_immedok(Handle);
        set => Terminal.Curses.immedok(Handle, value);
    }

    /// <summary>
    ///     Gets or sets the window's management of the hardware cursor. This is an internal property and should not
    ///     be called by developers.
    /// </summary>
    internal bool IgnoreHardwareCaret
    {
        get => Terminal.Curses.is_leaveok(Handle);
        set
        {
            foreach (var window in _windows)
            {
                window.IgnoreHardwareCaret = value;
            }

            Terminal.Curses.leaveok(Handle, value);
        }
    }

    /// <summary>
    ///     Disposes the current instance.
    /// </summary>
    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }

    private ComplexChar MakeComplexChar(string element, Style style)
    {
        Debug.Assert(element != null);
        Debug.Assert(element.Length > 0);

        Terminal.Curses.setcchar(out var @char, element, (uint) style.Attributes, style.ColorMixture.Handle,
                    IntPtr.Zero)
                .Check(nameof(Terminal.Curses.setcchar), "Failed to convert string to complex character.");

        return @char;
    }

    private ComplexChar MakeComplexChar(Rune rune, Style style) => MakeComplexChar(rune.ToString(), style);

    private (Rune rune, Style style) BreakComplexChar(ComplexChar @char)
    {
        var builder = new StringBuilder(10);
        Terminal.Curses.getcchar(@char, builder, out var attrs, out var colorPair, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.getcchar), "Failed to deconstruct the complex character.");

        return (Rune.GetRuneAt(builder.ToString(), 0),
            new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } });
    }

    /// <summary>
    ///     Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        Terminal.Curses.wattr_on(Handle, (uint) attributes, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.wattr_on), "Failed to enable window attributes.");
    }

    /// <summary>
    ///     Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        Terminal.Curses.wattr_off(Handle, (uint) attributes, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.wattr_off), "Failed to disable window attributes.");
    }

    /// <summary>
    ///     Tries to move the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="x" /> or <paramref name="y" /> are negative.</exception>
    public bool TryMoveCaretTo(int x, int y)
    {
        if (x < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return !Terminal.Curses.wmove(Handle, y, x)
                        .Failed();
    }

    /// <summary>
    ///     Moves the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
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

        Terminal.Curses.wscrl(Handle, lines)
                .Check(nameof(Terminal.Curses.wscrl), "Failed to scroll the contents of the window up.");
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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
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

        Terminal.Curses.wscrl(Handle, -lines)
                .Check(nameof(Terminal.Curses.wscrl), "Failed to scroll the contents of the window down.");
    }

    /// <summary>
    ///     Inserts <paramref name="lines" /> empty lines at the current caret position.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void InsertEmptyLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Terminal.Curses.winsdelln(Handle, lines)
                .Check(nameof(Terminal.Curses.winsdelln), "Failed to insert blank lines into the window.");
    }

    /// <summary>
    ///     Deletes <paramref name="lines" /> lines starting with the current caret position. All lines below move up.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DeleteLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Terminal.Curses.winsdelln(Handle, -lines)
                .Check(nameof(Terminal.Curses.winsdelln), "Failed to delete lines from the window.");
    }

    /// <summary>
    ///     Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="width">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentException">The <paramref name="width" /> is less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ChangeTextStyle(int width, Style style)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The length should be greater than zero.");
        }

        Terminal.Curses.wchgat(Handle, width, (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.wchgat), "Failed to change style of characters in the window.");
    }

    /// <summary>
    ///     Writes a text at the caret position at the current window and optionally, advance the caret.
    /// </summary>
    /// <param name="str">The text to write.</param>
    /// <param name="style">The style of the text.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="str" /> is <c>null</c>.</exception>
    public void WriteText(string str, Style style)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var enumerator = StringInfo.GetTextElementEnumerator(str);
        while (enumerator.MoveNext())
        {
            var @char = MakeComplexChar(enumerator.GetTextElement(), style);
            if (Terminal.Curses.wadd_wch(Handle, @char)
                        .Failed())
            {
                break;
            }
        }
    }

    /// <summary>
    ///     Draws a vertical line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="style">The style to use.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawVerticalLine(Rune @char, Style style, int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Terminal.Curses.wvline_set(Handle, MakeComplexChar(@char, style), length)
                .Check(nameof(Terminal.Curses.wvline_set), "Failed to draw a vertical line.");
    }

    /// <summary>
    ///     Draws a vertical line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawVerticalLine(int length)
    {
        Terminal.Curses.wvline(Handle, 0, length)
                .Check(nameof(Terminal.Curses.wvline), "Failed to draw a vertical line.");
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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DrawBorder(Rune leftSideChar, Rune rightSideChar, Rune topSideChar, Rune bottomSideChar,
        Rune topLeftCornerChar, Rune topRightCornerChar, Rune bottomLeftCornerChar, Rune bottomRightCornerChar,
        Style style)
    {
        var leftSide = MakeComplexChar(leftSideChar, style);
        var rightSide = MakeComplexChar(rightSideChar, style);
        var topSide = MakeComplexChar(topSideChar, style);
        var bottomSide = MakeComplexChar(bottomSideChar, style);
        var topLeftCorner = MakeComplexChar(topLeftCornerChar, style);
        var topRightCorner = MakeComplexChar(topRightCornerChar, style);
        var bottomLeftCorner = MakeComplexChar(bottomLeftCornerChar, style);
        var bottomRightCorner = MakeComplexChar(bottomRightCornerChar, style);

        Terminal.Curses.wborder_set(Handle, leftSide, rightSide, topSide, bottomSide,
                    topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner)
                .Check(nameof(Terminal.Curses.wborder_set), "Failed to draw a window border.");
    }

    /// <summary>
    ///     Draws a border around the window's edges using standard characters.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DrawBorder()
    {
        Terminal.Curses.wborder(Handle, 0, 0, 0, 0,
                    0, 0, 0, 0)
                .Check(nameof(Terminal.Curses.wborder), "Failed to draw a window border.");
    }

    /// <summary>
    ///     Draws a horizontal line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="style">The style to use.</param>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawHorizontalLine(Rune @char, Style style, int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Terminal.Curses.whline_set(Handle, MakeComplexChar(@char, style), length)
                .Check(nameof(Terminal.Curses.whline_set), "Failed to draw a horizontal line.");
    }

    /// <summary>
    ///     Draws a horizontal line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    public void DrawHorizontalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Terminal.Curses.whline(Handle, 0, length)
                .Check(nameof(Terminal.Curses.whline), "Failed to draw a horizontal line.");
    }

    /// <summary>
    ///     Removes the text under the caret and moves the contents of the line to the left.
    /// </summary>
    /// <param name="count">The number of characters to remove.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    public void RemoveText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        while (count > 0)
        {
            if (Terminal.Curses.wdelch(Handle)
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
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
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

        Terminal.Curses.win_wchnstr(Handle, arr, count)
                .Check(nameof(Terminal.Curses.win_wchnstr), "Failed to get the text from the window.");

        return arr.Select(BreakComplexChar)
                  .ToArray();
    }

    /// <summary>
    ///     Clears the contents of the row/window.
    /// </summary>
    /// <param name="strategy">The strategy to use.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Clear(ClearStrategy strategy)
    {
        switch (strategy)
        {
            case ClearStrategy.Full:
                Terminal.Curses.werase(Handle)
                        .Check(nameof(Terminal.Curses.werase), "Failed to queue a window erase.");

                break;
            case ClearStrategy.LineFromCaret:
                if (CaretPosition.X < Size.Width - 1)
                {
                    Terminal.Curses.wclrtoeol(Handle)
                            .Check(nameof(Terminal.Curses.wclrtoeol), "Failed to clear the line from the caret.");
                }

                break;
            case ClearStrategy.FullFromCaret:
                Terminal.Curses.wclrtobot(Handle)
                        .Check(nameof(Terminal.Curses.wclrtobot), "Failed to clear the window from the caret.");

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
                Terminal.Curses.overlay(Handle, window.Handle)
                        .Check(nameof(Terminal.Curses.overlay), "Failed to overlay window.");

                break;
            case ReplaceStrategy.Overwrite:
                Terminal.Curses.overwrite(Handle, window.Handle)
                        .Check(nameof(Terminal.Curses.overwrite), "Failed to overwrite window.");

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

        Terminal.Curses.copywin(Handle, window.Handle, srcRect.Top, srcRect.Left, destRect.Top,
                    destRect.Left, destRect.Bottom, destRect.Right,
                    Convert.ToInt32(strategy == ReplaceStrategy.Overlay))
                .Check(nameof(Terminal.Curses.copywin), "Failed to copy the window contents.");
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

        Terminal.Curses.wtouchln(Handle, line, count, 1)
                .Check(nameof(Terminal.Curses.wtouchln), "Failed to mark lines as dirty.");
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
    public bool IsPointWithin(Point point) => Terminal.Curses.wenclose(Handle, point.Y, point.X);

    /// <summary>
    ///     Checks if a given rectangle fits within the current window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>The result of the check.</returns>
    public bool IsRectangleWithin(Rectangle rect) =>
        IsPointWithin(new(rect.X, rect.Y)) && IsPointWithin(new(rect.X + rect.Width - 1, rect.Y + rect.Bottom - 1));

    /// <summary>
    ///     Checks if the row at <paramref name="y" /> is dirty.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="y" /> is outside the bounds.</exception>
    public bool IsRowDirty(int y)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Terminal.Curses.is_linetouched(Handle, y);
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
        Terminal.Curses.clearok(Handle, entireScreen)
                .Check(nameof(Terminal.Curses.clearok), "Failed to configure the refresh mode.");

        if (batch)
        {
            Terminal.Curses.wnoutrefresh(Handle)
                    .Check(nameof(Terminal.Curses.wnoutrefresh), "Failed to queue window refresh.");
        } else
        {
            Terminal.Curses.wrefresh(Handle)
                    .Check(nameof(Terminal.Curses.wrefresh), "Failed to perform window refresh.");
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

        Terminal.Curses.wredrawln(Handle, line, count)
                .Check(nameof(Terminal.Curses.wredrawln), "Failed to perform line refresh.");
    }

    /// <summary>
    ///     Removes the window form the terminal.
    /// </summary>
    public void Remove()
    {
        Terminal.AssertNotDisposed();
        Cleanup();
    }

    /// <summary>
    ///     Removes the window form the terminal.
    /// </summary>
    private void Cleanup()
    {
        if (_handle != IntPtr.Zero)
        {
            // Dispose of all the windows
            var windows = _windows.ToArray();
            foreach (var window in windows)
            {
                window.Dispose();
            }

            Parent?._windows.Remove(this);
            Terminal.Curses.delwin(Handle); // Ignore potential error.

            _handle = IntPtr.Zero;
        }
    }

    /// <summary>
    ///     The destructor. Calls <see cref="Cleanup" />.
    /// </summary>
    ~Window() { Cleanup(); }
}
