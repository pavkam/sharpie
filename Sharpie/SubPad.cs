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
///     Represents a Curses sub-pad and contains all it's functionality.
/// </summary>
[PublicAPI]
public sealed class SubPad: Surface, ISubPad
{
    /// <summary>
    ///     Initializes the sub-pad using the given Curses handle.
    /// </summary>
    /// <param name="curses">The Curses backend.</param>
    /// <param name="parent">The parent pad.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="curses" /> or <paramref name="parent" /> are <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal SubPad(ICursesProvider curses, Pad parent, IntPtr handle): base(curses, handle)
    {
        EnableScrolling = true;
        
        Pad = parent ?? throw new ArgumentNullException(nameof(parent));
        parent.AddChild(this);
    }

    /// <inheritdoc cref="ISubPad.Pad"/>
    public IPad Pad { get; }

    /// <inheritdoc cref="ISubPad.Location"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Point Location
    {
        get =>
            new(Curses.getparx(Handle)
                      .Check(nameof(Curses.getparx), "Failed to get sub-pad X coordinate."), Curses.getpary(Handle)
                .Check(nameof(Curses.getpary), "Failed to get sub-pad Y coordinate."));
        set
        {
            if (!Pad.IsRectangleWithin(new(value, Size)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.mvderwin(Handle, value.Y, value.X)
                  .Check(nameof(Curses.mvderwin), "Failed to move sub-pad to new coordinates.");

        }
    }

    /// <inheritdoc cref="ISubPad.Size"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            if (!Pad.IsRectangleWithin(new(Location, value)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wresize(Handle, value.Height, value.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the sub-pad.");
        }
    }

    /// <inheritdoc cref="ISubPad.Duplicate"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ISubPad Duplicate()
    {
        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the sub-pad.");

        return new SubPad(Curses, (Pad) Pad, handle) { ManagedCaret = ManagedCaret };
    }
    
    /// <inheritdoc cref="Surface.Delete"/>
    protected override void Delete()
    {
        if (Pad is Pad p)
        {
            p.RemoveChild(this);
        }

        base.Delete();
    }
}
