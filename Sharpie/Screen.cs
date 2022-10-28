namespace Sharpie;

using System.Drawing;
using Curses;
using JetBrains.Annotations;

/// <summary>
///     The core curses screen class. Implements screen-related functionality.
/// </summary>
[PublicAPI]
public sealed class Screen: Window
{
    /// <summary>
    /// Initializes the screen using a window handle. The <paramref name="windowHandle"/> should be
    /// a screen and not a regular window.
    /// </summary>
    /// <param name="terminal">The terminal instance.</param>
    /// <param name="windowHandle">The screen handle.</param>
    internal Screen(Terminal terminal, IntPtr windowHandle): base(terminal, null, windowHandle) { }

    ///<inheritdoc cref="Window.Location"/>
    /// <remarks>
    /// The setter will always throw in this implementation as moving the main window is not allowed.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">Always throws on the setter.</exception>
    public override Point Location
    {
        get => base.Location;
        set => throw new NotSupportedException("Cannot move the screen window.");
    }

    ///<inheritdoc cref="Window.Location"/>
    /// <remarks>
    /// The setter will always throw in this implementation changing the size of the main window is not allowed.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">Always throws on the setter.</exception>
    public override Size Size
    {
        get => base.Size;
        set => throw new NotSupportedException("Cannot resize the screen window.");
    }

    /// <summary>
    /// Created a new window in the screen.
    /// </summary>
    /// <param name="area">The area for the new window.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area"/> is outside the screen bounds.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateWindow(Rectangle area)
    {
        if (!IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.newwin(area.Height, area.Width, area.Y, area.X)
                             .Check(nameof(Terminal.Curses.newwin), "Failed to create a new window.");

        return new(Terminal, this, handle);
    }

    /// <summary>
    /// Created a new sub-window in the parent window.
    /// </summary>
    /// <param name="window">The parent window.</param>
    /// <param name="area">The area of the window to put the sub-window in.</param>
    /// <remarks>
    /// </remarks>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="area"/> is outside the bounds of the parent.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateSubWindow(Window window, Rectangle area)
    {
        if (!window.IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.derwin(window.Handle, area.Height, area.Width, area.Y, area.X)
                             .Check(nameof(Terminal.Curses.derwin), "Failed to create a new sub-window.");

        return new(Terminal, this, handle);
    }

    /// <summary>
    /// Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="InvalidOperationException">Trying to duplicate the screen window.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window DuplicateWindow(Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }
        if (window == this)
        {
            throw new InvalidOperationException("Cannot duplicate the screen window.");
        }

        return new(Terminal, this, Terminal.Curses.dupwin(window.Handle)
                                           .Check(nameof(Terminal.Curses.dupwin), "Failed to duplicate an existing window."));
    }

    /// <summary>
    /// Created a new pad.
    /// </summary>
    /// <param name="size">The pad size.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="size"/> is invalid.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Pad CreatePad(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.newpad(size.Height, size.Width)
                             .Check(nameof(Terminal.Curses.newpad), "Failed to create a new pad.");

        return new(Terminal, this, handle);
    }

    /// <summary>
    /// Created a new sub-pad.
    /// </summary>
    /// <param name="pad">The parent pad.</param>
    /// <param name="area">The are of the pad to use.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal or the pad have been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="area"/> is outside the pad's bounds.</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="pad"/> is <c>null</c>.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Pad CreateSubPad(Pad pad, Rectangle area)
    {
        if (pad == null)
        {
            throw new ArgumentNullException(nameof(pad));
        }

        if (!pad.IsRectangleWithin(area))
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        var handle = Terminal.Curses.subpad(pad.Handle, area.Height, area.Width, area.Top, area.Right)
                             .Check(nameof(Terminal.Curses.subpad), "Failed to create a new sub-pad.");

        return new(Terminal, pad, handle);
    }

    /// <summary>
    /// Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        Terminal.AssertNotDisposed();
        Terminal.Curses.doupdate()
                .Check(nameof(Terminal.Curses.doupdate));
    }

    private static (Key key, ModifierKey modifierKey) ConvertKey(uint keyCode)
    {
        return (RawKey) keyCode switch
        {
            RawKey.F1 => (Key.F1, ModifierKey.None),
            RawKey.F2 => (Key.F2, ModifierKey.None),
            RawKey.F3 => (Key.F3, ModifierKey.None),
            RawKey.F4 => (Key.F4, ModifierKey.None),
            RawKey.F5 => (Key.F5, ModifierKey.None),
            RawKey.F6 => (Key.F6, ModifierKey.None),
            RawKey.F7 => (Key.F7, ModifierKey.None),
            RawKey.F8 => (Key.F8, ModifierKey.None),
            RawKey.F9 => (Key.F9, ModifierKey.None),
            RawKey.F10 => (Key.F10, ModifierKey.None),
            RawKey.F11 => (Key.F11, ModifierKey.None),
            RawKey.F12 => (Key.F12, ModifierKey.None),
            RawKey.ShiftF1 => (Key.F1, ModifierKey.Shift),
            RawKey.ShiftF2 => (Key.F2, ModifierKey.Shift),
            RawKey.ShiftF3 => (Key.F3, ModifierKey.Shift),
            RawKey.ShiftF4 => (Key.F4, ModifierKey.Shift),
            RawKey.ShiftF5 => (Key.F5, ModifierKey.Shift),
            RawKey.ShiftF6 => (Key.F6, ModifierKey.Shift),
            RawKey.ShiftF7 => (Key.F7, ModifierKey.Shift),
            RawKey.ShiftF8 => (Key.F8, ModifierKey.Shift),
            RawKey.ShiftF9 => (Key.F9, ModifierKey.Shift),
            RawKey.ShiftF10 => (Key.F10, ModifierKey.Shift),
            RawKey.ShiftF11 => (Key.F11, ModifierKey.Shift),
            RawKey.ShiftF12 => (Key.F12, ModifierKey.Shift),
            RawKey.CtrlF1 => (Key.F1, ModifierKey.Ctrl),
            RawKey.CtrlF2 => (Key.F2, ModifierKey.Ctrl),
            RawKey.CtrlF3 => (Key.F3, ModifierKey.Ctrl),
            RawKey.CtrlF4 => (Key.F4, ModifierKey.Ctrl),
            RawKey.CtrlF5 => (Key.F5, ModifierKey.Ctrl),
            RawKey.CtrlF6 => (Key.F6, ModifierKey.Ctrl),
            RawKey.CtrlF7 => (Key.F7, ModifierKey.Ctrl),
            RawKey.CtrlF8 => (Key.F8, ModifierKey.Ctrl),
            RawKey.CtrlF9 => (Key.F9, ModifierKey.Ctrl),
            RawKey.CtrlF10 => (Key.F10, ModifierKey.Ctrl),
            RawKey.CtrlF11 => (Key.F11, ModifierKey.Ctrl),
            RawKey.CtrlF12 => (Key.F12, ModifierKey.Ctrl),
            RawKey.AltF1 => (Key.F1, ModifierKey.Alt),
            RawKey.AltF2 => (Key.F2, ModifierKey.Alt),
            RawKey.AltF3 => (Key.F3, ModifierKey.Alt),
            RawKey.AltF4 => (Key.F4, ModifierKey.Alt),
            RawKey.AltF5 => (Key.F5, ModifierKey.Alt),
            RawKey.AltF6 => (Key.F6, ModifierKey.Alt),
            RawKey.AltF7 => (Key.F7, ModifierKey.Alt),
            RawKey.AltF8 => (Key.F8, ModifierKey.Alt),
            RawKey.AltF9 => (Key.F9, ModifierKey.Alt),
            RawKey.AltF10 => (Key.F10, ModifierKey.Alt),
            RawKey.AltF11 => (Key.F11, ModifierKey.Alt),
            RawKey.AltF12 => (Key.F12, ModifierKey.Alt),
            RawKey.ShiftAltF1 => (Key.F1, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF2 => (Key.F2, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF3 => (Key.F3, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF4 => (Key.F4, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF5 => (Key.F5, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF6 => (Key.F6, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF7 => (Key.F7, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF8 => (Key.F8, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF9 => (Key.F9, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF10 => (Key.F10, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF11 => (Key.F11, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.ShiftAltF12 => (Key.F12, ModifierKey.Alt | ModifierKey.Shift),
            RawKey.Up => (Key.KeypadUp, ModifierKey.None),
            RawKey.Down => (Key.KeypadDown, ModifierKey.None),
            RawKey.Left => (Key.KeypadLeft, ModifierKey.None),
            RawKey.Right => (Key.KeypadRight, ModifierKey.None),
            RawKey.Home => (Key.KeypadHome, ModifierKey.None),
            RawKey.End => (Key.KeypadEnd, ModifierKey.None),
            RawKey.PageDown => (Key.KeypadPageDown, ModifierKey.None),
            RawKey.PageUp => (Key.KeypadPageUp, ModifierKey.None),
            RawKey.DeleteChar => (Key.DeleteChar, ModifierKey.None),
            RawKey.InsertChar => (Key.InsertChar, ModifierKey.None),
            RawKey.Tab => (Key.Tab, ModifierKey.None),
            RawKey.BackTab => (Key.Tab, ModifierKey.Shift),
            RawKey.Backspace => (Key.Backspace, ModifierKey.None),
            RawKey.ShiftUp => (Key.KeypadUp, ModifierKey.Shift),
            RawKey.ShiftDown => (Key.KeypadDown, ModifierKey.Shift),
            RawKey.ShiftLeft => (Key.KeypadLeft, ModifierKey.Shift),
            RawKey.ShiftRight => (Key.KeypadRight, ModifierKey.Shift),
            RawKey.ShiftHome => (Key.KeypadHome, ModifierKey.Shift),
            RawKey.ShiftEnd => (Key.KeypadEnd, ModifierKey.Shift),
            RawKey.ShiftPageDown => (Key.KeypadPageDown, ModifierKey.Shift),
            RawKey.ShiftPageUp => (Key.KeypadPageUp, ModifierKey.Shift),
            RawKey.AltUp => (Key.KeypadUp, ModifierKey.Alt),
            RawKey.AltDown => (Key.KeypadDown, ModifierKey.Alt),
            RawKey.AltLeft => (Key.KeypadLeft, ModifierKey.Alt),
            RawKey.AltRight => (Key.KeypadRight, ModifierKey.Alt),
            RawKey.AltHome => (Key.KeypadHome, ModifierKey.Alt),
            RawKey.AltEnd => (Key.KeypadEnd, ModifierKey.Alt),
            RawKey.AltPageDown => (Key.KeypadPageDown, ModifierKey.Alt),
            RawKey.AltPageUp => (Key.KeypadPageUp, ModifierKey.Alt),
            RawKey.CtrlUp => (Key.KeypadUp, ModifierKey.Ctrl),
            RawKey.CtrlDown => (Key.KeypadDown, ModifierKey.Ctrl),
            RawKey.CtrlLeft => (Key.KeypadLeft, ModifierKey.Ctrl),
            RawKey.CtrlRight => (Key.KeypadRight, ModifierKey.Ctrl),
            RawKey.CtrlHome => (Key.KeypadHome, ModifierKey.Ctrl),
            RawKey.CtrlEnd => (Key.KeypadEnd, ModifierKey.Ctrl),
            RawKey.CtrlPageDown => (Key.KeypadPageDown, ModifierKey.Ctrl),
            RawKey.CtrlPageUp => (Key.KeypadPageUp, ModifierKey.Ctrl),
            RawKey.ShiftCtrlUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Ctrl),
            RawKey.ShiftAltUp => (Key.KeypadUp, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltDown => (Key.KeypadDown, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltLeft => (Key.KeypadLeft, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltRight => (Key.KeypadRight, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltPageDown => (Key.KeypadPageDown, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltPageUp => (Key.KeypadPageUp, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltHome => (Key.KeypadHome, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.ShiftAltEnd => (Key.KeypadEnd, ModifierKey.Shift | ModifierKey.Alt),
            RawKey.AltCtrlPageDown => (Key.KeypadPageDown, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlPageUp => (Key.KeypadPageUp, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlHome => (Key.KeypadHome, ModifierKey.Alt | ModifierKey.Ctrl),
            RawKey.AltCtrlEnd => (Key.KeypadEnd, ModifierKey.Alt | ModifierKey.Ctrl),
            var _ => (Key.Unknown, ModifierKey.None)
        };
    }

    private static (MouseButton button, MouseButtonState state, ModifierKey modifierKey) ConvertMouseEvent(
        RawMouseEvent.EventType type)
    {
        var modifierKey = ModifierKey.None;
        var button = MouseButton.Unknown;
        var state = MouseButtonState.Unknown;

        if (type.HasFlag(RawMouseEvent.EventType.Alt))
        {
            modifierKey |= ModifierKey.Alt;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Ctrl))
        {
            modifierKey |= ModifierKey.Ctrl;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Shift))
        {
            modifierKey |= ModifierKey.Shift;
        }

        if (type.HasFlag(RawMouseEvent.EventType.Button1Released))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1Pressed))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1Clicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1DoubleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button1TripleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Released))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Pressed))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2Clicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2DoubleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button2TripleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Released))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Pressed))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3Clicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3DoubleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button3TripleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Released))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Pressed))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4Clicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4DoubleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button4TripleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.TripleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Released))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Released;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Pressed))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Pressed;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5Clicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Clicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5DoubleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.DoubleClicked;
        } else if (type.HasFlag(RawMouseEvent.EventType.Button5TripleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.TripleClicked;
        }

        return (button, state, modifierKey);
    }

    public bool TryReadEvent(int timeoutMillis, out Event? @event)
    {
        /*

        Curses.define_key("\x001bO2C", 1024); //shift right
        Curses.define_key("\x001bO2B", 1024); //shift down
        Curses.define_key("\x001bO2A", 1024); //shift up
        Curses.define_key("\x001bO2D", 1024); //shift left
        Curses.define_key("\x001bO2H", 1024); //shift home
        Curses.define_key("\x001bO2F", 1024); //shift end of line

        Curses.define_key("\x001b06C", 1024); //shift ctrl right
        Curses.define_key("\x001b06B", 1024); //shift ctrl down
        Curses.define_key("\x001b06A", 1024); //shift ctrl up
        Curses.define_key("\x001b06D", 1024); //shift ctrl left
        Curses.define_key("\x001b06H", 1024); //shift ctrl home
        Curses.define_key("\x001b06F", 1024); //shift ctrl end of line

        Curses.define_key("\x001bb", 1024); //alt right
        Curses.define_key("\x001bf", 1024); //alt left

        Curses.define_key("\x001b" + (char)258, 1024); //alt up

         */
        Terminal.Curses.wtimeout(Handle, timeoutMillis);

        @event = null;
        var result = Terminal.Curses.wget_wch(Handle, out var keyCode);
        if (result == (uint) RawKey.Yes)
        {
            switch (keyCode)
            {
                case (uint) RawKey.Resize:
                    @event = new TerminalResizeEvent(Size);
                    break;
                case (uint) RawKey.Mouse:
                    if (Terminal.Curses.getmouse(out var mouseEvent).Failed())
                    {
                        return false;
                    }

                    var (button, state, mouseMod) = ConvertMouseEvent((RawMouseEvent.EventType)mouseEvent.buttonState);
                    if (button == 0)
                    {
                        @event = new MouseMoveEvent(new(mouseEvent.x, mouseEvent.y));
                    } else
                    {
                        @event = new MouseActionEvent(new(mouseEvent.x, mouseEvent.y), button, state, mouseMod);
                    }

                    break;
                default:
                    var (key, keyMod) = ConvertKey(keyCode);
                    @event = new KeyEvent(key, new('\0'), Terminal.Curses.key_name(keyCode), keyMod);
                    break;
            }

            return true;
        }

        if (result.Failed())
        {
            if (keyCode == 27 && TryReadEvent(10, out var other) && other?.Type == EventType.KeyPress)
            {
                // Special escape sequence handling.
            }

            var keyName = Terminal.Curses.key_name(keyCode);
            @event = new KeyEvent(Key.Character, new(keyCode), keyName, ModifierKey.None);
            return true;
        }

        return false;
    }

}
