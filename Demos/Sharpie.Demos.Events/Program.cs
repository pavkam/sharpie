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

// Create the terminal instance without any non-standard settings.

#pragma warning disable CA1416 // Validate platform compatibility -- this is a demo
using var terminal = new Terminal(CursesBackend.Load(), new(UseStandardKeySequenceResolvers: true));
#pragma warning restore CA1416 // Validate platform compatibility

// Set the main screen attributes for text and drawings.
terminal.Screen.ColorMixture = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Blue);

// Draw a border on the screen.
terminal.Screen.DrawBorder();

// Create a child window within the terminal to operate within.
// The other cells contain the border so we don't want to overwrite those.
var subWindow = terminal.Screen.Window(new(1, 1, terminal.Screen.Size.Width - 2, terminal.Screen.Size.Height - 2));

// Force a refresh so that all drawings will be actually pushed to teh screen.
using (terminal.AtomicRefresh())
{
    terminal.Screen.Refresh();
    subWindow.Refresh();
}

subWindow.WriteText($"Using {terminal.CursesVersion} on {terminal.Name}\n");

// Process all events coming from the terminal.
foreach (var @event in terminal.Events.Listen(subWindow))
{
    // Write the  event that occured.
    subWindow.WriteText($"{@event}\n");

    // If the event is a resize, change the size of the child window
    // to allow for the screen to maintain its border.
    // And then redraw the border of the main screen.
    if (@event is TerminalResizeEvent re)
    {
        subWindow.Size = new(re.Size.Width - 2, re.Size.Height - 2);
        terminal.Screen.DrawBorder();

        using (terminal.AtomicRefresh())
        {
            terminal.Screen.MarkDirty();
            terminal.Screen.Refresh();

            subWindow.MarkDirty();
            subWindow.Refresh();
        }
    }

    // If the user pressed CTRL+C, break the loop.
    if (@event is KeyEvent { Key: Key.Character, Char.IsAscii: true, Char.Value: 'C', Modifiers: ModifierKey.Ctrl })
    {
        break;
    }
}
