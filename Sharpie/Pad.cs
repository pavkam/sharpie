namespace Sharpie;

using System.Drawing;
using JetBrains.Annotations;

/// <summary>
/// Represents pad which is a special type of window.
/// </summary>
[PublicAPI]
public sealed class Pad: Window
{
    /// <summary>
    /// Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="terminal">The curses functionality provider.</param>
    /// <param name="parent">The parent window or pad.</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Pad(Terminal terminal, Window parent, IntPtr windowHandle): base(terminal, parent, windowHandle) { }

    /// <inheritdoc cref="Window.ImmediateRefresh"/>
    /// <remarks>
    /// This functionality is disabled in the pads. Any call to set the value to <c>true</c> will
    /// fail with error.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws on write.</exception>
    public override bool ImmediateRefresh
    {
        get => base.ImmediateRefresh;
        set => throw new NotSupportedException("Pads cannot have immediate refresh enabled.");
    }

    /// <inheritdoc cref="Window.Refresh"/>
    /// <remarks>
    /// This functionality is disabled in the pads. Use the overloaded version of this method.
    /// </remarks>
    /// <exception cref="NotSupportedException">Always throws.</exception>
    public override void Refresh(bool batch, bool entireScreen)
    {
        throw new NotSupportedException("Pads cannot be refreshed in this way.");
    }

    /// <summary>
    /// Refreshes the pad by synchronizing it to the terminal screen.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <param name="entireScreen">If <c>true</c>, when this refresh happens, the entire screen is redrawn.</param>
    /// <param name="rect">The rectangle of the pad to place onto the screen.</param>
    /// <param name="screenPos">The point on the screen to place that rectangle.</param>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Refresh(bool batch, bool entireScreen, Rectangle rect, Point screenPos)
    {
        if (!IsRectangleWithin(rect))
        {
            throw new ArgumentOutOfRangeException(nameof(rect));
        }

        var destRect = new Rectangle(screenPos, new(rect.Bottom - rect.Top, rect.Right - rect.Left));
        if (!Terminal.Screen.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(screenPos));
        }

        if (entireScreen)
        {
            Terminal.Curses.clearok(Handle, true)
                    .Check(nameof(Terminal.Curses.clearok));
        }

        if (batch)
        {
            Terminal.Curses.pnoutrefresh(Handle, rect.Top, rect.Left, destRect.Top, destRect.Left,
                        destRect.Bottom, destRect.Right)
                    .Check(nameof(Terminal.Curses.pnoutrefresh));
        } else
        {
            Terminal.Curses.prefresh(Handle, rect.Top, rect.Left, destRect.Top, destRect.Left,
                        destRect.Bottom, destRect.Right)
                    .Check(nameof(Terminal.Curses.prefresh));
        }
    }
}
