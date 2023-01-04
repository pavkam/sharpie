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

[assembly: ExcludeFromCodeCoverage]

// Create a new terminal instance with an invisible cursor.
using var terminal = new Terminal(CursesBackend.Load(), new(CaretMode: CaretMode.Invisible));

// Setup the message and a number of rotating styles that will be applied for each letter of the message.
var message = "\x001 Let the ASCII fun begin! \x003";
var styles = Enumerable.Range(0, message.Length)
                       .Select(i => new Style
                       {
                           Attributes = VideoAttribute.None,
                           ColorMixture = terminal.Colors.MixColors((short) (i + 10), (short) StandardColor.Default)
                       })
                       .ToArray();

// This method draws the given string and applies color starting with a specific shift.
void DrawFunAsciiMessage(ITerminal t, string str, int colorShift)
{
    var x = 0;
    var y = 0;

    foreach (var ch in str)
    {
        var gl = new AsciiGlyph((byte) ch, styles[colorShift % styles.Length]);
        t.Screen.Draw(new(x, y), gl);

        x += gl.Size.Width;
        if (x >= t.Screen.Size.Width - gl.Size.Width)
        {
            x = 0;
            y += gl.Size.Height;
        }

        colorShift++;
    }
}

// A repeating timer that draws the message with different colors.
var colorShift = 0;
terminal.Repeat(t =>
{
    DrawFunAsciiMessage(t, message, colorShift++);
    t.Screen.Refresh();

    return Task.CompletedTask;
}, 100);

// The main loop -- we need to monitor for resizes.
terminal.Run((t, e) =>
{
    if (e is TerminalResizeEvent)
    {
        t.Screen.Clear();
    }

    return Task.CompletedTask;
});
