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
public class MouseActionEventTests
{
    private readonly MouseActionEvent _event1 = new(new(1, 2), MouseButton.Button2, MouseButtonState.Clicked,
        ModifierKey.Alt);

    [TestMethod]
    public void Ctor_InitializesPropertiesCorrectly()
    {
        _event1.Type.ShouldBe(EventType.MouseAction);
        _event1.Position.ShouldBe(new(1, 2));
        _event1.Button.ShouldBe(MouseButton.Button2);
        _event1.State.ShouldBe(MouseButtonState.Clicked);
        _event1.Modifiers.ShouldBe(ModifierKey.Alt);
    }

    [TestMethod]
    public void ToString_ProperlyFormatsKeyPress_WithNoModifiers()
    {
        var e = new MouseActionEvent(new(1, 2), MouseButton.Button2, MouseButtonState.Clicked, ModifierKey.None);

        e.ToString()
         .ShouldBe("Mouse Button2-Clicked @ 1x2");
    }

    [TestMethod]
    public void ToString_ProperlyFormatsKeyPress_WithModifiers()
    {
        var e = new MouseActionEvent(new(1, 2), MouseButton.Button2, MouseButtonState.Clicked,
            ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift);

        e.ToString()
         .ShouldBe("Mouse CTRL-SHIFT-ALT-Button2-Clicked @ 1x2");
    }

    [TestMethod, DataRow(null), DataRow("")]
    public void Equals_ReturnsFalse_IfNotMouseActionEvent(object? b)
    {
        _event1.Equals(b)
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentPosition()
    {
        _event1.Equals(new MouseActionEvent(new(2, 1), _event1.Button, _event1.State, _event1.Modifiers))
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentButton()
    {
        _event1.Equals(new MouseActionEvent(_event1.Position, MouseButton.Button4, _event1.State, _event1.Modifiers))
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentState()
    {
        _event1.Equals(new MouseActionEvent(_event1.Position, _event1.Button, MouseButtonState.DoubleClicked,
                   _event1.Modifiers))
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsFalse_IfDifferentModifiers()
    {
        _event1.Equals(new MouseActionEvent(_event1.Position, _event1.Button, _event1.State, ModifierKey.Shift))
               .ShouldBeFalse();
    }

    [TestMethod]
    public void Equals_ReturnsTrue_IfAllPropertiesAreSame()
    {
        _event1.Equals(new MouseActionEvent(_event1.Position, _event1.Button, _event1.State, _event1.Modifiers))
               .ShouldBeTrue();
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentPosition()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new MouseActionEvent(new(2, 1), _event1.Button, _event1.State, _event1.Modifiers)
                   .GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentButton()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new MouseActionEvent(_event1.Position, MouseButton.Button4, _event1.State,
                   _event1.Modifiers).GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentState()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new MouseActionEvent(_event1.Position, _event1.Button, MouseButtonState.DoubleClicked,
                   _event1.Modifiers).GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsDifferent_IfDifferentModifiers()
    {
        _event1.GetHashCode()
               .ShouldNotBe(new MouseActionEvent(_event1.Position, _event1.Button, _event1.State, ModifierKey.Shift)
                   .GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsEqual_IfAllPropertiesAreSame()
    {
        _event1.GetHashCode()
               .ShouldBe(new MouseActionEvent(_event1.Position, _event1.Button, _event1.State, _event1.Modifiers)
                   .GetHashCode());
    }
}
