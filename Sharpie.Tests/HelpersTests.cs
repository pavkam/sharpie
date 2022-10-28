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

using System.Text;
using Curses;

[TestClass]
public class HelpersTests
{
    private Mock<ICursesProvider> _cursesMock = null!;

    [TestInitialize] public void TestInitialize() { _cursesMock = new(); }

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
        var exception = Should.Throw<CursesException>(() => { (-1).Check("operation", "message"); });
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
        var exception = Should.Throw<CursesException>(() => { IntPtr.Zero.Check("operation", "message"); });
        exception.Operation.ShouldBe("operation");
        exception.Message.ShouldBe("The call to operation failed: message");
    }

    [TestMethod]
    public void ConvertMillisToTenths_Throws_IfValueIsNegative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { Helpers.ConvertMillisToTenths(-1); });
    }

    [TestMethod]
    public void ConvertMillisToTenths_ReturnsZero_IfValueIsZero()
    {
        Helpers.ConvertMillisToTenths(0)
               .ShouldBe(0);
    }

    [TestMethod]
    public void ConvertMillisToTenths_RoundsUp_IfValueBelow100()
    {
        Helpers.ConvertMillisToTenths(1)
               .ShouldBe(1);
    }

    [TestMethod]
    public void ConvertMillisToTenths_RoundsUp_IfValueBelowMid100s()
    {
        Helpers.ConvertMillisToTenths(450)
               .ShouldBe(5);
    }

    [TestMethod]
    public void ConvertMillisToTenths_AppliesMaximum()
    {
        Helpers.ConvertMillisToTenths(256000)
               .ShouldBe(255);
    }

    [TestMethod, DataRow(0x1F, "\x241F"), DataRow(0x7F, "\x247F"), DataRow(0x9F, "\x249F")]
    public void ToComplexChar_ConvertsSpecialAsciiToUnicode(int ch, string expected)
    {
        _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<ushort>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, expected, It.IsAny<uint>(), It.IsAny<ushort>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod, DataRow(0x20, "\x0020"), DataRow(0x7E, "\x007E"), DataRow(0xA0, "\x00A0")]
    public void ToComplexChar_DoesNotConvertOtherAsciiToUnicode(int ch, string expected)
    {
        _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<ushort>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, expected, It.IsAny<uint>(), It.IsAny<ushort>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod]
    public void ToComplexChar_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<ushort>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => _cursesMock.Object.ToComplexChar(new('a'), Style.Default));
    }

    [TestMethod]
    public void FromComplexChar_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(), out It.Ref<uint>.IsAny,
                       out It.Ref<ushort>.IsAny, It.IsAny<IntPtr>()))
                   .Returns(-1);

        var c = new ComplexChar();
        Should.Throw<CursesException>(() => _cursesMock.Object.FromComplexChar(c));
    }

    [TestMethod]
    public void FromComplexChar_ReturnsCursesChar()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(), out It.Ref<uint>.IsAny,
                       out It.Ref<ushort>.IsAny, It.IsAny<IntPtr>()))
                   .Returns((ComplexChar _, StringBuilder sb, out uint attrs, out ushort colorPair,
                       IntPtr _) =>
                   {
                       sb.Append('H');
                       attrs = (uint) VideoAttribute.Dim;
                       colorPair = 10;
                       return 0;
                   });

        var (rune, style) = _cursesMock.Object.FromComplexChar(new());
        rune.ShouldBe(new('H'));
        style.Attributes.ShouldBe(VideoAttribute.Dim);
        style.ColorMixture.ShouldBe(new() { Handle = 10 });
    }
}
