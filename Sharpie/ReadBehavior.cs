namespace Sharpie;

/// <summary>
/// Defines different behaviours a read operation can take. These values can be combined together to change
/// the way <see cref="Window.TryRead"/> deals with input.
/// </summary>
public enum ReadBehavior
{
    /// <summary>
    /// Wait indefinitely (or the amount specified in <see cref="Terminal.ReadTimeoutMillis"/>.
    /// Also will wait <see cref="Terminal.EscapeSequenceWaitDelay"/> if an escape char is read.
    /// </summary>
    Wait = 0,
    /// <summary>
    /// Keypad sequence are treated in raw mode and not processed. The developer will be responsible with
    /// understanding them.
    /// </summary>
    RawKeypadSequences = 1,
    /// <summary>
    /// Escape sequences are not guaranteed to be processed when ESC character is encountered.
    /// </summary>
    RawEscapeSequences = RawKeypadSequences << 1,
    /// <summary>
    /// Do not wait for input at all. Returns immediately if nothing is in the queue.
    /// </summary>
    NoWait = RawEscapeSequences << 1,
}
