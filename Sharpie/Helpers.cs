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
///     Internal helper routines.
/// </summary>
internal static class Helpers
{
    public const int CursesErrorResult = -1;

    /// <summary>
    ///     Gets the raw value of a given <see cref="ComplexChar" /> and validates its compatibility with the backend.
    /// </summary>
    /// <param name="char">The character.</param>
    /// <typeparam name="T">The type of the raw payload.</typeparam>
    /// <returns>The raw payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="char" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="char" /> is not valid in the backend.</exception>
    public static T GetRawValue<T>(this ComplexChar @char)
    {
        return @char == null
            ? throw new ArgumentNullException(nameof(@char))
            : @char.Raw is T raw ? raw : throw new ArgumentException("Incompatible complex character", nameof(@char));
    }

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
    /// <exception cref="CursesOperationException">Thrown if <paramref name="code" /> indicates an error.</exception>
    public static int Check(this int code, string operation, string message) => code == CursesErrorResult ? throw new CursesOperationException(operation, message) : code;

    /// <summary>
    ///     Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="ptr">The return pointer.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="ptr" /> value.</returns>
    /// <exception cref="CursesOperationException">Thrown if <paramref name="ptr" /> is zero.</exception>
    public static IntPtr Check(this IntPtr ptr, string operation, string message) => ptr == IntPtr.Zero ? throw new CursesOperationException(operation, message) : ptr;

    /// <summary>
    ///     Converts a given rune to a complex character.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="rune">The rune to convert.</param>
    /// <param name="style">The style to apply.</param>
    /// <returns>The complex character.</returns>
    /// <exception cref="CursesOperationException">Thrown if <paramref name="rune" /> failed to convert to a complex char.</exception>
    public static ComplexChar ToComplexChar(this ICursesBackend curses, Rune rune, Style style)
    {
        // Convert the special characters into Unicode.
        if (rune.IsAscii &&
            rune.Value != ControlCharacter.NewLine &&
            rune.Value != '\b' &&
            rune.Value != ControlCharacter.Tab &&
            rune.Value <= 0x1F ||
            rune.Value is >= 0X7F and <= 0x9F)
        {
            rune = new(rune.Value + 0x2400);
        }

        // Use Curses to encode the characters.
        _ = curses.setcchar(out var @char, rune.ToString(), style.Attributes, style.ColorMixture.Handle, IntPtr.Zero)
              .Check(nameof(curses.setcchar), "Failed to convert string to complex character.");

        return @char;
    }

    /// <summary>
    ///     Converts a complex char into a rune and its style.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="char">The char to breakdown.</param>
    /// <returns>The rune and the style.</returns>
    public static (Rune rune, Style style) FromComplexChar(this ICursesBackend curses, ComplexChar @char)
    {
        // Use Curses to decode the characters. Assume 10 characters is enough in the string.

        var builder = new StringBuilder(10);
        _ = curses.getcchar(@char, builder, out var attrs, out var colorPair, IntPtr.Zero)
              .Check(nameof(curses.getcchar), "Failed to deconstruct the complex character.");

        return (Rune.GetRuneAt(builder.ToString(), 0),
            new()
            {
                Attributes = attrs,
                ColorMixture = new()
                {
                    Handle = colorPair
                }
            });
    }

    /// <summary>
    ///     Enumerates a given interval using 1/2s.
    /// </summary>
    /// <param name="start">The start of the interval.</param>
    /// <param name="count">The length of the interval.</param>
    /// <returns>An enumerable that returns the halves.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count" /> is negative.</exception>
    public static IEnumerable<(int coord, bool start)> EnumerateInHalves(float start, float count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var halves = (int) Math.Floor(count * 2);
        var allHalves = (int) Math.Floor(start * 2);

        while (halves > 0)
        {
            halves--;

            var left = allHalves % 2 == 0;
            var index = allHalves / 2;
            yield return (index, left);

            allHalves++;
        }
    }

    /// <summary>
    ///     Calculates the intersection between two segments.
    /// </summary>
    /// <param name="seg1Start">The start of the first segment.</param>
    /// <param name="seg1Len">The length of the first segment.</param>
    /// <param name="seg2Start">The start of the second segment.</param>
    /// <param name="seg2Len">The length of the second segment.</param>
    /// <returns>A tuple containing the intersection.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (int start, int count) IntersectSegments(int seg1Start, int seg1Len, int seg2Start, int seg2Len)
    {
        if (seg1Len < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seg1Len));
        }

        if (seg2Len < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seg2Len));
        }

        var seg1End = seg1Start + seg1Len - 1;
        var seg2End = seg2Start + seg2Len - 1;

        if (seg1End < seg2Start || seg2End < seg1Start)
        {
            return (-1, 0);
        }

        var intStart = Math.Max(seg1Start, seg2Start);
        var intEnd = Math.Min(seg1End, seg2End);

        return (intStart, intEnd - intStart + 1);
    }

    /// <summary>
    ///     Draws a line between two points in the drawing using block characters.
    /// </summary>
    /// <param name="start">The starting point.</param>
    /// <param name="end">The ending point.</param>
    /// <param name="traceFunc">The trace function invoked for each point.</param>
    public static void TraceLineInHalves(PointF start, PointF end, Action<PointF> traceFunc)
    {
        Debug.Assert(traceFunc != null);

        var x1 = (int) Math.Floor(start.X * 2);
        var y1 = (int) Math.Floor(start.Y * 2);
        var x2 = (int) Math.Floor(end.X * 2);
        var y2 = (int) Math.Floor(end.Y * 2);

        var w = x2 - x1;
        var h = y2 - y1;

        var dx1 = Math.Sign(w);
        var dx2 = Math.Sign(w);

        var dy1 = Math.Sign(h);
        var dy2 = 0;

        var longest = Math.Abs(w);
        var shortest = Math.Abs(h);

        if (longest <= shortest)
        {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);
            dy2 = Math.Sign(h);
            dx2 = 0;
        }

        var numerator = longest >> 1;
        for (var i = 0; i <= longest; i++)
        {
            traceFunc(new(x1 / 2F, y1 / 2F));

            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x1 += dx1;
                y1 += dy1;
            }
            else
            {
                x1 += dx2;
                y1 += dy2;
            }
        }
    }

    /// <summary>
    ///     Refreshes a surface using terminal's <see cref="Terminal.AtomicRefresh" /> facility.
    /// </summary>
    /// <param name="t">The terminal.</param>
    /// <param name="s">The surface.</param>
    public static void Refresh(this Terminal t, ISurface s)
    {
        Debug.Assert(t != null);
        Debug.Assert(s != null);

        _ = t.AtomicRefreshOpen
            ? t.Curses.wnoutrefresh(s.Handle)
             .Check(nameof(t.Curses.wnoutrefresh), "Failed to queue window refresh.")
            : t.Curses.wrefresh(s.Handle)
             .Check(nameof(t.Curses.wrefresh), "Failed to perform window refresh.");
    }

    /// <summary>
    ///     Intersects the given <paramref name="area" /> with the total area of a surface and returns the intersection.
    /// </summary>
    /// <param name="size">The size of the destination surface.</param>
    /// <param name="area">The desired area.</param>
    /// <returns><c>true</c> if the intersection is not empty; <c>false</c> otherwise.</returns>
    public static bool AdjustToActualArea(this Size size, ref Rectangle area) =>
        new Rectangle(new(0, 0), size).AdjustToActualArea(ref area);

    /// <summary>
    ///     Intersects the given <paramref name="area" /> with the total area of a surface and returns the intersection.
    /// </summary>
    /// <param name="total">The size of the destination surface.</param>
    /// <param name="area">The desired area.</param>
    /// <returns><c>true</c> if the intersection is not empty; <c>false</c> otherwise.</returns>
    public static bool AdjustToActualArea(this Rectangle total, ref Rectangle area)
    {
        total.Intersect(area);
        area = total;

        return total is { Width: > 0, Height: > 0 };
    }

    /// <summary>
    ///     Intersects the given <paramref name="area" /> with the total area of a surface and returns the intersection.
    /// </summary>
    /// <param name="size">The size of the destination surface.</param>
    /// <param name="area">The desired area.</param>
    /// <returns><c>true</c> if the intersection is not empty; <c>false</c> otherwise.</returns>
    public static bool AdjustToActualArea(this Size size, ref RectangleF area)
    {
        var r = new RectangleF(new(0, 0), size);
        r.Intersect(area);
        area = r;

        return !r.IsEmpty;
    }
}
