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

    [TestMethod, DataRow(0x20, "\x0020"), DataRow(0x7E, "\x007E"), DataRow(0xA0, "\x00A0"), DataRow((int) '\n', "\n"),
     DataRow((int) '\b', "\b"), DataRow((int) '\t', "\t")]
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

    [TestMethod, DataRow(RawKey.F1, Key.F1, ModifierKey.None), DataRow(RawKey.F2, Key.F2, ModifierKey.None),
     DataRow(RawKey.F3, Key.F3, ModifierKey.None), DataRow(RawKey.F4, Key.F4, ModifierKey.None),
     DataRow(RawKey.F5, Key.F5, ModifierKey.None), DataRow(RawKey.F6, Key.F6, ModifierKey.None),
     DataRow(RawKey.F7, Key.F7, ModifierKey.None), DataRow(RawKey.F8, Key.F8, ModifierKey.None),
     DataRow(RawKey.F9, Key.F9, ModifierKey.None), DataRow(RawKey.F10, Key.F10, ModifierKey.None),
     DataRow(RawKey.F11, Key.F11, ModifierKey.None), DataRow(RawKey.F12, Key.F12, ModifierKey.None),
     DataRow(RawKey.ShiftF1, Key.F1, ModifierKey.Shift), DataRow(RawKey.ShiftF2, Key.F2, ModifierKey.Shift),
     DataRow(RawKey.ShiftF3, Key.F3, ModifierKey.Shift), DataRow(RawKey.ShiftF4, Key.F4, ModifierKey.Shift),
     DataRow(RawKey.ShiftF5, Key.F5, ModifierKey.Shift), DataRow(RawKey.ShiftF6, Key.F6, ModifierKey.Shift),
     DataRow(RawKey.ShiftF7, Key.F7, ModifierKey.Shift), DataRow(RawKey.ShiftF8, Key.F8, ModifierKey.Shift),
     DataRow(RawKey.ShiftF9, Key.F9, ModifierKey.Shift), DataRow(RawKey.ShiftF10, Key.F10, ModifierKey.Shift),
     DataRow(RawKey.ShiftF11, Key.F11, ModifierKey.Shift), DataRow(RawKey.ShiftF12, Key.F12, ModifierKey.Shift),
     DataRow(RawKey.CtrlF1, Key.F1, ModifierKey.Ctrl), DataRow(RawKey.CtrlF2, Key.F2, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlF3, Key.F3, ModifierKey.Ctrl), DataRow(RawKey.CtrlF4, Key.F4, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlF5, Key.F5, ModifierKey.Ctrl), DataRow(RawKey.CtrlF6, Key.F6, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlF7, Key.F7, ModifierKey.Ctrl), DataRow(RawKey.CtrlF8, Key.F8, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlF9, Key.F9, ModifierKey.Ctrl), DataRow(RawKey.CtrlF10, Key.F10, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlF11, Key.F11, ModifierKey.Ctrl), DataRow(RawKey.CtrlF12, Key.F12, ModifierKey.Ctrl),
     DataRow(RawKey.AltF1, Key.F1, ModifierKey.Alt), DataRow(RawKey.AltF2, Key.F2, ModifierKey.Alt),
     DataRow(RawKey.AltF3, Key.F3, ModifierKey.Alt), DataRow(RawKey.AltF4, Key.F4, ModifierKey.Alt),
     DataRow(RawKey.AltF5, Key.F5, ModifierKey.Alt), DataRow(RawKey.AltF6, Key.F6, ModifierKey.Alt),
     DataRow(RawKey.AltF7, Key.F7, ModifierKey.Alt), DataRow(RawKey.AltF8, Key.F8, ModifierKey.Alt),
     DataRow(RawKey.AltF9, Key.F9, ModifierKey.Alt), DataRow(RawKey.AltF10, Key.F10, ModifierKey.Alt),
     DataRow(RawKey.AltF11, Key.F11, ModifierKey.Alt), DataRow(RawKey.AltF12, Key.F12, ModifierKey.Alt),
     DataRow(RawKey.ShiftAltF1, Key.F1, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF2, Key.F2, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF3, Key.F3, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF4, Key.F4, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF5, Key.F5, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF6, Key.F6, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF7, Key.F7, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF8, Key.F8, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF9, Key.F9, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF10, Key.F10, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF11, Key.F11, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.ShiftAltF12, Key.F12, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawKey.Up, Key.KeypadUp, ModifierKey.None), DataRow(RawKey.Down, Key.KeypadDown, ModifierKey.None),
     DataRow(RawKey.Left, Key.KeypadLeft, ModifierKey.None), DataRow(RawKey.Right, Key.KeypadRight, ModifierKey.None),
     DataRow(RawKey.Home, Key.KeypadHome, ModifierKey.None), DataRow(RawKey.End, Key.KeypadEnd, ModifierKey.None),
     DataRow(RawKey.PageDown, Key.KeypadPageDown, ModifierKey.None),
     DataRow(RawKey.PageUp, Key.KeypadPageUp, ModifierKey.None),
     DataRow(RawKey.DeleteChar, Key.DeleteChar, ModifierKey.None),
     DataRow(RawKey.InsertChar, Key.InsertChar, ModifierKey.None), DataRow(RawKey.Tab, Key.Tab, ModifierKey.None),
     DataRow(RawKey.BackTab, Key.Tab, ModifierKey.Shift), DataRow(RawKey.Backspace, Key.Backspace, ModifierKey.None),
     DataRow(RawKey.ShiftUp, Key.KeypadUp, ModifierKey.Shift),
     DataRow(RawKey.ShiftDown, Key.KeypadDown, ModifierKey.Shift),
     DataRow(RawKey.ShiftLeft, Key.KeypadLeft, ModifierKey.Shift),
     DataRow(RawKey.ShiftRight, Key.KeypadRight, ModifierKey.Shift),
     DataRow(RawKey.ShiftHome, Key.KeypadHome, ModifierKey.Shift),
     DataRow(RawKey.ShiftEnd, Key.KeypadEnd, ModifierKey.Shift),
     DataRow(RawKey.ShiftPageDown, Key.KeypadPageDown, ModifierKey.Shift),
     DataRow(RawKey.ShiftPageUp, Key.KeypadPageUp, ModifierKey.Shift),
     DataRow(RawKey.AltUp, Key.KeypadUp, ModifierKey.Alt), DataRow(RawKey.AltDown, Key.KeypadDown, ModifierKey.Alt),
     DataRow(RawKey.AltLeft, Key.KeypadLeft, ModifierKey.Alt),
     DataRow(RawKey.AltRight, Key.KeypadRight, ModifierKey.Alt),
     DataRow(RawKey.AltHome, Key.KeypadHome, ModifierKey.Alt), DataRow(RawKey.AltEnd, Key.KeypadEnd, ModifierKey.Alt),
     DataRow(RawKey.AltPageDown, Key.KeypadPageDown, ModifierKey.Alt),
     DataRow(RawKey.AltPageUp, Key.KeypadPageUp, ModifierKey.Alt),
     DataRow(RawKey.CtrlUp, Key.KeypadUp, ModifierKey.Ctrl), DataRow(RawKey.CtrlDown, Key.KeypadDown, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlLeft, Key.KeypadLeft, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlRight, Key.KeypadRight, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlHome, Key.KeypadHome, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlEnd, Key.KeypadEnd, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlPageDown, Key.KeypadPageDown, ModifierKey.Ctrl),
     DataRow(RawKey.CtrlPageUp, Key.KeypadPageUp, ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlUp, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlDown, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlLeft, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlRight, Key.KeypadRight, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlHome, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlEnd, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlPageDown, Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftCtrlPageUp, Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawKey.ShiftAltUp, Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltDown, Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltLeft, Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltRight, Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltPageDown, Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltPageUp, Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltHome, Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.ShiftAltEnd, Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
     DataRow(RawKey.AltCtrlPageDown, Key.KeypadPageDown, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(RawKey.AltCtrlPageUp, Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(RawKey.AltCtrlHome, Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(RawKey.AltCtrlEnd, Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((uint) 9999, Key.Unknown, ModifierKey.None)]
    public void ConvertKeyPressEvent_ConvertsKnownMappings(uint rawKey, Key expKey, ModifierKey expMod)
    {
        var result = Helpers.ConvertKeyPressEvent(rawKey);
        result.key.ShouldBe(expKey);
        result.modifierKey.ShouldBe(expMod);
    }

    [TestMethod, DataRow(RawMouseEvent.EventType.Button1Released, MouseButton.Button1, MouseButtonState.Released),
     DataRow(RawMouseEvent.EventType.Button1Pressed, MouseButton.Button1, MouseButtonState.Pressed),
     DataRow(RawMouseEvent.EventType.Button1Clicked, MouseButton.Button1, MouseButtonState.Clicked),
     DataRow(RawMouseEvent.EventType.Button1DoubleClicked, MouseButton.Button1, MouseButtonState.DoubleClicked),
     DataRow(RawMouseEvent.EventType.Button1TripleClicked, MouseButton.Button1, MouseButtonState.TripleClicked),
     DataRow(RawMouseEvent.EventType.Button2Released, MouseButton.Button2, MouseButtonState.Released),
     DataRow(RawMouseEvent.EventType.Button2Pressed, MouseButton.Button2, MouseButtonState.Pressed),
     DataRow(RawMouseEvent.EventType.Button2Clicked, MouseButton.Button2, MouseButtonState.Clicked),
     DataRow(RawMouseEvent.EventType.Button2DoubleClicked, MouseButton.Button2, MouseButtonState.DoubleClicked),
     DataRow(RawMouseEvent.EventType.Button2TripleClicked, MouseButton.Button2, MouseButtonState.TripleClicked),
     DataRow(RawMouseEvent.EventType.Button3Released, MouseButton.Button3, MouseButtonState.Released),
     DataRow(RawMouseEvent.EventType.Button3Pressed, MouseButton.Button3, MouseButtonState.Pressed),
     DataRow(RawMouseEvent.EventType.Button3Clicked, MouseButton.Button3, MouseButtonState.Clicked),
     DataRow(RawMouseEvent.EventType.Button3DoubleClicked, MouseButton.Button3, MouseButtonState.DoubleClicked),
     DataRow(RawMouseEvent.EventType.Button3TripleClicked, MouseButton.Button3, MouseButtonState.TripleClicked),
     DataRow(RawMouseEvent.EventType.Button4Released, MouseButton.Button4, MouseButtonState.Released),
     DataRow(RawMouseEvent.EventType.Button4Pressed, MouseButton.Button4, MouseButtonState.Pressed),
     DataRow(RawMouseEvent.EventType.Button4Clicked, MouseButton.Button4, MouseButtonState.Clicked),
     DataRow(RawMouseEvent.EventType.Button4DoubleClicked, MouseButton.Button4, MouseButtonState.DoubleClicked),
     DataRow(RawMouseEvent.EventType.Button4TripleClicked, MouseButton.Button4, MouseButtonState.TripleClicked)]
    public void ConvertMouseActionEvent_ConvertsKnownMappings(ulong evt, MouseButton expButton,
        MouseButtonState expState)
    {
        var result = Helpers.ConvertMouseActionEvent((RawMouseEvent.EventType) evt);
        result.button.ShouldBe(expButton);
        result.state.ShouldBe(expState);
    }

    [TestMethod, DataRow((ulong) 0, ModifierKey.None), DataRow(RawMouseEvent.EventType.Alt, ModifierKey.Alt),
     DataRow(RawMouseEvent.EventType.Ctrl, ModifierKey.Ctrl), DataRow(RawMouseEvent.EventType.Shift, ModifierKey.Shift),
     DataRow(RawMouseEvent.EventType.Alt | RawMouseEvent.EventType.Ctrl, ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow(RawMouseEvent.EventType.Shift | RawMouseEvent.EventType.Ctrl, ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(RawMouseEvent.EventType.Alt | RawMouseEvent.EventType.Shift, ModifierKey.Alt | ModifierKey.Shift),
     DataRow(RawMouseEvent.EventType.Ctrl | RawMouseEvent.EventType.Shift, ModifierKey.Ctrl | ModifierKey.Shift),
     DataRow(RawMouseEvent.EventType.Alt | RawMouseEvent.EventType.Ctrl | RawMouseEvent.EventType.Shift,
         ModifierKey.Alt | ModifierKey.Shift | ModifierKey.Ctrl)]
    public void ConvertMouseActionEvent_MapsModifiers(ulong evt, ModifierKey expMod)
    {
        var result = Helpers.ConvertMouseActionEvent((RawMouseEvent.EventType) evt);
        result.modifierKey.ShouldBe(expMod);
    }
   
    [TestMethod, 
     DataRow(Key.Character, 0x01b, ModifierKey.Shift, Key.Escape, '\0', ModifierKey.Shift),
     DataRow(Key.Character, '\t', ModifierKey.Shift, Key.Tab, '\0', ModifierKey.Shift),
     DataRow(Key.Character, 0x7f, ModifierKey.Shift, Key.Backspace, '\0', ModifierKey.Shift),
     DataRow(Key.Character, 0, ModifierKey.Shift, Key.Character, ' ', ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(Key.Character, 1, ModifierKey.Shift, Key.Character, 'A', ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(Key.Character, 26, ModifierKey.Shift, Key.Character, 'Z', ModifierKey.Shift | ModifierKey.Ctrl)
    ]
    public void TryConvertKeyEventSequence_ResolvesSequenceOf1(Key inKey, int inCode, ModifierKey inMod, 
        Key expKey, int expCode, ModifierKey expMod)
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("key_name");

        var result = Helpers.TryConvertKeyEventSequence(_cursesMock.Object,
            new[] { new KeyEvent(inKey, new(inCode), "dummy", inMod) });
        
        result.ShouldNotBeNull();
        result.Key.ShouldBe(expKey);
        result.Char.ShouldBe(new (expCode));
        result.Modifiers.ShouldBe(expMod);
        result.Name.ShouldBe("key_name");
    }
    
    [TestMethod, 
     DataRow(Key.Character, 'f', ModifierKey.Shift, Key.KeypadRight, '\0', ModifierKey.Shift | ModifierKey.Alt, true),
     DataRow(Key.Character, 'b', ModifierKey.Shift, Key.KeypadLeft, '\0', ModifierKey.Shift | ModifierKey.Alt, true),
     DataRow(Key.F1, '\0', ModifierKey.Shift, Key.F1, '\0', ModifierKey.Shift | ModifierKey.Alt, false),
     DataRow(Key.Character, 'A', ModifierKey.Shift, Key.Character, 'A', ModifierKey.Shift | ModifierKey.Alt, true),
     DataRow(Key.Character, '.', ModifierKey.None, Key.Character, '.', ModifierKey.Alt, true),
    ]
    public void TryConvertKeyEventSequence_ResolvesSequenceOf2_UsingEscape(Key inKey, int inCode, ModifierKey inMod, 
        Key expKey, int expCode, ModifierKey expMod, bool chName)
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("new_name");

        var result = Helpers.TryConvertKeyEventSequence(_cursesMock.Object,
            new[] { new KeyEvent(Key.Escape, new('\0'), "none", ModifierKey.None),
                new KeyEvent(inKey, new(inCode), "orig_name", inMod) });
        
        result.ShouldNotBeNull();
        result.Key.ShouldBe(expKey);
        result.Char.ShouldBe(new (expCode));
        result.Modifiers.ShouldBe(expMod);
        result.Name.ShouldBe(chName ? "new_name" : "orig_name");
    }
    
    [TestMethod, 
     DataRow('A', Key.KeypadUp),
     DataRow('B', Key.KeypadDown),
     DataRow('C', Key.KeypadRight),
     DataRow('D', Key.KeypadLeft),
     DataRow('E', Key.KeypadPageUp),
     DataRow('F', Key.KeypadEnd),
     DataRow('G', Key.KeypadPageDown),
     DataRow('H', Key.KeypadHome),
    ]
    public void TryConvertKeyEventSequence_ResolvesSequenceOf3_AltKeyPad(int ch, Key key)
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("new_name");

        var result = Helpers.TryConvertKeyEventSequence(_cursesMock.Object,
            new[] { 
                new KeyEvent(Key.Character, new('O'), null, ModifierKey.Alt),
                new KeyEvent(Key.Character, new('8'), null, ModifierKey.None),
                new KeyEvent(Key.Character, new(ch), "orig_name", ModifierKey.None) });
        
        result.ShouldNotBeNull();
        result.Key.ShouldBe(key);
        result.Modifiers.ShouldBe(ModifierKey.Shift | ModifierKey.Ctrl | ModifierKey.Alt);
        result.Name.ShouldBe("new_name");
    }
    
    [TestMethod]
    public void TryConvertKeyEventSequence_ThrowsIfCursesIsNull()
    {
        Should.Throw<ArgumentException>(() =>
            Helpers.TryConvertKeyEventSequence(null!, Array.Empty<KeyEvent>()));
    }
    
    [TestMethod]
    public void TryConvertKeyEventSequence_ThrowsIfEventsIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
            Helpers.TryConvertKeyEventSequence(_cursesMock.Object, null!));
    }
    
    [TestMethod]
    public void TryConvertKeyEventSequence_ThrowsIfEventsIsEmpty()
    {
        Should.Throw<ArgumentException>(() =>
            Helpers.TryConvertKeyEventSequence(_cursesMock.Object, Array.Empty<KeyEvent>()));
    }
}
