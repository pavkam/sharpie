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
public class TerminalSurfaceTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfTerminalIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new TerminalSurface(null!, new(1)));
    }

    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Verify(v => v.nodelay(sa.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(sa.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(sa.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.notimeout(sa.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.syncok(sa.Handle, It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public void Terminal_IsInitialized()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        sa.Terminal.ShouldBe(_terminal);
        ((ITerminalSurface) sa).Terminal.ShouldBe(_terminal);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh1_Fails_IfCursesFails_NoBatch()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => sa.Refresh())
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh1_Fails_IfCursesFails_InBatch()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.wnoutrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        using (_terminal.AtomicRefresh())
        {
            Should.Throw<CursesOperationException>(() => sa.Refresh())
                  .Operation.ShouldBe("wnoutrefresh");
        }
    }

    [TestMethod]
    public void Refresh1_Succeeds_IfCursesSucceeds_NoBatch()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        sa.Refresh();
        _cursesMock.Verify(v => v.wrefresh(sa.Handle), Times.Once);
    }

    [TestMethod]
    public void Refresh1_Succeeds_IfCursesSucceeds_InBatch()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        using (_terminal.AtomicRefresh())
        {
            sa.Refresh();
        }

        _cursesMock.Verify(v => v.wnoutrefresh(sa.Handle), Times.Once);
    }

    [TestMethod]
    public void Refresh2_Throws_IfYIsNegative()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.getmaxy(sa.Handle))
                   .Returns(10);

        Should.Throw<ArgumentOutOfRangeException>(() => sa.Refresh(-1, 1));
    }

    [TestMethod]
    public void Refresh2_Throws_IfCountIsNegative()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.getmaxy(sa.Handle))
                   .Returns(10);

        Should.Throw<ArgumentOutOfRangeException>(() => sa.Refresh(0, -1));
    }

    [TestMethod]
    public void Refresh2_AdjustsYAndCountToMatchActualHeight()
    {
        var sw = new TerminalSurface(_terminal, new(1));
        _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(10);

        sw.Refresh(4, 10);

        _cursesMock.Verify(v => v.wredrawln(sw.Handle, 4, 6), Times.Once);
    }

    [TestMethod]
    public void Refresh2_DoesNotDoAnythingIfNotInBounds()
    {
        var sw = new TerminalSurface(_terminal, new(1));
        _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(10);

        sw.Refresh(14, 5);
        _cursesMock.Verify(v => v.wredrawln(sw.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFails()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.getmaxy(sa.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.wredrawln(sa.Handle, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => sa.Refresh(1, 1))
              .Operation.ShouldBe("wredrawln");
    }

    [TestMethod]
    public void Refresh2_Throws_IfWithinBatch()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.getmaxy(sa.Handle))
                   .Returns(10);

        using (_terminal.AtomicRefresh())
        {
            Should.Throw<InvalidOperationException>(() => sa.Refresh(1, 1));
        }
    }

    [TestMethod]
    public void Refresh2_Succeeds_IfCursesSucceeds()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.getmaxy(sa.Handle))
                   .Returns(10);

        Should.NotThrow(() => sa.Refresh(1, 9));
    }

    [TestMethod]
    public void ImmediateRefresh_Returns_IfCursesSucceeded()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        _cursesMock.Setup(s => s.is_immedok(It.IsAny<IntPtr>()))
                   .Returns(true);

        sa.ImmediateRefresh.ShouldBeTrue();
    }

    [TestMethod]
    public void ImmediateRefresh_Sets_IfCursesSucceeded()
    {
        var sa = new TerminalSurface(_terminal, new(1));
        sa.ImmediateRefresh = true;

        _cursesMock.Verify(v => v.immedok(sa.Handle, true), Times.Once);
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var sa = new TerminalSurface(_terminal, new(1));

        sa.Destroy();
        sa.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }
}

