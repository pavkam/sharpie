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

namespace Sharpie.Tests;

using Font;

[TestClass]
public class FigletFontParserTests
{
    [TestMethod, DataRow(""), DataRow("hello"), DataRow("1 2 3 4 5")]
    public void ParseHeader_Throws_IfNotEnoughComponents(string str) => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader(str));

    [TestMethod]
    public void ParseHeader_Throws_IfInvalidSignature() => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader("wrong 63 63 127 0 2"));

    [TestMethod]
    public void ParseHeader_Throws_IfHardBlankIsInvalid() => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader("flf2a$$ 8 6 59 15 10 0 24463"));

    [TestMethod, DataRow("flf2a$ -1 1 3 1 1 1 1 1 1"), DataRow("flf2a$ 1 -1 3 1 1 1 1 1 1"),
     DataRow("flf2a$ 2 1 -1 1 1 1 1 1 1"), DataRow("flf2a$ 2 1 3 -2 1 1 1 1 1"), DataRow("flf2a$ 2 1 3 1 -1 1 1 1 1"),
     DataRow("flf2a$ 2 1 3 1 1 -1 1 1 1"), DataRow("flf2a$ 2 1 3 1 1 1 -1 1 1")]
    public void ParseHeader_Throws_IfComponentsAreInvalid(string str) => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader(str));

    [TestMethod]
    public void ParseHeader_Throws_IfBaselineIsInvalid_1() => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader("flf2a$ 8 0 59 15 10 0 24463"));

    [TestMethod]
    public void ParseHeader_Throws_IfBaselineIsInvalid_2() => Should.Throw<FormatException>(() => FigletFontParser.ParseHeader("flf2a$ 8 9 59 15 10 0 24463"));

    [TestMethod]
    public void ParseHeader_ParsesTheHeaderAsExpected_1()
    {
        var h = FigletFontParser.ParseHeader("flf2a$ 8 6 59 15 10 0 24463");
        h.ShouldBe((
            new FigletHeader('$', 8, 6,
                FigletAttribute.HorizontalSmushingRule1 |
                FigletAttribute.HorizontalSmushingRule2 |
                FigletAttribute.HorizontalSmushingRule3 |
                FigletAttribute.HorizontalSmushingRule4 |
                FigletAttribute.HorizontalSmushing |
                FigletAttribute.VerticalSmushingRule1 |
                FigletAttribute.VerticalSmushingRule2 |
                FigletAttribute.VerticalSmushingRule3 |
                FigletAttribute.VerticalSmushingRule4 |
                FigletAttribute.VerticalSmushingRule5 |
                FigletAttribute.VerticalSmushing, FigletScriptDirection.LeftToRight), 59, 10));
    }

    [TestMethod]
    public void ParseHeader_ParsesTheHeaderAsExpected_2()
    {
        var h = FigletFontParser.ParseHeader("flf2a. 3 2 4 -1 55 1 0");
        h.ShouldBe((new('.', 3, 2, FigletAttribute.FullWidth, FigletScriptDirection.RightToLeft), 4, 55));
    }

    [TestMethod]
    public void ParseHeader_ParsesTheHeaderAsExpected_3()
    {
        var h = FigletFontParser.ParseHeader("flf2a 3 2 4 0 0 1 0");
        h.ShouldBe((new('\0', 3, 2, FigletAttribute.HorizontalFitting, FigletScriptDirection.RightToLeft), 4, 0));
    }

    [TestMethod]
    public void ParseHeader_ParsesTheHeaderAsExpected_4()
    {
        var h = FigletFontParser.ParseHeader("flf2a 3 2 4 15 0 1 0");
        h.ShouldBe((
            new('\0', 3, 2,
                FigletAttribute.HorizontalSmushingRule1 |
                FigletAttribute.HorizontalSmushingRule2 |
                FigletAttribute.HorizontalSmushingRule3 |
                FigletAttribute.HorizontalSmushingRule4, FigletScriptDirection.RightToLeft), 4, 0));
    }

    [TestMethod]
    public void ParseHeader_ParsesTheHeaderAsExpected_5()
    {
        var h = FigletFontParser.ParseHeader("flf2a 3 2 4 -1 0");
        h.ShouldBe((new('\0', 3, 2, FigletAttribute.FullWidth, FigletScriptDirection.LeftToRight), 4, 0));
    }

    [TestMethod]
    public void ParseCodeTag_Throws_WhenLineIsEmpty() => Should.Throw<FormatException>(() => FigletFontParser.ParseCodeTag(string.Empty));

    [TestMethod]
    public void ParseCodeTag_Throws_WhenInvalid_1() => Should.Throw<FormatException>(() => FigletFontParser.ParseCodeTag("bad"));

    [TestMethod]
    public void ParseCodeTag_Throws_WhenInvalid_2() => Should.Throw<FormatException>(() => FigletFontParser.ParseCodeTag("bad something"));

    [TestMethod, DataRow("0", 0, ""), DataRow(" 0   hello", 0, "hello"), DataRow("-1", -1, ""), DataRow("010", 8, ""),
     DataRow("-010", -8, ""), DataRow("0xFF j", 255, "j"), DataRow("-0x00FF j", -255, "j"),
     DataRow("0x02BC", 0x02BC, "")]
    public void ParseCodeTag_Returns(string str, int cp, string d)
    {
        FigletFontParser.ParseCodeTag(str)
                        .ShouldBe((cp, d));
    }

    [TestMethod]
    public void ParseCharacterRow_Throws_WhenLineIsEmpty() => Should.Throw<FormatException>(() => FigletFontParser.ParseCharacterRow(2, string.Empty));

    [TestMethod]
    public void ParseCharacterRow_Throws_WhenLengthIsGreaterThanMax() => Should.Throw<FormatException>(() => FigletFontParser.ParseCharacterRow(2, "123"));

    [TestMethod]
    public void ParseCharacterRow_CutsWithoutLastChar()
    {
        FigletFontParser.ParseCharacterRow(10, "123@")
                        .ShouldBe("123");
    }

    [TestMethod]
    public void ParseCharacterRow_CutsWithoutLastChars()
    {
        FigletFontParser.ParseCharacterRow(10, "123@@@")
                        .ShouldBe("123");
    }

    [TestMethod]
    public async Task ParseStandardCharacterAsync_Throws_IfCharacterIncompleteAsync()
    {
        const string ch = "1234567@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseStandardCharacterAsync(2, 100, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseStandardCharacterAsync_Throws_IfCharacterHasDiffLineSizesAsync()
    {
        const string ch = "1234567@\n123456@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseStandardCharacterAsync(2, 100, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseStandardCharacterAsync_Throws_IfCharacterLineTooLongAsync()
    {
        const string ch = "1@\n12@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseStandardCharacterAsync(2, 2, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseStandardCharacterAsync_ReturnsCharacterAndLines_1Async()
    {
        const string ch = "123@@";

        var (rows, width) = await FigletFontParser.ParseStandardCharacterAsync(1, 100, new StringReader(ch));

        rows.ShouldBe(new[] { "123" });
        width.ShouldBe(3);
    }

    [TestMethod]
    public async Task ParseStandardCharacterAsync_ReturnsCharacterAndLines_2Async()
    {
        const string ch = "12@\n34@\n56@@";

        var (rows, width) = await FigletFontParser.ParseStandardCharacterAsync(3, 100, new StringReader(ch));

        rows.ShouldBe(new[] { "12", "34", "56" });
        width.ShouldBe(2);
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_Throws_IfCodePointInvalidAsync()
    {
        const string ch = "ab\n123@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseCodeTaggedCharacterAsync(1, 100, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_Throws_IfCharacterIncompleteAsync()
    {
        const string ch = "10\n1234567@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseCodeTaggedCharacterAsync(2, 100, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_Throws_IfCharacterHasDiffLineSizesAsync()
    {
        const string ch = "10\n1234567@\n123456@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseCodeTaggedCharacterAsync(2, 100, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_Throws_IfCharacterLineTooLongAsync()
    {
        const string ch = "10\n1@\n12@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseCodeTaggedCharacterAsync(2, 2, new StringReader(ch)));
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_ReturnsCharacterAndLines_1Async()
    {
        const string ch = "10\n123@@";

        var (rows, width, cp) = await FigletFontParser.ParseCodeTaggedCharacterAsync(1, 100, new StringReader(ch));

        rows.ShouldBe(new[] { "123" });
        width.ShouldBe(3);
        cp.ShouldBe(10);
    }

    [TestMethod]
    public async Task ParseCodeTaggedCharacterAsync_ReturnsCharacterAndLines_2Async()
    {
        const string ch = "  -0xffff hello world  \n12@\n34@\n56@@";

        var (rows, width, cp) = await FigletFontParser.ParseCodeTaggedCharacterAsync(3, 100, new StringReader(ch));

        rows.ShouldBe(new[] { "12", "34", "56" });
        width.ShouldBe(2);
        cp.ShouldBe(-0xffff);
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_Throws_IfNoCharsAndExpectedCommentsAsync()
    {
        const string fnt = "flf2a 3 2 4 15 1";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseFontFileAsync(0, 0, new StringReader(fnt)));
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_ReturnsNoCharacters_IfNoneExpectedAsync()
    {
        const string fnt = "flf2a 3 2 4 -1 1\nthis is a comment line";

        var (header, chars) = await FigletFontParser.ParseFontFileAsync(0, 0, new StringReader(fnt));

        header.ShouldBe(new('\0', 3, 2, FigletAttribute.FullWidth, FigletScriptDirection.LeftToRight));

        chars.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_Throws_IfExpectedOneCharAsync()
    {
        const string fnt = "flf2a 3 2 4 15 1\ncomment";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseFontFileAsync(0, 1, new StringReader(fnt)));
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_Throws_IfCharHAsLongedWidthThanExpectedAsync()
    {
        const string fnt = "flf2a 2 1 2 -1 1\nthis is a comment line\n123@\n456@@";

        _ = await Should.ThrowAsync<FormatException>(() =>
            FigletFontParser.ParseFontFileAsync(0, 1, new StringReader(fnt)));
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_ReturnsHeaderAndOneCharAsync()
    {
        const string fnt = "flf2a 2 1 5 -1 1\nthis is a comment line\n123@\n456@@";

        var (_, chars) = await FigletFontParser.ParseFontFileAsync(0, 1, new StringReader(fnt));

        chars.Count.ShouldBe(1);
        var (rows, width) = chars[0];

        rows.ShouldBe(new[] { "123", "456" });
        width.ShouldBe(3);
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_ReturnsHeaderAndOneTaggedCharAsync()
    {
        const string fnt = "flf2a 2 1 5 -1 1\nthis is a comment line\n200\n123@\n456@@";

        var (_, chars) = await FigletFontParser.ParseFontFileAsync(0, 0, new StringReader(fnt));

        chars.Count.ShouldBe(1);
        var (rows, width) = chars[200];

        rows.ShouldBe(new[] { "123", "456" });
        width.ShouldBe(3);
    }

    [TestMethod]
    public async Task ParseFontFileAsync1_ReturnsHeaderAndTwoCharsAsync()
    {
        const string fnt = "flf2a 2 1 5 -1 1\nthis is a comment line\n123@\n456@@\n200\nabc@\ndef@@";

        var (_, chars) = await FigletFontParser.ParseFontFileAsync(0, 1, new StringReader(fnt));

        chars.Count.ShouldBe(2);
        var (rows, width) = chars[0];
        rows.ShouldBe(new[] { "123", "456" });
        width.ShouldBe(3);

        (rows, width) = chars[200];
        rows.ShouldBe(new[] { "abc", "def" });
        width.ShouldBe(3);
    }

    [TestMethod]
    public async Task ParseFontFileAsync2_ParsesTheFileAsExpectedAsync()
    {
        using var reader = new StreamReader("Fixtures/big.flf");
        var (header, chars) = await FigletFontParser.ParseFontFileAsync(reader);

        header.ShouldBe(new('$', 8, 6,
            FigletAttribute.HorizontalSmushingRule1 |
            FigletAttribute.HorizontalSmushingRule2 |
            FigletAttribute.HorizontalSmushingRule3 |
            FigletAttribute.HorizontalSmushingRule4 |
            FigletAttribute.HorizontalSmushing |
            FigletAttribute.VerticalSmushingRule1 |
            FigletAttribute.VerticalSmushingRule2 |
            FigletAttribute.VerticalSmushingRule3 |
            FigletAttribute.VerticalSmushingRule4 |
            FigletAttribute.VerticalSmushingRule5 |
            FigletAttribute.VerticalSmushing, FigletScriptDirection.LeftToRight));

        chars.Count.ShouldBe(255);

        var (rows, width) = chars['!'];
        rows.ShouldBe(new[] { "  _ ", " | |", " | |", " | |", " |_|", " (_)", "    ", "    " });
        width.ShouldBe(4);

        (rows, width) = chars[0x03A9];
        rows.ShouldBe(new[]
        {
            @"    ____   ",
            @"   / __ \  ",
            @"  | |  | | ",
            @"  | |  | | ",
            @"  _\ \/ /_ ",
            @" (___||___)",
            @"           ",
            @"           "
        });

        width.ShouldBe(11);
    }
}
