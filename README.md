[![Build and Test](https://github.com/pavkam/sharpie/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/pavkam/sharpie/actions/workflows/build-and-test.yml)

# Sharpie
**Sharpie** is a terminal manipulation library based on Curses and targeting .NET 6 (dotnet core).

# Reasons
There are a few libraries out there that already offer bindings to *NCurses*. One of the more used one is [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui). But uthers such as [dotnet-curses](https://github.com/MV10/dotnet-curses) also exist.

So why another one? The are many reasons, but the most important ones are:
1. There is no .NET, object-oriented version of pure Curses,
2. Existing versions are old, or are targeting old versions of .NET which do not benefit from numerous advances in the .NET platform,
3. No other library exposes all of curses functionality,
4. Testing is either very limited or completely non-existent.
5. And finally -- **because I wanted to dabble in Curses**.

# How To
First, you need to build a `Terminal` instance by using the `TerminalBuilder` helper class:
```csharp
var terminal = 
  Terminal.UsingCurses(NativeCursesProvider.Instance)
          .WithSoftKeyLabels(SoftLabelKeyMode.ThreeTwoThree)
          .WithMouse()
          .Build();
```
The `terminal` instance can then be used to access the main screen object `terminal.Screen` which allows drawing and reading terminal commands. Other functionality includes creating windows and pads (`CreateWindow`, `CreatePad`) and manipulating their life-lime.

To set the background of the screen one can easily do it:
```csharp
terminal.Screen.Background = (new('.'), new()
{
    Attributes = VideoAttribute.Dim,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Green)
});
```

The `new('.')` creates a `System.Text.Rune` that will be displayed on each free cell of the screen. The `terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Green)` creates a _color mixture_ which is the combination of a background and foreground colors used in each cell.

To read any events from the terminal one can:
```csharp
while (true) {
    if (!terminal.Screen.TryReadEvent(Timeout.Infinite, out var e))
    {
        continue;
    }

    if (e is KeyEvent { Char.Value: 'q' })
    {
        break;
    }

    terminal.Screen.WriteText($"{e}\n", Style.Default);
}
```

As you can imagine, there are numerous other uses built into the library.
