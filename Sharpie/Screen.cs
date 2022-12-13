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
///     The main Curses screen class. Implements screen-related functionality.
///     Only one instance of this class can be active at one time.
/// </summary>
[PublicAPI]
public sealed class Screen: TerminalSurface, IScreen
{
    private readonly IList<Pad> _pads = new List<Pad>();
    private readonly object _syncRoot = new();
    private readonly IList<Window> _windows = new List<Window>();

    /// <summary>
    ///     Initializes the pad using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent terminal.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal Screen(Terminal parent, IntPtr handle): base(parent, handle)
    {
        Curses.notimeout(Handle, false)
              .Check(nameof(Curses.notimeout), "Failed to disable no-read-timeout mode.");

        Curses.keypad(Handle, true)
              .Check(nameof(Curses.keypad), "Failed to enable the keypad resolution mode.");

        Curses.syncok(Handle, true)
              .Check(nameof(Curses.syncok), "Failed to enable auto-sync mode.");
    }

    /// <inheritdoc cref="IScreen.Windows" />
    public IEnumerable<Window> Windows => _windows;

    /// <inheritdoc cref="IScreen.Pads" />
    public IEnumerable<IPad> Pads => _pads;

    /// <inheritdoc cref="IScreen.Windows" />
    IEnumerable<IWindow> IScreen.Windows => _windows;

    /// <inheritdoc cref="IScreen.Pads" />
    IEnumerable<IPad> IScreen.Pads => _pads;

    /// <inheritdoc cref="TerminalSurface.Refresh()" />
    public override void Refresh()
    {
        base.Refresh();

        lock (_syncRoot)
        {
            foreach (var child in _windows)
            {
                Terminal.Refresh(child);
            }
        }
    }

    /// <inheritdoc cref="Surface.MarkDirty(int, int)" />
    public override void MarkDirty(int y, int count)
    {
        base.MarkDirty(y, count);
        lock (_syncRoot)
        {
            foreach (var child in Windows)
            {
                var ly = child.Location.Y;
                var (iy, ic) = Helpers.IntersectSegments(y, count, ly, child.Size.Height);

                if (iy > -1 && ic > 0)
                {
                    child.MarkDirty(iy - ly, ic);
                }
            }
        }
    }

    /// <inheritdoc cref="Surface.MarkDirty(int, int)" />
    public override void Refresh(int y, int count)
    {
        base.Refresh(y, count);
        lock (_syncRoot)
        {
            foreach (var child in Windows)
            {
                var ly = child.Location.Y;
                var (iy, ic) = Helpers.IntersectSegments(y, count, ly, child.Size.Height);

                if (iy > -1 && ic > 0)
                {
                    Curses.wredrawln(child.Handle, iy - ly, ic)
                          .Check(nameof(Curses.wredrawln), "Failed to perform line refresh of child.");
                }
            }
        }
    }

    /// <inheritdoc cref="IScreen.Window" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow Window(Rectangle area)
    {
        if (!IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.newwin(area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Curses.newwin), "Failed to create a new window.");

        return new Window(this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="IScreen.Pad" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IPad Pad(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        AssertAlive();
        var handle = Curses.newpad(size.Height, size.Width)
                           .Check(nameof(Curses.newpad), "Failed to create a new pad.");

        return new Pad(this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <summary>
    ///     Registers a window as a child. This is an internal function.
    /// </summary>
    /// <param name="window">The window to register.</param>
    internal void AddChild(Window window)
    {
        Debug.Assert(window != null);
        Debug.Assert(!window.Disposed);
        Debug.Assert(window.Screen == this);
        Debug.Assert(!_windows.Contains(window));

        lock (_syncRoot)
        {
            _windows.Add(window);
        }
    }

    /// <summary>
    ///     Un-registers a window as a child. This is an internal function.
    /// </summary>
    /// <param name="window">The window to un-register.</param>
    internal void RemoveChild(Window window)
    {
        Debug.Assert(window != null);
        Debug.Assert(!window.Disposed);
        Debug.Assert(window.Screen == this);
        Debug.Assert(_windows.Contains(window));

        lock (_syncRoot)
        {
            _windows.Remove(window);
        }
    }

    /// <summary>
    ///     Registers a pad as a child. This is an internal function.
    /// </summary>
    /// <param name="pad">The pad to register.</param>
    internal void AddChild(Pad pad)
    {
        Debug.Assert(pad != null);
        Debug.Assert(!pad.Disposed);
        Debug.Assert(pad.Screen == this);
        Debug.Assert(!_pads.Contains(pad));

        lock (_syncRoot)
        {
            _pads.Add(pad);
        }
    }

    /// <summary>
    ///     Un-registers a pad as a child. This is an internal function.
    /// </summary>
    /// <param name="pad">The pad to un-register.</param>
    internal void RemoveChild(Pad pad)
    {
        Debug.Assert(pad != null);
        Debug.Assert(!pad.Disposed);
        Debug.Assert(pad.Screen == this);
        Debug.Assert(_pads.Contains(pad));

        lock (_syncRoot)
        {
            _pads.Remove(pad);
        }
    }

    /// <summary>
    ///     Marks all the windows on top of <paramref name="window" /> as dirty and refreshes them;
    ///     And then proceeds to do the same for each touched window.
    /// </summary>
    /// <param name="window">The window to start with.</param>
    internal void RefreshUp(Window window)
    {
        Debug.Assert(window != null);
        Debug.Assert(!window.Disposed);
        Debug.Assert(window.Screen == this);
        Debug.Assert(_windows.Contains(window));

        var a1 = new Rectangle(window.Location, window.Size);

        lock (_syncRoot)
        {
            var i = _windows.IndexOf(window);
            for (var j = i + 1; j < _windows.Count; j++)
            {
                var up = _windows[j];

                var a2 = new Rectangle(up.Location, up.Size);
                if (a1.IntersectsWith(a2))
                {
                    up.MarkDirty();
                    Terminal.Refresh(up);
                    
                    RefreshUp(up);
                }
            }
        }
    }

    /// <summary>
    ///     Brings the <paramref name="window" /> to the front of the stack.
    /// </summary>
    /// <param name="window">The window to bring.</param>
    internal void BringToFront(Window window)
    {
        Debug.Assert(window != null);
        Debug.Assert(!window.Disposed);
        Debug.Assert(window.Screen == this);
        Debug.Assert(_windows.Contains(window));

        lock (_syncRoot)
        {
            var i = _windows.IndexOf(window);
            if (i < _windows.Count - 1)
            {
                _windows.RemoveAt(i);
                _windows.Add(window);
            } else
            {
                return;
            }
        }

        window.MarkDirty();
        window.Refresh();
    }

    /// <summary>
    ///     Sends the <paramref name="window" /> to the back of the stack.
    /// </summary>
    /// <param name="window">The window to send.</param>
    internal void SendToBack(Window window)
    {
        Debug.Assert(window != null);
        Debug.Assert(!window.Disposed);
        Debug.Assert(window.Screen == this);
        Debug.Assert(_windows.Contains(window));

        lock (_syncRoot)
        {
            var i = _windows.IndexOf(window);
            if (i > 0)
            {
                _windows.RemoveAt(i);
                _windows.Insert(0, window);
            } else
            {
                return;
            }
        }

        RefreshUp(window);
    }

    /// <summary>
    ///     Adjusts the area of all the windows in the screen.
    /// </summary>
    internal void AdjustChildrenToExplicitArea()
    {
        lock (_syncRoot)
        {
            foreach (var window in _windows.ToArray())
            {
                window.AdjustToExplicitArea();
            }
        }
    }

    /// <inheritdoc cref="Surface.Delete" />
    /// <summary>
    ///     Deletes the screen window.
    /// </summary>
    protected override void Delete()
    {
        lock (_syncRoot)
        {
            foreach (var window in _windows.ToArray())
            {
                window.Destroy();
            }

            foreach (var pad in _pads.ToArray())
            {
                pad.Destroy();
            }

            _pads.Clear();
            _windows.Clear();
        }

        base.Delete();

        Curses.endwin();
    }
}
