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
///    Defines the traits needed to implement a terminal. The only existing implementation of this interface is <see cref="Terminal"/> class.
/// </summary>
[PublicAPI]
public interface ITerminal
{
    /// <summary>
    ///     Checks whether the terminal has been disposed of and is no longer usable.
    /// </summary>
    public bool Disposed { get; }

    /// <summary>
    ///     Gets the terminal's baud rate.
    /// </summary>
    int BaudRate { get; }

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    IColorManager Colors { get; }

    /// <summary>
    ///     Provides access to the terminal's color management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    ISoftLabelKeyManager SoftLabelKeys { get; }
    
    /// <summary>
    ///     Returns the name of the terminal.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Returns the long description of the terminal.
    /// </summary>
    string? Description { get; }

    /// <summary>
    ///     Returns the version of the Curses library in use.
    /// </summary>
    string? CursesVersion { get; }

    /// <summary>
    ///     Gets the combination of supported terminal attributes.
    /// </summary>
    VideoAttribute SupportedAttributes { get; }

    /// <summary>
    ///     The screen instance. Use this property to access the entire screen functionality.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    IScreen Screen { get; }

    /// <summary>
    ///     The event pump instance that can be used to read events from the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    IEventPump Events { get; }

    /// <summary>
    ///     Specifies whether the terminal supports hardware line insert and delete.
    /// </summary>
    bool HasHardwareLineEditor { get; }

    /// <summary>
    ///     Specifies whether the terminal supports hardware character insert and delete.
    /// </summary>
    bool HasHardwareCharEditor { get; }

    /// <summary>
    ///     Gets the currently defined kill character. \0 is returned if none is defined.
    /// </summary>
    Rune? CurrentKillChar { get; }

    /// <summary>
    ///     Gets the currently defined erase character. \0 is returned if none is defined.
    /// </summary>
    Rune? CurrentEraseChar { get; }

    /// <summary>
    ///     Sets the terminal title.
    /// </summary>
    /// <param name="title">The title of the terminal.</param>
    void SetTitle(string title);

    /// <summary>
    ///     Attempts to notify the user with audio or flashing alert.
    /// </summary>
    /// <remarks>The actual notification depends on terminal support.</remarks>
    /// <param name="silent">The alert mode.</param>
    void Alert(bool silent);

    /// <summary>
    ///     Runs the application main loop and dispatches each event to <paramref name="eventAction" />.
    /// </summary>
    /// <param name="eventAction">The method to accept the events.</param>
    /// <param name="stopOnCtrlC">Set to <c>true</c> if CTRL+C should interrupt the main loop.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventAction" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if another <see cref="RunAsync"/> is already running.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    Task RunAsync(Func<Event, Task> eventAction, bool stopOnCtrlC = true);

    /// <summary>
    ///     Delegates an action to be executed on the main thread.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action" /> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    void Delegate(Func<Task> action);

    /// <summary>
    ///     Sets up a delayed action that is to be executed after some time.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="delayMillis">The delay in milliseconds.</param>
    /// <param name="state">User-supplied state object.</param>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <returns>The interval object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="delayMillis" /> is negative.</exception>
    IInterval Delay<TState>(Func<Terminal, TState?, Task> action, int delayMillis, TState? state);

    /// <summary>
    ///     Sets up a delayed action that is to be executed after some time.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="delayMillis">The delay in milliseconds.</param>
    /// <returns>The interval object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="delayMillis" /> is negative.</exception>
    IInterval Delay(Func<Terminal, Task> action, int delayMillis);

    /// <summary>
    ///     Sets up a delayed action that is to be executed after some time.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="intervalMillis">The interval in milliseconds.</param>
    /// <param name="immediate">If <c>true</c>, triggers the execution of the action immediately.</param>
    /// <param name="state">User-supplied state object.</param>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <returns>The interval object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="intervalMillis" /> is negative.</exception>
    IInterval Repeat<TState>(Func<Terminal, TState?, Task> action, int intervalMillis, bool immediate = false,
        TState? state = default);

    /// <summary>
    ///     Sets up a delayed action that is to be executed after some time.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="intervalMillis">The interval in milliseconds.</param>
    /// <param name="immediate">If <c>true</c>, triggers the execution of the action immediately.</param>
    /// <returns>The interval object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="intervalMillis" /> is negative.</exception>
    IInterval Repeat(Func<Terminal, Task> action, int intervalMillis, bool immediate = false);

    /// <summary>
    /// Enqueues a stop signal for the <see cref="RunAsync"/> method.
    /// </summary>
    /// <param name="wait">If <c>true</c>, waits until running completes.</param>
    void Stop(bool wait = false);
}
