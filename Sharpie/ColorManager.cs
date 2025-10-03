/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
    private const short _standardColorCount = (short) StandardColor.White + 1;
    private readonly bool _ignorant;
    private short _nextPairHandle = 1;

    /// <summary>
    ///     Initializes color manager for a Curse provider.
    /// </summary>
    /// <param name="parent">The parent terminal.</param>
    /// <param name="enabled">Specifies whether colors are enabled.</param>
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="parent" /> is <c>null</c>.</exception>
    /// <remarks>This method is not thread-safe.</remarks>
    internal ColorManager(Terminal parent, bool enabled)
    {
        Terminal = parent ?? throw new ArgumentNullException(nameof(parent));

        if (enabled && Terminal.Curses.has_colors())
        {
            if (Terminal.Curses.start_color()
                        .Failed())
            {
                Mode = ColorMode.Disabled;
            }
            else
            {
                _ignorant = Terminal.Curses.use_default_colors()
                                    .Failed();

                var extended = !Terminal.Curses.init_pair(1, _standardColorCount, _standardColorCount)
                                        .Failed();

                var standard = !Terminal.Curses.init_pair(1, (short) StandardColor.White, (short) StandardColor.White)
                                        .Failed();

                Mode = extended ? ColorMode.Extended : standard ? ColorMode.Standard : ColorMode.Disabled;
            }
        }
        else
        {
            Mode = ColorMode.Disabled;
        }
    }

    /// <inheritdoc cref="IColorManager.Terminal" />
    public Terminal Terminal
    {
        get;
    }

    /// <inheritdoc cref="IColorManager.Terminal" />
    ITerminal IColorManager.Terminal => Terminal;

    /// <inheritdoc cref="IColorManager.Mode" />
    public ColorMode Mode
    {
        get;
    }

    /// <inheritdoc cref="IColorManager.CanRedefineColors" />
    public bool CanRedefineColors => Terminal.Curses.can_change_color();

    /// <inheritdoc cref="IColorManager.MixColors(short, short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public ColorMixture MixColors(short fgColor, short bgColor)
    {
        AssertHasColors();
        AssertSynchronized();

        var mixture = new ColorMixture { Handle = _nextPairHandle };
        RemixColors(mixture, fgColor, bgColor);

        _nextPairHandle++;

        return mixture;
    }

    /// <inheritdoc cref="IColorManager.MixColors(StandardColor, StandardColor)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public ColorMixture MixColors(StandardColor fgColor, StandardColor bgColor) =>
        MixColors((short) fgColor, (short) bgColor);

    /// <inheritdoc cref="IColorManager.RemixColors(ColorMixture, short, short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RemixColors(ColorMixture mixture, short fgColor, short bgColor)
    {
        AssertHasColors();
        AssertSynchronized();

        _ = Terminal.Curses.init_pair(mixture.Handle, MassageColor(fgColor, StandardColor.White),
                    MassageColor(bgColor, StandardColor.Black))
                .Check(nameof(Terminal.Curses.init_pair), "Failed to redefine an existing color mixture.");
    }

    /// <inheritdoc cref="IColorManager.RemixColors(ColorMixture, StandardColor, StandardColor)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RemixColors(ColorMixture mixture, StandardColor fgColor, StandardColor bgColor) =>
        RemixColors(mixture, (short) fgColor, (short) bgColor);

    /// <inheritdoc cref="IColorManager.RemixDefaultColors(short, short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RemixDefaultColors(short fgColor, short bgColor)
    {
        AssertHasColors();
        AssertSynchronized();

        _ = Terminal.Curses.assume_default_colors(MassageColor(fgColor, StandardColor.White),
                    MassageColor(bgColor, StandardColor.Black))
                .Check(nameof(Terminal.Curses.assume_default_colors), "Failed to redefine the default color mixture.");
    }

    /// <inheritdoc cref="IColorManager.RemixDefaultColors(StandardColor, StandardColor)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RemixDefaultColors(StandardColor fgColor, StandardColor bgColor) =>
        RemixDefaultColors((short) fgColor, (short) bgColor);

    /// <inheritdoc cref="IColorManager.UnMixColors" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public (short fgColor, short bgColor) UnMixColors(ColorMixture mixture)
    {
        AssertHasColors();
        AssertSynchronized();

        _ = Terminal.Curses.pair_content(mixture.Handle, out var fgColor, out var bgColor)
                .Check(nameof(Terminal.Curses.pair_content), "Failed to extract colors from the color mixture.");

        return (fgColor, bgColor);
    }

    /// <inheritdoc cref="IColorManager.RedefineColor(short, short, short, short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RedefineColor(short color, short red, short green, short blue)
    {
        AssertCanRedefineColors();
        AssertSynchronized();

        const short maxColor = 1000;
        _ = Terminal.Curses.init_color(color, Math.Min(red, maxColor), Math.Min(green, maxColor), Math.Min(blue, maxColor))
                .Check(nameof(Terminal.Curses.init_color), "Failed to redefine a terminal color.");
    }

    /// <inheritdoc cref="IColorManager.RedefineColor(StandardColor, short, short, short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public void RedefineColor(StandardColor color, short red, short green, short blue) =>
        RedefineColor((short) color, red, green, blue);

    /// <inheritdoc cref="IColorManager.BreakdownColor(short)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public (short red, short green, short blue) BreakdownColor(short color)
    {
        AssertCanRedefineColors();
        AssertSynchronized();

        _ = Terminal.Curses.color_content(color, out var red, out var green, out var blue)
                .Check(nameof(Terminal.Curses.color_content),
                    "Failed to extract RGB information from a terminal color.");

        return (red, green, blue);
    }

    /// <inheritdoc cref="IColorManager.BreakdownColor(StandardColor)" />
    /// <exception cref="CursesOperationException">A Curses error occurred.</exception>
    public (short red, short green, short blue) BreakdownColor(StandardColor color) => BreakdownColor((short) color);

    private short MassageColor(short color, StandardColor replacement)
    {
        return color == -1
            ? _ignorant ? (short) replacement : color
            : Mode == ColorMode.Standard ? (short) (color % _standardColorCount) : color;
    }

    private void AssertHasColors()
    {
        if (Mode == ColorMode.Disabled)
        {
            throw new NotSupportedException("The terminal does not support color mode.");
        }
    }

    private void AssertCanRedefineColors()
    {
        if (!CanRedefineColors)
        {
            throw new NotSupportedException("The terminal does not support color redefinition.");
        }
    }

    private void AssertSynchronized() => Terminal.AssertSynchronized();
}
