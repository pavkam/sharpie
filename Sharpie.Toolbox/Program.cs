using System.Diagnostics.CodeAnalysis;
using Sharpie;

[assembly: ExcludeFromCodeCoverage]

var terminal = new Terminal(NativeCursesProvider.Instance, new());

terminal.Screen.ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Blue);
terminal.Screen.DrawBorder();
terminal.Screen.Refresh();

var subWindow = terminal.Screen.CreateWindow(
    new(1, 1, terminal.Screen.Size.Width - 2, terminal.Screen.Size.Height - 2));

NativeCursesProvider.Instance.define_key("\x001b\x0109", 1024);

foreach (var @event in subWindow.ProcessEvents(CancellationToken.None))
{
    subWindow.WriteText($"{@event}\n");
    if (@event is TerminalResizeEvent re)
    {
        subWindow.Size = new(re.Size.Width - 2, re.Size.Height - 2);
        terminal.Screen.DrawBorder();
    }
    
    if (@event is KeyEvent { Char.Value: 'C', Modifiers: ModifierKey.Ctrl })
    {
        break;
    }
}
Console.WriteLine();
// Dispose this thing.
terminal.Dispose();
