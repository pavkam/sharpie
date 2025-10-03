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
public class SoftLabelKeyManagerTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private SoftLabelKeyManager _mgr1 = null!;
    private SoftLabelKeyManager _mgr2 = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new());

        _mgr1 = new(_terminal, SoftLabelKeyMode.ThreeTwoThree);
        _mgr2 = new(_terminal, SoftLabelKeyMode.Disabled);
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    [TestMethod,
     SuppressMessage("ReSharper", "ObjectCreationAsStatement"),
     SuppressMessage("Performance", "CA1806:Do not ignore method results")]
    public void Ctor_Throws_IfCursesIsNull() =>
        Should.Throw<ArgumentNullException>(() => { _ = new SoftLabelKeyManager(null!, SoftLabelKeyMode.FourFour); });

    [TestMethod]
    public void Terminal_IsInitialized()
    {
        _mgr1.Terminal.ShouldBe(_terminal);
        ((ISoftLabelKeyManager) _mgr1).Terminal.ShouldBe(_terminal);
    }

    [TestMethod, DataRow(SoftLabelKeyMode.FourFour, 8), DataRow(SoftLabelKeyMode.FourFourFour, 12),
     DataRow(SoftLabelKeyMode.ThreeTwoThree, 8), DataRow(SoftLabelKeyMode.FourFourFourWithIndex, 12)]
    public void LabelCount_ReturnsTheCorrectNumberBasedOnMode(SoftLabelKeyMode mode, int expected)
    {
        var mgr = new SoftLabelKeyManager(_terminal, mode);
        mgr.LabelCount.ShouldBe(expected);
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void LabelCount_Throws_IfNotEnabled()
    {
        _ = Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.LabelCount;
        });
    }

    [TestMethod]
    public void Enabled_IsTrue_IfModeIsNotDisabled() => _mgr1.Enabled.ShouldBeTrue();

    [TestMethod]
    public void Enabled_IsFalse_IfModeIsDisabled() => _mgr2.Enabled.ShouldBeFalse();

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void Style_Throws_IfNotEnabled()
    {
        _ = Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.Style;
        });

        _ = Should.Throw<NotSupportedException>(() => { _mgr2.Style = Style.Default; });
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void StyleGet_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_attr(out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() =>
              {
                  var l = _mgr1.Style;
              })
              .Operation.ShouldBe("slk_attr");
    }

    [TestMethod]
    public void StyleSet_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_attr_set(It.IsAny<VideoAttribute>(), It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.Style = Style.Default; })
              .Operation.ShouldBe("slk_attr_set");
    }

    [TestMethod]
    public void StyleGet_ReturnsStyle_FromCurses()
    {
        _ = _cursesMock.Setup(s => s.slk_attr(out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny))
                   .Returns((out VideoAttribute va, out short cp) =>
                   {
                       va = VideoAttribute.Italic;
                       cp = 15;

                       return 0;
                   });

        var style = _mgr1.Style;

        style.Attributes.ShouldBe(VideoAttribute.Italic);
        style.ColorMixture.ShouldBe(new()
        {
            Handle = 15
        });
    }

    [TestMethod]
    public void StyleSet_CallsCurses()
    {
        _mgr1.Style = new()
        {
            Attributes = VideoAttribute.Bold,
            ColorMixture = new()
            {
                Handle = 5
            }
        };

        _cursesMock.Verify(v => v.slk_attr_set(VideoAttribute.Bold, 5, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void ColorMixture_Throws_IfNotEnabled()
    {
        _ = Should.Throw<NotSupportedException>(() =>
        {
            var l = _mgr2.ColorMixture;
        });

        _ = Should.Throw<NotSupportedException>(() => { _mgr2.ColorMixture = ColorMixture.Default; });
    }

    [TestMethod, SuppressMessage("ReSharper", "UnusedVariable")]
    public void ColorMixtureGet_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_attr(out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() =>
              {
                  var l = _mgr1.ColorMixture;
              })
              .Operation.ShouldBe("slk_attr");
    }

    [TestMethod]
    public void ColorMixtureGet_ReturnsStyle_FromCurses()
    {
        _ = _cursesMock.Setup(s => s.slk_attr(out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny))
                   .Returns((out VideoAttribute va, out short cp) =>
                   {
                       va = VideoAttribute.Italic;
                       cp = 15;

                       return 0;
                   });

        var mix = _mgr1.ColorMixture;
        mix.ShouldBe(new()
        {
            Handle = 15
        });
    }

    [TestMethod]
    public void ColorMixtureSet_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_color(It.IsAny<short>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.ColorMixture = ColorMixture.Default; })
              .Operation.ShouldBe("slk_color");
    }

    [TestMethod]
    public void ColorMixtureSet_Succeeds_IfCursesSucceeds()
    {
        _mgr1.ColorMixture = new()
        {
            Handle = 5
        };

        _cursesMock.Verify(v => v.slk_color(5), Times.Once);
    }

    [TestMethod]
    public void SetLabel_Throws_IfTitleIsNull() =>
        Should.Throw<ArgumentNullException>(() => { _mgr1.SetLabel(0, null!, SoftLabelKeyAlignment.Center); });

    [TestMethod]
    public void SetLabel_Throws_IfIndexIsInvalid()
    {
        _ = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _mgr1.SetLabel(-1, "anything", SoftLabelKeyAlignment.Center);
        });

        _ = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _mgr1.SetLabel(_mgr1.LabelCount, "anything", SoftLabelKeyAlignment.Center);
        });
    }

    [TestMethod]
    public void SetLabel_Throws_IfNotEnabled() =>
        Should.Throw<NotSupportedException>(() => { _mgr2.SetLabel(0, "anything", SoftLabelKeyAlignment.Center); });

    [TestMethod]
    public void SetLabel_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_set(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.SetLabel(0, "test", SoftLabelKeyAlignment.Left); })
              .Operation.ShouldBe("slk_set");
    }

    [TestMethod]
    public void SetLabel_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.slk_set(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                   .Returns(0);

        _mgr1.SetLabel(2, "test", SoftLabelKeyAlignment.Left);

        _cursesMock.Verify(v => v.slk_set(3, "test", (int) SoftLabelKeyAlignment.Left), Times.Once);
    }

    [TestMethod]
    public void EnableAttributes_Throws_IfNotEnabled() => Should.Throw<NotSupportedException>(() => { _mgr2.EnableAttributes(VideoAttribute.Italic); });

    [TestMethod]
    public void EnableAttributes_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_attr_on(It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.EnableAttributes(VideoAttribute.Italic); })
              .Operation.ShouldBe("slk_attr_on");
    }

    [TestMethod]
    public void EnableAttributes_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.slk_attr_on(It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _mgr1.EnableAttributes(VideoAttribute.Italic);

        _cursesMock.Verify(v => v.slk_attr_on(VideoAttribute.Italic, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void DisableAttributes_Throws_IfNotEnabled() =>
        Should.Throw<NotSupportedException>(() => { _mgr2.DisableAttributes(VideoAttribute.Italic); });

    [TestMethod]
    public void DisableAttributes_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_attr_off(It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.DisableAttributes(VideoAttribute.Italic); })
              .Operation.ShouldBe("slk_attr_off");
    }

    [TestMethod]
    public void DisableAttributes_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.slk_attr_off(It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(0);

        _mgr1.DisableAttributes(VideoAttribute.Italic);

        _cursesMock.Verify(v => v.slk_attr_off(VideoAttribute.Italic, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void Clear_Throws_IfNotEnabled() => Should.Throw<NotSupportedException>(() => { _mgr2.Clear(); });

    [TestMethod]
    public void Clear_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_clear())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.Clear(); })
              .Operation.ShouldBe("slk_clear");
    }

    [TestMethod]
    public void Clear_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.slk_clear())
                   .Returns(0);

        _mgr1.Clear();

        _cursesMock.Verify(v => v.slk_clear(), Times.Once);
    }

    [TestMethod]
    public void Restore_Throws_IfNotEnabled() => Should.Throw<NotSupportedException>(() => { _mgr2.Restore(); });

    [TestMethod]
    public void Restore_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_restore())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.Restore(); })
              .Operation.ShouldBe("slk_restore");
    }

    [TestMethod]
    public void Restore_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.slk_restore())
                   .Returns(0);

        _mgr1.Restore();

        _cursesMock.Verify(v => v.slk_restore(), Times.Once);
    }

    [TestMethod]
    public void MarkDirty_Throws_IfNotEnabled() => Should.Throw<NotSupportedException>(() => { _mgr2.MarkDirty(); });

    [TestMethod]
    public void MarkDirty_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.slk_touch())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.MarkDirty(); })
              .Operation.ShouldBe("slk_touch");
    }

    [TestMethod]
    public void MarkDirty_Succeeds_IfCursesSucceeds()
    {
        _mgr1.MarkDirty();

        _cursesMock.Verify(v => v.slk_touch(), Times.Once);
    }

    [TestMethod]
    public void Refresh_Throws_IfNotEnabled() =>
        Should.Throw<NotSupportedException>(() => { _mgr2.Refresh(); });

    [TestMethod]
    public void Refresh_Throws_IfCursesFails_NoQueue()
    {
        _ = _cursesMock.Setup(s => s.slk_refresh())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => { _mgr1.Refresh(); })
              .Operation.ShouldBe("slk_refresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_InBatch()
    {
        _ = _cursesMock.Setup(s => s.slk_noutrefresh())
                   .Returns(-1);

        using (_terminal.AtomicRefresh())
        {
            Should.Throw<CursesOperationException>(() => { _mgr1.Refresh(); })
                  .Operation.ShouldBe("slk_noutrefresh");
        }
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_InBatch()
    {
        using (_terminal.AtomicRefresh())
        {
            _mgr1.Refresh();
        }

        _cursesMock.Verify(v => v.slk_noutrefresh(), Times.Once);
    }

    [TestMethod]
    public void Refresh_Succeeds_IfCursesSucceeds_NoBatch()
    {
        _mgr1.Refresh();
        _cursesMock.Verify(v => v.slk_refresh(), Times.Once);
    }
}
