using System.Diagnostics.CodeAnalysis;
using Sharpie;

[assembly: ExcludeFromCodeCoverage]

Environment.SetEnvironmentVariable("TERM", "xterm-1003");

var terminal = new Terminal(NativeCursesProvider.Instance, new());

terminal.Screen.ApplyPendingRefreshes();

while (true)
{
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
