namespace Sharpie;

/// <summary>
/// Defines the possible strategies of clearing contents of a window.
/// </summary>
public enum ClearStrategy
{
    /// <summary>
    /// Clears the entire window.
    /// </summary>
    Full,
    /// <summary>
    /// Clears the contents of the line from the position of the caret down to the end of the window.
    /// </summary>
    FullFromCaret,
    /// <summary>
    /// Clears the contents of the line from the position of the caret.
    /// </summary>
    LineFromCaret,
}
