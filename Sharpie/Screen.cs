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
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(ICursesProvider cursesProvider, IntPtr windowHandle): base(cursesProvider, windowHandle, 0, 0, 0, 0) { }

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

        CursesProvider.AssertNotDisposed();
        var handle = CursesProvider.newwin(height, width, y, x)
                                   .TreatNullAsError();

        return new(CursesProvider, handle, x, y, width, height);
    }
}
