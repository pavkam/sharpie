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
public class ScreenTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private Screen _screen1 = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _cursesMock.Setup(s => s.getmaxx(new(1)))
                   .Returns(5);

        _cursesMock.Setup(s => s.getmaxy(new(1)))
                   .Returns(6);

        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _screen1 = new(_cursesMock.Object, new(1));
    }

    [TestMethod]
    public void Ctor_Throws_IfCursesIsNull() { Should.Throw<ArgumentNullException>(() => new Screen(null!, new(1))); }

    [TestMethod]
    public void Ctor_Throws_IfHandleIsZero()
    {
        Should.Throw<ArgumentException>(() => new Screen(_cursesMock.Object, IntPtr.Zero));
    }

    [TestMethod] public void Parent_IsNull() { _screen1.Parent.ShouldBeNull(); }

    [TestMethod] public void Location_Get_ReturnsZero_Always() { _screen1.Location.ShouldBe(Point.Empty); }

    [TestMethod]
    public void Location_Set_Throws_Always()
    {
        Should.Throw<NotSupportedException>(() => _screen1.Location = Point.Empty);
    }

    [TestMethod] public void Size_Get_ReturnsTheSize() { _screen1.Size.ShouldBe(new(5, 6)); }

    [TestMethod]
    public void Size_Set_Throws_Always() { Should.Throw<NotSupportedException>(() => _screen1.Size = new(1, 1)); }

    [TestMethod]
    public void CreateWindow_Throws_IfAreaOutsideBoundaries()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.CreateWindow(new(0, 0, 1, 1)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateWindow_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.CreateWindow(new(0, 0, 1, 1)))
              .Operation.ShouldBe("newwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateWindow_ReturnsNewWindow_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        var w = _screen1.CreateWindow(new(0, 0, 1, 1));
        w.Handle.ShouldBe(new(2));
        w.Parent.ShouldBe(_screen1);
    }

    [TestMethod]
    public void CreateSubWindow_Throws_IfWindowIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _screen1.CreateSubWindow(null!, new(0, 0, 1, 1)));
    }

    [TestMethod]
    public void CreateSubWindow_Throws_IfAreaOutsideBoundaries()
    {
        var p = new Window(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.CreateSubWindow(p, new(0, 0, 1, 1)));
    }

    [TestMethod]
    public void CreateSubWindow_Throws_IfWindowIsPad()
    {
        var p = new Pad(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        Should.Throw<InvalidOperationException>(() => _screen1.CreateSubWindow(p, new(0, 0, 1, 1)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateSubWindow_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.CreateSubWindow(w, new(0, 0, 1, 1)))
              .Operation.ShouldBe("derwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateSubWindow_ReturnsNewWindow_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, _screen1, new(2));

        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sw = _screen1.CreateSubWindow(w, new(0, 0, 1, 1));
        sw.Handle.ShouldBe(new(3));
        sw.Parent.ShouldBe(w);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateSubWindow_ReturnsNewWindowInScreen_IfParentIsScreen()
    {
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sw = _screen1.CreateSubWindow(_screen1, new(0, 0, 1, 1));
        sw.Handle.ShouldBe(new(3));
        sw.Parent.ShouldBe(_screen1);
    }

    [TestMethod]
    public void DuplicateWindow_Throws_IfWindowIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _screen1.DuplicateWindow(null!));
    }

    [TestMethod]
    public void DuplicateWindow_Throws_IfWindowIsScreen()
    {
        Should.Throw<InvalidOperationException>(() => _screen1.DuplicateWindow(_screen1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DuplicateWindow_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.DuplicateWindow(w))
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DuplicateWindow_ReturnsNewWindow_IfCursesSucceeds()
    {
        var p = new Window(_cursesMock.Object, _screen1, new(2));
        var w = new Window(_cursesMock.Object, p, new(3));
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        var sw = _screen1.DuplicateWindow(w);
        sw.Handle.ShouldBe(new(4));
        sw.Parent.ShouldBe(p);
        sw.ShouldBeOfType<Window>();
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DuplicateWindow_ReturnsNewPad_IfWindowWasPad()
    {
        var w = new Pad(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(3));

        var sw = _screen1.DuplicateWindow(w);
        sw.ShouldBeOfType<Pad>();
    }

    [TestMethod]
    public void CreatePad_Throws_IfWidthLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.CreatePad(new(0, 1)));
    }

    [TestMethod]
    public void CreatePad_Throws_IfHeightLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.CreatePad(new(1, 0)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreatePad_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.CreatePad(new(1, 1)))
              .Operation.ShouldBe("newpad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreatePad_ReturnsNewPad_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        var w = _screen1.CreatePad(new(1, 1));
        w.Handle.ShouldBe(new(2));
        w.Parent.ShouldBe(_screen1);
    }

    [TestMethod]
    public void CreateSubPad_Throws_IfPadIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _screen1.CreateSubPad(null!, new(0, 0, 1, 1)));
    }

    [TestMethod]
    public void CreateSubPad_Throws_IfAreaOutsideBoundaries()
    {
        var p = new Pad(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.CreateSubPad(p, new(0, 0, 1, 1)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateSubPad_Throws_IfCursesFails()
    {
        var p = new Pad(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.subpad(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.CreateSubPad(p, new(0, 0, 1, 1)))
              .Operation.ShouldBe("subpad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CreateSubPad_ReturnsNewPad_IfCursesSucceeds()
    {
        var p = new Pad(_cursesMock.Object, _screen1, new(2));

        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.subpad(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sp = _screen1.CreateSubPad(p, new(0, 0, 1, 1));
        sp.Handle.ShouldBe(new(3));
        sp.Parent.ShouldBe(p);
    }

    [TestMethod]
    public void ApplyPendingRefreshes_Throws_IfScreenIsDisposed()
    {
        _screen1.Dispose();

        Should.Throw<ObjectDisposedException>(() => _screen1.ApplyPendingRefreshes());
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ApplyPendingRefreshes_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.doupdate())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _screen1.ApplyPendingRefreshes())
              .Operation.ShouldBe("doupdate");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ApplyPendingRefreshes_DrawsAll_IfCursesSucceeds()
    {
        _screen1.ApplyPendingRefreshes();

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Dispose_UsesProperCursesDeletion()
    {
        _screen1.Dispose();

        _cursesMock.Verify(v => v.endwin(), Times.Once);
        _cursesMock.Verify(v => v.delwin(new(1)), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Dispose_Succeeds_EventIfCursesFails()
    {
        _cursesMock.Setup(s => s.endwin())
                   .Returns(-1);

        _screen1.Dispose();
        _screen1.Disposed.ShouldBe(true);
    }

    [TestMethod]
    public void ForceInvalidateAndRefresh_CallsInADeepScrub()
    {
        var w1 = new Window(_cursesMock.Object, _screen1, new(6));
        var w2 = new Window(_cursesMock.Object, w1, new(7));

        _cursesMock.Setup(s => s.getmaxy(_screen1.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.getmaxy(w1.Handle))
                   .Returns(10);


        _screen1.ForceInvalidateAndRefresh();

        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(_screen1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.clearok(_screen1.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_screen1.Handle), Times.Once);
    }

    [TestMethod]
    public void Use_RegistersResolver()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("alex");

        _screen1.Use((_, nameFunc) => (new(Key.F1, new(ControlCharacter.Null), nameFunc(1), ModifierKey.None), 1));
        var done = _screen1.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.KeypadHome, new(ControlCharacter.Null), "test-1", ModifierKey.None),
                new KeyEvent(Key.F6, new(ControlCharacter.Null), "test-2", ModifierKey.None)
            }, false, out var resolved);

        done.ShouldBe(1);
        resolved.ShouldNotBeNull();
        resolved.Key.ShouldBe(Key.F1);
        resolved.Char.ShouldBe(new(ControlCharacter.Null));
        resolved.Modifiers.ShouldBe(ModifierKey.None);
        resolved.Name.ShouldBe("alex");
    }

    [TestMethod]
    public void Use_Throws_IfResolverIsNull() { Should.Throw<ArgumentNullException>(() => _screen1.Use(null!)); }

    [TestMethod]
    public void Uses_Throws_IfResolverIsNull() { Should.Throw<ArgumentNullException>(() => _screen1.Uses(null!)); }

    [TestMethod]
    public void Uses_ReturnsTrue_IfResolverRegistered()
    {
        _screen1.Use(KeySequenceResolver.AltKeyResolver);
        _screen1.Uses(KeySequenceResolver.AltKeyResolver)
                .ShouldBeTrue();
    }

    [TestMethod]
    public void Uses_ReturnsFalse_IfResolverNotRegistered()
    {
        _screen1.Uses(KeySequenceResolver.AltKeyResolver)
                .ShouldBeFalse();
    }

    [TestMethod]
    public void TryResolveKeySequence_Throws_IfSequenceIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _screen1.TryResolveKeySequence(null!, false, out var _));
    }

    [TestMethod]
    public void TryResolveKeySequence_IgnoresEmptySequences()
    {
        var count = _screen1.TryResolveKeySequence(Array.Empty<KeyEvent>(), false, out var resolved);

        count.ShouldBe(0);
        resolved.ShouldBeNull();
    }

    [TestMethod]
    public void TryResolveKeySequence_ReturnsKeysIndividually_IfNoResolvers()
    {
        var k1 = new KeyEvent(Key.KeypadHome, new(ControlCharacter.Null), null, ModifierKey.None);
        var k2 = new KeyEvent(Key.F1, new(ControlCharacter.Null), null, ModifierKey.None);

        var count = _screen1.TryResolveKeySequence(new[] { k1, k2 }, false, out var resolved);

        count.ShouldBe(1);
        resolved.ShouldBe(k1);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void TryResolveKeySequence_WaitForMoreChars_IfBestIsFalse(bool inv)
    {
        if (inv)
        {
            _screen1.Use(KeySequenceResolver.AltKeyResolver);
            _screen1.Use(KeySequenceResolver.KeyPadModifiersResolver);
        } else
        {
            _screen1.Use(KeySequenceResolver.KeyPadModifiersResolver);
            _screen1.Use(KeySequenceResolver.AltKeyResolver);
        }

        var count = _screen1.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
                new KeyEvent(Key.Character, new('O'), null, ModifierKey.None)
            }, false, out var resolved);

        count.ShouldBe(2);
        resolved.ShouldBeNull();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void TryResolveKeySequence_ReturnsBest_IfBestIsTrue(bool inv)
    {
        if (inv)
        {
            _screen1.Use(KeySequenceResolver.AltKeyResolver);
            _screen1.Use(KeySequenceResolver.KeyPadModifiersResolver);
        } else
        {
            _screen1.Use(KeySequenceResolver.KeyPadModifiersResolver);
            _screen1.Use(KeySequenceResolver.AltKeyResolver);
        }

        var count = _screen1.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
                new KeyEvent(Key.Character, new('O'), null, ModifierKey.None)
            }, true, out var resolved);

        count.ShouldBe(2);
        resolved.ShouldNotBeNull();
    }
}
