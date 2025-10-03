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
public class SurfaceTests
{
    private Mock<ICursesBackend> _cursesMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));
    }

    [TestMethod]
    public void Ctor_Throws_WhenCursesIfNull() =>
        Should.Throw<ArgumentNullException>(() => new Surface(null!, IntPtr.MaxValue));

    [TestMethod]
    public void Ctor_Throws_WhenHandleIsZero() =>
        Should.Throw<ArgumentException>(() => new Surface(_cursesMock.Object, IntPtr.Zero));

    [TestMethod]
    public void Ctor_ConfiguresSurface_InCurses()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        _cursesMock.Verify(v => v.nodelay(s.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.scrollok(s.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.keypad(s.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.notimeout(s.Handle, It.IsAny<bool>()), Times.Never);
        _cursesMock.Verify(v => v.syncok(s.Handle, It.IsAny<bool>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureSurface_FailsInCurses_1()
    {
        _ = _cursesMock.Setup(s => s.nodelay(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Surface(_cursesMock.Object, new(1)))
              .Operation.ShouldBe("nodelay");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureSurface_FailsInCurses_2()
    {
        _ = _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Surface(_cursesMock.Object, new(1)))
              .Operation.ShouldBe("scrollok");
    }

    [TestMethod]
    public void Curses_IsInitialized()
    {
        var s = new Surface(_cursesMock.Object, IntPtr.MaxValue);
        s.Curses.ShouldBe(_cursesMock.Object);
    }

    [TestMethod]
    public void Handle_IsInitialized()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Handle.ShouldBe(new(22));
    }

    [TestMethod]
    public void Handle_Throws_IfDisposed()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Dispose();

        _ = Should.Throw<ObjectDisposedException>(() => s.Handle);
    }

    [TestMethod]
    public void AssertAlive_DoesNothing_IfNotDisposed()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        Should.NotThrow(s.AssertAlive);
    }

    [TestMethod]
    public void AssertAlive_Throws_IfDisposed()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Dispose();

        _ = Should.Throw<ObjectDisposedException>(s.AssertAlive);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedRepresentation()
    {
        var s = new Surface(_cursesMock.Object, new(999));
        _cursesMock.MockArea(s, new Size(100, 200));

        s.ToString()
         .ShouldBe("Surface #000003E7 (100x200)");
    }

    [TestMethod]
    public void Scrollable_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var s = new Surface(_cursesMock.Object, new(1));
        s.Scrollable.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Scrollable_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        var s = new Surface(_cursesMock.Object, new(1))
        {
            Scrollable = enabled
        };

        _cursesMock.Verify(v => v.scrollok(new(1), enabled), Times.AtMost(2));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Scrollable_Throws_IfCursesFails()
    {
        var sw = new Surface(_cursesMock.Object, new(1));

        _ = _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => sw.Scrollable = false)
              .Operation.ShouldBe("scrollok");
    }

    [TestMethod]
    public void Disposed_ReturnsFalse_IfNotDisposed()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Disposed.ShouldBeFalse();
    }

    [TestMethod]
    public void Disposed_ReturnsTrue_IfDisposed()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Dispose();
        s.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Style_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s =>
            s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero)
        )
        .Returns((IntPtr _, out VideoAttribute a, out short p, IntPtr _) =>
        {
            a = VideoAttribute.Italic;
            p = 22;

            return 0;
        });


        var sw = new Surface(_cursesMock.Object, new(1));
        var s = sw.Style;

        s.Attributes.ShouldBe(VideoAttribute.Italic);
        s.ColorMixture.ShouldBe(new()
        {
            Handle = 22
        });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Get_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s =>
            s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero)
        )
        .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.Style)
              .Operation.ShouldBe("wattr_get");
    }

    [TestMethod]
    public void Style_Set_SetsValue_IfCursesSucceeded()
    {
        var s = new Surface(_cursesMock.Object, new(1))
        {
            Style = new()
            {
                Attributes = VideoAttribute.Italic,
                ColorMixture = new()
                {
                    Handle = 22
                }
            }
        };

        _cursesMock.Verify(v => v.wattr_set(It.IsAny<IntPtr>(), VideoAttribute.Italic, 22, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Set_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s =>
            s.wattr_set(It.IsAny<IntPtr>(), It.IsAny<VideoAttribute>(), It.IsAny<short>(), IntPtr.Zero)
        )
        .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.Style = Style.Default)
              .Operation.ShouldBe("wattr_set");
    }

    [TestMethod]
    public void ColorMixture_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s =>
            s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero)
        )
        .Returns((IntPtr _, out VideoAttribute a, out short p, IntPtr _) =>
        {
            a = 0;
            p = 22;

            return 0;
        });

        var s = new Surface(_cursesMock.Object, new(1));
        s.ColorMixture.ShouldBe(new()
        {
            Handle = 22
        });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ColorMixture_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s =>
            s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero)
        )
        .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.ColorMixture)
              .Operation.ShouldBe("wattr_get");
    }

    [TestMethod]
    public void ColorMixture_Set_SetsValue_IfCursesSucceeded()
    {
        var s = new Surface(_cursesMock.Object, new(1))
        {
            ColorMixture = new()
            {
                Handle = 22
            }
        };

        _cursesMock.Verify(v => v.wcolor_set(It.IsAny<IntPtr>(), 22, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ColorMixture_Set_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wcolor_set(It.IsAny<IntPtr>(), It.IsAny<short>(), IntPtr.Zero))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.ColorMixture = ColorMixture.Default)
              .Operation.ShouldBe("wcolor_set");
    }

    [TestMethod]
    public void Background_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s =>
            s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(), out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, It.IsAny<IntPtr>())
        )
        .Returns((ComplexChar _, StringBuilder sb, out VideoAttribute attrs, out short colorPair, IntPtr _) =>
        {
            _ = sb.Append('H');
            attrs = VideoAttribute.Dim;
            colorPair = 10;
            return 0;
        });

        var s = new Surface(_cursesMock.Object, new(1));
        var (@char, style) = s.Background;
        style.ShouldBe(new()
        {
            Attributes = VideoAttribute.Dim,
            ColorMixture = new()
            {
                Handle = 10
            }
        });
        @char.ShouldBe(new('H'));

        _cursesMock.Verify(v => v.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<ComplexChar>.IsAny), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Get_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<ComplexChar>.IsAny))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.Background)
              .Operation.ShouldBe("wgetbkgrnd");
    }

    [TestMethod]
    public void Background_Set_SetsValue_IfCursesSucceeded()
    {
        var s = new Surface(_cursesMock.Object, new(1))
        {
            Background = (new('a'), new()
            {
                Attributes = VideoAttribute.Blink,
                ColorMixture = new()
                {
                    Handle = 22
                }
            })
        };

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, "a", VideoAttribute.Blink, 22, It.IsAny<IntPtr>()),
            Times.Once);

        _cursesMock.Verify(v => v.wbkgrnd(new(1), It.IsAny<ComplexChar>()));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Set_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wbkgrnd(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.Background = (new('a'), Style.Default))
              .Operation.ShouldBe("wbkgrnd");
    }

    [TestMethod]
    public void Size_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(22);

        var s = new Surface(_cursesMock.Object, new(1));
        s.Size.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Get_Throws_IfCursesFails_1()
    {
        _ = _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.Size)
              .Operation.ShouldBe("getmaxx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Get_Throws_IfCursesFails_2()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.Size)
              .Operation.ShouldBe("getmaxy");
    }

    [TestMethod]
    public void Origin_ShouldBeZero()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.Origin.ShouldBe(new(0, 0));
    }

    [TestMethod]
    public void Area_ShouldBeBoundBySize()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s, new Size(58, 78));
        s.Area.ShouldBe(new(0, 0, 58, 78));
    }

    [TestMethod]
    public void CaretLocation_Get_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _ = _cursesMock.Setup(s => s.getcury(It.IsAny<IntPtr>()))
                   .Returns(22);

        var s = new Surface(_cursesMock.Object, new(1));
        s.CaretLocation.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretLocation_Get_Throws_IfCursesFails_1()
    {
        _ = _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.CaretLocation)
              .Operation.ShouldBe("getcurx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretLocation_Get_Throws_IfCursesFails_2()
    {
        _ = _cursesMock.Setup(s => s.getcury(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.CaretLocation)
              .Operation.ShouldBe("getcury");
    }

    [TestMethod]
    public void CaretLocation_Set_SetsValue_IfCursesSucceeded()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s, new Size(100, 100));

        s.CaretLocation = new(11, 22);

        _cursesMock.Verify(v => v.wmove(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretLocation_Set_Throws_IfCursesFails()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sw, new Size(100, 100));

        _ = _cursesMock.Setup(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => sw.CaretLocation = new(1, 1))
              .Operation.ShouldBe("wmove");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretLocation_Set_Throws_IfOutsideArea()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s, new Size(1, 1));

        _ = Should.Throw<ArgumentOutOfRangeException>(() => s.CaretLocation = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretLocation_Set_UpdatesLocation_IfInsideArea()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s, new Size(100, 100));

        s.CaretLocation = new(5, 5);
        _cursesMock.Verify(v => v.wmove(new(1), 5, 5), Times.Once);
    }

    [TestMethod]
    public void Dirty_Returns_IfCursesSucceeded()
    {
        _ = _cursesMock.Setup(s => s.is_wintouched(It.IsAny<IntPtr>()))
                   .Returns(true);

        var s = new Surface(_cursesMock.Object, new(1));
        s.Dirty.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_DisposesTheSurface()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Dispose();
        s.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_CanBeCalledTwice()
    {
        var s = new Surface(_cursesMock.Object, new(22));
        s.Dispose();
        Should.NotThrow(s.Dispose);
    }

    [TestMethod]
    public void EnableAttributes_CallsCurses()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.EnableAttributes(VideoAttribute.Bold);

        _cursesMock.Verify(v => v.wattr_on(new(1), VideoAttribute.Bold, IntPtr.Zero), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void EnableAttributes_Throws_IfCursesCallFails()
    {
        _ = _cursesMock.Setup(s => s.wattr_on(It.IsAny<IntPtr>(), It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.EnableAttributes(VideoAttribute.Bold))
              .Operation.ShouldBe("wattr_on");
    }

    [TestMethod]
    public void DisableAttributes_CallsCurses()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DisableAttributes(VideoAttribute.Bold);

        _cursesMock.Verify(v => v.wattr_off(new(1), VideoAttribute.Bold, IntPtr.Zero), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DisableAttributes_Throws_IfCursesCallFails()
    {
        _ = _cursesMock.Setup(s => s.wattr_off(It.IsAny<IntPtr>(), It.IsAny<VideoAttribute>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));

        Should.Throw<CursesOperationException>(() => s.DisableAttributes(VideoAttribute.Bold))
              .Operation.ShouldBe("wattr_off");
    }

    [TestMethod]
    public void ScrollUp_DoesNothing_IfLinesIsLessThanOne()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        sf.ScrollUp(0);

        _cursesMock.Verify(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void ScrollUp_UsesHeight_IfLinesIsGreaterThanTheHeight()
    {
        var sf = new Surface(_cursesMock.Object, new(1));

        _ = _cursesMock.Setup(s => s.is_scrollok(sf.Handle))
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.getmaxy(sf.Handle))
                   .Returns(10);

        sf.ScrollUp(11);

        _cursesMock.Verify(s => s.wscrl(sf.Handle, 10));
    }

    [TestMethod]
    public void ScrollUp_Throws_IfScrollingNotEnabled()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(false);

        var s = new Surface(_cursesMock.Object, new(1));
        _ = Should.Throw<NotSupportedException>(() => s.ScrollUp(1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ScrollUp_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.ScrollUp(1))
              .Operation.ShouldBe("wscrl");
    }

    [TestMethod]
    public void ScrollUp_Scrolls_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var sw = new Surface(_cursesMock.Object, new(1));
        sw.ScrollUp(1);

        _cursesMock.Verify(s => s.wscrl(new(1), 1));
    }

    [TestMethod]
    public void ScrollDown_DoesNothing_IfLinesIsLessThanOne()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        sf.ScrollDown(0);

        _cursesMock.Verify(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void ScrollDown_UsesHeight_IfLinesIsGreaterThanTheHeight()
    {
        var sf = new Surface(_cursesMock.Object, new(1));

        _ = _cursesMock.Setup(s => s.is_scrollok(sf.Handle))
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.getmaxy(sf.Handle))
                   .Returns(10);

        sf.ScrollDown(11);

        _cursesMock.Verify(s => s.wscrl(sf.Handle, -10));
    }

    [TestMethod]
    public void ScrollDown_Throws_IfScrollingNotEnabled()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(false);

        var s = new Surface(_cursesMock.Object, new(1));
        _ = Should.Throw<NotSupportedException>(() => s.ScrollDown(1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ScrollDown_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.ScrollDown(1))
              .Operation.ShouldBe("wscrl");
    }

    [TestMethod]
    public void ScrollDown_Scrolls_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var sw = new Surface(_cursesMock.Object, new(1));
        sw.ScrollDown(1);

        _cursesMock.Verify(s => s.wscrl(new(1), -1));
    }

    [TestMethod]
    public void InsertEmptyLines_DoesNothing_IfLinesIsLessThanOne()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        sf.InsertEmptyLines(0);

        _cursesMock.Verify(s => s.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void InsertEmptyLines_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.InsertEmptyLines(1))
              .Operation.ShouldBe("winsdelln");
    }

    [TestMethod]
    public void InsertEmptyLines_AddsLines_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.InsertEmptyLines(1);

        _cursesMock.Verify(s => s.winsdelln(new(1), 1));
    }

    [TestMethod]
    public void DeleteLines_DoesNothing_IfLinesIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DeleteLines(0);

        _cursesMock.Verify(v => v.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DeleteLines_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DeleteLines(1))
              .Operation.ShouldBe("winsdelln");
    }

    [TestMethod]
    public void DeleteLines_DeletesLines_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.DeleteLines(1);

        _cursesMock.Verify(s => s.winsdelln(new(1), -1));
    }

    [TestMethod]
    public void ChangeTextStyle_DoesNothing_IfWidthIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.ChangeTextStyle(0, Style.Default);

        _cursesMock.Verify(
            v => v.wchgat(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<VideoAttribute>(), It.IsAny<short>(),
                It.IsAny<IntPtr>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ChangeTextStyle_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wchgat(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<VideoAttribute>(),
                       It.IsAny<short>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.ChangeTextStyle(1, Style.Default))
              .Operation.ShouldBe("wchgat");
    }

    [TestMethod]
    public void ChangeTextStyle_AddsLines_IfCursesSucceeds()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.ChangeTextStyle(3, new()
        {
            Attributes = VideoAttribute.Bold,
            ColorMixture = new()
            {
                Handle = 22
            }
        });

        _cursesMock.Verify(v => v.wchgat(new(1), 3, VideoAttribute.Bold, 22, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void DrawVerticalLine1_DoesNothing_IfWLengthIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        s.DrawVerticalLine(0, new('a'), Style.Default);

        _cursesMock.Verify(v => v.wvline_set(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>(), It.IsAny<int>()),
            Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawVerticalLine1_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wvline_set(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DrawVerticalLine(1, new('a'), Style.Default))
              .Operation.ShouldBe("wvline_set");
    }

    [TestMethod]
    public void DrawVerticalLine1_DrawsLine_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.DrawVerticalLine(3, new('a'),
            new()
            {
                Attributes = VideoAttribute.Bold,
                ColorMixture = new()
                {
                    Handle = 22
                }
            });

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                It.IsAny<short>(), IntPtr.Zero), Times.Once);

        _cursesMock.Verify(v => v.wvline_set(new(1), It.IsAny<ComplexChar>(), 3), Times.Once);
    }

    [TestMethod]
    public void DrawVerticalLine2_DoesNothing_IfLengthIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DrawVerticalLine(0);

        _cursesMock.Verify(v => v.wvline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawVerticalLine2_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wvline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DrawVerticalLine(1))
              .Operation.ShouldBe("wvline");
    }

    [TestMethod]
    public void DrawVerticalLine2_DrawsLine_IfCursesSucceeds()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DrawVerticalLine(3);

        _cursesMock.Verify(v => v.wvline(new(1), 0, 3), Times.Once);
    }

    [TestMethod]
    public void DrawHorizontalLine1_DoesNothing_IfLengthIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DrawHorizontalLine(0, new('a'), Style.Default);

        _cursesMock.Verify(v => v.whline_set(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>(), It.IsAny<int>()),
            Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawHorizontalLine1_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.whline_set(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DrawHorizontalLine(1, new('a'), Style.Default))
              .Operation.ShouldBe("whline_set");
    }

    [TestMethod]
    public void DrawHorizontalLine1_DrawsLine_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.DrawHorizontalLine(3, new('a'),
            new()
            {
                Attributes = VideoAttribute.Bold,
                ColorMixture = new()
                {
                    Handle = 22
                }
            });

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                It.IsAny<short>(), IntPtr.Zero), Times.Once);

        _cursesMock.Verify(v => v.whline_set(new(1), It.IsAny<ComplexChar>(), 3), Times.Once);
    }

    [TestMethod]
    public void DrawHorizontalLine2_DoesNothing_IfWidthIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DrawHorizontalLine(0);

        _cursesMock.Verify(v => v.whline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawHorizontalLine2_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.whline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DrawHorizontalLine(1))
              .Operation.ShouldBe("whline");
    }

    [TestMethod]
    public void DrawHorizontalLine2_DrawsLine_IfCursesSucceeds()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.DrawHorizontalLine(3);

        _cursesMock.Verify(v => v.whline(new(1), 0, 3), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawBorder_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wborder_set(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(),
                       It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(),
                       It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.DrawBorder(new('a'), new('a'), new('a'), new('a'), new('a'),
                  new('a'), new('a'), new('a'), Style.Default))
              .Operation.ShouldBe("wborder_set");
    }

    [TestMethod]
    public void DrawBorder_DrawsBorder_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.DrawBorder(new('a'), new('a'), new('a'), new('a'), new('a'),
            new('a'), new('a'), new('a'), Style.Default);

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<ComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<VideoAttribute>(),
                It.IsAny<short>(), IntPtr.Zero), Times.Exactly(8));

        _cursesMock.Verify(
            s => s.wborder_set(new(1), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(),
                It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(), It.IsAny<ComplexChar>(),
                It.IsAny<ComplexChar>()), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawBorder2_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wborder(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(),
                       It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(s.DrawBorder)
              .Operation.ShouldBe("wborder");
    }

    [TestMethod]
    public void DrawBorder2_DrawsBorder_IfCursesSucceeds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        sw.DrawBorder();

        _cursesMock.Verify(s => s.wborder(new(1), 0, 0, 0, 0,
            0, 0, 0, 0), Times.Once);
    }

    [TestMethod]
    public void LineDirty_DoesNothing_IfYIsNegative()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        sf.LineDirty(-1)
          .ShouldBeFalse();

        _cursesMock.Verify(s => s.is_linetouched(It.IsAny<IntPtr>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void LineDirty_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _ = _cursesMock.Setup(s => s.is_linetouched(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(true);

        var s = new Surface(_cursesMock.Object, new(1));
        s.LineDirty(1)
         .ShouldBeTrue();
    }

    [TestMethod]
    public void MarkDirty1_DoesNothing_IfCountIsNegative()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.MarkDirty(0, -1);

        _cursesMock.Verify(v => v.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [TestMethod]
    public void MarkDirty1_AdjustsYAndCountToMatchActualHeight()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        _ = _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(10);

        sw.MarkDirty(4, 10);

        _cursesMock.Verify(v => v.wtouchln(sw.Handle, 4, 6, 1), Times.Once);
    }

    [TestMethod]
    public void MarkDirty1_DoesNotDoAnythingIfNotInBounds()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        _ = _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(10);

        sw.MarkDirty(14, 5);
        _cursesMock.Verify(v => v.wtouchln(sw.Handle, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void MarkDirty1_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _ = _cursesMock.Setup(s => s.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), 1))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.MarkDirty(1, 1))
              .Operation.ShouldBe("wtouchln");
    }

    [TestMethod]
    public void MarkDirty1_Succeeds_IfCursesSucceeds()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.NotThrow(() => s.MarkDirty(1, 9));

        _cursesMock.Verify(v => v.wtouchln(s.Handle, 1, 9, 1));
    }

    [TestMethod]
    public void MarkDirty2_Calls_MarkDirty1()
    {
        _ = _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(99);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.NotThrow(s.MarkDirty);

        _cursesMock.Verify(v => v.wtouchln(s.Handle, 0, 99, 1));
    }

    [TestMethod]
    public void Clear_AsksCurses_1()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.Clear();

        _cursesMock.Verify(v => v.werase(new(1)), Times.Once);
    }

    [TestMethod]
    public void Clear_AsksCurses_2()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.Clear(ClearStrategy.LineFromCaret);

        _cursesMock.Verify(v => v.wclrtoeol(new(1)), Times.Once);
    }

    [TestMethod]
    public void Clear_AsksCurses_3()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.Clear(ClearStrategy.FullFromCaret);

        _cursesMock.Verify(v => v.wclrtobot(new(1)), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_1()
    {
        _ = _cursesMock.Setup(v => v.werase(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.Clear())
              .Operation.ShouldBe("werase");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_2()
    {
        _ = _cursesMock.Setup(v => v.wclrtoeol(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.Clear(ClearStrategy.LineFromCaret))
              .Operation.ShouldBe("wclrtoeol");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_3()
    {
        _ = _cursesMock.Setup(v => v.wclrtobot(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.Clear(ClearStrategy.FullFromCaret))
              .Operation.ShouldBe("wclrtobot");
    }

    [TestMethod]
    public void WriteText1_Throws_IfStringIsNull()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _ = Should.Throw<ArgumentNullException>(() => s.WriteText(null!, Style.Default));
    }

    [TestMethod]
    public void WriteText1_DoesNotCallCurse_IfEmptyString()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.WriteText("", Style.Default);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Never);
    }

    [TestMethod]
    public void WriteText1_CallsCursesForEachChar()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.WriteText("12345", Style.Default);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Exactly(5));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void WriteText1_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wadd_wch(new(1), It.IsAny<ComplexChar>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.WriteText("12345", Style.Default))
              .Operation.ShouldBe("wadd_wch");
    }

    [TestMethod]
    public void WriteText1_StopsAtWidthIfNotWrapping()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(3, 1));

        sf.WriteText("1234", Style.Default, false);

        _cursesMock.Verify(v => v.wadd_wch(sf.Handle, It.IsAny<ComplexChar>()), Times.Exactly(3));
    }

    [TestMethod]
    public void WriteText2_Calls_WriteText1()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.WriteText("12345");

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Exactly(5));
    }

    [TestMethod]
    public void WriteText2_StopsAtWidthIfNotWrapping()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(3, 1));

        sf.WriteText("1234", false);

        _cursesMock.Verify(v => v.wadd_wch(sf.Handle, It.IsAny<ComplexChar>()), Times.Exactly(3));
    }

    [TestMethod]
    public void WriteText3_DoesNotCallCurse_IfUninitialized()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.WriteText(new StyledText());

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Never);
    }

    [TestMethod]
    public void WriteText3_DoesNotCallCurse_IfEmpty()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.WriteText(new StyledText("", Style.Default));

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Never);
    }

    [TestMethod]
    public void WriteText3_CallsCursesForEachCharInAllParts()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        var s1 = new Style { Attributes = VideoAttribute.Blink, ColorMixture = new() { Handle = 1 } };
        var s2 = new Style { Attributes = VideoAttribute.Italic, ColorMixture = new() { Handle = 2 } };

        var text = new StyledText("h", s1).Plus("w", s2);
        sf.WriteText(text);

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, "h", VideoAttribute.Blink, 1, It.IsAny<IntPtr>()),
            Times.Once);

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, "w", VideoAttribute.Italic, 2, It.IsAny<IntPtr>()),
            Times.Once);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Exactly(2));
    }

    [TestMethod]
    public void WriteText3_StopsAtWidthIfNotWrapping()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(3, 1));

        var text = new StyledText("1234", Style.Default);
        sf.WriteText(text, false);

        _cursesMock.Verify(v => v.wadd_wch(sf.Handle, It.IsAny<ComplexChar>()), Times.Exactly(3));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void WriteText3_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.wadd_wch(new(1), It.IsAny<ComplexChar>()))
                   .Returns(-1);

        var s = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => s.WriteText(new StyledText("12345", Style.Default)))
              .Operation.ShouldBe("wadd_wch");
    }

    [TestMethod]
    public void NextLine_Calls_WriteText2()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.NextLine();

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, "\n", VideoAttribute.None, 0, It.IsAny<IntPtr>()),
            Times.Once);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Once);
    }

    [TestMethod]
    public void RemoveText_DoesNothing_IfCountIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.RemoveText(0);

        _cursesMock.Verify(v => v.wdelch(It.IsAny<IntPtr>()), Times.Never);
    }

    [TestMethod]
    public void RemoveText_CallsCurses_ForCountTimes()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        s.RemoveText(10);

        _cursesMock.Verify(v => v.wdelch(new(1)), Times.Exactly(10));
    }

    [TestMethod]
    public void RemoveText_CallsCurses_UntilTheFirstFailure()
    {
        var count = 0;
        _ = _cursesMock.Setup(s => s.wdelch(It.IsAny<IntPtr>()))
                   .Returns((IntPtr _) => count++ < 5 ? 0 : -1);

        var s = new Surface(_cursesMock.Object, new(1));
        s.RemoveText(10);

        _cursesMock.Verify(v => v.wdelch(new(1)), Times.Exactly(6));
    }

    [TestMethod]
    public void GetText_DoesNothing_IfCountIsLessThanOne()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        s.GetText(0)
         .ShouldBeEmpty();

        _cursesMock.Verify(v => v.win_wch(It.IsAny<IntPtr>(), out It.Ref<ComplexChar>.IsAny), Times.Never);
        _cursesMock.Verify(v => v.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void GetText_AsksCurses_WithTheGivenCountUpToEnd()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));
        _ = _cursesMock.Setup(s => s.getcurx(sf.Handle))
                   .Returns(2);

        var ch = 'a';
        _ = _cursesMock.Setup(s => s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero))
                   .Returns((ComplexChar _, StringBuilder sb, out VideoAttribute a, out short cp,
                       IntPtr _) =>
                   {
                       _ = sb.Append(ch++);
                       a = Style.Default.Attributes;
                       cp = Style.Default.ColorMixture.Handle;

                       return 0;
                   });


        var txt = sf.GetText(5);
        txt.ShouldBe(new[] { (new('a'), Style.Default), (new Rune('b'), Style.Default), (new('c'), Style.Default) });

        _cursesMock.Verify(v => v.wmove(sf.Handle, 0, 3), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, 0, 4), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, 0, 2), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(3));
        _cursesMock.Verify(v => v.win_wch(sf.Handle, out It.Ref<ComplexChar>.IsAny), Times.Exactly(3));
    }

    [TestMethod]
    public void GetText_AsksCurses_UntilReadFails()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));
        _ = _cursesMock.Setup(s => s.getcurx(sf.Handle))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.getcury(sf.Handle))
                   .Returns(3);

        _ = _cursesMock.Setup(s => s.win_wch(sf.Handle, out It.Ref<ComplexChar>.IsAny))
                   .Returns(-1);

        var txt = sf.GetText(5);
        txt.ShouldBeEmpty();

        _cursesMock.Verify(v => v.wmove(sf.Handle, 3, 2), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _cursesMock.Verify(v => v.win_wch(sf.Handle, out It.Ref<ComplexChar>.IsAny), Times.Once);
    }

    [TestMethod]
    public void GetText_AsksCurses_UntilMovingCursorFails()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));
        _ = _cursesMock.Setup(s => s.getcurx(sf.Handle))
                   .Returns(2);

        _ = _cursesMock.Setup(s => s.getcury(sf.Handle))
                   .Returns(3);

        _ = _cursesMock.Setup(s => s.wmove(sf.Handle, 3, 3))
                   .Returns(-1);

        _ = _cursesMock.Setup(s => s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<VideoAttribute>.IsAny, out It.Ref<short>.IsAny, IntPtr.Zero))
                   .Returns((ComplexChar _, StringBuilder sb, out VideoAttribute a, out short cp,
                       IntPtr _) =>
                   {
                       _ = sb.Append('a');
                       a = Style.Default.Attributes;
                       cp = Style.Default.ColorMixture.Handle;

                       return 0;
                   });

        var txt = sf.GetText(5);
        txt.ShouldBe(new[] { (new Rune('a'), Style.Default) });

        _cursesMock.Verify(v => v.wmove(sf.Handle, 3, 3), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, 3, 2), Times.Once);
        _cursesMock.Verify(v => v.wmove(sf.Handle, It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        _cursesMock.Verify(v => v.win_wch(sf.Handle, out It.Ref<ComplexChar>.IsAny), Times.Once);
    }

    [TestMethod]
    public void Replace_Throws_IfSurfaceIsNull()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _ = Should.Throw<ArgumentNullException>(() => s.Replace(null!, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Throws_IfSurfaceIsItself()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        _ = Should.Throw<ArgumentException>(() => s.Replace(s, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Succeeds_IfCursesSucceeds_Overlay()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        var s2 = new Surface(_cursesMock.Object, new(2));

        s1.Replace(s2, ReplaceStrategy.Overlay);

        _cursesMock.Verify(v => v.overlay(new(1), new(2)), Times.Once);
        _cursesMock.Verify(v => v.overwrite(new(1), new(2)), Times.Never);
    }

    [TestMethod]
    public void Replace_Succeeds_IfCursesSucceeds_Overwrite()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        var s2 = new Surface(_cursesMock.Object, new(2));

        s1.Replace(s2, ReplaceStrategy.Overwrite);

        _cursesMock.Verify(v => v.overlay(new(1), new(2)), Times.Never);
        _cursesMock.Verify(v => v.overwrite(new(1), new(2)), Times.Once);
    }

    [TestMethod]
    public void Replace_Throws_IfCursesFails_Overlay()
    {
        _ = _cursesMock.Setup(v => v.overlay(It.IsAny<IntPtr>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s1 = new Surface(_cursesMock.Object, new(1));
        var s2 = new Surface(_cursesMock.Object, new(2));

        Should.Throw<CursesOperationException>(() => s1.Replace(s2, ReplaceStrategy.Overlay))
              .Operation.ShouldBe("overlay");
    }

    [TestMethod]
    public void Replace_Throws_IfCursesFails_Overwrite()
    {
        _ = _cursesMock.Setup(v => v.overwrite(It.IsAny<IntPtr>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var s1 = new Surface(_cursesMock.Object, new(1));
        var s2 = new Surface(_cursesMock.Object, new(2));

        Should.Throw<CursesOperationException>(() => s1.Replace(s2, ReplaceStrategy.Overwrite))
              .Operation.ShouldBe("overwrite");
    }

    [TestMethod]
    public void Replace2_Throws_IfSurfaceIsNull()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _ = Should.Throw<ArgumentNullException>(() =>
            s.Replace(null!, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_Throws_IfSurfaceIsItself()
    {
        var s = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s, new Size(1, 1));

        _ = Should.Throw<ArgumentException>(() => s.Replace(s, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_DoesNothing_IfTheDestinationAreaIsOutsideBounds()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(10, 50));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(10, 50));

        s1.Replace(s2, new(0, 0, 5, 5), new(999, 999), ReplaceStrategy.Overlay);

        _cursesMock.Verify(
            v => v.copywin(It.IsAny<IntPtr>(), It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void Replace2_DoesNothing_IfTheSourceAreaIsOutsideBounds()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(10, 50));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(10, 50));

        s1.Replace(s2, new(11, 11, 5, 5), new(0, 0), ReplaceStrategy.Overlay);

        _cursesMock.Verify(
            v => v.copywin(It.IsAny<IntPtr>(), It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void Replace2_CopiesAdjustedArea()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(10, 50));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(10, 50));

        s1.Replace(s2, new(3, 4, 5, 5), new(5, 5), ReplaceStrategy.Overlay);

        _cursesMock.Verify(v => v.copywin(s1.Handle, s2.Handle, 4, 3, 5,
            5, 9, 9, 1), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.copywin(It.IsAny<IntPtr>(), It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(100, 100));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(100, 100));

        Should.Throw<CursesOperationException>(() =>
              {
                  s1.Replace(s2, new(1, 1, 5, 5), new(0, 0), ReplaceStrategy.Overlay);
              })
              .Operation.ShouldBe("copywin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_CallsCurses_IfCursesOverlay()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(100, 100));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(100, 100));

        s1.Replace(s2, new(1, 2, 3, 4), new(5, 6), ReplaceStrategy.Overlay);
        _cursesMock.Verify(s => s.copywin(s1.Handle, s2.Handle, 2, 1, 6,
            5, 9, 7, 1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_CallsCurses_IfCursesOverwrite()
    {
        var s1 = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(s1, new Size(100, 100));

        var s2 = new Surface(_cursesMock.Object, new(2));
        _cursesMock.MockArea(s2, new Size(100, 100));

        s1.Replace(s2, new(1, 2, 3, 4), new(5, 6), ReplaceStrategy.Overwrite);
        _cursesMock.Verify(s => s.copywin(s1.Handle, s2.Handle, 2, 1, 6,
            5, 9, 7, 0));
    }

    [TestMethod]
    public void ManagedCaret_Get_ReturnsWhatCursesSays()
    {
        _ = _cursesMock.Setup(s => s.is_leaveok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var sw = new Surface(_cursesMock.Object, new(1));
        sw.ManagedCaret.ShouldBeTrue();
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ManagedCaret_Set_Throws_IfCursesFails()
    {
        _ = _cursesMock.Setup(s => s.leaveok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        var sw = new Surface(_cursesMock.Object, new(1));
        Should.Throw<CursesOperationException>(() => sw.ManagedCaret = true)
              .Operation.ShouldBe("leaveok");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawCell_Throws_IfCursesFails_1()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));

        _ = _cursesMock.Setup(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => ((IDrawSurface) sf).DrawCell(new(1, 1), new('A'), Style.Default))
              .Operation.ShouldBe("wmove");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawCell_Throws_IfCursesFails_2()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));

        _ = _cursesMock.Setup(s => s.wadd_wch(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => ((IDrawSurface) sf).DrawCell(new(1, 1), new('A'), Style.Default))
              .Operation.ShouldBe("wadd_wch");
    }

    [TestMethod]
    public void DrawCell_CallsCurses()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));

        ((IDrawSurface) sf).DrawCell(new(3, 4), new('A'), Style.Default);

        _cursesMock.Verify(s => s.wmove(new(1), 4, 3), Times.Once);
        _cursesMock.Verify(s => s.wadd_wch(new(1), It.IsAny<ComplexChar>()), Times.Once);
    }

    [TestMethod]
    public void DrawCell_DoesNothing_IfLocationOutsideBounds()
    {
        var sf = new Surface(_cursesMock.Object, new(1));
        _cursesMock.MockArea(sf, new Size(5, 5));

        ((IDrawSurface) sf).DrawCell(new(6, 6), new('A'), Style.Default);

        _cursesMock.Verify(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _cursesMock.Verify(s => s.wadd_wch(It.IsAny<IntPtr>(), It.IsAny<ComplexChar>()), Times.Never);
    }

    [TestMethod]
    public void Draw1_Throws_IfDrawingIsNull()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        _ = Should.Throw<ArgumentNullException>(() => s.Draw(new(0, 0), new(0, 0, 1, 1), null!));
    }

    [TestMethod]
    public void Draw1_CallsDrawing_DrawTo_ToDraw()
    {
        var drawingMock = new Mock<IDrawable>();

        var area = new Rectangle(1, 2, 100, 200);
        var location = new Point(10, 20);

        var s = new Surface(_cursesMock.Object, new(1));
        s.Draw(location, area, drawingMock.Object);

        drawingMock.Verify(v => v.DrawOnto(s, area, location), Times.Once);
    }

    [TestMethod]
    public void Draw2_CallsDrawing_DrawTo_ToDraw()
    {
        var drawingMock = new Mock<IDrawable>();
        _ = drawingMock.Setup(s => s.Size)
                   .Returns(new Size(100, 200));

        var location = new Point(10, 20);
        var area = new Rectangle(0, 0, 100, 200);

        var s = new Surface(_cursesMock.Object, new(1));
        s.Draw(location, drawingMock.Object);

        drawingMock.Verify(v => v.DrawOnto(s, area, location), Times.Once);
    }

    [TestMethod, DataRow(0, 0, true), DataRow(-1, 0, false), DataRow(9, 19, true), DataRow(10, 9, false)]
    public void IsPointWithin_Checks_IfPointInside(int x, int y, bool exp)
    {
        var sw = new Surface(_cursesMock.Object, new(1));

        _ = _cursesMock.Setup(s => s.getmaxx(sw.Handle))
                   .Returns(10);

        _ = _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(20);

        sw.IsPointWithin(new(x, y))
          .ShouldBe(exp);
    }

    [TestMethod, DataRow(0, 0, 2, 2, true), DataRow(1, 2, 2, 2, true), DataRow(11, 12, -2, -2, false),
     DataRow(12, 16, 3, 3, false), DataRow(5, 5, 10, 10, false), DataRow(0, 0, 10, 10, true)]
    public void IsRectangleWithin_Checks_IfAreaInside(int x, int y, int w, int h,
        bool exp)
    {
        var sw = new Surface(_cursesMock.Object, new(1));

        _ = _cursesMock.Setup(s => s.getmaxx(sw.Handle))
                   .Returns(10);

        _ = _cursesMock.Setup(s => s.getmaxy(sw.Handle))
                   .Returns(10);

        sw.IsRectangleWithin(new(x, y, w, h))
          .ShouldBe(exp);
    }

    [TestMethod]
    public void Destroy_CallsCurses()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        s.Destroy();
        s.Disposed.ShouldBeTrue();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }

    [TestMethod]
    public void Destroy_Succeeds_EventIfCursesFails()
    {
        var sw = new Surface(_cursesMock.Object, new(1));
        _ = _cursesMock.Setup(s => s.delwin(It.IsAny<IntPtr>()))
                   .Returns(-1);

        sw.Destroy();
        sw.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_CallsCurses()
    {
        var s = new Surface(_cursesMock.Object, new(1));

        s.Dispose();

        _cursesMock.Verify(v => v.delwin(new(1)), Times.Once);
    }

    private sealed class Surface(ICursesBackend curses, IntPtr handle): Sharpie.Surface(curses, handle)
    {
        protected internal override void AssertSynchronized()
        {
        }
    }
}
