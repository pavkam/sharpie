namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the possible strategies or replacing the contents of a window with the contents
/// of another window.
/// </summary>
[PublicAPI]
public enum ReplaceStrategy
{
    /// <summary>
    /// Overwrites the contents of the the window including blank space.
    /// </summary>
    Overwrite,
    /// <summary>
    /// Overwrites the contents of the the window excluding blank space (the contents of the destination are preserved in that case).
    /// </summary>
    Overlay,
}
