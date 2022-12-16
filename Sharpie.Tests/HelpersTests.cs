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
        var exception = Should.Throw<CursesOperationException>(() => { (-1).Check("operation", "message"); });
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
        var exception = Should.Throw<CursesOperationException>(() => { IntPtr.Zero.Check("operation", "message"); });
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
        _cursesMock.Setup(s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<CursesComplexChar>.IsAny, expected, It.IsAny<uint>(), It.IsAny<short>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod, DataRow(0x20, "\x0020"), DataRow(0x7E, "\x007E"), DataRow(0xA0, "\x00A0"),
     DataRow((int) ControlCharacter.NewLine, "\n"), DataRow((int) '\b', "\b"),
     DataRow((int) ControlCharacter.Tab, "\t")]
    public void ToComplexChar_DoesNotConvertOtherAsciiToUnicode(int ch, string expected)
    {
        _cursesMock.Setup(s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Object.ToComplexChar(new(ch), Style.Default);
        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<CursesComplexChar>.IsAny, expected, It.IsAny<uint>(), It.IsAny<short>(),
                It.IsAny<IntPtr>()), Times.Once);
    }

    [TestMethod]
    public void ToComplexChar_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _cursesMock.Object.ToComplexChar(new('a'), Style.Default));
    }

    [TestMethod]
    public void FromComplexChar_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, It.IsAny<IntPtr>()))
                   .Returns(-1);

        var c = new CursesComplexChar();
        Should.Throw<CursesOperationException>(() => _cursesMock.Object.FromComplexChar(c));
    }

    [TestMethod]
    public void FromComplexChar_ReturnsCursesChar()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<short>.IsAny, It.IsAny<IntPtr>()))
                   .Returns((CursesComplexChar _, StringBuilder sb, out uint attrs, out short colorPair,
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

    [TestMethod, DataRow(CursesKey.F1, Key.F1, ModifierKey.None), DataRow(CursesKey.F2, Key.F2, ModifierKey.None),
     DataRow(CursesKey.F3, Key.F3, ModifierKey.None), DataRow(CursesKey.F4, Key.F4, ModifierKey.None),
     DataRow(CursesKey.F5, Key.F5, ModifierKey.None), DataRow(CursesKey.F6, Key.F6, ModifierKey.None),
     DataRow(CursesKey.F7, Key.F7, ModifierKey.None), DataRow(CursesKey.F8, Key.F8, ModifierKey.None),
     DataRow(CursesKey.F9, Key.F9, ModifierKey.None), DataRow(CursesKey.F10, Key.F10, ModifierKey.None),
     DataRow(CursesKey.F11, Key.F11, ModifierKey.None), DataRow(CursesKey.F12, Key.F12, ModifierKey.None),
     DataRow(CursesKey.ShiftF1, Key.F1, ModifierKey.Shift), DataRow(CursesKey.ShiftF2, Key.F2, ModifierKey.Shift),
     DataRow(CursesKey.ShiftF3, Key.F3, ModifierKey.Shift), DataRow(CursesKey.ShiftF4, Key.F4, ModifierKey.Shift),
     DataRow(CursesKey.ShiftF5, Key.F5, ModifierKey.Shift), DataRow(CursesKey.ShiftF6, Key.F6, ModifierKey.Shift),
     DataRow(CursesKey.ShiftF7, Key.F7, ModifierKey.Shift), DataRow(CursesKey.ShiftF8, Key.F8, ModifierKey.Shift),
     DataRow(CursesKey.ShiftF9, Key.F9, ModifierKey.Shift), DataRow(CursesKey.ShiftF10, Key.F10, ModifierKey.Shift),
     DataRow(CursesKey.ShiftF11, Key.F11, ModifierKey.Shift), DataRow(CursesKey.ShiftF12, Key.F12, ModifierKey.Shift),
     DataRow(CursesKey.CtrlF1, Key.F1, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF2, Key.F2, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlF3, Key.F3, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF4, Key.F4, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlF5, Key.F5, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF6, Key.F6, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlF7, Key.F7, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF8, Key.F8, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlF9, Key.F9, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF10, Key.F10, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlF11, Key.F11, ModifierKey.Ctrl), DataRow(CursesKey.CtrlF12, Key.F12, ModifierKey.Ctrl),
     DataRow(CursesKey.AltF1, Key.F1, ModifierKey.Alt), DataRow(CursesKey.AltF2, Key.F2, ModifierKey.Alt),
     DataRow(CursesKey.AltF3, Key.F3, ModifierKey.Alt), DataRow(CursesKey.AltF4, Key.F4, ModifierKey.Alt),
     DataRow(CursesKey.AltF5, Key.F5, ModifierKey.Alt), DataRow(CursesKey.AltF6, Key.F6, ModifierKey.Alt),
     DataRow(CursesKey.AltF7, Key.F7, ModifierKey.Alt), DataRow(CursesKey.AltF8, Key.F8, ModifierKey.Alt),
     DataRow(CursesKey.AltF9, Key.F9, ModifierKey.Alt), DataRow(CursesKey.AltF10, Key.F10, ModifierKey.Alt),
     DataRow(CursesKey.AltF11, Key.F11, ModifierKey.Alt), DataRow(CursesKey.AltF12, Key.F12, ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltF1, Key.F1, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF2, Key.F2, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF3, Key.F3, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF4, Key.F4, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF5, Key.F5, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF6, Key.F6, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF7, Key.F7, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF8, Key.F8, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF9, Key.F9, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF10, Key.F10, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF11, Key.F11, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.ShiftAltF12, Key.F12, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesKey.Up, Key.KeypadUp, ModifierKey.None), DataRow(CursesKey.Down, Key.KeypadDown, ModifierKey.None),
     DataRow(CursesKey.Left, Key.KeypadLeft, ModifierKey.None),
     DataRow(CursesKey.Right, Key.KeypadRight, ModifierKey.None),
     DataRow(CursesKey.Home, Key.KeypadHome, ModifierKey.None), DataRow(CursesKey.End, Key.KeypadEnd, ModifierKey.None),
     DataRow(CursesKey.PageDown, Key.KeypadPageDown, ModifierKey.None),
     DataRow(CursesKey.PageUp, Key.KeypadPageUp, ModifierKey.None),
     DataRow(CursesKey.DeleteChar, Key.DeleteChar, ModifierKey.None),
     DataRow(CursesKey.InsertChar, Key.InsertChar, ModifierKey.None), DataRow(CursesKey.Tab, Key.Tab, ModifierKey.None),
     DataRow(CursesKey.BackTab, Key.Tab, ModifierKey.Shift),
     DataRow(CursesKey.Backspace, Key.Backspace, ModifierKey.None),
     DataRow(CursesKey.ShiftUp, Key.KeypadUp, ModifierKey.Shift),
     DataRow(CursesKey.ShiftDown, Key.KeypadDown, ModifierKey.Shift),
     DataRow(CursesKey.ShiftLeft, Key.KeypadLeft, ModifierKey.Shift),
     DataRow(CursesKey.ShiftRight, Key.KeypadRight, ModifierKey.Shift),
     DataRow(CursesKey.ShiftHome, Key.KeypadHome, ModifierKey.Shift),
     DataRow(CursesKey.ShiftEnd, Key.KeypadEnd, ModifierKey.Shift),
     DataRow(CursesKey.ShiftPageDown, Key.KeypadPageDown, ModifierKey.Shift),
     DataRow(CursesKey.ShiftPageUp, Key.KeypadPageUp, ModifierKey.Shift),
     DataRow(CursesKey.AltUp, Key.KeypadUp, ModifierKey.Alt),
     DataRow(CursesKey.AltDown, Key.KeypadDown, ModifierKey.Alt),
     DataRow(CursesKey.AltLeft, Key.KeypadLeft, ModifierKey.Alt),
     DataRow(CursesKey.AltRight, Key.KeypadRight, ModifierKey.Alt),
     DataRow(CursesKey.AltHome, Key.KeypadHome, ModifierKey.Alt),
     DataRow(CursesKey.AltEnd, Key.KeypadEnd, ModifierKey.Alt),
     DataRow(CursesKey.AltPageDown, Key.KeypadPageDown, ModifierKey.Alt),
     DataRow(CursesKey.AltPageUp, Key.KeypadPageUp, ModifierKey.Alt),
     DataRow(CursesKey.CtrlUp, Key.KeypadUp, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlDown, Key.KeypadDown, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlLeft, Key.KeypadLeft, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlRight, Key.KeypadRight, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlHome, Key.KeypadHome, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlEnd, Key.KeypadEnd, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlPageDown, Key.KeypadPageDown, ModifierKey.Ctrl),
     DataRow(CursesKey.CtrlPageUp, Key.KeypadPageUp, ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlUp, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlDown, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlLeft, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlRight, Key.KeypadRight, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlHome, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlEnd, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlPageDown, Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftCtrlPageUp, Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesKey.ShiftAltUp, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltDown, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltLeft, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltRight, Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltPageDown, Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltPageUp, Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltHome, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.ShiftAltEnd, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(CursesKey.AltCtrlPageDown, Key.KeypadPageDown, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(CursesKey.AltCtrlPageUp, Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(CursesKey.AltCtrlHome, Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(CursesKey.AltCtrlEnd, Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((uint) 9999, Key.Unknown, ModifierKey.None)]
    public void ConvertKeyPressEvent_ConvertsKnownMappings(uint rawKey, Key expKey, ModifierKey expMod)
    {
        var result = Helpers.ConvertKeyPressEvent(rawKey);
        result.key.ShouldBe(expKey);
        result.modifierKey.ShouldBe(expMod);
    }

    [TestMethod, DataRow(CursesMouseEvent.EventType.Button1Released, MouseButton.Button1, MouseButtonState.Released),
     DataRow(CursesMouseEvent.EventType.Button1Pressed, MouseButton.Button1, MouseButtonState.Pressed),
     DataRow(CursesMouseEvent.EventType.Button1Clicked, MouseButton.Button1, MouseButtonState.Clicked),
     DataRow(CursesMouseEvent.EventType.Button1DoubleClicked, MouseButton.Button1, MouseButtonState.DoubleClicked),
     DataRow(CursesMouseEvent.EventType.Button1TripleClicked, MouseButton.Button1, MouseButtonState.TripleClicked),
     DataRow(CursesMouseEvent.EventType.Button2Released, MouseButton.Button2, MouseButtonState.Released),
     DataRow(CursesMouseEvent.EventType.Button2Pressed, MouseButton.Button2, MouseButtonState.Pressed),
     DataRow(CursesMouseEvent.EventType.Button2Clicked, MouseButton.Button2, MouseButtonState.Clicked),
     DataRow(CursesMouseEvent.EventType.Button2DoubleClicked, MouseButton.Button2, MouseButtonState.DoubleClicked),
     DataRow(CursesMouseEvent.EventType.Button2TripleClicked, MouseButton.Button2, MouseButtonState.TripleClicked),
     DataRow(CursesMouseEvent.EventType.Button3Released, MouseButton.Button3, MouseButtonState.Released),
     DataRow(CursesMouseEvent.EventType.Button3Pressed, MouseButton.Button3, MouseButtonState.Pressed),
     DataRow(CursesMouseEvent.EventType.Button3Clicked, MouseButton.Button3, MouseButtonState.Clicked),
     DataRow(CursesMouseEvent.EventType.Button3DoubleClicked, MouseButton.Button3, MouseButtonState.DoubleClicked),
     DataRow(CursesMouseEvent.EventType.Button3TripleClicked, MouseButton.Button3, MouseButtonState.TripleClicked),
     DataRow(CursesMouseEvent.EventType.Button4Released, MouseButton.Button4, MouseButtonState.Released),
     DataRow(CursesMouseEvent.EventType.Button4Pressed, MouseButton.Button4, MouseButtonState.Pressed),
     DataRow(CursesMouseEvent.EventType.Button4Clicked, MouseButton.Button4, MouseButtonState.Clicked),
     DataRow(CursesMouseEvent.EventType.Button4DoubleClicked, MouseButton.Button4, MouseButtonState.DoubleClicked),
     DataRow(CursesMouseEvent.EventType.Button4TripleClicked, MouseButton.Button4, MouseButtonState.TripleClicked)]
    public void ConvertMouseActionEvent_ConvertsKnownMappings(int evt, MouseButton expButton, MouseButtonState expState)
    {
        var result = Helpers.ConvertMouseActionEvent((CursesMouseEvent.EventType) evt);
        result.button.ShouldBe(expButton);
        result.state.ShouldBe(expState);
    }

    [TestMethod, DataRow(0, ModifierKey.None), DataRow(CursesMouseEvent.EventType.Alt, ModifierKey.Alt),
     DataRow(CursesMouseEvent.EventType.Ctrl, ModifierKey.Ctrl),
     DataRow(CursesMouseEvent.EventType.Shift, ModifierKey.Shift),
     DataRow(CursesMouseEvent.EventType.Alt | CursesMouseEvent.EventType.Ctrl, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(CursesMouseEvent.EventType.Shift | CursesMouseEvent.EventType.Ctrl, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(CursesMouseEvent.EventType.Alt | CursesMouseEvent.EventType.Shift, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(CursesMouseEvent.EventType.Ctrl | CursesMouseEvent.EventType.Shift, ModifierKey.Ctrl | ModifierKey.Shift),
     DataRow(CursesMouseEvent.EventType.Alt | CursesMouseEvent.EventType.Ctrl | CursesMouseEvent.EventType.Shift,
         ModifierKey.Alt | ModifierKey.Shift | ModifierKey.Ctrl)]
    public void ConvertMouseActionEvent_MapsModifiers(int evt, ModifierKey expMod)
    {
        var result = Helpers.ConvertMouseActionEvent((CursesMouseEvent.EventType) evt);
        result.modifierKey.ShouldBe(expMod);
    }

    [TestMethod]
    public void ValidOrNull_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => Helpers.ValidOrNull(null!));
    }

    [TestMethod]
    public void ValidOrNull_ReturnsNull_IfTermNameFailsWithDllNotFoundException()
    {
        _cursesMock.Setup(s => s.termname())
                   .Throws<DllNotFoundException>();

        _cursesMock.Object.ValidOrNull()
                   .ShouldBeNull();
    }

    [TestMethod]
    public void ValidOrNull_ReturnsNull_IfTermNameFailsWithEntryPointNotFoundException()
    {
        _cursesMock.Setup(s => s.termname())
                   .Throws<EntryPointNotFoundException>();

        _cursesMock.Object.ValidOrNull()
                   .ShouldBeNull();
    }

    [TestMethod]
    public void ValidOrNull_Throws_IfUnexpectedErrorOccurs()
    {
        _cursesMock.Setup(s => s.termname())
                   .Throws<ArgumentOutOfRangeException>();

        Should.Throw<ArgumentOutOfRangeException>(() => _cursesMock.Object.ValidOrNull());
    }

    [TestMethod]
    public void ValidOrNull_ReturnsCurses_IfTermNameDoesNotFail()
    {
        _cursesMock.Object.ValidOrNull()
                   .ShouldBe(_cursesMock.Object);
    }

    [TestMethod]
    public void EnumerateInHalves_Throws_IfStartIsNegative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.EnumerateInHalves(-1, 1)
                                                               .ToArray());
    }

    [TestMethod]
    public void EnumerateInHalves_Throws_IfCountIsNegative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.EnumerateInHalves(0, -1)
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
    public void IntersectSegments_Throws_IfLength1IsLessThanZero()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.IntersectSegments(0, -1, 0, 1));
    }

    [TestMethod]
    public void IntersectSegments_Throws_IfLength2IsLessThanZero()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Helpers.IntersectSegments(0, 1, 0, -1));
    }

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
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        using var sa = new TerminalSurface(terminal, new(1));

        _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => terminal.Refresh(sa))
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Fails_IfCursesFails_InBatch()
    {
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        using var sa = new TerminalSurface(terminal, new(1));

        _cursesMock.Setup(s => s.wnoutrefresh(It.IsAny<IntPtr>()))
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
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        var sa = new TerminalSurface(terminal, new(1));

        terminal.Refresh(sa);
        _cursesMock.Verify(v => v.wrefresh(sa.Handle), Times.Once);
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_InBatch()
    {
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        using var terminal = new Terminal(_cursesMock.Object, new());
        var sa = new TerminalSurface(terminal, new(1));

        using (terminal.AtomicRefresh())
        {
            terminal.Refresh(sa);
        }

        _cursesMock.Verify(v => v.wnoutrefresh(sa.Handle), Times.Once);
    }
}

