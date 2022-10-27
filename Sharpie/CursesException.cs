namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// A Curses exception.
/// </summary>
[PublicAPI]
public sealed class CursesException: Exception
{
    /// <summary>
    /// The operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Creates a new instance of this exception.
    /// </summary>
    /// <param name="operation">The failed operation.</param>
    /// <param name="message">The message.</param>
    internal CursesException(string operation, string message): base($"The call to {operation} failed: {message}") =>
        Operation = operation;
}
