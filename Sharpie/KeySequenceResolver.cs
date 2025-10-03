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
///     Defines the next delegate used to resolve key sequences.
/// </summary>
/// <param name="sequence">The sequence of events.</param>
/// <returns>
///     The resolved key (if any) and the number of input events resolved.
///     If the count is <c>-1</c> it means nothing matches.
/// </returns>
public delegate (KeyEvent? key, int count) ResolveEscapeSequenceFunc(IReadOnlyList<KeyEvent> sequence);

/// <summary>
///     Contains the definitions and implementations of input middlewares.
/// </summary>
public static class KeySequenceResolver
{
    private static readonly Dictionary<int, Key> _keyMap = new()
    {
        { 'A', Key.KeypadUp },
        { 'B', Key.KeypadDown },
        { 'C', Key.KeypadRight },
        { 'D', Key.KeypadLeft },
        { 'E', Key.KeypadPageUp },
        { 'F', Key.KeypadEnd },
        { 'G', Key.KeypadPageDown },
        { 'H', Key.KeypadHome }
    };

    /// <summary>
    ///     Special character resolver middleware.
    /// </summary>
    /// <remarks>
    ///     This middleware converts key such as \n, \t, escape and etc. to their key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <returns>
    ///     The resolved key (if any) and the number of input events resolved.
    ///     If the count is <c>-1</c> it means nothing matches.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sequence" /> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) SpecialCharacterResolver(IReadOnlyList<KeyEvent> sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var one = sequence.Count > 0 ? sequence[0] : null;
        var key = one switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Escape } => new(Key.Escape,
                new(ControlCharacter.Null), null, one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Tab } => new(Key.Tab,
                new(ControlCharacter.Null), null, one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.NewLine } => new(Key.Return,
                new(ControlCharacter.Null), null, one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 0x7f } => new(Key.Backspace,
                new(ControlCharacter.Null), null, one.Modifiers),
            var _ => (KeyEvent?) null
        };

        return key != null ? (key, 1) : (null, 0);
    }

    /// <summary>
    ///     Control+character resolver middleware.
    /// </summary>
    /// <remarks>
    ///     This middleware converts characters in the form of CTRL+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <returns>
    ///     The resolved key (if any) and the number of input events resolved.
    ///     If the count is <c>-1</c> it means nothing matches.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sequence" /> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) ControlKeyResolver(IReadOnlyList<KeyEvent> sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var one = sequence.Count > 0 ? sequence[0] : null;
        return one switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and >= 1 and <= 26, Modifiers: var mod } => (
                new(Key.Character, new(ch + 'A' - 1), null, mod | ModifierKey.Ctrl), 1),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 0, Modifiers: var spMod } => (
                new(Key.Character, new(' '), null, spMod | ModifierKey.Ctrl), 1),
            var _ => (null, 0)
        };
    }

    /// <summary>
    ///     Alt+character/key resolver middleware.
    /// </summary>
    /// <remarks>
    ///     This middleware converts characters in the form of ALT+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <returns>
    ///     The resolved key (if any) and the number of input events resolved.
    ///     If the count is <c>-1</c> it means nothing matches.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sequence" /> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) AltKeyResolver(IReadOnlyList<KeyEvent> sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }


        var one = sequence.Count > 0 ? sequence[0] : null;
        if (one is not
            {
                Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Escape, Modifiers: ModifierKey.None
            })
        {
            return (null, 0);
        }

        var two = sequence.Count > 1 ? sequence[1] : null;
        KeyEvent? key = two switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 'f', Modifiers: var mod } => new(Key.KeypadRight,
                new(ControlCharacter.Null), null, mod | ModifierKey.Alt),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 'b', Modifiers: var mod } => new(Key.KeypadLeft,
                new(ControlCharacter.Null), null, mod | ModifierKey.Alt),
            { Key: Key.Unknown or Key.Escape } => null,
            { Key: var k and not Key.Character, Modifiers: var mod, Name: var n } => new(k, new(ControlCharacter.Null),
                n, mod | ModifierKey.Alt),
            { Char: var ch, Modifiers: var mod } => new(Key.Character, ch, null, mod | ModifierKey.Alt),
            var _ => null
        };

        return key != null ? (key, 2) : (null, 1);
    }

    /// <summary>
    ///     Complex keypad resolver middleware.
    /// </summary>
    /// <remarks>
    ///     This middleware converts complex sequences such as CTRL/SHIFT/ALT+KeyPad to proper key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <returns>
    ///     The resolved key (if any) and the number of input events resolved.
    ///     If the count is <c>-1</c> it means nothing matches.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sequence" /> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) KeyPadModifiersResolver(IReadOnlyList<KeyEvent> sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var one = sequence.Count > 0 ? sequence[0] : null;
        if (one is not { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Escape })
        {
            return (null, 0);
        }

        var two = sequence.Count > 1 ? sequence[1] : null;
        if (two is not { Key: Key.Character, Char.IsAscii: true, Char.Value: 'O', Modifiers: ModifierKey.None })
        {
            return (null, 1);
        }

        var three = sequence.Count > 2 ? sequence[2] : null;
        if (three is not { Key: Key.Character, Char.IsAscii: true, Char.Value: var csiModifier and >= '1' and <= '9' })
        {
            return (null, 2);
        }

        var four = sequence.Count > 3 ? sequence[3] : null;
        if (four is not { Key: Key.Character, Char.IsAscii: true, Char.Value: var arrow and >= 'A' and <= 'H' })
        {
            return (null, 3);
        }

        var mods = (ModifierKey) (csiModifier - '1');
        return (new(_keyMap[arrow], new(ControlCharacter.Null), null, mods), 4);
    }
}
