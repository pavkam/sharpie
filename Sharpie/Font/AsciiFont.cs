/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
///     Defines the traits implemented by ASCII font providers.
/// </summary>
[PublicAPI]
public abstract class AsciiFont: IAsciiFont
{
    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="height">The height of the font.</param>
    /// <param name="baseline">The font's baseline.</param>
    /// <param name="layout">The font's layout.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null of empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="height"/> is less than one.</exception>
    protected AsciiFont(string name, int height, int baseline, AsciiFontLayout layout)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (baseline <= 0 || baseline > height)
        {
            throw new ArgumentOutOfRangeException(nameof(baseline));
        }

        Baseline = baseline;
        Height = height;
        Name = name;
        Layout = layout;
    }

    /// <inheritdoc cref="IAsciiFont.Baseline"/>
    public int Baseline
    {
        get;
    }

    /// <inheritdoc cref="IAsciiFont.Height"/>
    public int Height
    {
        get;
    }

    /// <inheritdoc cref="IAsciiFont.Name"/>
    public string Name
    {
        get;
    }

    /// <inheritdoc cref="IAsciiFont.Name"/>
    public AsciiFontLayout Layout
    {
        get;
    }

    /// <inheritdoc cref="IAsciiFont.HasGlyph"/>
    public abstract bool HasGlyph(Rune @char);

    /// <inheritdoc cref="IAsciiFont.GetGlyph"/>
    public virtual IDrawable GetGlyph(Rune @char, Style style) => GetGlyphs([@char], style);

    /// <inheritdoc cref="IAsciiFont.GetGlyphs(ReadOnlySpan{Rune}, Style)"/>
    public abstract IDrawable GetGlyphs(ReadOnlySpan<Rune> chars, Style style);

    /// <inheritdoc cref="IAsciiFont.GetGlyphs(string, Style)"/>
    public virtual IDrawable GetGlyphs(string text, Style style) =>
        GetGlyphs(text.EnumerateRunes()
                      .ToArray(), style);
}
