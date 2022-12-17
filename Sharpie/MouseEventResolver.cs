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

namespace Sharpie;

/// <summary>
///     Resolves and normalizes mouse events. Class used internally.
/// </summary>
internal sealed class MouseEventResolver
{
    private MouseButton _previousButton = 0;
    private ModifierKey _previousMods = ModifierKey.None;
    private Point _previousPos = new(-1, -1);
    private MouseButtonState _previousState = 0;

    /// <summary>
    ///     Processes a given <see cref="MouseActionEvent" />.
    /// </summary>
    /// <param name="event">The mouse event to process.</param>
    /// <returns>A list of new events that resulted from breaking the given one.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="event" /> is <c>null</c>.</exception>
    public IEnumerable<Event> Process(MouseActionEvent @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        var output = new List<Event>();

        // Register a mouse move if needed.
        var moved = _previousPos != @event.Position;
        if (moved)
        {
            _previousPos = @event.Position;
            output.Add(new MouseMoveEvent(@event.Position));
        }

        switch (@event)
        {
            case { State: MouseButtonState.Clicked }:
                output.Add(new MouseActionEvent(@event.Position, @event.Button, MouseButtonState.Pressed,
                    @event.Modifiers));

                output.Add(new MouseActionEvent(@event.Position, @event.Button, MouseButtonState.Released,
                    @event.Modifiers));

                _previousState = MouseButtonState.Released;
                _previousButton = @event.Button;
                _previousMods = @event.Modifiers;

                break;
            case { State: MouseButtonState.Released } when _previousState == MouseButtonState.Pressed:
                _previousState = MouseButtonState.Released;
                _previousMods = @event.Modifiers;

                output.Add(new MouseActionEvent(@event.Position, _previousButton, MouseButtonState.Released,
                    @event.Modifiers));

                break;
            case { State: MouseButtonState.Released } when _previousState != MouseButtonState.Pressed:
                break;
            case { State: MouseButtonState.Pressed } when _previousState == MouseButtonState.Pressed && !moved:
                _previousMods = @event.Modifiers;
                _previousButton = @event.Button;

                output.Add(@event);

                break;
            case { State: var state, Modifiers: var mods } when state != _previousState || mods != _previousMods:
                // All other cases
                _previousState = @event.State;
                _previousButton = @event.Button;
                _previousMods = @event.Modifiers;

                output.Add(@event);

                break;
        }

        return output;
    }

    /// <summary>
    ///     Processes a given <see cref="MouseMoveEvent" />.
    /// </summary>
    /// <param name="event">The mouse event to process.</param>
    /// <returns>A list of new events that resulted from breaking the given one.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="event" /> is <c>null</c>.</exception>
    public IEnumerable<Event> Process(MouseMoveEvent @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        // Silence excessive moves if required.
        if (_previousPos == @event.Position)
        {
            return Array.Empty<Event>();
        }

        // Save the position and return.
        _previousPos = @event.Position;
        return new[] { @event };
    }
}
