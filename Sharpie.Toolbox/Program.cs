using System.Text;
using Sharpie;
using Sharpie.Curses;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
                       .WithSoftKeyLabels(SoftKeyLabelMode.ThreeTwoThree)
                       .WithMouse()
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

Event r;
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
terminal.Screen.TryReadEvent(ReadBehavior.Wait, out r);
