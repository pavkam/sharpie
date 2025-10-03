/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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
public class SubPadTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private Pad _parent = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
        _parent = new(_terminal.Screen, new(1));
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    [TestMethod]
    public void Ctor_Throws_IfScreenIsNull() => Should.Throw<ArgumentException>(() => new SubPad(null!, IntPtr.MaxValue));

    [TestMethod]
    public void Pad_IsInitialized()
    {
        var sp = new SubPad(_parent, IntPtr.MaxValue);

        sp.Pad.ShouldBe(_parent);
        ((ISubPad) sp).Pad.ShouldBe(_parent);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedRepresentation()
    {
        var sp = new SubPad(_parent, new(999));
        _cursesMock.MockArea(sp, new Rectangle(5, 6, 100, 200));

        sp.ToString()
          .ShouldBe("SubPad #000003E7 (100x200 @ 5x6)");
    }

    [TestMethod]
    public void Location_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(22);

        var sp = new SubPad(_parent, new(1));
        sp.Location.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_1()
    {
        _ = _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var sp = new SubPad(_parent, new(2));

        Should.Throw<CursesOperationException>(() => sp.Location)
              .Operation.ShouldBe("getparx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_Throws_IfCursesFails_2()
    {
        _ = _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var sp = new SubPad(_parent, new(2));

        Should.Throw<CursesOperationException>(() => sp.Location)
              .Operation.ShouldBe("getpary");
    }

    [TestMethod]
    public void Location_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sp = new SubPad(_parent, new(2));
        _cursesMock.MockArea(sp, new Size(1, 1));

        sp.Location = new(11, 22);

        _cursesMock.Verify(v => v.mvderwin(sp.Handle, 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sp = new SubPad(_parent, new(2));
        _cursesMock.MockArea(sp, new Size(1, 1));

        _ = _cursesMock.Setup(s => s.mvderwin(sp.Handle, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);


        Should.Throw<CursesOperationException>(() => sp.Location = new(1, 1))
              .Operation.ShouldBe("mvderwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfOutsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(1, 1));

        var sp = new SubPad(_parent, new(1));
        _cursesMock.MockArea(sp, new Size(1, 1));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => sp.Location = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_UpdatesLocation_IfInsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var sp = new SubPad(_parent, new(2));
        _cursesMock.MockArea(sp, new Size(1, 1));

        sp.Location = new(5, 5);

        _cursesMock.Verify(v => v.mvderwin(new(2), 5, 5), Times.Once);
    }

    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var w = new SubPad(_parent, new(1))
        {
            Size = new(11, 22)
        };

        _cursesMock.Verify(v => v.wresize(w.Handle, 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var w = new SubPad(_parent, new(1));

        _ = _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod]
    public void Size_Set_Throws_AdjustedAreaIsEmpty()
    {
        _cursesMock.MockArea(_parent, new Size(10, 10));

        var sp = new SubPad(_parent, new(10));
        _ = _cursesMock.Setup(s => s.getparx(sp.Handle))
                   .Returns(15);

        _ = _cursesMock.Setup(s => s.getpary(sp.Handle))
                   .Returns(15);

        _ = Should.Throw<ArgumentOutOfRangeException>(() => sp.Size = new(5, 5));
    }

    [TestMethod]
    public void Size_Set_AdjustsSizeToMatchParent()
    {
        _cursesMock.MockArea(_parent, new Size(8, 18));

        var sp = new SubPad(_parent, new(10));
        _ = _cursesMock.Setup(s => s.getparx(sp.Handle))
                   .Returns(5);

        _ = _cursesMock.Setup(s => s.getpary(sp.Handle))
                   .Returns(6);

        sp.Size = new(10, 10);

        _cursesMock.Verify(v => v.wresize(sp.Handle, 10, 3), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_UpdatesSize_IfInsideParent()
    {
        _cursesMock.MockArea(_parent, new Size(100, 100));

        var w = new SubPad(_parent, new(1))
        {
            Size = new(5, 5)
        };

        _cursesMock.Verify(v => v.wresize(w.Handle, 5, 5), Times.Once);
    }

    [TestMethod]
    public void Origin_Calls_Location()
    {
        var sp = new SubPad(_parent, new(1));

        _ = _cursesMock.Setup(s => s.getparx(sp.Handle))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getpary(sp.Handle))
                   .Returns(22);

        sp.Origin.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Duplicate_Throws_IfCursesFails()
    {
        var sp = new SubPad(_parent, new(2));

        Should.Throw<CursesOperationException>(() => sp.Duplicate())
              .Operation.ShouldBe("dupwin");
    }

    [TestMethod]
    public void Duplicate_ReturnsNewPad_IfCursesSucceeds()
    {
        var sp = new SubPad(_parent, new(2));

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(3));

        var sp1 = sp.Duplicate();

        sp1.Pad.ShouldBe(_parent);
        sp1.Handle.ShouldBe(new(3));
        _parent.SubPads.ShouldContain(sp1);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Duplicate_PreservesManagedCaret(bool mc)
    {
        var sp = new SubPad(_parent, new(3)) { ManagedCaret = mc };

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(4));

        _ = _cursesMock.Setup(s => s.is_leaveok(sp.Handle))
                   .Returns(mc);

        var sp1 = sp.Duplicate();

        _cursesMock.Verify(s => s.leaveok(sp1.Handle, mc), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Destroy_RemovesWindowFromParent()
    {
        var sp = new SubPad(_parent, new(1));
        sp.Destroy();

        _parent.SubPads.ShouldBeEmpty();
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var sp = new SubPad(_parent, new(1));

        sp.Destroy();
        sp.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }
}
