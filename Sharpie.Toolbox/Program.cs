using System.Diagnostics.CodeAnalysis;
using Sharpie;

[assembly: ExcludeFromCodeCoverage]

using var terminal = new Terminal(NativeCursesProvider.Instance, new());

terminal.Screen.ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Blue);
terminal.Screen.DrawBorder();
terminal.Screen.Refresh();

var subWindow = terminal.Screen.CreateWindow(
    new(1, 1, terminal.Screen.Size.Width - 2, terminal.Screen.Size.Height - 2));
var subSubWindow = terminal.Screen.CreateSubWindow(subWindow,
    new(1, 1, subWindow.Size.Width - 2, subWindow.Size.Height - 2));
var subSubSubWindow = terminal.Screen.CreateSubWindow(subSubWindow,
    new(1, 1, subSubWindow.Size.Width - 5, subSubWindow.Size.Height - 2));

foreach (var @event in subSubSubWindow.ProcessEvents(CancellationToken.None))
{
    subSubSubWindow.WriteText($"{@event}\n");
    if (@event is TerminalResizeEvent re)
    {
        subWindow.Size = new(re.Size.Width - 2, re.Size.Height - 2);
        subSubWindow.Size = new(subWindow.Size.Width - 2, subWindow.Size.Height - 2);
        subSubSubWindow.Size = new(subSubWindow.Size.Width - 2, subSubWindow.Size.Height - 2);
        terminal.Screen.DrawBorder();
    }

    if (@event is KeyEvent { Char.Value: 'C', Modifiers: ModifierKey.Ctrl })
    {
        break;
    }
}
