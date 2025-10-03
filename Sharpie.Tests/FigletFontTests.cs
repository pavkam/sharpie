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

namespace Sharpie.Tests;

using Font;

[TestClass]
public class FigletFontTests
{
    private static readonly (string[] rows, int width) _char0 = (new[] { "abc1", "def2", "ghi3" }, 4);
    private static readonly (string[] rows, int width) _char1 = (new[] { "123", "456", "789" }, 3);
    private static readonly (string[] rows, int width) _char2 = (new[] { "1", "$", "3" }, 1);

    private static readonly Style _style1 = new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = new()
        {
            Handle = 99
        }
    };

    private static readonly FigletHeader _header = new('$', 3, 2, FigletAttribute.FullWidth,
        FigletScriptDirection.LeftToRight);

    private readonly FigletFont _font = new("name", _header,
        new Dictionary<int, (string[] rows, int width)> { { 1, _char1 }, { 0, _char0 }, { 3, _char2 } });

    [TestMethod]
    public void Name_ReturnsTheSuppliedValue() => _font.Name.ShouldBe("name");

    [TestMethod]
    public void Height_ReturnsTheSuppliedValue() => _font.Height.ShouldBe(3);

    [TestMethod]
    public void Baseline_ReturnsTheSuppliedValue() => _font.Baseline.ShouldBe(2);

    [TestMethod, DataRow(FigletAttribute.FullWidth, AsciiFontLayout.FullWidth),
     DataRow((FigletAttribute) 0x01000000, AsciiFontLayout.FullWidth),
     DataRow(FigletAttribute.HorizontalFitting | FigletAttribute.VerticalSmushing,
         AsciiFontLayout.Fitted),
     DataRow(FigletAttribute.HorizontalSmushing | FigletAttribute.VerticalFitting,
         AsciiFontLayout.Smushed)]
    public void DefaultLayout_ReturnsTheExpectedCombinations(FigletAttribute attr, AsciiFontLayout exp)
    {
        var header = _header with
        {
            Attributes = attr
        };
        var font = new FigletFont("name", header, new Dictionary<int, (string[] rows, int width)>());

        font.Layout.ShouldBe(exp);
    }

    [TestMethod]
    public void HasGlyph_ReturnsFalse_IfCharNotDefined()
    {
        _font.HasGlyph(new(2))
             .ShouldBeFalse();
    }

    [TestMethod]
    public void HasGlyph_ReturnsTrue_IfCharDefined()
    {
        _font.HasGlyph(new(1))
             .ShouldBeTrue();
    }

    [TestMethod]
    public void GetGlyphs_Throws_IfSpanIsEmpty()
    {
        _ = Should.Throw<ArgumentException>(() =>
            _font.GetGlyphs(Array.Empty<Rune>(), _style1));
    }

    [TestMethod]
    public void GetGlyphs_ReturnsTheExpectedGlyphsIncludingDefault_GreaterWidth()
    {
        var k = new[] { new Rune(1), new Rune(256), new Rune(3) };

        var glyph = _font.GetGlyphs(k, _style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new Rune('1'), _style1), (new('4'), _style1), (new('7'), _style1) },
            { (new('2'), _style1), (new('5'), _style1), (new('8'), _style1) },
            { (new('3'), _style1), (new('6'), _style1), (new('9'), _style1) },

            { (new('a'), _style1), (new('d'), _style1), (new('g'), _style1) },
            { (new('b'), _style1), (new('e'), _style1), (new('h'), _style1) },
            { (new('c'), _style1), (new('f'), _style1), (new('i'), _style1) },
            { (new('1'), _style1), (new('2'), _style1), (new('3'), _style1) },

            { (new('1'), _style1), (new(' '), _style1), (new('3'), _style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyphs_ReturnsTheExpectedGlyphsIncludingReplacement_GreaterWidth()
    {
        var font = new FigletFont("name", _header, new Dictionary<int, (string[] rows, int width)>()
            { { 1, _char1 }, { 3, _char0 } });

        var k = new[] { new Rune(1), new Rune(256), new Rune(3) };

        var glyphs = font.GetGlyphs(k, _style1);
        var contents = glyphs.GetContents();

        var cols = new[,]
        {
            { (new Rune('1'), _style1), (new('4'), _style1), (new('7'), _style1) },
            { (new('2'), _style1), (new('5'), _style1), (new('8'), _style1) },
            { (new('3'), _style1), (new('6'), _style1), (new('9'), _style1) },

            { (new('┌'), _style1), (new('│'), _style1), (new('└'), _style1) },
            { (new('─'), _style1), (new(' '), _style1), (new('─'), _style1) },
            { (new('┐'), _style1), (new('│'), _style1), (new Rune('┘'), _style1) },

            { (new Rune('a'), _style1), (new('d'), _style1), (new('g'), _style1) },
            { (new('b'), _style1), (new('e'), _style1), (new('h'), _style1) },
            { (new('c'), _style1), (new('f'), _style1), (new('i'), _style1) },
            { (new('1'), _style1), (new('2'), _style1), (new('3'), _style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyphs_ReplacesHardBlanksWithWhitespaces()
    {
        var header = _header with
        {
            Height = 1,
            BaseLine = 1,
            Attributes = FigletAttribute.HorizontalSmushing
        };
        var font = new FigletFont("name", header, new Dictionary<int, (string[] rows, int width)>
        {
            { 1, (new[] { "AB" }, 2) },
            { 2, (new[] { "12" }, 2) }
        });

        var k = new[] { new Rune(1), new Rune(2) };

        var glyphs = font.GetGlyphs(k, _style1);
        var contents = glyphs.GetContents();

        var cols = new[,]
        {
            { (new Rune('A'), _style1) },
            { (new('1'), _style1) },
            { (new('2'), _style1) }
        };

        contents.ShouldBe(cols);
    }


    [TestMethod]
    public void GetGlyphs_MergesCharactersUsingLayoutRules()
    {
        var glyph = _font.GetGlyphs(new[] { new Rune(3) }, _style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new Rune('1'), _style1), (new(' '), _style1), (new('3'), _style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyphs_ReturnsChars_WithWidthOfOne()
    {
        var header = _header with
        {
            Height = 1,
            BaseLine = 1
        };
        var font = new FigletFont("name", header,
            new Dictionary<int, (string[] rows, int width)>()
            {
                {'A', (new[] { "A" }, 1)},
                {'B', (new[] { "B" }, 1)}
            });

        var k = new[] { new Rune('A'), new Rune(256), new Rune('B') };

        var glyphs = font.GetGlyphs(k, _style1);
        var contents = glyphs.GetContents();

        var cols = new[,]
        {
            { (new Rune('A'), _style1) },
            { (new Rune('□'), _style1) },
            { (new Rune('B'), _style1) },
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public async Task LoadAsync1_Throws_IfNameIsNullOrEmptyAsync() =>
        await Should.ThrowAsync<ArgumentException>(() => FigletFont.LoadAsync("", new StringReader(string.Empty)));

    [TestMethod]
    public async Task LoadAsync1_Throws_IfReaderIsNullAsync() =>
        await Should.ThrowAsync<ArgumentNullException>(() => FigletFont.LoadAsync("name", null!));

    [TestMethod]
    public async Task LoadAsync1_Throws_IfFailedToLoadFontAsync() =>
        await Should.ThrowAsync<FormatException>(() => FigletFont.LoadAsync("font", new StringReader(string.Empty)));

    [TestMethod]
    public async Task LoadAsync1_LoadsTheFontAsExpectedAsync()
    {
        using var reader = File.OpenText("Fixtures/big.flf");
        var font = await FigletFont.LoadAsync("font", reader);

        font.Height.ShouldBe(8);
        font.Name.ShouldBe("font");
        font.HasGlyph(new(ControlCharacter.Null))
            .ShouldBeFalse();

        font.HasGlyph(new(ControlCharacter.Whitespace))
            .ShouldBeTrue();

        font.HasGlyph(new(255))
            .ShouldBeTrue();
    }

    [TestMethod]
    public async Task LoadAsync2_Throws_IfPathIsNullOrEmptyAsync()
    {
        var dn = new Mock<IDotNetSystemAdapter>();

        _ = await Should.ThrowAsync<ArgumentException>(() => FigletFont.LoadAsync(dn.Object, ""));
    }

    [TestMethod]
    public async Task LoadAsync2_Throws_IfFailedToOpenFileAsync()
    {
        var dn = new Mock<IDotNetSystemAdapter>();
        _ = dn.Setup(s => s.OpenFileAsText(It.IsAny<string>()))
          .Throws<FileNotFoundException>();

        _ = await Should.ThrowAsync<FileNotFoundException>(() => FigletFont.LoadAsync(dn.Object, "font"));

        dn.Verify(v => v.GetFileName(It.IsAny<string>()), Times.Never);
        dn.Verify(v => v.OpenFileAsText("font"), Times.Once);
    }

    [TestMethod]
    public async Task LoadAsync2_LoadsTheFontAsExpectedAsync()
    {
        const string f = "Fixtures/big.flf";
        var dn = new Mock<IDotNetSystemAdapter>();

        _ = dn.Setup(s => s.GetFileName(f))
          .Returns("font_name");

        _ = dn.Setup(s => s.OpenFileAsText(f))
          .Returns(File.OpenText(f));

        var font = await FigletFont.LoadAsync(dn.Object, f);

        font.Height.ShouldBe(8);
        font.Name.ShouldBe("font_name");
        font.HasGlyph(new(ControlCharacter.Null))
            .ShouldBeFalse();

        font.HasGlyph(new(ControlCharacter.Whitespace))
            .ShouldBeTrue();

        font.HasGlyph(new(255))
            .ShouldBeTrue();
    }
}
