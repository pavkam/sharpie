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
public class WindowTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private Screen _screen = null!;
    private Terminal _terminal = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new(ManagedWindows: TestContext.TestName!.Contains("_WhenManaged_")));
        _screen = _terminal.Screen;
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    [TestMethod]
    public void Ctor_Throws_IfScreenIsNull() => Should.Throw<ArgumentException>(() => new Window(null!, IntPtr.MaxValue));

    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var w = new Window(_screen, new(1));

        _cursesMock.Verify(v => v.nodelay(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.notimeout(w.Handle, false), Times.Never);
        _cursesMock.Verify(v => v.syncok(w.Handle, true), Times.Once);
    }

    [TestMethod]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_1()
    {
        _ = _cursesMock.Setup(s => s.keypad(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_screen, new(1)))
              .Operation.ShouldBe("keypad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_2()
    {
        _ = _cursesMock.Setup(s => s.syncok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_screen, new(1)))
              .Operation.ShouldBe("syncok");
    }

    [TestMethod]
    public void Ctor_RegistersItselfIntoParent()
    {
        var w = new Window(_screen, IntPtr.MaxValue);
        _screen.Windows.ShouldContain(w);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedRepresentation()
    {
        var w = new Window(_screen, new(999));
        _cursesMock.MockArea(w, new Rectangle(5, 6, 100, 200));

        w.ToString()
         .ShouldBe("Window #000003E7 (100x200 @ 5x6)");
    }

    [TestMethod]
    public void Visible_WhenManaged_IsTrueByDefault()
    {
        var w = new Window(_screen, IntPtr.MaxValue);

        w.Visible.ShouldBeTrue();
    }

    [TestMethod]
    public void Screen_IsInitialized()
    {
        var w = new Window(_screen, IntPtr.MaxValue);

        w.Screen.ShouldBe(_screen);
        ((IScreen) w.Screen).ShouldBe(_screen);
    }

    [TestMethod]
    public void SubWindows_IsEmpty_WhenCreated()
    {
        var w = new Window(_screen, new(22));
        w.SubWindows.ShouldBeEmpty();
    }

    [TestMethod]
    public void SubWindows_Throws_IfWindowIsDestroyed()
    {
        var w = new Window(_screen, new(22));
        w.Destroy();

        _ = Should.Throw<ObjectDisposedException>(() => w.SubWindows.ToArray());
    }

    [TestMethod]
    public void SubWindows_ContainsTheChild_WhenPassedAsParent()
    {
        var w = new Window(_screen, IntPtr.MaxValue);
        var sw = new SubWindow(w, IntPtr.MaxValue);
        w.SubWindows.ShouldContain(sw);
    }

    [TestMethod]
    public void SubWindows_DoesNotContainTheChild_WhenChildDestroyed()
    {
        var w = new Window(_screen, IntPtr.MaxValue);
        var sw = new SubWindow(w, IntPtr.MaxValue);
        sw.Dispose();

        w.SubWindows.ShouldBeEmpty();
    }

    [TestMethod]
    public void Area_ShouldBeBoundByLocationAndSize()
    {
        var w = new Window(_screen, new(1));

        _cursesMock.MockArea(w, new Rectangle(7, 8, 58, 78));

        w.Area.ShouldBe(new(7, 8, 58, 78));
    }

    [TestMethod]
    public void Location_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.getbegx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getbegy(It.IsAny<IntPtr>()))
                   .Returns(22);

        var w = new Window(_screen, new(1));
        w.Location.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_1()
    {
        var w = new Window(_screen, new(1));

        _ = _cursesMock.Setup(s => s.getbegx(w.Handle))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_2()
    {
        var w = new Window(_screen, new(1));

        _ = _cursesMock.Setup(s => s.getbegy(w.Handle))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegy");
    }

    [TestMethod]
    public void Location_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w = new Window(_screen, new(1))
        {
            Location = new(11, 22)
        };

        _cursesMock.Verify(v => v.mvwin(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));
        var w = new Window(_screen, new(1));

        _ = _cursesMock.Setup(s => s.mvwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Location = new(1, 1))
              .Operation.ShouldBe("mvwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockArea(_screen, new Size(1, 1));

        var w = new Window(_screen, new(1));
        _cursesMock.MockArea(w, new Size(1, 1));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => w.Location = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_UpdatesLocation_IfInsideParent()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Size(1, 1));

        w.Location = new(5, 5);

        _cursesMock.Verify(v => v.mvwin(new(2), 5, 5), Times.Once);
    }

    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w = new Window(_screen, new(1))
        {
            Size = new(11, 22)
        };

        _cursesMock.Verify(v => v.wresize(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w = new Window(_screen, new(1));

        _ = _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_AdjustsAreaBasedOnParent()
    {
        _cursesMock.MockArea(_screen, new Size(90, 80));

        var w = new Window(_screen, new(1));
        _cursesMock.MockArea(w, new Rectangle(10, 10, 100, 100));

        w.Size = new(100, 100);

        _cursesMock.Verify(v => v.wresize(w.Handle, 70, 80), Times.Once);
    }

    [TestMethod]
    public void Size_Set_Throws_IfAdjustedArea_IsOutsideParent()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w = new Window(_screen, new(1));
        _cursesMock.MockArea(w, new Rectangle(101, 101, 1, 1));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => w.Size = new(6, 6));
    }

    [TestMethod]
    public void Origin_Calls_Location()
    {
        var w = new Window(_screen, new(1));

        _ = _cursesMock.Setup(s => s.getbegx(w.Handle))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getbegy(w.Handle))
                   .Returns(22);

        w.Origin.ShouldBe(new(11, 22));
    }

    [TestMethod]
    public void SubWindow_Throws_IfAdjustedAreaIsEmpty()
    {
        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Rectangle(5, 6, 18, 24));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => w.SubWindow(new(30, 30, 2, 2)));
    }

    [TestMethod]
    public void SubWindow_AdjustsArea_ToMatchParent()
    {
        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Rectangle(5, 6, 18, 24));

        _ = _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        _ = w.SubWindow(new(20, 28, 15, 18));

        _cursesMock.Verify(v => v.derwin(w.Handle, 2, 3, 28, 20), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void SubWindow_Throws_IfCursesFails()
    {
        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Size(1, 1));

        _ = _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => w.SubWindow(new(0, 0, 1, 1)))
              .Operation.ShouldBe("derwin");
    }

    [TestMethod]
    public void SubWindow_ReturnsNewWindow_IfCursesSucceeds()
    {
        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Size(100, 100));

        _ = _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sw = w.SubWindow(new(0, 0, 1, 1));
        sw.Handle.ShouldBe(new(3));
        sw.Window.ShouldBe(w);
        w.SubWindows.ShouldContain(sw);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void SubWindow_PreservesManagedCaret(bool mc)
    {
        var w = new Window(_screen, new(2));
        _cursesMock.MockArea(w, new Size(100, 100));

        _ = _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        _ = _cursesMock.Setup(s => s.is_leaveok(w.Handle))
                   .Returns(mc);

        var sw = w.SubWindow(new(0, 0, 1, 1));
        _cursesMock.Verify(s => s.leaveok(sw.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Duplicate_Throws_IfCursesFails()
    {
        var w = new Window(_screen, new(3));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(w.Duplicate)
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod]
    public void Duplicate_ReturnsNewWindow_IfCursesSucceeds()
    {
        var w = new Window(_screen, new(3));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        var sw = w.Duplicate();

        sw.Handle.ShouldBe(new(4));
        sw.Screen.ShouldBe(_screen);
        _screen.Windows.ShouldContain(sw);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Duplicate_PreservesManagedCaret(bool mc)
    {
        var w = new Window(_screen, new(3)) { ManagedCaret = mc };

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        _ = _cursesMock.Setup(s => s.is_leaveok(w.Handle))
                   .Returns(mc);

        var sw = w.Duplicate();

        _cursesMock.Verify(s => s.leaveok(sw.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Destroy_RemovesWindowFromParent()
    {
        var w = new Window(_screen, new(1));
        w.Destroy();

        _screen.Windows.ShouldBeEmpty();
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var w = new Window(_screen, new(1));

        w.Destroy();
        w.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }

    [TestMethod]
    public void Destroy_DestroysChildren()
    {
        var w = new Window(_screen, new(1));
        var sw = new SubWindow(w, new(2));

        w.Destroy();
        sw.Disposed.ShouldBeTrue();
    }

    [TestMethod, DataRow(5, 6, 10, 11, 9,
         8, 4, 2, true), DataRow(0, 0, 5, 6, 2,
         2, 2, 2, true), DataRow(0, 0, 5, 6, 0,
         0, 0, 0, true), DataRow(0, 0, 5, 6, 100,
         100, 0, 0, false)]
    public void AdjustToExplicitArea_RecalculatesTheSizeAsExpected(int x, int y, int width, int height,
        int scrWidth, int scrHeight, int expWidth, int expHeight,
        bool call)
    {
        var h = new IntPtr(1);
        _cursesMock.MockArea(h, new(x, y, width, height));
        var w = new Window(_screen, h);

        _cursesMock.MockArea(_screen, new Size(scrWidth, scrHeight));

        w.AdjustToExplicitArea();

        if (call)
        {
            _cursesMock.Verify(v => v.wresize(w.Handle, expHeight, expWidth), Times.Once);
        }
        else
        {
            _cursesMock.Verify(v => v.wresize(w.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }

    [TestMethod]
    public void AdjustToExplicitArea_DoesNotThrow_IfCursesFails_1()
    {
        var h = new IntPtr(1);
        _cursesMock.MockArea(h, new(0, 0, 10, 10));
        var w = new Window(_screen, h);

        _cursesMock.MockArea(_screen, new Size(5, 5));

        _ = _cursesMock.Setup(s => s.wresize(w.Handle, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.NotThrow(w.AdjustToExplicitArea);
    }

    [TestMethod, DataRow(0, 0, 8, 8, true), DataRow(5, 5, 15, 15, true), DataRow(5, 5, 5, 5, false)]
    public void AdjustToExplicitArea_RecalculatesTheLocationAsExpected(int x, int y, int newX, int newY,
        bool call)
    {
        var h = new IntPtr(1);
        _cursesMock.MockArea(h, new(x, y, 10, 10));
        var w = new Window(_screen, h);

        _cursesMock.MockArea(h, new(newX, newY, 10, 10));

        w.AdjustToExplicitArea();

        if (call)
        {
            _cursesMock.Verify(v => v.mvwin(w.Handle, x, y), Times.Once);
        }
        else
        {
            _cursesMock.Verify(v => v.mvwin(w.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }

    [TestMethod]
    public void AdjustToExplicitArea_DoesNotThrow_IfCursesFails_2()
    {
        var h = new IntPtr(1);
        _cursesMock.MockArea(h, new(5, 5, 10, 10));
        var w = new Window(_screen, h);

        _cursesMock.MockArea(h, new(6, 6, 4, 4));

        w.AdjustToExplicitArea();

        _ = _cursesMock.Setup(s => s.mvwin(w.Handle, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.NotThrow(w.AdjustToExplicitArea);
    }

    [TestMethod]
    public void AdjustToExplicitArea_UnderstandNegativeHeightAndWidth()
    {
        var h = new IntPtr(1);

        _cursesMock.MockArea(_screen, new Size(10, 10));
        _cursesMock.MockArea(h, new(5, 5, 2, 2));
        var w = new Window(_screen, h);

        _cursesMock.MockArea(_screen, new Size(3, 3));

        w.AdjustToExplicitArea();

        _cursesMock.Verify(v => v.wresize(w.Handle, 0, 0), Times.Once);
    }

    [TestMethod]
    public void BringToFront_WhenUnmanaged_Throws()
    {
        var w = new Window(_screen, new(1));

        _ = Should.Throw<InvalidOperationException>(w.BringToFront);
    }

    [TestMethod]
    public void BringToFront_WhenManaged_RefreshesWindow_IfNotInFront()
    {
        var w1 = new Window(_screen, new(1));
        var w2 = new Window(_screen, new(2));

        w1.BringToFront();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);

        w2.BringToFront();

        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Exactly(2));
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }

    [TestMethod]
    public void BringToFront_WhenManaged_DoesNotRefreshWindow_IfInFront()
    {
        var w = new Window(_screen, new(1));

        w.BringToFront();

        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void BringToFront_WhenManaged_DoesNotUpdate_IfInvisible()
    {
        var w1 = new Window(_screen, new(1))
        {
            Visible = false
        };

        w1.BringToFront();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }

    [TestMethod]
    public void SendToBack_WhenUnmanaged_Throws()
    {
        var w = new Window(_screen, new(1));

        _ = Should.Throw<InvalidOperationException>(w.SendToBack);
    }

    [TestMethod]
    public void SendToBack_WhenManaged_TouchesAndRefreshesAffectedWindowsAbove_IfNotInBack()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.SendToBack();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void SendToBack_WhenManaged_DoesNotTouchWindowsThatDoNotIntersect()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(50, 50, 10, 10));

        w2.SendToBack();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void SendToBack_WhenManaged_WhenManaged_DoesNotDoAnything_IfInBack()
    {
        var w = new Window(_screen, new(1));

        w.SendToBack();

        _cursesMock.Verify(v => v.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);

        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void SendToBack_WhenManaged_DoesNotUpdate_IfInvisible()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));
        w2.Visible = false;

        w2.SendToBack();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }

    [TestMethod]
    public void Refresh1_WhenUnmanaged_OnlyUpdatesWindow()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(4));
        _cursesMock.MockArea(w2, new Rectangle(0, 0, 5, 5));

        w1.Refresh();

        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(w2.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void Refresh1_WhenManaged_TouchesAndRefreshesAffectedWindowAndAbove()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.Refresh();

        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w3.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w4.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Refresh1_WhenManaged_DoesNotTouchInvisibleWindowsAbove()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));
        w3.Visible = false;

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.Refresh();
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w3.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w4.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Exactly(2));
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }

    [TestMethod]
    public void Refresh1_WhenManaged_DoesNothing_IfNotVisible()
    {
        var w1 = new Window(_screen, new(1))
        {
            Visible = false
        };

        w1.Refresh();

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Refresh2_WhenUnmanaged_OnlyRedrawsAffectedLinesOfWindow()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));
        var w2 = new Window(_screen, new(4));
        _cursesMock.MockArea(w2, new Rectangle(0, 0, 5, 5));

        w1.Refresh(0, 10);

        _cursesMock.Verify(v => v.wrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wrefresh(w2.Handle), Times.Never);
        _cursesMock.Verify(v => v.wredrawln(w1.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _cursesMock.Verify(v => v.wredrawln(w2.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void Refresh2_WhenManaged_TouchesAndRefreshesAffectedWindowAndAbove()
    {
        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.Refresh(0, 1);

        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wredrawln(w2.Handle, 0, 1), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Refresh2_WhenManaged_DoesNothing_IfNotVisible()
    {
        var w1 = new Window(_screen, new(1))
        {
            Visible = false
        };

        w1.Refresh(1, 1);

        _cursesMock.Verify(v => v.wredrawln(w1.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Visible_Set_WhenUnmanaged_Throws()
    {
        var w = new Window(_screen, new(1));
        _ = Should.Throw<InvalidOperationException>(() => w.Visible = false);
    }

    [TestMethod]
    public void Visible_Get_WhenUnmanaged_Throws()
    {
        var w = new Window(_screen, IntPtr.MaxValue);

        _ = Should.Throw<InvalidOperationException>(() => _ = w.Visible.ToString());
    }

    [TestMethod]
    public void Visible_WhenManaged_SetToTrue_DoesNothingIfAlreadyTrue()
    {
        var w = new Window(_screen, IntPtr.MaxValue)
        {
            Visible = true
        };

        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Never);
    }

    [TestMethod]
    public void Visible_WhenManaged_SetToFalse_HidesTheWindow()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.Visible = false;

        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w3.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);

        _cursesMock.Verify(v => v.wnoutrefresh(_screen.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Once);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Visible_WhenManaged_SetToFalse_DoesNothingIfAlreadyFalse()
    {
        var w = new Window(_screen, IntPtr.MaxValue)
        {
            Visible = false
        };

        _cursesMock.Verify(v => v.doupdate(), Times.Once);

        w.Visible = false;

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Visible_WhenManaged_SetToTrue_ShowsTheWindow()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        var w3 = new Window(_screen, new(3));
        _cursesMock.MockArea(w3, new Rectangle(11, 11, 10, 10));

        var w4 = new Window(_screen, new(4));
        _cursesMock.MockArea(w4, new Rectangle(50, 50, 10, 10));

        w2.Visible = false;
        w2.Visible = true;

        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(w3.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Exactly(2));
        _cursesMock.Verify(v => v.wnoutrefresh(_screen.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w2.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w3.Handle), Times.Exactly(2));
        _cursesMock.Verify(v => v.wnoutrefresh(w4.Handle), Times.Once);

        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }

    [TestMethod]
    public void Visible_WhenManaged_Set_SkipsOtherInvisibleWindows()
    {
        _cursesMock.MockArea(_screen, new Size(100, 100));

        var w1 = new Window(_screen, new(1));
        _cursesMock.MockArea(w1, new Rectangle(0, 0, 10, 10));

        var w2 = new Window(_screen, new(2));
        _cursesMock.MockArea(w2, new Rectangle(5, 5, 10, 10));

        w1.Visible = false;

        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);

        w2.Visible = false;

        _cursesMock.Verify(v => v.wtouchln(w1.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Never);
        _cursesMock.Verify(v => v.wtouchln(w2.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(w1.Handle), Times.Never);
        _cursesMock.Verify(v => v.doupdate(), Times.Exactly(2));
    }
}
