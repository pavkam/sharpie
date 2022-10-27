namespace Sharpie;

using System.Drawing;
using JetBrains.Annotations;

/// <summary>
/// The terminal resized event.
/// </summary>
[PublicAPI]
public sealed class TerminalResizeEvent: Event
{
    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="size">The new size.</param>
    internal TerminalResizeEvent(Size size): base(EventType.TerminalResize) => Size = size;

    /// <summary>
    /// The new size of the terminal.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Returns the string representation of the event.
    /// </summary>
    /// <returns>The string.</returns>
    public override string ToString() => $"Resize [{Size.Width} x {Size.Height}]";
}
