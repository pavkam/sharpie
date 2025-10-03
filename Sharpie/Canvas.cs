/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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
///     A general-purpose drawing surface that can be latter draw onto any object that implements
///     <see cref="IDrawSurface" />.
///     Supports multiple types of drawing operations most commonly used in terminal apps.
/// </summary>
[PublicAPI]
public sealed class Canvas: IDrawable, IDrawSurface
{
    /// <summary>
    ///     The possible shapes used by the check.
    /// </summary>
    [PublicAPI]
    public enum CheckGlyphStyle
    {
        /// <summary>
        ///     A circle.
        /// </summary>
        Circle = 0,

        /// <summary>
        ///     A diamond.
        /// </summary>
        Diamond,

        /// <summary>
        ///     A square.
        /// </summary>
        Square
    }

    /// <summary>
    ///     The types fills.
    /// </summary>
    [PublicAPI]
    public enum FillStyle
    {
        /// <summary>
        ///     Normal, empty.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Filled.
        /// </summary>
        Black
    }

    /// <summary>
    ///     Describes the size of a glyph.
    /// </summary>
    [PublicAPI]
    public enum GlyphSize
    {
        /// <summary>
        ///     Normal size.
        /// </summary>
        Normal,

        /// <summary>
        ///     Small size.
        /// </summary>
        Small
    }

    /// <summary>
    ///     The gradient style.
    /// </summary>
    [PublicAPI]
    public enum GradientGlyphStyle
    {
        /// <summary>
        ///     Bottom-to-top fill.
        /// </summary>
        BottomToTop,

        /// <summary>
        ///     Left-to-right fill.
        /// </summary>
        LeftToRight
    }

    /// <summary>
    ///     The possible line styles used in <see cref="Line(PointF, float, Orientation, LineStyle, Style)" />.
    /// </summary>
    public enum LineStyle
    {
        /// <summary>
        ///     Light style (default).
        /// </summary>
        Light,

        /// <summary>
        ///     Heavy line.
        /// </summary>
        Heavy,

        /// <summary>
        ///     Light dashed line.
        /// </summary>
        LightDashed,

        /// <summary>
        ///     Heavy dashed line.
        /// </summary>
        HeavyDashed,

        /// <summary>
        ///     Double line.
        /// </summary>
        Double
    }

    /// <summary>
    ///     Describes the available text orientations.
    /// </summary>
    [PublicAPI]
    public enum Orientation
    {
        /// <summary>
        ///     Horizontal text.
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Vertical text.
        /// </summary>
        Vertical
    }

    /// <summary>
    ///     The types of possible shades.
    /// </summary>
    [PublicAPI]
    public enum ShadeGlyphStyle
    {
        /// <summary>
        ///     No shade.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Light shade.
        /// </summary>
        Light,

        /// <summary>
        ///     Medium shade.
        /// </summary>
        Medium,

        /// <summary>
        ///     Dark shade.
        /// </summary>
        Dark
    }

    /// <summary>
    ///     Describes the orientation of a glyph.
    /// </summary>
    [PublicAPI]
    public enum TriangleGlyphStyle
    {
        /// <summary>
        ///     Towards up.
        /// </summary>
        Up,

        /// <summary>
        ///     Towards down.
        /// </summary>
        Down,

        /// <summary>
        ///     Towards left.
        /// </summary>
        Left,

        /// <summary>
        ///     Towards right.
        /// </summary>
        Right
    }

    private static readonly Dictionary<BlockQuadrant, Rune> _blockCharacters = new()
    {
        { BlockQuadrant.TopLeft, new('▘') },
        { BlockQuadrant.TopRight, new('▝') },
        { BlockQuadrant.BottomLeft, new('▖') },
        { BlockQuadrant.BottomRight, new('▗') },
        { BlockQuadrant.TopLeft | BlockQuadrant.TopRight, new('▀') },
        { BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('▄') },
        { BlockQuadrant.TopLeft | BlockQuadrant.BottomLeft, new('▌') },
        { BlockQuadrant.TopRight | BlockQuadrant.BottomRight, new('▐') },
        { BlockQuadrant.TopLeft | BlockQuadrant.BottomRight, new('▚') },
        { BlockQuadrant.TopRight | BlockQuadrant.BottomLeft, new('▞') },
        { BlockQuadrant.TopLeft | BlockQuadrant.BottomLeft | BlockQuadrant.TopRight, new('▛') },
        { BlockQuadrant.TopLeft | BlockQuadrant.TopRight | BlockQuadrant.BottomRight, new('▜') },
        { BlockQuadrant.TopLeft | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('▙') },
        { BlockQuadrant.TopRight | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('▟') },
        {
            BlockQuadrant.TopLeft | BlockQuadrant.TopRight | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight,
            new('█')
        }
    };

    private static readonly Rune[] _checkCharacters = "●◯◆◇■□".Select(c => new Rune(c))
                                                             .ToArray();

    private static readonly Rune[] _triangleCharacters = "▲△▴▵▼▽▾▿◀◁◂◃▶▷▸▹".Select(c => new Rune(c))
                                                                          .ToArray();

    private static readonly Rune[] _shadeCharacters = " ░▒▓".Select(c => new Rune(c))
                                                           .ToArray();

    private static readonly Rune[] _horizontalGradientCharacters = " ▁▂▃▄▅▆▇█".Select(c => new Rune(c))
                                                                             .ToArray();

    private static readonly Rune[] _verticalGradientCharacters = " ▏▎▍▌▋▊▉█".Select(c => new Rune(c))
                                                                       .ToArray();

    private static readonly Dictionary<LineSideAndStyle, Rune> _boxCharacters = new()
    {
        // LIGHT
        { LineSideAndStyle.RightLight, new('╶') },
        { LineSideAndStyle.LeftLight, new('╴') },
        { LineSideAndStyle.TopLight, new('╵') },
        { LineSideAndStyle.BottomLight, new('╷') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight, new('─') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightHeavy, new('╼') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight, new('┘') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomLight, new('┐') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight, new('│') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomHeavy, new('╽') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight, new('└') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight, new('┌') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopHeavy, new('┚') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopDouble, new('╜') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomHeavy, new('┒') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomDouble, new('╖') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopHeavy, new('┖') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopDouble, new('╙') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomHeavy, new('┎') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomDouble, new('╓') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.LeftHeavy, new('┙') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.LeftDouble, new('╛') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.RightHeavy, new('┕') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.RightDouble, new('╘') },
        { LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy, new('┑') },
        { LineSideAndStyle.BottomLight | LineSideAndStyle.LeftDouble, new('╕') },
        { LineSideAndStyle.BottomLight | LineSideAndStyle.RightHeavy, new('┍') },
        { LineSideAndStyle.BottomLight | LineSideAndStyle.RightDouble, new('╒') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopLight, new('┴') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight, new('┬') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight, new('┤') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight | LineSideAndStyle.RightHeavy, new('┶') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomHeavy, new('┧') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomLight | LineSideAndStyle.RightHeavy, new('┮') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomLight | LineSideAndStyle.TopHeavy, new('┦') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopHeavy, new('┸') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopDouble, new('╨') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.BottomHeavy, new('┰') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.BottomDouble, new('╥') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight, new('├') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.LeftHeavy, new('┵') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomHeavy, new('┟') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy, new('┭') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight | LineSideAndStyle.TopHeavy, new('┞') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy, new('┥') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftDouble, new('╡') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.RightHeavy, new('┝') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.RightDouble, new('╞') },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomLight,
            new('┼')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomHeavy,
            new('╁')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.RightLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.TopHeavy,
            new('╀')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.RightHeavy,
            new('┾')
        },
        {
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.LeftHeavy,
            new('┽')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopHeavy |
            LineSideAndStyle.BottomHeavy,
            new('╂')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble,
            new('╫')
        },
        {
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.RightHeavy,
            new('┿')
        },
        {
            LineSideAndStyle.TopLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble,
            new('╪')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.BottomHeavy,
            new('╆')
        },
        {
            LineSideAndStyle.LeftLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.TopHeavy,
            new('╄')
        },
        {
            LineSideAndStyle.RightLight |
            LineSideAndStyle.TopLight |
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.BottomHeavy,
            new('╅')
        },
        {
            LineSideAndStyle.RightLight |
            LineSideAndStyle.BottomLight |
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.TopHeavy,
            new('╃')
        },

        // HEAVY
        { LineSideAndStyle.RightHeavy, new('╺') },
        { LineSideAndStyle.LeftHeavy, new('╸') },
        { LineSideAndStyle.TopHeavy, new('╹') },
        { LineSideAndStyle.BottomHeavy, new('╻') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy, new('━') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightLight, new('╾') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy, new('┃') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomLight, new('╿') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.TopHeavy, new('┛') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.BottomHeavy, new('┓') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy, new('┗') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy, new('┏') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.LeftHeavy, new('┫') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.RightHeavy, new('┣') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy, new('┻') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy, new('┳') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.RightLight, new('┹') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomLight, new('┩') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.RightLight, new('┱') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.TopLight, new('┪') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopLight, new('┷') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopDouble, new('╨') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomLight, new('┯') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomDouble, new('╥') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.LeftLight, new('┺') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomLight, new('┡') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.LeftLight, new('┲') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.TopLight, new('┢') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.LeftLight, new('┨') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.LeftDouble, new('╡') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.RightLight, new('┠') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.RightDouble, new('╞') },
        {
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.TopHeavy |
            LineSideAndStyle.BottomHeavy,
            new('╋')
        },
        {
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.TopHeavy |
            LineSideAndStyle.BottomLight,
            new('╇')
        },
        {
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.BottomHeavy |
            LineSideAndStyle.TopLight,
            new('╈')
        },
        {
            LineSideAndStyle.LeftHeavy |
            LineSideAndStyle.TopHeavy |
            LineSideAndStyle.BottomHeavy |
            LineSideAndStyle.RightLight,
            new('╉')
        },
        {
            LineSideAndStyle.RightHeavy |
            LineSideAndStyle.TopHeavy |
            LineSideAndStyle.BottomHeavy |
            LineSideAndStyle.LeftLight,
            new('╊')
        },

        // LIGHT DASHED
        { LineSideAndStyle.RightLightDashed | LineSideAndStyle.LeftLightDashed, new('┄') },
        { LineSideAndStyle.RightLightDashed | LineSideAndStyle.LeftLight, new('┄') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.LeftLightDashed, new('┄') },
        { LineSideAndStyle.TopLightDashed | LineSideAndStyle.BottomLightDashed, new('┆') },
        { LineSideAndStyle.TopLightDashed | LineSideAndStyle.BottomLight, new('┆') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLightDashed, new('┆') },

        // HEAVY DASHED
        { LineSideAndStyle.RightHeavyDashed | LineSideAndStyle.LeftHeavyDashed, new('┅') },
        { LineSideAndStyle.RightHeavyDashed | LineSideAndStyle.LeftHeavy, new('┅') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.LeftHeavyDashed, new('┅') },
        { LineSideAndStyle.TopHeavyDashed | LineSideAndStyle.BottomHeavyDashed, new('┇') },
        { LineSideAndStyle.TopHeavyDashed | LineSideAndStyle.BottomHeavy, new('┇') },
        { LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavyDashed, new('┇') },

        // DOUBLE
        { LineSideAndStyle.RightDouble, new('═') },
        { LineSideAndStyle.LeftDouble, new('═') },
        { LineSideAndStyle.TopDouble, new('║') },
        { LineSideAndStyle.BottomDouble, new('║') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble, new('═') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightLight, new('═') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightHeavy, new('═') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.TopDouble, new('╝') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.BottomDouble, new('╗') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble, new('║') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomLight, new('║') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomHeavy, new('║') },
        { LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble, new('╚') },
        { LineSideAndStyle.RightDouble | LineSideAndStyle.BottomDouble, new('╔') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble, new('╩') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.BottomDouble, new('╦') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopLight, new('╧') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopHeavy, new('╧') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.BottomLight, new('╤') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.BottomHeavy, new('╤') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.LeftDouble, new('╣') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.RightDouble, new('╠') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.LeftLight, new('╢') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.LeftHeavy, new('╢') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.RightLight, new('╟') },
        { LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.RightHeavy, new('╟') },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomLight,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomHeavy,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.TopLight,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.TopHeavy,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.RightLight,
            new('╬')
        },
        {
            LineSideAndStyle.LeftDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.RightHeavy,
            new('╬')
        },
        {
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.LeftLight,
            new('╬')
        },
        {
            LineSideAndStyle.RightDouble |
            LineSideAndStyle.TopDouble |
            LineSideAndStyle.BottomDouble |
            LineSideAndStyle.LeftHeavy,
            new('╬')
        }
    };

    private readonly Cell[,] _cells;

    /// <summary>
    ///     Creates a new instances of this class with a given surface <paramref name="size" />.
    /// </summary>
    /// <param name="size">The size of the drawing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="size" /> is invalid.</exception>
    public Canvas(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Size = size;
        _cells = new Cell[size.Width, size.Height];
    }

    /// <inheritdoc cref="IDrawable.Size" />
    public Size Size
    {
        get;
    }

    /// <inheritdoc cref="IDrawable.DrawOnto" />
    public void DrawOnto(IDrawSurface destination, Rectangle srcArea, Point destLocation)
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (!Size.AdjustToActualArea(ref srcArea))
        {
            return;
        }

        var destArea = srcArea with
        {
            X = destLocation.X,
            Y = destLocation.Y
        };
        if (!destination.Size.AdjustToActualArea(ref destArea))
        {
            return;
        }

        for (var x = srcArea.X; x < destArea.Width; x++)
        {
            for (var y = srcArea.Y; y < destArea.Height; y++)
            {
                var cell = _cells[x, y];
                if (cell.Rune.Value != 0)
                {
                    destination.DrawCell(new(x + destLocation.X, y + destLocation.Y), cell.Rune, cell.Style);
                }
            }
        }
    }

    /// <inheritdoc cref="IDrawSurface.DrawCell" />
    void IDrawSurface.DrawCell(Point location, Rune rune, Style style) => SetCell(location.X, location.Y, rune, style);

    private bool InArea(int x, int y) => x >= 0 && x < Size.Width && y >= 0 && y < Size.Height;

    private void SetCell(int x, int y, Rune rune, Style style)
    {
        if (!InArea(x, y))
        {
            return;
        }

        if (rune is { IsAscii: true, Value: <= ControlCharacter.Escape })
        {
            rune = new(ControlCharacter.Whitespace);
        }

        _cells[x, y] = new()
        {
            Rune = rune,
            Style = style,
            Special = 0
        };
    }

    private void SetCell(int x, int y, BlockQuadrant quads, Style style)
    {
        if (!InArea(x, y))
        {
            return;
        }

        var b = (_cells[x, y]
                    .Block ??
                0) |
            quads;

        _cells[x, y] = new()
        {
            Special = (int) b,
            Style = style,
            Rune = _blockCharacters[b]
        };
    }

    private void SetCell(int x, int y, LineSideAndStyle stl, Style style)
    {
        if (!InArea(x, y))
        {
            return;
        }

        var b = (_cells[x, y]
                    .Line ??
                0) |
            stl;

        bool tryGet(LineSideAndStyle replaceWhat, LineSideAndStyle replaceWith, out Rune r)
        {
            if (replaceWhat != 0 && b.HasFlag(replaceWhat))
            {
                b = (b & ~replaceWhat) | replaceWith;
            }

            return _boxCharacters.TryGetValue(b, out r);
        }

        if (!tryGet(0, 0, out var rune) &&
            !tryGet(LineSideAndStyle.RightLightDashed, LineSideAndStyle.RightLight, out rune) &&
            !tryGet(LineSideAndStyle.LeftLightDashed, LineSideAndStyle.LeftLight, out rune) &&
            !tryGet(LineSideAndStyle.TopLightDashed, LineSideAndStyle.TopLight, out rune) &&
            !tryGet(LineSideAndStyle.BottomLightDashed, LineSideAndStyle.BottomLight, out rune) &&
            !tryGet(LineSideAndStyle.RightHeavyDashed, LineSideAndStyle.RightHeavy, out rune) &&
            !tryGet(LineSideAndStyle.LeftHeavyDashed, LineSideAndStyle.LeftHeavy, out rune) &&
            !tryGet(LineSideAndStyle.TopHeavyDashed, LineSideAndStyle.TopHeavy, out rune) &&
            !tryGet(LineSideAndStyle.BottomHeavyDashed, LineSideAndStyle.BottomHeavy, out rune) &&
            !tryGet(LineSideAndStyle.RightDouble, LineSideAndStyle.RightHeavy, out rune) &&
            !tryGet(LineSideAndStyle.LeftDouble, LineSideAndStyle.LeftHeavy, out rune) &&
            !tryGet(LineSideAndStyle.TopDouble, LineSideAndStyle.TopHeavy, out rune) &&
            !tryGet(LineSideAndStyle.BottomDouble, LineSideAndStyle.BottomHeavy, out rune))
        {
            rune = new(0);
        }

        if (rune is { IsAscii: true, Value: 0 })
        {
            throw new ArgumentOutOfRangeException(nameof(stl));
        }

        _cells[x, y] = new()
        {
            Special = -(int) b,
            Style = style,
            Rune = rune
        };
    }

    /// <summary>
    ///     Fills a given <paramref name="area" /> with a given <paramref name="rune" /> and <paramref name="style" />.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="style">The cell style.</param>
    public void Fill(Rectangle area, Rune rune, Style style)
    {
        if (!Size.AdjustToActualArea(ref area))
        {
            return;
        }

        for (var x = area.Left; x < area.Right; x++)
        {
            for (var y = area.Top; y < area.Bottom; y++)
            {
                SetCell(x, y, rune, style);
            }
        }
    }

    /// <summary>
    ///     Fills a given <paramref name="area" /> with a given <paramref name="shadeGlyph" /> and
    ///     <paramref name="style" />.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="shadeGlyph">The share to draw.</param>
    /// <param name="style">The cell style.</param>
    public void Fill(Rectangle area, ShadeGlyphStyle shadeGlyph, Style style)
    {
        if (shadeGlyph < 0 || (int) shadeGlyph > _shadeCharacters.Length)
        {
            throw new ArgumentException("Invalid shade style value.", nameof(shadeGlyph));
        }

        var shadeChar = _shadeCharacters[(int) shadeGlyph];
        Fill(area, shadeChar, style);
    }

    /// <summary>
    ///     Writes a given <paramref name="text" /> into the drawing. If the text length exceed the available space,
    ///     it is cropped.
    /// </summary>
    /// <param name="location">The start location of the text.</param>
    /// <param name="text">The text.</param>
    /// <param name="style">The text style.</param>
    /// <param name="orientation">The orientation for the text.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text" /> is <c>null</c>.</exception>
    public void Text(Point location, string text, Orientation orientation, Style style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length == 0)
        {
            return;
        }

        var x = location.X;
        var y = location.Y;

        foreach (var c in text.EnumerateRunes())
        {
            if (x >= Size.Width || y >= Size.Height)
            {
                break;
            }

            SetCell(x, y, c, style);

            if (orientation == Orientation.Horizontal)
            {
                x++;
            }
            else
            {
                y++;
            }
        }
    }

    /// <summary>
    ///     Draws a glyph at a given <paramref name="location" /> using the given text style.
    /// </summary>
    /// <param name="location">The cell location.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="style">The text style.</param>
    public void Glyph(Point location, Rune rune, Style style) => SetCell(location.X, location.Y, rune, style);

    /// <summary>
    ///     Draws a glyph at a given <paramref name="location" /> using the provide styles.
    /// </summary>
    /// <param name="location">The cell location.</param>
    /// <param name="checkGlyphStyle">The check style of the glyph.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="style">The cell style.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="checkGlyphStyle" /> or <paramref name="fillStyle" /> are
    ///     invalid.
    /// </exception>
    public void Glyph(Point location, CheckGlyphStyle checkGlyphStyle, FillStyle fillStyle, Style style)
    {
        var index = (int) checkGlyphStyle * 2 + (int) fillStyle;
        if (index < 0 || index >= _checkCharacters.Length)
        {
            throw new ArgumentException("Invalid style and fill combination.");
        }

        Glyph(location, _checkCharacters[index], style);
    }

    /// <summary>
    ///     Draws a glyph at a given <paramref name="location" /> using the provide styles.
    /// </summary>
    /// <param name="location">The cell location.</param>
    /// <param name="triangleGlyphStyle">The orientation of the glyph.</param>
    /// <param name="glyphSize">The glyph size.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="style">The cell style.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="triangleGlyphStyle" /> or <paramref name="glyphSize" />
    ///     or <paramref name="fillStyle" /> are invalid.
    /// </exception>
    public void Glyph(Point location, TriangleGlyphStyle triangleGlyphStyle, GlyphSize glyphSize, FillStyle fillStyle,
        Style style)
    {
        var index = (int) triangleGlyphStyle * 4 + (int) glyphSize * 2 + (int) fillStyle;
        if (index < 0 || index >= _triangleCharacters.Length)
        {
            throw new ArgumentException("Invalid parameter combination");
        }

        var rune = _triangleCharacters[(int) triangleGlyphStyle * 4 + (int) glyphSize * 2 + (int) fillStyle];
        Glyph(location, rune, style);
    }

    /// <summary>
    ///     Draws a glyph at a given <paramref name="location" /> using the provide gradient style and fill count.
    /// </summary>
    /// <param name="location">The cell location.</param>
    /// <param name="gradientGlyphStyle">The gradient style.</param>
    /// <param name="fill">The glyph fill count.</param>
    /// <param name="style">The cell style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="fill" /> is invalid.</exception>
    public void Glyph(Point location, GradientGlyphStyle gradientGlyphStyle, int fill, Style style)
    {
        var runes = gradientGlyphStyle == GradientGlyphStyle.BottomToTop
            ? _horizontalGradientCharacters
            : _verticalGradientCharacters;

        if (fill < 0 || fill >= runes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(fill));
        }

        Glyph(location, runes[fill], style);
    }

    /// <summary>
    ///     Draws a line starting at a given starting at a given point vertically or horizontally using line drawing
    ///     characters.
    /// </summary>
    /// <param name="location">The start location.</param>
    /// <param name="orientation">The line orientation.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="lineStyle">The line style.</param>
    /// <param name="style">The cell style.</param>
    public void Line(PointF location, float length, Orientation orientation, LineStyle lineStyle,
        Style style)
    {
        if (length < 0)
        {
            return;
        }

        if (orientation == Orientation.Horizontal)
        {
            if (location.X + length < 0)
            {
                return;
            }

            var y = (int) Math.Floor(location.Y);

            foreach (var (i, left) in Helpers.EnumerateInHalves(location.X, length))
            {
                if (i >= Size.Width)
                {
                    break;
                }

                var stl = (left, lineStyle) switch
                {
                    (true, LineStyle.Light) => LineSideAndStyle.LeftLight,
                    (true, LineStyle.Heavy) => LineSideAndStyle.LeftHeavy,
                    (true, LineStyle.LightDashed) => LineSideAndStyle.LeftLightDashed,
                    (true, LineStyle.HeavyDashed) => LineSideAndStyle.LeftHeavyDashed,
                    (true, LineStyle.Double) => LineSideAndStyle.LeftDouble,
                    (false, LineStyle.Light) => LineSideAndStyle.RightLight,
                    (false, LineStyle.Heavy) => LineSideAndStyle.RightHeavy,
                    (false, LineStyle.LightDashed) => LineSideAndStyle.RightLightDashed,
                    (false, LineStyle.HeavyDashed) => LineSideAndStyle.RightHeavyDashed,
                    (false, LineStyle.Double) => LineSideAndStyle.RightDouble,
                    var _ => (LineSideAndStyle) 0
                };

                SetCell(i, y, stl, style);
            }
        }
        else
        {
            if (location.Y + length < 0)
            {
                return;
            }

            var x = (int) Math.Floor(location.X);

            foreach (var (i, top) in Helpers.EnumerateInHalves(location.Y, length))
            {
                if (i >= Size.Height)
                {
                    break;
                }

                var stl = (top, lineStyle) switch
                {
                    (true, LineStyle.Light) => LineSideAndStyle.TopLight,
                    (true, LineStyle.Heavy) => LineSideAndStyle.TopHeavy,
                    (true, LineStyle.LightDashed) => LineSideAndStyle.TopLightDashed,
                    (true, LineStyle.HeavyDashed) => LineSideAndStyle.TopHeavyDashed,
                    (true, LineStyle.Double) => LineSideAndStyle.TopDouble,
                    (false, LineStyle.Light) => LineSideAndStyle.BottomLight,
                    (false, LineStyle.Heavy) => LineSideAndStyle.BottomHeavy,
                    (false, LineStyle.LightDashed) => LineSideAndStyle.BottomLightDashed,
                    (false, LineStyle.HeavyDashed) => LineSideAndStyle.BottomHeavyDashed,
                    (false, LineStyle.Double) => LineSideAndStyle.BottomDouble,
                    var _ => (LineSideAndStyle) 0
                };

                SetCell(x, i, stl, style);
            }
        }
    }

    /// <summary>
    ///     Draws a line between two points in the drawing using block characters.
    /// </summary>
    /// <param name="startLocation">The starting cell.</param>
    /// <param name="endLocation">The ending cell.</param>
    /// <param name="style">The cell style.</param>
    public void Line(PointF startLocation, PointF endLocation, Style style) => Helpers.TraceLineInHalves(startLocation, endLocation, p => Point(p, style));

    /// <summary>
    ///     Draws a rectangle starting in a given <paramref name="perimeter" />.
    /// </summary>
    /// <param name="perimeter">The box perimeter.</param>
    /// <param name="lineStyle">The line style.</param>
    /// <param name="style">The text style.</param>
    public void Box(Rectangle perimeter, LineStyle lineStyle, Style style)
    {
        if (!Size.AdjustToActualArea(ref perimeter))
        {
            return;
        }

        Line(new(perimeter.X + 0.5F, perimeter.Y), Math.Max(0.5F, perimeter.Width - 1), Orientation.Horizontal,
            lineStyle, style);

        Line(new(perimeter.X + 0.5F, perimeter.Bottom - 1), Math.Max(0.5F, perimeter.Width - 1), Orientation.Horizontal,
            lineStyle, style);

        Line(new(perimeter.X, perimeter.Y + 0.5F), Math.Max(0.5F, perimeter.Height - 1), Orientation.Vertical,
            lineStyle, style);

        Line(new(perimeter.Right - 1, perimeter.Y + 0.5F), Math.Max(0.5F, perimeter.Height - 1), Orientation.Vertical,
            lineStyle, style);
    }

    /// <summary>
    ///     Draws a block-based rectangle in the given <paramref name="area" />.
    /// </summary>
    /// <param name="area">The area of the rectangle.</param>
    /// <param name="style">The text style to use.</param>
    public void Rectangle(RectangleF area, Style style)
    {
        if (!Size.AdjustToActualArea(ref area))
        {
            return;
        }

        foreach (var (x, left) in Helpers.EnumerateInHalves(area.X, area.Width))
        {
            foreach (var (y, top) in Helpers.EnumerateInHalves(area.Y, area.Height))
            {
#pragma warning disable IDE0072 // Add missing cases -- all cases are covered
                var quad = (left, top) switch
                {
                    (true, true) => BlockQuadrant.TopLeft,
                    (true, false) => BlockQuadrant.BottomLeft,
                    (false, true) => BlockQuadrant.TopRight,
                    (false, false) => BlockQuadrant.BottomRight,
                };
#pragma warning restore IDE0072 // Add missing cases
                SetCell(x, y, quad, style);
            }
        }
    }

    /// <summary>
    ///     Draws a block-based point in the given <paramref name="location" />.
    /// </summary>
    /// <param name="location">The location of the point.</param>
    /// <param name="style">The text style to use.</param>
    public void Point(PointF location, Style style)
    {
        var x = (int) Math.Floor(location.X * 2);
        var y = (int) Math.Floor(location.Y * 2);
#pragma warning disable IDE0072 // Add missing cases -- all cases are covered
        var quad = (x % 2 == 0, y % 2 == 0) switch
        {
            (true, true) => BlockQuadrant.TopLeft,
            (true, false) => BlockQuadrant.BottomLeft,
            (false, true) => BlockQuadrant.TopRight,
            (false, false) => BlockQuadrant.BottomRight,
        };
#pragma warning restore IDE0072 // Add missing cases
        SetCell(x / 2, y / 2, quad, style);
    }

    [Flags]
    private enum BlockQuadrant
    {
        TopLeft = 1 << 0,
        TopRight = 1 << 1,
        BottomLeft = 1 << 2,
        BottomRight = 1 << 3
    }

    [Flags]
    private enum LineSideAndStyle
    {
        RightLight = 1 << 0,
        LeftLight = 1 << 1,
        TopLight = 1 << 2,
        BottomLight = 1 << 3,
        RightHeavy = 1 << 4,
        LeftHeavy = 1 << 5,
        TopHeavy = 1 << 6,
        BottomHeavy = 1 << 7,
        RightLightDashed = 1 << 8,
        LeftLightDashed = 1 << 9,
        TopLightDashed = 1 << 10,
        BottomLightDashed = 1 << 11,
        RightHeavyDashed = 1 << 12,
        LeftHeavyDashed = 1 << 13,
        TopHeavyDashed = 1 << 14,
        BottomHeavyDashed = 1 << 15,
        RightDouble = 1 << 16,
        LeftDouble = 1 << 17,
        TopDouble = 1 << 18,
        BottomDouble = 1 << 19
    }

    private readonly struct Cell
    {
        public int Special
        {
            get; init;
        }
        public Rune Rune
        {
            get; init;
        }
        public Style Style
        {
            get; init;
        }

        public BlockQuadrant? Block => Special > 0 ? (BlockQuadrant) Special : null;
        public LineSideAndStyle? Line => Special < 0 ? (LineSideAndStyle) (-Special) : null;
    }
}
