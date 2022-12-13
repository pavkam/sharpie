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
using Sharpie.Backend;

[assembly: ExcludeFromCodeCoverage]
using var terminal = new Terminal(NativeCursesProvider.Instance, new(CaretMode: CaretMode.Visible));

var rnd = new Random();

short c = 100;

void MakeWindow()
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

for (var x = 0; x < 50; x++)
{
    MakeWindow();
}

terminal.Screen.Refresh();

terminal.Repeat(t =>
{
    var x = rnd.Next(0, t.Screen.Windows.Count());
    t.Screen.Windows.ElementAt(x)
     .SendToBack();

    return Task.CompletedTask;
}, 1000);

// Run the main loop.
terminal.Run((_, _) => Task.CompletedTask);
