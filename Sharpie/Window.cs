namespace Sharpie;

using Curses;
using JetBrains.Annotations;

/// <summary>
/// Represents a window and contains all it's functionality.
/// </summary>
[PublicAPI]
public class Window
{
    private readonly ICursesProvider _cursesProvider;
    private bool _enableProcessingKeypadKeys;
    private bool _useHardwareLineInsertAndDelete;
    private bool _useHardwareCharacterInsertAndDelete;
    private bool _immediateRefresh;

    /// <summary>
    /// The Curses handle for the window.
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Initializes the window using a Curses handle.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <param name="windowHandle">The window handle.</param>
    internal Window(ICursesProvider cursesProvider, IntPtr windowHandle)
    {
        _cursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));
        Handle = windowHandle;

        _useHardwareCharacterInsertAndDelete = _cursesProvider.has_ic();
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
            _cursesProvider.AssertNotDisposed();

            return _enableProcessingKeypadKeys;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.keypad(Handle, value)
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
            _cursesProvider.AssertNotDisposed();

            return _useHardwareLineInsertAndDelete;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            if (_cursesProvider.has_il())
            {
                _cursesProvider.idlok(Handle, value).TreatError();
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
            _cursesProvider.AssertNotDisposed();

            return _useHardwareCharacterInsertAndDelete;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            if (_cursesProvider.has_ic())
            {
                _cursesProvider.idcok(Handle, value);
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
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.wattr_get(Handle, out var attrs, out var colorPair, IntPtr.Zero).TreatError();
            return new() { Attributes = (VideoAttribute) attrs, ColorPair = new() { Handle = colorPair } };
        }
        set
        {
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.wattr_set(Handle, (uint) value.Attributes, value.ColorPair.Handle, IntPtr.Zero).TreatError();
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
            _cursesProvider.AssertNotDisposed();

            _cursesProvider.wcolor_set(Handle, value.Handle, IntPtr.Zero).TreatError();
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
        _cursesProvider.AssertNotDisposed();

        _cursesProvider.wattr_off(Handle, (uint) attributes, IntPtr.Zero).TreatError();
    }

    /// <summary>
    /// Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        _cursesProvider.AssertNotDisposed();

        _cursesProvider.wattr_off(Handle, (uint) attributes, IntPtr.Zero).TreatError();
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

        _cursesProvider.AssertNotDisposed();
        return _cursesProvider.wmove(Handle, y, x) != Helpers.CursesErrorResult;
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

        _cursesProvider.AssertNotDisposed();

        _cursesProvider.wchgat(Handle, length, (uint) style.Attributes, style.ColorPair.Handle, IntPtr.Zero)
                       .TreatError();
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
            _cursesProvider.AssertNotDisposed();
            return _immediateRefresh;
        }
        set
        {
            _cursesProvider.AssertNotDisposed();
            _cursesProvider.immedok(Handle, value);

            _immediateRefresh = value;
        }
    }

    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void Refresh(bool fullScreen) { _cursesProvider.clearok(Handle, fullScreen).TreatError(); }
}
