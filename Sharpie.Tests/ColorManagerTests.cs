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

using Curses;

[TestClass]
public class ColorManagerTests
{
    private Mock<ICursesProvider> _cursesProviderMock = null!;
    private ColorManager _colorManager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesProviderMock = new();
        _cursesProviderMock.Setup(s => s.has_colors())
                           .Returns(true);

        _cursesProviderMock.Setup(s => s.can_change_color())
                           .Returns(true);

        _colorManager = new(_cursesProviderMock.Object, true);
    }

    [TestMethod, SuppressMessage("ReSharper", "ObjectCreationAsStatement"),
     SuppressMessage("Performance", "CA1806:Do not ignore method results")]
    public void Ctor_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => { new ColorManager(null!, true); });
    }

    [TestMethod]
    public void Ctor_EnablesColors_WhenSupported()
    {
        _cursesProviderMock.Setup(s => s.has_colors())
                           .Returns(true);

        var mgr = new ColorManager(_cursesProviderMock.Object, true);
        mgr.Enabled.ShouldBeTrue();
    }

    [TestMethod]
    public void Ctor_DoesNotEnableColors_WhenSupported()
    {
        var mgr = new ColorManager(_cursesProviderMock.Object, false);
        mgr.Enabled.ShouldBeFalse();
    }

    [TestMethod]
    public void Ctor_DoesNotEnableColors_WhenNotSupported()
    {
        _cursesProviderMock.Setup(s => s.has_colors())
                           .Returns(false);

        var mgr = new ColorManager(_cursesProviderMock.Object, true);
        mgr.Enabled.ShouldBeFalse();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void ColorsAreSupported_ReturnsCursesResponse(bool value)
    {
        _cursesProviderMock.Setup(s => s.has_colors())
                           .Returns(value);

        _colorManager.ColorsAreSupported.ShouldBe(value);
        _cursesProviderMock.Verify(v => v.has_colors(), Times.Exactly(2));
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void CanRedefineColors_ReturnsCursesResponse(bool value)
    {
        _cursesProviderMock.Setup(s => s.can_change_color())
                           .Returns(value);

        _colorManager.CanRedefineColors.ShouldBe(value);
        _cursesProviderMock.Verify(v => v.can_change_color(), Times.Once);
    }

    [TestMethod]
    public void MixColors_ReturnsColorMixtureAndIncrementsTheHandle_IfCursesSucceeds()
    {
        _cursesProviderMock.Setup(s => s.init_pair(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
                           .Returns(0);

        _colorManager.MixColors(2, 3)
                     .ShouldBe(new() { Handle = 1 });

        _colorManager.MixColors(4, 5)
                     .ShouldBe(new() { Handle = 2 });

        _cursesProviderMock.Verify(v => v.init_pair(1, 2, 3), Times.Once);
        _cursesProviderMock.Verify(v => v.init_pair(2, 4, 5), Times.Once);
    }

    [TestMethod]
    public void MixColors_ThrowsAndDoesNotIncrementHandle_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.init_pair(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.MixColors(2, 3); })
              .Operation.ShouldBe("init_pair");

        Should.Throw<CursesException>(() => { _colorManager.MixColors(4, 5); })
              .Operation.ShouldBe("init_pair");

        _cursesProviderMock.Verify(v => v.init_pair(1, 2, 3), Times.Once);
        _cursesProviderMock.Verify(v => v.init_pair(1, 4, 5), Times.Once);
    }

    [TestMethod]
    public void MixColors2_CallsCursesAsWell()
    {
        _colorManager.MixColors(StandardColor.Black, StandardColor.Cyan)
                     .ShouldBe(new() { Handle = 1 });

        _cursesProviderMock.Verify(v => v.init_pair(1, (ushort) StandardColor.Black, (ushort) StandardColor.Cyan),
            Times.Once);
    }

    [TestMethod]
    public void RemixColors_FinishesOK_IfCursesSucceeds()
    {
        _cursesProviderMock.Setup(s => s.init_pair(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
                           .Returns(0);

        _colorManager.RemixColors(new() { Handle = 1 }, 2, 3);
        _cursesProviderMock.Verify(v => v.init_pair(1, 2, 3), Times.Once);
    }

    [TestMethod]
    public void RemixColors_ThrowsAndDoesNotIncrementHandle_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.init_pair(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.RemixColors(new() { Handle = 1 }, 2, 3); })
              .Operation.ShouldBe("init_pair");

        _cursesProviderMock.Verify(v => v.init_pair(1, 2, 3), Times.Once);
    }

    [TestMethod]
    public void RemixColors2_CallsCursesAsWell()
    {
        _colorManager.RemixColors(new() { Handle = 1 }, StandardColor.Black, StandardColor.Cyan);
        _cursesProviderMock.Verify(v => v.init_pair(1, (ushort) StandardColor.Black, (ushort) StandardColor.Cyan),
            Times.Once);
    }

    [TestMethod]
    public void RemixDefaultColors_FinishesOK_IfCursesSucceeds()
    {
        _cursesProviderMock.Setup(s => s.assume_default_colors(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(0);

        _colorManager.RemixDefaultColors(2, 3);
        _cursesProviderMock.Verify(v => v.assume_default_colors(2, 3), Times.Once);
    }

    [TestMethod]
    public void RemixDefaultColors_ThrowsAndDoesNotIncrementHandle_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.assume_default_colors(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.RemixDefaultColors(2, 3); })
              .Operation.ShouldBe("assume_default_colors");

        _cursesProviderMock.Verify(v => v.assume_default_colors(2, 3), Times.Once);
    }

    [TestMethod]
    public void RemixDefaultColors2_CallsCursesAsWell()
    {
        _colorManager.RemixDefaultColors(StandardColor.Black, StandardColor.Cyan);
        _cursesProviderMock.Verify(v => v.assume_default_colors((int) StandardColor.Black, (int) StandardColor.Cyan),
            Times.Once);
    }

    [TestMethod]
    public void RedefineColor_Throws_IfRedefineColorsNotSupported()
    {
        _cursesProviderMock.Setup(s => s.can_change_color())
                           .Returns(false);

        Should.Throw<NotSupportedException>(() => { _colorManager.RedefineColor(1, 1, 1, 1); });

        _cursesProviderMock.Verify(v => v.can_change_color(), Times.Once);
    }

    [TestMethod]
    public void RedefineColor_Throws_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.init_color(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ushort>(),
                               It.IsAny<ushort>()))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.RedefineColor(1, 2, 3, 4); })
              .Operation.ShouldBe("init_color");

        _cursesProviderMock.Verify(v => v.init_color(1, 2, 3, 4), Times.Once);
    }

    [TestMethod]
    public void RedefineColor_Succeeds_IfCursesSucceeds()
    {
        _colorManager.RedefineColor(1, 2, 3, 4);
        _cursesProviderMock.Verify(v => v.init_color(1, 2, 3, 4), Times.Once);
    }

    [TestMethod]
    public void RedefineColor_UsesMaximumOf1000_IfColorsExceedThat()
    {
        _colorManager.RedefineColor(1, 1001, 2000, 30000);

        _cursesProviderMock.Verify(v => v.init_color(1, 1000, 1000, 1000), Times.Once);
    }

    [TestMethod]
    public void RedefineColor2_AlsoCallsCurses()
    {
        _colorManager.RedefineColor(StandardColor.Magenta, 2, 3, 4);
        _cursesProviderMock.Verify(v => v.init_color((ushort) StandardColor.Magenta, 2, 3, 4), Times.Once);
    }

    [TestMethod]
    public void BreakdownColor_Throws_IfRedefineColorsNotSupported()
    {
        _cursesProviderMock.Setup(s => s.can_change_color())
                           .Returns(false);

        Should.Throw<NotSupportedException>(() => { _colorManager.BreakdownColor(1); });

        _cursesProviderMock.Verify(v => v.can_change_color(), Times.Once);
    }

    private delegate int ColorContentCallback(ushort color, out ushort red, out ushort green, out ushort blue);

    [TestMethod]
    public void BreakdownColor_Throws_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.color_content(It.IsAny<ushort>(), out It.Ref<ushort>.IsAny,
                               out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.BreakdownColor(1); })
              .Operation.ShouldBe("color_content");

        _cursesProviderMock.Verify(
            v => v.color_content(1, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny),
            Times.Once);
    }

    [TestMethod]
    public void BreakdownColor_ReturnsColors_IfCursesSucceeds()
    {
        _cursesProviderMock.Setup(s => s.color_content(It.IsAny<ushort>(), out It.Ref<ushort>.IsAny,
                               out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny))
                           .Returns(new ColorContentCallback((ushort _, out ushort r, out ushort g, out ushort b) =>
                           {
                               r = 10;
                               g = 20;
                               b = 30;
                               return 0;
                           }));

        var (red, green, blue) = _colorManager.BreakdownColor(1);
        red.ShouldBe((ushort) 10);
        green.ShouldBe((ushort) 20);
        blue.ShouldBe((ushort) 30);

        _cursesProviderMock.Verify(
            v => v.color_content(1, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny),
            Times.Once);
    }

    [TestMethod]
    public void BreakdownColor2_AlsoCallsCurses()
    {
        _cursesProviderMock.Setup(s => s.color_content(It.IsAny<ushort>(), out It.Ref<ushort>.IsAny,
                               out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny))
                           .Returns(0);

        _colorManager.BreakdownColor(StandardColor.Blue);
        _cursesProviderMock.Verify(
            v => v.color_content((ushort)StandardColor.Blue, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny),
            Times.Once);
    }

    private delegate int PairContentCallback(ushort pair, out ushort fgColor, out ushort bgColor);

    [TestMethod]
    public void UnMix_Throws_IfCursesFails()
    {
        _cursesProviderMock.Setup(s => s.pair_content(It.IsAny<ushort>(), out It.Ref<ushort>.IsAny,
                               out It.Ref<ushort>.IsAny))
                           .Returns(-1);

        Should.Throw<CursesException>(() => { _colorManager.UnMix(new() { Handle = 1 }); })
              .Operation.ShouldBe("pair_content");

        _cursesProviderMock.Verify(v => v.pair_content(1, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny),
            Times.Once);
    }

    [TestMethod]
    public void UnMix_ReturnsColors_IfCursesSucceeds()
    {
        _cursesProviderMock.Setup(s => s.pair_content(It.IsAny<ushort>(), out It.Ref<ushort>.IsAny,
                               out It.Ref<ushort>.IsAny))
                           .Returns(new PairContentCallback((ushort _, out ushort fg, out ushort bg) =>
                           {
                               fg = 20;
                               bg = 30;
                               return 0;
                           }));

        var (fgColor, bgColor) = _colorManager.UnMix(new() { Handle = 1 });
        fgColor.ShouldBe((ushort) 20);
        bgColor.ShouldBe((ushort) 30);

        _cursesProviderMock.Verify(v => v.pair_content(1, out It.Ref<ushort>.IsAny, out It.Ref<ushort>.IsAny),
            Times.Once);
    }
}
