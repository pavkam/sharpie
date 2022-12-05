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
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
        _screen1 = new(_cursesMock.Object, _terminal, new(1));
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    private void MockLargeArea(IntPtr window)
    {
        _cursesMock.Setup(s => s.getmaxx(window))
                   .Returns(1000);

        _cursesMock.Setup(s => s.getmaxy(window))
                   .Returns(1000);
    }

    private void MockSmallArea(IntPtr window)
    {
        _cursesMock.Setup(s => s.getmaxx(window))
                   .Returns(1);

        _cursesMock.Setup(s => s.getmaxy(window))
                   .Returns(1);
    }

    [TestMethod]
    public void Ctor_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new Screen(null!, _terminal, new(1)));
    }

    [TestMethod]
    public void Ctor_Throws_IfTerminalIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new Screen(_cursesMock.Object, null!, new(1)));
    }

    [TestMethod]
    public void Ctor_Throws_IfHandleIsZero()
    {
        Should.Throw<ArgumentException>(() => new Screen(_cursesMock.Object, _terminal, IntPtr.Zero));
    }

    [TestMethod] public void Terminal_IsInitialized() { _screen1.Terminal.ShouldBe(_terminal); }

    [TestMethod] public void Parent_IsNull() { _screen1.Parent.ShouldBeNull(); }

    [TestMethod] public void Location_Get_ReturnsZero_Always() { _screen1.Location.ShouldBe(Point.Empty); }

    [TestMethod]
    public void Location_Set_Throws_Always()
    {
        Should.Throw<NotSupportedException>(() => _screen1.Location = Point.Empty);
    }

    [TestMethod]
    public void Size_Get_ReturnsTheSize()
    {
        MockLargeArea(new(1));
        _screen1.Size.ShouldBe(new(1000, 1000));
    }

    [TestMethod]
    public void Size_Set_Throws_Always() { Should.Throw<NotSupportedException>(() => _screen1.Size = new(1, 1)); }

    [TestMethod]
    public void SubWindow_Throws_IfAreaOutsideBoundaries()
    {
        MockSmallArea(new(1));

        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.SubWindow(new(0, 0, 2, 2)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void SubWindow_Throws_IfCursesFails()
    {
        MockLargeArea(new(1));

        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.SubWindow(new(0, 0, 1, 1)))
              .Operation.ShouldBe("newwin");
    }
    
    [TestMethod]
    public void Duplicate_Throws_Always()
    {
        Should.Throw<InvalidOperationException>(() => _screen1.Duplicate());
    }

    [TestMethod]
    public void Pad_Throws_IfWidthLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.Pad(new(0, 1)));
    }

    [TestMethod]
    public void Pad_Throws_IfHeightLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen1.Pad(new(1, 0)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Pad_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen1.Pad(new(1, 1)))
              .Operation.ShouldBe("newpad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Pad_ReturnsNewPad_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        var w = _screen1.Pad(new(1, 1));
        w.Handle.ShouldBe(new(2));
        w.Parent.ShouldBe(_screen1);
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
}
