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
public sealed class Window: TerminalSurface, IWindow
{
    private readonly IList<SubWindow> _subWindows = new List<SubWindow>();
    private Rectangle _explicitArea;
    
    /// <summary>
    ///     Initializes the window using the given Curses handle.
    /// </summary>
    /// <param name="parent">The parent screen.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal Window(Screen parent, IntPtr handle): base(parent != null! ? parent.Terminal : null!, handle)
    {
        Curses.keypad(Handle, true)
              .Check(nameof(Curses.keypad), "Failed to enable the keypad resolution mode.");

        Curses.syncok(Handle, true)
              .Check(nameof(Curses.syncok), "Failed to enable auto-sync mode.");

        Screen = parent!;
        parent!.AddChild(this);

        _explicitArea = new(Location, Size);
    }

    /// <inheritdoc cref="IWindow.Screen" />
    public Screen Screen { get; }

    /// <inheritdoc cref="IWindow.Screen" />
    IScreen IWindow.Screen => Screen;

    /// <inheritdoc cref="IWindow.SubWindows" />
    public IEnumerable<ISubWindow> SubWindows => _subWindows;

    /// <inheritdoc cref="IWindow.UseHardwareLineEdit" />
    public bool UseHardwareLineEdit
    {
        get => Curses.is_idlok(Handle);
        set
        {
            if (Curses.has_il())
            {
                Curses.idlok(Handle, value)
                      .Check(nameof(Curses.idlok), "Failed to change the hardware line mode.");
            }
        }
    }

    /// <inheritdoc cref="IWindow.UseHardwareCharEdit" />
    public bool UseHardwareCharEdit
    {
        get => Curses.is_idcok(Handle);
        set
        {
            if (Curses.has_ic())
            {
                Curses.idcok(Handle, value);
            }
        }
    }

    /// <inheritdoc cref="IWindow.Location" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Point Location
    {
        get =>
            new(Curses.getbegx(Handle)
                      .Check(nameof(Curses.getbegx), "Failed to get window X coordinate."), Curses.getbegy(Handle)
                .Check(nameof(Curses.getbegy), "Failed to get window Y coordinate."));
        set
        {
            var size = Size;
            if (!((IScreen) Screen).IsRectangleWithin(new(value, size)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.mvwin(Handle, value.Y, value.X)
                  .Check(nameof(Curses.mvwin), "Failed to move window to new coordinates.");
            
            _explicitArea = new(value, size);
        }
    }

    /// <inheritdoc cref="IWindow.Size" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            var location = Location;
            if (!((IScreen) Screen).IsRectangleWithin(new(location, value)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wresize(Handle, value.Height, value.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the window.");
            
            _explicitArea = new(location, value);
        }
    }

    private void AdjustToExplicitArea()
    {
        var screenSize = Screen.Size;
        var size = Size;
        var location = Location;

        var w = Math.Min(screenSize.Width - _explicitArea.X, _explicitArea.Width);
        var h = Math.Min(screenSize.Height - _explicitArea.Y, _explicitArea.Height);

        if ((w != size.Width || h != size.Height) && w > 0 && h > 0)
        {
            Curses.wresize(Handle, h, w);
        }

        if (location.X > _explicitArea.X || location.Y > _explicitArea.Y)
        {
            Curses.mvwin(Handle, _explicitArea.Y, _explicitArea.X);
        }
    }

    /// <inheritdoc cref="TerminalSurface.Refresh()" />
    public override void Refresh()
    {
        AdjustToExplicitArea();
        base.Refresh();
    }
    
    /// <inheritdoc cref="TerminalSurface.Refresh(int, int)" />
    public override void Refresh(int y, int count)
    {
        AdjustToExplicitArea();
        base.Refresh(y, count);
    }

    /// <inheritdoc cref="TerminalSurface.MarkDirty(int, int)" />
    public override void MarkDirty(int y, int count)
    {
        AdjustToExplicitArea();
        base.MarkDirty(y, count);
    }
    
    /// <inheritdoc cref="IWindow.SubWindow" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ISubWindow SubWindow(Rectangle area)
    {
        if (!((IWindow) this).IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.derwin(Handle, area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Curses.derwin), "Failed to create a new sub-window.");

        return new SubWindow(this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="IWindow.Duplicate" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow Duplicate()
    {
        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the window.");

        return new Window(Screen, handle) { ManagedCaret = ManagedCaret };
    }

    /// <summary>
    ///     Registers a sub-window as a child. This is an internal function.
    /// </summary>
    /// <param name="subWindow">The sub-window to register.</param>
    internal void AddChild(SubWindow subWindow)
    {
        Debug.Assert(subWindow != null);
        Debug.Assert(!subWindow.Disposed);
        Debug.Assert(subWindow.Window == this);
        Debug.Assert(!_subWindows.Contains(subWindow));

        _subWindows.Add(subWindow);
    }

    /// <summary>
    ///     Un-registers a sub-window as a child. This is an internal function.
    /// </summary>
    /// <param name="subWindow">The sub-window to un-register.</param>
    internal void RemoveChild(SubWindow subWindow)
    {
        Debug.Assert(subWindow != null);
        Debug.Assert(!subWindow.Disposed);
        Debug.Assert(subWindow.Window == this);
        Debug.Assert(_subWindows.Contains(subWindow));

        _subWindows.Remove(subWindow);
    }

    /// <inheritdoc cref="Surface.Delete" />
    protected override void Delete()
    {
        foreach (var window in _subWindows.ToArray())
        {
            window.Destroy();
        }

        if (Screen != null!)
        {
            Screen.RemoveChild(this);
        }

        base.Delete();
    }
}
