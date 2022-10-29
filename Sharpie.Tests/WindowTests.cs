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

using System.Text;
using Curses;

[TestClass]
public class WindowTests
{
    private Mock<ICursesProvider> _cursesMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
    }

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

        Should.Throw<CursesException>(() => new Window(_cursesMock.Object, null, new(1))).Operation.ShouldBe("keypad");
    }
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_2()
    {
        _cursesMock.Setup(s => s.notimeout(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => new Window(_cursesMock.Object, null, new(1))).Operation.ShouldBe("notimeout");
    }
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_3()
    {
        _cursesMock.Setup(s => s.nodelay(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => new Window(_cursesMock.Object, null, new(1))).Operation.ShouldBe("nodelay");
    }
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_4()
    {
        _cursesMock.Setup(s => s.syncok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => new Window(_cursesMock.Object, null, new(1))).Operation.ShouldBe("syncok");
    }
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfConfigureWindow_FailsInCurses_5()
    {
        _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => new Window(_cursesMock.Object, null, new(1))).Operation.ShouldBe("scrollok");
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
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void EnableScrolling_Throws_IfCursesFails()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        
        _cursesMock.Setup(s => s.scrollok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesException>(() => w.EnableScrolling = false).Operation.ShouldBe("scrollok");
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
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void UseHardwareLineEdit_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.has_il())
                   .Returns(true);
        
        _cursesMock.Setup(s => s.idlok(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);
        
        var w = new Window(_cursesMock.Object, null, new(1));
        
        Should.Throw<CursesException>(() => w.UseHardwareLineEdit = false).Operation.ShouldBe("idlok");
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

    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Get_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wattr_get(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny,  out It.Ref<ushort>.IsAny, IntPtr.Zero))
                   .Returns(-1);
        
        var w = new Window(_cursesMock.Object, null, new(1));
        
        Should.Throw<CursesException>(() => w.Style).Operation.ShouldBe("wattr_get");
    }
    
    [TestMethod]
    public void Style_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Style = new() { Attributes = VideoAttribute.Italic, ColorMixture = new() { Handle = 22 } };

        _cursesMock.Verify(v => v.wattr_set(It.IsAny<IntPtr>(), (uint)VideoAttribute.Italic, 22,
            IntPtr.Zero));
    }
      
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Style_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wattr_set(It.IsAny<IntPtr>(),  It.IsAny<uint>(), It.IsAny<ushort>(), IntPtr.Zero))
                   .Returns(-1);
        
        var w = new Window(_cursesMock.Object, null, new(1));
        
        Should.Throw<CursesException>(() => w.Style = Style.Default).Operation.ShouldBe("wattr_set");
    }
    
    [TestMethod]
    public void ColorMixture_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.ColorMixture = new() { Handle = 22 };

        _cursesMock.Verify(v => v.wcolor_set(It.IsAny<IntPtr>(), 22,
            IntPtr.Zero));
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void ColorMixture_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wcolor_set(It.IsAny<IntPtr>(), It.IsAny<ushort>(), IntPtr.Zero))
                   .Returns(-1);

        var w = new Window(_cursesMock.Object, null, new(1));

        Should.Throw<CursesException>(() => w.ColorMixture = ColorMixture.Default)
              .Operation.ShouldBe("wcolor_set");
    }
    
    [TestMethod]
    public void Background_Get_Returns_IfCursesSucceeded()
    {
        _cursesMock.Setup(s => s.getcchar(It.IsAny<ComplexChar>(), It.IsAny<StringBuilder>(), out It.Ref<uint>.IsAny,
                       out It.Ref<ushort>.IsAny, It.IsAny<IntPtr>()))
                   .Returns((ComplexChar _, StringBuilder sb, out uint attrs, out ushort colorPair,
                       IntPtr _) =>
                   {
                       sb.Append('H');
                       attrs = (uint) VideoAttribute.Dim;
                       colorPair = 10;
                       return 0;
                   });
        
        var w = new Window(_cursesMock.Object, null, new(1));
        var bk = w.Background;
        bk.style.ShouldBe(new()
        {
            Attributes = VideoAttribute.Dim,
            ColorMixture = new() { Handle = 10 }
        });
        bk.@char.ShouldBe(new('H'));

        _cursesMock.Verify(v => v.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<ComplexChar>.IsAny), Times.Once);
    }
    
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Get_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wgetbkgrnd(It.IsAny<IntPtr>(), out It.Ref<ComplexChar>.IsAny))
                   .Returns(-1);
        
        var w = new Window(_cursesMock.Object, null, new(1));
        Should.Throw<CursesException>(() => w.Background).Operation.ShouldBe("wgetbkgrnd");
    }
    
    [TestMethod]
    public void Background_Set_SetsValue_IfCursesSucceeded()
    {
        var w = new Window(_cursesMock.Object, null, new(1));
        w.Background = (new('a'),
            new() { Attributes = VideoAttribute.Blink, ColorMixture = new() { Handle = 22 } });

        _cursesMock.Verify(
            v => v.setcchar(out It.Ref<ComplexChar>.IsAny, "a", (uint)VideoAttribute.Blink, 22,
                It.IsAny<IntPtr>()), Times.Once);
        _cursesMock.Verify(v => v.wbkgrnd(new(1), It.IsAny<ComplexChar>()));
    }
      
    [TestMethod]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Background_Set_Throws_IfCursesFails()
    {
        _cursesMock.Setup(s => s.wbkgrnd(It.IsAny<IntPtr>(),  It.IsAny<ComplexChar>()))
                   .Returns(-1);
        
        var w = new Window(_cursesMock.Object, null, new(1));
        
        Should.Throw<CursesException>(() => w.Background = (new('a'), Style.Default)).Operation.ShouldBe("wbkgrnd");
    }
}
