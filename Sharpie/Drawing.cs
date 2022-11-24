namespace Sharpie;

/// <summary>
///     A general-purpose drawing surface that can be latter applied on a <see cref="Window"/>.
///     Supports multiple types of drawing operations most commonly used in terminal apps.
/// </summary>
public sealed class Drawing1
{
    /// <summary>
    ///     Defines a fill quadrant that can be combined together to define the final character.
    /// </summary>
    [PublicAPI, Flags]
    public enum Quadrant
    {
        /// <summary>
        ///     Nothing filled.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        TopLeft = 1 << 0,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        TopRight = 1 << 1,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        BottomLeft = 1 << 2,

        /// <summary>
        ///     Fill the top-left quarter.
        /// </summary>
        BottomRight = 1 << 3
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
    public enum TextOrientation
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

    private readonly struct Cell
    {
        public int _special { get; init; }
        public Rune _rune { get; init; }
        public Style Style { get; init; }
    }

    private readonly Cell[,] _cells;
    private readonly Size _size;

    private static readonly Dictionary<Quadrant, Rune> BlockCharacters = new()
    {
        { Quadrant.None, new(' ') },
        { Quadrant.TopLeft, new('▘') },
        { Quadrant.TopRight, new('▝') },
        { Quadrant.BottomLeft, new('▖') },
        { Quadrant.BottomRight, new('▗') },
        { Quadrant.TopLeft | Quadrant.TopRight, new('▀') },
        { Quadrant.BottomLeft | Quadrant.BottomRight, new('▄') },
        { Quadrant.TopLeft | Quadrant.BottomLeft, new('▌') },
        { Quadrant.TopRight | Quadrant.BottomRight, new('▐') },
        { Quadrant.TopLeft | Quadrant.BottomRight, new('▚') },
        { Quadrant.TopRight | Quadrant.BottomLeft, new('▞') },
        { Quadrant.TopLeft | Quadrant.BottomLeft | Quadrant.TopRight, new('▛') },
        { Quadrant.TopLeft | Quadrant.TopRight | Quadrant.BottomRight, new('▜') },
        { Quadrant.TopLeft | Quadrant.BottomLeft | Quadrant.BottomRight, new('▙') },
        { Quadrant.TopRight | Quadrant.BottomLeft | Quadrant.BottomRight, new('▟') }
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
        _cells[x, y] = new() { _rune = rune, Style = style };
    }

    private void ValidateLocation(Point location)
    {
        if (location.X < 0 || location.X >= _size.Width || location.Y < 0 || location.Y >= _size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(location));
        }
    }

    private void ValidateLocation(Rectangle area)
    {
        if (area.Left < 0 || area.Top < 0 || area.Right - 1 >= _size.Width || area.Bottom - 1 >= _size.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(area));
        }
    }
    
    /// <summary>
    /// Fills a given <paramref name="area"/> with a given <paramref name="rune"/> and <paramref name="style"/>.
    /// </summary>
    /// <param name="area">The area to fill.</param>
    /// <param name="rune">The rune to draw.</param>
    /// <param name="style">The style to apply to the runes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="area"/> is invalid.</exception>
    public void Fill(Rectangle area, Rune rune, Style style)
    {
        ValidateLocation(area);
        
        for (var x = area.Left; x < area.Right; x++)
        {
            for (var y = area.Top; y < area.Bottom; y++)
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
    public void Fill(Rectangle area, ShadeStyle shade, Style style)
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
    public void Text(Point location, string text, Style style, TextOrientation orientation = TextOrientation.Horizontal)
    {
        ValidateLocation(location);

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
        foreach (var c in text.EnumerateRunes().TakeWhile(c => SetCell(x, y, c, style)))
        {
            if (orientation == TextOrientation.Horizontal)
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
    public void Glyph(Point location, 
        CheckStyle checkStyle, 
        FillStyle fillStyle,
        Style style)
    {
        ValidateLocation(location);
        
        var index = (int) checkStyle * 2 + (int) fillStyle;
        if (index < 0 || index >= CheckCharacters.Length)
        {
            throw new ArgumentException("Invalid style and fill combination.");
        }

        var rune = CheckCharacters[index];
        SetCell(location.X, location.Y, rune, style);
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
        Point location,
        GlyphDirection glyphDirection, 
        GlyphSize glyphSize, 
        FillStyle fillStyle,
        Style style)
    {
        var index = (int) glyphDirection * 4 + (int) glyphSize * 2 + (int) fillStyle;
        if (index < 0 || index >= TriangleCharacters.Length)
        {
            throw new ArgumentException("Invalid parameter combination");
        }

        var rune = TriangleCharacters[(int) glyphDirection * 4 + (int) glyphSize * 2 + (int) fillStyle];
        SetCell(location.X, location.Y, rune, style);
    }
    
    public void Block(Point location, Quadrant quadrants, int count)
    {
        if (!BlockCharacters.TryGetValue(quadrants, out var r))
        {
            throw new ArgumentException("Invalid quadrant combination.", nameof(quadrants));
        }
    }

}
