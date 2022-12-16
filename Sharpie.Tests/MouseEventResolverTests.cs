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
public class MouseEventResolverTests
{
    private MouseEventResolver _eventResolver = null!;

    [TestInitialize] public void TestInitialize() { _eventResolver = new(); }

    [TestMethod]
    public void Process1_Throws_IfEventIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _eventResolver.Process((MouseMoveEvent) null!));
    }

    [TestMethod]
    public void Process1_ReturnsSameEvent_IfDifferentPosition()
    {
        var e = new MouseMoveEvent(new(1, 1));
        _eventResolver.Process(e)
                      .Single()
                      .ShouldBe(e);
    }

    [TestMethod]
    public void Process1_EatsEvent_IfSamePosition()
    {
        var e1 = new MouseMoveEvent(new(1, 1));
        var e2 = new MouseMoveEvent(new(1, 1));

        _eventResolver.Process(e1);
        _eventResolver.Process(e2)
                      .ShouldBeEmpty();
    }

    [TestMethod]
    public void Process2_Throws_IfEventIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _eventResolver.Process((MouseActionEvent) null!));
    }

    [TestMethod]
    public void Process2_IssuesMoveFirst_IfDifferentPosition()
    {
        var e = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Released, ModifierKey.None);
        _eventResolver.Process(e)
                      .First()
                      .ShouldBe(new MouseMoveEvent(new(1, 1)));
    }

    [TestMethod]
    public void Process2_DoesNotIssueAMove_IfSamePosition()
    {
        var e = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);

        _eventResolver.Process(new MouseMoveEvent(new(1, 1)));
        _eventResolver.Process(e)
                      .First()
                      .Type.ShouldNotBe(EventType.MouseMove);
    }

    [TestMethod]
    public void Process2_IssuesActionSecond_IfDifferentPosition()
    {
        var e = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);
        _eventResolver.Process(e)
                      .Last()
                      .ShouldBe(e);
    }

    [TestMethod]
    public void Process2_IssuesOnlyAction_IfSamePosition()
    {
        var e = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);

        _eventResolver.Process(new MouseMoveEvent(new(1, 1)));
        _eventResolver.Process(e)
                      .Single()
                      .ShouldBe(e);
    }

    [TestMethod]
    public void Process2_EatsButtonPress_IfFirstOneNotReleasedYet_AndPositionChanged()
    {
        var e1 = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);
        var e2 = new MouseActionEvent(new(1, 2), MouseButton.Button2, MouseButtonState.Pressed, ModifierKey.None);

        _eventResolver.Process(e1);
        _eventResolver.Process(e2)
                      .Single()
                      .Type.ShouldBe(EventType.MouseMove);
    }

    [TestMethod]
    public void Process2_DoesNotEatButtonPress_IfFirstOneNotReleasedYet_AndPositionDidNotChange()
    {
        var e1 = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);
        var e2 = new MouseActionEvent(new(1, 1), MouseButton.Button2, MouseButtonState.Pressed, ModifierKey.None);

        _eventResolver.Process(e1);
        _eventResolver.Process(e2)
                      .Single()
                      .ShouldBe(e2);
    }

    [TestMethod]
    public void Process2_AssumesFirstButtonReleased_EvenIfSecondIsDifferent()
    {
        var e1 = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.None);
        var e2 = new MouseActionEvent(new(1, 1), MouseButton.Button2, MouseButtonState.Released, ModifierKey.None);

        _eventResolver.Process(e1);
        _eventResolver.Process(e2)
                      .Single()
                      .ShouldBe(new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Released,
                          ModifierKey.None));
    }

    [TestMethod]
    public void Process2_EatsEvent_IfItWasReleaseWithoutAnythingBeingPressedBefore()
    {
        var e1 = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Released, ModifierKey.None);

        _eventResolver.Process(e1)
                      .Single()
                      .Type.ShouldBe(EventType.MouseMove);
    }

    [TestMethod]
    public void Process2_ConvertsClicksIntoPressAndReleaseEvents()
    {
        var e1 = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Clicked, ModifierKey.Ctrl);
        _eventResolver.Process(new MouseMoveEvent(new(1, 1)));
        var two = _eventResolver.Process(e1)
                                .ToArray();

        two.Length.ShouldBe(2);
        two[0]
            .ShouldBe(new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.Ctrl));

        two[1]
            .ShouldBe(new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Released,
                ModifierKey.Ctrl));
    }
}

