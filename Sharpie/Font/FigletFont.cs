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

namespace Sharpie.Font;

/// <summary>
///     FIGlet ASCII font. See http://www.jave.de/figlet/figfont.html and https://github.com/cmatsuoka/figlet for details
///     and available fonts.
/// </summary>
[PublicAPI]
public sealed class FigletFont: IAsciiFont
{
    private readonly IReadOnlyDictionary<int, (string[] rows, int width)> _figletCharacters;
    private readonly FigletHeader _header;
    private DefaultCharacterHelper? _defaultCharacterHelper;

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="header">The header.</param>
    /// <param name="characters">The characters from the font.</param>
    internal FigletFont(string name, FigletHeader header,
        IReadOnlyDictionary<int, (string[] rows, int width)> characters)
    {
        Debug.Assert(!string.IsNullOrEmpty(name));
        Debug.Assert(header != null);
        Debug.Assert(characters != null);

        Name = name;
        _header = header;
        _figletCharacters = characters;
    }

    /// <summary>
    ///     The font character base line (on which upper-case letters are drawn).
    /// </summary>
    public int Baseline => _header.BaseLine;

    /// <summary>
    ///     The default font layout (as desired by the font).
    /// </summary>
    public FigletLayout DefaultLayout
    {
        get
        {
            var res = FigletLayout.FullWidth;
            if (_header.Attributes.HasFlag(FigletAttribute.HorizontalSmushing))
            {
                res |= FigletLayout.HorizontalSmush;
            }

            if (_header.Attributes.HasFlag(FigletAttribute.HorizontalFitting))
            {
                res |= FigletLayout.HorizontalFit;
            }

            if (_header.Attributes.HasFlag(FigletAttribute.VerticalSmushing))
            {
                res |= FigletLayout.VerticalSmush;
            }

            if (_header.Attributes.HasFlag(FigletAttribute.VerticalFitting))
            {
                res |= FigletLayout.VerticalFit;
            }

            return res;
        }
    }

    /// <inheritdoc cref="IAsciiFont.Height" />
    public int Height => _header.Height;

    /// <inheritdoc cref="IAsciiFont.Name" />
    public string Name { get; }

    /// <inheritdoc cref="IAsciiFont.HasGlyph" />
    public bool HasGlyph(Rune @char) => _figletCharacters.ContainsKey(@char.Value);

    /// <inheritdoc cref="IAsciiFont.GetGlyph" />
    public IDrawable GetGlyph(Rune @char, Style style) => GetGlyphs(new[] { @char }, style);

    /// <inheritdoc cref="IAsciiFont.GetGlyphs" />
    public IDrawable GetGlyphs(ReadOnlySpan<Rune> chars, Style style)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Chars cannot be empty.", nameof(chars));
        }

        char MergeFunc(char l, char r) =>
            FigletLayoutEvaluator.HorizontalJoin(_header.HardBlankChar, _header.Attributes, l, r);

        string[]? rep = null;
        foreach (var @char in chars)
        {
            string[]? rows;
            if (_figletCharacters.TryGetValue(@char.Value, out var fc) || _figletCharacters.TryGetValue(0, out fc))
            {
                (rows, _) = fc;
            } else
            {
                _defaultCharacterHelper ??= new(Height);
                rows = _defaultCharacterHelper.Rows;
            }

            Debug.Assert(rows.Length == Height);
            Debug.Assert(rows.All(row => row.Length ==
                rows[0]
                    .Length));

            if (rep == null)
            {
                rep = rows;
                continue;
            }

            if (_header.Attributes == FigletAttribute.FullWidth)
            {
                rep = rep.Select((row, i) => row + rows[i])
                         .ToArray();
            } else
            {
                rep = FigletLayoutEvaluator.Join(MergeFunc, rep, rows);
            }
        }

        Debug.Assert(rep != null);

        return DrawCharacterToCanvas(rep, style);
    }

    private IDrawable DrawCharacterToCanvas(string[] rows, Style style)
    {
        Debug.Assert(rows.Length == Height);
        var width = rows[0]
            .Length;

        Debug.Assert(rows.All(row => row.Length == width));

        var canvas = new Canvas(new(width, Height));
        for (var y = 0; y < Height; y++)
        {
            Debug.Assert(rows[y]
                    .Length ==
                width);

            for (var x = 0; x < width; x++)
            {
                var ch = rows[y][x];
                if (ch == _header.HardBlankChar)
                {
                    ch = ControlCharacter.Whitespace;
                }

                canvas.Glyph(new(x, y), new(ch), style);
            }
        }

        return canvas;
    }

    /// <summary>
    ///     Loads a FIGlet font file from a given <paramref name="reader" />.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="reader">The reader.</param>
    /// <returns>A <see cref="FigletFont" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="reader" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name" /> is <c>null</c> or empty.</exception>
    public static async Task<FigletFont> LoadAsync(string name, TextReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(name));
        }

        var (header, chars) = await FigletFontParser.ParseFontFileAsync(reader);
        return new(name, header, chars);
    }

    /// <summary>
    ///     Loads a FIGlet font file from a given file at <paramref name="path" />.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system adapter.</param>
    /// <param name="path">The font file name.</param>
    /// <returns>A <see cref="FigletFont" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="path" /> is <c>null</c> or empty.</exception>
    internal static async Task<FigletFont> LoadAsync(IDotNetSystemAdapter dotNetSystemAdapter, string path)
    {
        Debug.Assert(dotNetSystemAdapter != null);

        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(path));
        }

        using var reader = dotNetSystemAdapter.OpenFileAsText(path);
        var fontName = dotNetSystemAdapter.GetFileName(path);

        Debug.Assert(fontName != null);

        return await LoadAsync(fontName, dotNetSystemAdapter.OpenFileAsText(path));
    }

    /// <summary>
    ///     Loads a FIGlet font file from a given file at <paramref name="path" />.
    /// </summary>
    /// <param name="path">The font file name.</param>
    /// <returns>A <see cref="FigletFont" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="path" /> is <c>null</c> or empty.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested.")]
    public static Task<FigletFont> LoadAsync(string path) => LoadAsync(IDotNetSystemAdapter.Instance, path);

    private sealed class DefaultCharacterHelper: IDrawSurface
    {
        private readonly char[][] _rows;

        public DefaultCharacterHelper(int size)
        {
            var drawable = GetReplacementCharacter(size);
            Size = drawable.Size;

            _rows = Enumerable.Range(0, Size.Height)
                              .Select(_ => new char[Size.Width])
                              .ToArray();

            drawable.DrawOnto(this, new(new(0, 0), drawable.Size), new(0, 0));
        }

        public string[] Rows =>
            _rows.Select(r => new string(r))
                 .ToArray();

        public Size Size { get; }

        public void DrawCell(Point location, Rune rune, Style style)
        {
            _rows[location.Y][location.X] = (char) rune.Value;
        }

        private static IDrawable GetReplacementCharacter(int size)
        {
            var canvas = new Canvas(new(size, size));

            if (size > 1)
            {
                var rect = new Rectangle(new(0, 0), canvas.Size);

                canvas.Fill(rect, new Rune(ControlCharacter.Whitespace), Style.Default);
                canvas.Box(rect, Canvas.LineStyle.Light, Style.Default);
            } else
            {
                canvas.Glyph(new(0, 0), new('□'), Style.Default);
            }

            return canvas;
        }
    }
}
