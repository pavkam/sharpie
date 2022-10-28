namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
///     Exposes functionality to manage colors.
/// </summary>
[PublicAPI]
public sealed class ColorManager
{
    private readonly bool _enabled;
    private ushort _nextPairHandle = 1;
    private Terminal _terminal;

    /// <summary>
    ///     Initializes color manager for a Curse provider.
    /// </summary>
    /// <param name="terminal">The parent terminal.</param>
    /// <param name="enabled">Specifies whether colors are enabled.</param>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    internal ColorManager(Terminal terminal, bool enabled)
    {
        _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

        if (enabled && ColorsAreSupported)
        {
            _terminal.Curses.start_color()
                     .Check(nameof(_terminal.Curses.start_color), "Failed to initialize terminal color mode.");

            _terminal.Curses.use_default_colors()
                     .Check(nameof(_terminal.Curses.use_default_colors),
                         "Failed to defined the default colors of the terminal.");
        }

        _enabled = true;
    }

    /// <summary>
    ///     Specifies whether the colors are enabled.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool Enabled
    {
        get
        {
            _terminal.AssertNotDisposed();

            return _enabled;
        }
    }

    /// <summary>
    ///     Specifies whether the terminal supports colors.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool ColorsAreSupported
    {
        get
        {
            _terminal.AssertNotDisposed();

            return _terminal.Curses.has_colors();
        }
    }

    /// <summary>
    ///     Specifies whether the terminal supports redefining colors.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool CanRedefineColors
    {
        get
        {
            _terminal.AssertNotDisposed();

            return _terminal.Curses.can_change_color();
        }
    }

    /// <summary>
    ///     Creates a new color mixture from the given colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The maximum number of pairs has been exhausted.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>A new color mixture.</returns>
    public ColorMixture MixColors(ushort fgColor, ushort bgColor)
    {
        if (_nextPairHandle == 0x100)
        {
            throw new InvalidOperationException("Exhausted the maximum number of color pairs.");
        }

        _terminal.AssertNotDisposed();
        _terminal.Curses.init_pair(_nextPairHandle, fgColor, bgColor)
                 .Check(nameof(_terminal.Curses.init_pair), "Failed to create a new color mixture.");

        var mixture = new ColorMixture { Handle = _nextPairHandle };
        _nextPairHandle++;

        return mixture;
    }

    /// <summary>
    ///     Creates a new color mixture from the given standard colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The maximum number of pairs has been exhausted.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <returns>A new color mixture.</returns>
    public ColorMixture MixColors(StandardColor fgColor, StandardColor bgColor) =>
        MixColors((ushort) fgColor, (ushort) bgColor);

    /// <summary>
    ///     Redefines an existing color pair with the given colors.
    /// </summary>
    /// <param name="mixture">The color mixture to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColorMixture(ColorMixture mixture, ushort fgColor, ushort bgColor)
    {
        _terminal.AssertNotDisposed();
        _terminal.Curses.init_pair(mixture.Handle, fgColor, bgColor)
                 .Check(nameof(_terminal.Curses.init_pair), "Failed to redefine an existing color mixture.");
    }

    /// <summary>
    ///     Redefines an existing color pair with the given standard colors.
    /// </summary>
    /// <param name="mixture">The color mixture to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColorMixture(ColorMixture mixture, StandardColor fgColor, StandardColor bgColor)
    {
        RedefineColorMixture(mixture, (ushort) fgColor, (ushort) bgColor);
    }

    /// <summary>
    ///     Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineDefaultColorMixture(ushort fgColor, ushort bgColor)
    {
        _terminal.AssertNotDisposed();
        _terminal.Curses.assume_default_colors(fgColor, bgColor)
                 .Check(nameof(_terminal.Curses.assume_default_colors),
                     "Failed to redefine the default color mixture.");
    }

    /// <summary>
    ///     Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineDefaultColorMixture(StandardColor fgColor, StandardColor bgColor)
    {
        RedefineDefaultColorMixture((ushort) fgColor, (ushort) bgColor);
    }

    /// <summary>
    ///     Redefines the color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">If any of the three components is greater than 1000.</exception>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColor(ushort color, ushort red, ushort green, ushort blue)
    {
        if (red > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(color));
        }

        if (green > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(color));
        }

        if (blue > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(color));
        }

        _terminal.AssertNotDisposed();
        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        _terminal.Curses.init_color(color, red, green, blue)
                 .Check(nameof(_terminal.Curses.init_color), "Failed to redefine a terminal color.");
    }

    /// <summary>
    ///     Redefines the standard color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">If any of the three components is greater than 1000.</exception>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColor(StandardColor color, ushort red, ushort green, ushort blue) =>
        RedefineColor((ushort) color, red, green, blue);

    /// <summary>
    ///     Extracts the RBG attributes from a color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (ushort red, ushort green, ushort blue) BreakdownColor(ushort color)
    {
        _terminal.AssertNotDisposed();

        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        _terminal.Curses.color_content(color, out var red, out var green, out var blue)
                 .Check(nameof(_terminal.Curses.color_content),
                     "Failed to extract RGB information from a terminal color.");

        return (red, green, blue);
    }

    /// <summary>
    ///     Extracts the RBG attributes from a standard color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (ushort red, ushort green, ushort blue) BreakdownColor(StandardColor color) =>
        BreakdownColor((ushort) color);

    /// <summary>
    ///     Extracts the colors of a color mixture.
    /// </summary>
    /// <param name="mixture">The color mixture to get the colors from.</param>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (ushort fgColor, ushort bgColor) BreakdownMixture(ColorMixture mixture)
    {
        _terminal.AssertNotDisposed();
        _terminal.Curses.pair_content(mixture.Handle, out var fgColor, out var bgColor)
                 .Check(nameof(_terminal.Curses.pair_content), "Failed to extract colors from the color mixture.");

        return (fgColor, bgColor);
    }
}
