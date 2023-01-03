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
    private const int Timeout = 10000;

    private Mock<ICursesBackend> _cursesMock = null!;
    private EventPump _pump = null!;
    private CancellationTokenSource _source = null!;
    private Terminal _terminal = null!;
    private Window _window = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    private Event[] SimulateActualEvents(ISurface sf, int expectedCount, params CursesEvent?[] cursesEvents)
    {
        var q = new Queue<CursesEvent?>(cursesEvents);
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent? ce) =>
                   {
                       ce = q.Dequeue();
                       return ce == null ? -1 : 0;
                   });

        var cts = new CancellationTokenSource();
        using var en = _pump.Listen(sf, cts.Token)
                            .GetEnumerator();

        var result = new List<Event>();
        while (en.MoveNext())
        {
            if (en.Current is StartEvent or StopEvent)
            {
                continue;
            }

            expectedCount--;

            result.Add(en.Current);
            if (expectedCount == 0)
            {
                cts.Cancel();
            }
        }

        return result.ToArray();
    }

    private Event? SimulateActualEvent(ISurface sf, params CursesEvent?[] cursesEvents) =>
        SimulateActualEvents(sf, 1, cursesEvents)
            .SingleOrDefault();

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();

        _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(100));

        _terminal = new(_cursesMock.Object,
            new(UseStandardKeySequenceResolvers: false,
                ManagedWindows: TestContext.TestName!.Contains("_WhenManaged_")));

        _pump = new(_terminal);
        _window = new(_terminal.Screen, new(2));
        _source = new();
    }

    [TestCleanup] public void TestCleanup() { _terminal.Dispose(); }

    [TestMethod]
    public void Ctor_Throws_IfTerminalIsNull() { Should.Throw<ArgumentNullException>(() => new EventPump(null!)); }

    [TestMethod]
    public void Terminal_IsInitialized()
    {
        _pump.Terminal.ShouldBe(_terminal);
        ((IEventPump) _pump).Terminal.ShouldBe(_terminal);
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

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen1_AsksCursesForEvent_AfterEmittingStart()
    {
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent ce) =>
                   {
                       ce = new CursesCharEvent('a');
                       return 0;
                   });

        _pump.Listen(_window, CancellationToken.None)
             .First(f => f.Type != EventType.Start)
             .ToString();

        _cursesMock.Verify(s => s.wget_event(_window.Handle, It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!),
            Times.Once);
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_EmitsStartEvent()
    {
        _pump.Listen(_window, CancellationToken.None)
             .First()
             .ShouldBe(new StartEvent());
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_EmitsStopEvent()
    {
        var f = true;
        foreach (var e in _pump.Listen(_window, _source.Token))
        {
            if (f)
            {
                e.ShouldBe(new StartEvent());
                f = false;
            } else
            {
                e.ShouldBe(new StopEvent());
            }

            _source.Cancel();
        }
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_KeepsReadingUntilCancelled()
    {
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent ce) =>
                   {
                       ce = new CursesCharEvent('a');
                       return 0;
                   });

        var count = 5;
        foreach (var e in _pump.Listen(_window, _source.Token))
        {
            if (e.Type == EventType.KeyPress)
            {
                count--;
                if (count == 0)
                {
                    _source.Cancel();
                }
            }
        }

        count.ShouldBe(0);

        _cursesMock.Verify(v => v.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!),
            Times.Exactly(5));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_KeepsAskingCurses_IfNoEventsReceivedOnRead()
    {
        var a = new CursesCharEvent('a');
        var b = new CursesCharEvent('b');
        var c = new CursesCharEvent('c');
        var d = new CursesCharEvent('d');
        var e = new CursesCharEvent('e');

        SimulateActualEvents(_window, 5, null, a, null,
            b, null, c, null, d,
            null, e);

        _cursesMock.Verify(v => v.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!),
            Times.Exactly(10));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_AsksCursesToUpdateTerminal_WhenNoEventReceived()
    {
        SimulateActualEvents(_window, 1, null, new CursesResizeEvent());
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_DoesNotAsksCursesToUpdateTerminal_WhenNoEventReceived_AndAtomic()
    {
        using (_terminal.AtomicRefresh())
        {
            SimulateActualEvents(_window, 1, null, new CursesCharEvent('A'));
            _cursesMock.Verify(v => v.doupdate(), Times.Never);
        }
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_AsksScreenToAdjustWindowsToExplicitArea()
    {
        var h1 = new IntPtr(1);
        _cursesMock.MockArea(h1, new(0, 0, 5, 5));
        var w1 = new Window(_terminal.Screen, h1);

        var h2 = new IntPtr(2);
        _cursesMock.MockArea(h2, new(1, 1, 3, 3));
        var w2 = new Window(_terminal.Screen, h2);

        _cursesMock.MockArea(_terminal.Screen, new Size(2, 2));

        SimulateActualEvents(_terminal.Screen, 1, new CursesResizeEvent());

        _cursesMock.Verify(v => v.wresize(w1.Handle, 2, 2), Times.Once);
        _cursesMock.Verify(v => v.wresize(w2.Handle, 1, 1), Times.Once);
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_WhenUnmanaged_OnResize_DoesNotUpdateTheScreen()
    {
        _cursesMock.MockArea(_terminal.Screen, new Size(20, 10));

        SimulateActualEvents(_terminal.Screen, 1, new CursesResizeEvent());

        _cursesMock.Verify(v => v.wtouchln(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);

        _cursesMock.Verify(v => v.wrefresh(It.IsAny<IntPtr>()), Times.Never);
        _cursesMock.Verify(v => v.wnoutrefresh(It.IsAny<IntPtr>()), Times.Never);
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_WhenManaged_OnResize_UpdatesTheScreenAndWindow()
    {
        _cursesMock.MockArea(_terminal.Screen, new Size(20, 10));
        _cursesMock.MockArea(_window, new Rectangle(0, 0, 5, 5));

        SimulateActualEvents(_terminal.Screen, 2, new CursesResizeEvent(), new CursesCharEvent('A'));

        _cursesMock.Verify(v => v.wtouchln(_terminal.Screen.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wtouchln(_window.Handle, It.IsAny<int>(), It.IsAny<int>(), 1), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(_terminal.Screen.Handle), Times.Once);
        _cursesMock.Verify(v => v.wnoutrefresh(_window.Handle), Times.Once);
        _cursesMock.Verify(v => v.doupdate(), Times.Once);
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesTerminalResizeEvents()
    {
        _cursesMock.MockArea(_terminal.Screen, new Size(20, 10));

        var events = SimulateActualEvents(_terminal.Screen, 1, new CursesResizeEvent());
        events.ShouldBe(new Event[] { new TerminalResizeEvent(new(20, 10)) });
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesMouseMoveEvents()
    {
        var e = SimulateActualEvent(_terminal.Screen,
            new CursesMouseEvent(5, 6, MouseButton.Unknown, MouseButtonState.None, ModifierKey.None));

        e.ShouldBe(new MouseMoveEvent(new(5, 6)));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesMouseMoveEvents_AndUsesInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        var ce = new CursesMouseEvent(5, 6, MouseButton.Unknown, MouseButtonState.None, ModifierKey.None);
        var e = SimulateActualEvent(_terminal.Screen, ce, ce);

        e.ShouldBe(new MouseMoveEvent(new(5, 6)));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesMouseActionEvents()
    {
        var ce = new CursesMouseEvent(5, 6, MouseButton.Button1, MouseButtonState.Clicked, ModifierKey.Alt);
        var e = SimulateActualEvent(_terminal.Screen, ce, ce);

        e.ShouldBe(new MouseActionEvent(new(5, 6), MouseButton.Button1, MouseButtonState.Clicked, ModifierKey.Alt));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesMouseActionEvents_AndUsesInternalMouseResolver()
    {
        _pump.UseInternalMouseEventResolver = true;

        var ce = new CursesMouseEvent(5, 6, MouseButton.Button1, MouseButtonState.Clicked, ModifierKey.Alt);
        var e = SimulateActualEvents(_terminal.Screen, 2, ce, ce);

        e.ShouldBe(new Event[]
        {
            new MouseMoveEvent(new(5, 6)),
            new MouseActionEvent(new(5, 6), MouseButton.Button1, MouseButtonState.Pressed, ModifierKey.Alt),
            new MouseActionEvent(new(5, 6), MouseButton.Button1, MouseButtonState.Released, ModifierKey.Alt)
        });
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesKeypadEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesKeyEvent(Key.KeypadUp, ModifierKey.Alt));
        e.ShouldBe(new KeyEvent(Key.KeypadUp, new(ControlCharacter.Null), "yup", ModifierKey.Alt));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesCharacterEvents()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesCharEvent('A'));
        e.ShouldBe(new KeyEvent(Key.Character, new('A'), "yup", ModifierKey.None));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesTranslatedCharacters_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.SpecialCharacterResolver);
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesCharEvent(ControlCharacter.Tab));
        e.ShouldBe(new KeyEvent(Key.Tab, new(ControlCharacter.Null), "yup", ModifierKey.None));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_DoesNotProcessesTranslatedCharacters_IfNoMiddlewareInstalled()
    {
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesCharEvent(ControlCharacter.Tab));
        e.ShouldBe(new KeyEvent(Key.Character, new(ControlCharacter.Tab), "yup", ModifierKey.None));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesTranslatedSeq2Events_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesCharEvent(ControlCharacter.Escape),
            new CursesCharEvent('a'));

        e.ShouldBe(new KeyEvent(Key.Character, new('a'), "yup", ModifierKey.Alt));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_DoesNotProcessTranslatedSeq2Events_IfResolverNotInstalled()
    {
        _cursesMock.Setup(s => s.key_name('a'))
                   .Returns("-a-");

        _cursesMock.Setup(s => s.key_name(ControlCharacter.Escape))
                   .Returns("-esc-");

        var e = SimulateActualEvents(_terminal.Screen, 2, new CursesCharEvent(ControlCharacter.Escape),
            new CursesCharEvent('a'));

        e.ShouldBe(new Event[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), "-esc-", ModifierKey.None),
            new KeyEvent(Key.Character, new('a'), "-a-", ModifierKey.None)
        });
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesTranslatedSeq4Events_IfResolverInstalled()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);
        _pump.Use(KeySequenceResolver.KeyPadModifiersResolver);

        _cursesMock.Setup(s => s.key_name(It.IsAny<uint>()))
                   .Returns("yup");

        var e = SimulateActualEvent(_terminal.Screen, new CursesCharEvent(ControlCharacter.Escape),
            new CursesCharEvent('O'), new CursesCharEvent('8'), new CursesCharEvent('A'));

        e.ShouldBe(new KeyEvent(Key.KeypadUp, new(ControlCharacter.Null), "yup",
            ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift));
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ConsidersEscapeBreaks()
    {
        _pump.Use(KeySequenceResolver.SpecialCharacterResolver);

        var e = SimulateActualEvents(_terminal.Screen, 2, new CursesCharEvent(ControlCharacter.Escape),
            new CursesCharEvent(ControlCharacter.Escape));

        e.ShouldBe(new Event[]
        {
            new KeyEvent(Key.Escape, new(ControlCharacter.Null), null, ModifierKey.None),
            new KeyEvent(Key.Escape, new(ControlCharacter.Null), null, ModifierKey.None)
        });
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ConsidersBreaksInSequences()
    {
        _pump.Use(KeySequenceResolver.AltKeyResolver);

        var e = SimulateActualEvents(_terminal.Screen, 3, new CursesCharEvent(ControlCharacter.Escape),
            new CursesMouseEvent(1, 2, MouseButton.Unknown, MouseButtonState.None, ModifierKey.None),
            new CursesCharEvent('A'));

        e.ShouldBe(new Event[]
        {
            new KeyEvent(Key.Character, new(ControlCharacter.Escape), null, ModifierKey.None),
            new MouseMoveEvent(new(1, 2)),
            new KeyEvent(Key.Character, new('A'), null, ModifierKey.None)
        });
    }

    [TestMethod, Timeout(Timeout)]
    public void Listen1_ProcessesDelegatesFirst()
    {
        _pump.Delegate("hello");
        _pump.Delegate("world");

        var e = SimulateActualEvents(_terminal.Screen, 3, new CursesCharEvent('A'));

        e.ShouldBe(new Event[]
        {
            new DelegateEvent("hello"),
            new DelegateEvent("world"),
            new KeyEvent(Key.Character, new('A'), null, ModifierKey.None)
        });
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen2_CreatesDummyPad()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(10));

        _pump.Listen(CancellationToken.None)
             .First()
             .ToString();

        _cursesMock.Verify(v => v.newpad(1, 1), Times.Once);
        _cursesMock.Verify(v => v.keypad(new(10), true), Times.Once);
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Listen2_Throws_IfFailedToCreateDummyPad()
    {
        Should.Throw<CursesOperationException>(() => _pump.Listen(CancellationToken.None)
                                                          .First())
              .Operation.ShouldBe("newpad");
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen2_DestroysDummyPad_EvenIfExceptionThrown()
    {
        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(10));

        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Throws<InvalidProgramException>();

        Should.Throw<InvalidProgramException>(() => _pump.Listen(CancellationToken.None)
                                                         .First(f => f.Type != EventType.Start));

        _cursesMock.Verify(v => v.delwin(new(10)), Times.Once);
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen2_CallsCurses_ForDummyPad()
    {
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent ce) =>
                   {
                       ce = new CursesCharEvent('a');
                       return 0;
                   });

        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(10));

        _pump.Listen(CancellationToken.None)
             .First(f => f.Type != EventType.Start)
             .ToString();

        _cursesMock.Verify(s => s.wget_event(new(10), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!), Times.Once);
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen3_Calls_Listen1()
    {
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent ce) =>
                   {
                       ce = new CursesCharEvent('a');
                       return 0;
                   });

        _pump.Listen(_window)
             .First(f => f.Type != EventType.Start)
             .ToString();

        _cursesMock.Verify(s => s.wget_event(_window.Handle, It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!),
            Times.Once);
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Listen4_Calls_Listen2()
    {
        _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent ce) =>
                   {
                       ce = new CursesCharEvent('a');
                       return 0;
                   });

        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(10));

        _pump.Listen()
             .First(f => f.Type != EventType.Start)
             .ToString();

        _cursesMock.Verify(s => s.wget_event(new(10), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!), Times.Once);
    }

    [TestMethod]
    public void Delegate_Throws_IfObjectIsNull() { Should.Throw<ArgumentNullException>(() => _pump.Delegate(null!)); }

    [TestMethod, Timeout(Timeout)]
    public void Delegate_EnqueuesObjectForProcessing()
    {
        _pump.Delegate("hello");

        _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(1));

        foreach (var e in _pump.Listen(_source.Token)
                               .Where(e => e.Type != EventType.Start && e.Type != EventType.Stop))
        {
            _source.Cancel();

            e.ShouldBe(new DelegateEvent("hello"));
        }
    }
}
