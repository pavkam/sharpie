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
public class ColorMixtureTests
{
    [TestMethod] public void Default_HasCorrectHandle() { ColorMixture.Default.Handle.ShouldBe((ushort) 0); }

    [TestMethod] public void Ctor_StoresTheHandle() { new ColorMixture { Handle = 100 }.Handle.ShouldBe((ushort) 100); }

    [TestMethod]
    public void ToString_ProperlyFormats()
    {
        new ColorMixture { Handle = 999 }.ToString()
                                         .ShouldBe("Mixture [999]");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotColorMixture(object? b)
    {
        ColorMixture.Default.Equals(b)
                    .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentHandle()
    {
        ColorMixture.Default.Equals(new ColorMixture { Handle = 1 })
                    .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfSameHandle()
    {
        ColorMixture.Default.Equals(new ColorMixture { Handle = 0 })
                    .ShouldBeTrue();
    }

    [TestMethod]
    public void HashCode_IsTheSame_ForSameHandles()
    {
        var cm1 = new ColorMixture { Handle = 1 };
        var cm2 = new ColorMixture { Handle = 1 };

        cm1.GetHashCode()
           .ShouldBe(cm2.GetHashCode());
    }
    
    [TestMethod]
    public void HashCode_IsTDifferent_ForDifferentHandles()
    {
        var cm1 = new ColorMixture { Handle = 1 };
        var cm2 = new ColorMixture { Handle = 2 };

        cm1.GetHashCode()
           .ShouldNotBe(cm2.GetHashCode());
    }
  
    [TestMethod]
    public void EqualOperator_ReturnsFalse_IfDifferentHandle()
    {
        Assert.IsFalse(new ColorMixture { Handle = 1 } == ColorMixture.Default);
    }

    [TestMethod]
    public void EqualOperator_ReturnsTrue_IfSameHandle()
    {
        Assert.IsTrue(new ColorMixture { Handle = 0 } == ColorMixture.Default);
    }
    
    [TestMethod]
    public void NotEqualOperator_ReturnsTrue_IfDifferentHandle()
    {
        Assert.IsTrue(new ColorMixture { Handle = 1 } != ColorMixture.Default);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsFalse_IfSameHandle()
    {
        Assert.IsFalse(new ColorMixture { Handle = 0 } != ColorMixture.Default);
    }
}
