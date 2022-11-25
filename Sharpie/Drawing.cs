namespace Sharpie;

/// <summary>
///     A general-purpose drawing surface that can be latter applied on a <see cref="Window"/>.
///     Supports multiple types of drawing operations most commonly used in terminal apps.
/// </summary>
public sealed class Drawing
{
    /// <summary>
    ///     The possible line styles used in <see cref="Line"/>.
    /// </summary>
    public enum LineStyle
    {
        /// <summary>
        /// Light style (default).
        /// </summary>
        Light,

        /// <summary>
        /// Heavy line.
        /// </summary>
        Heavy,

        /// <summary>
        /// Light dashed line.
        /// </summary>
        LightDashed,

        /// <summary>
        /// Heavy dashed line.
        /// </summary>
        HeavyDashed,

        /// <summary>
        /// Double line.
        /// </summary>
        Double,
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
    ///     The possible shapes used by the check.
    /// </summary>
    [PublicAPI]
    public enum CheckStyle
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
    ///     Describes the orientation of a glyph.
    /// </summary>
    [PublicAPI]
    public enum GlyphDirection
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
        Vertical,
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
    ///     The types of possible shades.
    /// </summary>
    [PublicAPI]
    public enum ShadeStyle
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
    ///     The gradient style.
    /// </summary>
    [PublicAPI]
    public enum GradientStyle
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
        BottomDouble = 1 << 19,
    }

    private readonly struct Cell
    {
        public int Special { get; init; }
        public Rune Rune { get; init; }
        public Style Style { get; init; }

        public BlockQuadrant? Block => Special > 0 ? (BlockQuadrant) Special : null;
    }

    private readonly Cell[,] _cells;
    private readonly Size _size;

    private static readonly Dictionary<BlockQuadrant, Rune> BlockCharacters = new()
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
        { BlockQuadrant.TopRight | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('▟') }
    };
    
    private static readonly Rune[] CheckCharacters = "◯●◇◆□■".Select(c => new Rune(c))
                                                    .ToArray();


    private static readonly Rune[] TriangleCharacters = "▲△▴▵▼▽▾▿◀◁◂◃▶▷▸▹".Select(c => new Rune(c))
                                                                 .ToArray();

    
    private static readonly Rune[] ShadeCharacters = " ░▒▓".Select(c => new Rune(c))
                                                       .ToArray();
    
    
    private static readonly Rune[] HorizontalGradientCharacters = " ▁▂▃▄▅▆▇█".Select(c => new Rune(c))
                                                                 .ToArray();

    private static readonly Rune[] VerticalGradientCharacters = " ▏▎▍▌▋▊▉█".Select(c => new Rune(c))
                                                               .ToArray();
    
    
    private static readonly Dictionary<LineSideAndStyle, Rune> BoxCharacters = new()
    {
        // LIGHT
        { LineSideAndStyle.RightLight, new('╶') },
        { LineSideAndStyle.LeftLight, new('╴') },
        { LineSideAndStyle.TopLight, new('╵') },
        { LineSideAndStyle.BottomLight, new('╷') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight , new('─') },
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
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight |  LineSideAndStyle.BottomLight, new('┬') },
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
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight, new('┼') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomHeavy, new('╁') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight | LineSideAndStyle.TopHeavy, new('╀') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.RightHeavy, new('┾') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy, new('┽') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy, new('╂') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.RightLight | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble, new('╫') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy, new('┿') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble, new('╪') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.TopLight | LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy, new('╆') },
        { LineSideAndStyle.LeftLight | LineSideAndStyle.BottomLight | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy, new('╄') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.TopLight | LineSideAndStyle.LeftHeavy | LineSideAndStyle.BottomHeavy, new('╅') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.BottomLight | LineSideAndStyle.LeftHeavy | LineSideAndStyle.TopHeavy, new('╃') },
        
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
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy  | LineSideAndStyle.TopHeavy, new('┻') },
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
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy, new('╋') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomLight, new('╇') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.RightHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.TopLight, new('╈') },
        { LineSideAndStyle.LeftHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.RightLight, new('╉') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.TopHeavy | LineSideAndStyle.BottomHeavy | LineSideAndStyle.LeftLight, new('╊') },

        // LIGHT DASHED
        { LineSideAndStyle.RightLightDashed | LineSideAndStyle.LeftLightDashed, new('┄') },
        { LineSideAndStyle.RightLightDashed | LineSideAndStyle.LeftLight, new('┄') },
        { LineSideAndStyle.RightLight | LineSideAndStyle.LeftLightDashed, new('┄') },
        { LineSideAndStyle.TopLightDashed | LineSideAndStyle.BottomLightDashed, new('┆') },
        { LineSideAndStyle.TopLightDashed | LineSideAndStyle.BottomLight, new('┆') },
        { LineSideAndStyle.TopLight | LineSideAndStyle.BottomLightDashed, new('┆') },

        // HEAVY DASHED
        { LineSideAndStyle.RightHeavyDashed | LineSideAndStyle.LeftHeavyDashed, new('┅') },
        { LineSideAndStyle.RightHeavyDashed | LineSideAndStyle.LeftHeavy , new('┅') },
        { LineSideAndStyle.RightHeavy | LineSideAndStyle.LeftHeavyDashed , new('┅') },
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
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomLight, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomHeavy, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.TopLight, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.RightDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.TopHeavy, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.RightLight, new('╬') },
        { LineSideAndStyle.LeftDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.RightHeavy, new('╬') },
        { LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.LeftLight, new('╬') },
        { LineSideAndStyle.RightDouble | LineSideAndStyle.TopDouble | LineSideAndStyle.BottomDouble | LineSideAndStyle.LeftHeavy, new('╬') },
    };

    /// <summary>
    /// Creates a new instances of this class with a given surface <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The size of the drawing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="size"/> is invalid.</exception>
    public Drawing1(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        _size = size;
        _cells = new Cell[size.Width, size.Height];
    }

    private bool SetCell(int x, int y, Rune rune, Style style)
    {
        if (x < 0 || x >= _size.Width || y < 0 || y >= _size.Height)
        {
            return false;
        }
        
        _cells[x, y] = new() { Rune = rune, Style = style, Special = 0 };

        return true;
    }

    private void SetCell(int x, int y, BlockQuadrant quads, Style style)
    {
        Debug.Assert(x < 0 || x >= _size.Width || y < 0 || y >= _size.Height);

        var b = (_cells[x, y]
                    .Block ??
                0) |
            quads;

        _cells[x, y] = new() { Special = (int) b, Style = style, Rune = BlockCharacters[b] };
    }

    private Point ValidateLocation(PointF location)
    {
        if (location.X < 0 || location.X >= _size.Width || location.Y < 0 || location.Y >= _size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(location));
        }

        return new((int) Math.Floor(location.X), (int) Math.Floor(location.Y));
    }

    private Rectangle ValidateArea(RectangleF area)
    {
        if (area.Left < 0 || area.Top < 0 || area.Right - 1 >= _size.Width || area.Bottom - 1 >= _size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        return new((int) Math.Floor(area.X), (int) Math.Floor(area.Y), (int) Math.Floor(area.Width),
            (int) Math.Floor(area.Height));
    }

    /// <summary>
    /// Fills a given <paramref name="area"/> with a given <paramref name="rune"/> and <paramref name="style"/>.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="style">The style to apply to the runes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Fill(RectangleF area, Rune rune, Style style)
    {
        var fixedArea = ValidateArea(area);
        
        for (var x = fixedArea.Left; x < fixedArea.Right; x++)
        {
            for (var y = fixedArea.Top; y < fixedArea.Bottom; y++)
            {
                SetCell(x, y, rune, style);
            }
        }
    }
    
    /// <summary>
    /// Fills a given <paramref name="area"/> with a given <paramref name="shade"/> and <paramref name="style"/>.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="shade">The share to draw.</param>
    /// <param name="style">The style to apply to the runes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Fill(RectangleF area, ShadeStyle shade, Style style)
    {
        if (shade < 0 || (int) shade > ShadeCharacters.Length)
        {
            throw new ArgumentException("Invalid shade style value.", nameof(shade));
        }

        var shadeChar = ShadeCharacters[(int) shade];
        Fill(area, shadeChar, style);
    }

    /// <summary>
    ///     Fills a given <paramref name="area"/> using <paramref name="orientation"/> for up to <paramref name="percent"/>.
    /// </summary>
    /// <param name="area">The area fill.</param>
    /// <param name="orientation">The gradient orientation.</param>
    /// <param name="percent">The percent of gradient that is filled.</param>
    /// <param name="style">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="percent" /> is negative or greater than
    ///     <c>1</c> or <paramref name="area" /> is invalid.
    /// </exception>
    public void Fill(Rectangle area, GradientStyle orientation, Style style, double percent)
    {
        ValidateLocation(area);

        var (fillRunes, cells) = orientation switch
        {
            GradientStyle.BottomToTop => (HorizontalGradientCharacters, area.Height),
            var _ => (VerticalGradientCharacters, area.Width)
        };

        var (fullCellCount, charIndex) = Helpers.CalculateOptionDistribution(cells, fillRunes.Length, percent);
        
        var gradient = new Rune[cells];
        Array.Fill(gradient, fillRunes.Last(), 0, fullCellCount);
        if (fullCellCount < cells - 1)
        {
            Array.Fill(gradient, fillRunes.First(), fullCellCount + 1, cells - fullCellCount);
        }

        gradient[fullCellCount] = fillRunes[charIndex];

        if (orientation == GradientStyle.BottomToTop)
        {
            for (var x = area.Left; x < area.Right; x++)
            {
                for (var y = 0; y < area.Height; y++)
                {
                    SetCell(x, y + area.Top, gradient[^y], style);
                }
            }
        } else
        {
            for (var y = area.Top; y < area.Bottom; y++)
            {
                for (var x = 0; x < area.Width; x++)
                {
                    SetCell(x + area.Left, y, gradient[x], style);
                }
            }
        }
    }

    /// <summary>
    /// Writes a given <paramref name="text"/> into the drawing. If the text length exceed the available space,
    /// it is cropped.
    /// </summary>
    /// <param name="location">The start location of the text.</param>
    /// <param name="text">The text.</param>
    /// <param name="style">The style to use for drawing.</param>
    /// <param name="orientation">The orientation for the text.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Text(PointF location, string text, Style style, Orientation orientation = Orientation.Horizontal)
    {
        var preciseLocation = ValidateLocation(location);

        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length == 0)
        {
            return;
        }

        var x = preciseLocation.X;
        var y = preciseLocation.Y;
        
        foreach (var c in text.EnumerateRunes())
        {
            if (x >= _size.Width || y >= _size.Height)
            {
                break;
            }
            
            SetCell(x, y, c, style);
            if (orientation == Orientation.Horizontal)
            {
                x++;
            } else
            {
                y++;
            }
        }
        
        foreach (var c in text.EnumerateRunes().TakeWhile(c => SetCell(x, y, c, style)))
        {
            if (orientation == Orientation.Horizontal)
            {
                x++;
            } else
            {
                y++;
            }
        }
    }
    
    /// <summary>
    /// Draws a glyph at a given <paramref name="location"/> using the provide styles.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="checkStyle">The check style of the glyph.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="style">The general style.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="checkStyle"/> or <paramref name="fillStyle"/> are invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Glyph(PointF location, 
        CheckStyle checkStyle, 
        FillStyle fillStyle,
        Style style)
    {
        var preciseLocation = ValidateLocation(location);
        
        var index = (int) checkStyle * 2 + (int) fillStyle;
        if (index < 0 || index >= CheckCharacters.Length)
        {
            throw new ArgumentException("Invalid style and fill combination.");
        }

        var rune = CheckCharacters[index];
        SetCell(preciseLocation.X, preciseLocation.Y, rune, style);
    }
    
    /// <summary>
    /// Draws a glyph at a given <paramref name="location"/> using the provide styles.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="glyphDirection">The orientation of the glyph.</param>
    /// <param name="glyphSize">The glyph size.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="style">The general style.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="glyphDirection"/> or <paramref name="glyphSize"/> or <paramref name="fillStyle"/> are invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Glyph(
        PointF location,
        GlyphDirection glyphDirection, 
        GlyphSize glyphSize, 
        FillStyle fillStyle,
        Style style)
    {
        var preciseLocation = ValidateLocation(location);
        
        var index = (int) glyphDirection * 4 + (int) glyphSize * 2 + (int) fillStyle;
        if (index < 0 || index >= TriangleCharacters.Length)
        {
            throw new ArgumentException("Invalid parameter combination");
        }

        var rune = TriangleCharacters[(int) glyphDirection * 4 + (int) glyphSize * 2 + (int) fillStyle];
        SetCell(preciseLocation.X, preciseLocation.Y, rune, style);
    }

    /// <summary>
    /// Draws a line starting at a given <paramref name="location"/> for a given <paramref name="length"/> and <paramref name="lineStyle"/>.
    /// </summary>
    /// <param name="location">The start location.</param>
    /// <param name="orientation">The line orientation.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="lineStyle">The line style.</param>
    /// <param name="textStyle">The text style.</param>
    public void Line(PointF location, Orientation orientation, float length, LineStyle lineStyle,
        Style textStyle)
    {
        
    }

    public static Rune Get(SideAndStyle lines)
    {
        if (TryGet(lines, 0, 0, out var result) ||
            TryGet(lines, SideAndStyle.RightLightDashed, SideAndStyle.RightLight, out result) ||
            TryGet(lines, SideAndStyle.LeftLightDashed, SideAndStyle.LeftLight, out result) ||
            TryGet(lines, SideAndStyle.TopLightDashed, SideAndStyle.TopLight, out result) ||
            TryGet(lines, SideAndStyle.BottomLightDashed, SideAndStyle.BottomLight, out result) ||
            TryGet(lines, SideAndStyle.RightHeavyDashed, SideAndStyle.RightHeavy, out result) ||
            TryGet(lines, SideAndStyle.LeftHeavyDashed, SideAndStyle.LeftHeavy, out result) ||
            TryGet(lines, SideAndStyle.TopHeavyDashed, SideAndStyle.TopHeavy, out result) ||
            TryGet(lines, SideAndStyle.BottomHeavyDashed, SideAndStyle.BottomHeavy, out result) ||
            TryGet(lines, SideAndStyle.RightDouble, SideAndStyle.RightHeavy, out result) ||
            TryGet(lines, SideAndStyle.LeftDouble, SideAndStyle.LeftHeavy, out result) ||
            TryGet(lines, SideAndStyle.TopDouble, SideAndStyle.TopHeavy, out result) ||
            TryGet(lines, SideAndStyle.BottomDouble, SideAndStyle.BottomHeavy, out result))
        {
            return result;
        }

        throw new ArgumentException("Invalid line combination.", nameof(lines));
    }

    private static bool TryGet(SideAndStyle lines, SideAndStyle replaceWhat, SideAndStyle replaceWith, out Rune rune)
    {
        if (replaceWhat != 0 && lines.HasFlag(replaceWhat))
        {
            lines = (lines & ~replaceWhat) | replaceWith;
        }

        return BoxCharacters.TryGetValue(lines, out rune);
    }
    
    
    /// <summary>
    /// Draws a block-based rectangle in the given <paramref name="area"/>.
    /// </summary>
    /// <param name="area">The area of the rectangle</param>
    /// <param name="style">The text style to use.</param>
    public void BlockRectangle(RectangleF area, Style style)
    {
        ValidateArea(area);

        var iX = (int)Math.Floor(area.X * 2);
        var iW = (int)Math.Floor(area.Right * 2);
        var iY = (int)Math.Floor(area.Y * 2);
        var iH = (int)Math.Floor(area.Bottom * 2);

        for (var x = iX; x < iW; x++)
        {
            for (var y = iY; y < iH; y++)
            {
                var quad = (x % 2, y % 2) switch
                {
                    (0, 0) => BlockQuadrant.TopLeft,
                    (0, 1) => BlockQuadrant.BottomLeft,
                    (1, 0) => BlockQuadrant.TopRight,
                    (1, 1) => BlockQuadrant.BottomRight,
                    var _ => (BlockQuadrant)0
                };

                SetCell(x / 2, y / 2, quad, style);
            }
        }
    }
}
