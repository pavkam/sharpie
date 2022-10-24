namespace Sharpie;

using System.Drawing;
using JetBrains.Annotations;

/// <summary>
/// Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window: IDisposable
{
    protected Terminal Terminal { get; }

    private bool _enableProcessingKeypadKeys;
    private bool _useHardwareLineInsertAndDelete;
    private bool _useHardwareCharacterInsertAndDelete;
    private bool _immediateRefresh;

    /// <summary>
    /// The Curses handle for the window.
    /// </summary>
    public IntPtr Handle { get; private set; }

    /// <summary>
    /// Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="terminal">The curses functionality provider.</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Window(Terminal terminal, IntPtr windowHandle)
    {
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        Handle = windowHandle;

        _useHardwareCharacterInsertAndDelete = Terminal.Curses.has_ic();
    }

    /// <summary>
    /// Checks if the window is not disposed.
    /// </summary>
    public bool IsDisposed => Terminal.IsDisposed || Handle == IntPtr.Zero;

    /// <summary>
    /// Asserts that the window is not disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The window has been disposed.</exception>
    internal void AssertNotDisposed()
    {
        Terminal.AssertNotDisposed();

        if (Handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException("The window has been disposed and is no longer usable.");
        }
    }

    /// <summary>
    /// Enables or disables the processing of keypad keys.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableProcessingKeypadKeys
    {
        get
        {
            AssertNotDisposed();

            return _enableProcessingKeypadKeys;
        }
        set
        {
            AssertNotDisposed();

            Terminal.Curses.keypad(Handle, value)
                    .TreatError();

            _enableProcessingKeypadKeys = value;
        }
    }

    /// <summary>
    /// Enables or disables the use of hardware line insert/delete handling fpr this window.
    /// </summary>
    /// <remarks>
    /// This functionality only works if hardware has support for it. Consult <see cref="Sharpie.Terminal.SupportsHardwareLineInsertAndDelete" />
    /// Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool UseHardwareLineInsertAndDelete
    {
        get
        {
            AssertNotDisposed();

            return _useHardwareLineInsertAndDelete;
        }
        set
        {
            AssertNotDisposed();

            if (Terminal.Curses.has_il())
            {
                Terminal.Curses.idlok(Handle, value)
                        .TreatError();

                _useHardwareLineInsertAndDelete = value;
            }
        }
    }

    /// <summary>
    /// Enables or disables the use of hardware character insert/delete handling for this window.
    /// </summary>
    ///    <remarks>
    /// This functionality only works if hardware has support for it. Consult <see cref="Sharpie.Terminal.SupportsHardwareCharacterInsertAndDelete" />
    /// Default is <c>true</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool UseHardwareCharacterInsertAndDelete
    {
        get
        {
            AssertNotDisposed();

            return _useHardwareCharacterInsertAndDelete;
        }
        set
        {
            AssertNotDisposed();

            if (Terminal.Curses.has_ic())
            {
                Terminal.Curses.idcok(Handle, value);
                _useHardwareCharacterInsertAndDelete = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Style Style
    {
        get
        {
            AssertNotDisposed();

            Terminal.Curses.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero)
                    .TreatError();

            return new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } };
        }
        set
        {
            AssertNotDisposed();

            Terminal.Curses.wattr_set(Handle, (uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                    .TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set
        {
            AssertNotDisposed();

            Terminal.Curses.wcolor_set(Handle, value.Handle, IntPtr.Zero)
                    .TreatError();
        }
    }

    /// <summary>
    /// Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        AssertNotDisposed();

        Terminal.Curses.wattr_on(Handle, (uint) attributes, IntPtr.Zero)
                .TreatError();
    }

    /// <summary>
    /// Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        AssertNotDisposed();

        Terminal.Curses.wattr_off(Handle, (uint) attributes, IntPtr.Zero)
                .TreatError();
    }

    /// <summary>
    /// Tries to move the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="x"/> or <paramref name="y"/> are negative.</exception>
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

        AssertNotDisposed();
        return Terminal.Curses.wmove(Handle, y, x) != Helpers.CursesErrorResult;
    }

    /// <summary>
    /// Moves the caret to a given position within the window.
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
    /// Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="width">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentException">The <paramref name="width"/> is less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ChangeTextStyle(int width, Style style)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The length should be greater than zero.");
        }

        AssertNotDisposed();

        Terminal.Curses.wchgat(Handle, width, (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
                .TreatError();
    }

    /// <summary>
    /// Writes a character at the caret position at the current window and advances the caret.
    /// </summary>
    /// <param name="char">The character to write.</param>
    /// <param name="style">The applied style.</param>
    /// <param name="insert">If <c>true</c>, the text is inserted before the caret and not at the caret position.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void WriteText(char @char, Style style, bool insert = false)
    {
        AssertNotDisposed();

        Terminal.Curses.setcchar(out var cChar, @char.ToString(), (uint) style.Attributes, style.ColorMixture.Handle,
            IntPtr.Zero);

        if (insert)
        {
            Terminal.Curses.wins_wch(Handle, cChar)
                    .TreatError();
        }
        else
        {
            Terminal.Curses.wadd_wch(Handle, cChar)
                    .TreatError();
        }
    }

    /// <summary>
    /// Writes a text at the caret position at the current window and advance the caret.
    /// </summary>
    /// <param name="str">The text to write.</param>
    /// <param name="insert">If <c>true</c>, the text is inserted before the caret and not at the caret position.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="str"/> is <c>null</c>.</exception>
    public void WriteText(FormattedText str, bool insert = false)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        AssertNotDisposed();

        if (insert)
        {
            Terminal.Curses.wadd_wchnstr(Handle, str.Characters, str.Characters.Length)
                    .TreatError();
        } else
        {
            foreach (var c in str.Characters)
            {
                Terminal.Curses.wins_wch(Handle, c)
                        .TreatError();
            }
        }
    }

    /// <summary>
    /// Clears the contents of the current row.
    /// </summary>
    /// <param name="strategy">The strategy to use.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Clear(ClearStrategy strategy)
    {
        AssertNotDisposed();

        switch (strategy)
        {
            case ClearStrategy.Full:
                Terminal.Curses.wclear(Handle)
                        .TreatError();

                break;
            case ClearStrategy.LineFromCaret:
                if (CaretPosition.X < Size.Width - 1)
                {
                    Terminal.Curses.wclrtoeol(Handle)
                            .TreatError();
                }

                break;
            case ClearStrategy.FullFromCaret:
                Terminal.Curses.wclrtobot(Handle)
                        .TreatError();

                break;
        }
    }

    /// <summary>
    /// Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="window"/> is null.</exception>
    public void Replace(Window window, ReplaceStrategy strategy)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        AssertNotDisposed();
        switch (strategy)
        {
            case ReplaceStrategy.Overlay:
                Terminal.Curses.overlay(Handle, window.Handle)
                        .TreatError();

                break;
            case ReplaceStrategy.Overwrite:
                Terminal.Curses.overwrite(Handle, window.Handle)
                        .TreatError();

                break;
        }
    }

    /// <summary>
    /// Replaces the content of a given window with the contents of the current window.
    /// </summary>
    /// <param name="window">The window to copy contents to.</param>
    /// <param name="strategy">The used strategy.</param>
    /// <param name="srcRect">The source rectangle to copy.</param>
    /// <param name="destPos">The destination position.</param>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="window"/> is null.</exception>
    public void Replace(Window window, Rectangle srcRect, Point destPos, ReplaceStrategy strategy)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        AssertNotDisposed();
        window.AssertNotDisposed();

        var destRect = new Rectangle(destPos, new(srcRect.Bottom - srcRect.Top, srcRect.Right - srcRect.Left));
        if (!window.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(srcRect));
        }

        Terminal.Curses.copywin(Handle, window.Handle, srcRect.Top, srcRect.Left, destRect.Top,
                    destRect.Left, destRect.Bottom, destRect.Right,
                    Convert.ToInt32(strategy == ReplaceStrategy.Overlay))
                .TreatError();
    }

    /// <summary>
    /// Checks if a given point fits within the current window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>The result of the check.</returns>
    public bool IsPointWithin(Point point)
    {
        var size = Size;
        return point.X >= 0 && point.X < size.Width && point.Y >= 0 && point.Y < size.Height;
    }

    /// <summary>
    /// Checks if a given rectangle fits within the current window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or either of the windows have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>The result of the check.</returns>
    public bool IsRectangleWithin(Rectangle rect)
    {
        var size = Size;
        return rect.Left >= 0 &&
            rect.Left < size.Width &&
            rect.Top >= 0 &&
            rect.Top < size.Height &&
            rect.Right >= rect.Left &&
            rect.Right < size.Width &&
            rect.Bottom >= rect.Top &&
            rect.Bottom < size.Height;
    }

    /// <summary>
    /// Gets or sets the location of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is outside the bounds.</exception>
    public Point Location
    {
        get
        {
            AssertNotDisposed();

            return new(Terminal.Curses.getbegx(Handle)
                               .TreatError(), Terminal.Curses.getbegy(Handle)
                                                      .TreatError());
        }
        set
        {
            AssertNotDisposed();

            if (value.X < 1 || value.Y < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Terminal.Curses.mvwin(Handle, value.Y, value.X)
                    .TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is outside the bounds.</exception>
    public Size Size
    {
        get
        {
            AssertNotDisposed();

            return new(Terminal.Curses.getmaxx(Handle)
                               .TreatError(), Terminal.Curses.getmaxy(Handle)
                                                      .TreatError());
        }
        set
        {
            AssertNotDisposed();

            if (value.Width < 1 || value.Height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Terminal.Curses.wresize(Handle, value.Height, value.Width)
                    .TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the current position of the caret within the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is outside the window bounds.</exception>
    public Point CaretPosition
    {
        get
        {
            AssertNotDisposed();

            return new(Terminal.Curses.getcurx(Handle)
                               .TreatError(), Terminal.Curses.getcury(Handle)
                                                      .TreatError());
        }
        set
        {
            AssertNotDisposed();

            if (value.X < 0 || value.Y < 1 || value.X >= Size.Width || value.Y >= Size.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Terminal.Curses.wmove(Handle, value.Y, value.X)
                    .TreatError();
        }
    }

    /// <summary>
    /// Checks if the row at <paramref name="y"/> is dirty.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="y"/> is outside the bounds.</exception>
    public bool IsRowDirty(int y)
    {
        AssertNotDisposed();

        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Terminal.Curses.is_linetouched(Handle, y);
    }

    /// <summary>
    /// Specifies whether the window has some "dirty" parts that need to be synchronized
    /// to the console.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool IsDirty
    {
        get
        {
            AssertNotDisposed();

            return Terminal.Curses.is_wintouched(Handle);
        }
    }

    /// <summary>
    /// Set or get the immediate refresh capability of the window.
    /// </summary>
    /// <remarks>
    /// Immediate refresh will redraw the window on each change.
    /// This might be very slow for most use cases.
    /// Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    public bool ImmediateRefresh
    {
        get
        {
            AssertNotDisposed();
            return _immediateRefresh;
        }
        set
        {
            AssertNotDisposed();
            Terminal.Curses.immedok(Handle, value);

            _immediateRefresh = value;
        }
    }

    /// <summary>
    /// Refreshes the window by synchronizing it to the terminal.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Refresh(bool batch)
    {
        AssertNotDisposed();

        if (batch)
        {
            Terminal.Curses.wnoutrefresh(Handle)
                    .TreatError();
        } else
        {
            Terminal.Curses.wrefresh(Handle)
                    .TreatError();
        }
    }

    /// <summary>
    /// Removes the window form the terminal.
    /// </summary>
    public void Remove()
    {
        Terminal.AssertNotDisposed();
        Cleanup();
    }

    /// <summary>
    /// Removes the window form the terminal.
    /// </summary>
    private void Cleanup()
    {
        Terminal.RemoveWindow(this);
        Terminal.Curses.delwin(Handle); // Ignore potential error.

        Handle = IntPtr.Zero;
    }

    /// <summary>
    /// Disposes the current instance.
    /// </summary>
    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The destructor. Calls <see cref="Cleanup"/>.
    /// </summary>
    ~Window() { Cleanup(); }
}
