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

namespace Sharpie.Backend;

/// <summary>
///     The base type for all back-end independent Curses events.
/// </summary>
public abstract record CursesEvent;

/// <summary>
///     Character input event.
/// </summary>
/// <param name="Char">The character that was read from the terminal.</param>
/// <param name="Name">The description of the character (backend-specific).</param>
/// <param name="Modifiers">The key modifiers.</param>
public sealed record CursesCharEvent(string? Name, char Char, ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Key input event.
/// </summary>
/// <param name="Key">The key that was read from the terminal.</param>
/// <param name="Name">The description of the character (backend-specific).</param>
/// <param name="Modifiers">The key modifiers.</param>
public sealed record CursesKeyEvent(string? Name, Key Key, ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Mouse input event.
/// </summary>
/// <param name="X">The mouse X coordinate.</param>
/// <param name="Y">The mouse Y coordinate.</param>
/// <param name="Button">The mouse button.</param>
/// <param name="State">The mouse button state.</param>
/// <param name="Modifiers">Key modifiers.</param>
public sealed record CursesMouseEvent(int X, int Y, MouseButton Button, MouseButtonState State,
    ModifierKey Modifiers): CursesEvent;

/// <summary>
///     Terminal resize event.
/// </summary>
public sealed record CursesResizeEvent: CursesEvent;
