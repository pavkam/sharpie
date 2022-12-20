namespace Sharpie.Abstractions;

/// <summary>
///     Describes a timed interval.
/// </summary>
[PublicAPI]
public interface IInterval: IDisposable
{
    /// <summary>
    ///     Stops the interval from repeating/executing at the next tick.
    /// </summary>
    void Stop() => Dispose();
}
