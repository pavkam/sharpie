namespace Sharpie;

using Curses;

/// <summary>
///     The core curses screen class. Implements screen-related functionality.
/// </summary>
public sealed class Screen: Window
{
    /// <summary>
    /// Initializes the screen using a window handle. The <paramref name="windowHandle"/> should be
    /// a screen and not a regular window.
    /// </summary>
    /// <param name="terminal">The terminal instance.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(Terminal terminal, IntPtr windowHandle): base(terminal, windowHandle) { }

    /// <summary>
    /// Created a new window in the screen.
    /// </summary>
    /// <param name="x">The X coordinate of the window location.</param>
    /// <param name="y">The Y coordinate of the window location.</param>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="x"/> or <paramref name="y"/> are negative;
    /// or <paramref name="width"/>, <paramref name="height"/> are less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateWindow(int x, int y, int width, int height)
    {
        if (x < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.newwin(height, width, y, x)
                             .TreatNullAsError();

        return new(Terminal, handle);
    }

    /// <summary>
    /// Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window DuplicateWindow(Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        window.AssertNotDisposed();

        return new(Terminal, Terminal.Curses.dupwin(window.Handle)
                                     .TreatNullAsError());
    }

    /// <summary>
    /// Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        AssertNotDisposed();
        Terminal.Curses.doupdate()
                .TreatError();
    }
}
