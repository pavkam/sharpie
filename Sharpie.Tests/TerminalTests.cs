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
public class TerminalTests
{
    private readonly TerminalOptions _settings = new(ManualFlush: true);
    private Mock<ICursesProvider> _cursesMock = null!;
    private Terminal? _terminal;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(1));

        _terminal = null;
    }

    [TestCleanup] public void TestCleanup() { _terminal?.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfAnotherTerminalInstanceIsAlive()
    {
        _terminal = new(_cursesMock.Object, new());
        Should.Throw<InvalidOperationException>(() => new Terminal(_cursesMock.Object, new()));
    }

    [TestMethod]
    public void Ctor_Throws_IfCursesIsNull() { Should.Throw<ArgumentNullException>(() => new Terminal(null!, new())); }

    [TestMethod]
    public void Ctor_Throws_IfMouseClickIntervalNegative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Terminal(_cursesMock.Object, new(MouseClickInterval: -1)));
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_SetsTheApplicationLocale(bool enable)
    {
        _terminal = new(_cursesMock.Object, new());

        _cursesMock.Verify(v => v.set_unicode_locale());
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_NotifiesCurses_AboutUseEnvironmentOverrides(bool enable)
    {
        _terminal = new(_cursesMock.Object, new(UseEnvironmentOverrides: enable));

        _cursesMock.Verify(v => v.use_env(enable));
    }

    [TestMethod]
    public void Ctor_CreatesNewScreen_ByAskingCurses()
    {
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(10));

        _terminal = new(_cursesMock.Object, new());
        _terminal.Screen.Handle.ShouldBe(new(10));
    }

    [TestMethod]
    public void Ctor_InitializesScreenIgnoreHardwareCaret_IfCaretDisabled()
    {
        _terminal = new(_cursesMock.Object, new(CaretMode: CaretMode.Invisible));

        _cursesMock.Verify(v => v.leaveok(_terminal.Screen.Handle, true), Times.Once);
    }

    [TestMethod]
    public void Ctor_InitializesScreenIgnoreHardwareCaret_IfCaretEnabled()
    {
        _terminal = new(_cursesMock.Object, new(CaretMode: CaretMode.Visible));
        _cursesMock.Verify(v => v.leaveok(_terminal.Screen.Handle, false), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfCursesFails_WhenCreatingScreen()
    {
        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(0));

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new()))
              .Operation.ShouldBe("initscr");
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_Initializes_ColorManager(bool enabled)
    {
        _cursesMock.Setup(s => s.has_colors())
                   .Returns(true);

        _terminal = new(_cursesMock.Object, new(enabled));

        _terminal.Colors.Enabled.ShouldBe(enabled);
        ((ITerminal) _terminal).Colors.ShouldBe(_terminal.Colors);
    }

    [TestMethod, DataRow(SoftLabelKeyMode.Disabled), DataRow(SoftLabelKeyMode.ThreeTwoThree)]
    public void Ctor_Initializes_SoftLabelKeyManager(SoftLabelKeyMode mode)
    {
        _terminal = new(_cursesMock.Object, new(SoftLabelKeyMode: mode));

        _terminal.SoftLabelKeys.Enabled.ShouldBe(mode != SoftLabelKeyMode.Disabled);
        ((ITerminal) _terminal).SoftLabelKeys.ShouldBe(_terminal.SoftLabelKeys);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_PreparesManualFlushMode_ByAskingCurses(bool enabled)
    {
        _terminal = new(_cursesMock.Object, new(ManualFlush: enabled));
        _cursesMock.Verify(v => v.intrflush(IntPtr.Zero, enabled), Times.Once);
        _cursesMock.Verify(v => v.qiflush(), enabled ? Times.Never : Times.Once);
        _cursesMock.Verify(v => v.noqiflush(), enabled ? Times.Once : Times.Never);
    }

    [TestMethod, DataRow(true), DataRow(false), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_IfCursesFails_WhenSettingUpManualFlush(bool enabled)
    {
        _cursesMock.Setup(v => v.intrflush(IntPtr.Zero, It.IsAny<bool>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new()))
              .Operation.ShouldBe("intrflush");
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_PreparesEchoInput_ByAskingCurses(bool enabled)
    {
        _terminal = new(_cursesMock.Object, new(EchoInput: enabled));
        _cursesMock.Verify(v => v.echo(), enabled ? Times.Once : Times.Never);
        _cursesMock.Verify(v => v.noecho(), enabled ? Times.Never : Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPreparesEchoInput(bool enabled)
    {
        _cursesMock.Setup(s => s.echo())
                   .Returns(-1);

        _cursesMock.Setup(s => s.noecho())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(EchoInput: enabled)))
              .Operation.ShouldBe(enabled ? "echo" : "noecho");
    }

    [TestMethod]
    public void Ctor_PreparesTranslateReturnToNewLineChar_ByAskingCurses()
    {
        _terminal = new(_cursesMock.Object, new());

        _cursesMock.Verify(v => v.nl(), Times.Never);
        _cursesMock.Verify(v => v.nonl(), Times.Once);
    }

    [TestMethod]
    public void Ctor_PreparesHeader_ByAskingCurses()
    {
        _cursesMock.Setup(s => s.ripoffline(1, It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Callback((int _, ICursesProvider.ripoffline_callback cb) =>
                   {
                       cb(new(100), 1)
                           .ShouldBe(0);
                   });

        _terminal = new(_cursesMock.Object, new(AllocateHeader: true));

        _terminal.Header.ShouldNotBeNull();
        _terminal.Header!.Handle.ShouldBe(new(100));
        ((ITerminal) _terminal).Header.ShouldBe(_terminal.Header);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenPreparingHeader_IfCursesFails()
    {
        _cursesMock.Setup(s => s.ripoffline(It.IsAny<int>(), It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(AllocateHeader: true)))
              .Operation.ShouldBe("ripoffline");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenPreparingHeader_IfCursesFailsToProvideValidHandle()
    {
        _cursesMock.Setup(s => s.ripoffline(It.IsAny<int>(), It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Callback((int _, ICursesProvider.ripoffline_callback cb) => { cb(IntPtr.Zero, 0); });

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(AllocateHeader: true)))
              .Operation.ShouldBe("ripoffline");
    }

    [TestMethod]
    public void Ctor_PreparesFooter_ByAskingCurses()
    {
        _cursesMock.Setup(s => s.ripoffline(-1, It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Callback((int _, ICursesProvider.ripoffline_callback cb) =>
                   {
                       cb(new(100), 1)
                           .ShouldBe(0);
                   });

        _terminal = new(_cursesMock.Object, new(AllocateFooter: true));

        _terminal.Footer.ShouldNotBeNull();
        _terminal.Footer!.Handle.ShouldBe(new(100));
        ((ITerminal) _terminal).Footer.ShouldBe(_terminal.Footer);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenPreparingFooter_IfCursesFails()
    {
        _cursesMock.Setup(s => s.ripoffline(It.IsAny<int>(), It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(AllocateFooter: true)))
              .Operation.ShouldBe("ripoffline");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenPreparingFooter_IfCursesFailsToProvideValidHandle()
    {
        _cursesMock.Setup(s => s.ripoffline(It.IsAny<int>(), It.IsAny<ICursesProvider.ripoffline_callback>()))
                   .Callback((int _, ICursesProvider.ripoffline_callback cb) => { cb(IntPtr.Zero, 0); });

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(AllocateFooter: true)))
              .Operation.ShouldBe("ripoffline");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPreparesTranslateReturnToNewLineChar()
    {
        _cursesMock.Setup(s => s.nonl())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new()))
              .Operation.ShouldBe("nonl");
    }

    [TestMethod]
    public void Ctor_PreparesCaretMode_ByAskingCurses()
    {
        _terminal = new(_cursesMock.Object, new(CaretMode: CaretMode.VeryVisible));

        _cursesMock.Verify(v => v.curs_set((int) CaretMode.VeryVisible), Times.Once);
    }

    [TestMethod, DataRow(true), DataRow(false), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPreparesCaretMode(bool enabled)
    {
        _cursesMock.Setup(s => s.curs_set(It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() =>
                  new Terminal(_cursesMock.Object, new(CaretMode: CaretMode.VeryVisible)))
              .Operation.ShouldBe("curs_set");
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void Ctor_PreparesUseMouse_ByAskingCurses(bool enabled)
    {
        _terminal = new(_cursesMock.Object, new(UseMouse: enabled, MouseClickInterval: 999));
        _terminal.Events.UseInternalMouseEventResolver.ShouldBeFalse();

        _cursesMock.Verify(v => v.mouseinterval(999), enabled ? Times.Once : Times.Never);

        var expMask = enabled
            ? (int) CursesMouseEvent.EventType.ReportPosition | (int) CursesMouseEvent.EventType.All
            : 0;

        _cursesMock.Verify(v => v.mousemask(expMask, out It.Ref<int>.IsAny), Times.Once);
    }

    [TestMethod]
    public void Ctor_PreparesUseMouse_WithoutClickInterval_ByAskingCurses()
    {
        _terminal = new(_cursesMock.Object, new(UseMouse: true, MouseClickInterval: null));
        _cursesMock.Verify(v => v.mouseinterval(0), Times.Once);

        _terminal.Events.UseInternalMouseEventResolver.ShouldBeTrue();
    }

    [TestMethod, DataRow(true), DataRow(false), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPreparesUseMouse_1(bool enabled)
    {
        _cursesMock.Setup(s => s.mousemask(It.IsAny<int>(), out It.Ref<int>.IsAny))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(UseMouse: enabled)))
              .Operation.ShouldBe("mousemask");
    }

    [TestMethod, DataRow(true), DataRow(false), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPreparesUseMouse_2(bool enabled)
    {
        _cursesMock.Setup(s => s.mouseinterval(It.IsAny<int>()))
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(UseMouse: enabled)))
              .Operation.ShouldBe("mouseinterval");
    }

    [TestMethod, DataRow(false, false), DataRow(false, true), DataRow(true, false), DataRow(true, true)]
    public void Ctor_PreparesInput_ByAskingCurses(bool useInputBuffering, bool suppressControlKeys)
    {
        _terminal = new(_cursesMock.Object,
            new(UseInputBuffering: useInputBuffering, SuppressControlKeys: suppressControlKeys));

        _cursesMock.Verify(v => v.noraw(), useInputBuffering ? Times.Once : Times.Never);
        _cursesMock.Verify(v => v.nocbreak(), useInputBuffering ? Times.Once : Times.Never);
        _cursesMock.Verify(v => v.raw(), !useInputBuffering && suppressControlKeys ? Times.Once : Times.Never);
        _cursesMock.Verify(v => v.cbreak(), !useInputBuffering && !suppressControlKeys ? Times.Once : Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPrepareInput_1()
    {
        _cursesMock.Setup(s => s.noraw())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(UseInputBuffering: true)))
              .Operation.ShouldBe("noraw");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPrepareInput_2()
    {
        _cursesMock.Setup(s => s.nocbreak())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => new Terminal(_cursesMock.Object, new(UseInputBuffering: true)))
              .Operation.ShouldBe("nocbreak");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPrepareInput_3()
    {
        _cursesMock.Setup(s => s.raw())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() =>
                  new Terminal(_cursesMock.Object, new(UseInputBuffering: false, SuppressControlKeys: true)))
              .Operation.ShouldBe("raw");
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Ctor_Throws_WhenCursesFailsToPrepareInput_4()
    {
        _cursesMock.Setup(s => s.cbreak())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() =>
                  new Terminal(_cursesMock.Object, new(UseInputBuffering: false, SuppressControlKeys: false)))
              .Operation.ShouldBe("cbreak");
    }

    [TestMethod]
    public void Ctor_PreparesMeta_ByAskingCurses()
    {
        _terminal = new(_cursesMock.Object, new());

        _cursesMock.Verify(v => v.meta(IntPtr.Zero, false));
    }

    [TestMethod]
    public void Ctor_WillNotThrow_IfCursesFailsToPrepareMeta()
    {
        _cursesMock.Setup(v => v.meta(It.IsAny<IntPtr>(), It.IsAny<bool>()))
                   .Returns(-1);

        Should.NotThrow(() => { _terminal = new(_cursesMock.Object, new()); });
    }

    [TestMethod]
    public void Ctor_RegistersStandardKeySequenceResolvers_IfAsked()
    {
        _terminal = new(_cursesMock.Object, new());
        _terminal.Events.Uses(KeySequenceResolver.SpecialCharacterResolver)
                 .ShouldBeTrue();

        _terminal.Events.Uses(KeySequenceResolver.ControlKeyResolver)
                 .ShouldBeTrue();

        _terminal.Events.Uses(KeySequenceResolver.AltKeyResolver)
                 .ShouldBeTrue();

        _terminal.Events.Uses(KeySequenceResolver.KeyPadModifiersResolver)
                 .ShouldBeTrue();
    }

    [TestMethod]
    public void Ctor_DoesNotRegisterStandardKeySequenceResolvers_IfAsked()
    {
        _terminal = new(_cursesMock.Object, new(UseStandardKeySequenceResolvers: false));

        _terminal.Events.Uses(KeySequenceResolver.SpecialCharacterResolver)
                 .ShouldBeFalse();

        _terminal.Events.Uses(KeySequenceResolver.ControlKeyResolver)
                 .ShouldBeFalse();

        _terminal.Events.Uses(KeySequenceResolver.AltKeyResolver)
                 .ShouldBeFalse();

        _terminal.Events.Uses(KeySequenceResolver.KeyPadModifiersResolver)
                 .ShouldBeFalse();
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void BaudRate_Throws_IfCursesFails()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.baudrate())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _terminal.BaudRate.ShouldBe(999))
              .Operation.ShouldBe("baudrate");
    }

    [TestMethod]
    public void BaudRate_Returns_IfCursesSucceeds()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.baudrate())
                   .Returns(999);

        _terminal.BaudRate.ShouldBe(999);
    }

    [TestMethod]
    public void Colors_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _terminal.Dispose();
        Should.Throw<ObjectDisposedException>(() => _terminal.Colors);
    }

    [TestMethod]
    public void Colors_ReturnsTheColorManager_IfTerminalAlive()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Colors.ShouldNotBeNull();
    }

    [TestMethod]
    public void SoftLabelKeys_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _terminal.Dispose();
        Should.Throw<ObjectDisposedException>(() => _terminal.SoftLabelKeys);
    }

    [TestMethod]
    public void SoftLabelKeys_ReturnsTheColorManager_IfTerminalAlive()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.SoftLabelKeys.ShouldNotBeNull();
    }

    [TestMethod]
    public void Header_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();

        Should.Throw<ObjectDisposedException>(() => _terminal.Header);
    }

    [TestMethod]
    public void Footer_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();

        Should.Throw<ObjectDisposedException>(() => _terminal.Footer);
    }

    [TestMethod]
    public void Screen_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();
        Should.Throw<ObjectDisposedException>(() => _terminal.Screen);
    }

    [TestMethod]
    public void Screen_ReturnsTheInstance_IfTerminalAlive()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Screen.ShouldNotBeNull();
    }

    [TestMethod]
    public void Events_Throws_IfTerminalDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();
        Should.Throw<ObjectDisposedException>(() => _terminal.Events);
    }

    [TestMethod]
    public void Events_ReturnsTheInstance_IfTerminalAlive()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Events.ShouldNotBeNull();
        ((ITerminal) _terminal).Events.ShouldBe(_terminal.Events);
    }

    [TestMethod]
    public void Name_Returns_IfCursesSucceeds()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.termname())
                   .Returns("test");

        _terminal.Name.ShouldBe("test");
    }

    [TestMethod]
    public void Name_ReturnsNull_IfCursesReturnsNull()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.termname())
                   .Returns((string) null!);

        _terminal.Name.ShouldBe(null);
    }

    [TestMethod]
    public void Description_Returns_IfCursesSucceeds()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.longname())
                   .Returns("test");

        _terminal.Description.ShouldBe("test");
    }

    [TestMethod]
    public void Description_ReturnsNull_IfCursesReturnsNull()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.longname())
                   .Returns((string) null!);

        _terminal.Description.ShouldBe(null);
    }

    [TestMethod]
    public void Version_Returns_IfCursesSucceeds()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.curses_version())
                   .Returns("test");

        _terminal.CursesVersion.ShouldBe("test");
    }

    [TestMethod]
    public void CursesVersion_ReturnsNull_IfCursesReturnsNull()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.curses_version())
                   .Returns((string) null!);

        _terminal.CursesVersion.ShouldBe(null);
    }

    [TestMethod]
    public void SupportedAttributes_Returns_WhateverCursesReturns()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.term_attrs())
                   .Returns((uint) VideoAttribute.Italic);

        _terminal.SupportedAttributes.ShouldBe(VideoAttribute.Italic);
    }

    [TestMethod]
    public void HasHardwareLineEditor_Returns_WhateverCursesReturns()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.has_il())
                   .Returns(true);

        _terminal.HasHardwareLineEditor.ShouldBeTrue();
    }

    [TestMethod]
    public void HasHardwareCharEditor_Returns_WhateverCursesReturns()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.has_ic())
                   .Returns(true);

        _terminal.HasHardwareCharEditor.ShouldBeTrue();
    }

    [TestMethod]
    public void CurrentKillChar_ReturnsTheRune_IfCursesHasCharDefined()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.killwchar(out It.Ref<uint>.IsAny))
                   .Returns((out uint ch) =>
                   {
                       ch = 'A';
                       return 0;
                   });

        _terminal.CurrentKillChar.ShouldBe(new('A'));
    }

    [TestMethod]
    public void CurrentKillChar_ReturnsTheNull_IfCursesHasNoCharDefined()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.killwchar(out It.Ref<uint>.IsAny))
                   .Returns((out uint ch) =>
                   {
                       ch = 0;
                       return -1;
                   });

        _terminal.CurrentKillChar.ShouldBeNull();
    }

    [TestMethod]
    public void CurrentEraseChar_ReturnsTheRune_IfCursesHasCharDefined()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.erasewchar(out It.Ref<uint>.IsAny))
                   .Returns((out uint ch) =>
                   {
                       ch = 'A';
                       return 0;
                   });

        _terminal.CurrentEraseChar.ShouldBe(new('A'));
    }

    [TestMethod]
    public void CurrentEraseChar_ReturnsTheNull_IfCursesHasNoCharDefined()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.erasewchar(out It.Ref<uint>.IsAny))
                   .Returns((out uint ch) =>
                   {
                       ch = 0;
                       return -1;
                   });

        _terminal.CurrentEraseChar.ShouldBeNull();
    }

    [TestMethod]
    public void Curses_ReturnsTheCursesBackend()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Curses.ShouldBe(_cursesMock.Object);
    }

    [TestMethod]
    public void Alert_ShouldDoCursesBeep_IfSpecified()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Alert(false);

        _cursesMock.Verify(v => v.beep(), Times.Once);
    }

    [TestMethod]
    public void Alert_Throws_IfCursesBeepFails()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _cursesMock.Setup(s => s.beep())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _terminal.Alert(false))
              .Operation.ShouldBe("beep");
    }

    [TestMethod]
    public void Alert_ShouldDoCursesFlash_IfSpecified()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Alert(true);

        _cursesMock.Verify(v => v.flash(), Times.Once);
    }

    [TestMethod]
    public void Alert_Throws_IfCursesFlashFails()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _cursesMock.Setup(s => s.flash())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _terminal.Alert(true))
              .Operation.ShouldBe("flash");
    }

    [TestMethod]
    public void AtomicRefresh_UpdatesScreenWhenObjectIsDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        using (_terminal.AtomicRefresh())
        {
        }

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void AtomicRefresh_IsReentrant()
    {
        _terminal = new(_cursesMock.Object, _settings);
        using (_terminal.AtomicRefresh())
        {
            using (_terminal.AtomicRefresh())
            {
            }
        }

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void AtomicRefreshOpen_ProperlyIdentifiesAtomicBlocks()
    {
        _terminal = new(_cursesMock.Object, _settings);
        
        _terminal.AtomicRefreshOpen.ShouldBeFalse();
        using (_terminal.AtomicRefresh())
        {
            using (_terminal.AtomicRefresh())
            {
                _terminal.AtomicRefreshOpen.ShouldBeTrue();
            }
            _terminal.AtomicRefreshOpen.ShouldBeTrue();
        }
        _terminal.AtomicRefreshOpen.ShouldBeFalse();
    }

    [TestMethod]
    public void TryUpdate_ReturnsTrueAndCallsCursesIfNotBatch()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.TryUpdate()
                 .ShouldBeTrue();

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void TryUpdate_ReturnsFalseAndDoesNotCallCursesIfBatch()
    {
        _terminal = new(_cursesMock.Object, _settings);
        using (_terminal.AtomicRefresh())
        {
            _terminal.TryUpdate()
                     .ShouldBeFalse();

            _cursesMock.Verify(v => v.doupdate(), Times.Never);
        }
    }

    [TestMethod]
    public void TryUpdate_Throws_IfTerminalIsDisposed()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();

        Should.Throw<ObjectDisposedException>(() => _terminal.TryUpdate());
    }

    [TestMethod, SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void TryUpdate_Throws_IfCursesFails()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _cursesMock.Setup(s => s.doupdate())
                   .Returns(-1);

        Should.Throw<CursesOperationException>(() => _terminal.TryUpdate())
              .Operation.ShouldBe("doupdate");
    }

    [TestMethod]
    public void Disposed_ReturnsFalse_IfTerminalIsAlive()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Disposed.ShouldBeFalse();
        _terminal.Disposed.ShouldBe(_terminal.Screen.Disposed);
    }

    [TestMethod]
    public void Disposed_ReturnsTrue_IfDisposeIsCalled()
    {
        _terminal = new(_cursesMock.Object, _settings);
        _terminal.Dispose();
        _terminal.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_DisposesTheScreen()
    {
        _terminal = new(_cursesMock.Object, _settings);
        var screen = _terminal.Screen;

        _terminal.Dispose();
        screen.Disposed.ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_RestoresCursesDefaults()
    {
        _cursesMock.Setup(s => s.mouseinterval(It.IsAny<int>()))
                   .Returns(199);

        _cursesMock.Setup(s => s.mousemask(It.IsAny<int>(), out It.Ref<int>.IsAny))
                   .Returns((int _, out int o) =>
                   {
                       o = 888;
                       return 0;
                   });

        _cursesMock.Setup(s => s.curs_set(It.IsAny<int>()))
                   .Returns(66);

        _terminal = new(_cursesMock.Object, _settings);

        _terminal.Dispose();

        _cursesMock.Verify(v => v.mouseinterval(199), Times.Once);
        _cursesMock.Verify(v => v.mousemask(888, out It.Ref<int>.IsAny), Times.Once);
        _cursesMock.Verify(v => v.curs_set(66), Times.Once);
    }

    [TestMethod]
    public void Dispose_DoeNotThrow_IfCursesFails()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _cursesMock.Setup(s => s.mouseinterval(It.IsAny<int>()))
                   .Returns(-1);

        _cursesMock.Setup(s => s.mousemask(It.IsAny<int>(), out It.Ref<int>.IsAny))
                   .Returns((int _, out int o) =>
                   {
                       o = 888;
                       return -1;
                   });

        _cursesMock.Setup(s => s.curs_set(It.IsAny<int>()))
                   .Returns(-1);

        Should.NotThrow(() => _terminal.Dispose());
    }

    [TestMethod]
    public void SetTitle_Throws_IfTitleIsNull()
    {
        _terminal = new(_cursesMock.Object, _settings);
        Should.Throw<ArgumentNullException>(() => _terminal.SetTitle(null!));
    }

    [TestMethod]
    public void SetTitle_AsksCurses()
    {
        _terminal = new(_cursesMock.Object, _settings);

        _terminal.SetTitle("title");
        _cursesMock.Verify(v => v.set_title("title"), Times.Once);
    }
}
