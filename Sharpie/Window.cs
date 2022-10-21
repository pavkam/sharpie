namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window
{
    /// <summary>
    /// The Curses handle for the window.
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="windowHandle">The window handle.</param>
    internal Window(IntPtr windowHandle) => Handle = windowHandle;
}
