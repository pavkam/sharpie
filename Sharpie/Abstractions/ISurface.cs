/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
///     Defines the base traits required for <see cref="Window" />, <see cref="Screen" /> and <see cref="Pad" /> classes.
/// </summary>
[PublicAPI]
public interface ISurface: IDrawSurface
{
    /// <summary>
    ///     The Curses handle for the surface.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    public IntPtr Handle
    {
        get;
    }

    /// <summary>
    ///     Checks if the surface has been disposed and is no longer usable.
    /// </summary>
    public bool Disposed
    {
        get;
    }

    /// <summary>
    ///     Gets or sets the ability of the surface to scroll its contents when writing
    ///     needs a new line.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool Scrollable
    {
        get; set;
    }

    /// <summary>
    ///     Gets or sets the style of the surface.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    Style Style
    {
        get; set;
    }

    /// <summary>
    ///     Gets or sets the color mixture of the surface.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ColorMixture ColorMixture
    {
        get; set;
    }

    /// <summary>
    ///     Gets or sets the surface background.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    (Rune @char, Style style) Background
    {
        get; set;
    }

    /// <summary>
    ///     Gets the size of the surface.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    new Size Size
    {
        get;
    }

    /// <summary>
    ///     Gets or sets the current position of the caret within the surface.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the surface bounds.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    Point CaretLocation
    {
        get; set;
    }

    /// <summary>
    ///     Specifies whether the surface has some "dirty" parts that need to be synchronized
    ///     to the console.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool Dirty
    {
        get;
    }

    /// <summary>
    ///     Controls whether the caret is managed by the application and should not be managed by the hardware.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool ManagedCaret
    {
        get; set;
    }

    /// <summary>
    ///     Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void EnableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DisableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Scrolls the contents of the surface <paramref name="lines" /> up. Only works for scrollable surfaces.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the surface.
    /// </exception>
    /// <exception cref="NotSupportedException">The <see cref="Scrollable" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void ScrollUp(int lines);

    /// <summary>
    ///     Scrolls the contents of the surface <paramref name="lines" /> down. Only works for scrollable surfaces.
    /// </summary>
    /// <param name="lines">Number of lines to scroll.</param>
    /// <exception cref="NotSupportedException">The <see cref="Scrollable" /> is <c>false</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void ScrollDown(int lines);

    /// <summary>
    ///     Inserts <paramref name="lines" /> empty lines at the current caret position.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void InsertEmptyLines(int lines);

    /// <summary>
    ///     Deletes <paramref name="lines" /> lines starting with the current caret position. All lines below move up.
    /// </summary>
    /// <param name="lines">Number of lines to inserts.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="lines" /> is less than one or greater than the size
    ///     of the surface.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DeleteLines(int lines);

    /// <summary>
    ///     Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="width">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void ChangeTextStyle(int width, Style style);

    /// <summary>
    ///     Writes a text at the caret position at the current surface and advances the caret.
    /// </summary>
    /// <param name="text">The styled text to write.</param>
    /// <param name="wrap">If <c>true</c>, text will be wrapped automatically to next line.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void WriteText(StyledText text, bool wrap = true);

    /// <summary>
    ///     Writes a text at the caret position at the current surface and advances the caret.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="style">The style of the text.</param>
    /// <param name="wrap">If <c>true</c>, text will be wrapped automatically to next line.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="text" /> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void WriteText(string text, Style style, bool wrap = true);

    /// <summary>
    ///     Writes a text at the caret position at the current surface and advances the caret.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="wrap">If <c>true</c>, text will be wrapped automatically to next line.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="text" /> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void WriteText(string text, bool wrap = true);

    /// <summary>
    ///     Helper method that moves the caret to the start of the next line. If the surface is <see cref="Scrollable" />, and
    ///     the
    ///     caret if on the last line, it will push the contents of the surface up by one line.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void NextLine();

    /// <summary>
    ///     Draws text using an ASCII <paramref name="font" /> at the caret position at the current
    ///     surface and advances the caret.
    /// </summary>
    /// <param name="font">The ASCII font to draw with.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="style">The style of the text.</param>
    /// <param name="wrap">If <c>true</c>, text will be wrapped automatically to next line.</param>
    /// <param name="interpretSpecialChars">If <c>true</c>, special characters will be treated as such (e.g. \n)</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="font" /> or <paramref name="text" /> are <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawText(IAsciiFont font, string text, Style style, bool interpretSpecialChars = true,
        bool wrap = true);

    /// <summary>
    ///     Draws text using an ASCII <paramref name="font" /> at the caret position at the current
    ///     surface and advances the caret.
    /// </summary>
    /// <param name="font">The ASCII font to draw with.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="wrap">If <c>true</c>, text will be wrapped automatically to next line.</param>
    /// <param name="interpretSpecialChars">If <c>true</c>, special characters will be treated as such (e.g. \n)</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="font" /> or <paramref name="text" /> are <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawText(IAsciiFont font, string text, bool interpretSpecialChars = true, bool wrap = true);

    /// <summary>
    ///     Draws a vertical line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="style">The style to use.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawVerticalLine(int length, Rune @char, Style style);

    /// <summary>
    ///     Draws a vertical line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawVerticalLine(int length);

    /// <summary>
    ///     Draws a horizontal line from the current caret position downwards.
    /// </summary>
    /// <param name="char">The character to use for the line.</param>
    /// <param name="style">The style to use.</param>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawHorizontalLine(int length, Rune @char, Style style);

    /// <summary>
    ///     Draws a horizontal line using the standard line character from the current caret position downwards.
    /// </summary>
    /// <param name="length">The length of the line.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
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
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawBorder(Rune leftSideChar, Rune rightSideChar, Rune topSideChar, Rune bottomSideChar,
        Rune topLeftCornerChar, Rune topRightCornerChar, Rune bottomLeftCornerChar, Rune bottomRightCornerChar,
        Style style);

    /// <summary>
    ///     Draws a border around the surface's edges using standard characters.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DrawBorder();

    /// <summary>
    ///     Removes the text under the caret and moves the contents of the line to the left.
    /// </summary>
    /// <param name="count">The number of characters to remove.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void RemoveText(int count);

    /// <summary>
    ///     Gets the text from the surface at the caret position to the right.
    /// </summary>
    /// <param name="count">The number of characters to get.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    (Rune @char, Style style)[] GetText(int count);

    /// <summary>
    ///     Clears the contents of the row/surface.
    /// </summary>
    /// <param name="strategy">The strategy to use.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Clear(ClearStrategy strategy = ClearStrategy.Full);

    /// <summary>
    ///     Replaces the content of a given surface with the contents of the current surface.
    /// </summary>
    /// <param name="surface">The surface to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="surface" /> is null.</exception>
    /// <exception cref="ArgumentException">The contents of <paramref name="surface" /> cannot be replaced.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Replace(ISurface surface, ReplaceStrategy strategy);

    /// <summary>
    ///     Replaces the content of a given surface with the contents of the current surface.
    /// </summary>
    /// <param name="surface">The surface to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <param name="srcRect">The source rectangle to copy.</param>
    /// <param name="destPos">The destination position.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="surface" /> is null.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Replace(ISurface surface, Rectangle srcRect, Point destPos, ReplaceStrategy strategy);

    /// <summary>
    ///     Marks a number of lines within the surface as <see cref="Dirty" />.
    /// </summary>
    /// <param name="y">The line to start with.</param>
    /// <param name="count">The count of lines to mark dirty.</param>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void MarkDirty(int y, int count);

    /// <summary>
    ///     Marks the entire contents of the surface as <see cref="Dirty" />.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void MarkDirty();

    /// <summary>
    ///     Checks if a given point fits within the current surface.
    /// </summary>
    /// <returns>The result of the check.</returns>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool IsPointWithin(Point point);

    /// <summary>
    ///     Checks if a given rectangle fits within the current surface.
    /// </summary>
    /// <returns>The result of the check.</returns>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool IsRectangleWithin(Rectangle rect);

    /// <summary>
    ///     Draws a given <paramref name="drawable" /> to the surface.
    /// </summary>
    /// <param name="area">The area of the drawing to draw.</param>
    /// <param name="drawable">The drawing to draw.</param>
    /// <param name="location">The location of the drawing.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="drawable" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="location" /> or  <paramref name="area" /> are
    ///     out of bounds.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Draw(Point location, Rectangle area, IDrawable drawable);

    /// <summary>
    ///     Draws a given <paramref name="drawable" /> to the surface.
    /// </summary>
    /// <param name="drawable">The drawing to draw.</param>
    /// <param name="location">The location of the drawing.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="drawable" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location" /> is out of bounds.</exception>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Draw(Point location, IDrawable drawable);

    /// <summary>
    ///     Checks if the line at <paramref name="y" /> is dirty.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Surface is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool LineDirty(int y);

    /// <summary>
    ///     Removes the surface form the parent, destroys all children and itself.
    /// </summary>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Destroy();
}
