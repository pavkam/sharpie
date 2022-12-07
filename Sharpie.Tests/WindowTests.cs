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
public class WindowTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private Terminal _terminal = null!;
    private Screen _screen = null!;
  
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
    public void Ctor_Throws_IfScreenIsNull()
    {
        Should.Throw<ArgumentException>(() => new Window(_cursesMock.Object, null!, IntPtr.MaxValue));
    }
    
    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));

        _cursesMock.Verify(v => v.nodelay(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(w.Handle, true), Times.Never);
        _cursesMock.Verify(v => v.notimeout(w.Handle, false), Times.Never);
        _cursesMock.Verify(v => v.syncok(w.Handle, true), Times.Never);
    }

    [TestMethod]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_1()
    {
        _cursesMock.Setup(s => s.keypad(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, _screen, new(1)))
              .Operation.ShouldBe("keypad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_2()
    {
        _cursesMock.Setup(s => s.notimeout(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, _screen, new(1)))
              .Operation.ShouldBe("notimeout");
    }
    
    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_3()
    {
        _cursesMock.Setup(s => s.syncok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, _screen, new(1)))
              .Operation.ShouldBe("syncok");
    }
    
    [TestMethod]
    public void Ctor_RegistersItselfIntoParent()
    {
        var w = new Window(_cursesMock.Object, _screen, IntPtr.MaxValue);

        _screen.Windows.ShouldContain(w);
    }

    [TestMethod]
    public void Screen_IsInitialized()
    {
        var w = new Window(_cursesMock.Object, _screen, IntPtr.MaxValue);

        w.Screen.ShouldBe(_screen);
    }
    
    [TestMethod]
    public void SubWindows_IsEmpty_WhenCreated()
    {
        var w = new Window(_cursesMock.Object, _screen, new(22));
        w.SubWindows.ShouldBeEmpty();
    }
    
    [TestMethod]
    public void SubWindows_ContainsTheChild_WhenPassedAsParent()
    {
        var w = new Window(_cursesMock.Object, _screen, IntPtr.MaxValue);
        var c1 = new SubWindow(_cursesMock.Object, w, IntPtr.MaxValue);
        w.SubWindows.ShouldContain(c1);
    }

    [TestMethod]
    public void SubWindows_DoesNotContainTheChild_WhenChildDestroyed()
    {
        var w = new Window(_cursesMock.Object, _screen, IntPtr.MaxValue);
        var c1 = new SubWindow(_cursesMock.Object, w, IntPtr.MaxValue);
        c1.Dispose();
        
        w.SubWindows.ShouldBeEmpty();
    }
    
    [TestMethod]
    public void UseHardwareLineEdit_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_idlok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareLineEdit.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareLineEdit_Set_DoesNotSetValue_IfNotSupportedByHardware(bool enabled)
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(false);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareLineEdit = enabled;

        _cursesMock.Verify(v => v.idlok(new(1), enabled), Times.Never);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareLineEdit_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(true);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareLineEdit = enabled;

        _cursesMock.Verify(v => v.idlok(new(1), enabled), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void UseHardwareLineEdit_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(true);

        _cursesMock.Setup(s => s.idlok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));

        Should.Throw<CursesOperationException>(() => w.UseHardwareLineEdit = false)
              .Operation.ShouldBe("idlok");
    }

    [TestMethod]
    public void UseHardwareCharEdit_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_idcok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareCharEdit.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareCharEdit_Set_DoesNotSetValue_IfNotSupportedByHardware(bool enabled)
    {
        _cursesMock.Setup(s => s.has_ic())
                   .Returns(false);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareCharEdit = enabled;

        _cursesMock.Verify(v => v.idcok(new(1), enabled), Times.Never);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareCharEdit_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        _cursesMock.Setup(s => s.has_ic())
                   .Returns(true);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.UseHardwareCharEdit = enabled;

        _cursesMock.Verify(v => v.idcok(new(1), enabled), Times.Once);
    }

    [TestMethod]
    public void Location_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getbegx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _cursesMock.Setup(s => s.getbegy(It.IsAny<IntPtr>()))
                   .Returns(22);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Location.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.getbegx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.getbegy(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegy");
    }

    [TestMethod]
    public void Location_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Location = new(11, 22);

        _cursesMock.Verify(v => v.mvwin(new(1), 22, 11), Times.Once);
    }
    
    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));

        _cursesMock.Setup(s => s.mvwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Location = new(1, 1))
              .Operation.ShouldBe("mvwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockSmallArea(_screen);
        
        var w = new Window(_cursesMock.Object, _screen, new(1));
        _cursesMock.MockSmallArea(w);
        
        Should.Throw<ArgumentOutOfRangeException>(() => w.Location = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_UpdatesLocation_IfInsideParent()
    {
        _cursesMock.MockLargeArea(_screen);

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        var w = new Window(_cursesMock.Object, _screen, new(2));
        _cursesMock.MockSmallArea(w);
        
        w.Location = new(5, 5);

        _cursesMock.Verify(v => v.mvwin(new(2), 5, 5), Times.Once);
    }
    
    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Size = new(11, 22);

        _cursesMock.Verify(v => v.wresize(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));

        _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockSmallArea(_screen);

        var w = new Window(_cursesMock.Object, _screen, new(1));

        Should.Throw<ArgumentOutOfRangeException>(() => w.Size = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_UpdatesSize_IfInsideParent()
    {
        _cursesMock.MockLargeArea(_screen);

        var w = new Window(_cursesMock.Object, _screen, new(1));

        w.Size = new(5, 5);

        _cursesMock.Verify(v => v.wresize(w.Handle, 5, 5), Times.Once);
    }

    [TestMethod]
    public void ImmediateRefresh_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_immedok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.ImmediateRefresh.ShouldBeTrue();
    }

    [TestMethod]
    public void ImmediateRefresh_Sets_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.ImmediateRefresh = true;

        _cursesMock.Verify(v => v.immedok(new(1), true), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Refresh_AsksCursesForEntireScreenRefresh_BasedOnEntireScreen(bool enable)
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Refresh(enable, true);

        _cursesMock.Verify(v => v.clearok(w.Handle, enable), Times.AtMost(2));
    }

    [TestMethod]
    public void Refresh_AsksCursesForQueue_IfBatchIsTrue()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Refresh(true, false);

        _cursesMock.Verify(v => v.wnoutrefresh(w.Handle), Times.Once);
    }

    [TestMethod]
    public void Refresh_AsksCursesForRefresh_IfBatchIsFalse()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Refresh(false, false);

        _cursesMock.Verify(v => v.wrefresh(w.Handle), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.clearok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(false, false))
              .Operation.ShouldBe("clearok");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(false, false))
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_3()
    {
        _cursesMock.Setup(s => s.wnoutrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(true, false))
              .Operation.ShouldBe("wnoutrefresh");
    }

    [TestMethod]
    public void Refresh2_Throws_IfYIsNegative()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(-1, 1));
    }

    [TestMethod]
    public void Refresh2_Throws_IfYIsOutsideBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(10, 1));
    }

    [TestMethod]
    public void Refresh2_Throws_IfCountIsLessThanOne()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(0, 0));
    }

    [TestMethod]
    public void Refresh2_Throws_IfCountGreaterThanBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(1, 10));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _cursesMock.Setup(s => s.wredrawln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(1, 1))
              .Operation.ShouldBe("wredrawln");
    }

    [TestMethod]
    public void Refresh2_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, _screen, new(1));
        Should.NotThrow(() => w.Refresh(1, 9));
    }

    [TestMethod]
    public void SubWindow_Throws_IfAreaOutsideBoundaries()
    {
        var w = new Window(_cursesMock.Object, _screen, new(2));
        _cursesMock.MockSmallArea(w);

        Should.Throw<ArgumentOutOfRangeException>(() => w.SubWindow(new(0, 0, 2, 2)));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void SubWindow_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen, new(2));
        _cursesMock.MockSmallArea(w);
        
        _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => w.SubWindow(new(0, 0, 1, 1)))
              .Operation.ShouldBe("derwin");
    }

    [TestMethod]
    public void SubWindow_ReturnsNewWindow_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, _screen, new(2));
        _cursesMock.MockLargeArea(w);

        _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
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
        var w = new Window(_cursesMock.Object, _screen, new(2)) { ManagedCaret = mc };
        _cursesMock.MockLargeArea(w);

        _cursesMock.Setup(s => s.derwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>()))
                   .Returns(new IntPtr(3));

        var sw = w.SubWindow(new(0, 0, 1, 1));
        sw.ManagedCaret.ShouldBe(mc);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Duplicate_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, _screen, new(3));
        
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(() => w.Duplicate())
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod]
    public void Duplicate_ReturnsNewWindow_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, _screen, new(3));
        
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        var sw = w.Duplicate();
        
        sw.Handle.ShouldBe(new(4));
        sw.Screen.ShouldBe(_screen);
        _screen.Windows.ShouldContain(sw);
    }
    
    [TestMethod, DataRow(true), DataRow(false)]
    public void Duplicate_PreservesManagedCaret(bool mc)
    {
        var w = new Window(_cursesMock.Object, _screen, new(3)) { ManagedCaret = mc };
        
        _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        var sw = w.Duplicate();
        sw.ManagedCaret.ShouldBe(mc);
    }
    
    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Destroy_RemovesWindowFromParent()
    {
        var w = new Window(_cursesMock.Object, _screen, new(1));
        w.Destroy();
        
        _screen.Windows.ShouldBeEmpty();
    }
}
