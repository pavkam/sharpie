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
///     Defines a key event.
/// </summary>
[PublicAPI, DebuggerDisplay("{ToString(), nq}")]
public sealed class KeyEvent: Event
{
    /// <summary>
    ///     Creates a new instance of the class.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    /// <param name="char">The char that was pressed if it was not a key.</param>
    /// <param name="keyName">The name of the key.</param>
    /// <param name="modifiers">The modifiers at the time of the press.</param>
    internal KeyEvent(Key key, Rune @char, string? keyName, ModifierKey modifiers) : base(EventType.KeyPress)
    {
        Key = key;
        Char = @char;
        Modifiers = modifiers;
        Name = keyName;
    }

    /// <summary>
    ///     The key that was pressed. If the value is <see cref="Key.Character" /> then <see cref="Char" /> contains
    ///     the actual code of the key.
    /// </summary>
    public Key Key
    {
        get;
    }

    /// <summary>
    ///     The character that was pressed. Only valid if <see cref="Key" /> is <see cref="Key.Character" />.
    /// </summary>
    public Rune Char
    {
        get;
    }

    /// <summary>
    ///     The printable name of the key.
    /// </summary>
    public string? Name
    {
        get;
    }

    /// <summary>
    ///     The modifier keys that were present at the time.
    /// </summary>
    public ModifierKey Modifiers
    {
        get;
    }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString()
    {
        var modifiers = new StringBuilder();
        if (Modifiers.HasFlag(ModifierKey.Ctrl))
        {
            _ = modifiers.Append("CTRL-");
        }

        if (Modifiers.HasFlag(ModifierKey.Shift))
        {
            _ = modifiers.Append("SHIFT-");
        }

        if (Modifiers.HasFlag(ModifierKey.Alt))
        {
            _ = modifiers.Append("ALT-");
        }

        var fmtName = string.IsNullOrEmpty(Name) ? Name : $" ({Name})";
        return Key == Key.Character
            ? $"{modifiers}'{Char}'{fmtName}"
            : $"{modifiers}{Key}{fmtName}";
    }

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is KeyEvent ke &&
        ke.Char == Char &&
        ke.Key == Key &&
        ke.Modifiers == Modifiers &&
        ke.Name == Name &&
        ke.Type == Type &&
        obj.GetType() == GetType();

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Char, Key, Modifiers, Name, Type);
}
