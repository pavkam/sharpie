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

    [TestInitialize] public void TestInitialize() { _cursesMock = new(); }

    [TestMethod]
    public void Ctor_Throws_WhenCursesIfNull()
    {
        Should.Throw<ArgumentNullException>(() => new Window(null!, null, IntPtr.MaxValue));
    }

    [TestMethod]
    public void Ctor_Throws_WhenHandleIsZero()
    {
        Should.Throw<ArgumentException>(() => new Window(_cursesMock.Object, null, IntPtr.Zero));
    }

    [TestMethod]
    public void Ctor_ConfiguresWindow_InCurses()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        _cursesMock.Verify(v => v.keypad(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.notimeout(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.nodelay(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.syncok(w.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.scrollok(w.Handle, true), Times.Once);
    }

    [TestMethod]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_1()
    {
        _cursesMock.Setup(s => s.keypad(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, null, new(1)))
              .Operation.ShouldBe("keypad");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_2()
    {
        _cursesMock.Setup(s => s.notimeout(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, null, new(1)))
              .Operation.ShouldBe("notimeout");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_3()
    {
        _cursesMock.Setup(s => s.nodelay(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, null, new(1)))
              .Operation.ShouldBe("nodelay");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_4()
    {
        _cursesMock.Setup(s => s.syncok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, null, new(1)))
              .Operation.ShouldBe("syncok");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_5()
    {
        _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Window(_cursesMock.Object, null, new(1)))
              .Operation.ShouldBe("scrollok");
    }

    [TestMethod]
    public void Ctor_RegistersItselfIntoParent()
    {
        var p = new Window(_cursesMock.Object, null, IntPtr.MaxValue);
        var w = new Window(_cursesMock.Object, p, IntPtr.MaxValue);

        p.Children.ShouldContain(w);
    }

    [TestMethod]
    public void Parent_IsNull_IfNotSupplied()
    {
        var w = new Window(_cursesMock.Object, null, IntPtr.MaxValue);
        w.Parent.ShouldBeNull();
    }

    [TestMethod]
    public void Parent_IsNotNull_IfSupplied()
    {
        var p = new Window(_cursesMock.Object, null, IntPtr.MaxValue);
        var w = new Window(_cursesMock.Object, p, IntPtr.MaxValue);

        w.Parent.ShouldBe(p);
    }

    [TestMethod]
    public void Curses_IsInitialized()
    {
        var w = new Window(_cursesMock.Object, null, IntPtr.MaxValue);
        w.Curses.ShouldBe(_cursesMock.Object);
    }

    [TestMethod]
    public void Handle_IsInitialized()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Handle.ShouldBe(new(22));
    }

    [TestMethod]
    public void Handle_Throws_IfDisposed()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Dispose();

        Should.Throw<ObjectDisposedException>(() => w.Handle);
    }

    [TestMethod]
    public void AssertAlive_DoesNothing_IfNotDisposed()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        Should.NotThrow(() => w.AssertAlive());
    }

    [TestMethod]
    public void AssertAlive_Throws_IfDisposed()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Dispose();

        Should.Throw<ObjectDisposedException>(() => w.AssertAlive());
    }

    [TestMethod]
    public void ChildrenParent_RelationshipIsAutoManaged()
    {
        var p = new Window(_cursesMock.Object, null, IntPtr.MaxValue);
        p.Children.ShouldBeEmpty();

        var c1 = new Window(_cursesMock.Object, p, IntPtr.MaxValue);
        p.Children.ShouldContain(c1);
        c1.Children.ShouldBeEmpty();

        var c2 = new Window(_cursesMock.Object, c1, IntPtr.MaxValue);
        p.Children.ShouldContain(c1);
        c1.Children.ShouldContain(c2);
        c2.Children.ShouldBeEmpty();

        c1.Dispose();
        c2.Disposed.ShouldBeTrue();
        c1.Children.ShouldBeEmpty();
        p.Children.ShouldBeEmpty();
        p.Disposed.ShouldBeFalse();
    }

    [TestMethod]
    public void Children_IsEmpty_WhenCreated()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Children.ShouldBeEmpty();
    }

    [TestMethod]
    public void EnableScrolling_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.EnableScrolling.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void EnableScrolling_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.EnableScrolling = enabled;

        _cursesMock.Verify(v => v.scrollok(new(1), enabled), Times.AtMost(2));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void EnableScrolling_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.EnableScrolling = false)
              .Operation.ShouldBe("scrollok");
    }

    [TestMethod]
    public void Disposed_ReturnsFalse_IfNotDisposed()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Disposed.ShouldBeFalse();
    }

    [TestMethod]
    public void Disposed_ReturnsTrue_IfDisposed()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Dispose();
        w.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void UseHardwareLineEdit_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_idlok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.UseHardwareLineEdit.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareLineEdit_Set_DoesNotSetValue_IfNotSupportedByHardware(bool enabled)
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.UseHardwareLineEdit = enabled;

        _cursesMock.Verify(v => v.idlok(new(1), enabled), Times.Never);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareLineEdit_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
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

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.UseHardwareLineEdit = false)
              .Operation.ShouldBe("idlok");
    }

    [TestMethod]
    public void UseHardwareCharEdit_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_idcok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.UseHardwareCharEdit.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareCharEdit_Set_DoesNotSetValue_IfNotSupportedByHardware(bool enabled)
    {
        _cursesMock.Setup(s => s.has_ic())
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.UseHardwareCharEdit = enabled;

        _cursesMock.Verify(v => v.idcok(new(1), enabled), Times.Never);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void UseHardwareCharEdit_Set_SetsValue_IfCursesSucceeded(bool enabled)
    {
        _cursesMock.Setup(s => s.has_ic())
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.UseHardwareCharEdit = enabled;

        _cursesMock.Verify(v => v.idcok(new(1), enabled), Times.Once);
    }

    [TestMethod]
    public void Style_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny,
                       IntPtr.Zero))
                   .Returns((IntPtr _, out uint a, out ushort p, IntPtr _) =>
                   {
                       a = (uint) VideoAttribute.Italic;
                       p = 22;

                       return 0;
                   });


        var w = new Window(_cursesMock.Object, null, new(1));
        var s = w.Style;
        s.Attributes.ShouldBe(VideoAttribute.Italic);
        s.ColorMixture.ShouldBe(new() { Handle = 22 });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Get_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny,
                       IntPtr.Zero))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Style)
              .Operation.ShouldBe("wattr_get");
    }

    [TestMethod]
    public void Style_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Style = new() { Attributes = VideoAttribute.Italic, ColorMixture = new() { Handle = 22 } };

        _cursesMock.Verify(v => v.wattr_set(It.IsAny<IntPtr>(), (uint) VideoAttribute.Italic, 22, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wattr_set(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<ushort>(), IntPtr.Zero))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Style = Style.Default)
              .Operation.ShouldBe("wattr_set");
    }

    [TestMethod]
    public void ColorMixture_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny,
                       IntPtr.Zero))
                   .Returns((IntPtr _, out uint a, out ushort p, IntPtr _) =>
                   {
                       a = 0;
                       p = 22;

                       return 0;
                   });


        var w = new Window(_cursesMock.Object, null, new(1));
        w.ColorMixture.ShouldBe(new() { Handle = 22 });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ColorMixture_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny,
                       IntPtr.Zero))
                   .Returns(-1);


        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.ColorMixture)
              .Operation.ShouldBe("wattr_get");
    }

    [TestMethod]
    public void ColorMixture_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.ColorMixture = new() { Handle = 22 };

        _cursesMock.Verify(v => v.wcolor_set(It.IsAny<IntPtr>(), 22, IntPtr.Zero));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ColorMixture_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wcolor_set(It.IsAny<IntPtr>(), It.IsAny<ushort>(), IntPtr.Zero))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.ColorMixture = ColorMixture.Default)
              .Operation.ShouldBe("wcolor_set");
    }

    [TestMethod]
    public void Background_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny, It.IsAny<IntPtr>()))
                   .Returns((CursesComplexChar _, StringBuilder sb, out uint attrs, out ushort colorPair,
                       IntPtr _) =>
                   {
                       sb.Append('H');
                       attrs = (uint) VideoAttribute.Dim;
                       colorPair = 10;
                       return 0;
                   });

        var w = new Window(_cursesMock.Object, null, new(1));
        var bk = w.Background;
        bk.style.ShouldBe(new() { Attributes = VideoAttribute.Dim, ColorMixture = new() { Handle = 10 } });
        bk.@char.ShouldBe(new('H'));

        _cursesMock.Verify(v => v.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<CursesComplexChar>.IsAny), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Get_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<CursesComplexChar>.IsAny))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Background)
              .Operation.ShouldBe("wgetbkgrnd");
    }

    [TestMethod]
    public void Background_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Background = (new('a'), new() { Attributes = VideoAttribute.Blink, ColorMixture = new() { Handle = 22 } });

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<CursesComplexChar>.IsAny, "a", (uint) VideoAttribute.Blink, 22,
                It.IsAny<IntPtr>()), Times.Once);

        _cursesMock.Verify(v => v.wbkgrnd(new(1), It.IsAny<CursesComplexChar>()));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wbkgrnd(It.IsAny<IntPtr>(), It.IsAny<CursesComplexChar>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Background = (new('a'), Style.Default))
              .Operation.ShouldBe("wbkgrnd");
    }

    [TestMethod]
    public void Location_Get_ForMains_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getbegx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _cursesMock.Setup(s => s.getbegy(It.IsAny<IntPtr>()))
                   .Returns(22);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.Location.ShouldBe(new(11, 22));
    }

    [TestMethod]
    public void Location_Get_ForSubs_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(22);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));
        w.Location.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_ForMains_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.getbegx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_ForMains_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.getbegy(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getbegy");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_ForSubs_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.getparx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getparx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Get_ForSubs_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.getpary(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        Should.Throw<CursesOperationException>(() => w.Location)
              .Operation.ShouldBe("getpary");
    }

    [TestMethod]
    public void Location_Set_ForMains_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Location = new(11, 22);

        _cursesMock.Verify(v => v.mvwin(new(1), 22, 11), Times.Once);
    }

    [TestMethod]
    public void Location_Set_ForSubs_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        w.Location = new(11, 22);

        _cursesMock.Verify(v => v.mvderwin(new(2), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_ForMains_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        _cursesMock.Setup(s => s.mvwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Location = new(1, 1))
              .Operation.ShouldBe("mvwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_ForSubs_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.mvderwin(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(1));

        Should.Throw<CursesOperationException>(() => w.Location = new(1, 1))
              .Operation.ShouldBe("mvderwin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_Throws_IfOutsideParent()
    {
        _cursesMock.Setup(s => s.getmaxx(new(1)))
                   .Returns(5);

        _cursesMock.Setup(s => s.getmaxy(new(1)))
                   .Returns(5);

        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns((IntPtr _, int y, int x) => y is >= 0 and < 10 && x is >= 0 and < 10);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        Should.Throw<ArgumentOutOfRangeException>(() => w.Location = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Location_Set_UpdatesLocation_IfInsideParent()
    {
        _cursesMock.Setup(s => s.getmaxx(new(1)))
                   .Returns(5);

        _cursesMock.Setup(s => s.getmaxy(new(1)))
                   .Returns(5);

        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var p = new Screen(_cursesMock.Object, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        w.Location = new(5, 5);

        _cursesMock.Verify(v => v.mvwin(new(2), 5, 5), Times.Once);
    }

    [TestMethod]
    public void Size_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(22);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.Size.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Get_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Size)
              .Operation.ShouldBe("getmaxx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Get_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.Size)
              .Operation.ShouldBe("getmaxy");
    }

    [TestMethod]
    public void Size_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Size = new(11, 22);

        _cursesMock.Verify(v => v.wresize(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        _cursesMock.Setup(s => s.wresize(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.Size = new(1, 1))
              .Operation.ShouldBe("wresize");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_Throws_IfOutsideParent()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        Should.Throw<ArgumentOutOfRangeException>(() => w.Size = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Size_Set_UpdatesSize_IfInsideParent()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var p = new Window(_cursesMock.Object, null, new(1));
        var w = new Window(_cursesMock.Object, p, new(2));

        w.Size = new(5, 5);

        _cursesMock.Verify(v => v.wresize(new(2), 5, 5), Times.Once);
    }

    [TestMethod]
    public void CaretPosition_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(11);

        _cursesMock.Setup(s => s.getcury(It.IsAny<IntPtr>()))
                   .Returns(22);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.CaretPosition.ShouldBe(new(11, 22));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretPosition_Get_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.CaretPosition)
              .Operation.ShouldBe("getcurx");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretPosition_Get_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.getcury(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.CaretPosition)
              .Operation.ShouldBe("getcury");
    }

    [TestMethod]
    public void CaretPosition_Set_SetsValue_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.CaretPosition = new(11, 22);

        _cursesMock.Verify(v => v.wmove(new(1), 22, 11), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretPosition_Set_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => w.CaretPosition = new(1, 1))
              .Operation.ShouldBe("wmove");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretPosition_Set_Throws_IfOutsideArea()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<ArgumentOutOfRangeException>(() => w.CaretPosition = new(6, 6));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CaretPosition_Set_UpdatesLocation_IfInsideArea()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));

        w.CaretPosition = new(5, 5);
        _cursesMock.Verify(v => v.wmove(new(1), 5, 5), Times.Once);
    }

    [TestMethod]
    public void IsDirty_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_wintouched(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.IsDirty.ShouldBeTrue();
    }

    [TestMethod]
    public void ImmediateRefresh_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.is_immedok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.ImmediateRefresh.ShouldBeTrue();
    }

    [TestMethod]
    public void ImmediateRefresh_Sets_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.ImmediateRefresh = true;

        _cursesMock.Verify(v => v.immedok(new(1), true), Times.Once);
    }

    [TestMethod]
    public void Dispose_DisposesTheWindow()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Dispose();
        w.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_CanBeCalledTwice()
    {
        var w = new Window(_cursesMock.Object, null, new(22));
        w.Dispose();
        Should.NotThrow(() => w.Dispose());
    }

    [TestMethod]
    public void EnableAttributes_CallsCurses()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.EnableAttributes(VideoAttribute.Bold);

        _cursesMock.Verify(v => v.wattr_on(new(1), (uint) VideoAttribute.Bold, IntPtr.Zero), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void EnableAttributes_Throws_IfCursesCallFails()
    {
        _cursesMock.Setup(s => s.wattr_on(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.EnableAttributes(VideoAttribute.Bold))
              .Operation.ShouldBe("wattr_on");
    }

    [TestMethod]
    public void DisableAttributes_CallsCurses()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DisableAttributes(VideoAttribute.Bold);

        _cursesMock.Verify(v => v.wattr_off(new(1), (uint) VideoAttribute.Bold, IntPtr.Zero), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DisableAttributes_Throws_IfCursesCallFails()
    {
        _cursesMock.Setup(s => s.wattr_off(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesOperationException>(() => w.DisableAttributes(VideoAttribute.Bold))
              .Operation.ShouldBe("wattr_off");
    }

    [TestMethod]
    public void TryMoveCaretTo_ReturnsFalse_IfCoordinatesOutsideWindow()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.TryMoveCaretTo(1, 1)
         .ShouldBeFalse();
    }

    [TestMethod]
    public void TryMoveCaretTo_ReturnsFalse_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.TryMoveCaretTo(1, 1)
         .ShouldBeFalse();
    }

    [TestMethod]
    public void TryMoveCaretTo_ReturnsTrue_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wmove(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(0);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.TryMoveCaretTo(1, 1)
         .ShouldBeTrue();
    }

    [TestMethod]
    public void MoveCaretTo_Throws_IfMovingFails()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.MoveCaretTo(1, 1));
    }

    [TestMethod]
    public void MoveCaretTo_Succeeds_IfMovingSucceeds()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.NotThrow(() => w.MoveCaretTo(1, 1));
    }

    [TestMethod]
    public void ScrollUp_Throws_IfLinesIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.ScrollUp(0));
    }

    [TestMethod]
    public void ScrollUp_Throws_IfLinesIsGreaterThanTheHeight()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.ScrollUp(11));
    }

    [TestMethod]
    public void ScrollUp_Throws_IfScrollingNotEnabled()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<NotSupportedException>(() => w.ScrollUp(1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ScrollUp_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.ScrollUp(1))
              .Operation.ShouldBe("wscrl");
    }

    [TestMethod]
    public void ScrollUp_Scrolls_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.ScrollUp(1);

        _cursesMock.Verify(s => s.wscrl(new(1), 1));
    }

    [TestMethod]
    public void ScrollDown_Throws_IfLinesIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.ScrollDown(0));
    }

    [TestMethod]
    public void ScrollDown_Throws_IfLinesIsGreaterThanTheHeight()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.ScrollDown(11));
    }

    [TestMethod]
    public void ScrollDown_Throws_IfScrollingNotEnabled()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(false);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<NotSupportedException>(() => w.ScrollDown(1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ScrollDown_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wscrl(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.ScrollDown(1))
              .Operation.ShouldBe("wscrl");
    }

    [TestMethod]
    public void ScrollDown_Scrolls_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(2);

        _cursesMock.Setup(s => s.is_scrollok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.ScrollDown(1);

        _cursesMock.Verify(s => s.wscrl(new(1), -1));
    }

    [TestMethod]
    public void InsertEmptyLines_Throws_IfLinesIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.InsertEmptyLines(0));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void InsertEmptyLines_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.InsertEmptyLines(1))
              .Operation.ShouldBe("winsdelln");
    }

    [TestMethod]
    public void InsertEmptyLines_AddsLines_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.InsertEmptyLines(1);

        _cursesMock.Verify(s => s.winsdelln(new(1), 1));
    }

    [TestMethod]
    public void DeleteLines_Throws_IfLinesIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.DeleteLines(0));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DeleteLines_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.winsdelln(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DeleteLines(1))
              .Operation.ShouldBe("winsdelln");
    }

    [TestMethod]
    public void DeleteLines_AddsLines_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DeleteLines(1);

        _cursesMock.Verify(s => s.winsdelln(new(1), -1));
    }

    [TestMethod]
    public void ChangeTextStyle_Throws_IfWidthIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.ChangeTextStyle(0, Style.Default));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ChangeTextStyle_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wchgat(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<ushort>(),
                       It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.ChangeTextStyle(1, Style.Default))
              .Operation.ShouldBe("wchgat");
    }

    [TestMethod]
    public void ChangeTextStyle_AddsLines_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.ChangeTextStyle(3, new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 22 } });

        _cursesMock.Verify(v => v.wchgat(new(1), 3, (uint) VideoAttribute.Bold, 22, IntPtr.Zero), Times.Once);
    }

    [TestMethod]
    public void DrawVerticalLine_Throws_IfWidthIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.DrawVerticalLine(0, new('a'), Style.Default));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawVerticalLine_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wvline_set(It.IsAny<IntPtr>(), It.IsAny<CursesComplexChar>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawVerticalLine(1, new('a'), Style.Default))
              .Operation.ShouldBe("wvline_set");
    }

    [TestMethod]
    public void DrawVerticalLine_DrawsLine_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawVerticalLine(3, new('a'),
            new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 22 } });

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                It.IsAny<ushort>(), IntPtr.Zero), Times.Once);

        _cursesMock.Verify(v => v.wvline_set(new(1), It.IsAny<CursesComplexChar>(), 3), Times.Once);
    }

    [TestMethod]
    public void DrawVerticalLine2_Throws_IfWidthIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.DrawVerticalLine(0));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawVerticalLine2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wvline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawVerticalLine(1))
              .Operation.ShouldBe("wvline");
    }

    [TestMethod]
    public void DrawVerticalLine2_DrawsLine_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawVerticalLine(3);

        _cursesMock.Verify(v => v.wvline(new(1), 0, 3), Times.Once);
    }

    [TestMethod]
    public void DrawHorizontalLine_Throws_IfWidthIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.DrawHorizontalLine(0, new('a'), Style.Default));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawHorizontalLine_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.whline_set(It.IsAny<IntPtr>(), It.IsAny<CursesComplexChar>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawHorizontalLine(1, new('a'), Style.Default))
              .Operation.ShouldBe("whline_set");
    }

    [TestMethod]
    public void DrawHorizontalLine_DrawsLine_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawHorizontalLine(3, new('a'),
            new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 22 } });

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                It.IsAny<ushort>(), IntPtr.Zero), Times.Once);

        _cursesMock.Verify(v => v.whline_set(new(1), It.IsAny<CursesComplexChar>(), 3), Times.Once);
    }

    [TestMethod]
    public void DrawHorizontalLine2_Throws_IfWidthIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentException>(() => w.DrawHorizontalLine(0));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawHorizontalLine2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.whline(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawHorizontalLine(1))
              .Operation.ShouldBe("whline");
    }

    [TestMethod]
    public void DrawHorizontalLine2_DrawsLine_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawHorizontalLine(3);

        _cursesMock.Verify(v => v.whline(new(1), 0, 3), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawBorder_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wborder_set(It.IsAny<IntPtr>(), It.IsAny<CursesComplexChar>(),
                       It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(),
                       It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(),
                       It.IsAny<CursesComplexChar>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawBorder(new('a'), new('a'), new('a'), new('a'), new('a'),
                  new('a'), new('a'), new('a'), Style.Default))
              .Operation.ShouldBe("wborder_set");
    }

    [TestMethod]
    public void DrawBorder_DrawsBorder_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawBorder(new('a'), new('a'), new('a'), new('a'), new('a'),
            new('a'), new('a'), new('a'), Style.Default);

        _cursesMock.Verify(
            s => s.setcchar(out It.Ref<CursesComplexChar>.IsAny, It.IsAny<string>(), It.IsAny<uint>(),
                It.IsAny<ushort>(), IntPtr.Zero), Times.Exactly(8));

        _cursesMock.Verify(
            s => s.wborder_set(new(1), It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(),
                It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(),
                It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>(), It.IsAny<CursesComplexChar>()),
            Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void DrawBorder2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wborder(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(),
                       It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.DrawBorder())
              .Operation.ShouldBe("wborder");
    }

    [TestMethod]
    public void DrawBorder2_DrawsBorder_IfCursesSucceeds()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.DrawBorder();

        _cursesMock.Verify(s => s.wborder(new(1), 0, 0, 0, 0,
            0, 0, 0, 0), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Refresh_AsksCursesForEntireScreenRefresh_BasedOnEntireScreen(bool enable)
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Refresh(enable, true);

        _cursesMock.Verify(v => v.clearok(new(1), enable), Times.AtMost(2));
    }

    [TestMethod]
    public void Refresh_AsksCursesForQueue_IfBatchIsTrue()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Refresh(true, false);

        _cursesMock.Verify(v => v.wnoutrefresh(new(1)), Times.Once);
    }

    [TestMethod]
    public void Refresh_AsksCursesForRefresh_IfBatchIsFalse()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Refresh(false, false);

        _cursesMock.Verify(v => v.wrefresh(new(1)), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(s => s.clearok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(false, false))
              .Operation.ShouldBe("clearok");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(s => s.wrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(false, false))
              .Operation.ShouldBe("wrefresh");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh_Throws_IfCursesFails_3()
    {
        _cursesMock.Setup(s => s.wnoutrefresh(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(true, false))
              .Operation.ShouldBe("wnoutrefresh");
    }

    [TestMethod]
    public void Refresh2_Throws_IfYIsNegative()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(-1, 1));
    }

    [TestMethod]
    public void Refresh2_Throws_IfYIsOutsideBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(10, 1));
    }

    [TestMethod]
    public void Refresh2_Throws_IfCountIsLessThanOne()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(0, 0));
    }

    [TestMethod]
    public void Refresh2_Throws_IfCountGreaterThanBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(1, 10));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Refresh2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _cursesMock.Setup(s => s.wredrawln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Refresh(1, 1))
              .Operation.ShouldBe("wredrawln");
    }

    [TestMethod]
    public void Refresh2_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.NotThrow(() => w.Refresh(1, 9));
    }

    [TestMethod]
    public void Refresh3_AsksCursesForRefresh()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Refresh();

        _cursesMock.Verify(v => v.clearok(w.Handle, false), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(w.Handle), Times.Once);
    }

    [TestMethod]
    public void IsLineDirty_Throws_IfLineIsNegative()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.IsLineDirty(-1));
    }

    [TestMethod]
    public void IsLineDirty_Throws_IfYIsOutsideBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.IsLineDirty(10));
    }

    [TestMethod]
    public void IsLineDirty_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _cursesMock.Setup(s => s.is_linetouched(It.IsAny<IntPtr>(), It.IsAny<int>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.IsLineDirty(1)
         .ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void IsPointWithin_ReturnsTrue_IfCursesSaysSo(bool yes)
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(yes);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.IsPointWithin(new(100, 100))
         .ShouldBe(yes);
    }

    [TestMethod, DataRow(true, true), DataRow(true, false), DataRow(false, true), DataRow(false, false)]
    public void IsIsRectangleWithin_AsksCursesTwice(bool yes1, bool yes2)
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), 0, 0))
                   .Returns(yes1);

        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), 4, 4))
                   .Returns(yes2);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.IsRectangleWithin(new(0, 0, 5, 5))
         .ShouldBe(yes1 && yes2);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Invalidate_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _cursesMock.Setup(s => s.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), 1))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Invalidate())
              .Operation.ShouldBe("wtouchln");
    }

    [TestMethod]
    public void Invalidate_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.NotThrow(() => w.Invalidate());
    }

    [TestMethod]
    public void Invalidate2_Throws_IfYIsNegative()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Refresh(-1, 1));
    }

    [TestMethod]
    public void Invalidate2_Throws_IfYIsOutsideBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Invalidate(10, 1));
    }

    [TestMethod]
    public void Invalidate2_Throws_IfCountIsLessThanOne()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Invalidate(0, 0));
    }

    [TestMethod]
    public void Invalidate2_Throws_IfCountGreaterThanBounds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.Invalidate(1, 10));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Invalidate2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        _cursesMock.Setup(s => s.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), 1))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Invalidate(1, 1))
              .Operation.ShouldBe("wtouchln");
    }

    [TestMethod]
    public void Invalidate2_Succeeds_IfCursesSucceeds()
    {
        _cursesMock.Setup(s => s.getmaxy(It.IsAny<IntPtr>()))
                   .Returns(10);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.NotThrow(() => w.Invalidate(1, 9));
    }

    [TestMethod]
    public void Clear_AsksCurses_1()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Clear(ClearStrategy.Full);

        _cursesMock.Verify(v => v.werase(new(1)), Times.Once);
    }

    [TestMethod]
    public void Clear_AsksCurses_2()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Clear(ClearStrategy.LineFromCaret);

        _cursesMock.Verify(v => v.wclrtoeol(new(1)), Times.Once);
    }

    [TestMethod]
    public void Clear_AsksCurses_3()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Clear(ClearStrategy.FullFromCaret);

        _cursesMock.Verify(v => v.wclrtobot(new(1)), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_1()
    {
        _cursesMock.Setup(v => v.werase(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Clear(ClearStrategy.Full))
              .Operation.ShouldBe("werase");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_2()
    {
        _cursesMock.Setup(v => v.wclrtoeol(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Clear(ClearStrategy.LineFromCaret))
              .Operation.ShouldBe("wclrtoeol");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Clear_Throws_IfCursesFails_3()
    {
        _cursesMock.Setup(v => v.wclrtobot(It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.Clear(ClearStrategy.FullFromCaret))
              .Operation.ShouldBe("wclrtobot");
    }

    [TestMethod]
    public void WriteText_Throws_IfStringIsNull()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentNullException>(() => w.WriteText(null!, Style.Default));
    }

    [TestMethod]
    public void WriteText_DoesNotCallCurse_IfEmptyString()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.WriteText("", Style.Default);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<CursesComplexChar>()), Times.Never);
    }

    [TestMethod]
    public void WriteText_CallsCursesForEachChar()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.WriteText("12345", Style.Default);

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<CursesComplexChar>()), Times.Exactly(5));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void WriteText_Succeeds_IfCursesNotFailedForAllChars()
    {
        var count = 0;
        _cursesMock.Setup(s => s.wadd_wch(new(1), It.IsAny<CursesComplexChar>()))
                   .Returns((IntPtr _, CursesComplexChar _) => count++ < 3 ? -1 : 0);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.WriteText("12345", Style.Default);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void WriteText_Throws_IfNoCharacterGotWritten()
    {
        _cursesMock.Setup(s => s.wadd_wch(new(1), It.IsAny<CursesComplexChar>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.WriteText("12345", Style.Default))
              .Operation.ShouldBe("wadd_wch");
    }

    [TestMethod]
    public void WriteText2_CallsCursesAlso()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.WriteText("12345");

        _cursesMock.Verify(v => v.wadd_wch(new(1), It.IsAny<CursesComplexChar>()), Times.Exactly(5));
    }

    [TestMethod]
    public void RemoveText_Throws_IfCountIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.RemoveText(0));
    }

    [TestMethod]
    public void RemoveText_CallsCurses_ForCountTimes()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.RemoveText(10);

        _cursesMock.Verify(v => v.wdelch(new(1)), Times.Exactly(10));
    }

    [TestMethod]
    public void RemoveText_CallsCurses_UntilTheFirstFailure()
    {
        var count = 0;
        _cursesMock.Setup(s => s.wdelch(It.IsAny<IntPtr>()))
                   .Returns((IntPtr _) => count++ < 5 ? 0 : -1);

        var w = new Window(_cursesMock.Object, null, new(1));
        w.RemoveText(10);

        _cursesMock.Verify(v => v.wdelch(new(1)), Times.Exactly(6));
    }

    [TestMethod]
    public void GetText_Throws_IfCountIsLessThanOne()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentOutOfRangeException>(() => w.GetText(0));
    }

    [TestMethod]
    public void GetText_AsksCurses_WithTheGivenCount()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(20);

        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny, IntPtr.Zero))
                   .Returns((CursesComplexChar _, StringBuilder sb, out uint a, out ushort cp,
                       IntPtr _) =>
                   {
                       sb.Append('a');
                       a = 0;
                       cp = 0;

                       return 0;
                   });

        var w = new Window(_cursesMock.Object, null, new(1));
        w.GetText(15)
         .Length.ShouldBe(15);

        _cursesMock.Verify(v => v.win_wchnstr(new(1), It.IsAny<CursesComplexChar[]>(), 15), Times.Once);
    }

    [TestMethod]
    public void GetText_AsksCurses_WithTheDeltaCount_IfNotEnoughLength()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(20);

        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny, IntPtr.Zero))
                   .Returns((CursesComplexChar _, StringBuilder sb, out uint a, out ushort cp,
                       IntPtr _) =>
                   {
                       sb.Append('a');
                       a = 0;
                       cp = 0;

                       return 0;
                   });

        var w = new Window(_cursesMock.Object, null, new(1));
        w.GetText(50)
         .Length.ShouldBe(20);

        _cursesMock.Verify(v => v.win_wchnstr(new(1), It.IsAny<CursesComplexChar[]>(), 20), Times.Once);
    }

    [TestMethod]
    public void GetText_GetsContents_FromCurses()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(20);

        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(0);

        var ch = 'a';
        _cursesMock.Setup(s => s.getcchar(It.IsAny<CursesComplexChar>(), It.IsAny<StringBuilder>(),
                       out It.Ref<uint>.IsAny, out It.Ref<ushort>.IsAny, IntPtr.Zero))
                   .Returns((CursesComplexChar _, StringBuilder sb, out uint a, out ushort cp,
                       IntPtr _) =>
                   {
                       sb.Append(ch);
                       a = ch;
                       cp = ch;

                       ch++;
                       return 0;
                   });

        var w = new Window(_cursesMock.Object, null, new(1));
        var chars = w.GetText(3);

        chars[0]
            .@char.ShouldBe(new('a'));

        chars[0]
            .style.Attributes.ShouldBe((VideoAttribute) 'a');

        chars[0]
            .style.ColorMixture.ShouldBe(new() { Handle = 'a' });

        chars[1]
            .@char.ShouldBe(new('b'));

        chars[1]
            .style.Attributes.ShouldBe((VideoAttribute) 'b');

        chars[1]
            .style.ColorMixture.ShouldBe(new() { Handle = 'b' });

        chars[2]
            .@char.ShouldBe(new('c'));

        chars[2]
            .style.Attributes.ShouldBe((VideoAttribute) 'c');

        chars[2]
            .style.ColorMixture.ShouldBe(new() { Handle = 'c' });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void GetText_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.getmaxx(It.IsAny<IntPtr>()))
                   .Returns(20);

        _cursesMock.Setup(s => s.getcurx(It.IsAny<IntPtr>()))
                   .Returns(0);

        _cursesMock.Setup(s => s.win_wchnstr(It.IsAny<IntPtr>(), It.IsAny<CursesComplexChar[]>(), It.IsAny<int>()))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w.GetText(1))
              .Operation.ShouldBe("win_wchnstr");
    }

    [TestMethod]
    public void Replace_Throws_IfWindowIsNull()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentNullException>(() => w.Replace(null!, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Throws_IfWindowIsItself()
    {
        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<ArgumentException>(() => w.Replace(w, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Throws_IfWindowIsDescendant()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        var w3 = new Window(_cursesMock.Object, w2, new(3));

        Should.Throw<ArgumentException>(() => w1.Replace(w3, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Throws_IfWindowIsAncestor()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        var w3 = new Window(_cursesMock.Object, w2, new(3));

        Should.Throw<ArgumentException>(() => w3.Replace(w1, ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace_Succeeds_IfCursesSucceeds_Overlay()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        w1.Replace(w2, ReplaceStrategy.Overlay);

        _cursesMock.Verify(v => v.overlay(new(1), new(2)), Times.Once);
        _cursesMock.Verify(v => v.overwrite(new(1), new(2)), Times.Never);
    }

    [TestMethod]
    public void Replace_Succeeds_IfCursesSucceeds_Overwrite()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        w1.Replace(w2, ReplaceStrategy.Overwrite);

        _cursesMock.Verify(v => v.overlay(new(1), new(2)), Times.Never);
        _cursesMock.Verify(v => v.overwrite(new(1), new(2)), Times.Once);
    }

    [TestMethod]
    public void Replace_Throws_IfCursesFails_Overlay()
    {
        _cursesMock.Setup(v => v.overlay(It.IsAny<IntPtr>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        Should.Throw<CursesOperationException>(() => w1.Replace(w2, ReplaceStrategy.Overlay))
              .Operation.ShouldBe("overlay");
    }

    [TestMethod]
    public void Replace_Throws_IfCursesFails_Overwrite()
    {
        _cursesMock.Setup(v => v.overwrite(It.IsAny<IntPtr>(), It.IsAny<IntPtr>()))
                   .Returns(-1);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        Should.Throw<CursesOperationException>(() => w1.Replace(w2, ReplaceStrategy.Overwrite))
              .Operation.ShouldBe("overwrite");
    }

    [TestMethod]
    public void Replace2_Throws_IfWindowIsNull()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<ArgumentNullException>(() =>
            w.Replace(null!, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_Throws_IfWindowIsItself()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<ArgumentException>(() => w.Replace(w, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_Throws_IfWindowIsDescendant()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        var w3 = new Window(_cursesMock.Object, w2, new(3));

        Should.Throw<ArgumentException>(() => w1.Replace(w3, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_Throws_IfWindowIsAncestor()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        var w3 = new Window(_cursesMock.Object, w2, new(3));

        Should.Throw<ArgumentException>(() => w3.Replace(w1, new(0, 0, 1, 1), new(0, 0), ReplaceStrategy.Overlay));
    }

    [TestMethod]
    public void Replace2_Throws_IfTheSourceRectIsOutsideTheBounds()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            w1.Replace(w2, new(1, 1, 5, 5), new(0, 0), ReplaceStrategy.Overlay);
        });
    }

    [TestMethod]
    public void Replace2_Throws_IfTheDestinationAreaIsOutsideTheBounds()
    {
        _cursesMock.Setup(s => s.wenclose(new(1), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.wenclose(new(2), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(false);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            w1.Replace(w2, new(0, 0, 5, 5), new(6, 6), ReplaceStrategy.Overlay);
        });
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        _cursesMock.Setup(s => s.copywin(It.IsAny<IntPtr>(), It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(),
                       It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(-1);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        Should.Throw<CursesOperationException>(() => { w1.Replace(w2, new(1, 1, 5, 5), new(0, 0), ReplaceStrategy.Overlay); })
              .Operation.ShouldBe("copywin");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_CallsCurses_IfCursesOverlay()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        w1.Replace(w2, new(1, 2, 3, 4), new(5, 6), ReplaceStrategy.Overlay);
        _cursesMock.Verify(s => s.copywin(new(1), new(2), 2, 1, 6,
            5, 9, 9, 1));
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Replace2_CallsCurses_IfCursesOverwrite()
    {
        _cursesMock.Setup(s => s.wenclose(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(true);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, null, new(2));

        w1.Replace(w2, new(1, 2, 3, 4), new(5, 6), ReplaceStrategy.Overwrite);
        _cursesMock.Verify(s => s.copywin(new(1), new(2), 2, 1, 6,
            5, 9, 9, 0));
    }

    [TestMethod]
    public void IsRelatedTo_Throws_IfWindowIsNull()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<ArgumentNullException>(() => { w1.IsRelatedTo(null!); });
    }

    [TestMethod]
    public void IsRelatedTo_ReturnsTrue_IfWindowsAreRelated()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        var w3 = new Window(_cursesMock.Object, w2, new(3));

        w1.IsRelatedTo(w1)
          .ShouldBeTrue();

        w1.IsRelatedTo(w2)
          .ShouldBeTrue();

        w1.IsRelatedTo(w3)
          .ShouldBeTrue();

        w2.IsRelatedTo(w1)
          .ShouldBeTrue();

        w2.IsRelatedTo(w3)
          .ShouldBeTrue();

        w3.IsRelatedTo(w1)
          .ShouldBeTrue();

        w3.IsRelatedTo(w2)
          .ShouldBeTrue();
    }

    [TestMethod]
    public void IgnoreHardwareCaret_Get_ReturnsWhatCursesSays()
    {
        _cursesMock.Setup(s => s.is_leaveok(It.IsAny<IntPtr>()))
                   .Returns(true);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        w1.IgnoreHardwareCaret.ShouldBeTrue();
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void IgnoreHardwareCaret_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.leaveok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        var w1 = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesOperationException>(() => w1.IgnoreHardwareCaret = true)
              .Operation.ShouldBe("leaveok");
    }

    [TestMethod]
    public void IgnoreHardwareCaret_Set_UpdatesWindowAndChildren()
    {
        var w1 = new Window(_cursesMock.Object, null, new(1));
        var w2 = new Window(_cursesMock.Object, w1, new(2));
        w1.IgnoreHardwareCaret = true;

        _cursesMock.Verify(v => v.leaveok(w1.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.leaveok(w2.Handle, true), Times.Once);
    }
}
