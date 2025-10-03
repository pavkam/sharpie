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
using Sharpie.Font;

[assembly: ExcludeFromCodeCoverage]

// Create a new terminal instance with an invisible cursor.
#pragma warning disable CA1416 // Validate platform compatibility -- this is a demo
using var terminal = new Terminal(CursesBackend.Load(), new(CaretMode: CaretMode.Invisible, AllocateHeader: true));
#pragma warning restore CA1416 // Validate platform compatibility

// Setup the message and a number of rotating styles that will be applied for each letter of the message.
var message = "\x001 Let the ASCII fun begin! \x003";
var styles = Enumerable.Range(0, message.Length)
                       .Select(i => new Style
                       {
                           Attributes = VideoAttribute.None,
                           ColorMixture = terminal.Colors.MixColors((short) (i + 10), (short) StandardColor.Default)
                       })
                       .ToArray();

// Load all fonts.
var fonts = new List<IAsciiFont> { DosCp866AsciiFont.FullWidth };
foreach (var path in Directory.EnumerateFiles("Fonts/", "*.flf"))
{
    fonts.Add(await FigletFont.LoadAsync(path));
}

var fontIndex = 0;

// Configure header
terminal.Header!.Background = (new(ControlCharacter.Whitespace),
    new()
    {
        Attributes = VideoAttribute.Bold,
        ColorMixture = terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Magenta)
    });

terminal.Header.WriteText("Press TAB to cycle through fonts.");
terminal.Header.Refresh();

// This method draws the given string and applies color starting with a specific shift.
void drawFunAsciiMessage(ITerminal t, string str, int cs)
{
    var d = fonts[fontIndex]
        .GetGlyphs(str, styles[cs % styles.Length]);

    t.Screen.Draw(new(0, 0), d);
}

// A repeating timer that draws the message with different colors.
var colorShift = 0;
terminal.Repeat(t =>
{
    drawFunAsciiMessage(t, message, colorShift++);
    t.Screen.Refresh();

    return Task.CompletedTask;
}, 100);

// The main loop -- we need to monitor for resizes.
terminal.Run((t, e) =>
{
    switch (e)
    {
        case TerminalResizeEvent:
            t.Screen.Clear();
            break;
        case KeyEvent { Key: Key.Tab }:
            t.Screen.Clear();
            fontIndex = (fontIndex + 1) % fonts.Count;
            break;
        default:
            break;
    }

    return Task.CompletedTask;
});
