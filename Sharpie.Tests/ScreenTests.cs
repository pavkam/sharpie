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

using System.Drawing;

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

        Should.Throw<CursesException>(() => _screen1.CreateWindow(new(0, 0, 1, 1)))
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

        _cursesMock.Setup(s => s.is_pad(new(2)))
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

        Should.Throw<CursesException>(() => _screen1.CreateSubWindow(w, new(0, 0, 1, 1)))
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

        Should.Throw<CursesException>(() => _screen1.DuplicateWindow(w))
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

        _cursesMock.Verify(v => v.is_pad(new(3)), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DuplicateWindow_ReturnsNewPad_IfWindowWasPad()
    {
        var w = new Pad(_cursesMock.Object, _screen1, new(2));
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(3));

        _cursesMock.Setup(s => s.is_pad(new(2)))
                   .Returns(true);

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

        Should.Throw<CursesException>(() => _screen1.CreatePad(new(1, 1)))
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

        Should.Throw<CursesException>(() => _screen1.CreateSubPad(p, new(0, 0, 1, 1)))
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

        Should.Throw<CursesException>(() => _screen1.ApplyPendingRefreshes())
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
}
