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

namespace Sharpie;

/// <summary>
///     Represents a Curses sub-pad and contains all it's functionality.
/// </summary>
[PublicAPI, DebuggerDisplay("{ToString(), nq}")]
public sealed class SubPad: Surface, ISubPad
{
    /// <summary>
    ///     Initializes the sub-pad using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent pad.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal SubPad(Pad parent, IntPtr handle) : base(parent != null! ? parent.Curses : null!, handle)
    {
        Pad = parent!;
        parent!.AddChild(this);
    }

    /// <inheritdoc cref="ISubPad.Pad" />
    public Pad Pad
    {
        get;
    }

    /// <summary>
    ///     Returns the value of <see cref="Location" />.
    /// </summary>
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    protected internal override Point Origin => Location;

    /// <inheritdoc cref="ISubPad.Pad" />
    IPad ISubPad.Pad => Pad;

    /// <inheritdoc cref="ISubPad.Location" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public Point Location
    {
        get
        {
            AssertSynchronized();

            return new(Curses.getparx(Handle)
                             .Check(nameof(Curses.getparx), "Failed to get sub-pad X coordinate."), Curses
                .getpary(Handle)
                .Check(nameof(Curses.getpary), "Failed to get sub-pad Y coordinate."));
        }
        set
        {
            if (!Pad.IsRectangleWithin(new(value, Size)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _ = Curses.mvderwin(Handle, value.Y, value.X)
                  .Check(nameof(Curses.mvderwin), "Failed to move sub-pad to new coordinates.");
        }
    }

    /// <inheritdoc cref="ISubPad.Size" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            var area = new Rectangle(Location, value);
            if (!Pad.Size.AdjustToActualArea(ref area))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _ = Curses.wresize(Handle, area.Height, area.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the sub-pad.");
        }
    }

    /// <inheritdoc cref="ISubPad.Duplicate" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public ISubPad Duplicate()
    {
        AssertSynchronized();

        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the sub-pad.");

        return new SubPad(Pad, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="Surface.AssertSynchronized" />
    protected internal override void AssertSynchronized()
    {
        if (Pad != null!)
        {
            Pad.AssertSynchronized();
        }
    }

    /// <inheritdoc cref="Surface.Delete" />
    protected override void Delete()
    {
        AssertSynchronized();

        if (Pad != null!)
        {
            Pad.RemoveChild(this);
        }

        base.Delete();
    }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() =>
        $"{GetType().Name} #{Handle:X8} ({Size.Width}x{Size.Height} @ {Location.X}x{Location.Y})";
}
