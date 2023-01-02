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
public class ComplexCharTests
{
    [TestMethod]
    public void Ctor_StoresTheRawValue()
    {
        new ComplexChar("hello").Raw.ShouldBe("hello");
    }

    [TestMethod]
    public void ToString_ProperlyFormats_UsingPayload()
    {
        new ComplexChar("hello").ToString().ShouldBe("hello");
        new ComplexChar(123).ToString().ShouldBe("123");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotComplexChar(object? b)
    {
        var c = new ComplexChar(1);
        c.Equals(b)
         .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentRaw()
    {
        var c1 = new ComplexChar(1);
        var c2 = new ComplexChar(2);
        
        c1.Equals(c2)
          .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfSameRaw()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("hello");
        
        c1.Equals(c2)
          .ShouldBeTrue();
    }

    [TestMethod]
    public void GetHashCode_IsTheSame_ForSameRaws()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("hello");

        c1.GetHashCode()
          .ShouldBe(c2.GetHashCode());
    }
    
    [TestMethod]
    public void GetHashCode_IsZero_ForNullRaw()
    {
        var c = new ComplexChar(null);

        c.GetHashCode()
          .ShouldBe(0);
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_ForDifferentRaws()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("world");

        c1.GetHashCode()
           .ShouldNotBe(c2.GetHashCode());
    }

    [TestMethod]
    public void EqualOperator_ReturnsFalse_IfDifferentRaw()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("world");
        
        Assert.IsFalse(c1 == c2);
    }

    [TestMethod]
    public void EqualOperator_ReturnsTrue_IfSameRaw()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("hello");
        
        Assert.IsTrue(c1 == c2);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsTrue_IfDifferentRaw()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("world");
        
        Assert.IsTrue(c1 != c2);
    }

    [TestMethod]
    public void NotEqualOperator_ReturnsFalse_IfSameRaw()
    {
        var c1 = new ComplexChar("hello");
        var c2 = new ComplexChar("hello");

        Assert.IsFalse(c1 != c2);
    }
}
