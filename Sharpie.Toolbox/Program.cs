using System.Diagnostics.CodeAnalysis;
using Sharpie;

[assembly: ExcludeFromCodeCoverage]

var terminal = new Terminal(NativeCursesProvider.Instance, new(UseMouse: false));

foreach (var @event in terminal.Screen.ProcessEvents(CancellationToken.None))
{
    terminal.Screen.WriteText($"{@event}\n", Style.Default);
    if (@event is KeyEvent { Key: Key.Interrupt })
    {
        break;
    }
}

terminal.Dispose();
