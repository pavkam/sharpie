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

namespace Sharpie;

/// <summary>
///     Represents a Curses window and contains all it's functionality.
/// </summary>
[PublicAPI]
public sealed class ScreenArea: Surface, IScreenArea
{
    /// <summary>
    ///     Initializes the window using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent terminal.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal ScreenArea(Terminal parent, IntPtr handle): base(parent != null! ? parent.Curses : null!, handle) =>
        Terminal = parent!;

    /// <inheritdoc cref="IScreenArea.Terminal"/>
    public ITerminal Terminal { get; }

    /// <inheritdoc cref="IScreenArea.ImmediateRefresh"/>
    public bool ImmediateRefresh
    {
        get => Curses.is_immedok(Handle);
        set => Curses.immedok(Handle, value);
    }

    /// <inheritdoc cref="IScreenArea.Refresh"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh(bool batch = false)
    {
        if (batch)
        {
            Curses.wnoutrefresh(Handle)
                  .Check(nameof(Curses.wnoutrefresh), "Failed to queue window refresh.");
        } else
        {
            Curses.wrefresh(Handle)
                  .Check(nameof(Curses.wrefresh), "Failed to perform window refresh.");
        }
    }
}
