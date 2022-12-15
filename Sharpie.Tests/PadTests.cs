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
public class PadTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private Pad _pad = null!;
    private Screen _screen = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
        _screen = _terminal.Screen;
        _pad = new(_screen, new(2));
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfScreenIsNull() { Should.Throw<ArgumentException>(() => new Pad(null!, IntPtr.MaxValue)); }

    [TestMethod]
    public void Screen_IsInitialized()
    {
        var p = new Pad(_screen, IntPtr.MaxValue);

        p.Screen.ShouldBe(_screen);
        ((IPad) p).Screen.ShouldBe(_screen);
    }

    [TestMethod]
    public void SubPads_IsEmpty_WhenCreated()
    {
        var p = new Pad(_screen, new(22));
        p.SubPads.ShouldBeEmpty();
    }

    [TestMethod]
    public void SubPads_Throws_IfPadIsDestroyed()
    {
        var p = new Pad(_screen, new(22));
        p.Destroy();

        Should.Throw<ObjectDisposedException>(() => p.SubPads.ToArray());
    }

    [TestMethod]
    public void SubPads_ContainsTheChild_WhenPassedAsParent()
    {
        var p = new Pad(_screen, IntPtr.MaxValue);
        var sp = new SubPad(p, IntPtr.MaxValue);
        p.SubPads.ShouldContain(sp);
    }

    [TestMethod]
    public void SubPads_DoesNotContainTheChild_WhenChildDestroyed()
    {
        var p = new Pad(_screen, IntPtr.MaxValue);
        var sp = new SubPad(p, IntPtr.MaxValue);
        sp.Dispose();

        p.SubPads.ShouldBeEmpty();
    }

    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockLargeArea(_screen);

        var p = new Pad(_screen, new(1));
        p.Size = new(11, 22);

        _cursesMock.Verify(v => v.wresize(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockLargeArea(_screen);

        var p = new Pad(_screen, new(1));

        _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => p.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockSmallArea(_screen);

        var p = new Pad(_screen, new(1));

        Should.Throw<ArgumentOutOfRangeException>(() => p.Size = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_UpdatesSize_IfInsideParent()
    {
        _cursesMock.MockLargeArea(_screen);

        var p = new Pad(_screen, new(1));

        p.Size = new(5, 5);

        _cursesMock.Verify(v => v.wresize(p.Handle, 5, 5), Times.Once);
    }

    [TestMethod]
    public void Refresh1_Throws_IfTheRectIsOutsideThePadBounds()
    {
        _cursesMock.MockSmallArea(_pad);
        Should.Throw<ArgumentOutOfRangeException>(() => { _pad.Refresh(new(1, 1, 5, 5), new(0, 0)); });
    }

    [TestMethod]
    public void Refresh1_Throws_IfTheRectIsOutsideTheScreenBounds()
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.MockSmallArea(_screen);
        Should.Throw<ArgumentOutOfRangeException>(() => { _pad.Refresh(new(1, 1, 5, 5), new(0, 0)); });
    }

    [TestMethod]
    public void Refresh1_Throws_IfThePositionIsOutsideTheBounds()
    {
        _cursesMock.MockSmallArea(_pad);
        Should.Throw<ArgumentOutOfRangeException>(() => { _pad.Refresh(new(0, 0, 5, 5), new(6, 6)); });
    }

    [TestMethod]
    public void Refresh1_CallsCurses_InBatch()
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.MockLargeArea(_screen);

        using (_terminal.AtomicRefresh())
        {
            _pad.Refresh(new(0, 1, 2, 3), new(2, 3));
        }

        _cursesMock.Verify(v => v.pnoutrefresh(_pad.Handle, 1, 0, 3, 2,
            6, 4), Times.Once);
    }

    [TestMethod]
    public void Refresh1_CallsCurses_NoBatch()
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.MockLargeArea(_screen);

        _pad.Refresh(new(0, 1, 2, 3), new(2, 3));

        _cursesMock.Verify(v => v.prefresh(_pad.Handle, 1, 0, 3, 2,
            6, 4), Times.Once);
    }

    [TestMethod]
    public void Refresh2_Calls_Refresh1_InBatch()
    {
        _cursesMock.MockLargeArea(_screen);
        _cursesMock.MockArea(_pad, new(0, 0, 5, 6));

        using (_terminal.AtomicRefresh())
        {
            _pad.Refresh(new(2, 3));
        }

        _cursesMock.Verify(v => v.pnoutrefresh(_pad.Handle, 0, 0, 3, 2,
            9, 7), Times.Once);
    }

    [TestMethod]
    public void Refresh2_Calls_Refresh1_NoBatch()
    {
        _cursesMock.MockLargeArea(_screen);
        _cursesMock.MockArea(_pad, new(0, 0, 5, 6));
        _pad.Refresh(new(2, 3));

        _cursesMock.Verify(v => v.prefresh(_pad.Handle, 0, 0, 3, 2,
            9, 7), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh1_Throws_IfCursesFails_InBatch()
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.MockLargeArea(_screen);

        _cursesMock.Setup(s => s.pnoutrefresh(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        using (_terminal.AtomicRefresh())
        {
            Should.Throw<CursesOperationException>(() => { _pad.Refresh(new(0, 0, 1, 1), new(0, 0)); })
                  .Operation.ShouldBe("pnoutrefresh");
        }
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh1_Throws_IfCursesFails_NoBatch()
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.MockLargeArea(_screen);

        _cursesMock.Setup(s => s.prefresh(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _pad.Refresh(new(0, 0, 1, 1), new(0, 0)); })
              .Operation.ShouldBe("prefresh");
    }

    [TestMethod]
    public void SubPad_Throws_IfAreaOutsideBoundaries()
    {
        _cursesMock.MockSmallArea(_pad);

        Should.Throw<ArgumentOutOfRangeException>(() =>
            _pad.SubPad(new(0, 0, _pad.Size.Width + 1, _pad.Size.Height + 1)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void SubPad_Throws_IfCursesFails()
    {
        _cursesMock.MockLargeArea(_pad);

        _cursesMock.Setup(s => s.subpad(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _pad.SubPad(new(0, 0, 1, 1)))
              .Operation.ShouldBe("subpad");
    }

    [TestMethod]
    public void SubPad_ReturnsNewPad_IfCursesSucceeds()
    {
        _cursesMock.MockLargeArea(_pad);

        _cursesMock.Setup(s => s.subpad(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sp = _pad.SubPad(new(0, 0, 1, 1));
        sp.Handle.ShouldBe(new(3));
        sp.Pad.ShouldBe(_pad);
        _pad.SubPads.ShouldContain(sp);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void SubPad_PreservesManagedCaret(bool mc)
    {
        _cursesMock.MockLargeArea(_pad);
        _cursesMock.Setup(s => s.subpad(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        _cursesMock.Setup(s => s.is_leaveok(_pad.Handle))
                   .Returns(mc);

        var sw = _pad.SubPad(new(0, 0, 1, 1));

        _cursesMock.Verify(s => s.leaveok(sw.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Duplicate_Throws_IfCursesFails()
    {
        Should.Throw<CursesOperationException>(() => _pad.Duplicate())
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod]
    public void Duplicate_ReturnsNewPad_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(3));

        var p = _pad.Duplicate();

        p.Screen.ShouldBe(_screen);
        p.Handle.ShouldBe(new(3));
        _screen.Pads.ShouldContain(p);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Duplicate_PreservesManagedCaret(bool mc)
    {
        var p = new Pad(_screen, new(3)) { ManagedCaret = mc };

        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        _cursesMock.Setup(s => s.is_leaveok(p.Handle))
                   .Returns(mc);

        var sw = p.Duplicate();

        _cursesMock.Verify(s => s.leaveok(sw.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Destroy_RemovesWindowFromParent()
    {
        var p = new Pad(_screen, new(1));
        p.Destroy();

        _screen.Windows.ShouldBeEmpty();
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var p = new Pad(_screen, new(1));

        p.Destroy();
        p.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }

    [TestMethod]
    public void Destroy_DestroysChildren()
    {
        var p = new Pad(_screen, new(1));
        var sp = new SubPad(p, new(2));

        p.Destroy();
        sp.Disposed.ShouldBeTrue();
    }
}
