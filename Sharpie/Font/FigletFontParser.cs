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
///     Parses FIGlet font files ('.flf').
/// </summary>
internal static class FigletFontParser
{
    private const string Signature = "flf2a";
    private const int StandardCharacterCount = 102;
    private const int StartCharacterCodePoint = 32;

    /// <summary>
    ///     Parses the header of the font file.
    /// </summary>
    /// <param name="line">The header line.</param>
    /// <returns>The header details.</returns>
    /// <exception cref="FormatException">Thrown if the <paramref name="line" /> is not correctly formatted.</exception>
    public static (FigletHeader header, int maxLineWidth, int commentCount) ParseHeader(string line)
    {
        Debug.Assert(line != null);

        var components = line.Split(' ');
        if (components.Length < 6)
        {
            throw new FormatException("Header: Invalid number of components");
        }

        var signatureAndHardBlank = components[0];
        if (!signatureAndHardBlank.StartsWith(Signature))
        {
            throw new FormatException($"Header: Invalid signature `{signatureAndHardBlank}`");
        }

        var hardBlank = '\0';
        if (signatureAndHardBlank.Length > Signature.Length)
        {
            var hardBlankStr = signatureAndHardBlank[Signature.Length..];
            if (hardBlankStr.Length > 1)
            {
                throw new FormatException($"Header: Invalid hard blank value `{hardBlankStr}`");
            }

            hardBlank = hardBlankStr[0];
        }

        if (!int.TryParse(components[1], out var height) || height <= 0)
        {
            throw new FormatException($"Header: Invalid height value `{height}`");
        }

        if (!int.TryParse(components[2], out var baseLine) || baseLine <= 0 || baseLine > height)
        {
            throw new FormatException($"Header: Invalid baseline value `{baseLine}`");
        }

        if (!int.TryParse(components[3], out var maxWidth) || maxWidth <= 1)
        {
            throw new FormatException($"Header: Invalid max length value `{maxWidth}`");
        }

        if (!int.TryParse(components[4], out var oldLayout) || oldLayout < -1)
        {
            throw new FormatException($"Header: Invalid old layout value `{oldLayout}`");
        }

        if (!int.TryParse(components[5], out var commentLineCount) || commentLineCount < 0)
        {
            throw new FormatException($"Header: Invalid comment count value `{commentLineCount}`");
        }

        var direction = 0;
        if (components.Length > 6 && !int.TryParse(components[6], out direction) || direction is < 0 or > 1)
        {
            throw new FormatException($"Header: Invalid direction value `{direction}`");
        }

        var fullLayout = 0;
        if (components.Length > 7 && !int.TryParse(components[7], out fullLayout) || fullLayout < 0)
        {
            throw new FormatException($"Header: Invalid full layout value `{fullLayout}`");
        }

        var layout = FigletLayout.FullWidth;
        if (fullLayout != 0)
        {
            layout = (FigletLayout) fullLayout;
        } else if (oldLayout == 0)
        {
            layout = FigletLayout.HorizontalFitting;
        } else if (oldLayout != -1)
        {
            layout = (FigletLayout) oldLayout;
        }

        if (layout.HasFlag(FigletLayout.VerticalSmushing))
        {
            layout &= ~FigletLayout.VerticalFitting;
        }

        if (layout.HasFlag(FigletLayout.HorizontalSmushing))
        {
            layout &= ~FigletLayout.HorizontalFitting;
        }

        return (new(hardBlank, height, baseLine, layout, (FigletScriptDirection) direction), maxWidth,
            commentLineCount);
    }

    private static int ParseNumber(string n)
    {
        Debug.Assert(n != null);

        var sign = 1;
        var index = 0;
        var @base = 10;

        if (n.StartsWith('-'))
        {
            sign = -1;
            index++;
        }

        if (n.IndexOf("0x", index, StringComparison.Ordinal) == index)
        {
            index += 2;
            @base = 16;
        } else if (n.IndexOf('0', index) == index)
        {
            if (index == n.Length - 1)
            {
                return 0;
            }

            index++;
            @base = 8;
        }

        try
        {
            return sign * Convert.ToInt32(n[index..], @base);
        } catch (FormatException)
        {
            throw new FormatException($"Character: Invalid code point {n}.");
        }
    }

    /// <summary>
    ///     Parses a code tag of a character.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>The parsed code point and description.</returns>
    /// <exception cref="FormatException">Throws if the <paramref name="line" /> is invalid.</exception>
    public static (int tag, string description) ParseCodeTag(string line)
    {
        Debug.Assert(line != null);

        var tagLine = line.Trim();
        return tagLine.IndexOf(' ') switch
        {
            -1 when tagLine.Length == 0 => throw new FormatException("Character: expected code point."),
            -1 => (ParseNumber(tagLine), string.Empty),
            var i => (ParseNumber(tagLine[..i]), tagLine[(i + 1)..]
                .Trim())
        };
    }

    /// <summary>
    ///     Parses a character row.
    /// </summary>
    /// <param name="maxWidth">The maximum expected length.</param>
    /// <param name="line">The line to parse.</param>
    /// <returns>The extract part of the row.</returns>
    /// <exception cref="FormatException">Thrown if the <paramref name="line" /> is invalid.</exception>
    public static string ParseCharacterRow(int maxWidth, string line)
    {
        Debug.Assert(maxWidth > 1);
        Debug.Assert(line != null);

        if (line.Length == 0)
        {
            throw new FormatException("Character: Empty line detected.");
        }

        if (line.Length > maxWidth)
        {
            throw new FormatException($"Character: Line longer than {maxWidth} detected.");
        }

        var last = line.Length - 1;
        while (line[last] == line[^1])
        {
            last--;
        }

        return line[..(last + 1)];
    }

    private static async Task<string> GetLineAsync(TextReader reader)
    {
        Debug.Assert(reader != null);

        var line = await reader.ReadLineAsync();
        if (line == null)
        {
            throw new FormatException("File: Unexpected end of font file.");
        }

        return line;
    }

    /// <summary>
    ///     Parses a standard character out of the <paramref name="reader" />.
    /// </summary>
    /// <param name="height">The expected number of lines.</param>
    /// <param name="maxWidth">The maximum expected length.</param>
    /// <param name="reader">The text reader.</param>
    /// <returns>The rows and the width of the rows.</returns>
    /// <exception cref="FormatException">Thrown if the read character is invalid.</exception>
    public static async Task<(string[] rows, int width)> ParseStandardCharacterAsync(int height, int maxWidth,
        TextReader reader)
    {
        Debug.Assert(height > 0);
        Debug.Assert(maxWidth > 1);
        Debug.Assert(reader != null);

        var rows = new string[height];
        for (var row = 0; row < height; row++)
        {
            rows[row] = ParseCharacterRow(maxWidth, await GetLineAsync(reader));
        }

        var width = rows[0]
            .Length;

        if (rows.Any(row => row.Length != width))
        {
            throw new FormatException("Character: Not all rows are of the same length in character.");
        }

        return (rows, width);
    }

    /// <summary>
    ///     Parses a code-tagged character out of the <paramref name="reader" />.
    /// </summary>
    /// <param name="height">The expected number of lines.</param>
    /// <param name="maxWidth">The maximum expected length.</param>
    /// <param name="reader">The text reader.</param>
    /// <returns>The rows, width and code point of the character.</returns>
    /// <exception cref="FormatException">Thrown if the read character is invalid.</exception>
    public static async Task<(string[] rows, int width, int codePoint)> ParseCodeTaggedCharacterAsync(int height,
        int maxWidth, TextReader reader)
    {
        Debug.Assert(height > 0);
        Debug.Assert(maxWidth > 1);
        Debug.Assert(reader != null);

        var (codePoint, _) = ParseCodeTag(await GetLineAsync(reader));
        var (rows, width) = await ParseStandardCharacterAsync(height, maxWidth, reader);

        return (rows, width, codePoint);
    }

    /// <summary>
    ///     Parses a FIGlet font file ('.flf) from a given <paramref name="reader" />.
    /// </summary>
    /// <param name="standardCharStart">The first standard character code point.</param>
    /// <param name="standardCharCount">The number of obligatory standard characters.</param>
    /// <param name="reader">The text reader.</param>
    /// <returns>The header and collection of characters of the font.</returns>
    public static async Task<(FigletHeader, IReadOnlyDictionary<int, (string[] rows, int width)>)> ParseFontFileAsync(
        int standardCharStart, int standardCharCount, TextReader reader)
    {
        Debug.Assert(standardCharStart >= 0);
        Debug.Assert(standardCharCount >= 0);
        Debug.Assert(reader != null);

        var (header, maxLineWidth, commentCount) = ParseHeader(await GetLineAsync(reader));

        for (var cl = 0; cl < commentCount; cl++)
        {
            await GetLineAsync(reader);
        }

        var characters = new Dictionary<int, (string[] rows, int width)>();
        for (var codePoint = standardCharStart; codePoint < standardCharStart + standardCharCount; codePoint++)
        {
            var ch = await ParseStandardCharacterAsync(header.Height, maxLineWidth, reader);
            characters[codePoint] = ch;
        }

        while (reader.Peek() != -1)
        {
            var (rows, width, codePoint) = await ParseCodeTaggedCharacterAsync(header.Height, maxLineWidth, reader);

            characters[codePoint] = (rows, width);
        }

        return new(header, characters);
    }

    /// <summary>
    ///     Parses a FIGlet font file ('.flf) from a given <paramref name="reader" />.
    /// </summary>
    /// <param name="reader">The text reader.</param>
    /// <returns>The header and collection of characters of the font.</returns>
    public static Task<(FigletHeader, IReadOnlyDictionary<int, (string[] rows, int width)>)>
        ParseFontFileAsync(TextReader reader) =>
        ParseFontFileAsync(StartCharacterCodePoint, StandardCharacterCount, reader);
}
