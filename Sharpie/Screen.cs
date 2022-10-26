namespace Sharpie;

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
    internal Screen(Terminal terminal, IntPtr windowHandle): base(terminal, windowHandle) { }

    /// <summary>
    /// Created a new window in the screen.
    /// </summary>
    /// <param name="x">The X coordinate of the window location.</param>
    /// <param name="y">The Y coordinate of the window location.</param>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="x"/> or <paramref name="y"/> are negative;
    /// or <paramref name="width"/>, <paramref name="height"/> are less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window CreateWindow(int x, int y, int width, int height)
    {
        if (x < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.newwin(height, width, y, x)
                             .TreatNullAsError();

        return new(Terminal, handle);
    }

    /// <summary>
    /// Duplicates and existing window, including its attributes.
    /// </summary>
    /// <param name="window">The window to duplicate.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Window DuplicateWindow(Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        window.AssertNotDisposed();

        return new(Terminal, Terminal.Curses.dupwin(window.Handle)
                                     .TreatNullAsError());
    }

    /// <summary>
    /// Created a new pad.
    /// </summary>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    /// <returns>A new window object.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="width"/>, <paramref name="height"/> are less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Pad CreatePad(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Terminal.AssertNotDisposed();
        var handle = Terminal.Curses.newpad(height, width)
                             .TreatNullAsError();

        return new(Terminal, handle);
    }

    /// <summary>
    /// Applies all queued refreshes to the terminal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal of the given window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ApplyPendingRefreshes()
    {
        AssertNotDisposed();
        Terminal.Curses.doupdate()
                .TreatError();
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

    private static (MouseButton button, MouseButtonState state, ModifierKey modifierKey) ConvertMouseEvent(RawMouseEvent.EventType type)
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

        // Button 1
        if (type.HasFlag(RawMouseEvent.EventType.Button1Released))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Released;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button1Pressed))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Pressed;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button1Clicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.Clicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button1DoubleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.DoubleClicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button1TripleClicked))
        {
            button = MouseButton.Button1;
            state = MouseButtonState.TripleClicked;
        }

        // Button 2
        if (type.HasFlag(RawMouseEvent.EventType.Button2Released))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Released;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button2Pressed))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Pressed;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button2Clicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.Clicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button2DoubleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.DoubleClicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button2TripleClicked))
        {
            button = MouseButton.Button2;
            state = MouseButtonState.TripleClicked;
        }

        // Button 3
        if (type.HasFlag(RawMouseEvent.EventType.Button3Released))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Released;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button3Pressed))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Pressed;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button3Clicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.Clicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button3DoubleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.DoubleClicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button3TripleClicked))
        {
            button = MouseButton.Button3;
            state = MouseButtonState.TripleClicked;
        }

        // Button 4
        if (type.HasFlag(RawMouseEvent.EventType.Button4Released))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Released;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button4Pressed))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Pressed;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button4Clicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.Clicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button4DoubleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.DoubleClicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button4TripleClicked))
        {
            button = MouseButton.Button4;
            state = MouseButtonState.TripleClicked;
        }

        // Button 5
        if (type.HasFlag(RawMouseEvent.EventType.Button5Released))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Released;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button5Pressed))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Pressed;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button5Clicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.Clicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button5DoubleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.DoubleClicked;
        }
        else if (type.HasFlag(RawMouseEvent.EventType.Button5TripleClicked))
        {
            button = MouseButton.Button5;
            state = MouseButtonState.TripleClicked;
        }

        return (button, state, modifierKey);
    }

    public bool TryReadEvent(ReadBehavior behavior, out Event? @event)
    {
        AssertNotDisposed();

        var enableKeypad = !behavior.HasFlag(ReadBehavior.RawKeypadSequences);
        var enableNoDelay = behavior.HasFlag(ReadBehavior.RawEscapeSequences);
        var enableNoTimeout = behavior.HasFlag(ReadBehavior.NoWait);

        if (Terminal.Curses.is_keypad(Handle) != enableKeypad)
        {
            Terminal.Curses.keypad(Handle, enableKeypad).TreatError();
        }
        if (Terminal.Curses.is_nodelay(Handle) != enableNoDelay)
        {
            Terminal.Curses.nodelay(Handle, enableNoDelay).TreatError();
        }
        if (Terminal.Curses.is_notimeout(Handle) != enableNoTimeout)
        {
            Terminal.Curses.notimeout(Handle, enableNoTimeout).TreatError();
        }

        @event = null;
        var result = Terminal.Curses.wget_wch(Handle, out var keyCode);
        if (result == (uint) RawKey.Yes)
        {
            switch (keyCode)
            {
                case (uint) RawKey.Resize:
                    @event = new() { Type = EventType.ResizeTerminal };
                    break;
                case (uint) RawKey.Mouse:
                    Terminal.Curses.getmouse(out var mouseEvent)
                            .TreatError();

                    var (button, state, mouseMod) = ConvertMouseEvent((RawMouseEvent.EventType)mouseEvent.buttonState);
                    @event = new()
                    {
                        Type = button == 0 ? EventType.MouseMove : EventType.MouseClick,
                        MousePosition = new(mouseEvent.x, mouseEvent.y),
                        MouseButton = button,
                        MouseButtonState = state,
                        Modifier = mouseMod,
                    };

                    break;
                default:
                    var (key, keyMod) = ConvertKey(keyCode);
                    @event = new()
                    {
                        Type = EventType.KeyPress,
                        Key = key,
                        Modifier = keyMod,
                    };

                    break;
            }

            return true;
        }

        if (result != Helpers.CursesErrorResult)
        {
            @event = new()
            {
                Type = EventType.KeyPress,
                Key = Key.Character,
                Char = new(keyCode),
                Modifier =  ModifierKey.None,
            };
        }

        return false;
    }

}
