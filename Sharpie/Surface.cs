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
///     Represents a surface and contains all its functionality.
/// </summary>
[PublicAPI]
public abstract class Surface: ISurface, IDisposable
{
    private IntPtr _handle;

    /// <summary>
    ///     Initializes the surface using a Curses handle.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="handle">The surface handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="curses" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    /// <remarks>This method is not thread-safe.</remarks>
    internal Surface(ICursesProvider curses, IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("The surface handle has an invalid value.");
        }

        Curses = curses ?? throw new ArgumentNullException(nameof(curses));
        _handle = handle;

        Curses.scrollok(Handle, true)
              .Check(nameof(Curses.scrollok), "Failed to change the scrolling mode.");

        Curses.nodelay(Handle, false)
              .Check(nameof(Curses.nodelay), "Failed to disable read-delay mode.");
    }

    /// <summary>
    ///     The curses backend.
    /// </summary>
    protected internal ICursesProvider Curses { get; }

    /// <summary>
    ///     Returns the origin of the surface (location within a potential parent). This
    ///     implementation always returns <c>(0, 0)</c>.
    /// </summary>
    protected internal virtual Point Origin => Point.Empty;

    /// <summary>
    ///     Disposes the current instance.
    /// </summary>
    public void Dispose()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="ISurface.Handle" />
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    public IntPtr Handle
    {
        get
        {
            AssertAlive();

            return _handle;
        }
    }

    /// <inheritdoc cref="ISurface.ManagedCaret" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public bool ManagedCaret
    {
        get
        {
            AssertSynchronized();
            return Curses.is_leaveok(Handle);
        }
        set
        {
            AssertSynchronized();
            Curses.leaveok(Handle, value)
                  .Check(nameof(Curses.leaveok), "Failed to change the caret manage mode.");
        }
    }

    /// <inheritdoc cref="ISurface.Scrollable" />
    public bool Scrollable
    {
        get
        {
            AssertSynchronized();
            return Curses.is_scrollok(Handle);
        }
        set
        {
            AssertSynchronized();
            Curses.scrollok(Handle, value)
                  .Check(nameof(Curses.scrollok), "Failed to change the scrolling mode.");
        }
    }

    /// <inheritdoc cref="ISurface.Disposed" />
    public bool Disposed => _handle == IntPtr.Zero;

    /// <inheritdoc cref="ISurface.Style" />
    public Style Style
    {
        get
        {
            AssertSynchronized();

            Curses.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero)
                  .Check(nameof(Curses.wattr_get), "Failed to get the surface style.");

            return new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } };
        }
        set
        {
            AssertSynchronized();
            Curses.wattr_set(Handle, (uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                  .Check(nameof(Curses.wattr_set), "Failed to set the surface style.");
        }
    }

    /// <inheritdoc cref="ISurface.ColorMixture" />
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set
        {
            AssertSynchronized();
            Curses.wcolor_set(Handle, value.Handle, IntPtr.Zero)
                  .Check(nameof(Curses.wcolor_set), "Failed to set the surface color mixture.");
        }
    }

    /// <inheritdoc cref="ISurface.Background" />
    public (Rune @char, Style style) Background
    {
        get
        {
            AssertSynchronized();

            Curses.wgetbkgrnd(Handle, out var @char)
                  .Check(nameof(Curses.wgetbkgrnd), "Failed to get the surface background.");

            return Curses.FromComplexChar(@char);
        }
        set
        {
            AssertSynchronized();

            Curses.wbkgrnd(Handle, Curses.ToComplexChar(value.@char, value.style))
                  .Check(nameof(Curses.wbkgrnd), "Failed to set the surface background.");
        }
    }

    /// <inheritdoc cref="ISurface.Size" />
    public Size Size
    {
        get
        {
            AssertSynchronized();
            return new(Curses.getmaxx(Handle)
                             .Check(nameof(Curses.getmaxx), "Failed to get surface width."), Curses.getmaxy(Handle)
                .Check(nameof(Curses.getmaxy), "Failed to get surface height."));
        }
    }

    /// <inheritdoc cref="ISurface.CaretPosition" />
    public Point CaretPosition
    {
        get
        {
            AssertSynchronized();
            return new(Curses.getcurx(Handle)
                             .Check(nameof(Curses.getcurx), "Failed to get caret X position."), Curses.getcury(Handle)
                .Check(nameof(Curses.getcury), "Failed to get caret Y position."));
        }
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

    /// <inheritdoc cref="ISurface.Dirty" />
    public bool Dirty
    {
        get
        {
            AssertSynchronized();
            return Curses.is_wintouched(Handle);
        }
    }

    /// <inheritdoc cref="ISurface.LineDirty" />
    public bool LineDirty(int y)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Curses.is_linetouched(Handle, y);
    }

    /// <inheritdoc cref="IDrawSurface.DrawCell" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    void IDrawSurface.DrawCell(Point location, Rune rune, Style textStyle)
    {
        AssertSynchronized();

        Curses.wmove(Handle, location.Y, location.X)
              .Check(nameof(Curses.wmove), "Failed to move the caret to the given coordinates.");

        Curses.wadd_wch(Handle, Curses.ToComplexChar(rune, textStyle))
              .Check(nameof(Curses.wadd_wch), "Failed to write character to the surface.");
    }

    /// <inheritdoc cref="IDrawSurface.CoversArea" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    bool IDrawSurface.CoversArea(Rectangle area) => IsRectangleWithin(area);

    /// <inheritdoc cref="ISurface.EnableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        AssertSynchronized();

        Curses.wattr_on(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_on), "Failed to enable surface attributes.");
    }

    /// <inheritdoc cref="ISurface.DisableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        AssertSynchronized();

        Curses.wattr_off(Handle, (uint) attributes, IntPtr.Zero)
              .Check(nameof(Curses.wattr_off), "Failed to disable surface attributes.");
    }

    /// <inheritdoc cref="ISurface.ScrollUp" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ScrollUp(int lines)
    {
        if (lines <= 0 || lines > Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        if (!Scrollable)
        {
            throw new NotSupportedException("The surface is not scroll-enabled.");
        }

        Curses.wscrl(Handle, lines)
              .Check(nameof(Curses.wscrl), "Failed to scroll the contents of the surface up.");
    }

    /// <inheritdoc cref="ISurface.ScrollDown" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ScrollDown(int lines)
    {
        if (lines <= 0 || lines > Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        if (!Scrollable)
        {
            throw new NotSupportedException("The surface is not scroll-enabled.");
        }

        Curses.wscrl(Handle, -lines)
              .Check(nameof(Curses.wscrl), "Failed to scroll the contents of the surface down.");
    }

    /// <inheritdoc cref="ISurface.InsertEmptyLines" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void InsertEmptyLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        AssertSynchronized();

        Curses.winsdelln(Handle, lines)
              .Check(nameof(Curses.winsdelln), "Failed to insert blank lines into the surface.");
    }

    /// <inheritdoc cref="ISurface.DeleteLines" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DeleteLines(int lines)
    {
        if (lines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lines));
        }

        AssertSynchronized();

        Curses.winsdelln(Handle, -lines)
              .Check(nameof(Curses.winsdelln), "Failed to delete lines from the surface.");
    }

    /// <inheritdoc cref="ISurface.ChangeTextStyle" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void ChangeTextStyle(int width, Style style)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The length should be greater than zero.");
        }

        AssertSynchronized();

        Curses.wchgat(Handle, width, (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(Curses.wchgat), "Failed to change style of characters in the surface.");
    }

    /// <inheritdoc cref="ISurface.WriteText(string,Sharpie.Style)" />
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

        AssertSynchronized();

        foreach (var rune in str.EnumerateRunes())
        {
            Curses.wadd_wch(Handle, Curses.ToComplexChar(rune, style))
                  .Check(nameof(Curses.wadd_wch), "Failed to write character to the terminal.");
        }
    }

    /// <inheritdoc cref="ISurface.WriteText(string)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void WriteText(string str) => WriteText(str, Style.Default);

    /// <inheritdoc cref="ISurface.DrawVerticalLine(int,System.Text.Rune,Sharpie.Style)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawVerticalLine(int length, Rune @char, Style style)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        AssertSynchronized();

        Curses.wvline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.wvline_set), "Failed to draw a vertical line.");
    }

    /// <inheritdoc cref="ISurface.DrawVerticalLine(int)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawVerticalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        AssertSynchronized();

        Curses.wvline(Handle, 0, length)
              .Check(nameof(Curses.wvline), "Failed to draw a vertical line.");
    }

    /// <inheritdoc cref="ISurface.DrawHorizontalLine(int,System.Text.Rune,Sharpie.Style)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawHorizontalLine(int length, Rune @char, Style style)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        AssertSynchronized();

        Curses.whline_set(Handle, Curses.ToComplexChar(@char, style), length)
              .Check(nameof(Curses.whline_set), "Failed to draw a horizontal line.");
    }

    /// <inheritdoc cref="ISurface.DrawHorizontalLine(int)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawHorizontalLine(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        AssertSynchronized();

        Curses.whline(Handle, 0, length)
              .Check(nameof(Curses.whline), "Failed to draw a horizontal line.");
    }

    /// <inheritdoc
    ///     cref="ISurface.DrawBorder(System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,System.Text.Rune,Sharpie.Style)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawBorder(Rune leftSideChar, Rune rightSideChar, Rune topSideChar, Rune bottomSideChar,
        Rune topLeftCornerChar, Rune topRightCornerChar, Rune bottomLeftCornerChar, Rune bottomRightCornerChar,
        Style style)
    {
        AssertSynchronized();

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
              .Check(nameof(Curses.wborder_set), "Failed to draw a surface border.");
    }

    /// <inheritdoc cref="ISurface.DrawBorder()" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DrawBorder()
    {
        AssertSynchronized();

        Curses.wborder(Handle, 0, 0, 0, 0,
                  0, 0, 0, 0)
              .Check(nameof(Curses.wborder), "Failed to draw a surface border.");
    }

    /// <inheritdoc cref="ISurface.RemoveText" />
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    public void RemoveText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        AssertSynchronized();

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

    /// <inheritdoc cref="ISurface.RemoveText" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public (Rune @char, Style style)[] GetText(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        AssertSynchronized();

        count = Math.Min(count, Size.Width - CaretPosition.X);
        var arr = new CursesComplexChar[count];

        Curses.win_wchnstr(Handle, arr, count)
              .Check(nameof(Curses.win_wchnstr), "Failed to get the text from the surface.");

        return arr.Select(ch => Curses.FromComplexChar(ch))
                  .ToArray();
    }

    /// <inheritdoc cref="ISurface.Clear" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Clear(ClearStrategy strategy = ClearStrategy.Full)
    {
        AssertSynchronized();

        switch (strategy)
        {
            case ClearStrategy.Full:
                Curses.werase(Handle)
                      .Check(nameof(Curses.werase), "Failed to queue a surface erase.");

                break;
            case ClearStrategy.LineFromCaret:
                Curses.wclrtoeol(Handle)
                      .Check(nameof(Curses.wclrtoeol), "Failed to clear the line from the caret.");

                break;
            case ClearStrategy.FullFromCaret:
                Curses.wclrtobot(Handle)
                      .Check(nameof(Curses.wclrtobot), "Failed to clear the surface from the caret.");

                break;
        }
    }

    /// <inheritdoc cref="ISurface.Replace(Sharpie.Abstractions.ISurface, Sharpie.ReplaceStrategy)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public virtual void Replace(ISurface surface, ReplaceStrategy strategy)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        if (surface == this)
        {
            throw new ArgumentException(nameof(surface));
        }

        AssertSynchronized();

        switch (strategy)
        {
            case ReplaceStrategy.Overlay:
                Curses.overlay(Handle, surface.Handle)
                      .Check(nameof(Curses.overlay), "Failed to overlay surface.");

                break;
            case ReplaceStrategy.Overwrite:
                Curses.overwrite(Handle, surface.Handle)
                      .Check(nameof(Curses.overwrite), "Failed to overwrite surface.");

                break;
        }
    }

    /// <inheritdoc
    ///     cref="ISurface.Replace(Sharpie.Abstractions.ISurface, System.Drawing.Rectangle, System.Drawing.Point, Sharpie.ReplaceStrategy)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Replace(ISurface surface, Rectangle srcRect, Point destPos, ReplaceStrategy strategy)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        if (surface == this)
        {
            throw new ArgumentException(nameof(surface));
        }

        if (!IsRectangleWithin(srcRect))
        {
            throw new ArgumentOutOfRangeException(nameof(srcRect));
        }

        var destRect = new Rectangle(destPos, new(srcRect.Bottom - srcRect.Top, srcRect.Right - srcRect.Left));
        if (!surface.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(destPos));
        }

        Curses.copywin(Handle, surface.Handle, srcRect.Top, srcRect.Left, destRect.Top,
                  destRect.Left, destRect.Bottom, destRect.Right, Convert.ToInt32(strategy == ReplaceStrategy.Overlay))
              .Check(nameof(Curses.copywin), "Failed to copy the surface contents.");
    }

    /// <inheritdoc cref="ISurface.MarkDirty(int, int)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public virtual void MarkDirty(int y, int count)
    {
        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var (actY, actCount) = Helpers.IntersectSegments(y, count, 0, Size.Height);

        if (actY > -1 && actCount > 0)
        {
            Curses.wtouchln(Handle, actY, actCount, 1)
                  .Check(nameof(Curses.wtouchln), "Failed to mark lines as dirty.");
        }
    }

    /// <inheritdoc cref="ISurface.MarkDirty()" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void MarkDirty() => MarkDirty(0, Size.Height);

    /// <inheritdoc cref="ISurface.IsPointWithin" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public bool IsPointWithin(Point point)
    {
        var size = Size;
        var origin = Origin;
        return point.X >= origin.X && point.Y >= origin.Y && point.X < size.Width && point.Y < size.Height;
    }

    /// <inheritdoc cref="ISurface.IsRectangleWithin" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public bool IsRectangleWithin(Rectangle rect) =>
        IsPointWithin(new(rect.Left, rect.Top)) && IsPointWithin(new(rect.Right - 1, rect.Bottom - 1));

    /// <inheritdoc cref="ISurface.Draw(Point, Rectangle, IDrawable)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Draw(Point location, Rectangle area, IDrawable drawable)
    {
        if (drawable == null)
        {
            throw new ArgumentNullException(nameof(drawable));
        }

        AssertSynchronized();

        drawable.DrawTo(this, area, location);
    }

    /// <inheritdoc cref="ISurface.Draw(Point, IDrawable)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Draw(Point location, IDrawable drawable) =>
        Draw(location, new(0, 0, drawable.Size.Width, drawable.Size.Height), drawable);

    /// <inheritdoc cref="ISurface.Destroy" />
    public void Destroy()
    {
        if (!Disposed)
        {
            Delete();
        }
    }

    /// <summary>
    ///     Asserts that executing thread is bound to the correct synchronization context.
    /// </summary>
    /// <exception cref="CursesSynchronizationException">Thrown if current thread is not bound to the correct context.</exception>
    protected internal abstract void AssertSynchronized();

    /// <summary>
    ///     Asserts that the surface is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The surface has been disposed.</exception>
    protected internal void AssertAlive()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException("The surface has been disposed and is no longer usable.");
        }
    }

    /// <summary>
    ///     Deletes the surface from the curses backend.
    /// </summary>
    protected virtual void Delete()
    {
        AssertSynchronized();

        Debug.Assert(_handle != IntPtr.Zero);

        Curses.delwin(_handle);
        _handle = IntPtr.Zero;
    }

    /// <summary>
    ///     The destructor. Calls <see cref="Destroy" />.
    /// </summary>
    ~Surface() { Destroy(); }
}
