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
public class AsciiFontTests
{
    private static readonly Style Style1 = new()
    {
        Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 }
    };

    private class TestAsciiFont: AsciiFont
    {
        public Rune[]? GetGlyphsChars;
        public Style? GetGlyphsStyle;
        public readonly IDrawable Drawable = new Mock<IDrawable>().Object;

        public TestAsciiFont(string name, int height, int baseline, AsciiFontLayout layout): base(name, height,
            baseline, layout)
        {
        }

        public override bool HasGlyph(Rune @char) => false;

        public override IDrawable GetGlyphs(ReadOnlySpan<Rune> chars, Style style)
        {
            GetGlyphsChars = chars.ToArray();
            GetGlyphsStyle = style;

            return Drawable;
        }
    }

    [TestMethod]
    public void Ctor_Throws_IfNameIsNullOrEmpty()
    {
        Should.Throw<ArgumentException>(() => new TestAsciiFont("", 8, 6, AsciiFontLayout.FullWidth));
    }

    [TestMethod]
    public void Ctor_Throws_IfHeightLessThanOne()
    {
        Should.Throw<ArgumentException>(() => new TestAsciiFont("name", 0, 6, AsciiFontLayout.FullWidth));
    }

    [TestMethod]
    public void Ctor_Throws_IfBaselineLessThanOne()
    {
        Should.Throw<ArgumentException>(() => new TestAsciiFont("name", 8, 0, AsciiFontLayout.FullWidth));
    }

    [TestMethod]
    public void Ctor_Throws_IfBaselineLessGreaterThanHeight()
    {
        Should.Throw<ArgumentException>(() => new TestAsciiFont("name", 8, 9, AsciiFontLayout.FullWidth));
    }

    [TestMethod]
    public void Ctor_SetsTheExpectedProperties()
    {
        var f = new TestAsciiFont("name", 8, 6, AsciiFontLayout.Fitted);

        f.Name.ShouldBe("name");
        f.Height.ShouldBe(8);
        f.Baseline.ShouldBe(6);
        f.Layout.ShouldBe(AsciiFontLayout.Fitted);
    }

    [TestMethod]
    public void GetGlyphs2_CallsGetGlyphs1()
    {
        var f = new TestAsciiFont("name", 8, 6, AsciiFontLayout.Fitted);
        const string str = "hello";
        var arr = str.EnumerateRunes()
                     .ToArray();

        f.GetGlyphs(str, Style1).ShouldBe(f.Drawable);

        f.GetGlyphsChars.ShouldBe(arr);
        f.GetGlyphsStyle.ShouldBe(Style1);
    }

    [TestMethod]
    public void GetGlyph_CallsGetGlyphs1()
    {
        var f = new TestAsciiFont("name", 8, 6, AsciiFontLayout.Fitted);
        f.GetGlyph(new('A'), Style1).ShouldBe(f.Drawable);

        f.GetGlyphsChars.ShouldBe(new[] { new Rune('A') });
        f.GetGlyphsStyle.ShouldBe(Style1);
    }
}
