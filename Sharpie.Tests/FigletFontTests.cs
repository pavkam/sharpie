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
    private static readonly (string[] rows, int width) Char0 = (new[] { "abc1", "def2", "ghi3" }, 4);
    private static readonly (string[] rows, int width) Char1 = (new[] { "123", "456", "789" }, 3);
    private static readonly (string[] rows, int width) Char2 = (new[] { "1", "$", "3" }, 1);

    private static readonly Style Style1 = new()
    {
        Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 }
    };

    private static readonly FigletHeader Header = new('$', 3, 2, FigletLayout.FullWidth,
        FigletScriptDirection.LeftToRight);

    private readonly FigletFont _font = new("name", Header,
        new Dictionary<int, (string[] rows, int width)> { { 1, Char1 }, { 0, Char0 }, { 3, Char2 } });

    [TestMethod] public void Name_ReturnsTheSuppliedValue() { _font.Name.ShouldBe("name"); }

    [TestMethod] public void Height_ReturnsTheSuppliedValue() { _font.Height.ShouldBe(3); }

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
    public void GetGlyph_ReturnsTheProperGlyph_IfFound()
    {
        var glyph = _font.GetGlyph(new(1), Style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new Rune('1'), Style1), (new('4'), Style1), (new('7'), Style1) },
            { (new('2'), Style1), (new('5'), Style1), (new('8'), Style1) },
            { (new('3'), Style1), (new('6'), Style1), (new('9'), Style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyph_ReturnsTheDefaultGlyph_IfNotFound()
    {
        var glyph = _font.GetGlyph(new(2), Style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new Rune('a'), Style1), (new('d'), Style1), (new('g'), Style1) },
            { (new('b'), Style1), (new('e'), Style1), (new('h'), Style1) },
            { (new('c'), Style1), (new('f'), Style1), (new('i'), Style1) },
            { (new('1'), Style1), (new('2'), Style1), (new('3'), Style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyph_ReturnsReplacesHardBlanksWithWhitespaces()
    {
        var glyph = _font.GetGlyph(new(3), Style1);
        var contents = glyph.GetContents();

        var cols = new[,] { { (new Rune('1'), Style1), (new(' '), Style1), (new('3'), Style1) } };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyph_ReturnsReplacementGlyph_IfNotFoundAndNoDefault_WithHeightGreaterThanOne()
    {
        var font = new FigletFont("name", Header, new Dictionary<int, (string[] rows, int width)>());

        var glyph = font.GetGlyph(new(2), Style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new('┌'), Style1), (new('│'), Style1), (new('└'), Style1) },
            { (new('─'), Style1), (new(' '), Style1), (new('─'), Style1) },
            { (new('┐'), Style1), (new('│'), Style1), (new Rune('┘'), Style1) }
        };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public void GetGlyph_ReturnsReplacementGlyph_IfNotFoundAndNoDefault_WithWidthOfOne()
    {
        var header = Header with { Height = 1 };
        var font = new FigletFont("name", header, new Dictionary<int, (string[] rows, int width)>());

        var glyph = font.GetGlyph(new(2), Style1);
        var contents = glyph.GetContents();

        var cols = new[,] { { (new Rune('□'), Style1) } };

        contents.ShouldBe(cols);
    }

    [TestMethod]
    public async Task LoadAsync1_Throws_IfNameIsNullOrEmpty()
    {
        await Should.ThrowAsync<ArgumentException>(() => FigletFont.LoadAsync("", new StringReader(string.Empty)));
    }

    [TestMethod]
    public async Task LoadAsync1_Throws_IfReaderIsNull()
    {
        await Should.ThrowAsync<ArgumentNullException>(() => FigletFont.LoadAsync("name", null!));
    }

    [TestMethod]
    public async Task LoadAsync1_Throws_IfFailedToLoadFont()
    {
        await Should.ThrowAsync<FormatException>(() => FigletFont.LoadAsync("font", new StringReader(string.Empty)));
    }

    [TestMethod]
    public async Task LoadAsync1_LoadsTheFontAsExpected()
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
    public async Task LoadAsync2_Throws_IfPathIsNullOrEmpty()
    {
        var dn = new Mock<IDotNetSystemAdapter>();

        await Should.ThrowAsync<ArgumentException>(() => FigletFont.LoadAsync(dn.Object, ""));
    }

    [TestMethod]
    public async Task LoadAsync2_Throws_IfFailedToOpenFile()
    {
        var dn = new Mock<IDotNetSystemAdapter>();
        dn.Setup(s => s.OpenFileAsText(It.IsAny<string>()))
          .Throws<FileNotFoundException>();

        await Should.ThrowAsync<FileNotFoundException>(() => FigletFont.LoadAsync(dn.Object, "font"));

        dn.Verify(v => v.GetFileName(It.IsAny<string>()), Times.Never);
        dn.Verify(v => v.OpenFileAsText("font"), Times.Once);
    }

    [TestMethod]
    public async Task LoadAsync2_LoadsTheFontAsExpected()
    {
        const string f = "Fixtures/big.flf";
        var dn = new Mock<IDotNetSystemAdapter>();

        dn.Setup(s => s.GetFileName(f))
          .Returns("font_name");

        dn.Setup(s => s.OpenFileAsText(f))
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
