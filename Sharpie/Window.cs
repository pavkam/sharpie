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
///     Represents a Curses window and contains all it's functionality.
/// </summary>
[PublicAPI, DebuggerDisplay("{ToString(), nq}")]
public sealed class Window: TerminalSurface, IWindow
{
    private readonly IList<SubWindow> _subWindows = new List<SubWindow>();
    private Rectangle _explicitArea;
    private IReadOnlyList<SubWindow> _roSubWindows = Array.Empty<SubWindow>();
    private bool _visible = true;

    /// <summary>
    ///     Initializes the window using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent screen.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    /// <remarks>This method is not thread-safe.</remarks>
    internal Window(Screen parent, IntPtr handle) : base(parent != null! ? parent.Terminal : null!, handle)
    {
        _ = Curses.keypad(Handle, true)
              .Check(nameof(Curses.keypad), "Failed to enable the keypad resolution mode.");

        _ = Curses.syncok(Handle, true)
              .Check(nameof(Curses.syncok), "Failed to enable auto-sync mode.");

        Screen = parent!;
        parent!.AddChild(this);

        _explicitArea = new(Location, Size);
    }

    /// <inheritdoc cref="IWindow.Screen" />
    public Screen Screen
    {
        get;
    }

    /// <summary>
    ///     Returns the value of <see cref="Location" />.
    /// </summary>
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    protected internal override Point Origin => Location;

    /// <inheritdoc cref="IWindow.Screen" />
    IScreen IWindow.Screen => Screen;

    /// <inheritdoc cref="IWindow.SubWindows" />
    public IEnumerable<ISubWindow> SubWindows
    {
        get
        {
            AssertAlive();
            AssertSynchronized();

            return _roSubWindows;
        }
    }

    /// <inheritdoc cref="IWindow.Location" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public Point Location
    {
        get
        {
            AssertSynchronized();
            return new(Curses.getbegx(Handle)
                             .Check(nameof(Curses.getbegx), "Failed to get window X coordinate."), Curses
                .getbegy(Handle)
                .Check(nameof(Curses.getbegy), "Failed to get window Y coordinate."));
        }
        set
        {
            var size = Size;
            if (!Screen.IsRectangleWithin(new(value, size)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _ = Curses.mvwin(Handle, value.Y, value.X)
                  .Check(nameof(Curses.mvwin), "Failed to move window to new coordinates.");

            _explicitArea = new(value, size);
        }
    }

    /// <inheritdoc cref="IWindow.Size" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            var area = new Rectangle(Location, value);
            if (!Screen.Size.AdjustToActualArea(ref area))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _ = Curses.wresize(Handle, area.Height, area.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the window.");

            _explicitArea = area;
        }
    }

    /// <inheritdoc cref="TerminalSurface.Refresh()" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public override void Refresh()
    {
        AssertSynchronized();

        if (Screen.ManagedWindows)
        {
            if (_visible)
            {
                using (Terminal.AtomicRefresh())
                {
                    base.Refresh();
                    Screen.RefreshUp(this);
                }
            }
        }
        else
        {
            base.Refresh();
        }
    }

    /// <inheritdoc cref="TerminalSurface.Refresh(int, int)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public override void Refresh(int y, int count)
    {
        AssertSynchronized();

        if (Screen.ManagedWindows)
        {
            if (_visible)
            {
                base.Refresh(y, count);

                using (Terminal.AtomicRefresh())
                {
                    Screen.RefreshUp(this);
                }
            }
        }
        else
        {
            base.Refresh(y, count);
        }
    }

    /// <inheritdoc cref="IWindow.SendToBack" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void SendToBack()
    {
        AssertManagedWindows();
        AssertSynchronized();

        using (Terminal.AtomicRefresh())
        {
            Screen.SendToBack(this);
        }
    }

    /// <inheritdoc cref="IWindow.BringToFront" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void BringToFront()
    {
        AssertManagedWindows();
        AssertSynchronized();

        using (Terminal.AtomicRefresh())
        {
            Screen.BringToFront(this);
        }
    }

    /// <inheritdoc cref="IWindow.Visible" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public bool Visible
    {
        get
        {
            AssertManagedWindows();
            AssertSynchronized();

            return _visible;
        }
        set
        {
            AssertManagedWindows();
            AssertSynchronized();

            if (value != _visible)
            {
                _visible = value;
                using (Terminal.AtomicRefresh())
                {
                    Screen.ChangeVisibility(this, value);
                }
            }
        }
    }

    /// <inheritdoc cref="IWindow.SubWindow" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public ISubWindow SubWindow(Rectangle area)
    {
        if (!Area.AdjustToActualArea(ref area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.derwin(Handle, area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Curses.derwin), "Failed to create a new sub-window.");

        return new SubWindow(this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="IWindow.Duplicate" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public IWindow Duplicate()
    {
        AssertSynchronized();

        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the window.");

        return new Window(Screen, handle) { ManagedCaret = ManagedCaret };
    }

    private void AssertManagedWindows()
    {
        if (!Screen.ManagedWindows)
        {
            throw new InvalidOperationException("This operation is only available when windows are managed.");
        }
    }

    /// <summary>
    ///     Adjusts the window's area to be as close (or equal) to its explicit area.
    /// </summary>
    internal void AdjustToExplicitArea()
    {
        AssertSynchronized();

        var screenSize = Screen.Size;
        var size = Size;
        var location = Location;

        var w = Math.Max(0, Math.Min(screenSize.Width - _explicitArea.X, _explicitArea.Width));
        var h = Math.Max(0, Math.Min(screenSize.Height - _explicitArea.Y, _explicitArea.Height));

        if (w != size.Width || h != size.Height)
        {
            _ = Curses.wresize(Handle, h, w);
        }

        var x = Math.Min(location.X, _explicitArea.X);
        var y = Math.Min(location.Y, _explicitArea.Y);

        if (location.X != x || location.Y != y)
        {
            _ = Curses.mvwin(Handle, y, x);
        }
    }

    /// <summary>
    ///     Registers a sub-window as a child. This is an internal function.
    /// </summary>
    /// <param name="subWindow">The sub-window to register.</param>
    /// <remarks>This method is not thread-safe.</remarks>
    internal void AddChild(SubWindow subWindow)
    {
        Debug.Assert(subWindow != null);
        Debug.Assert(!subWindow.Disposed);
        Debug.Assert(subWindow.Window == this);
        Debug.Assert(!_roSubWindows.Contains(subWindow));

        _subWindows.Add(subWindow);
        _roSubWindows = _subWindows.ToArray();
    }

    /// <summary>
    ///     Un-registers a sub-window as a child. This is an internal function.
    /// </summary>
    /// <param name="subWindow">The sub-window to un-register.</param>
    /// <remarks>This method is not thread-safe.</remarks>
    internal void RemoveChild(SubWindow subWindow)
    {
        Debug.Assert(subWindow != null);
        Debug.Assert(!subWindow.Disposed);
        Debug.Assert(subWindow.Window == this);
        Debug.Assert(_roSubWindows.Contains(subWindow));

        _ = _subWindows.Remove(subWindow);
        _roSubWindows = _subWindows.ToArray();
    }

    /// <inheritdoc cref="Surface.Delete" />
    protected override void Delete()
    {
        AssertSynchronized();

        foreach (var window in _roSubWindows)
        {
            window.Destroy();
        }

        if (Screen != null!)
        {
            Screen.RemoveChild(this);
        }

        base.Delete();
    }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() =>
        $"{GetType().Name} #{Handle:X8} ({Size.Width}x{Size.Height} @ {Location.X}x{Location.Y})";
}
