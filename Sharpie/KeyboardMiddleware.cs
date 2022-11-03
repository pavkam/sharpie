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
///     Defines the next delegate in the middleware chain.
/// </summary>
/// <param name="sequence">The sequence of events.</param>
/// <param name="nameFunc">The name provider function.</param>
public delegate void KeyboardMiddlewareNextFunc(IList<KeyEvent> sequence, KeyNameFunc nameFunc);

/// <summary>
///     Defines the delegate used as a keyboard middleware.
/// </summary>
/// <remarks>
/// Implementors of this delegate can choose to call <paramref name="next"/> at any moment. Or can choose not to call it at all.
/// </remarks>
/// <param name="sequence">The sequence of events.</param>
/// <param name="nameFunc">The name provider function.</param>
/// <param name="next">The next middleware in the chain.</param>
public delegate void KeyboardMiddlewareFunc(IList<KeyEvent> sequence, KeyNameFunc nameFunc,
    KeyboardMiddlewareNextFunc next);

/// <summary>
/// Contains the definitions and implementations of input middlewares.
/// </summary>
public static class KeyboardMiddleware
{
    /// <summary>
    /// Special character resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts key such as \n, \t, escape and etc. to their key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/>, <paramref name="nameFunc"/> or <paramref name="next"/> is <c>null</c>.</exception>
    public static void SpecialCharacterResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc,
        KeyboardMiddlewareNextFunc next)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        // Call the next middleware to to its job first.
        next(sequence, nameFunc);

        if (sequence.Count == 1)
        {
            var single = sequence.Single();
            var @override = single switch
            {
                { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and 0x1b } => new(Key.Escape, new('\0'),
                    nameFunc((uint) ch), single.Modifiers),
                { Key: Key.Character, Char.IsAscii: true, Char.Value: '\t' } => new(Key.Tab, new('\0'),
                    nameFunc((uint) CursesKey.Tab), single.Modifiers),
                { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and '\n' } => new(Key.Return, new('\0'),
                    nameFunc((uint) ch), single.Modifiers),
                { Key: Key.Character, Char.IsAscii: true, Char.Value: 0x7f } => new(Key.Backspace, new('\0'),
                    nameFunc((uint) CursesKey.Backspace), single.Modifiers),
                { Key: Key.Character, Char.IsAscii: true, Char.Value: 0 } => new(Key.Character, new(' '), nameFunc(' '),
                    single.Modifiers | ModifierKey.Ctrl),
                var _ => (KeyEvent?) null
            };

            // Replace the sequence of needed.
            if (@override != null)
            {
                sequence.Clear();
                sequence.Add(@override);
            }
        }
    }

    /// <summary>
    /// Control+character resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts characters in the form of CTRL+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/>, <paramref name="nameFunc"/> or <paramref name="next"/> is <c>null</c>.</exception>
    public static void ControlKeyResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc,
        KeyboardMiddlewareNextFunc next)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        // Call the next middleware to to its job first.
        next(sequence, nameFunc);

        if (sequence.Count == 1 &&
            sequence.Single() is
                { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and >= 1 and <= 26, Modifiers: var mod })
        {
            // Replace the sequence of needed.
            var @override = new KeyEvent(Key.Character, new(ch + 'A' - 1), nameFunc((uint) ch + 'A' - 1),
                mod | ModifierKey.Ctrl);

            sequence.Clear();
            sequence.Add(@override);
        }
    }

    /// <summary>
    /// Alt+character/key resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts characters in the form of ALT+.. to proper keys.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/>, <paramref name="nameFunc"/> or <paramref name="next"/> is <c>null</c>.</exception>
    public static void AltKeyResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc, KeyboardMiddlewareNextFunc next)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        // Call the next middleware to to its job first.
        next(sequence, nameFunc);

        if (sequence.Count == 2 && (
                sequence[0] is { Key: Key.Escape, Modifiers: ModifierKey.None } || 
                sequence[0] is { Key: Key.Character, Char.IsAscii: true, Char.Value: 0x1b, Modifiers: ModifierKey.None }))
        {
            var second = sequence[1];
            KeyEvent? @override = second switch
            {
                { Key: Key.Character, Char.IsAscii: true, Char.Value: 'f' } => new(Key.KeypadRight, new('\0'),
                    nameFunc((uint) CursesKey.AltRight), second.Modifiers | ModifierKey.Alt),
                { Key: Key.Character, Char.IsAscii: true, Char.Value: 'b' } => new(Key.KeypadLeft, new('\0'),
                    nameFunc((uint) CursesKey.AltLeft), second.Modifiers | ModifierKey.Alt),
                { Key: Key.Unknown or Key.Escape } => null,
                { Key: not Key.Character } => new(second.Key, second.Char, second.Name,
                    second.Modifiers | ModifierKey.Alt),
                var _ => new(Key.Character, second.Char, nameFunc((uint) second.Char.Value),
                    second.Modifiers | ModifierKey.Alt)
            };

            if (@override != null)
            {
                sequence.Clear();
                sequence.Add(@override);
            }
        }
    }

    /// <summary>
    /// Complex keypad resolver middleware.
    /// </summary>
    /// <remarks>
    /// This middleware converts complex sequences such as CTRL/SHIFT/ALT+KeyPad to proper key representations.
    /// </remarks>
    /// <param name="sequence">The input sequence.</param>
    /// <param name="nameFunc">The name resolver function.</param>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Either <paramref name="sequence"/>, <paramref name="nameFunc"/> or <paramref name="next"/> is <c>null</c>.</exception>
    public static void KeyPadModifiersResolver(IList<KeyEvent> sequence, KeyNameFunc nameFunc,
        KeyboardMiddlewareNextFunc next)
    {
        if (nameFunc == null)
        {
            throw new ArgumentNullException(nameof(nameFunc));
        }

        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        // Call the next middleware to to its job first.
        next(sequence, nameFunc);

        if (sequence.Count == 3)
        {
            KeyEvent? @override = null;

            if (sequence[0] is
                    { Key: Key.Character, Char.IsAscii: true, Char.Value: 'O', Modifiers: ModifierKey.Alt } &&
                sequence[1] is
                    { Key: Key.Character, Char.IsAscii: true, Char.Value: var modCh and >= '1' and <= '9' } &&
                sequence[2] is { Key: Key.Character, Char.IsAscii: true, Char.Value: var ch and >= 'A' and <= 'H' })
            {
                var (rawKey, key) = (char) ch switch
                {
                    'A' => (CursesKey.Up, Key.KeypadUp),
                    'B' => (CursesKey.Down, Key.KeypadDown),
                    'C' => (CursesKey.Right, Key.KeypadRight),
                    'D' => (CursesKey.Left, Key.KeypadLeft),
                    'E' => (CursesKey.PageUp, Key.KeypadPageUp),
                    'F' => (CursesKey.End, Key.KeypadEnd),
                    'G' => (CursesKey.PageDown, Key.KeypadPageDown),
                    'H' => (CursesKey.Home, Key.KeypadHome),
                    var _ => (CursesKey.Yes, Key.Unknown)
                };

                var mods = (ModifierKey) (modCh - '1');
                @override = new(key, new('\0'), nameFunc((uint) rawKey), mods);
            }

            if (@override != null)
            {
                sequence.Clear();
                sequence.Add(@override);
            }
        }
    }
}