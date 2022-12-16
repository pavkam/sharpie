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
///     Represents a screen-bound surface and contains all its functionality.
/// </summary>
[PublicAPI]
public class TerminalSurface: Surface, ITerminalSurface
{
    /// <summary>
    ///     Initializes the surface using a Curses handle.
    /// </summary>
    /// <param name="parent">The parent terminal.</param>
    /// <param name="handle">The surface handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    /// <remarks>This method is not thread-safe.</remarks>
    internal TerminalSurface(Terminal parent, IntPtr handle): base(parent != null! ? parent.Curses : null!, handle) =>
        Terminal = parent!;

    /// <inheritdoc cref="ITerminalSurface.Terminal" />
    public Terminal Terminal { get; }

    /// <inheritdoc cref="ITerminalSurface.Terminal" />
    ITerminal ITerminalSurface.Terminal => Terminal;

    /// <inheritdoc cref="ITerminalSurface.ImmediateRefresh" />
    public bool ImmediateRefresh
    {
        get
        {
            AssertSynchronized();
            return Curses.is_immedok(Handle);
        }
        set
        {
            AssertSynchronized();
            Curses.immedok(Handle, value);
        }
    }

    /// <inheritdoc cref="ITerminalSurface.Refresh()" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public virtual void Refresh()
    {
        AssertSynchronized();
        Terminal.Refresh(this);
    }

    /// <inheritdoc cref="ITerminalSurface.Refresh(int, int)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public virtual void Refresh(int y, int count)
    {
        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (Terminal.AtomicRefreshOpen)
        {
            throw new InvalidOperationException("Cannot perform a line based refresh within an atomic refresh block.");
        }

        var (actY, actCount) = Helpers.IntersectSegments(y, count, 0, Size.Height);

        if (actY > -1 && actCount > 0)
        {
            Curses.wredrawln(Handle, actY, actCount)
                  .Check(nameof(Curses.wredrawln), "Failed to perform line refresh.");
        }
    }

    /// <inheritdoc cref="Surface.AssertSynchronized" />
    protected internal override void AssertSynchronized()
    {
        if (Terminal != null!)
        {
            Terminal.AssertSynchronized();
        }
    }
}

