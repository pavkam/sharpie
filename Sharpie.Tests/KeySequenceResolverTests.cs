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
public class KeySequenceResolverTests
{
    [TestMethod]
    public void SpecialCharacterResolver_ThrowsIfSequenceIsNull() =>
        Should.Throw<ArgumentNullException>(() => KeySequenceResolver.SpecialCharacterResolver(null!));

    [TestMethod]
    public void ControlKeyResolver_ThrowsIfSequenceIsNull() =>
        Should.Throw<ArgumentNullException>(() => KeySequenceResolver.ControlKeyResolver(null!));

    [TestMethod]
    public void AltKeyResolver_ThrowsIfSequenceIsNull() =>
        Should.Throw<ArgumentNullException>(() => KeySequenceResolver.AltKeyResolver(null!));

    [TestMethod]
    public void KeyPadModifiersResolver_ThrowsIfSequenceIsNull() =>
        Should.Throw<ArgumentNullException>(() => KeySequenceResolver.KeyPadModifiersResolver(null!));

    [TestMethod, DataRow(Key.Character, 0x01b, ModifierKey.Shift, Key.Escape, ControlCharacter.Null,
         ModifierKey.Shift), DataRow(Key.Character, ControlCharacter.Tab, ModifierKey.Shift, Key.Tab,
         ControlCharacter.Null,
         ModifierKey.Shift),
     DataRow(Key.Character, ControlCharacter.NewLine, ModifierKey.Shift, Key.Return, ControlCharacter.Null,
         ModifierKey.Shift), DataRow(Key.Character, 0x7f, ModifierKey.Shift, Key.Backspace, ControlCharacter.Null,
         ModifierKey.Shift)]
    public void SpecialCharacterResolver_ReturnsTheExpectedResult_ForKnown(Key inKey, int inCode, ModifierKey inMod,
        Key expKey, int expCode, ModifierKey expMod)
    {
        var (key, count) = KeySequenceResolver.SpecialCharacterResolver(
            new[] { new KeyEvent(inKey, new(inCode), "dummy", inMod) });

        count.ShouldBe(1);
        _ = key.ShouldNotBeNull();
        key.Key.ShouldBe(expKey);
        key.Char.ShouldBe(new(expCode));
        key.Modifiers.ShouldBe(expMod);
        key.Name.ShouldBe(null);
    }

    [TestMethod,
     DataRow(Key.Character, 'a'),
     DataRow(Key.Unknown, ControlCharacter.Null),
     DataRow(Key.Backspace, ControlCharacter.Null),
     DataRow(Key.F1, ControlCharacter.Null)]
    public void SpecialCharacterResolver_ReturnsTheExpectedResult_ForUnknown(Key inKey, int inCode)
    {
        var (key, count) = KeySequenceResolver.SpecialCharacterResolver(
            new[] { new KeyEvent(inKey, new(inCode), "dummy", ModifierKey.None) });

        count.ShouldBe(0);
        key.ShouldBeNull();
    }

    [TestMethod,
     DataRow(Key.Character, 0, ModifierKey.Shift, Key.Character, ' ', ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(Key.Character, 1, ModifierKey.Shift, Key.Character, 'A', ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow(Key.Character, 26, ModifierKey.Shift, Key.Character, 'Z', ModifierKey.Shift | ModifierKey.Ctrl)]
    public void ControlKeyResolver_ReturnsTheExpectedResult_ForKnown(Key inKey, int inCode, ModifierKey inMod,
        Key expKey, int expCode, ModifierKey expMod)
    {
        var (key, count) = KeySequenceResolver.ControlKeyResolver(
            new[] { new KeyEvent(inKey, new(inCode), "dummy", inMod) });

        count.ShouldBe(1);
        _ = key.ShouldNotBeNull();
        key.Key.ShouldBe(expKey);
        key.Char.ShouldBe(new(expCode));
        key.Modifiers.ShouldBe(expMod);
        key.Name.ShouldBe(null);
    }

    [TestMethod,
     DataRow(Key.Character, 'a'),
     DataRow(Key.Unknown, ControlCharacter.Null),
     DataRow(Key.Backspace, ControlCharacter.Null),
     DataRow(Key.F1, ControlCharacter.Null)]
    public void ControlKeyResolver_ReturnsTheExpectedResult_ForUnknown(Key inKey, int inCode)
    {
        var (key, count) = KeySequenceResolver.ControlKeyResolver(
            new[] { new KeyEvent(inKey, new(inCode), "dummy", ModifierKey.None) });

        count.ShouldBe(0);
        key.ShouldBeNull();
    }

    [TestMethod,
     DataRow(Key.Character, 'f', ModifierKey.Shift, Key.KeypadRight, ControlCharacter.Null,
        ModifierKey.Shift | ModifierKey.Alt, true),
     DataRow(Key.Character, 'b', ModifierKey.Shift, Key.KeypadLeft, ControlCharacter.Null,
         ModifierKey.Shift | ModifierKey.Alt, true),
     DataRow(Key.F1, ControlCharacter.Null, ModifierKey.Shift, Key.F1, ControlCharacter.Null,
         ModifierKey.Shift | ModifierKey.Alt, false), DataRow(Key.Character, 'A', ModifierKey.Shift, Key.Character, 'A',
         ModifierKey.Shift | ModifierKey.Alt, true), DataRow(Key.Character, '.', ModifierKey.None, Key.Character, '.',
         ModifierKey.Alt, true)]
    public void AltKeyResolver_ReturnsTheExpectedResult_ForKnown(Key inKey, int inCode, ModifierKey inMod, Key expKey,
        int expCode, ModifierKey expMod, bool chName)
    {
        var (key, count) = KeySequenceResolver.AltKeyResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), "none", ModifierKey.None),
            new KeyEvent(inKey, new(inCode), "orig_name", inMod)
        });

        count.ShouldBe(2);
        _ = key.ShouldNotBeNull();
        key.Key.ShouldBe(expKey);
        key.Char.ShouldBe(new(expCode));
        key.Modifiers.ShouldBe(expMod);
        key.Name.ShouldBe(chName ? null : "orig_name");
    }

    [TestMethod]
    public void AltKeyResolver_ReturnsTheExpectedResult_ForUnknown()
    {
        var (key, count) = KeySequenceResolver.AltKeyResolver(new[]
        {
            new KeyEvent(Key.Delete, new(ControlCharacter.Null), "none", ModifierKey.None),
            new KeyEvent(Key.F1, new(ControlCharacter.Null), "orig_name", ModifierKey.None)
        });

        count.ShouldBe(0);
        key.ShouldBeNull();
    }

    [TestMethod]
    public void AltKeyResolver_ReturnsTheExpectedResult_ForPartial()
    {
        var (key, count) = KeySequenceResolver.AltKeyResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), "none", ModifierKey.None),
            new KeyEvent(Key.Escape, new(ControlCharacter.Null), "orig_name", ModifierKey.None)
        });

        count.ShouldBe(1);
        key.ShouldBeNull();
    }

    [TestMethod, DataRow('A', Key.KeypadUp), DataRow('B', Key.KeypadDown), DataRow('C', Key.KeypadRight),
     DataRow('D', Key.KeypadLeft), DataRow('E', Key.KeypadPageUp), DataRow('F', Key.KeypadEnd),
     DataRow('G', Key.KeypadPageDown), DataRow('H', Key.KeypadHome)]
    public void KeyPadModifiersResolver_ReturnsTheExpectedResult_ForKnown(int ch, Key expKey)
    {
        var (key, count) = KeySequenceResolver.KeyPadModifiersResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('O'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('8'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new(ch), "orig_name", ModifierKey.None)
        });

        count.ShouldBe(4);
        _ = key.ShouldNotBeNull();
        key.Key.ShouldBe(expKey);
        key.Modifiers.ShouldBe(ModifierKey.Shift | ModifierKey.Ctrl | ModifierKey.Alt);
        key.Name.ShouldBe(null);
    }

    [TestMethod]
    public void KeyPadModifiersResolver_ReturnsTheExpectedResult_ForPartial_1()
    {
        var (key, count) = KeySequenceResolver.KeyPadModifiersResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('a'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('b'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('c'), null, ModifierKey.None)
        });

        count.ShouldBe(1);
        key.ShouldBeNull();
    }

    [TestMethod]
    public void KeyPadModifiersResolver_ReturnsTheExpectedResult_ForPartial_2()
    {
        var (key, count) = KeySequenceResolver.KeyPadModifiersResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('O'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('b'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('c'), null, ModifierKey.None)
        });

        count.ShouldBe(2);
        key.ShouldBeNull();
    }

    [TestMethod]
    public void KeyPadModifiersResolver_ReturnsTheExpectedResult_ForPartial_3()
    {
        var (key, count) = KeySequenceResolver.KeyPadModifiersResolver(new[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('O'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('2'), null, ModifierKey.None),
            new KeyEvent(Key.Character, new('c'), null, ModifierKey.None)
        });

        count.ShouldBe(3);
        key.ShouldBeNull();
    }

    [TestMethod]
    public void KeyPadModifiersResolver_ReturnsTheExpectedResult_Unknown()
    {
        var (key, count) = KeySequenceResolver.KeyPadModifiersResolver(
            new[] { new KeyEvent(Key.Character, new('j'), null, ModifierKey.None) });

        count.ShouldBe(0);
        key.ShouldBeNull();
    }
}
