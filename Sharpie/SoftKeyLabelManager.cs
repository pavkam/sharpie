namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Adds support for soft function keys.
/// </summary>
[PublicAPI]
public sealed class SoftKeyLabelManager
{
    private readonly Terminal _terminal;
    private readonly SoftKeyLabelMode _mode;

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="terminal">The terminal instance.</param>
    /// <param name="mode">The mode of the manager.</param>
    public SoftKeyLabelManager(Terminal terminal, SoftKeyLabelMode mode)
    {
        _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        _mode = mode;

        if (mode != SoftKeyLabelMode.Disabled)
        {
            terminal.Curses.slk_init((int) mode);
        }
    }

    /// <summary>
    /// Specifies if the manager is enabled.
    /// </summary>
    public bool IsEnabled => _mode != SoftKeyLabelMode.Disabled;

    private void AssertNotDisposedAndEnabled()
    {
        _terminal.AssertNotDisposed();
        if (_mode == SoftKeyLabelMode.Disabled)
        {
            throw new NotSupportedException("The soft key labels were not configured during terminal initialization.");
        }
    }

    /// <summary>
    /// Gets the number of labels within the soft key label panel.
    /// </summary>
    public int LabelCount => _mode is SoftKeyLabelMode.FourFourFour or SoftKeyLabelMode.FourFourFourWithIndex ? 12 : 8;

    /// <summary>
    /// Sets a given label within the soft key label panel.
    /// </summary>
    /// <param name="index">The index of the label.</param>
    /// <param name="title">The title of the label.</param>
    /// <param name="align">Alignment of the label title.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="title"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> negative or greater than <see cref="LabelCount"/>.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void SetLabel(int index, string title, SoftKeyLabelAlignment align)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        if (index < 0 || index >= LabelCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        AssertNotDisposedAndEnabled();

        _terminal.Curses.slk_wset(index, title, (int) align)
                 .TreatError();
    }

    /// <summary>
    /// Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public Style Style
    {
        get
        {
            AssertNotDisposedAndEnabled();

            var attrsAndColors = _terminal.Curses.slk_attr()
                                          .TreatError();

            var colorPair = (ushort) _terminal.Curses.COLOR_PAIR((uint)attrsAndColors);
            return new() { Attributes = (VideoAttribute) attrsAndColors, ColorMixture = new() { Handle = colorPair } };
        }
        set
        {
            AssertNotDisposedAndEnabled();

            _terminal.Curses.slk_attr_set((uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                     .TreatError();
        }
    }

    /// <summary>
    /// Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set
        {
            AssertNotDisposedAndEnabled();

            _terminal.Curses.slk_color(value.Handle)
                    .TreatError();
        }
    }

    /// <summary>
    /// Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        AssertNotDisposedAndEnabled();

        _terminal.Curses.slk_attr_on((uint) attributes, IntPtr.Zero)
                 .TreatError();
    }

    /// <summary>
    /// Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        AssertNotDisposedAndEnabled();

        _terminal.Curses.slk_attr_off((uint) attributes, IntPtr.Zero)
                .TreatError();
    }

    /// <summary>
    /// Clears the soft key labels from the screen. They can be restored by calling <see cref="Restore"/> method.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void Clear()
    {
        AssertNotDisposedAndEnabled();
        _terminal.Curses.slk_clear()
                 .TreatError();
    }

    /// <summary>
    /// Restores the soft key labels to the screen. They can be cleared by calling <see cref="Clear"/> method.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void Restore()
    {
        AssertNotDisposedAndEnabled();
        _terminal.Curses.slk_restore()
                 .TreatError();
    }

    /// <summary>
    /// Invalidates the soft key labels. They will be queued for refresh the next time <see cref="Refresh"/> is called.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void Invalidate()
    {
        AssertNotDisposedAndEnabled();
        _terminal.Curses.slk_touch()
                 .TreatError();
    }

    /// <summary>
    /// Refreshes the soft label keys immediately.
    /// </summary>
    /// <param name="batch">If <c>true</c>, refresh is queued until the next screen update.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    public void Refresh(bool batch)
    {
        AssertNotDisposedAndEnabled();

        if (batch)
        {
            _terminal.Curses.slk_noutrefresh()
                     .TreatError();
        } else
        {
            _terminal.Curses.slk_refresh()
                     .TreatError();
        }
    }
}
