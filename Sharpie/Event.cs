namespace Sharpie;

using System.Drawing;
using System.Text;
using JetBrains.Annotations;

/// <summary>
/// Contains the details of a Curses event.
/// </summary>
[PublicAPI]
public sealed class Event
{
    /// <summary>
    /// The type of the event.
    /// </summary>
    public EventType Type { get; init; }

    public MouseButton MouseButton { get; init; }
    public MouseButtonState MouseButtonState { get; init; }
    public Point MousePosition { get; init; }

    public ModifierKey Modifier { get; init; }
    public Key Key { get; init; }
    public Rune Char { get; init; }
}
