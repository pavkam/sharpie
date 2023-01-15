/*
Copyright (c) 2023, Alexandru Ciobanu
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

namespace Sharpie.Font;

/// <summary>
///     Evaluates FIGlet character layouts
/// </summary>
internal static class FigletLayoutEvaluator
{
    private static readonly IReadOnlySet<char> UnderscoreReplaceChars = new HashSet<char>
    {
        '|',
        '/',
        '\\',
        '[',
        ']',
        '{',
        '}',
        '(',
        ')',
        '<',
        '>'
    };

    private static readonly IReadOnlyDictionary<char, int> ClassReplaceChars = new Dictionary<char, int>
    {
        { '|', 0 },
        { '/', 1 },
        { '\\', 1 },
        { '[', 2 },
        { ']', 2 },
        { '{', 3 },
        { '}', 3 },
        { '(', 4 },
        { ')', 4 },
        { '<', 5 },
        { '>', 5 }
    };

    /// <summary>
    ///     Merges two characters together using a given merge function.
    /// </summary>
    /// <param name="mergeFunc">The merge function.</param>
    /// <param name="left">The accumulated joined string.</param>
    /// <param name="right">The right character.</param>
    public static void Join(Func<char, char, char> mergeFunc, StringBuilder[] left, string[] right)
    {
        Debug.Assert(mergeFunc != null);
        Debug.Assert(left != null);
        Debug.Assert(right != null);
        Debug.Assert(left.Length == right.Length);
        Debug.Assert(left.All(s => s.Length ==
            left[0]
                .Length));

        Debug.Assert(right.All(s => s.Length ==
            right[0]
                .Length));

        // join the character rows and setup touch points.
        var count = left.Length;
        var limits = left.Select((v, i) =>
                         {
                             var l = v.Length;
                             v.Append(right[i]);
                             return l - 1;
                         })
                         .ToArray();

        // merge each set of touching characters.
        var finished = false;
        while (!finished)
        {
            // check if this iteration is valid.
            var iterationValid = true;
            for (var y = 0; y < count; y++)
            {
                var current = left[y];
                var touch = limits[y];
                if (touch == -1 || touch == current.Length - 1)
                {
                    iterationValid = false;
                    break;
                }

                var l = current[touch];
                var r = current[touch + 1];

                if (r != ControlCharacter.Whitespace &&
                    l != ControlCharacter.Whitespace &&
                    mergeFunc(l, r) == ControlCharacter.Null)
                {
                    iterationValid = false;
                    break;
                }
            }

            if (!iterationValid)
            {
                break;
            }

            for (var y = 0; y < count; y++)
            {
                var current = left[y];
                var touch = limits[y];
                Debug.Assert(touch >= 0 && touch < current.Length - 1);

                var l = current[touch];
                var r = current[touch + 1];

                if (r == ControlCharacter.Whitespace)
                {
                    current.Remove(touch + 1, 1);
                } else if (l == ControlCharacter.Whitespace)
                {
                    current.Remove(touch, 1);
                    touch--;
                } else
                {
                    var m = mergeFunc(l, r);
                    Debug.Assert(m != ControlCharacter.Null);
                    current.Remove(touch, 2)
                           .Insert(touch, m);

                    finished = true;
                }

                limits[y] = touch;
            }
        }

        Debug.Assert(left.All(m => m.Length ==
            left[0]
                .Length));

    }

    /// <summary>
    ///     Joins two characters in horizontal mode.
    /// </summary>
    /// <param name="hardBlankChar">The hard blank character.</param>
    /// <param name="attributes">Layout attributes.</param>
    /// <param name="left">The lest character.</param>
    /// <param name="right">The right character.</param>
    /// <returns>The merged character of <c>\0</c> if merging not allowed.</returns>
    public static char HorizontalJoin(char hardBlankChar, FigletAttribute attributes, char left, char right)
    {
        Debug.Assert(attributes != FigletAttribute.FullWidth);
        Debug.Assert(attributes.HasFlag(FigletAttribute.HorizontalFitting) !=
            attributes.HasFlag(FigletAttribute.HorizontalSmushing));

        if (attributes.HasFlag(FigletAttribute.HorizontalFitting))
        {
            return '\0';
        }

        if (attributes == FigletAttribute.HorizontalSmushing)
        {
            return right;
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule1))
        {
            if (left == right && left != hardBlankChar)
            {
                return left;
            }
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule2))
        {
            if (left == '_' && UnderscoreReplaceChars.Contains(right))
            {
                return right;
            }

            if (right == '_' && UnderscoreReplaceChars.Contains(left))
            {
                return left;
            }
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule3))
        {
            if (ClassReplaceChars.TryGetValue(left, out var lp) &&
                ClassReplaceChars.TryGetValue(right, out var rp) &&
                rp != lp)
            {
                return lp > rp ? left : right;
            }
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule4))
        {
            switch (left, right)
            {
                case ('[', ']'):
                case (']', '['):
                case ('{', '}'):
                case ('}', '{'):
                case ('(', ')'):
                case (')', '('):
                    return '|';
            }
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule5))
        {
            switch (left, right)
            {
                case ('/', '\\'):
                case ('\\', '/'):
                    return 'Y';
                case ('>', '<'):
                    return 'X';
            }
        }

        if (attributes.HasFlag(FigletAttribute.HorizontalSmushingRule6))
        {
            if (left == hardBlankChar && right == hardBlankChar)
            {
                return hardBlankChar;
            }
        }

        return '\0';
    }
}
