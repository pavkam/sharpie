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
///     Defines the possible key types.
/// </summary>
[PublicAPI]
public enum Key: uint
{
    /// <summary>
    ///     A simple character key.
    /// </summary>
    Character,

    /// <summary>
    ///     Unknown key.
    /// </summary>
    Unknown,

    /// <summary>
    ///     The Return/Enter key.
    /// </summary>
    Return,

    /// <summary>
    ///     The Escape key.
    /// </summary>
    Escape,

    /// <summary>
    ///     The delete key.
    /// </summary>
    Delete,

    /// <summary>
    ///     Backspace key.
    /// </summary>
    Backspace,

    /// <summary>
    ///     Arrow up key.
    /// </summary>
    KeypadUp,

    /// <summary>
    ///     Arrow down key.
    /// </summary>
    KeypadDown,

    /// <summary>
    ///     Arrow left key.
    /// </summary>
    KeypadLeft,

    /// <summary>
    ///     Arrow right key.
    /// </summary>
    KeypadRight,

    /// <summary>
    ///     Page up key.
    /// </summary>
    KeypadPageUp,

    /// <summary>
    ///     Page down key.
    /// </summary>
    KeypadPageDown,

    /// <summary>
    ///     Home key.
    /// </summary>
    KeypadHome,

    /// <summary>
    ///     End key.
    /// </summary>
    KeypadEnd,

    /// <summary>
    ///     Delete character key.
    /// </summary>
    DeleteChar,

    /// <summary>
    ///     Insert character key.
    /// </summary>
    InsertChar,

    /// <summary>
    ///     Tab key.
    /// </summary>
    Tab,

    /// <summary>
    ///     F1 key.
    /// </summary>
    F1,

    /// <summary>
    ///     F2 key.
    /// </summary>
    F2,

    /// <summary>
    ///     F3 key.
    /// </summary>
    F3,

    /// <summary>
    ///     F4 key.
    /// </summary>
    F4,

    /// <summary>
    ///     F5 key.
    /// </summary>
    F5,

    /// <summary>
    ///     F6 key.
    /// </summary>
    F6,

    /// <summary>
    ///     F7 key.
    /// </summary>
    F7,

    /// <summary>
    ///     F8 key.
    /// </summary>
    F8,

    /// <summary>
    ///     F9 key.
    /// </summary>
    F9,

    /// <summary>
    ///     F10 key.
    /// </summary>
    F10,

    /// <summary>
    ///     F11 key.
    /// </summary>
    F11,

    /// <summary>
    ///     F12 key.
    /// </summary>
    F12
}

