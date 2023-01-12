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
using Sharpie.Backend;

[assembly: ExcludeFromCodeCoverage]

// Create the main terminal instance.
using var terminal = new Terminal(CursesBackend.Load(), new(CaretMode: CaretMode.Invisible));

// Prepare styles
var styles = Enum.GetValues<StandardColor>()
                 .Where(sc => sc != StandardColor.Default && sc != StandardColor.Black)
                 .Select(sc => new Style
                 {
                     ColorMixture = terminal.Colors.MixColors(sc, StandardColor.Default),
                     Attributes = VideoAttribute.Bold
                 })
                 .ToArray();

// Prepare the glyph.
var glyph = new Canvas(new(1, 1));
var glyphStyle = Canvas.TriangleGlyphStyle.Up;
var currentStyle = 0;
var x = -1;
var y = -1;
var dx = 1;
var dy = 1;

// Set up a timer that will animate the glyph.
terminal.Repeat(t =>
{
    glyph.Glyph(new(0, 0), glyphStyle, Canvas.GlyphSize.Normal, Canvas.FillStyle.Black, styles[currentStyle]);

    glyphStyle++;
    if (glyphStyle > Canvas.TriangleGlyphStyle.Right)
    {
        glyphStyle = Canvas.TriangleGlyphStyle.Up;
    }

    x += dx;
    y += dy;

    if (x <= 0)
    {
        x = 0;
        dx = 1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (x >= t.Screen.Size.Width)
    {
        x = t.Screen.Size.Width - 1;
        dx = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (y <= 0)
    {
        y = 0;
        dy = 1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (y >= t.Screen.Size.Height)
    {
        y = t.Screen.Size.Height - 1;
        dy = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    t.Screen.Clear();
    t.Screen.Draw(new(x, y), glyph);
    t.Screen.Refresh();

    return Task.CompletedTask;
}, 50);

// Run the main loop.
terminal.Run((_, _) => Task.FromResult(true));
