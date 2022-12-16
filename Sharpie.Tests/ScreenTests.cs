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
    private Screen _screen = null!;
    private Terminal _terminal = null!;
    
    [UsedImplicitly]
    public TestContext TestContext { get; set; } = null!;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new(ManagedWindows: TestContext.TestName!.Contains("_WhenManaged_")));
        _screen = _terminal.Screen;
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfTerminalIsNull() { Should.Throw<ArgumentNullException>(() => new Screen(null!, new(1))); }

    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var w = new Screen(_terminal, new(1));

        _cursesMock.Verify(v => v.nodelay(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.notimeout(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.syncok(w.Handle, true), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_1()
    {
        _cursesMock.Setup(s => s.notimeout(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Screen(_terminal, new(1)))
              .Operation.ShouldBe("notimeout");
    }

    [TestMethod]
    public void ManagedWindows_WhenManaged_IsTrue()
    {
        _terminal.Screen.ManagedWindows.ShouldBeTrue();
    }
    
    [TestMethod]
    public void ManagedWindows_WhenUnmanaged_IsFalse()
    {
        _terminal.Screen.ManagedWindows.ShouldBeFalse();
    }

    [TestMethod] public void Terminal_IsInitialized() { _screen.Terminal.ShouldBe(_terminal); }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Fails_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _screen.Refresh())
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds()
    {
        _screen.Refresh();
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
    }

    [TestMethod]
    public void Windows_IsEmpty_WhenCreated()
    {
        _screen.Windows.ShouldBeEmpty();
        ((IScreen) _screen).Windows.ShouldBe(_screen.Windows);
    }

    [TestMethod]
    public void Windows_ContainsTheChild_WhenPassedAsParent()
    {
        var w = new Window(_screen, IntPtr.MaxValue);

        _screen.Windows.ShouldContain(w);
    }

    [TestMethod]
    public void Windows_DoesNotContainTheChild_WhenChildDestroyed()
    {
        var w = new Window(_screen, IntPtr.MaxValue);
        w.Dispose();

        _screen.Windows.ShouldBeEmpty();
    }

    [TestMethod]
    public void Pads_IsEmpty_WhenCreated()
    {
        _screen.Pads.ShouldBeEmpty();
        ((IScreen) _screen).Pads.ShouldBe(_screen.Pads);
    }

    [TestMethod]
    public void Pads_Throws_IfScreenIsDestroyed()
    {
        _screen.Destroy();

        Should.Throw<ObjectDisposedException>(() => _screen.Pads.ToArray());
    }

    [TestMethod]
    public void Windows_Throws_IfScreenIsDestroyed()
    {
        _screen.Destroy();

        Should.Throw<ObjectDisposedException>(() => _screen.Windows.ToArray());
    }

    [TestMethod]
    public void Pads_ContainsTheChild_WhenPassedAsParent()
    {
        var p = new Pad(_screen, IntPtr.MaxValue);

        _screen.Pads.ShouldContain(p);
    }

    [TestMethod]
    public void Pads_DoesNotContainTheChild_WhenChildDestroyed()
    {
        var p = new Pad(_screen, IntPtr.MaxValue);
        p.Dispose();

        _screen.Pads.ShouldBeEmpty();
    }

    [TestMethod]
    public void Window_Throws_IfAreaOutsideBoundaries()
    {
        _cursesMock.MockSmallArea(_screen);
        Should.Throw<ArgumentOutOfRangeException>(() => _screen.Window(new(0, 0, 2, 2)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Window_Throws_IfCursesFails()
    {
        _cursesMock.MockLargeArea(_screen);

        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen.Window(new(0, 0, 1, 1)))
              .Operation.ShouldBe("newwin");
    }

    [TestMethod]
    public void Window_ReturnsNewWindow_IfCursesSucceeds()
    {
        _cursesMock.MockLargeArea(_screen);

        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        var w = _screen.Window(new(1, 2, 3, 4));
        w.Handle.ShouldBe(new(2));
        w.Screen.ShouldBe(_screen);
        _screen.Windows.ShouldContain(w);

        _cursesMock.Verify(v => v.newwin(4, 3, 2, 1), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Window_PreservesManagedCaret(bool mc)
    {
        _cursesMock.MockLargeArea(_screen);
        _cursesMock.Setup(s => s.newwin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        _cursesMock.Setup(s => s.is_leaveok(_screen.Handle))
                   .Returns(mc);

        var w = _screen.Window(new(1, 2, 3, 4));

        _cursesMock.Verify(s => s.leaveok(w.Handle, mc), Times.Once);
    }

    [TestMethod]
    public void Pad_Throws_IfWidthLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen.Pad(new(0, 1)));
    }

    [TestMethod]
    public void Pad_Throws_IfHeightLessThanOne()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => _screen.Pad(new(1, 0)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Pad_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => _screen.Pad(new(1, 1)))
              .Operation.ShouldBe("newpad");
    }

    [TestMethod]
    public void Pad_ReturnsNewPad_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        var w = _screen.Pad(new(1, 2));
        w.Handle.ShouldBe(new(2));
        w.Screen.ShouldBe(_screen);
        _screen.Pads.ShouldContain(w);

        _cursesMock.Verify(v => v.newpad(2, 1), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Pad_PreservesManagedCaret(bool mc)
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(2));

        _cursesMock.Setup(s => s.is_leaveok(_screen.Handle))
                   .Returns(mc);

        var p = _screen.Pad(new(1, 2));

        _cursesMock.Verify(s => s.leaveok(p.Handle, mc), Times.Once);
    }

    [TestMethod]
    public void MarkDirty_WhenUnmanaged_OnlyMarksScreenAsDirty()
    {
        var w = new Window(_screen, new(1));

        _cursesMock.MockArea(_screen, new(0, 0, 100, 100));
        _cursesMock.MockArea(w, new(0, 0, 100, 100));
        
        _screen.MarkDirty(1, 50);
        
        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, 1, 50, 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void MarkDirty_WhenManaged_PropagatesOnChildren()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(2));
        var w3 = new Window(_screen, new(3));

        _cursesMock.MockLargeArea(_screen);
        _cursesMock.MockArea(w1, new(0, 0, 100, 100));
        _cursesMock.MockArea(w2, new(100, 110, 50, 50));
        _cursesMock.MockArea(w3, new(300, 200, 100, 100));

        _screen.MarkDirty(75, 50);

        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, 75, 50, 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, 75, 25, 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, 0, 15, 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w3.Handle, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never());
    }
    
    [TestMethod]
    public void Refresh1_WhenManaged_RefreshesAllWindows_NoBatch()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(2));

        _screen.Refresh();

        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(w2.Handle), Times.Once);

        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void Refresh1_WhenManaged_RefreshesAllWindows_InBatch()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(2));

        using (_terminal.AtomicRefresh())
        {
            _screen.Refresh();
        }

        _cursesMock.Verify(v => v.wnoutrefresh(_screen.Handle));
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle));
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle));
        _cursesMock.Verify(v => v.doupdate());
    }
    
    [TestMethod]
    public void Refresh2_WhenUnmanaged_OnlyRefreshesTheScreen()
    {
        var w = new Window(_screen, new(1));

        _cursesMock.MockArea(_screen, new(0, 0, 100, 100));
        _cursesMock.MockArea(w, new(0, 0, 100, 100));

        _screen.Refresh(1, 50);

        _cursesMock.Verify(v => v.wredrawln(_screen.Handle, 1, 50), Times.Once);
        _cursesMock.Verify(v => v.wredrawln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }
    
    [TestMethod]
    public void Refresh2_WhenManaged_PropagatesOnChildren()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(2));
        var w3 = new Window(_screen, new(3));

        _cursesMock.MockLargeArea(_screen);
        _cursesMock.MockArea(w1, new(0, 0, 100, 100));
        _cursesMock.MockArea(w2, new(100, 110, 50, 50));
        _cursesMock.MockArea(w3, new(300, 200, 100, 100));

        _screen.Refresh(75, 50);

        _cursesMock.Verify(v => v.wredrawln(_screen.Handle, 75, 50), Times.Once);
        _cursesMock.Verify(v => v.wredrawln(w1.Handle, 75, 25), Times.Once);
        _cursesMock.Verify(v => v.wredrawln(w2.Handle, 0, 15), Times.Once);
        _cursesMock.Verify(v => v.wredrawln(w3.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        _screen.Destroy();

        _cursesMock.Verify(v => v.endwin(), Times.Once);
        _cursesMock.Verify(v => v.delwin(new(100)), Times.Once);
    }

    [TestMethod]
    public void Destroy_DestroysChildren()
    {
        var p = new Pad(_screen, new(1));
        var w = new Window(_screen, new(1));

        _screen.Destroy();
        p.Disposed.ShouldBeTrue();
        w.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void AdjustToExplicitArea_AdjustsEachChild()
    {
        var h1 = new IntPtr(1);
        _cursesMock.MockArea(h1, new(0, 0, 5, 5));
        var w1 = new Window(_screen, h1);
        var h2 = new IntPtr(2);
        _cursesMock.MockArea(h2, new(1, 1, 3, 3));
        var w2 = new Window(_screen, h2);

        _cursesMock.MockArea(_screen, new(0, 0, 2, 2));

        _screen.AdjustChildrenToExplicitArea();

        _cursesMock.Verify(v => v.wresize(w1.Handle, 2, 2), Times.Once);
        _cursesMock.Verify(v => v.wresize(w2.Handle, 1, 1), Times.Once);
    }
}
