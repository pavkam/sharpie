using System.Text;
using Sharpie;
using Sharpie.Curses;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
                       .WithSoftKeyLabels(SoftKeyLabelMode.ThreeTwoThree)
                       .WithMouse()
                       .Create();

Console.Write(terminal.Name);
/*
terminal.Screen.Background = (new('.'), new()
{
    Attributes = VideoAttribute.Dim,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Green)
});
*/
var lineStyle = new Style
{
    Attributes = VideoAttribute.Bold,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Green)
};

terminal.Screen.WriteText("Testing\n", new()
{
    Attributes = VideoAttribute.Bold,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Blue, StandardColor.Green)
});

terminal.SoftKeyLabels.SetLabel(0, "Hello", SoftKeyLabelAlignment.Center);
terminal.Screen.ApplyPendingRefreshes();
terminal.Screen.DrawBorder();

while (true) {
    if (!terminal.Screen.TryReadEvent(Timeout.Infinite, out var e) || e == null)
    {
        continue;
    }

    if (e is KeyEvent { Char.Value: 'q' })
    {
        break;
    }

    terminal.Screen.WriteText($"{e}\n", Style.Default);
}


terminal.Dispose();
