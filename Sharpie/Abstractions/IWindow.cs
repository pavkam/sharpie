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
///     Defines the traits implemented by the <see cref="Window"/> class.
/// </summary>
[PublicAPI]
public interface IWindow: IDrawSurface
{
    /// <summary>
    ///     The parent of this object.
    /// </summary>
    IWindow? Parent { get; }

    /// <summary>
    ///     Lists of children of this object.
    /// </summary>
    IEnumerable<IWindow> Children { get; }

    /// <summary>
    ///     Checks if the given <paramref name="window" /> is either a descendant or an ancestor of this window.
    /// </summary>
    /// <param name="window">The window to check.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is <c>null</c>.</exception>
    bool IsRelatedTo(IWindow window);
    
    /// <summary>
    ///     The Curses handle for the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    public IntPtr Handle { get; }

    /// <summary>
    ///     Checks if the window has been disposed and is no longer usable.
    /// </summary>
    public bool Disposed { get; }

    /// <summary>
    ///     Gets or sets the ability of the window to scroll its contents when writing
    ///     needs a new line.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool EnableScrolling { get; }

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
    ///     Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    Style Style { get; set; }

    /// <summary>
    ///     Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    ColorMixture ColorMixture { get; set; }

    /// <summary>
    ///     Gets or sets the window background.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    (Rune @char, Style style) Background { get; set; }

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
    Size Size { get; set; }

    /// <summary>
    ///     Gets or sets the current position of the caret within the window.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the window bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    Point CaretPosition { get; set; }

    /// <summary>
    ///     Specifies whether the window has some "dirty" parts that need to be synchronized
    ///     to the console.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool Invalidated { get; }

    /// <summary>
    ///     Set or get the immediate refresh capability of the window.
    /// </summary>
    /// <remarks>
    ///     Immediate refresh will redraw the window on each change.
    ///     This might be very slow for most use cases.
    ///     Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool ImmediateRefresh { get; set; }
    
    /// <summary>
    ///     Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void EnableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DisableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Scrolls the contents of the window <paramref name="lines" /> up. Only works for scrollable windows.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="NotSupportedException">The <see cref="EnableScrolling" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void ScrollUp(int lines);

    /// <summary>
    ///     Scrolls the contents of the window <paramref name="lines" /> down. Only works for scrollable windows.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="NotSupportedException">The <see cref="EnableScrolling" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void ScrollDown(int lines);

    /// <summary>
    ///     Inserts <paramref name="lines" /> empty lines at the current caret position.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void InsertEmptyLines(int lines);

    /// <summary>
    ///     Deletes <paramref name="lines" /> lines starting with the current caret position. All lines below move up.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the window.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DeleteLines(int lines);

    /// <summary>
    ///     Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="width">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ArgumentException">The <paramref name="width" /> is less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void ChangeTextStyle(int width, Style style);

    /// <summary>
    ///     Writes a text at the caret position at the current window and advances the caret.
    /// </summary>
    /// <param name="str">The text to write.</param>
    /// <param name="style">The style of the text.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="str" /> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void WriteText(string str, Style style);

    /// <summary>
    ///     Writes a text at the caret position at the current window and advances the caret.
    /// </summary>
    /// <remarks>
    ///     This method uses default style.
    /// </remarks>
    /// <param name="str">The text to write.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="str" /> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void WriteText(string str) => WriteText(str, Style.Default);

    /// <summary>
    ///     Draws a vertical line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="style">The style to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawVerticalLine(int length, Rune @char, Style style);

    /// <summary>
    ///     Draws a vertical line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawVerticalLine(int length);

    /// <summary>
    ///     Draws a horizontal line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="style">The style to use.</param>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawHorizontalLine(int length, Rune @char, Style style);

    /// <summary>
    ///     Draws a horizontal line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawHorizontalLine(int length);

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
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawBorder(Rune leftSideChar, Rune rightSideChar, Rune topSideChar, Rune bottomSideChar,
        Rune topLeftCornerChar, Rune topRightCornerChar, Rune bottomLeftCornerChar, Rune bottomRightCornerChar,
        Style style);

    /// <summary>
    ///     Draws a border around the window's edges using standard characters.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void DrawBorder();

    /// <summary>
    ///     Removes the text under the caret and moves the contents of the line to the left.
    /// </summary>
    /// <param name="count">The number of characters to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void RemoveText(int count);

    /// <summary>
    ///     Gets the text from the window at the caret position to the right.
    /// </summary>
    /// <param name="count">The number of characters to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count" /> less than one.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    (Rune @char, Style style)[] GetText(int count);

    /// <summary>
    ///     Clears the contents of the row/window.
    /// </summary>
    /// <param name="strategy">The strategy to use.</param>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Clear(ClearStrategy strategy = ClearStrategy.Full);

    /// <summary>
    ///     Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is null.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Replace(IWindow window, ReplaceStrategy strategy);

    /// <summary>
    ///     Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <param name="srcRect">The source rectangle to copy.</param>
    /// <param name="destPos">The destination position.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="window" /> is null.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Replace(IWindow window, Rectangle srcRect, Point destPos, ReplaceStrategy strategy);

    /// <summary>
    ///     Invalidates a number of lines within the window.
    /// </summary>
    /// <param name="y">The line to start with.</param>
    /// <param name="count">The count of lines to invalidate.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="y" /> and <paramref name="count" /> combination is
    ///     out of bounds.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Invalidate(int y, int count);

    /// <summary>
    ///     Invalidates the contents of the window thus forcing a redraw at the next <see cref="Refresh(bool,bool)" />.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Invalidate() { Invalidate(0, Size.Height); }

    /// <summary>
    ///     Checks if a given point fits within the current window.
    /// </summary>
    /// <returns>The result of the check.</returns>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool IsPointWithin(Point point)
    {
        var size = Size;
        return point.X >= 0 && point.Y >= 0 && point.X < size.Width && point.Y < size.Height;
    }

    /// <summary>
    ///     Checks if a given rectangle fits within the current window.
    /// </summary>
    /// <returns>The result of the check.</returns>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool IsRectangleWithin(Rectangle rect) =>
        IsPointWithin(new(rect.Left, rect.Top)) &&
        IsPointWithin(new(rect.Left + rect.Width - 1, rect.Top + rect.Height - 1));

    /// <summary>
    ///     Draws a given <paramref name="drawable" /> to the window.
    /// </summary>
    /// <param name="area">The area of the drawing to draw.</param>
    /// <param name="drawable">The drawing to draw.</param>
    /// <param name="location">The location of the drawing.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="drawable" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="location" /> or  <paramref name="area" /> are
    ///     out of bounds.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Draw(Point location, Rectangle area, IDrawable drawable)
    {
        if (drawable == null)
        {
            throw new ArgumentNullException(nameof(drawable));
        }

        drawable.DrawTo(this, area, location);
    }

    /// <summary>
    ///     Draws a given <paramref name="drawable" /> to the window.
    /// </summary>
    /// <param name="drawable">The drawing to draw.</param>
    /// <param name="location">The location of the drawing.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="drawable" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location" /> is out of bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Draw(Point location, IDrawable drawable) =>
        Draw(location, new(0, 0, drawable.Size.Width, drawable.Size.Height), drawable);

    /// <summary>
    ///     Checks if the line at <paramref name="y" /> is dirty.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="y" /> is outside the bounds.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    bool LineInvalidated(int y);

    /// <summary>
    ///     Refreshes the window by synchronizing it to the terminal.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <param name="entireScreen">If <c>true</c>, when this refresh happens, the entire screen is redrawn.</param>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Refresh(bool batch, bool entireScreen);

    /// <summary>
    ///     Refreshes the window by synchronizing it to the terminal with immediate redraw.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Refresh() => Refresh(false, false);

    /// <summary>
    ///     Refreshes a number of lines within the window.
    /// </summary>
    /// <param name="y">The starting line to refresh.</param>
    /// <param name="count">The number of lines to refresh.</param>
    /// <exception cref="ArgumentOutOfRangeException">The combination of lines and count exceed the window boundary.</exception>
    /// <exception cref="ObjectDisposedException">Window is no longer usable.</exception>
    void Refresh(int y, int count);

    /// <summary>
    ///     Removes the window form the parent, destroys all children and itself.
    /// </summary>
    void Destroy();
}
