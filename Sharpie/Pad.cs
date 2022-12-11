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
///     Represents a Curses pad, which is a special type of off-screen window.
/// </summary>
[PublicAPI]
public sealed class Pad: Surface, IPad
{
    private readonly IList<SubPad> _subPads = new List<SubPad>();

    /// <summary>
    ///     Initializes the pad using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent screen.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal Pad(Screen parent, IntPtr handle): base(parent != null! ? parent.Curses : null!, handle)
    {
        Screen = parent!;

        parent!.AddChild(this);
    }

    /// <inheritdoc cref="IPad.Screen" />
    IScreen IPad.Screen => Screen;
    
    /// <inheritdoc cref="IPad.Screen" />
    public IScreen Screen { get; }

    /// <inheritdoc cref="IWindow.SubWindows" />
    public IEnumerable<ISubPad> SubPads => _subPads;

    /// <inheritdoc cref="IPad.Size" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            if (!Screen.IsRectangleWithin(new(new(0, 0), value)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wresize(Handle, value.Height, value.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the sub-pad.");
        }
    }

    /// <inheritdoc cref="IPad.Refresh(System.Drawing.Rectangle,System.Drawing.Point)" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh(Rectangle srcArea, Point destLocation)
    {
        if (!((IPad) this).IsRectangleWithin(srcArea))
        {
            throw new ArgumentOutOfRangeException(nameof(srcArea));
        }

        var destRect = srcArea with { X = destLocation.X, Y = destLocation.Y };
        if (!Screen.IsRectangleWithin(destRect))
        {
            throw new ArgumentOutOfRangeException(nameof(destLocation));
        }

        ((Terminal) Screen.Terminal).WithinBatch(batch =>
        {
            if (batch)
            {
                Curses.pnoutrefresh(Handle, srcArea.Top, srcArea.Left, destRect.Top, destRect.Left,
                          destRect.Bottom, destRect.Right)
                      .Check(nameof(Terminal.Curses.pnoutrefresh), "Failed to queue pad refresh.");
            } else
            {
                Curses.prefresh(Handle, srcArea.Top, srcArea.Left, destRect.Top, destRect.Left,
                          destRect.Bottom, destRect.Right)
                      .Check(nameof(Terminal.Curses.prefresh), "Failed to perform pad refresh.");
            }
        });
    }

    /// <inheritdoc cref="IPad.SubPad" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ISubPad SubPad(Rectangle area)
    {
        if (!((IPad) this).IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.subpad(Handle, area.Height, area.Width, area.Top, area.Right)
                           .Check(nameof(Curses.subpad), "Failed to create a new sub-pad.");

        return new SubPad(this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="IPad.Duplicate" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IPad Duplicate()
    {
        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the pad.");

        return new Pad((Screen) Screen, handle) { ManagedCaret = ManagedCaret };
    }

    /// <summary>
    ///     Registers a sub-pad as a child. This is an internal function.
    /// </summary>
    /// <param name="subPad">The sub-pad to register.</param>
    internal void AddChild(SubPad subPad)
    {
        Debug.Assert(subPad != null);
        Debug.Assert(!subPad.Disposed);
        Debug.Assert(subPad.Pad == this);
        Debug.Assert(!_subPads.Contains(subPad));

        _subPads.Add(subPad);
    }

    /// <summary>
    ///     Un-registers a sub-pad as a child. This is an internal function.
    /// </summary>
    /// <param name="subPad">The sub-pad to un-register.</param>
    internal void RemoveChild(SubPad subPad)
    {
        Debug.Assert(subPad != null);
        Debug.Assert(!subPad.Disposed);
        Debug.Assert(subPad.Pad == this);
        Debug.Assert(_subPads.Contains(subPad));

        _subPads.Remove(subPad);
    }

    /// <inheritdoc cref="Surface.Delete" />
    protected override void Delete()
    {
        foreach (var subPad in _subPads.ToArray())
        {
            subPad.Destroy();
        }

        if (Screen is Screen s)
        {
            s.RemoveChild(this);
        }

        base.Delete();
    }
}
