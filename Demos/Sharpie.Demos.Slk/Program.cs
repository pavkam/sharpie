using System.Diagnostics.CodeAnalysis;
using Sharpie;
using Sharpie.Backend;

[assembly: ExcludeFromCodeCoverage]

// Create the main terminal instance and enable 4 * 4 SLK mode,
using var terminal = new Terminal(NativeCursesProvider.Instance,
    new(CaretMode: CaretMode.Invisible, SoftLabelKeyMode: SoftLabelKeyMode.FourFour));

// Configure SLK style.
terminal.SoftLabelKeys.Style = new()
{
    Attributes = VideoAttribute.Bold | VideoAttribute.Underline,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Red)
};

// Prepare the colors.
var index = 0;
var colors = new List<ColorMixture>();
foreach (var n in Enum.GetValues<StandardColor>().Where(s => s != StandardColor.Default))
{
    terminal.SoftLabelKeys.SetLabel(index++, n.ToString(), SoftLabelKeyAlignment.Left);
    colors.Add(terminal.Colors.MixColors(n, n));
}

terminal.SoftLabelKeys.Refresh();

// Run the main loop.
await terminal.RunAsync(@event =>
{
    if (@event is KeyEvent { Key: Key.Character, Char.Value: var k and >= '1' and <= '8' })
    {
        var color = k - '1';
        
        terminal.Screen.Background = (new(' '), new() { Attributes = VideoAttribute.None, ColorMixture = colors[color]});
        terminal.Screen.Refresh();
        terminal.SoftLabelKeys.Refresh();
    }
    
    return Task.CompletedTask;
});
