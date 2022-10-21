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
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(IntPtr windowHandle) : base(windowHandle)
    {
    }
}
