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

[TestClass]
public class HelpersTests
{
    private Mock<ICursesBackend> _cursesMock = null!;

    [TestInitialize]
    public void TestInitialize() => _cursesMock = new();

    [TestMethod]
    public void Failed_ReturnsTrue_IfCodeIsMinus1()
    {
        (-1).Failed()
            .ShouldBeTrue();
    }

    [TestMethod]
    public void Failed_ReturnsFalse_IfCodeIsNotMinus1()
    {
        1.Failed()
         .ShouldBeFalse();
    }

    [TestMethod]
    public void Check_ReturnsCode_IfCodeIsNotMinus1()
    {
        1.Check("operation", "message")
         .ShouldBe(1);
    }

    [TestMethod]
    public void Check_Throws_IfCodeIsMinus1()
    {
        var exception = Should.Throw<CursesOperationException>(() => { _ = (-1).Check("operation", "message"); });
        exception.Operation.ShouldBe("operation");
        exception.Message.ShouldBe("The call to operation failed: message");
    }

    [TestMethod]
    public void Check_ReturnsPointer_IfNotZeroPointer()
    {
        new IntPtr(1).Check("operation", "message")
                     .ShouldBe(new(1));
    }

    [TestMethod]
    public void Check_Throws_IfZeroPointer()
    {
        var exception = Should.Throw<CursesOperationException>(() => { _ = IntPtr.Zero.Check("operation", "message"); });
        exception.Operation.ShouldBe("operation");
        exception.Message.ShouldBe("The call to operation failed: message");
    }

    [TestMethod, DataRow(0x1F, "\x241F"), DataRow(0x7F, "\x247F"), DataRow(0x9F, "\x249F")]
    public void ToComplexChar_ConvertsSpecialAsciiToUnicode(int ch, string expected)
    {
        _ = _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _ = _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, expected, It.IsAny<VideoAttribute>(), It.IsAny<short>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod, DataRow(0x20, "\x0020"), DataRow(0x7E, "\x007E"), DataRow(0xA0, "\x00A0"),
     DataRow((int) ControlCharacter.NewLine, "\n"), DataRow((int) '\b', "\b"),
     DataRow((int) ControlCharacter.Tab, "\t")]
    public void ToComplexChar_DoesNotConvertOtherAsciiToUnicode(int ch, string expected)
    {
        _ = _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _ = _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, expected, It.IsAny<VideoAttribute>(), It.IsAny<short>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod]
    public void ToComplexChar_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        _ = Should.Throw<CursesOperationException>(() => _cursesMock.Object.ToComplexChar(new('a'), Style.Default));
    }

    [TestMethod]
    public void FromComplexChar_Throws_IfCursesFails()
    {
        var ch = new ComplexChar("test");

        _ = _cursesMock.Setup(s => s.getcchar(ch, It.IsAny<StringBuilder>(), out It.Ref<VideoAttribute>.IsAny,
                       out It.Ref<short>.IsAny, It.IsAny<IntPtr>()))
                   .Returns(-1);

        _ = Should.Throw<CursesOperationException>(() => _cursesMock.Object.FromComplexChar(ch));
    }

    [TestMethod]
    public void FromComplexChar_ReturnsCursesChar()
    {
        var ch = new ComplexChar("test");
        _ = _cursesMock.Setup(s => s.getcchar(ch, It.IsAny<StringBuilder>(), out It.Ref<VideoAttribute>.IsAny,
                       out It.Ref<short>.IsAny, It.IsAny<IntPtr>()))
                   .Returns((ComplexChar _, StringBuilder sb, out VideoAttribute attrs, out short colorPair,
                       IntPtr _) =>
                   {
                       _ = sb.Append('H');
                       attrs = VideoAttribute.Dim;
                       colorPair = 10;
                       return 0;
                   });

        var (rune, style) = _cursesMock.Object.FromComplexChar(ch);
        rune.ShouldBe(new('H'));
        style.Attributes.ShouldBe(VideoAttribute.Dim);
        style.ColorMixture.ShouldBe(new()
        {
            Handle = 10
        });
    }

    [TestMethod]
    public void EnumerateInHalves_Throws_IfCountIsNegative()
    {
        _ = Should.Throw<ArgumentOutOfRangeException>(() => Helpers.EnumerateInHalves(0, -1)
                                                               .ToArray());
    }

    [TestMethod]
    public void EnumerateInHalves_ReturnsNothingIsCountIsZero()
    {
        Helpers.EnumerateInHalves(0, 0)
               .ToArray()
               .ShouldBeEmpty();
    }

    [TestMethod]
    public void EnumerateInHalves_ReturnsNothingIsCountIsLessThanHalf()
    {
        Helpers.EnumerateInHalves(0, 0.45F)
               .ToArray()
               .ShouldBeEmpty();
    }

    [TestMethod]
    public void EnumerateInHalves_ReturnsOneIfCountIsApproxHalf()
    {
        var r = Helpers.EnumerateInHalves(0, 0.7F)
                       .ToArray();

        r.Length.ShouldBe(1);
        r[0]
            .ShouldBe((0, true));
    }

    [TestMethod]
    public void EnumerateInHalves_ReturnsTwoIfCountAllows()
    {
        var r = Helpers.EnumerateInHalves(0, 1.2F)
                       .ToArray();

        r.Length.ShouldBe(2);
        r[0]
            .ShouldBe((0, true));

        r[1]
            .ShouldBe((0, false));
    }

    [TestMethod]
    public void EnumerateInHalves_UnderstandsNegativeSpectrum()
    {
        var r = Helpers.EnumerateInHalves(-1.6f, 3)
                       .ToArray();

        r.ShouldBe(new[] { (-2, true), (-1, false), (-1, true), (0, false), (0, true), (0, false) });
    }

    [TestMethod]
    public void EnumerateInHalves_ReturnsThreeIfCountAllows()
    {
        var r = Helpers.EnumerateInHalves(0, 1.5F)
                       .ToArray();

        r.Length.ShouldBe(3);
        r[0]
            .ShouldBe((0, true));

        r[1]
            .ShouldBe((0, false));

        r[2]
            .ShouldBe((1, true));
    }

    [TestMethod]
    public void EnumerateInHalves_OffsetsByStart_1()
    {
        var r = Helpers.EnumerateInHalves(0.6F, 1)
                       .ToArray();

        r.Length.ShouldBe(2);
        r[0]
            .ShouldBe((0, false));

        r[1]
            .ShouldBe((1, true));
    }

    [TestMethod]
    public void EnumerateInHalves_OffsetsByStart_2()
    {
        var r = Helpers.EnumerateInHalves(1.5F, 1.2F)
                       .ToArray();

        r.Length.ShouldBe(2);
        r[0]
            .ShouldBe((1, false));

        r[1]
            .ShouldBe((2, true));
    }

    [TestMethod]
    public void IntersectSegments_Throws_IfLength1IsLessThanZero() =>
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.IntersectSegments(0, -1, 0, 1));

    [TestMethod]
    public void IntersectSegments_Throws_IfLength2IsLessThanZero() =>
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.IntersectSegments(0, 1, 0, -1));

    [TestMethod, DataRow(0, 0, 0, 5, -1,
         0), DataRow(0, 5, 0, 0, -1,
         0), DataRow(0, 5, 6, 5, -1,
         0), DataRow(0, 5, -10, 5, -1,
         0), DataRow(-10, 20, 0, 5, 0,
         5), DataRow(0, 10, -10, 11, 0,
         1), DataRow(0, 1, 0, 5, 0,
         1), DataRow(0, 10, 0, 5, 0,
         5)]
    public void IntersectSegments_CalculatesTheProperLength(int s1, int c1, int s2, int c2,
        int si, int sc)
    {
        var (rs, rc) = Helpers.IntersectSegments(s1, c1, s2, c2);

        rs.ShouldBe(si);
        rc.ShouldBe(sc);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Fails_IfCursesFails_NoBatch()
    {
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        using var sa = new TerminalSurface(terminal, new(1));

        _ = _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => terminal.Refresh(sa))
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Fails_IfCursesFails_InBatch()
    {
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        using var sa = new TerminalSurface(terminal, new(1));

        _ = _cursesMock.Setup(s => s.wnoutrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        using (terminal.AtomicRefresh())
        {
            Should.Throw<CursesOperationException>(() => terminal.Refresh(sa))
                  .Operation.ShouldBe("wnoutrefresh");
        }
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_NoBatch()
    {
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        var sa = new TerminalSurface(terminal, new(1));

        terminal.Refresh(sa);
        _cursesMock.Verify(v => v.wrefresh(sa.Handle), Times.Once);
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_InBatch()
    {
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        var sa = new TerminalSurface(terminal, new(1));

        using (terminal.AtomicRefresh())
        {
            terminal.Refresh(sa);
        }

        _cursesMock.Verify(v => v.wnoutrefresh(sa.Handle), Times.Once);
    }

    [TestMethod]
    public void TraceLineInHalves_GeneratesTheExpectedOutput_1()
    {
        var pts = new List<PointF>();
        Helpers.TraceLineInHalves(new(0, 0), new(0.5F, 0.5F), pts.Add);

        pts.ShouldBe(new[] { new PointF(0, 0), new PointF(0.5F, 0.5F) });
    }

    [TestMethod]
    public void TraceLineInHalves_GeneratesTheExpectedOutput_2()
    {
        var pts = new List<PointF>();
        Helpers.TraceLineInHalves(new(0, 0), new(2F, 2F), pts.Add);

        pts.ShouldBe(new[]
        {
            new PointF(0, 0), new PointF(0.5F, 0.5F), new PointF(1F, 1F), new PointF(1.5F, 1.5F), new PointF(2F, 2F)
        });
    }

    [TestMethod]
    public void TraceLineInHalves_GeneratesTheExpectedOutput_3()
    {
        var pts = new List<PointF>();
        Helpers.TraceLineInHalves(new(0, 0), new(0F, 1F), pts.Add);

        pts.ShouldBe(new[] { new PointF(0, 0), new PointF(0, 0.5F), new PointF(0, 1F) });
    }

    [TestMethod]
    public void TraceLineInHalves_GeneratesTheExpectedOutput_4()
    {
        var pts = new List<PointF>();
        Helpers.TraceLineInHalves(new(0, 0), new(1F, 0F), pts.Add);

        pts.ShouldBe(new[] { new PointF(0, 0), new PointF(0.5F, 0), new PointF(1F, 0) });
    }

    [TestMethod]
    public void TraceLineInHalves_GeneratesTheExpectedOutput_5()
    {
        var pts = new List<PointF>();
        Helpers.TraceLineInHalves(new(0, 0), new(-2, -2), pts.Add);

        pts.ShouldBe(new[]
        {
            new PointF(0, 0),
            new PointF(-0.5F, -0.5F),
            new PointF(-1F, -1F),
            new PointF(-1.5F, -1.5F),
            new PointF(-2F, -2F)
        });
    }

    [TestMethod, DataRow(100, 99), DataRow(99, 100), DataRow(100, 100)]
    public void AdjustToActualArea1_ReturnsFalse_IfNewAreaIsEmpty(int x, int y)
    {
        var a = new Rectangle(x, y, 10, 10);
        new Size(100, 100).AdjustToActualArea(ref a)
                          .ShouldBeFalse();
    }

    [TestMethod]
    public void AdjustToActualArea1_ReturnsTrueAndNewArea()
    {
        var a = new Rectangle(50, 50, 100, 100);
        new Size(100, 100).AdjustToActualArea(ref a)
                          .ShouldBeTrue();

        a.ShouldBe(new(50, 50, 50, 50));
    }

    [TestMethod, DataRow(100f, 99f), DataRow(99f, 100f), DataRow(100f, 100f)]
    public void AdjustToActualArea2_ReturnsFalse_IfNewAreaIsEmpty(float x, float y)
    {
        var a = new RectangleF(x, y, 10, 10);
        new Size(100, 100).AdjustToActualArea(ref a)
                          .ShouldBeFalse();
    }

    [TestMethod]
    public void AdjustToActualArea2_ReturnsTrueAndNewArea()
    {
        var a = new RectangleF(50, 50, 100, 100);
        Helpers.AdjustToActualArea(new(100, 100), ref a)
               .ShouldBeTrue();

        a.ShouldBe(new(50, 50, 50, 50));
    }

    [TestMethod, DataRow(110, 109), DataRow(109, 110), DataRow(110, 110)]
    public void AdjustToActualArea3_ReturnsFalse_IfNewAreaIsEmpty(int x, int y)
    {
        var a = new Rectangle(x, y, 10, 10);
        new Rectangle(10, 10, 100, 100).AdjustToActualArea(ref a)
                                       .ShouldBeFalse();
    }

    [TestMethod]
    public void AdjustToActualArea3_ReturnsTrueAndNewArea()
    {
        var a = new Rectangle(50, 50, 100, 100);
        new Rectangle(10, 10, 100, 100).AdjustToActualArea(ref a)
                                       .ShouldBeTrue();

        a.ShouldBe(new(50, 50, 60, 60));
    }

    [TestMethod]
    public void GetRawValue_Throws_IfCharIsNull() =>
        Should.Throw<ArgumentNullException>(() => ((ComplexChar) null!).GetRawValue<string>());

    [TestMethod]
    public void GetRawValue_Throws_IfCharOfBadType()
    {
        var c = new ComplexChar(12);
        _ = Should.Throw<ArgumentException>(c.GetRawValue<string>);
    }

    [TestMethod]
    public void GetRawValue_ReturnsTheRawValue_IfCharIsOk()
    {
        var c = new ComplexChar(12);
        c.GetRawValue<int>()
         .ShouldBe(12);
    }
}
