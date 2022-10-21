namespace Sharpie;

using Curses;
using JetBrains.Annotations;

/// <summary>
/// Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window
{
    protected ICursesProvider CursesProvider { get; }
    private bool _enableProcessingKeypadKeys;
    private bool _useHardwareLineInsertAndDelete;
    private bool _useHardwareCharacterInsertAndDelete;
    private bool _immediateRefresh;
    private int _x, _y, _width, _height;

    /// <summary>
    /// The Curses handle for the window.
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="x">The X coordinate of the window.</param>
    /// <param name="y">The Y coordinate of the window.</param>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    internal Window(ICursesProvider cursesProvider, IntPtr windowHandle, int x, int y, int width, int height)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;

        CursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));
        Handle = windowHandle;

        _useHardwareCharacterInsertAndDelete = CursesProvider.has_ic();
    }

    /// <summary>
    /// Enables or disables the processing of keypad keys.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool EnableProcessingKeypadKeys
    {
        get
        {
            CursesProvider.AssertNotDisposed();

            return _enableProcessingKeypadKeys;
        }
        set
        {
            CursesProvider.AssertNotDisposed();

            CursesProvider.keypad(Handle, value)
                           .TreatError();

            _enableProcessingKeypadKeys = value;
        }
    }

    /// <summary>
    /// Enables or disables the use of hardware line insert/delete handling fpr this window.
    /// </summary>
    /// <remarks>
    /// This functionality only works if hardware has support for it. Consult <see cref="Terminal.SupportsHardwareLineInsertAndDelete" />
    /// Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public bool UseHardwareLineInsertAndDelete
    {
        get
        {
            CursesProvider.AssertNotDisposed();

            return _useHardwareLineInsertAndDelete;
        }
        set
        {
            CursesProvider.AssertNotDisposed();

            if (CursesProvider.has_il())
            {
                CursesProvider.idlok(Handle, value).TreatError();
                _useHardwareLineInsertAndDelete = value;
            }
        }
    }

    /// <summary>
    /// Enables or disables the use of hardware character insert/delete handling for this window.
    /// </summary>
    ///    <remarks>
    /// This functionality only works if hardware has support for it. Consult <see cref="Terminal.SupportsHardwareCharacterInsertAndDelete" />
    /// Default is <c>true</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool UseHardwareCharacterInsertAndDelete
    {
        get
        {
            CursesProvider.AssertNotDisposed();

            return _useHardwareCharacterInsertAndDelete;
        }
        set
        {
            CursesProvider.AssertNotDisposed();

            if (CursesProvider.has_ic())
            {
                CursesProvider.idcok(Handle, value);
                _useHardwareCharacterInsertAndDelete = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public Style Style
    {
        get
        {
            CursesProvider.AssertNotDisposed();

            CursesProvider.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero).TreatError();
            return new() { Attributes = (VideoAttribute) attrs, ColorPair = new() { Handle = colorPair } };
        }
        set
        {
            CursesProvider.AssertNotDisposed();

            CursesProvider.wattr_set(Handle, (uint) value.Attributes, value.ColorPair.Handle, IntPtr.Zero).TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the color pair of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorPair ColorPair
    {
        get => Style.ColorPair;
        set
        {
            CursesProvider.AssertNotDisposed();

            CursesProvider.wcolor_set(Handle, value.Handle, IntPtr.Zero).TreatError();
        }
    }

    /// <summary>
    /// Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        CursesProvider.AssertNotDisposed();

        CursesProvider.wattr_on(Handle, (uint) attributes, IntPtr.Zero).TreatError();
    }

    /// <summary>
    /// Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        CursesProvider.AssertNotDisposed();

        CursesProvider.wattr_off(Handle, (uint) attributes, IntPtr.Zero).TreatError();
    }

    /// <summary>
    /// Tries to move the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="x"/> or <paramref name="y"/> are negative.</exception>
    public bool TryMoveCaretTo(int x, int y)
    {
        if (x < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }
        if (y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        CursesProvider.AssertNotDisposed();
        return CursesProvider.wmove(Handle, y, x) != Helpers.CursesErrorResult;
    }

    /// <summary>
    /// Moves the caret to a given position within the window.
    /// </summary>
    /// <param name="x">The new X.</param>
    /// <param name="y">The new Y.</param>
    /// <returns><c>true</c> if the caret was moved. <c>false</c> if the coordinates are out of the window.</returns>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentException">The given coordinates are outside the window.</exception>
    public void MoveCaretTo(int x, int y)
    {
        if (!TryMoveCaretTo(x, y))
        {
            throw new ArgumentException("The coordinates are outside the window.");
        }
    }

    /// <summary>
    /// Changes the style of the text on the current line and starting from the caret position.
    /// </summary>
    /// <param name="length">The number of characters to change.</param>
    /// <param name="style">The applied style.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="ArgumentException">The <paramref name="length"/> is less than one.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void ChangeTextStyle(int length, Style style)
    {
        if (length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "The length should be greater than zero.");
        }

        CursesProvider.AssertNotDisposed();

        CursesProvider.wchgat(Handle, length, (uint) style.Attributes, style.ColorPair.Handle, IntPtr.Zero)
                       .TreatError();
    }

    /// <summary>
    /// Gets or sets the location of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (int x, int y) Location
    {
        get
        {
            CursesProvider.AssertNotDisposed();

            return (_x, _y);
        }
        set
        {
            CursesProvider.AssertNotDisposed();
            CursesProvider.mvwin(Handle, value.y, value.x)
                          .TreatError();

            _x = value.x;
            _y = value.y;
        }
    }

    /// <summary>
    /// Set or get the immediate refresh capability of the window.
    /// </summary>
    /// <remarks>
    /// Immediate refresh will redraw the window on each change.
    /// This might be very slow for most use cases.
    /// Default is <c>false</c>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool ImmediateRefresh
    {
        get
        {
            CursesProvider.AssertNotDisposed();
            return _immediateRefresh;
        }
        set
        {
            CursesProvider.AssertNotDisposed();
            CursesProvider.immedok(Handle, value);

            _immediateRefresh = value;
        }
    }
}
