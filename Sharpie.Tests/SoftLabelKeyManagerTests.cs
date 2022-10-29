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
public class SoftLabelKeyManagerTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private SoftLabelKeyManager _mgr1 = null!;
    private SoftLabelKeyManager _mgr2 = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _mgr1 = new(_cursesMock.Object, SoftLabelKeyMode.ThreeTwoThree);
        _mgr2 = new(_cursesMock.Object, SoftLabelKeyMode.Disabled);
    }

    [TestMethod, SuppressMessage("ReSharper", "ObjectCreationAsStatement"),
     SuppressMessage("Performance", "CA1806:Do not ignore method results")]
    public void Ctor_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => { new SoftLabelKeyManager(null!, SoftLabelKeyMode.FourFour); });
    }

    [TestMethod, DataRow(SoftLabelKeyMode.FourFour, 8), DataRow(SoftLabelKeyMode.FourFourFour, 12),
     DataRow(SoftLabelKeyMode.ThreeTwoThree, 8), DataRow(SoftLabelKeyMode.FourFourFourWithIndex, 12)]
    public void LabelCount_ReturnsTheCorrectNumberBasedOnMode(SoftLabelKeyMode mode, int expected)
    {
        var mgr = new SoftLabelKeyManager(_cursesMock.Object, mode);
        mgr.LabelCount.ShouldBe(expected);
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void LabelCount_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.LabelCount;
        });
    }

    [TestMethod] public void Enabled_IsTrue_IfModeIsNotDisabled() { _mgr1.Enabled.ShouldBeTrue(); }

    [TestMethod] public void Enabled_IsFalse_IfModeIsDisabled() { _mgr2.Enabled.ShouldBeFalse(); }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void Style_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.Style;
        });

        Should.Throw<NotSupportedException>(() => { _mgr2.Style = Style.Default; });
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void StyleGet_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_attr())
                   .Returns(-1);

        Should.Throw<CursesException>(() =>
              {
                  var l = _mgr1.Style;
              })
              .Operation.ShouldBe("slk_attr");
    }

    [TestMethod]
    public void StyleSet_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_attr_set(It.IsAny<uint>(), It.IsAny<ushort>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Style = Style.Default; })
              .Operation.ShouldBe("slk_attr_set");
    }

    [TestMethod]
    public void StyleGet_ReturnsStyle_FromCurses()
    {
        _cursesMock.Setup(s => s.slk_attr())
                   .Returns(999);

        _cursesMock.Setup(s => s.COLOR_PAIR(It.IsAny<uint>()))
                   .Returns(100);

        var style = _mgr1.Style;
        style.Attributes.ShouldBe((VideoAttribute) 999);
        style.ColorMixture.ShouldBe(new() { Handle = 100 });

        _cursesMock.Verify(v => v.COLOR_PAIR(999));
    }

    [TestMethod]
    public void StyleSet_CallsCurses()
    {
        _mgr1.Style = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 5 } };

        _cursesMock.Verify(v => v.slk_attr_set((uint) VideoAttribute.Bold, 5, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void ColorMixture_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.ColorMixture;
        });

        Should.Throw<NotSupportedException>(() => { _mgr2.ColorMixture = ColorMixture.Default; });
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void ColorMixtureGet_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_attr())
                   .Returns(-1);

        Should.Throw<CursesException>(() =>
              {
                  var l = _mgr1.ColorMixture;
              })
              .Operation.ShouldBe("slk_attr");
    }

    [TestMethod]
    public void ColorMixtureGet_ReturnsStyle_FromCurses()
    {
        _cursesMock.Setup(s => s.slk_attr())
                   .Returns(999);

        _cursesMock.Setup(s => s.COLOR_PAIR(It.IsAny<uint>()))
                   .Returns(100);

        var mix = _mgr1.ColorMixture;
        mix.ShouldBe(new() { Handle = 100 });

        _cursesMock.Verify(v => v.COLOR_PAIR(999));
    }

    [TestMethod]
    public void ColorMixtureSet_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_color(It.IsAny<ushort>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.ColorMixture = ColorMixture.Default; })
              .Operation.ShouldBe("slk_color");
    }

    [TestMethod]
    public void ColorMixtureSet_Succeeds_IfCursesSucceeds()
    {
        _mgr1.ColorMixture = new() { Handle = 5 };

        _cursesMock.Verify(v => v.slk_color(5), Times.Once);
    }

    [TestMethod]
    public void SetLabel_Throws_IfTitleIsNull()
    {
        Should.Throw<ArgumentNullException>(() => { _mgr1.SetLabel(0, null!, SoftLabelKeyAlignment.Center); });
    }

    [TestMethod]
    public void SetLabel_Throws_IfIndexIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _mgr1.SetLabel(-1, "anything", SoftLabelKeyAlignment.Center);
        });

        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _mgr1.SetLabel(_mgr1.LabelCount, "anything", SoftLabelKeyAlignment.Center);
        });
    }

    [TestMethod]
    public void SetLabel_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() => { _mgr2.SetLabel(0, "anything", SoftLabelKeyAlignment.Center); });
    }

    [TestMethod]
    public void SetLabel_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_set(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.SetLabel(0, "test", SoftLabelKeyAlignment.Left); })
              .Operation.ShouldBe("slk_set");
    }

    [TestMethod]
    public void SetLabel_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.slk_set(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                   .Returns(0);

        _mgr1.SetLabel(2, "test", SoftLabelKeyAlignment.Left);

        _cursesMock.Verify(v => v.slk_set(3, "test", (int) SoftLabelKeyAlignment.Left), Times.Once);
    }

    [TestMethod]
    public void EnableAttributes_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() => { _mgr2.EnableAttributes(VideoAttribute.Italic); });
    }

    [TestMethod]
    public void EnableAttributes_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_attr_on(It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.EnableAttributes(VideoAttribute.Italic); })
              .Operation.ShouldBe("slk_attr_on");
    }

    [TestMethod]
    public void EnableAttributes_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.slk_attr_on(It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _mgr1.EnableAttributes(VideoAttribute.Italic);

        _cursesMock.Verify(v => v.slk_attr_on((uint) VideoAttribute.Italic, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void DisableAttributes_Throws_IfNotEnabled()
    {
        Should.Throw<NotSupportedException>(() => { _mgr2.DisableAttributes(VideoAttribute.Italic); });
    }

    [TestMethod]
    public void DisableAttributes_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_attr_off(It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.DisableAttributes(VideoAttribute.Italic); })
              .Operation.ShouldBe("slk_attr_off");
    }

    [TestMethod]
    public void DisableAttributes_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.slk_attr_off(It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _mgr1.DisableAttributes(VideoAttribute.Italic);

        _cursesMock.Verify(v => v.slk_attr_off((uint) VideoAttribute.Italic, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void Clear_Throws_IfNotEnabled() { Should.Throw<NotSupportedException>(() => { _mgr2.Clear(); }); }

    [TestMethod]
    public void Clear_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_clear())
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Clear(); })
              .Operation.ShouldBe("slk_clear");
    }

    [TestMethod]
    public void Clear_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.slk_clear())
                   .Returns(0);

        _mgr1.Clear();

        _cursesMock.Verify(v => v.slk_clear(), Times.Once);
    }

    [TestMethod]
    public void Restore_Throws_IfNotEnabled() { Should.Throw<NotSupportedException>(() => { _mgr2.Restore(); }); }

    [TestMethod]
    public void Restore_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_restore())
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Restore(); })
              .Operation.ShouldBe("slk_restore");
    }

    [TestMethod]
    public void Restore_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.slk_restore())
                   .Returns(0);

        _mgr1.Restore();

        _cursesMock.Verify(v => v.slk_restore(), Times.Once);
    }

    [TestMethod]
    public void Invalidate_Throws_IfNotEnabled() { Should.Throw<NotSupportedException>(() => { _mgr2.Invalidate(); }); }

    [TestMethod]
    public void Invalidate_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.slk_touch())
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Invalidate(); })
              .Operation.ShouldBe("slk_touch");
    }

    [TestMethod]
    public void Invalidate_Succeeds_IfCursesSucceeds()
    {
        _mgr1.Invalidate();

        _cursesMock.Verify(v => v.slk_touch(), Times.Once);
    }

    [TestMethod]
    public void Refresh_Throws_IfNotEnabled() { Should.Throw<NotSupportedException>(() => { _mgr2.Refresh(false); }); }

    [TestMethod]
    public void Refresh_Throws_IfCursesFails_ForQueue()
    {
        _cursesMock.Setup(s => s.slk_noutrefresh())
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Refresh(true); })
              .Operation.ShouldBe("slk_noutrefresh");
    }

    [TestMethod]
    public void Refresh_Throws_IfCursesFails_ForBatch()
    {
        _cursesMock.Setup(s => s.slk_refresh())
                   .Returns(-1);

        Should.Throw<CursesException>(() => { _mgr1.Refresh(false); })
              .Operation.ShouldBe("slk_refresh");
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_ForBatch()
    {
        _mgr1.Refresh(true);

        _cursesMock.Verify(v => v.slk_noutrefresh(), Times.Once);
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds()
    {
        _mgr1.Refresh(false);

        _cursesMock.Verify(v => v.slk_refresh(), Times.Once);
    }
}
