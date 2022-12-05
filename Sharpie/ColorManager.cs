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
///     Exposes functionality to manage colors.
/// </summary>
[PublicAPI]
public sealed class ColorManager: IColorManager
{
    private readonly ICursesProvider _curses;
    private short _nextPairHandle = 1;

    /// <summary>
    ///     Initializes color manager for a Curse provider.
    /// </summary>
    /// <param name="curses">The curses provider.</param>
    /// <param name="enabled">Specifies whether colors are enabled.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="curses" /> is <c>null</c>.</exception>
    internal ColorManager(ICursesProvider curses, bool enabled)
    {
        _curses = curses ?? throw new ArgumentNullException(nameof(curses));

        if (enabled && ColorsAreSupported)
        {
            _curses.start_color()
                   .Check(nameof(_curses.start_color), "Failed to initialize terminal color mode.");

            _curses.use_default_colors()
                   .Check(nameof(_curses.use_default_colors), "Failed to defined the default colors of the terminal.");

            Enabled = true;
        }
    }

    /// <inheritdoc cref="IColorManager.Enabled"/>
    public bool Enabled { get; }

    /// <inheritdoc cref="IColorManager.ColorsAreSupported"/>
    public bool ColorsAreSupported => _curses.has_colors();

    /// <inheritdoc cref="IColorManager.CanRedefineColors"/>
    public bool CanRedefineColors => _curses.can_change_color();

    /// <inheritdoc cref="IColorManager.MixColors(short, short)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ColorMixture MixColors(short fgColor, short bgColor)
    {
        _curses.init_pair(_nextPairHandle, fgColor, bgColor)
               .Check(nameof(_curses.init_pair), "Failed to create a new color mixture.");

        var mixture = new ColorMixture { Handle = _nextPairHandle };
        _nextPairHandle++;

        return mixture;
    }

    /// <inheritdoc cref="IColorManager.RemixColors(Sharpie.ColorMixture, short, short)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void RemixColors(ColorMixture mixture, short fgColor, short bgColor)
    {
        _curses.init_pair(mixture.Handle, fgColor, bgColor)
               .Check(nameof(_curses.init_pair), "Failed to redefine an existing color mixture.");
    }

    /// <inheritdoc cref="IColorManager.RemixDefaultColors(short, short)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void RemixDefaultColors(short fgColor, short bgColor)
    {
        _curses.assume_default_colors(fgColor, bgColor)
               .Check(nameof(_curses.assume_default_colors), "Failed to redefine the default color mixture.");
    }

    /// <inheritdoc cref="IColorManager.UnMixColors"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public (short fgColor, short bgColor) UnMixColors(ColorMixture mixture)
    {
        _curses.pair_content(mixture.Handle, out var fgColor, out var bgColor)
               .Check(nameof(_curses.pair_content), "Failed to extract colors from the color mixture.");

        return (fgColor, bgColor);
    }

    /// <inheritdoc cref="IColorManager.RedefineColor(short, short, short, short)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void RedefineColor(short color, short red, short green, short blue)
    {
        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        if (red > 1000)
        {
            red = 1000;
        }

        if (green > 1000)
        {
            green = 1000;
        }

        if (blue > 1000)
        {
            blue = 1000;
        }

        _curses.init_color(color, red, green, blue)
               .Check(nameof(_curses.init_color), "Failed to redefine a terminal color.");
    }

    /// <inheritdoc cref="IColorManager.BreakdownColor(short)"/>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public (short red, short green, short blue) BreakdownColor(short color)
    {
        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support error redefinition.");
        }

        _curses.color_content(color, out var red, out var green, out var blue)
               .Check(nameof(_curses.color_content), "Failed to extract RGB information from a terminal color.");

        return (red, green, blue);
    }
}
