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
public class StyleTests
{
    private readonly Style _style = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 1 } };

    [TestMethod]
    public void Default_HasValues()
    {
        Style.Default.Attributes.ShouldBe(VideoAttribute.None);
        Style.Default.ColorMixture.ShouldBe(ColorMixture.Default);
    }

    [TestMethod]
    public void Ctor_StoresTheProperties()
    {
        _style.Attributes.ShouldBe(VideoAttribute.Bold);
        _style.ColorMixture.ShouldBe(new() { Handle = 1 });
    }

    [TestMethod]
    public void ToString_ProperlyFormats()
    {
        _style.ToString()
              .ShouldBe("Bold, #0001");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotStyle(object? b)
    {
        _style.Equals(b)
                    .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentVideoAttributes()
    {
        _style.Equals(_style with { Attributes = VideoAttribute.Dim })
              .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentColorMixtures()
    {
        _style.Equals(_style with { ColorMixture = new() { Handle = 10 } })
              .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfAllPropertiesAreSame()
    {
        _style.Equals(new Style { Attributes = _style.Attributes, ColorMixture = _style.ColorMixture })
              .ShouldBeTrue();
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_ForDifferentAttributes()
    {
        _style.GetHashCode()
              .ShouldNotBe((_style with { Attributes = VideoAttribute.Dim }).GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_ForDifferentColorMixtures()
    {
        _style.GetHashCode()
              .ShouldNotBe((_style with { ColorMixture = new() { Handle = 10 } }).GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsTheSame_IfAllPropertiesAreSame()
    {
        _style.GetHashCode()
              .ShouldBe(new Style { Attributes = _style.Attributes, ColorMixture = _style.ColorMixture }.GetHashCode());
    }

    [TestMethod]
    public void EqualOperator_ReturnsFalse_IfNotEqual()
    {
        Assert.IsFalse(_style == _style with { Attributes = VideoAttribute.Underline });
    }

    [TestMethod]
    public void EqualOperator_ReturnsTrue_IfEqual()
    {
        Assert.IsTrue(new Style { Attributes = _style.Attributes, ColorMixture = _style.ColorMixture } == _style);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsTrue_IfDifferent()
    {
        Assert.IsTrue(_style != Style.Default with { Attributes = VideoAttribute.Underline });
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsFalse_IfEqual()
    {
        Assert.IsFalse(new Style { Attributes = _style.Attributes, ColorMixture = _style.ColorMixture } != _style);
    }
}
