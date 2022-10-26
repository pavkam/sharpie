using System.Text;
using Sharpie;
using Sharpie.Curses;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
                       .WithSoftKeyLabels(SoftKeyLabelMode.ThreeTwoThree)
                       //.WithMouse()
                       .Create();

Console.Write(terminal.Name);

terminal.Screen.WriteText("Testing\n", new()
{
    Attributes = VideoAttribute.Bold,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Blue, StandardColor.Green)
});

terminal.SoftKeyLabels.SetLabel(0, "Hello", SoftKeyLabelAlignment.Center);
terminal.Screen.ApplyPendingRefreshes();

while (true) {
    if (!terminal.Screen.TryReadEvent(ReadBehavior.Wait, out var e) || e == null)
    {
        continue;
    }

    if (e.Char.Value == 'q')
    {
        break;
    }

    terminal.Screen.WriteText($"{e.Type} -- {e.Key} -- {e.Char.Value} -- {e.Modifier} -- {e.MouseButton} -- {e.MouseButtonState}\n", Style.Default);
}


terminal.Dispose();
