namespace Sharpie.Abstractions;

/// <summary>
///     Describes a timed interval.
/// </summary>
[PublicAPI]
public interface IInterval
{
    /// <summary>
    ///     Stops the interval from repeating/executing next.
    /// </summary>
    void Stop();
}

