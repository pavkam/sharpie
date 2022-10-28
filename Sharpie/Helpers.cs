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

using Curses;

/// <summary>
///     Internal helper routines.
/// </summary>
public static class Helpers
{
    private const int CursesErrorResult = -1;

    /// <summary>
    ///     Checks if a given code shows a failure.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The result of the failure check.</returns>
    public static bool Failed(this int code) => code == CursesErrorResult;

    /// <summary>
    ///     Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="code">The return code.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="code" /> value.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="code" /> indicates an error.</exception>
    internal static int Check(this int code, string operation, string message)
    {
        if (code == CursesErrorResult)
        {
            throw new CursesException(operation, message);
        }

        return code;
    }

    /// <summary>
    ///     Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="ptr">The return pointer.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="ptr" /> value.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="ptr" /> is zero.</exception>
    internal static IntPtr Check(this IntPtr ptr, string operation, string message)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new CursesException(operation, message);
        }

        return ptr;
    }

    /// <summary>
    ///     Converts millis to tenths of a second by rounding up.
    /// </summary>
    /// <param name="value">The millis.</param>
    /// <returns>The value in 100s of millis.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="value"/> is less than zero.</exception>
    internal static int ConvertMillisToTenths(int value)
    {
        switch (value)
        {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(value));
            case 0:
                return 0;
            default:
            {
                var hundreds = value / 100 + (value % 100 > 0 ? 1 : 0);
                return Math.Min(255, hundreds);
            }
        }
    }

    /// <summary>
    /// Converts a given rune to a complex character.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="rune">The rune to convert.</param>
    /// <param name="style">The style to apply.</param>
    /// <returns>The complex character.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="rune" /> failed to convert to a complex char.</exception>
    public static ComplexChar ToComplexChar(this ICursesProvider curses, Rune rune, Style style)
    {
        // Convert the special characters into Unicode.
        if (rune.IsAscii && rune.Value != '\n' && rune.Value != '\b' && rune.Value != '\t' && rune.Value <= 0x1F ||
            rune.Value is >= 0X7F and <= 0x9F)
        {
            rune = new(rune.Value + 0x2400);
        }

        // Use Curses to encode the characters.
        curses.setcchar(out var @char, rune.ToString(), (uint) style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(curses.setcchar), "Failed to convert string to complex character.");

        return @char;
    }

    /// <summary>
    /// Converts a complex char into a rune and its style.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="char">The char to breakdown.</param>
    /// <returns>The rune and the style.</returns>
    public static (Rune rune, Style style) FromComplexChar(this ICursesProvider curses, ComplexChar @char)
    {
        // Use Curses to decode the characters. Assume 10 characters is enough in the string.

        var builder = new StringBuilder(10);
        curses.getcchar(@char, builder, out var attrs, out var colorPair, IntPtr.Zero)
              .Check(nameof(curses.getcchar), "Failed to deconstruct the complex character.");

        return (Rune.GetRuneAt(builder.ToString(), 0),
            new() { Attributes = (VideoAttribute) attrs, ColorMixture = new() { Handle = colorPair } });
    }
}
