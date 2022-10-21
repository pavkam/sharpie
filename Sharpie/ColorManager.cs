namespace Sharpie;

using Curses;
using JetBrains.Annotations;

/// <summary>
/// Exposes functionality to manage colors.
/// </summary>
[PublicAPI]
public sealed class ColorManager
{
    private ushort _nextPairHandle;
    private readonly bool _enabled;
    private readonly ICursesProvider _cursesProvider;

    /// <summary>
    /// Initializes color manager for a Curse provider.
    /// </summary>
    /// <param name="cursesProvider">The curses functionality provider.</param>
    /// <param name="enabled">Specifies whether colors are enabled.</param>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    internal ColorManager(ICursesProvider cursesProvider, bool enabled)
    {
        _cursesProvider = cursesProvider ?? throw new ArgumentNullException(nameof(cursesProvider));

        if (enabled && ColorsAreSupported)
        {
            _cursesProvider.start_color()
                           .TreatError();

            _cursesProvider.use_default_colors()
                           .TreatError();
        }

        _enabled = true;
    }

    /// <summary>
    /// Specifies whether the colors are enabled.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool Enabled
    {
        get
        {
            _cursesProvider.AssertNotDisposed();

            return _enabled;
        }
    }

    /// <summary>
    /// Specifies whether the terminal supports colors.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool ColorsAreSupported
    {
        get
        {
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.has_colors();
        }
    }

    /// <summary>
    /// Specifies whether the terminal supports redefining colors.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    public bool CanRedefineColors
    {
        get
        {
            _cursesProvider.AssertNotDisposed();

            return _cursesProvider.can_change_color();
        }
    }

    /// <summary>
    /// Creates a new color pair from the given colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The maximum number of pairs has been exhausted.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorPair CreateColorPair(ushort fgColor, ushort bgColor)
    {
        if (_nextPairHandle == 0x100)
        {
            throw new InvalidOperationException("Exhausted the maximum number of color pairs.");
        }

        _cursesProvider.AssertNotDisposed();
        _cursesProvider.init_pair(_nextPairHandle, fgColor, bgColor)
                       .TreatError();

        var colorPair = new ColorPair { Handle = _nextPairHandle };
        _nextPairHandle++;

        return colorPair;
    }

    /// <summary>
    /// Creates a new color pair from the given standard colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The maximum number of pairs has been exhausted.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public ColorPair CreateColorPair(StandardColor fgColor, StandardColor bgColor) => CreateColorPair((ushort) fgColor, (ushort) bgColor);

    /// <summary>
    /// Redefines an existing color pair with the given colors.
    /// </summary>
    /// <param name="colorPair">The color pair to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColorPair(ColorPair colorPair, ushort fgColor, ushort bgColor)
    {
        _cursesProvider.AssertNotDisposed();
        _cursesProvider.init_pair(colorPair.Handle, fgColor, bgColor)
                       .TreatError();
    }

    /// <summary>
    /// Redefines an existing color pair with the given standard colors.
    /// </summary>
    /// <param name="colorPair">The color pair to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColorPair(ColorPair colorPair, StandardColor fgColor, StandardColor bgColor)
    {
        RedefineColorPair(colorPair, (ushort) fgColor, (ushort) bgColor);
    }


    /// <summary>
    /// Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineDefaultPair(ushort fgColor, ushort bgColor)
    {
        _cursesProvider.AssertNotDisposed();
        _cursesProvider.assume_default_colors(fgColor, bgColor)
                       .TreatError();
    }

    /// <summary>
    /// Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineDefaultPair(StandardColor fgColor, StandardColor bgColor)
    {
        RedefineDefaultPair((ushort) fgColor, (ushort) bgColor);
    }

    /// <summary>
    /// Redefines the color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    /// Before calling this function make sure that terminal supports this functionality by checking <see cref="CanRedefineColors"/>
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

        _cursesProvider.AssertNotDisposed();
        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        _cursesProvider.init_color(color, red, green, blue)
                       .TreatError();
    }

    /// <summary>
    /// Redefines the standard color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    /// Before calling this function make sure that terminal supports this functionality by checking <see cref="CanRedefineColors"/>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">If any of the three components is greater than 1000.</exception>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public void RedefineColor(StandardColor color, ushort red, ushort green, ushort blue) =>
        RedefineColor((ushort)color, red, green, blue);

    /// <summary>
    /// Extracts the RBG attributes from a color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    /// Before calling this function make sure that terminal supports this functionality by checking <see cref="CanRedefineColors"/>
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (ushort red, ushort green, ushort blue) BreakdownColor(ushort color)
    {
        _cursesProvider.AssertNotDisposed();

        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        _cursesProvider.color_content(color, out var red, out var green, out var blue)
                       .TreatError();

        return (red, green, blue);
    }

    /// <summary>
    /// Extracts the RBG attributes from a standard color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    /// Before calling this function make sure that terminal supports this functionality by checking <see cref="CanRedefineColors"/>
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="CursesException">A Curses error occured.</exception>
    public (ushort red, ushort green, ushort blue) BreakdownColor(StandardColor color) => BreakdownColor((ushort) color);
}
