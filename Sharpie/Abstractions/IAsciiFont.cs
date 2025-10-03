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

namespace Sharpie.Abstractions;

using Font;

/// <summary>
///     Defines the traits implemented by ASCII font providers.
/// </summary>
[PublicAPI]
public interface IAsciiFont
{
    /// <summary>
    ///     The font height.
    /// </summary>
    int Height
    {
        get;
    }

    /// <summary>
    ///     The font baseline.
    /// </summary>
    int Baseline
    {
        get;
    }

    /// <summary>
    ///     The font's name.
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    ///     The font's layout.
    /// </summary>
    AsciiFontLayout Layout
    {
        get;
    }

    /// <summary>
    ///     Checks if the font contains a given glyph.
    /// </summary>
    /// <param name="char">The character.</param>
    /// <returns><c>true</c> if the font contains the given glyph; <c>false</c> otherwise.</returns>
    bool HasGlyph(Rune @char);

    /// <summary>
    ///     Tries to get a glyph for a given <paramref name="char" />.
    /// </summary>
    /// <param name="char">The character.</param>
    /// <param name="style">The style to apply to the glyph.</param>
    /// <returns>The output glyph, if found. Otherwise, the font will substitute the glyph with something else.</returns>
    IDrawable GetGlyph(Rune @char, Style style);

    /// <summary>
    ///     Tries to get a drawing for a given list of <paramref name="chars" />.
    /// </summary>
    /// <param name="chars">The characters.</param>
    /// <param name="style">The style to apply to the glyphs.</param>
    /// <returns>The output glyphs.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="chars" /> is empty.</exception>
    IDrawable GetGlyphs(ReadOnlySpan<Rune> chars, Style style);

    /// <summary>
    ///     Tries to get a drawing for a given <paramref name="text" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="style">The style to apply to the glyphs.</param>
    /// <returns>The output glyphs.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text" /> is empty.</exception>
    IDrawable GetGlyphs(string text, Style style);
}
