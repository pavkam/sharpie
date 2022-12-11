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

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
        _screen = (Screen) _terminal.Screen;
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

    [TestMethod] public void Windows_IsEmpty_WhenCreated() { _screen.Windows.ShouldBeEmpty(); }

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

    [TestMethod] public void Pads_IsEmpty_WhenCreated() { _screen.Pads.ShouldBeEmpty(); }

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
    public void Refresh_RefreshesAllWindows_NoBatch()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(1));

        _screen.Refresh();
        
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle));
        _cursesMock.Verify(v => v.wrefresh(w1.Handle));
        _cursesMock.Verify(v => v.wrefresh(w2.Handle));
    }
    
    [TestMethod]
    public void Refresh_RefreshesAllWindows_InBatch()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(1));

        using (_terminal.BatchUpdates())
        {
            _screen.Refresh();
        }

        _cursesMock.Verify(v => v.wnoutrefresh(_screen.Handle));
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle));
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle));
        _cursesMock.Verify(v => v.doupdate());
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
}
