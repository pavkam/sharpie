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
///     Defines the function that provides the name of a key.
/// </summary>
/// <param name="keyCode">The key code.</param>
/// <returns>The key name (if any). Otherwise it returns <c>null</c>.</returns>
public delegate string? KeyNameFunc(uint keyCode);

/// <summary>
///     Defines the next delegate used to resolve key sequences.
/// </summary>
/// <param name="sequence">The sequence of events.</param>
/// <param name="nameFunc">The name provider function.</param>
/// <returns>The resolved key (if any) and the number of input events resolved.
/// If the count is <c>-1</c> it means nothing matches.</returns>
public delegate (KeyEvent? key, int count) ResolveEscapeSequenceFunc(IList<KeyEvent> sequence, KeyNameFunc nameFunc);

/// <summary>
/// Contains the definitions and implementations of input middlewares.
/// </summary>
public static class KeySequenceResolver
{
    /// <summary>
    /// Special character resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts key such as \n, \t, escape and etc. to their key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <returns>The resolved key (if any) and the number of input events resolved.
    /// If the count is <c>-1</c> it means nothing matches.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/>, <paramref name="nameFunc"/> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) SpecialCharacterResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var one = sequence.Count > 0 ? sequence[0] : null;
        var key = one switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and ControlCharacter.Escape } => new(Key.Escape, new(ControlCharacter.Null),
                nameFunc((uint) ch), one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Tab } => new(Key.Tab, new(ControlCharacter.Null),
                nameFunc((uint) CursesKey.Tab), one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and ControlCharacter.NewLine } => new(Key.Return, new(ControlCharacter.Null),
                nameFunc((uint) ch), one.Modifiers),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 0x7f } => new(Key.Backspace, new(ControlCharacter.Null),
                nameFunc((uint) CursesKey.Backspace), one.Modifiers),
            var _ => (KeyEvent?) null
        };

        return key != null ? (key, 1) : (null, 0);
    }

    /// <summary>
    /// Control+character resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts characters in the form of CTRL+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <returns>The resolved key (if any) and the number of input events resolved.
    /// If the count is <c>-1</c> it means nothing matches.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/> or <paramref name="nameFunc"/> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) ControlKeyResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var one = sequence.Count > 0 ? sequence[0] : null;
        return one switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and >= 1 and <= 26, Modifiers: var mod } => (
                new(Key.Character, new(ch + 'A' - 1), nameFunc((uint) ch + 'A' - 1), mod | ModifierKey.Ctrl), 1),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 0, Modifiers: var spMod } => (
                new(Key.Character, new(' '), nameFunc(' '), spMod | ModifierKey.Ctrl), 1),
            var _ => (null, 0)
        };
    }

    /// <summary>
    /// Alt+character/key resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts characters in the form of ALT+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <returns>The resolved key (if any) and the number of input events resolved.
    /// If the count is <c>-1</c> it means nothing matches.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/> or <paramref name="nameFunc"/> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) AltKeyResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }


        var one = sequence.Count > 0 ? sequence[0] : null;
        if (one is not { Key: Key.Character, Char.IsAscii: true, Char.Value: ControlCharacter.Escape, Modifiers: ModifierKey.None })
        {
            return (null, 0);
        }

        var two = sequence.Count > 1 ? sequence[1] : null;
        KeyEvent? key = two switch
        {
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 'f', Modifiers: var mod } => new(Key.KeypadRight, new(ControlCharacter.Null),
                nameFunc((uint) CursesKey.AltRight), mod | ModifierKey.Alt),
            { Key: Key.Character, Char.IsAscii: true, Char.Value: 'b', Modifiers: var mod  } => new(Key.KeypadLeft, new(ControlCharacter.Null),
                nameFunc((uint) CursesKey.AltLeft), mod | ModifierKey.Alt),
            { Key: Key.Unknown or Key.Escape } => null,
            { Key: var k and not Key.Character, Modifiers: var mod, Name: var n } => new(k, new(ControlCharacter.Null), n, mod | ModifierKey.Alt),
            { Char: var ch, Modifiers: var mod } => new(Key.Character, ch, nameFunc((uint) two.Char.Value), mod | ModifierKey.Alt),
            var _ => null
        };

        return key != null ? (key, 2) : (null, 1);
    }

    private static readonly Dictionary<int, (CursesKey, Key)> KeyMap =
        new()
        {
            { 'A', (CursesKey.Up, Key.KeypadUp) },
            { 'B', (CursesKey.Down, Key.KeypadDown) },
            { 'C', (CursesKey.Right, Key.KeypadRight) },
            { 'D', (CursesKey.Left, Key.KeypadLeft) },
            { 'E', (CursesKey.PageUp, Key.KeypadPageUp) },
            { 'F', (CursesKey.End, Key.KeypadEnd) },
            { 'G', (CursesKey.PageDown, Key.KeypadPageDown) },
            { 'H', (CursesKey.Home, Key.KeypadHome) }
        };
    
    /// <summary>
    /// Complex keypad resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts complex sequences such as CTRL/SHIFT/ALT+KeyPad to proper key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <returns>The resolved key (if any) and the number of input events resolved.
    /// If the count is <c>-1</c> it means nothing matches.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/> or <paramref name="nameFunc"/> is <c>null</c>.</exception>
    public static (KeyEvent? key, int count) KeyPadModifiersResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

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

        var (rawKey, key) = KeyMap[arrow];

        var mods = (ModifierKey) (csiModifier - '1');
        return (new(key, new(ControlCharacter.Null), nameFunc((uint) rawKey), mods), 4);
    }
}