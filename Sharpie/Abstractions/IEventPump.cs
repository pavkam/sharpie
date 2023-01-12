/*
Copyright (c) 2022, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

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
