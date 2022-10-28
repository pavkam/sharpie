namespace Sharpie;

using System.Drawing;
using System.Text;
using JetBrains.Annotations;

/// <summary>
///     Contains the details of a Curses event.
/// </summary>
[PublicAPI]
public sealed class MouseActionEvent: Event
{
    /// <summary>
    ///     Creates a new instance of the class.
    /// </summary>
    /// <param name="point">The location of the mouse.</param>
    /// <param name="button">The actioned button.</param>
    /// <param name="state">The button state.</param>
    /// <param name="modifiers">The key modifiers.</param>
    internal MouseActionEvent(Point point, MouseButton button, MouseButtonState state, ModifierKey modifiers): base(
        EventType.MouseAction)
    {
        Position = point;
        Button = button;
        State = state;
        Modifiers = modifiers;
    }

    /// <summary>
    ///     The button that was actioned.
    /// </summary>
    public MouseButton Button { get; init; }

    /// <summary>
    ///     The state of the action.
    /// </summary>
    public MouseButtonState State { get; init; }

    /// <summary>
    ///     Modifier keys that were present at the time of the action.
    /// </summary>
    public ModifierKey Modifiers { get; init; }

    /// <summary>
    ///     The mouse position at the time of the action.
    /// </summary>
    public Point Position { get; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString()
    {
        var modifiers = new StringBuilder();
        if (Modifiers.HasFlag(ModifierKey.Ctrl))
        {
            modifiers.Append("CTRL-");
        }

        if (Modifiers.HasFlag(ModifierKey.Shift))
        {
            modifiers.Append("SHIFT-");
        }

        if (Modifiers.HasFlag(ModifierKey.Alt))
        {
            modifiers.Append("ALT-");
        }

        return $"Mouse [{modifiers}{Button}-{State} at {Position.X}, {Position.Y}]";
    }

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is MouseActionEvent mae &&
        mae.Button == Button &&
        mae.State == State &&
        mae.Modifiers == Modifiers &&
        mae.Position == Position &&
        mae.Type == Type &&
        obj.GetType() == GetType();

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Button, State, Modifiers, Position, Type);
}
