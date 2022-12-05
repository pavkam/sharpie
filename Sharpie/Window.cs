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
///     Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window: IWindow, IDisposable
{
    private readonly Window? _parent;
    private readonly IList<Window> _windows = new List<Window>();
    private IntPtr _handle;

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

        _parent = parent;
        _parent?._windows.Add(this);

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

    /// <inheritdoc cref="IWindow.Parent"/>
    public IWindow? Parent => _parent;
    
    /// <inheritdoc cref="IWindow.Handle"/>
    public IntPtr Handle
    {
        get
        {
            AssertAlive();
            return _handle;
        }
    }

    /// <inheritdoc cref="IWindow.Children"/>
    public IEnumerable<IWindow> Children => _windows;

    /// <inheritdoc cref="IWindow.EnableScrolling"/>
    public bool EnableScrolling
    {
        get => Curses.is_scrollok(Handle);
        set =>
            Curses.scrollok(Handle, value)
                  .Check(nameof(Curses.scrollok), "Failed to change the scrolling mode.");
    }

    /// <inheritdoc cref="IWindow.Disposed"/>
    public bool Disposed => _handle == IntPtr.Zero;

    /// <inheritdoc cref="IWindow.UseHardwareLineEdit"/>
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

    /// <inheritdoc cref="IWindow.UseHardwareCharEdit"/>
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

    /// <inheritdoc cref="IWindow.Style"/>
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

    /// <inheritdoc cref="IWindow.ColorMixture"/>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set =>
            Curses.wcolor_set(Handle, value.Handle, IntPtr.Zero)
                  .Check(nameof(Curses.wcolor_set), "Failed to set the window color mixture.");
    }

    /// <inheritdoc cref="IWindow.Background"/>
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

    /// <inheritdoc cref="IWindow.Location"/>
    public virtual Point Location
    {
        get
        {
            if (Parent != null && Parent is not Screen)
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

            if (Parent != null && Parent is not Screen)
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

    /// <inheritdoc cref="IWindow.Size"/>
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

    /// <inheritdoc cref="IWindow.CaretPosition"/>
    public Point CaretPosition
    {
        get =>
            new(Curses.getcurx(Handle)
                      .Check(nameof(Curses.getcurx), "Failed to get caret X position."), Curses.getcury(Handle)
                .Check(nameof(Curses.getcury), "Failed to get caret Y position."));
        set
        {
            if (!((IWindow)this).IsPointWithin(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wmove(Handle, value.Y, value.X)
                  .Check(nameof(Curses.wmove), "Failed to move the caret.");
        }
    }

    /// <inheritdoc cref="IWindow.Invalidated"/>
    public bool Invalidated => Curses.is_wintouched(Handle);

    /// <inheritdoc cref="IWindow.ImmediateRefresh"/>
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

            Curses.leaveok(Handle, value)
                  .Check(nameof(Curses.leaveok), "Failed to set hardware caret ignore mode.");
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

    /// <inheritdoc cref="IDrawSurface.DrawCell" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    void IDrawSurface.DrawCell(Point location, Rune rune, Style textStyle)
    {
        Curses.wmove(Handle, location.Y, location.X)
              .Check(nameof(Curses.wmove), "Failed to move the caret to the given coordinates.");

        Curses.wadd_wch(Handle, Curses.ToComplexChar(rune, textStyle))
              .Check(nameof(Curses.wadd_wch), "Failed to write character to the window.");
    }

    /// <inheritdoc cref="IDrawSurface.CoversArea" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    bool IDrawSurface.CoversArea(Rectangle area) => ((IWindow)this).IsRectangleWithin(area);

    /// <inheritdoc cref="IWindow.IsRelatedTo" />
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is <c>null</c>.</exception>
    public bool IsRelatedTo(IWindow window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        return window.Equals(this) ||
            Children.Any(child => window.IsRelatedTo(child)) ||
            window.Children.Any(child => child.IsRelatedTo(this));
    }

    /// <summary>
    ///     Asserts that the window is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The window has been disposed.</exception>
    protected internal void AssertAlive()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException("The window has been disposed and is no longer usable.");
        }
    }

    /// <inheritdoc cref="IWindow.EnableAttributes"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        Curses.wattr_on(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_on), "Failed to enable window attributes.");
    }

    /// <inheritdoc cref="IWindow.DisableAttributes"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        Curses.wattr_off(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_off), "Failed to disable window attributes.");
    }

    /// <inheritdoc cref="IWindow.ScrollUp"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
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

    /// <inheritdoc cref="IWindow.ScrollDown"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
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

    /// <inheritdoc cref="IWindow.InsertEmptyLines"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void InsertEmptyLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Curses.winsdelln(Handle, lines)
              .Check(nameof(Curses.winsdelln), "Failed to insert blank lines into the window.");
    }

    /// <inheritdoc cref="IWindow.DeleteLines"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DeleteLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        Curses.winsdelln(Handle, -lines)
              .Check(nameof(Curses.winsdelln), "Failed to delete lines from the window.");
    }

    /// <inheritdoc cref="IWindow.ChangeTextStyle"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ChangeTextStyle(int width, Style style)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The length should be greater than zero.");
        }

        Curses.wchgat(Handle, width, (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(Curses.wchgat), "Failed to change style of characters in the window.");
    }

    /// <inheritdoc cref="IWindow.WriteText(string,Sharpie.Style)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void WriteText(string str, Style style)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.Length == 0)
        {
            return;
        }

        foreach (var rune in str.EnumerateRunes())
        {
            Curses.wadd_wch(Handle, Curses.ToComplexChar(rune, style))
                  .Check(nameof(Curses.wadd_wch), "Failed to write character to the terminal.");
        }
    }

    /// <inheritdoc cref="IWindow.DrawVerticalLine(int,System.Text.Rune,Sharpie.Style)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawVerticalLine(int length, Rune @char, Style style)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.wvline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.wvline_set), "Failed to draw a vertical line.");
    }

    /// <inheritdoc cref="IWindow.DrawVerticalLine(int)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawVerticalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.wvline(Handle, 0, length)
              .Check(nameof(Curses.wvline), "Failed to draw a vertical line.");
    }

    /// <inheritdoc cref="IWindow.DrawHorizontalLine(int,System.Text.Rune,Sharpie.Style)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawHorizontalLine(int length, Rune @char, Style style)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.whline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.whline_set), "Failed to draw a horizontal line.");
    }

    /// <inheritdoc cref="IWindow.DrawHorizontalLine(int)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawHorizontalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Curses.whline(Handle, 0, length)
              .Check(nameof(Curses.whline), "Failed to draw a horizontal line.");
    }

    /// <inheritdoc cref="IWindow.DrawBorder(System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,Sharpie.Style)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
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

    /// <inheritdoc cref="IWindow.DrawBorder()"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawBorder()
    {
        Curses.wborder(Handle, 0, 0, 0, 0,
                  0, 0, 0, 0)
              .Check(nameof(Curses.wborder), "Failed to draw a window border.");
    }

    /// <inheritdoc cref="IWindow.RemoveText"/>
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

    /// <inheritdoc cref="IWindow.RemoveText"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public (Rune @char, Style style)[] GetText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        count = Math.Min(count, Size.Width - CaretPosition.X);
        var arr = new CursesComplexChar[count];

        Curses.win_wchnstr(Handle, arr, count)
              .Check(nameof(Curses.win_wchnstr), "Failed to get the text from the window.");

        return arr.Select(ch => Curses.FromComplexChar(ch))
                  .ToArray();
    }

    /// <inheritdoc cref="IWindow.Clear"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Clear(ClearStrategy strategy = ClearStrategy.Full)
    {
        switch (strategy)
        {
            case ClearStrategy.Full:
                Curses.werase(Handle)
                      .Check(nameof(Curses.werase), "Failed to queue a window erase.");

                break;
            case ClearStrategy.LineFromCaret:
                Curses.wclrtoeol(Handle)
                      .Check(nameof(Curses.wclrtoeol), "Failed to clear the line from the caret.");

                break;
            case ClearStrategy.FullFromCaret:
                Curses.wclrtobot(Handle)
                      .Check(nameof(Curses.wclrtobot), "Failed to clear the window from the caret.");

                break;
        }
    }

    /// <inheritdoc cref="IWindow.Replace(Sharpie.Abstractions.IWindow, Sharpie.ReplaceStrategy)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Replace(IWindow window, ReplaceStrategy strategy)
    {
        if (IsRelatedTo(window))
        {
            throw new ArgumentException("Cannot copy to a window that is related to this window.", nameof(window));
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

    /// <inheritdoc cref="IWindow.Replace(Sharpie.Abstractions.IWindow, System.Drawing.Rectangle, System.Drawing.Point, Sharpie.ReplaceStrategy)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Replace(IWindow window, Rectangle srcRect, Point destPos, ReplaceStrategy strategy)
    {
        if (IsRelatedTo(window))
        {
            throw new ArgumentException("Cannot copy to a window that is related to this window.", nameof(window));
        }

        if (!((IWindow)this).IsRectangleWithin(srcRect))
        {
            throw new ArgumentOutOfRangeException(nameof(srcRect));
        }

        var destRect = new Rectangle(destPos, new(srcRect.Bottom - srcRect.Top, srcRect.Right - srcRect.Left));
        if (!window.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(destPos));
        }

        Curses.copywin(Handle, window.Handle, srcRect.Top, srcRect.Left, destRect.Top,
                  destRect.Left, destRect.Bottom, destRect.Right, Convert.ToInt32(strategy == ReplaceStrategy.Overlay))
              .Check(nameof(Curses.copywin), "Failed to copy the window contents.");
    }

    /// <inheritdoc cref="IWindow.Invalidate(int, int)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Invalidate(int y, int count)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (count <= 0 || y + count - 1 >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Curses.wtouchln(Handle, y, count, 1)
              .Check(nameof(Curses.wtouchln), "Failed to mark lines as dirty.");
    }

    /// <inheritdoc cref="IWindow.LineInvalidated"/>
    public bool LineInvalidated(int y)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Curses.is_linetouched(Handle, y);
    }

    /// <inheritdoc cref="IWindow.Refresh(bool, bool)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
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

    /// <inheritdoc cref="IWindow.Refresh(int, int)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public virtual void Refresh(int y, int count)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (count < 1 || y + count - 1 >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Curses.wredrawln(Handle, y, count)
              .Check(nameof(Curses.wredrawln), "Failed to perform line refresh.");
    }

    /// <inheritdoc cref="IWindow.Destroy"/>
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

            _parent?._windows.Remove(this);

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
