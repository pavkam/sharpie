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

namespace Sharpie.Abstractions;

/// <summary>
///     Defines the traits needed to implement <see cref="ColorManager" />.
/// </summary>
[PublicAPI]
public interface IColorManager
{
    /// <summary>
    ///     The terminal this manager belongs to.
    /// </summary>
    ITerminal Terminal { get; }

    /// <summary>
    ///     Specifies whether the colors are enabled.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    ///     Specifies whether the terminal supports colors.
    /// </summary>
    bool ColorsAreSupported { get; }

    /// <summary>
    ///     Specifies whether the terminal supports redefining colors.
    /// </summary>
    bool CanRedefineColors { get; }

    /// <summary>
    ///     Creates a new color mixture from the given colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <returns>A new color mixture.</returns>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ColorMixture MixColors(short fgColor, short bgColor);

    /// <summary>
    ///     Creates a new color mixture from the given standard colors.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <returns>A new color mixture.</returns>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ColorMixture MixColors(StandardColor fgColor, StandardColor bgColor);

    /// <summary>
    ///     Redefines an existing color pair with the given colors.
    /// </summary>
    /// <param name="mixture">The color mixture to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void RemixColors(ColorMixture mixture, short fgColor, short bgColor);

    /// <summary>
    ///     Redefines an existing color pair with the given standard colors.
    /// </summary>
    /// <param name="mixture">The color mixture to redefine.</param>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void RemixColors(ColorMixture mixture, StandardColor fgColor, StandardColor bgColor);

    /// <summary>
    ///     Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void RemixDefaultColors(short fgColor, short bgColor);

    /// <summary>
    ///     Redefines the default colors of the terminal.
    /// </summary>
    /// <param name="fgColor">The foreground color.</param>
    /// <param name="bgColor">The background color.</param>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void RemixDefaultColors(StandardColor fgColor, StandardColor bgColor);

    /// <summary>
    ///     Extracts the colors of a color mixture.
    /// </summary>
    /// <param name="mixture">The color mixture to get the colors from.</param>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    /// <remarks>This operation is not thread safe.</remarks>
    (short fgColor, short bgColor) UnMixColors(ColorMixture mixture);

    /// <summary>
    ///     Redefines the color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />. This operation is not thread safe.
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    void RedefineColor(short color, short red, short green, short blue);

    /// <summary>
    ///     Redefines the standard color's RGB attributes (if supported).
    /// </summary>
    /// <param name="color">The color to redefine.</param>
    /// <param name="red">The value of red (0-1000).</param>
    /// <param name="green">The value of green (0-1000).</param>
    /// <param name="blue">The value of blue (0-1000).</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />. This operation is not thread safe.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">If any of the three components is greater than 1000.</exception>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    void RedefineColor(StandardColor color, short red, short green, short blue);

    /// <summary>
    ///     Extracts the RBG attributes from a color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />. This operation is not thread safe.
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    (short red, short green, short blue) BreakdownColor(short color);

    /// <summary>
    ///     Extracts the RBG attributes from a standard color.
    /// </summary>
    /// <param name="color">The color to get the RGB from.</param>
    /// <remarks>
    ///     Before calling this function make sure that terminal supports this functionality by checking
    ///     <see cref="CanRedefineColors" />. This operation is not thread safe.
    /// </remarks>
    /// <exception cref="NotSupportedException">If the terminal does not support redefining colors.</exception>
    /// <exception cref="CursesSynchronizationException">Thrown if this operation was expected to run on the main thread/context but wasn't.</exception>
    (short red, short green, short blue) BreakdownColor(StandardColor color);
}
