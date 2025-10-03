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
public class TerminalMainLoopTests
{
    private const int Timeout = 10000;

    private Mock<ICursesBackend> _cursesMock = null!;
    private Terminal _terminal = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(1));

        _ = _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(new IntPtr(10));

        _ = _cursesMock.Setup(s => s.wget_wch(It.IsAny<IntPtr>(), out It.Ref<uint>.IsAny))
                   .Returns((IntPtr _, out uint kc) =>
                   {
                       kc = 0;
                       return -1;
                   });

        _terminal = new(_cursesMock.Object, new());
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    private Task RunAsync()
    {
        var startedEvent = new ManualResetEvent(false);
        var ra = Task.Run(() => _terminal.Run((_, _) =>
        {
            _ = startedEvent.Set();
            return Task.CompletedTask;
        }));

        _ = startedEvent.WaitOne();

        return ra;
    }

    [TestMethod]
    public void Delegate_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Delegate(null!));

    [TestMethod]
    public void Delegate_Throws_IfTerminalDisposed()
    {
        _terminal.Dispose();
        _ = Should.Throw<ObjectDisposedException>(() => _terminal.Delegate(() => Task.CompletedTask));
    }

    [TestMethod]
    public void Stop_Throws_IfTerminalDisposed()
    {
        _terminal.Dispose();
        _ = Should.Throw<ObjectDisposedException>(() => _terminal.Stop());
    }

    [TestMethod]
    public void Delay1_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Delay(null!, 1));

    [TestMethod]
    public void Delay1_Throws_IfDelayIsNegative() => Should.Throw<ArgumentOutOfRangeException>(() => _terminal.Delay((_, _) => Task.CompletedTask, -1, "state"));

    [TestMethod]
    public void Delay2_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Delay(null!, 1, "state"));

    [TestMethod]
    public void Delay2_Throws_IfDelayIsNegative() => Should.Throw<ArgumentOutOfRangeException>(() => _terminal.Delay((_, _) => Task.CompletedTask, -1, false));

    [TestMethod]
    public void Repeat1_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Repeat(null!, 1));

    [TestMethod]
    public void Repeat1_Throws_IfDelayIsNegative()
    {
        _ = Should.Throw<ArgumentOutOfRangeException>(() =>
            _terminal.Repeat((_, _) => Task.CompletedTask, -1, false, "state"));
    }

    [TestMethod]
    public void Repeat2_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Repeat(null!, 1));

    [TestMethod]
    public void Repeat2_Throws_IfDelayIsNegative()
    {
        _ = Should.Throw<ArgumentOutOfRangeException>(() =>
            _terminal.Repeat((_, _) => Task.CompletedTask, -1, false, "state"));
    }

    [TestMethod]
    public void Run_Throws_IfActionIsNull() => Should.Throw<ArgumentNullException>(() => _terminal.Run(null!, false));

    [TestMethod]
    public void Run_Throws_IfTerminalDisposed()
    {
        _terminal.Dispose();
        _ = Should.Throw<ObjectDisposedException>(() => _terminal.Run((_, _) => Task.CompletedTask, false));
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Run_Throws_IfAnotherRunsAlready()
    {
        var ra1 = RunAsync();

        _ = Should.Throw<InvalidOperationException>(() => _terminal.Run((_, _) => Task.CompletedTask));

        _terminal.Stop();
        await ra1;
    }

    [TestMethod, Timeout(Timeout)]
    public void Run_Throws_IfInternalError()
    {
        _ = _cursesMock.Setup(s => s.newpad(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(IntPtr.Zero);

        _ = Should.Throw<CursesOperationException>(() => _terminal.Run((_, _) => Task.CompletedTask));
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Run_Resumes_IfStopped()
    {
        var ra1 = RunAsync();
        _terminal.Stop();

        await ra1;

        var ra2 = RunAsync();
        _terminal.Stop();
        await ra2;
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Delegate_DoesNotEnqueueAction_IfNotRunning()
    {
        var executed = false;
        _terminal.Delegate(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        var ra = RunAsync();

        _terminal.Stop();

        await ra;

        executed.ShouldBeFalse();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Stop_DoesNotEnqueueAction_IfNotRunning()
    {
        _terminal.Stop();

        var ra = RunAsync();

        var executed = false;
        _terminal.Delegate(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        _terminal.Stop();

        await ra;

        executed.ShouldBeTrue();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Delegate_EnqueuesAction()
    {
        var ra = RunAsync();

        var finished = false;
        _terminal.Delegate(() =>
        {
            finished = true;
            _terminal.Stop();
            return Task.CompletedTask;
        });

        await ra;

        finished.ShouldBeTrue();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Run_ExecutesDelegatedActionsInSequence()
    {
        var ra = RunAsync();

        var seq = "";
        _terminal.Delegate(() =>
        {
            seq += "1";
            return Task.CompletedTask;
        });

        _terminal.Delegate(() =>
        {
            seq += "2";
            _terminal.Stop();
            return Task.CompletedTask;
        });

        await ra;

        seq.ShouldBe("12");
    }

    [TestMethod, Timeout(Timeout)]
    public void Run_ReadsEventsFromPump()
    {
        _ = _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent kc) =>
                   {
                       kc = new CursesCharEvent(null, 'A', ModifierKey.None);
                       return 0;
                   });

        var events = new List<Event>();
        _terminal.Run((t, e) =>
        {
            t.ShouldBe(_terminal);
            events.Add(e);

            if (events.Count == 2)
            {
                _terminal.Stop();
            }

            return Task.CompletedTask;
        });

        (events[0] is StartEvent).ShouldBeTrue();
        (events[1] is KeyEvent { Key: Key.Character, Char.Value: 'A' }).ShouldBeTrue();
        (events[2] is StopEvent).ShouldBeTrue();
    }

    [TestMethod, Timeout(Timeout)]
    public void Stop_EnqueuesStoppingTheRun()
    {
        var ch = 'a';
        _ = _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent kc) =>
                   {
                       kc = new CursesCharEvent(null, ch++, ModifierKey.None);
                       return 0;
                   });

        _terminal.Run((_, e) =>
        {
            if (e.Type == EventType.KeyPress)
            {
                _terminal.Stop();
            }

            return Task.CompletedTask;
        });

        ch.ShouldNotBe('a');
    }

    [TestMethod, Timeout(Timeout), SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public async Task Stop_WaitsForThingsToFinish()
    {
        var ra = RunAsync();
        _terminal.Stop(true);

        await Should.NotThrowAsync(ra);
    }

    [TestMethod, Timeout(Timeout)]
    public void Run_StopsOnCtrlC()
    {
        _ = _cursesMock.Setup(s => s.wget_event(It.IsAny<IntPtr>(), It.IsAny<int>(), out It.Ref<CursesEvent>.IsAny!))
                   .Returns((IntPtr _, int _, out CursesEvent kc) =>
                   {
                       kc = new CursesCharEvent(null, (char) 3, ModifierKey.None);
                       return 0;
                   });

        var reached = false;
        _terminal.Run((_, e) =>
        {
            reached = e.Type == EventType.KeyPress;
            return Task.CompletedTask;
        });

        reached.ShouldBeFalse();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Delay1_EnqueuesAndExecutesTimerOnce()
    {
        var finished = false;
        var d = _terminal.Delay((t, s) =>
        {
            s.ShouldBe("state");
            t.ShouldBe(_terminal);

            finished = true;
            _terminal.Stop();
            return Task.CompletedTask;
        }, 100, "state");

        _ = d.ShouldNotBeNull();

        await RunAsync();

        finished.ShouldBeTrue();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Delay1_CanBeCancelledThroughReturnedInterval()
    {
        var executed = false;
        var d = _terminal.Delay(_ =>
        {
            executed = true;
            return Task.CompletedTask;
        }, 50);

        d.Stop();

        var stopTask = Task.Run(async () =>
        {
            await Task.Delay(100);
            _terminal.Stop();
        });

        await RunAsync();
        await stopTask;

        executed.ShouldBeFalse();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Delay2_EnqueuesAndExecutesTimerOnce()
    {
        var ra = RunAsync();

        var finished = false;
        _ = _terminal.Delay(t =>
        {
            finished = true;
            t.Stop();
            return Task.CompletedTask;
        }, 10);

        await ra;

        finished.ShouldBeTrue();
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Repeat1_EnqueuesAndExecutesTimerMultipleTimes()
    {
        var cycles = 0;
        var d = _terminal.Repeat((t, s) =>
        {
            s.ShouldBe("state");
            t.ShouldBe(_terminal);

            cycles++;

            if (cycles >= 10)
            {
                _terminal.Stop();
            }

            return Task.CompletedTask;
        }, 10, false, "state");

        _ = d.ShouldNotBeNull();

        await RunAsync();

        cycles.ShouldBe(10);
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Repeat1_CanBeCancelledThroughReturnedInterval()
    {
        var executed = 0;
        var d = _terminal.Repeat(_ =>
        {
            executed++;
            return Task.CompletedTask;
        }, 50);

        var stopTask = Task.Run(async () =>
        {
            await Task.Delay(100);
            d.Stop();
            await Task.Delay(100);
            _terminal.Stop();
        });

        await RunAsync();
        await stopTask;

        executed.ShouldBeInRange(1, 3);
    }

    [TestMethod, Timeout(Timeout)]
    public async Task Dispose_CancelsTheRunning_AndKillsTimers()
    {
        var ra = RunAsync();
        var count = Timer.ActiveCount;
        var tickedEvent = new ManualResetEvent(false);
        _ = _terminal.Repeat(_ =>
        {
            _ = tickedEvent.Set();
            return Task.CompletedTask;
        }, 10);

        _ = tickedEvent.WaitOne();
        _terminal.Dispose();
        await ra;

        await Task.Delay(500);
        Timer.ActiveCount.ShouldBeLessThanOrEqualTo(count);
    }

    [TestMethod, Timeout(Timeout)]
    public void Run_ResumesTimersAcrossRuns()
    {
        var m = new ManualResetEvent(false);

        _ = _terminal.Repeat(_ =>
        {
            _ = m.Set();
            return Task.CompletedTask;
        }, 10);

        _ = RunAsync();
        _ = m.WaitOne();
        _terminal.Stop(true);

        _ = m.Reset();
        _ = RunAsync();
        _ = m.WaitOne();
        _terminal.Stop(true);
    }
}
