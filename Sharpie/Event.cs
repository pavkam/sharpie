namespace Sharpie;
using JetBrains.Annotations;

/// <summary>
/// An event from the terminal.
/// </summary>
[PublicAPI]
public class Event
{
    /// <summary>
    /// The type of the event.
    /// </summary>
    public EventType Type { get; }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="type">The type of the event.</param>
    protected Event(EventType type) => Type = type;
}
