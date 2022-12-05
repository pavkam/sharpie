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
public class EventPumpTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private EventPump _pump = null!;
    private CancellationTokenSource _source = null!;
    private Terminal _terminal = null!;
    private Window _window = null!;

    private Event[] SimulateEvents(int count, IWindow w, params (int result, uint keyCode)[] raw)
    {
        var i = 0;

        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       if (i == raw.Length)
                       {
                           kc = 0;
                           return -1;
                       }

                       kc = raw[i]
                           .keyCode;

                       var res = raw[i]
                           .result;

                       i++;

                       return res;
                   });

        var events = new List<Event>();
        foreach (var e in _pump.Listen(w, _source.Token))
        {
            count--;
            if (count == 0)
            {
                _source.Cancel();
            }

            events.Add(e);
        }

        return events.ToArray();
    }

    private Event SimulateEvent(IWindow w, params (int result, uint keyCode)[] raw) =>
        SimulateEvents(1, w, raw)
            .Single();

    private Event SimulateEvent(params (int result, uint keyCode)[] raw) => SimulateEvent(_window, raw);

    private Event SimulateEventRep(int count, int result, uint keyCode) =>
        SimulateEvents(1, _window, Enumerable.Repeat((result, keyCode), count)
                                             .ToArray())
            .Single();

    private Event SimulateEvent(IWindow w, int result, uint keyCode) => SimulateEvent(w, (result, keyCode));
    private Event SimulateEvent(int result, uint keyCode) => SimulateEvent(_window, result, keyCode);

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object, new(UseStandardKeySequenceResolvers: false));
        _pump = new EventPump(_cursesMock.Object, _terminal.Screen);
        _window = new(_cursesMock.Object, _terminal.Screen, new(2));
        _source = new();
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfCursesIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new EventPump(null!, _terminal.Screen));
    }

    [TestMethod]
    public void Ctor_Throws_IfScreenIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new EventPump(_cursesMock.Object, null!));
    }

    [TestMethod]
    public void Use_RegistersResolver()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("alex");

        _pump.Use((_, nameFunc) => (new(Key.F1, new(ControlCharacter.Null), nameFunc(1), ModifierKey.None), 1));
        var done = _pump.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.KeypadHome, new(ControlCharacter.Null), "test-1", ModifierKey.None),
                new KeyEvent(Key.F6, new(ControlCharacter.Null), "test-2", ModifierKey.None)
            }, false, out var resolved);

        done.ShouldBe(1);
        resolved.ShouldNotBeNull();
        resolved.Key.ShouldBe(Key.F1);
        resolved.Char.ShouldBe(new(ControlCharacter.Null));
        resolved.Modifiers.ShouldBe(ModifierKey.None);
        resolved.Name.ShouldBe("alex");
    }

    [TestMethod]
    public void Use_Throws_IfResolverIsNull() { Should.Throw<ArgumentNullException>(() => _pump.Use(null!)); }

    [TestMethod]
    public void Uses_Throws_IfResolverIsNull() { Should.Throw<ArgumentNullException>(() => _pump.Uses(null!)); }

    [TestMethod]
    public void Uses_ReturnsTrue_IfResolverRegistered()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _pump.Uses(KeySequenceResolver.AltKeyResolver)
             .ShouldBeTrue();
    }

    [TestMethod]
    public void Uses_ReturnsFalse_IfResolverNotRegistered()
    {
        _pump.Uses(KeySequenceResolver.AltKeyResolver)
             .ShouldBeFalse();
    }

    [TestMethod]
    public void TryResolveKeySequence_Throws_IfSequenceIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _pump.TryResolveKeySequence(null!, false, out var _));
    }

    [TestMethod]
    public void TryResolveKeySequence_IgnoresEmptySequences()
    {
        var count = _pump.TryResolveKeySequence(Array.Empty<KeyEvent>(), false, out var resolved);

        count.ShouldBe(0);
        resolved.ShouldBeNull();
    }

    [TestMethod]
    public void TryResolveKeySequence_ReturnsKeysIndividually_IfNoResolvers()
    {
        var k1 = new KeyEvent(Key.KeypadHome, new(ControlCharacter.Null), null, ModifierKey.None);
        var k2 = new KeyEvent(Key.F1, new(ControlCharacter.Null), null, ModifierKey.None);

        var count = _pump.TryResolveKeySequence(new[] { k1, k2 }, false, out var resolved);

        count.ShouldBe(1);
        resolved.ShouldBe(k1);
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void TryResolveKeySequence_WaitForMoreChars_IfBestIsFalse(bool inv)
    {
        if (inv)
        {
            _pump.Use(KeySequenceResolver.AltKeyResolver);
            _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);
        } else
        {
            _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);
            _pump.Use(KeySequenceResolver.AltKeyResolver);
        }

        var count = _pump.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
                new KeyEvent(Key.Character, new('O'), null, ModifierKey.None)
            }, false, out var resolved);

        count.ShouldBe(2);
        resolved.ShouldBeNull();
    }

    [TestMethod, DataRow(true), DataRow(false)]
    public void TryResolveKeySequence_ReturnsBest_IfBestIsTrue(bool inv)
    {
        if (inv)
        {
            _pump.Use(KeySequenceResolver.AltKeyResolver);
            _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);
        } else
        {
            _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);
            _pump.Use(KeySequenceResolver.AltKeyResolver);
        }

        var count = _pump.TryResolveKeySequence(
            new[]
            {
                new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
                new KeyEvent(Key.Character, new('O'), null, ModifierKey.None)
            }, true, out var resolved);

        count.ShouldBe(2);
        resolved.ShouldNotBeNull();
    }

    [TestMethod]
    public void UseInternalMouseEventResolver_SetToTrue_InitializesMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        var me = new MouseMoveEvent(new(1, 1));
        _pump.TryResolveMouseEvent(me, out var l)
             .ShouldBeTrue();

        // ReSharper disable once PossibleMultipleEnumeration
        l.ShouldNotBeNull();

        // ReSharper disable once PossibleMultipleEnumeration
        var ll = l.ToArray();
        ll.Length.ShouldBe(1);
        ll[0]
            .ShouldBe(me);
    }

    [TestMethod]
    public void UseInternalMouseEventResolver_SetToFalse_UnInitializesMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;
        _pump.UseInternalMouseEventResolver = false;

        var me = new MouseMoveEvent(new(1, 1));
        _pump.TryResolveMouseEvent(me, out var _)
             .ShouldBeFalse();
    }

    [TestMethod]
    public void TryResolveMouseEvent1_CallsTheInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        var me = new MouseMoveEvent(new(1, 1));
        _pump.TryResolveMouseEvent(me, out var _)
             .ShouldBeTrue();
    }

    [TestMethod]
    public void TryResolveMouseEvent1_Throws_IfEventIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _pump.TryResolveMouseEvent((MouseMoveEvent) null!, out var _));
    }

    [TestMethod]
    public void TryResolveMouseEvent2_CallsTheInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        var me = new MouseActionEvent(new(1, 1), MouseButton.Button1, MouseButtonState.Clicked, ModifierKey.Ctrl);
        _pump.TryResolveMouseEvent(me, out var _)
             .ShouldBeTrue();
    }

    [TestMethod]
    public void TryResolveMouseEvent2_Throws_IfEventIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _pump.TryResolveMouseEvent((MouseActionEvent) null!, out var _));
    }

    [TestMethod]
    public void Listen1_ThrowsIfWindowIsNull()
    {
        Should.Throw<ArgumentNullException>(() => _pump.Listen(null!, CancellationToken.None)
                                                       .ToArray());
    }

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen1_CallsCurses_ForWindow()
    {
        _pump.Listen(_window, CancellationToken.None)
             .First();

        _cursesMock.Verify(s => s.wget_wch(_window.Handle, out It.Ref<uint>.IsAny), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen3_CallsCurses_ForScreen()
    {
        _pump.Listen(CancellationToken.None)
             .First();

        _cursesMock.Verify(s => s.wget_wch(_terminal.Screen.Handle, out It.Ref<uint>.IsAny), Times.Once);
    }

    [TestMethod]
    public void Listen1_KeepsReadingUntilCancelled()
    {
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = 'a';
                       return 0;
                   });

        var count = 5;
        foreach (var _ in _pump.Listen(_window, _source.Token))
        {
            count--;
            if (count == 0)
            {
                _source.Cancel();
            }
        }

        count.ShouldBe(0);

        _cursesMock.Verify(v => v.wtimeout(_window.Handle, It.IsAny<int>()), Times.Exactly(5));
        _cursesMock.Verify(v => v.wget_wch(_window.Handle, out It.Ref<uint>.IsAny), Times.Exactly(5));
    }

    [TestMethod]
    public void Listen1_SkipsBadReads()
    {
        SimulateEvents(5, _window, (-1, 0), (0, 0), (-1, 0),
            (0, 0), (-1, 0), (0, 0), (-1, 0), (0, 0),
            (-1, 0), (0, 0));

        _cursesMock.Verify(v => v.wtimeout(_window.Handle, It.IsAny<int>()), Times.Exactly(10));
        _cursesMock.Verify(v => v.wget_wch(_window.Handle, out It.Ref<uint>.IsAny), Times.Exactly(10));
    }

    [TestMethod]
    public void Listen1_ApplyPendingRefreshes_WhenReadFails()
    {
        SimulateEvents(1, _window, (-1, 0), (0, 0));
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Listen1_GoesDeepWithinChildren_ToApplyPendingRefreshes()
    {
        var w = new Window(_cursesMock.Object, _window, new(3));

        SimulateEvents(1, w, (-1, 0), (0, 0));

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void Listen1_ProcessesTerminalResizeEvents_InScreen()
    {
        _window.Dispose();

        _cursesMock.Setup(s => s.getmaxy(_terminal.Screen.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.getmaxx(_terminal.Screen.Handle))
                   .Returns(20);

        var @event = SimulateEvent(_terminal.Screen, (int) CursesKey.Yes, (uint) CursesKey.Resize);

        @event.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) @event).Size.ShouldBe(new(20, 10));

        _cursesMock.Verify(v => v.wtouchln(_terminal.Screen.Handle, 0, 10, 1), Times.Once);
        _cursesMock.Verify(v => v.clearok(_terminal.Screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_terminal.Screen.Handle), Times.Once);
    }

    [TestMethod]
    public void Listen1_ProcessesTerminalResizeEvents_InChild()
    {
        var otherWindow = new Window(_cursesMock.Object, _terminal.Screen, new(8));

        _cursesMock.Setup(s => s.getmaxy(_terminal.Screen.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.getmaxx(_terminal.Screen.Handle))
                   .Returns(20);

        _cursesMock.Setup(s => s.getmaxy(_window.Handle))
                   .Returns(5);

        _cursesMock.Setup(s => s.getmaxy(otherWindow.Handle))
                   .Returns(5);

        var e = SimulateEvent((int) CursesKey.Yes, (uint) CursesKey.Resize);
        e.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) e).Size.ShouldBe(new(20, 10));

        _cursesMock.Verify(v => v.wtouchln(_terminal.Screen.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(_window.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(otherWindow.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);

        _cursesMock.Verify(v => v.clearok(_terminal.Screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_terminal.Screen.Handle), Times.Once);
    }

    [TestMethod]
    public void Listen1_SkipsInvalidMouseEvents()
    {
        var skip = true;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       var dx = skip ? 10 : 0;
                       me = new()
                       {
                           x = dx + 5, y = dx + 6, buttonState = (uint) CursesMouseEvent.EventType.ReportPosition
                       };

                       var res = skip ? -1 : 0;
                       skip = !skip;

                       return res;
                   });

        var e = SimulateEventRep(2, (int) CursesKey.Yes, (uint) CursesKey.Mouse);
        e.Type.ShouldBe(EventType.MouseMove);
        ((MouseMoveEvent) e).Position.ShouldBe(new(5, 6));

        _cursesMock.Verify(v => v.getmouse(out It.Ref<CursesMouseEvent>.IsAny), Times.Exactly(2));
    }

    [TestMethod]
    public void Listen1_SkipsMouseEvents_WithBadButtons()
    {
        _pump.UseInternalMouseEventResolver = false;

        var skip = true;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new() { buttonState = skip ? 0 : (uint) CursesMouseEvent.EventType.Button2Released };

                       skip = !skip;
                       return 0;
                   });

        var e = SimulateEventRep(2, (int) CursesKey.Yes, (int) CursesKey.Mouse);
        ((MouseActionEvent) e).Button.ShouldBe(MouseButton.Button2);

        _cursesMock.Verify(v => v.getmouse(out It.Ref<CursesMouseEvent>.IsAny), Times.Exactly(2));
    }

    [TestMethod]
    public void Listen1_ProcessesMouseMoveEvents()
    {
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new() { x = 5, y = 6, buttonState = (uint) CursesMouseEvent.EventType.ReportPosition };

                       return 0;
                   });

        var e = SimulateEvent((int) CursesKey.Yes, (uint) CursesKey.Mouse);
        e.Type.ShouldBe(EventType.MouseMove);
        ((MouseMoveEvent) e).Position.ShouldBe(new(5, 6));
    }

    [TestMethod]
    public void Listen1_ProcessesMouseMoveEvents_AndUsesInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new() { x = 5, y = 6, buttonState = (uint) CursesMouseEvent.EventType.ReportPosition };

                       return 0;
                   });

        var e = SimulateEvents(1, _window, ((int) CursesKey.Yes, (uint) CursesKey.Mouse),
            ((int) CursesKey.Yes, (uint) CursesKey.Mouse));

        e.Length.ShouldBe(1);
        e[0]
            .ShouldBe(new MouseMoveEvent(new(5, 6)));
    }

    [TestMethod]
    public void Listen1_ProcessesMouseActionEvents()
    {
        _pump.UseInternalMouseEventResolver = false;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new()
                       {
                           x = 5,
                           y = 6,
                           buttonState = (uint) CursesMouseEvent.EventType.Button1Clicked |
                               (uint) CursesMouseEvent.EventType.Alt
                       };

                       return 0;
                   });

        var e = SimulateEvent((int) CursesKey.Yes, (uint) CursesKey.Mouse);
        e.Type.ShouldBe(EventType.MouseAction);

        var me = (MouseActionEvent) e;
        me.Button.ShouldBe(MouseButton.Button1);
        me.Modifiers.ShouldBe(ModifierKey.Alt);
        me.Position.ShouldBe(new(5, 6));
        me.State.ShouldBe(MouseButtonState.Clicked);
    }

    [TestMethod]
    public void Listen1_ProcessesMouseActionEvents_AndUsesInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new()
                       {
                           x = 5,
                           y = 6,
                           buttonState = (uint) CursesMouseEvent.EventType.Button1Pressed |
                               (uint) CursesMouseEvent.EventType.Alt
                       };

                       return 0;
                   });

        var e = SimulateEvents(2, _window, ((int) CursesKey.Yes, (uint) CursesKey.Mouse));
        e.Length.ShouldBe(2);
        e[0]
            .ShouldBe(new MouseMoveEvent(new(5, 6)));

        e[1]
            .ShouldBe(new MouseActionEvent(new(5, 6), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.Alt));
    }

    [TestMethod]
    public void Listen1_ProcessesKeypadEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent((int) CursesKey.Yes, (uint) CursesKey.AltUp);
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Modifiers.ShouldBe(ModifierKey.Alt);
        me.Char.ShouldBe(new(ControlCharacter.Null));
        me.Key.ShouldBe(Key.KeypadUp);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_ProcessesCharacterEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent(0, 'a');
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new('a'));
        me.Key.ShouldBe(Key.Character);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_ProcessesTranslatedCharacters_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.SpecialCharacterResolver);
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent(0, ControlCharacter.Tab);
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new(ControlCharacter.Null));
        me.Key.ShouldBe(Key.Tab);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_DoesNotProcessesTranslatedCharacters_IfNoMiddlewareInstalled()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent(0, ControlCharacter.Tab);
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new(ControlCharacter.Tab));
        me.Key.ShouldBe(Key.Character);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_ProcessesTranslatedSeq2Events_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent((0, ControlCharacter.Escape), (0, 'a'));
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new('a'));
        me.Modifiers.ShouldBe(ModifierKey.Alt);
        me.Key.ShouldBe(Key.Character);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_DoeNotProcessTranslatedSeq2Events_IfResolverNotInstalled()
    {
        _cursesMock.Setup(s => s.key_name('a'))
                   .Returns("-a-");

        _cursesMock.Setup(s => s.key_name(ControlCharacter.Escape))
                   .Returns("-esc-");

        var e = SimulateEvents(2, _window, (0, ControlCharacter.Escape), (0, 'a'));
        e.Length.ShouldBe(2);

        var me0 = (KeyEvent) e[0];
        me0.Char.ShouldBe(new(ControlCharacter.Escape));
        me0.Modifiers.ShouldBe(ModifierKey.None);
        me0.Key.ShouldBe(Key.Character);
        me0.Name.ShouldBe("-esc-");

        var me1 = (KeyEvent) e[1];
        me1.Char.ShouldBe(new('a'));
        me1.Modifiers.ShouldBe(ModifierKey.None);
        me1.Key.ShouldBe(Key.Character);
        me1.Name.ShouldBe("-a-");
    }

    [TestMethod]
    public void Listen1_ProcessesTranslatedSeq4Events_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);

        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateEvent((0, ControlCharacter.Escape), (0, 'O'), (0, '8'), (0, 'A'));
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new(ControlCharacter.Null));
        me.Modifiers.ShouldBe(ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift);
        me.Key.ShouldBe(Key.KeypadUp);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void Listen1_ConsidersEscapeBreaks()
    {
        _pump.Use(KeySequenceResolver.SpecialCharacterResolver);

        var e = SimulateEvents(2, _window, (0, ControlCharacter.Escape), (0, ControlCharacter.Escape));
        e.Length.ShouldBe(2);
        ((KeyEvent) e[0]).Key.ShouldBe(Key.Escape);
        ((KeyEvent) e[1]).Key.ShouldBe(Key.Escape);
    }

    [TestMethod]
    public void Listen1_ConsidersBreaksInSequences()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _cursesMock.Setup(s => s.getmouse(out It.Ref<CursesMouseEvent>.IsAny))
                   .Returns((out CursesMouseEvent me) =>
                   {
                       me = new() { buttonState = (uint) CursesMouseEvent.EventType.ReportPosition };
                       return 0;
                   });

        var e = SimulateEvents(3, _window, (0, ControlCharacter.Escape), ((int) CursesKey.Yes, (int) CursesKey.Mouse),
            (0, 'A'));

        e.Length.ShouldBe(3);

        ((KeyEvent) e[0]).Key.ShouldBe(Key.Character);
        ((KeyEvent) e[0]).Char.ShouldBe(new(ControlCharacter.Escape));

        e[1]
            .Type.ShouldBe(EventType.MouseMove);

        ((KeyEvent) e[2]).Char.ShouldBe(new('A'));
    }
}
