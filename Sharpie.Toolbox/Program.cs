
using Sharpie;
using Sharpie.Curses;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
                       .WithMouse()
                       .Build();

terminal.Screen.ApplyPendingRefreshes();

while (true) {
    if (!terminal.Screen.TryReadEvent(Timeout.Infinite, out var e))
    {
        continue;
    }

    if (e is KeyEvent { Key: Key.Interrupt })
    {
        break;
    }

    terminal.Screen.WriteText($"{e}\n", Style.Default);
}


terminal.Dispose();
