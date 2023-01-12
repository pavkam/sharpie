/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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
///     Contains the details of a Curses event.
/// </summary>
[PublicAPI, DebuggerDisplay("{ToString(), nq}")]
public sealed class MouseActionEvent: Event
{
    /// <summary>
    ///     Creates a new instance of the class.
    /// </summary>
    /// <param name="point">The location of the mouse.</param>
    /// <param name="button">The actioned button.</param>
    /// <param name="state">The button state.</param>
    /// <param name="modifiers">The key modifiers.</param>
    internal MouseActionEvent(Point point, MouseButton button, MouseButtonState state, ModifierKey modifiers): base(
        EventType.MouseAction)
    {
        Position = point;
        Button = button;
        State = state;
        Modifiers = modifiers;
    }

    /// <summary>
    ///     The button that was actioned.
    /// </summary>
    public MouseButton Button { get; }

    /// <summary>
    ///     The state of the action.
    /// </summary>
    public MouseButtonState State { get; }

    /// <summary>
    ///     Modifier keys that were present at the time of the action.
    /// </summary>
    public ModifierKey Modifiers { get; }

    /// <summary>
    ///     The mouse position at the time of the action.
    /// </summary>
    public Point Position { get; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString()
    {
        var modifiers = new StringBuilder();
        if (Modifiers.HasFlag(ModifierKey.Ctrl))
        {
            modifiers.Append("CTRL-");
        }

        if (Modifiers.HasFlag(ModifierKey.Shift))
        {
            modifiers.Append("SHIFT-");
        }

        if (Modifiers.HasFlag(ModifierKey.Alt))
        {
            modifiers.Append("ALT-");
        }

        return $"Mouse {modifiers}{Button}-{State} @ {Position.X}x{Position.Y}";
    }

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is MouseActionEvent mae &&
        mae.Button == Button &&
        mae.State == State &&
        mae.Modifiers == Modifiers &&
        mae.Position == Position &&
        mae.Type == Type &&
        obj.GetType() == GetType();

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Button, State, Modifiers, Position, Type);
}
