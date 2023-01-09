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
public class StyledTextTests
{
    private readonly Style _style = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 1 } };

    [TestMethod]
    public void Ctor_StoresTheTextAndStyle()
    {
        new StyledText("text", _style).Parts.ShouldBe(new[] { ("text", _style) });
    }

    [TestMethod]
    public void Ctor_Throws_IfTextIsNull() { Should.Throw<ArgumentNullException>(() => new StyledText(null!, _style)); }

    [TestMethod]
    public void ToString_ReturnsNullIfUninitialized()
    {
        new StyledText().ToString()
                        .ShouldBeNull();
    }

    [TestMethod]
    public void ToString_ProperlyFormats()
    {
        new StyledText("text", _style).Plus("cool",
                                          new()
                                          {
                                              Attributes = VideoAttribute.Blink, ColorMixture = new() { Handle = 2 }
                                          })
                                      .ToString()
                                      .ShouldBe("\"text\" @ Bold, #0001, \"cool\" @ Blink, #0002");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotSameObjectType(object? b)
    {
        new StyledText("text", _style).Equals(b)
                                      .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfLhsNotInitialized()
    {
        new StyledText().Equals(new StyledText("text", _style))
                        .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfRhsNotInitialized()
    {
        new StyledText("text", _style).Equals(new StyledText())
                                      .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfLengthsAreDifferent()
    {
        new StyledText("text", _style).Equals(new StyledText("text", _style).Plus("2", _style))
                                      .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentParts()
    {
        var v1 = new StyledText("text1", _style);
        var v2 = new StyledText("text2", _style);

        v1.Equals(v2)
          .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfAllPropertiesAreSame()
    {
        var v1 = new StyledText("text", _style).Plus("2", _style);
        var v2 = new StyledText("text", _style).Plus("2", _style);

        v1.Equals(v2)
          .ShouldBeTrue();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfBothAreNotInitialized()
    {
        var v1 = new StyledText();
        var v2 = new StyledText();

        v1.Equals(v2)
          .ShouldBeTrue();
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_WhenPartsAreDifferent()
    {
        var v1 = new StyledText("text", _style);
        var v2 = new StyledText("text",
            new() { Attributes = VideoAttribute.Blink, ColorMixture = new() { Handle = 1 } });

        v1.GetHashCode()
          .ShouldNotBe(v2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_WhenSomePartsAreDifferent()
    {
        var v1 = new StyledText("text", _style).Plus("2", _style);
        var v2 = new StyledText("text", _style).Plus("3", _style);

        v1.GetHashCode()
          .ShouldNotBe(v2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsZeroWhenNotInitialized()
    {
        new StyledText().GetHashCode()
                        .ShouldNotBe(0);
    }

    [TestMethod]
    public void GetHashCode_IsTheSame_WhenPartsAreEqual()
    {
        var v1 = new StyledText("text", _style).Plus("2", _style);
        var v2 = new StyledText("text", _style).Plus("2", _style);

        v1.GetHashCode()
          .ShouldBe(v2.GetHashCode());
    }

    [TestMethod]
    public void EqualOperator_ReturnsFalse_IfNotEqual()
    {
        var v1 = new StyledText("text1", _style);
        var v2 = new StyledText("text2", _style);

        Assert.IsFalse(v1 == v2);
    }

    [TestMethod]
    public void EqualOperator_ReturnsTrue_IfEqual()
    {
        var v1 = new StyledText("text", _style);
        var v2 = new StyledText("text", _style);

        Assert.IsTrue(v1 == v2);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsTrue_IfDifferent()
    {
        var v1 = new StyledText("text1", _style);
        var v2 = new StyledText("text2", _style);

        Assert.IsTrue(v1 != v2);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsFalse_IfEqual()
    {
        var v1 = new StyledText("text", _style);
        var v2 = new StyledText("text", _style);

        Assert.IsFalse(v1 != v2);
    }

    [TestMethod]
    public void PlusOperator_Concatenates()
    {
        var v1 = new StyledText("text1", _style);
        var v2 = new StyledText("text2", _style);

        (v1 + v2).Parts.ShouldBe(new[] { ("text1", _style), ("text2", _style) });
    }

    [TestMethod]
    public void Plus1_Throws_IfTextIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new StyledText("text", _style).Plus(null!, _style));
    }

    [TestMethod]
    public void Plus1_UsesOtherText_IfThisIsUninitialized()
    {
        new StyledText().Plus("text", _style)
                        .Parts.ShouldBe(new[] { ("text", _style) });
    }

    [TestMethod]
    public void Plus1_ConcatenatesWithOtherText()
    {
        new StyledText("text1", _style).Plus("text2", _style)
                                       .Parts.ShouldBe(new[] { ("text1", _style), ("text2", _style) });
    }

    [TestMethod]
    public void Plus2_UsesOtherText_IfThisIsUninitialized()
    {
        new StyledText().Plus(new("text", _style))
                        .Parts.ShouldBe(new[] { ("text", _style) });
    }

    [TestMethod]
    public void Plus2_UsesThisText_IfOtherIsUninitialized()
    {
        new StyledText("text", _style).Plus(new())
                                      .Parts.ShouldBe(new[] { ("text", _style) });
    }

    [TestMethod]
    public void Plus2_ConcatenatesWithOtherText()
    {
        new StyledText("text1", _style).Plus(new("text2", _style))
                                       .Parts.ShouldBe(new[] { ("text1", _style), ("text2", _style) });
    }
}
