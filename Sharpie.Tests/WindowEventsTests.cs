namespace Sharpie.Tests;

[TestClass]
public class WindowEventsTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private Screen _screen = null!;
    private CancellationTokenSource _source = null!;
    private Window _window = null!;

    private Event[] SimulateEvents(int count, Window w, params (int result, uint keyCode)[] raw)
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
        foreach (var e in w.ProcessEvents(_source.Token))
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

    private Event SimulateEvent(Window w, params (int result, uint keyCode)[] raw) =>
        SimulateEvents(1, w, raw)
            .Single();

    private Event SimulateEvent(params (int result, uint keyCode)[] raw) => SimulateEvent(_window, raw);

    private Event SimulateEventRep(int count, int result, uint keyCode) =>
        SimulateEvents(1, _window, Enumerable.Repeat((result, keyCode), count)
                                             .ToArray())
            .Single();

    private Event SimulateEvent(Window w, int result, uint keyCode) => SimulateEvent(w, (result, keyCode));
    private Event SimulateEvent(int result, uint keyCode) => SimulateEvent(_window, result, keyCode);

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _screen = new(_cursesMock.Object, new(1));

        _window = new(_cursesMock.Object, _screen, new(2));
        _source = new();
    }

    [TestMethod]
    public void ProcessEvents_KeepsReadingUntilCancelled()
    {
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = 'a';
                       return 0;
                   });

        var count = 5;
        foreach (var _ in _window.ProcessEvents(_source.Token))
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
    public void ProcessEvents_SkipsBadReads()
    {
        SimulateEvents(5, _window, (-1, 0), (0, 0), (-1, 0),
            (0, 0), (-1, 0), (0, 0), (-1, 0), (0, 0),
            (-1, 0), (0, 0));

        _cursesMock.Verify(v => v.wtimeout(_window.Handle, It.IsAny<int>()), Times.Exactly(10));
        _cursesMock.Verify(v => v.wget_wch(_window.Handle, out It.Ref<uint>.IsAny), Times.Exactly(10));
    }

    [TestMethod]
    public void ProcessEvents_ApplyPendingRefreshes_WhenReadFails()
    {
        SimulateEvents(1, _window, (-1, 0), (0, 0));
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_GoesDeepWithinChildren_ToApplyPendingRefreshes()
    {
        var w = new Window(_cursesMock.Object, _window, new(3));

        SimulateEvents(1, w, (-1, 0), (0, 0));

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_ProcessesTerminalResizeEvents_InScreen()
    {
        _window.Dispose();

        _cursesMock.Setup(s => s.getmaxy(_screen.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.getmaxx(_screen.Handle))
                   .Returns(20);

        var @event = SimulateEvent(_screen, (int) CursesKey.Yes, (uint) CursesKey.Resize);

        @event.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) @event).Size.ShouldBe(new(20, 10));

        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, 0, 10, 1), Times.Once);
        _cursesMock.Verify(v => v.clearok(_screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_ProcessesTerminalResizeEvents_InChild()
    {
        var otherWindow = new Window(_cursesMock.Object, _screen, new(8));

        _cursesMock.Setup(s => s.getmaxy(_screen.Handle))
                   .Returns(10);

        _cursesMock.Setup(s => s.getmaxx(_screen.Handle))
                   .Returns(20);

        _cursesMock.Setup(s => s.getmaxy(_window.Handle))
                   .Returns(5);

        _cursesMock.Setup(s => s.getmaxy(otherWindow.Handle))
                   .Returns(5);

        var e = SimulateEvent((int) CursesKey.Yes, (uint) CursesKey.Resize);
        e.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) e).Size.ShouldBe(new(20, 10));

        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(_window.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(otherWindow.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);

        _cursesMock.Verify(v => v.clearok(_screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_SkipsInvalidMouseEvents()
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
    public void ProcessEvents_SkipsMouseEvents_WithBadButtons()
    {
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
    public void ProcessEvents_ProcessesMouseMoveEvents()
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
    public void ProcessEvents_ProcessesMouseMoveEvents_AndUsesInternalMouseResolver()
    {
        _screen.UseInternalMouseEventResolver = true;
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
    public void ProcessEvents_ProcessesMouseActionEvents()
    {
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
    public void ProcessEvents_ProcessesMouseActionEvents_AndUsesInternalMouseResolver()
    {
        _screen.UseInternalMouseEventResolver = true;

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
    public void ProcessEvents_ProcessesKeypadEvents()
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
    public void ProcessEvents_ProcessesCharacterEvents()
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
    public void ProcessEvents_ProcessesTranslatedCharacters_IfResolverInstalled()
    {
        _screen.Use(KeySequenceResolver.SpecialCharacterResolver);
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
    public void ProcessEvents_DoesNotProcessesTranslatedCharacters_IfNoMiddlewareInstalled()
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
    public void ProcessEvents_ProcessesTranslatedSeq2Events_IfResolverInstalled()
    {
        _screen.Use(KeySequenceResolver.AltKeyResolver);
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
    public void ProcessEvents_DoeNotProcessTranslatedSeq2Events_IfResolverNotInstalled()
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
    public void ProcessEvents_ProcessesTranslatedSeq4Events_IfResolverInstalled()
    {
        _screen.Use(KeySequenceResolver.AltKeyResolver);
        _screen.Use(KeySequenceResolver.KeyPadModifiersResolver);

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
    public void ProcessEvents_ConsidersEscapeBreaks()
    {
        _screen.Use(KeySequenceResolver.SpecialCharacterResolver);

        var e = SimulateEvents(2, _window, (0, ControlCharacter.Escape), (0, ControlCharacter.Escape));
        e.Length.ShouldBe(2);
        ((KeyEvent) e[0]).Key.ShouldBe(Key.Escape);
        ((KeyEvent) e[1]).Key.ShouldBe(Key.Escape);
    }

    [TestMethod]
    public void ProcessEvents_ConsidersBreaksInSequences()
    {
        _screen.Use(KeySequenceResolver.AltKeyResolver);
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
