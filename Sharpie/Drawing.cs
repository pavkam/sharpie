namespace Sharpie;

/// <summary>
///     A general-purpose drawing surface that can be latter applied on a <see cref="IDrawSurface"/>.
///     Supports multiple types of drawing operations most commonly used in terminal apps.
/// </summary>
[PublicAPI]
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
        public LineSideAndStyle? Line => Special < 0 ? (LineSideAndStyle) (-Special) : null;
    }

    private readonly Cell[,] _cells;

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
        { BlockQuadrant.TopRight | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('▟') },
        { BlockQuadrant.TopLeft | BlockQuadrant.TopRight | BlockQuadrant.BottomLeft | BlockQuadrant.BottomRight, new('█') }
    };
    
    private static readonly Rune[] CheckCharacters = "●◯◆◇■□".Select(c => new Rune(c))
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
    /// The size of the drawing.
    /// </summary>
    public Size Size { get; }
    
    /// <summary>
    /// Creates a new instances of this class with a given surface <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The size of the drawing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="size"/> is invalid.</exception>
    public Drawing(Size size)
    {
        if (size.Width < 1 || size.Height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Size = size;
        _cells = new Cell[size.Width, size.Height];
    }

    private void SetCell(int x, int y, Rune rune, Style style)
    {
        Debug.Assert(x >= 0 && x < Size.Width && y >= 0 && y < Size.Height);
        
        _cells[x, y] = new() { Rune = rune, Style = style, Special = 0 };
    }

    private void SetCell(int x, int y, BlockQuadrant quads, Style style)
    {
        Debug.Assert(x >= 0 && x < Size.Width && y >= 0 && y < Size.Height);

        var b = (_cells[x, y]
                    .Block ??
                0) |
            quads;

        _cells[x, y] = new() { Special = (int) b, Style = style, Rune = BlockCharacters[b] };
    }

    private void SetCell(int x, int y, LineSideAndStyle stl, Style style)
    {
        Debug.Assert(x >= 0 && x < Size.Width && y >= 0 && y < Size.Height);

        var b = (_cells[x, y]
                    .Line ??
                0) |
            stl;

        bool TryGet(LineSideAndStyle replaceWhat, LineSideAndStyle replaceWith, out Rune r)
        {
            if (replaceWhat != 0 && b.HasFlag(replaceWhat))
            {
                b = (b & ~replaceWhat) | replaceWith;
            }

            return BoxCharacters.TryGetValue(b, out r);
        }

        if (!TryGet(0, 0, out var rune) &&
            !TryGet(LineSideAndStyle.RightLightDashed, LineSideAndStyle.RightLight, out rune) &&
            !TryGet(LineSideAndStyle.LeftLightDashed, LineSideAndStyle.LeftLight, out rune) &&
            !TryGet(LineSideAndStyle.TopLightDashed, LineSideAndStyle.TopLight, out rune) &&
            !TryGet(LineSideAndStyle.BottomLightDashed, LineSideAndStyle.BottomLight, out rune) &&
            !TryGet(LineSideAndStyle.RightHeavyDashed, LineSideAndStyle.RightHeavy, out rune) &&
            !TryGet(LineSideAndStyle.LeftHeavyDashed, LineSideAndStyle.LeftHeavy, out rune) &&
            !TryGet(LineSideAndStyle.TopHeavyDashed, LineSideAndStyle.TopHeavy, out rune) &&
            !TryGet(LineSideAndStyle.BottomHeavyDashed, LineSideAndStyle.BottomHeavy, out rune) &&
            !TryGet(LineSideAndStyle.RightDouble, LineSideAndStyle.RightHeavy, out rune) &&
            !TryGet(LineSideAndStyle.LeftDouble, LineSideAndStyle.LeftHeavy, out rune) &&
            !TryGet(LineSideAndStyle.TopDouble, LineSideAndStyle.TopHeavy, out rune) &&
            !TryGet(LineSideAndStyle.BottomDouble, LineSideAndStyle.BottomHeavy, out rune))
        {
            rune = new(0);
        }

        _cells[x, y] = new() { Special = -(int) b, Style = style, Rune = rune };
    }

    internal Point Validate(PointF location, int quanta = 1)
    {
        if (location.X < 0 || location.X >= Size.Width || location.Y < 0 || location.Y >= Size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(location));
        }

        return new((int) Math.Floor(location.X * quanta), (int) Math.Floor(location.Y * quanta));
    }

    internal Rectangle Validate(RectangleF area, int quanta = 1)
    {
        if (area.Left < 0 || area.Top < 0 || area.Right - 1 >= Size.Width || area.Bottom - 1 >= Size.Height
            || area.Width < 0 || area.Height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }

        return new((int) Math.Floor(area.X * quanta), (int) Math.Floor(area.Y * quanta), (int) Math.Floor(area.Width * quanta),
            (int) Math.Floor(area.Height * quanta));
    }

    /// <summary>
    /// Fills a given <paramref name="area"/> with a given <paramref name="rune"/> and <paramref name="textStyle"/>.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="textStyle">The text style..</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Fill(RectangleF area, Rune rune, Style textStyle)
    {
        var fixedArea = Validate(area);
        
        for (var x = fixedArea.Left; x < fixedArea.Right; x++)
        {
            for (var y = fixedArea.Top; y < fixedArea.Bottom; y++)
            {
                SetCell(x, y, rune, textStyle);
            }
        }
    }
    
    /// <summary>
    /// Fills a given <paramref name="area"/> with a given <paramref name="shadeGlyph"/> and <paramref name="textStyle"/>.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="shadeGlyph">The share to draw.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Fill(RectangleF area, ShadeGlyphStyle shadeGlyph, Style textStyle)
    {
        if (shadeGlyph < 0 || (int) shadeGlyph > ShadeCharacters.Length)
        {
            throw new ArgumentException("Invalid shade style value.", nameof(shadeGlyph));
        }

        var shadeChar = ShadeCharacters[(int) shadeGlyph];
        Fill(area, shadeChar, textStyle);
    }

    /// <summary>
    /// Writes a given <paramref name="text"/> into the drawing. If the text length exceed the available space,
    /// it is cropped.
    /// </summary>
    /// <param name="location">The start location of the text.</param>
    /// <param name="text">The text.</param>
    /// <param name="textStyle">The text style.</param>
    /// <param name="orientation">The orientation for the text.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Text(PointF location, string text, Style textStyle, Orientation orientation = Orientation.Horizontal)
    {
        var preciseLocation = Validate(location);

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
            if (x >= Size.Width || y >= Size.Height)
            {
                break;
            }
            
            SetCell(x, y, c, textStyle);
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
    /// Draws a glyph at a given <paramref name="location"/> using the given text style.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Glyph(
        PointF location,
        Rune rune,
        Style textStyle)
    {
        var preciseLocation = Validate(location);
        SetCell(preciseLocation.X, preciseLocation.Y, rune, textStyle);
    }
    
    /// <summary>
    /// Draws a glyph at a given <paramref name="location"/> using the provide styles.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="checkGlyphStyle">The check style of the glyph.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="checkGlyphStyle"/> or <paramref name="fillStyle"/> are invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Glyph(PointF location, 
        CheckGlyphStyle checkGlyphStyle, 
        FillStyle fillStyle,
        Style textStyle)
    {
        var index = (int) checkGlyphStyle * 2 + (int) fillStyle;
        if (index < 0 || index >= CheckCharacters.Length)
        {
            throw new ArgumentException("Invalid style and fill combination.");
        }

        Glyph(location, CheckCharacters[index], textStyle);
    }
    
    /// <summary>
    /// Draws a glyph at a given <paramref name="location"/> using the provide styles.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="triangleGlyphStyle">The orientation of the glyph.</param>
    /// <param name="glyphSize">The glyph size.</param>
    /// <param name="fillStyle">The fill style of the glyph.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="triangleGlyphStyle"/> or <paramref name="glyphSize"/> or <paramref name="fillStyle"/> are invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Glyph(
        PointF location,
        TriangleGlyphStyle triangleGlyphStyle, 
        GlyphSize glyphSize, 
        FillStyle fillStyle,
        Style textStyle)
    {
        var index = (int) triangleGlyphStyle * 4 + (int) glyphSize * 2 + (int) fillStyle;
        if (index < 0 || index >= TriangleCharacters.Length)
        {
            throw new ArgumentException("Invalid parameter combination");
        }
        
        var rune = TriangleCharacters[(int) triangleGlyphStyle * 4 + (int) glyphSize * 2 + (int) fillStyle];
        Glyph(location, rune, textStyle);
    }
    
    /// <summary>
    /// Draws a glyph at a given <paramref name="location"/> using the provide gradient style and fill count.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="gradientGlyphStyle">The gradient style.</param>
    /// <param name="fill">The glyph fill count.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> or <paramref name="fill"/> are invalid.</exception>
    public void Glyph(
        PointF location,
        GradientGlyphStyle gradientGlyphStyle,
        int fill,
        Style textStyle)
    {
        var runes = gradientGlyphStyle == GradientGlyphStyle.BottomToTop
            ? HorizontalGradientCharacters
            : VerticalGradientCharacters;

        if (fill < 0 || fill >= runes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(fill));
        }

        Glyph(location, runes[fill], textStyle);
    }

    /// <summary>
    /// Draws a line starting at a given <paramref name="location"/> for a given <paramref name="length"/> and <paramref name="lineStyle"/>.
    /// </summary>
    /// <param name="location">The start location.</param>
    /// <param name="orientation">The line orientation.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="lineStyle">The line style.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> or <paramref name="length"/> are invalid.</exception>
    public void Line(PointF location, float length, Orientation orientation, LineStyle lineStyle,
        Style textStyle)
    {
        var preciseArea = orientation == Orientation.Horizontal
            ? Validate((RectangleF) new(location.X, location.Y, length, 1), 2)
            : Validate((RectangleF) new(location.X, location.Y, 1, length), 2);

        if (orientation == Orientation.Horizontal)
        {
            for (var x = preciseArea.X; x < preciseArea.Right; x++)
            {
                var stl = (x % 2, lineStyle) switch
                {
                    (0, LineStyle.Light) => LineSideAndStyle.LeftLight,
                    (0, LineStyle.Heavy) => LineSideAndStyle.LeftHeavy,
                    (0, LineStyle.LightDashed) => LineSideAndStyle.LeftLightDashed,
                    (0, LineStyle.HeavyDashed) => LineSideAndStyle.LeftHeavyDashed,
                    (0, LineStyle.Double) => LineSideAndStyle.LeftDouble,
                    (1, LineStyle.Light) => LineSideAndStyle.RightLight,
                    (1, LineStyle.Heavy) => LineSideAndStyle.RightHeavy,
                    (1, LineStyle.LightDashed) => LineSideAndStyle.RightLightDashed,
                    (1, LineStyle.HeavyDashed) => LineSideAndStyle.RightHeavyDashed,
                    (1, LineStyle.Double) => LineSideAndStyle.RightDouble,
                    var _ => (LineSideAndStyle) 0
                };

                SetCell(x / 2, preciseArea.Y / 2, stl, textStyle);
            }
        } else
        {
            for (var y = preciseArea.Y; y < preciseArea.Bottom; y++)
            {
                var stl = (y % 2, lineStyle) switch
                {
                    (0, LineStyle.Light) => LineSideAndStyle.TopLight,
                    (0, LineStyle.Heavy) => LineSideAndStyle.TopHeavy,
                    (0, LineStyle.LightDashed) => LineSideAndStyle.TopLightDashed,
                    (0, LineStyle.HeavyDashed) => LineSideAndStyle.TopHeavyDashed,
                    (0, LineStyle.Double) => LineSideAndStyle.TopDouble,
                    (1, LineStyle.Light) => LineSideAndStyle.BottomLight,
                    (1, LineStyle.Heavy) => LineSideAndStyle.BottomHeavy,
                    (1, LineStyle.LightDashed) => LineSideAndStyle.BottomLightDashed,
                    (1, LineStyle.HeavyDashed) => LineSideAndStyle.BottomHeavyDashed,
                    (1, LineStyle.Double) => LineSideAndStyle.BottomDouble,
                    var _ => (LineSideAndStyle) 0
                };

                SetCell(preciseArea.X / 2, y / 2, stl, textStyle);
            }
        }
    }

    /// <summary>
    /// Draws a rectangle starting in a given <paramref name="perimeter"/>.
    /// </summary>
    /// <param name="perimeter">The box perimeter.</param>
    /// <param name="lineStyle">The line style.</param>
    /// <param name="textStyle">The text style.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="perimeter"/> is invalid.</exception>
    public void Box(RectangleF perimeter, LineStyle lineStyle, Style textStyle)
    {
        Line(new(perimeter.X, perimeter.Y), perimeter.Width, Orientation.Horizontal, lineStyle, textStyle);
        Line(new(perimeter.X, perimeter.Bottom - 1), perimeter.Width, Orientation.Horizontal, lineStyle, textStyle);
        Line(new(perimeter.X, perimeter.Y), perimeter.Height, Orientation.Vertical, lineStyle, textStyle);
        Line(new(perimeter.Right - 1, perimeter.Y), perimeter.Height, Orientation.Vertical, lineStyle, textStyle);
    }

    /// <summary>
    /// Draws a block-based rectangle in the given <paramref name="area"/>.
    /// </summary>
    /// <param name="area">The area of the rectangle</param>
    /// <param name="style">The text style to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Rectangle(RectangleF area, Style style)
    {
        var preciseArea = Validate(area, 2);

        for (var x = preciseArea.X; x < preciseArea.Right; x++)
        {
            for (var y = preciseArea.Y; y < preciseArea.Bottom; y++)
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
    
    /// <summary>
    /// Draws a block-based point in the given <paramref name="location"/>.
    /// </summary>
    /// <param name="location">The location of the point.</param>
    /// <param name="textStyle">The text style to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="location"/> is invalid.</exception>
    public void Point(PointF location, Style textStyle)
    {
        var preciseLocation = Validate(location, 2);
        var quad = (preciseLocation.X % 2, preciseLocation.Y % 2) switch
        {
            (0, 0) => BlockQuadrant.TopLeft,
            (0, 1) => BlockQuadrant.BottomLeft,
            (1, 0) => BlockQuadrant.TopRight,
            (1, 1) => BlockQuadrant.BottomRight,
            var _ => (BlockQuadrant)0
        };

        SetCell(preciseLocation.X / 2, preciseLocation.Y / 2, quad, textStyle);
    }

    /// <summary>
    /// Draws the drawing onto a given surface.
    /// </summary>
    /// <param name="destination">The surface to draw on.</param>
    /// <param name="srcArea">The source area to draw.</param>
    /// <param name="destLocation">The destination location.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="srcArea"/> or <paramref name="destLocation"/> are invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="destination"/> is <c>null</c>.</exception>
    public void DrawTo(IDrawSurface destination, Rectangle srcArea, Point destLocation)
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        Validate(srcArea);

        var destArea = srcArea with { X = destLocation.X, Y = destLocation.Y };
        if (!destination.CoversArea(destArea))
        {
            throw new ArgumentOutOfRangeException(nameof(destLocation));
        }

        for (var x = srcArea.X; x < srcArea.Right; x++)
        {
            for (var y = srcArea.Y; y < srcArea.Bottom; y++)
            {
                if (_cells[x, y]
                    .Rune.Value !=
                    0)
                {
                    destination.DrawCell(new(x, y), _cells[x, y]
                        .Rune, _cells[x, y]
                        .Style);
                }
            }
        }
    }
}
