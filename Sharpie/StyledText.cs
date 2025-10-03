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
///     Describes a text with mixed in styles. Can be used by <see cref="ISurface.WriteText(StyledText,bool)" />.
/// </summary>
[PublicAPI]
public readonly struct StyledText
{
    internal (string text, Style style)[]? Parts
    {
        get;
    }

    private StyledText((string text, Style style)[] parts) => Parts = parts;

    /// <summary>
    ///     Creates a new styled text with the given <paramref name="text" /> and <paramref name="style" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="style">The text style.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text" /> is <c>null</c>.</exception>
    public StyledText(string text, Style style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        Parts = new[] { (text, style) };
    }

    /// <summary>
    ///     Combines this styled text with another styled text.
    /// </summary>
    /// <param name="text">The other text to combine with.</param>
    /// <param name="style">The style of the text to combine with.</param>
    /// <returns>The combined styled text.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text" /> is <c>null</c>.</exception>
    public StyledText Plus(string text, Style style) => text == null ? throw new ArgumentNullException(nameof(text)) : Plus(new(text, style));

    /// <summary>
    ///     Combines this styled text with another styled text.
    /// </summary>
    /// <param name="rhs">The other styled text to combine with.</param>
    /// <returns>The combined styled text.</returns>
    public StyledText Plus(StyledText rhs)
    {
        if (Parts == null)
        {
            return rhs;
        }

        if (rhs.Parts == null)
        {
            return this;
        }

        var combined = new (string text, Style style)[Parts.Length + rhs.Parts.Length];
        Array.Copy(Parts, combined, Parts.Length);
        Array.Copy(rhs.Parts, 0, combined, Parts.Length, Parts.Length);

        return new(combined);
    }

    /// <summary>
    ///     Combines two styled texts together.
    /// </summary>
    /// <param name="lhs">The left hand side styled text to combine.</param>
    /// <param name="rhs">The left right side styled text to combine.</param>
    /// <returns>The combined styled text.</returns>
    public static StyledText operator +(StyledText lhs, StyledText rhs) => lhs.Plus(rhs);

    /// <inheritdoc cref="object.Equals(object?)" />
    public override bool Equals(object? obj)
    {
        if (obj?.GetType() != GetType())
        {
            return false;
        }

        var op = ((StyledText) obj).Parts;

#pragma warning disable IDE0072 // Add missing cases -- all cases are covered
        return (op, Parts) switch
        {
            (null, null) => true,
            (null, not null) => false,
            (not null, null) => false,
            (var l and not null, var r and not null) when l.Length != r.Length => false,
            (var l and not null, var r and not null) => !r.Where((t, i) => l[i] != t)
                                                          .Any(),
        };
#pragma warning restore IDE0072 // Add missing cases
    }

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode()
    {
        var h = new HashCode();
        if (Parts != null)
        {
            foreach (var p in Parts)
            {
                h.Add(p);
            }
        }

        return h.ToHashCode();
    }

    /// <inheritdoc cref="object.ToString" />
    public override string? ToString() => Parts == null ? null : string.Join(", ", Parts.Select(p => $"\"{p.text}\" @ {p.style}"));

    /// <summary>
    ///     Checks if two styled texts are equal.
    /// </summary>
    /// <param name="lhs">The left hand side styled text.</param>
    /// <param name="rhs">The left right side styled ..</param>
    /// <returns>The result of the check.</returns>
    public static bool operator ==(StyledText lhs, StyledText rhs) => lhs.Equals(rhs);

    /// <summary>
    ///     Checks if two styled texts are not equal.
    /// </summary>
    /// <param name="lhs">The left hand side styled text.</param>
    /// <param name="rhs">The left right side styled ..</param>
    /// <returns>The result of the check.</returns>
    public static bool operator !=(StyledText lhs, StyledText rhs) => !(lhs == rhs);
}
