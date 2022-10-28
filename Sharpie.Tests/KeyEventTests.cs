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
public class KeyEventTests
{
    private readonly KeyEvent _event1 = new(Key.F1, new('a'), "name", ModifierKey.Alt);
    
    [TestMethod]
    public void Ctr_InitializesPropertiesCorrectly()
    {
        _event1.Type.ShouldBe(EventType.KeyPress);
        _event1.Key.ShouldBe(Key.F1);
        _event1.Char.ShouldBe(new('a'));
        _event1.Name.ShouldBe("name");
        _event1.Modifiers.ShouldBe(ModifierKey.Alt);
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsKeyPress_WithNoModifiers_AndNoName()
    {
        var e = new KeyEvent(Key.F1, new('\0'), null, ModifierKey.None);
        
        e.ToString()
         .ShouldBe("Key [F1]");
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsKeyPress_WithNoModifiers_AndName()
    {
        var e = new KeyEvent(Key.F1, new('\0'), "name", ModifierKey.None);
        
        e.ToString()
         .ShouldBe("Key [name]");
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsKeyPress_WithModifiers_AndName()
    {
        var e = new KeyEvent(Key.F1, new('\0'), "name", ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift);
        
        e.ToString()
         .ShouldBe("Key [CTRL-SHIFT-ALT-name]");
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsChar_WithNoModifiers_AndNoName()
    {
        var e = new KeyEvent(Key.Character, new('a'), null, ModifierKey.None);
        
        e.ToString()
         .ShouldBe("Key ['a']");
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsChar_WithNoModifiers_AndName()
    {
        var e = new KeyEvent(Key.Character, new('a'), "name", ModifierKey.None);
        
        e.ToString()
         .ShouldBe("Key ['name']");
    }
    
    [TestMethod]
    public void ToString_ProperlyFormatsChar_WithModifiers_AndName()
    {
        var e = new KeyEvent(Key.Character, new('a'), "name", ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift);
        
        e.ToString()
         .ShouldBe("Key [CTRL-SHIFT-ALT-'name']");
    }
    
    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotKeyEvent(object? b)
    {
        _event1.Equals(b)
               .ShouldBeFalse();
    }
    
    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentKey()
    {
        _event1.Equals(new KeyEvent(Key.F2, _event1.Char, _event1.Name, _event1.Modifiers))
               .ShouldBeFalse();
    }
    
    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentChar()
    {
        _event1.Equals(new KeyEvent(_event1.Key, new('b'), _event1.Name, _event1.Modifiers))
               .ShouldBeFalse();
    }
    
    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentName()
    {
        _event1.Equals(new KeyEvent(_event1.Key, _event1.Char, "other", _event1.Modifiers))
               .ShouldBeFalse();
    }
    
    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentModifiers()
    {
        _event1.Equals(new KeyEvent(_event1.Key, _event1.Char, _event1.Name, ModifierKey.Shift))
               .ShouldBeFalse();
    }
    
    [TestMethod]
    public void Equals_ReturnsTrue_IfAllPropertiesAreSame()
    {
        _event1.Equals(new KeyEvent(_event1.Key, _event1.Char, _event1.Name, _event1.Modifiers))
               .ShouldBeTrue();
    }
    
    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentKey()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new KeyEvent(Key.F2, _event1.Char, _event1.Name, _event1.Modifiers).GetHashCode());
    }
    
    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentChar()
    {
        _event1.GetHashCode()
            .ShouldNotBe(new KeyEvent(_event1.Key, new('b'), _event1.Name, _event1.Modifiers).GetHashCode());
    }
    
    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentName()
    {
        _event1.GetHashCode()
            .ShouldNotBe(new KeyEvent(_event1.Key, _event1.Char, "other", _event1.Modifiers).GetHashCode());
    }
    
    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentModifiers()
    {
        _event1.GetHashCode()
            .ShouldNotBe(new KeyEvent(_event1.Key, _event1.Char, _event1.Name, ModifierKey.Shift).GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsEqual_IfAllPropertiesAreSame()
    {
        _event1.GetHashCode()
               .ShouldBe(new KeyEvent(_event1.Key, _event1.Char, _event1.Name, _event1.Modifiers).GetHashCode());
    }
}