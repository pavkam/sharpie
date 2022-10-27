namespace Sharpie;

using System.Drawing;
using JetBrains.Annotations;

/// <summary>
/// Defines a mouse move event.
/// </summary>
[PublicAPI]
public sealed class MouseMoveEvent: Event
{
    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="point">The location of the mouse.</param>
    public MouseMoveEvent(Point point): base(EventType.MouseMove) => Position = point;

    /// <summary>
    /// The mouse position.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// Returns the string representation of the event.
    /// </summary>
    /// <returns>The string.</returns>
    public override string ToString() => $"Mouse [{Position.X}, {Position.Y}]";
}
