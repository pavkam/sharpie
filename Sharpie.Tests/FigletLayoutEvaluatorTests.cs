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

namespace Sharpie.Tests;

using Font;

[TestClass]
public class FigletLayoutEvaluatorTests
{
    private static char Left(char l, char _) => l;
    private static char None(char l, char _) => ControlCharacter.Null;
    private static char Smush(char _, char __) => '+';

    [TestMethod]
    public void HorizontalJoin_IfFitting_AlwaysReturnsZero()
    {
        FigletLayoutEvaluator.HorizontalJoin('$', FigletAttribute.HorizontalFitting, 'A', 'B')
                             .ShouldBe(ControlCharacter.Null);
    }

    [TestMethod]
    public void HorizontalJoin_IfUniversalSmushing_AlwaysReturnsRight()
    {
        FigletLayoutEvaluator.HorizontalJoin('$', FigletAttribute.HorizontalSmushing, 'A', 'B')
                             .ShouldBe('B');
    }

    [TestMethod, DataRow('A', 'A', 'A'), DataRow('$', '$', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule1_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule1, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow('_', '|', '|'), DataRow('|', '_', '|'), DataRow('a', 'b', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule2_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule2, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow('|', '(', '('), DataRow(')', '>', '>'), DataRow('(', '\\', '('),
     DataRow('(', ')', ControlCharacter.Null), DataRow('a', 'b', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule3_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule3, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow('[', ']', '|'), DataRow(')', '(', '|'), DataRow('(', '{', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule4_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule4, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow('/', '\\', 'Y'), DataRow('\\', '/', 'Y'), DataRow('>', '<', 'X'),
     DataRow('>', '/', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule5_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule5, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow('$', '$', '$'), DataRow('$', '/', ControlCharacter.Null)]
    public void HorizontalJoin_IfSmushingRule6_ReturnsExpectedResult(char l, char r, char exp)
    {
        FigletLayoutEvaluator.HorizontalJoin('$',
                                 FigletAttribute.HorizontalSmushing | FigletAttribute.HorizontalSmushingRule6, l, r)
                             .ShouldBe(exp);
    }

    [TestMethod, DataRow(" ", " ", " "), DataRow("  ", " ", "  "), DataRow(" ", "  ", " "), DataRow("  ", "  ", "  ")]
    public void Join_RemovesAllWhiteSpacesFromRight(string ls, string rs, string exp)
    {
        FigletLayoutEvaluator.Join(Left, new[] { ls }, new[] { rs })
                             .ShouldBe(new[] { exp });
    }

    [TestMethod, DataRow("a  ", "   ", "a  "), DataRow("a  ", "  bc", "ac")]
    public void Join_RemovesAllWhiteSpacesFromRightUntilMerge(string ls, string rs, string exp)
    {
        FigletLayoutEvaluator.Join(Left, new[] { ls }, new[] { rs })
                             .ShouldBe(new[] { exp });
    }

    [TestMethod, DataRow("a  b", "   k   z", "a  bk   z"), DataRow("  start", "ed", "  started")]
    public void Join_RemovesAllWhiteSpacesFromRightUntilStop(string ls, string rs, string exp)
    {
        FigletLayoutEvaluator.Join(None, new[] { ls }, new[] { rs })
                             .ShouldBe(new[] { exp });
    }

    [TestMethod]
    public void Join_FitsTwoFiguresAsExpected_1()
    {
        var l = new[] { "####", "    " };
        var r = new[] { "    ", "####" };
        var e = new[] { "####", "####" };
        FigletLayoutEvaluator.Join(None, l, r)
                             .ShouldBe(e);
    }

    [TestMethod]
    public void Join_FitsTwoFiguresAsExpected_2()
    {
        var l = new[] { "1234", "|   " };
        var r = new[] { "   |", "5678" };
        var e = new[] { "1234|", "|5678" };
        FigletLayoutEvaluator.Join(None, l, r)
                             .ShouldBe(e);
    }

    [TestMethod]
    public void Join_FitsTwoFiguresAsExpected_3()
    {
        var l = new[] { "____", "| $ ", "|___" };
        var r = new[] { "    ", "1234", "    " };
        var e = new[] { "____   ", "| $1234", "|___   " };
        FigletLayoutEvaluator.Join(None, l, r)
                             .ShouldBe(e);
    }

    [TestMethod]
    public void Join_SmushesTwoFiguresAsExpected_1()
    {
        var l = new[] { "1234", "|   " };
        var r = new[] { "   |", "5678" };
        var e = new[] { "123+", "+678" };
        FigletLayoutEvaluator.Join(Smush, l, r)
                             .ShouldBe(e);
    }
}
