/*
Copyright (c) 2022, Alexandru Ciobanu
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

[TestClass]
public class DosCp866AsciiFontTests
{
    private readonly DosCp866AsciiFont _font = new();
    private readonly Style _style1 = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 } };

    [TestMethod, DataRow(0, true), DataRow(255, true), DataRow(256, false)]
    public void HasGlyph_ChecksIfGlyphInRange(int c, bool t)
    {
        _font.HasGlyph(new(c))
             .ShouldBe(t);
    }

    [TestMethod] public void Name_ReturnsTheExpectedValue() { _font.Name.ShouldBe("CP866 Block Characters"); }

    [TestMethod]
    public void GetGlyph_ReturnsA4By4Drawable()
    {
        var glyph = _font.GetGlyph(new('A'), _style1);
        glyph.Size.ShouldBe(new(4, 4));
    }

    [TestMethod]
    public void GetGlyph_ReturnsTheExpectedGlyph_IfFound()
    {
        var glyph = _font.GetGlyph(new('A'), _style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new('▗'), _style1), (new('█'), _style1), (new('█'), _style1), (new('▀'), _style1) },
            { (new('█'), _style1), (new(' '), _style1), (new('▀'), _style1), (new(' '), _style1) },
            { (new('▖'), _style1), (new('█'), _style1), (new('█'), _style1), (new('▀'), _style1) },
            { (new(' '), _style1), (new(' '), _style1), (new(' '), _style1), (new Rune(' '), _style1) }
        };

        contents.ShouldBe(cols);
    }
    
    [TestMethod]
    public void GetGlyph_ReturnsTheDefault_IfNotFound()
    {
        var glyph = _font.GetGlyph(new(256), _style1);
        var contents = glyph.GetContents();

        var cols = new[,]
        {
            { (new('┌'), _style1), (new('│'), _style1), (new('│'), _style1), (new('└'), _style1) },
            { (new('─'), _style1), (new(' '), _style1), (new(' '), _style1), (new('─'), _style1) },
            { (new('─'), _style1), (new(' '), _style1), (new(' '), _style1), (new('─'), _style1) },
            { (new('┐'), _style1), (new('│'), _style1), (new('│'), _style1), (new Rune('┘'), _style1) }
        };

        contents.ShouldBe(cols);
    }
}
