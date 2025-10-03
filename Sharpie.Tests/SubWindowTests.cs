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
public class SubWindowTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private Window _parent = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
        _parent = new(_terminal.Screen, new(200));
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    [TestMethod]
    public void Ctor_Throws_IfScreenIsNull() =>
        Should.Throw<ArgumentException>(() => new SubWindow(null!, IntPtr.MaxValue));

    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var sw = new SubWindow(_parent, new(1));

        _cursesMock.Verify(v => v.nodelay(sw.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(sw.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(sw.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.notimeout(sw.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.syncok(sw.Handle, It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public void Ctor_RegistersItselfIntoParent()
    {
        var sw = new SubWindow(_parent, IntPtr.MaxValue);

        _parent.SubWindows.ShouldContain(sw);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedRepresentation()
    {
        var sw = new SubWindow(_parent, new(999));
        _cursesMock.MockArea(sw, new Rectangle(5, 6, 100, 200));

        sw.ToString()
          .ShouldBe("SubWindow #000003E7 (100x200 @ 5x6)");
    }

    [TestMethod]
    public void Window_IsInitialized()
    {
        var sw = new SubWindow(_parent, IntPtr.MaxValue);

        sw.Window.ShouldBe(_parent);
        ((ISubWindow) sw).Window.ShouldBe(_parent);
    }

    [TestMethod]
    public void Location_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(22);

        var sw = new SubWindow(_parent, new(1));
        sw.Location.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_1()
    {
        _ = _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var sw = new SubWindow(_parent, new(2));

        Should.Throw<CursesOperationException>(() => sw.Location)
              .Operation.ShouldBe("getparx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_2()
    {
        _ = _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var sw = new SubWindow(_parent, new(2));

        Should.Throw<CursesOperationException>(() => sw.Location)
              .Operation.ShouldBe("getpary");
    }

    [TestMethod]
    public void Location_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(2));
        _cursesMock.MockArea(sw, new Size(1, 1));

        sw.Location = new(11, 22);

        _cursesMock.Verify(v => v.mvderwin(sw.Handle, 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(2));
        _cursesMock.MockArea(sw, new Size(1, 1));

        _ = _cursesMock.Setup(s => s.mvderwin(sw.Handle, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);


        Should.Throw<CursesOperationException>(() => sw.Location = new(1, 1))
              .Operation.ShouldBe("mvderwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(1, 1));

        var sw = new SubWindow(_parent, new(1));
        _cursesMock.MockArea(sw, new Size(1, 1));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => sw.Location = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_UpdatesLocation_IfInsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(2));
        _cursesMock.MockArea(sw, new Size(1, 1));

        sw.Location = new(5, 5);

        _cursesMock.Verify(v => v.mvderwin(new(2), 5, 5), Times.Once);
    }

    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(1))
        {
            Size = new(11, 22)
        };

        _cursesMock.Verify(v => v.wresize(sw.Handle, 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(1));

        _ = _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => sw.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod]
    public void Size_Set_Throws_AdjustedAreaIsEmpty()
    {
        _cursesMock.MockArea(_parent, new Size(10, 10));

        var sw = new SubWindow(_parent, new(10));
        _ = _cursesMock.Setup(s => s.getparx(sw.Handle))
                   .Returns(15);

        _ = _cursesMock.Setup(s => s.getpary(sw.Handle))
                   .Returns(15);

        _ = Should.Throw<ArgumentOutOfRangeException>(() => sw.Size = new(5, 5));
    }

    [TestMethod]
    public void Size_Set_AdjustsSizeToMatchParent()
    {
        _cursesMock.MockArea(_parent, new Size(8, 18));

        var sw = new SubWindow(_parent, new(10));
        _ = _cursesMock.Setup(s => s.getparx(sw.Handle))
                   .Returns(5);

        _ = _cursesMock.Setup(s => s.getpary(sw.Handle))
                   .Returns(6);

        sw.Size = new(10, 10);

        _cursesMock.Verify(v => v.wresize(sw.Handle, 10, 3), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_UpdatesSize_IfInsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sw = new SubWindow(_parent, new(1))
        {
            Size = new(5, 5)
        };

        _cursesMock.Verify(v => v.wresize(sw.Handle, 5, 5), Times.Once);
    }

    [TestMethod]
    public void Origin_Calls_Location()
    {
        var sw = new SubWindow(_parent, new(1));

        _ = _cursesMock.Setup(s => s.getparx(sw.Handle))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getpary(sw.Handle))
                   .Returns(22);

        sw.Origin.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Duplicate_Throws_IfCursesFails()
    {
        var sw = new SubWindow(_parent, new(3));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(IntPtr.Zero);

        Should.Throw<CursesOperationException>(sw.Duplicate)
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod]
    public void Duplicate_ReturnsNewWindow_IfCursesSucceeds()
    {
        var sw = new SubWindow(_parent, new(3));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        var sw1 = sw.Duplicate();

        sw1.Handle.ShouldBe(new(4));
        sw1.Window.ShouldBe(_parent);
        _parent.SubWindows.ShouldContain(sw1);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Duplicate_PreservesManagedCaret(bool mc)
    {
        var sw = new SubWindow(_parent, new(3));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        _ = _cursesMock.Setup(s => s.is_leaveok(sw.Handle))
                   .Returns(mc);

        var sw1 = sw.Duplicate();

        _cursesMock.Verify(s => s.leaveok(sw1.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Destroy_RemovesWindowFromParent()
    {
        var sw = new SubWindow(_parent, new(1));
        sw.Destroy();

        _parent.SubWindows.ShouldBeEmpty();
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var sw = new SubWindow(_parent, new(1));

        sw.Destroy();
        sw.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }
}
