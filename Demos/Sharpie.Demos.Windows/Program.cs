/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
using Sharpie.Backend;

[assembly: ExcludeFromCodeCoverage]

#pragma warning disable CA1416 // Validate platform compatibility -- this is a demo
using var terminal = new Terminal(CursesBackend.Load(), new(CaretMode: CaretMode.Visible, ManagedWindows: true));
#pragma warning restore CA1416 // Validate platform compatibility

var rnd = new Random();

short c = 100;

void makeWindow()
{
    var x = rnd.Next(0, terminal.Screen.Size.Width);
    var y = rnd.Next(0, terminal.Screen.Size.Height);
    var w = rnd.Next(1, terminal.Screen.Size.Width - x);
    var h = rnd.Next(1, terminal.Screen.Size.Height - y);

    var win = terminal.Screen.Window(new(x, y, w, h));


    win.Background = (new(' '),
        new()
        {
            Attributes = VideoAttribute.None,
            ColorMixture = terminal.Colors.MixColors((short) StandardColor.Default, c)
        });

    c++;
}

for (var x = 0; x < 10; x++)
{
    makeWindow();
}

terminal.Screen.Refresh();

terminal.Repeat(t =>
{
    var op = rnd.Next(4);
    var x = rnd.Next(0, t.Screen.Windows.Count());
    var win = t.Screen.Windows.ElementAt(x);
    switch (op)
    {
        case 0:
            win.SendToBack();
            break;
        case 1:
            win.BringToFront();
            break;
        case 2:
            win.Visible = true;
            break;
        case 3:
            win.Visible = false;
            break;
        default:
            break;
    }

    return Task.CompletedTask;
}, 1000);

// Run the main loop.
terminal.Run((_, _) => Task.CompletedTask);
