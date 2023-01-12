/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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

[assembly: ExcludeFromCodeCoverage]

// Create the main terminal instance and enable 4 * 4 SLK mode,
using var terminal = new Terminal(CursesBackend.Load(),
    new(CaretMode: CaretMode.Invisible, UseMouse: true, SoftLabelKeyMode: SoftLabelKeyMode.FourFour,
        AllocateHeader: true));

// Configure SLK style.
terminal.SoftLabelKeys.Style = new()
{
    Attributes = VideoAttribute.Bold | VideoAttribute.Underline,
    ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Red)
};

// Prepare the colors.
var index = 0;
var colors = new List<ColorMixture>();
foreach (var n in Enum.GetValues<StandardColor>()
                      .Where(s => s != StandardColor.Default))
{
    terminal.SoftLabelKeys.SetLabel(index++, n.ToString(), SoftLabelKeyAlignment.Left);
    colors.Add(terminal.Colors.MixColors(n, n));
}

void DrawHeader(ITerminal t)
{
    t.Header!.CaretLocation = new(0, 0);
    t.Header.WriteText("Press a number from 1 to 8 to change the color.");
    t.Header.DrawHorizontalLine(t.Header.Size.Width - t.Header.CaretLocation.X);
    t.Header.Refresh();
}

DrawHeader(terminal);
terminal.SoftLabelKeys.Refresh();

// Run the main loop.
terminal.Run((t, @event) =>
{
    switch (@event)
    {
        case TerminalResizeEvent:
            DrawHeader(t);
            t.SoftLabelKeys.Refresh();
            break;
        case KeyEvent { Key: Key.Character, Char.Value: var k and >= '1' and <= '8' }:
        {
            var color = k - '1';

            t.Screen.Background = (new(' '), new() { Attributes = VideoAttribute.None, ColorMixture = colors[color] });

            t.Screen.Refresh();
            t.SoftLabelKeys.Refresh();
            break;
        }
    }

    return Task.CompletedTask;
});
