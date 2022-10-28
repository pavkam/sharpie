namespace Sharpie;

using System.Drawing;
using JetBrains.Annotations;

/// <summary>
///     Defines a mouse move event.
/// </summary>
[PublicAPI]
public sealed class MouseMoveEvent: Event
{
    /// <summary>
    ///     Creates a new instance of the class.
    /// </summary>
    /// <param name="point">The location of the mouse.</param>
    public MouseMoveEvent(Point point): base(EventType.MouseMove) => Position = point;

    /// <summary>
    ///     The mouse position.
    /// </summary>
    public Point Position { get; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() => $"Mouse [{Position.X}, {Position.Y}]";

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is MouseMoveEvent mae && mae.Position == Position && mae.Type == Type && obj.GetType() == GetType();

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Position, Type);
}
