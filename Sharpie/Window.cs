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
public class Window: Surface, IWindow
{
    private readonly IList<SubWindow> _subWindows = new List<SubWindow>();

    /// <summary>
    ///     Initializes the window using the given Curses handle.
    /// </summary>
    /// <param name="curses">The Curses backend.</param>
    /// <param name="parent">The parent screen.</param>
    /// <param name="handle">The Curses handle.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="curses" /> or <paramref name="parent" /> are <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle" /> is invalid.</exception>
    internal Window(ICursesProvider curses, Screen parent, IntPtr handle): base(curses, handle)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        Curses.keypad(Handle, true)
              .Check(nameof(Curses.keypad), "Failed to enable the keypad resolution mode.");
        Curses.syncok(Handle, true)
              .Check(nameof(Curses.syncok), "Failed to enable auto-sync mode.");
        
        Screen = parent;
        parent.AddChild(this);
    }

    /// <inheritdoc cref="IWindow.Screen"/>
    public IScreen Screen { get; }

    /// <summary>
    /// Registers a sub-window as a child. This is an internal function.
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
    /// Un-registers a sub-window as a child. This is an internal function.
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

    /// <inheritdoc cref="IWindow.SubWindows"/>
    public IEnumerable<ISubWindow> SubWindows => _subWindows;

    /// <inheritdoc cref="IWindow.UseHardwareLineEdit"/>
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

    /// <inheritdoc cref="IWindow.UseHardwareCharEdit"/>
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

    /// <inheritdoc cref="IWindow.Location"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Point Location
    {
        get =>
            new(Curses.getbegx(Handle)
                      .Check(nameof(Curses.getbegx), "Failed to get window X coordinate."), Curses.getbegy(Handle)
                .Check(nameof(Curses.getbegy), "Failed to get window Y coordinate."));
        set
        {
            if (!Screen.IsRectangleWithin(new(value, Size)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.mvwin(Handle, value.Y, value.X)
                  .Check(nameof(Curses.mvwin), "Failed to move window to new coordinates.");
        }
    }

    /// <inheritdoc cref="IWindow.Size"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public new Size Size
    {
        get => base.Size;
        set
        {
            if (!Screen.IsRectangleWithin(new(Location, value)))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Curses.wresize(Handle, value.Height, value.Width)
                  .Check(nameof(Curses.wresize), "Failed to resize the window.");
        }
    }

    /// <inheritdoc cref="IWindow.ImmediateRefresh"/>
    public bool ImmediateRefresh
    {
        get => Curses.is_immedok(Handle);
        set => Curses.immedok(Handle, value);
    }

    /// <inheritdoc cref="IWindow.Refresh(bool, bool)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh(bool batch, bool entireScreen)
    {
        Curses.clearok(Handle, entireScreen)
              .Check(nameof(Curses.clearok), "Failed to configure the refresh mode.");

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

    /// <inheritdoc cref="IWindow.Refresh(int, int)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh(int y, int count)
    {
        if (y < 0 || y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (count < 1 || y + count - 1 >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Curses.wredrawln(Handle, y, count)
              .Check(nameof(Curses.wredrawln), "Failed to perform line refresh.");
    }

    /// <inheritdoc cref="IWindow.SubWindow"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ISubWindow SubWindow(Rectangle area)
    {
        if (!((IWindow) this).IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Curses.derwin(Handle, area.Height, area.Width, area.Y, area.X)
                           .Check(nameof(Curses.derwin), "Failed to create a new sub-window.");

        return new SubWindow(Curses, this, handle) { ManagedCaret = ManagedCaret };
    }

    /// <inheritdoc cref="IWindow.Duplicate"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public IWindow Duplicate()
    {
        var handle = Curses.dupwin(Handle)
                           .Check(nameof(Curses.dupwin), "Failed to duplicate the window.");

        return new Window(Curses, (Screen) Screen, handle) { ManagedCaret = ManagedCaret };
    }
    
    /// <inheritdoc cref="Surface.Delete"/>
    protected override void Delete()
    {
        foreach (var window in _subWindows.ToArray())
        {
            window.Destroy();
        }
        
        if (Screen is Screen s)
        {
            s.RemoveChild(this);
        }
        
        base.Delete();
    }
}
