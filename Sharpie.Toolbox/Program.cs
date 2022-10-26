using System.Text;
using Sharpie;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
                       .WithSoftKeyLabels(SoftKeyLabelMode.ThreeTwoThree)
        .Create();

Console.Write(terminal.Name);

terminal.Screen.WriteText("Hello", Style.Default);
terminal.Screen.MoveCaretTo(terminal.Screen.CaretPosition.X + 5, terminal.Screen.CaretPosition.Y);
terminal.Screen.WriteText("World", new()
{
    Attributes = VideoAttribute.Bold,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Blue, StandardColor.Green)
});
terminal.Screen.MoveCaretTo(terminal.Screen.CaretPosition.X + 5, terminal.Screen.CaretPosition.Y);
terminal.SoftKeyLabels.SetLabel(0, "Hello", SoftKeyLabelAlignment.Center);
terminal.Screen.ApplyPendingRefreshes();
Rune r;
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
