namespace Sharpie.Abstractions;

/// <summary>
///     Defines the traits needed to implement <see cref="EventPump" />.
/// </summary>
[PublicAPI]
public interface IEventPump
{
    /// <summary>
    ///     The terminal this pump belongs to.
    /// </summary>
    ITerminal Terminal { get; }

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <param name="surface">The surface to refresh during event processing.</param>
    /// <param name="cancellationToken">Cancellation token used to interrupt the process.</param>
    /// <returns>The event listening enumerable.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="surface" /> is <c>null</c>.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IEnumerable<Event> Listen(ISurface surface, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <param name="surface">The surface to refresh during event processing.</param>
    /// <returns>The event listening enumerable.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="surface" /> is <c>null</c>.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IEnumerable<Event> Listen(ISurface surface);

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token used to interrupt the process.</param>
    /// <returns>The event listening enumerable.</returns>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IEnumerable<Event> Listen(CancellationToken cancellationToken);

    /// <summary>
    ///     Gets an enumerable that is used to get enumerate events from Curses as they are generated.
    /// </summary>
    /// <returns>The event listening enumerable.</returns>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    IEnumerable<Event> Listen();

    /// <summary>
    ///     Registers a key sequence resolver into the input pipeline.
    /// </summary>
    /// <param name="resolver">The resolver to register.</param>
    /// <exception cref="ArgumentNullException">Thrown is <paramref name="resolver" /> is <c>null</c>.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Use(ResolveEscapeSequenceFunc resolver);

    /// <summary>
    ///     Checks if the screen has a given key sequence resolver registered.
    /// </summary>
    /// <param name="resolver">The resolver to check.</param>
    /// <returns><c>true</c> if the resolver is registered; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown is <paramref name="resolver" /> is <c>null</c>.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    bool Uses(ResolveEscapeSequenceFunc resolver);
}
