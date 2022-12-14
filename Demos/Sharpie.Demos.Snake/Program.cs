/*
Copyright (c) 2022, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Diagnostics.CodeAnalysis;
using Sharpie;
using Sharpie.Abstractions;
using Sharpie.Backend;
using Sharpie.Demos.Snake;

[assembly: ExcludeFromCodeCoverage]

// Create the main terminal instance.
using var terminal = new Terminal(NativeCursesProvider.Instance, new(CaretMode: CaretMode.Invisible, UseMouse: false, AllocateHeader: true));

// Configure the header.
terminal.Header!.Background = (new(' '),
    new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Black, StandardColor.Cyan)
    });

// Initialize the game.
var game = new Game(
    new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Red, StandardColor.Default)
    },
    new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Magenta, StandardColor.Default)
    },
    new()
    {
        Attributes = VideoAttribute.Blink,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Default)
    },
    new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Default, StandardColor.Red)
    });

void RedrawHeader(ITerminalSurface header)
{
    header.Clear();
    header.WriteText($"Score: {game.Score}");
    header.Refresh();
}

void RedrawScreen(IScreen screen)
{
    screen.Clear();
    screen.DrawBorder();
    game.Update(screen);
    screen.Refresh();
}

terminal.Repeat(t =>
{
    var score = game.Score;
    if (game.Tick())
    {
        game.Update(t.Screen);
        t.Screen.Refresh();

        if (game.Score != score)
        {
            RedrawHeader(t.Header!);
        }
    }

    return Task.CompletedTask;
}, 100);

// Run the main loop.
terminal.Run((t, e) =>
{
    switch (e)
    {
        case TerminalResizeEvent or StartEvent:
        {
            using (t.AtomicRefresh())
            {
                RedrawHeader(t.Header!);
                RedrawScreen(t.Screen);
            }

            game.ResetSize(new(1, 1, t.Screen.Size.Width - 2, t.Screen.Size.Height - 2));
            break;
        }
        case KeyEvent { Key: Key.KeypadUp }:
            game.Turn(Game.Direction.Up);
            break;
        case KeyEvent { Key: Key.KeypadDown }:
            game.Turn(Game.Direction.Down);
            break;
        case KeyEvent { Key: Key.KeypadLeft }:
            game.Turn(Game.Direction.Left);
            break;
        case KeyEvent { Key: Key.KeypadRight }:
            game.Turn(Game.Direction.Right);
            break;
    }

    return Task.FromResult(true);
});
