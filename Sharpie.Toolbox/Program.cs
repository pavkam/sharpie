using Sharpie;

var terminal = Terminal.UsingCurses(NativeCursesProvider.Instance)
        .Create();

terminal.Screen.WriteText("Hello", Style.Default);
terminal.Screen.MoveCaretTo(terminal.Screen.CaretPosition.X + 5, terminal.Screen.CaretPosition.Y);
terminal.Screen.WriteText("World", new()
{
    Attributes = VideoAttribute.Bold,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Blue, StandardColor.Green)
});
terminal.Screen.MoveCaretTo(terminal.Screen.CaretPosition.X + 5, terminal.Screen.CaretPosition.Y);

terminal.Screen.ApplyPendingRefreshes();
terminal.Screen.TryReadKey(ReadBehavior.Wait);
terminal.Screen.TryReadKey(ReadBehavior.Wait);
terminal.Screen.TryReadKey(ReadBehavior.Wait);
