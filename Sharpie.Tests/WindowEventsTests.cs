namespace Sharpie.Tests;

[TestClass]
public class WindowEventsTests
{
    private Mock<ICursesProvider> _cursesMock = null!;
    private CancellationTokenSource _source = null!;
    private Screen _screen = null!;
    private Window _window = null!;

    private Event GetOneEvent(int result, uint keyCode)
    {
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = keyCode;
                       return result;
                   });

        Event @event = null!;
        foreach (var e in _window.ProcessEvents(_source.Token))
        {
            _source.Cancel();
            @event = e;
        }

        return @event;
    }

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
        var skip = false;
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       skip = !skip;
                       if (skip)
                       {
                           kc = 'b';
                           return -1;
                       }

                       kc = 'b';
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

        _cursesMock.Verify(v => v.wtimeout(_window.Handle, It.IsAny<int>()), Times.Exactly(10));
        _cursesMock.Verify(v => v.wget_wch(_window.Handle, out It.Ref<uint>.IsAny), Times.Exactly(10));
    }
    
    [TestMethod]
    public void ProcessEvents_ApplyPendingRefreshes_WhenReadFails()
    {
        var skip = true;
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = 0;
                       var res = skip ? -1 : 0;
                       skip = !skip;
                       
                       return res;
                   });

        foreach (var _ in _window.ProcessEvents(_source.Token))
        {
            _source.Cancel();
        }

        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_ProcessesTerminalResizeEvents_InScreen()
    {
        _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = (uint) RawKey.Resize;
                       return (int) RawKey.Yes;
                   });
        
        _cursesMock.Setup(s => s.getmaxy(_screen.Handle))
                   .Returns(10);
        _cursesMock.Setup(s => s.getmaxx(_screen.Handle))
                   .Returns(20);

        Event @event = null!;
        foreach (var e in _screen.ProcessEvents(_source.Token))
        {
            _source.Cancel();
            @event = e;
        }
        
        @event.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) @event).Size.ShouldBe(new(20, 10));
        
        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, 0, 10, 1), Times.Once);
        _cursesMock.Verify(v => v.clearok(_screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
    }

    [TestMethod]
    public void ProcessEvents_ProcessesTerminalResizeEvents_InChild()
    {
        _cursesMock.Setup(s => s.getmaxy(_screen.Handle))
                   .Returns(10);
        _cursesMock.Setup(s => s.getmaxx(_screen.Handle))
                   .Returns(20);
        _cursesMock.Setup(s => s.getmaxx(_window.Handle))
                   .Returns(5);
        _cursesMock.Setup(s => s.getmaxy(_window.Handle))
                   .Returns(6);

        var e = GetOneEvent((int) RawKey.Yes, (uint) RawKey.Resize);
        e.Type.ShouldBe(EventType.TerminalResize);
        ((TerminalResizeEvent) e).Size.ShouldBe(new(20, 10));
        
        _cursesMock.Verify(v => v.wtouchln(_screen.Handle, 0, 10, 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(_window.Handle, 0, 6, 1), Times.Once);
        _cursesMock.Verify(v => v.clearok(_screen.Handle, true), Times.Once);
        _cursesMock.Verify(v => v.wrefresh(_screen.Handle), Times.Once);
    }
    
    [TestMethod]
    public void ProcessEvents_SkipsInvalidMouseEvents()
    {
        var skip = true;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<RawMouseEvent>.IsAny))
                   .Returns((out RawMouseEvent me) =>
                   {
                       var dx = skip ? 10 : 0;
                       me = new()
                       {
                           x = dx + 5,
                           y = dx + 6,
                           buttonState = (ulong) RawMouseEvent.EventType.ReportPosition | 0x100
                       };

                       var res = skip ? -1 : 0;
                       skip = !skip;

                       return res;
                   });

        var e = GetOneEvent((int) RawKey.Yes, (uint) RawKey.Mouse);
        e.Type.ShouldBe(EventType.MouseMove);
        ((MouseMoveEvent) e).Position.ShouldBe(new(5, 6));

        _cursesMock.Verify(v => v.getmouse(out It.Ref<RawMouseEvent>.IsAny), Times.Exactly(2));
    }

    [TestMethod]
    public void ProcessEvents_SkipsMouseEvents_WithBadButtons()
    {
        var skip = true;
        _cursesMock.Setup(s => s.getmouse(out It.Ref<RawMouseEvent>.IsAny))
                   .Returns((out RawMouseEvent me) =>
                   {
                       me = new() { buttonState = skip ? 0 : (ulong) RawMouseEvent.EventType.Button2Released };

                       skip = !skip;
                       return 0;
                   });

        var e = GetOneEvent((int) RawKey.Yes, (int) RawKey.Mouse);
        ((MouseActionEvent) e).Button.ShouldBe(MouseButton.Button2);

        _cursesMock.Verify(v => v.getmouse(out It.Ref<RawMouseEvent>.IsAny), Times.Exactly(2));
    }

    [TestMethod]
    public void ProcessEvents_ProcessesMouseMoveEvents()
    {
        _cursesMock.Setup(s => s.getmouse(out It.Ref<RawMouseEvent>.IsAny))
                   .Returns((out RawMouseEvent me) =>
                   {
                       me = new()
                       {
                           x = 5, y = 6, buttonState = (ulong) RawMouseEvent.EventType.ReportPosition | 0x100
                       };

                       return 0;
                   });

        var e = GetOneEvent((int) RawKey.Yes, (uint) RawKey.Mouse);
        e.Type.ShouldBe(EventType.MouseMove);
        ((MouseMoveEvent) e).Position.ShouldBe(new(5, 6));
    }

    [TestMethod]
    public void ProcessEvents_ProcessesMouseActionEvents()
    {
        _cursesMock.Setup(s => s.getmouse(out It.Ref<RawMouseEvent>.IsAny))
                   .Returns((out RawMouseEvent me) =>
                   {
                       me = new()
                       {
                           x = 5,
                           y = 6,
                           buttonState = (ulong) RawMouseEvent.EventType.Button1Clicked |
                               (ulong) RawMouseEvent.EventType.Alt
                       };

                       return 0;
                   });

        var e = GetOneEvent((int) RawKey.Yes, (uint) RawKey.Mouse);
        e.Type.ShouldBe(EventType.MouseAction);

        var me = (MouseActionEvent) e;
        me.Button.ShouldBe(MouseButton.Button1);
        me.Modifiers.ShouldBe(ModifierKey.Alt);
        me.Position.ShouldBe(new(5, 6));
        me.State.ShouldBe(MouseButtonState.Clicked);
    }

    [TestMethod]
    public void ProcessEvents_ProcessesKeypadEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = GetOneEvent((int) RawKey.Yes, (uint) RawKey.AltUp);
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Modifiers.ShouldBe(ModifierKey.Alt);
        me.Char.ShouldBe(new('\0'));
        me.Key.ShouldBe(Key.KeypadUp);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void ProcessEvents_ProcessesCharacterEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = GetOneEvent(0, 'a');
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new('a'));
        me.Key.ShouldBe(Key.Character);
        me.Name.ShouldBe("yup");
    }

    [TestMethod]
    public void ProcessEvents_ProcessesSpecialCharacterEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = GetOneEvent(0, 3);
        e.Type.ShouldBe(EventType.KeyPress);

        var me = (KeyEvent) e;
        me.Char.ShouldBe(new('\0'));
        me.Key.ShouldBe(Key.F1);
        me.Name.ShouldBe("yup");
    }
}
