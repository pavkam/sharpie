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

[TestClass]
public class DelegateEventTests
{
    private readonly DelegateEvent _event1 = new("A");

    [TestMethod]
    public void Ctor_InitializesPropertiesCorrectly()
    {
        _event1.Type.ShouldBe(EventType.Delegate);
        _event1.Object.ShouldBe("A");
    }

    [TestMethod]
    public void Ctor_Throws_IfObjectIsNull() { Should.Throw<ArgumentNullException>(() => new DelegateEvent(null!)); }

    [TestMethod]
    public void ToString_ProperlyFormats()
    {
        _event1.ToString()
               .ShouldBe("Delegate [A]");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotSameType(object? b)
    {
        _event1.Equals(b)
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfObjectDifferent()
    {
        _event1.Equals(new DelegateEvent("B"))
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfObjectEquals()
    {
        _event1.Equals(new DelegateEvent("A"))
               .ShouldBeTrue();
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_IfObjectDifferent()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new DelegateEvent("B").GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsEqual_IfObjectSame()
    {
        _event1.GetHashCode()
               .ShouldBe(new DelegateEvent("A").GetHashCode());
    }
}
