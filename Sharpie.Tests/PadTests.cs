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
    private Pad _pad1 = null!;
    private Screen _screen = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns((IntPtr _, int y, int x) => y is >= 0 and < 5 && x is >= 0 and < 5);

        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns((IntPtr _, int y, int x) => y is >= 0 and < 10 && x is >= 0 and < 10);

        _screen = new(_cursesMock.Object, new(1));
        _pad1 = new(_cursesMock.Object, _screen, new(2));
    }

    [TestMethod]
    public void Ctor_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new Pad(null!, _screen, IntPtr.MaxValue));
    }

    [TestMethod]
    public void Ctor_Throws_IfWindowIsNull()
    {
        Should.Throw<ArgumentException>(() => new Pad(_cursesMock.Object, null!, IntPtr.MaxValue));
    }

    [TestMethod]
    public void Ctor_Throws_IfHandleIsZero()
    {
        Should.Throw<ArgumentException>(() => new Pad(_cursesMock.Object, _screen, IntPtr.Zero));
    }

    [TestMethod]
    public void Ctor_Throws_IfParentNotPadOrScreen()
    {
        Should.Throw<ArgumentException>(() =>
            new Pad(_cursesMock.Object, new(_cursesMock.Object, null, IntPtr.MaxValue), IntPtr.Zero));
    }

    [TestMethod]
    public void Screen_ProperlyContainsTheScreen()
    {
        _pad1.Screen.ShouldBe(_screen);

        var pad2 = new Pad(_cursesMock.Object, _pad1, IntPtr.MaxValue);
        pad2.Screen.ShouldBe(_screen);
    }

    [TestMethod] public void ImmediateRefresh_AlwaysReturnsFalse() { _pad1.ImmediateRefresh.ShouldBe(false); }

    [TestMethod]
    public void ImmediateRefresh_Throws_WhenSet()
    {
        Should.Throw<NotSupportedException>(() => _pad1.ImmediateRefresh = true);
    }

    [TestMethod]
    public void Refresh_Throws_Always() { Should.Throw<NotSupportedException>(() => _pad1.Refresh(false, true)); }

    [TestMethod]
    public void Refresh2_Throws_IfTheRectIsOutsideTheBounds()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { _pad1.Refresh(false, false, new(1, 1, 5, 5), new(0, 0)); });
    }

    [TestMethod]
    public void Refresh2_Throws_IfThePositionIsOutsideTheBounds()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { _pad1.Refresh(false, false, new(0, 0, 5, 5), new(6, 6)); });
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Refresh2_SetsEntireScreenRefresh(bool entireScreen)
    {
        _pad1.Refresh(false, entireScreen, new(0, 0, 1, 1), new(0, 0));

        _cursesMock.Verify(v => v.clearok(_pad1.Handle, entireScreen), Times.Once);
    }

    [TestMethod]
    public void Refresh2_QueuesRefresh()
    {
        _pad1.Refresh(true, false, new(0, 1, 2, 3), new(2, 3));

        _cursesMock.Verify(v => v.pnoutrefresh(_pad1.Handle, 1, 0, 3, 2,
            6, 4), Times.Once);
    }

    [TestMethod]
    public void Refresh2_RefreshesDirectly()
    {
        _pad1.Refresh(false, false, new(0, 1, 2, 3), new(2, 3));

        _cursesMock.Verify(v => v.prefresh(_pad1.Handle, 1, 0, 3, 2,
            6, 4), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFailsAtSettingEntireScreenRefresh()
    {
        _cursesMock.Setup(s => s.clearok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _pad1.Refresh(false, false, new(0, 0, 1, 1), new(0, 0)); })
              .Operation.ShouldBe("clearok");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFailsAtQueueingRefresh()
    {
        _cursesMock.Setup(s => s.pnoutrefresh(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _pad1.Refresh(true, false, new(0, 0, 1, 1), new(0, 0)); })
              .Operation.ShouldBe("pnoutrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFailsAtRefreshingDirectly()
    {
        _cursesMock.Setup(s => s.prefresh(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _pad1.Refresh(false, false, new(0, 0, 1, 1), new(0, 0)); })
              .Operation.ShouldBe("prefresh");
    }

    [TestMethod] public void Location_Get_Throws_Always() { Should.Throw<NotSupportedException>(() => _pad1.Location); }

    [TestMethod]
    public void Location_Set_Throws_Always()
    {
        Should.Throw<NotSupportedException>(() => _pad1.Location = Point.Empty);
    }
}
