using Sharpie;

[assembly:System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

// Create the main terminal instance.
using var terminal = new Terminal(NativeCursesProvider.Instance, new(CaretMode: CaretMode.Invisible));

// Prepare styles
var styles = Enum.GetValues<StandardColor>()
                 .Where(sc => sc != StandardColor.Default && sc != StandardColor.Black)
                 .Select(sc => new Style
                 {
                     ColorMixture = terminal.Colors.MixColors(sc, StandardColor.Default),
                     Attributes = VideoAttribute.Bold
                 })
                 .ToArray();

// Prepare the glyph.
var glyph = new Drawing(new(1, 1));
var glyphStyle = Drawing.TriangleGlyphStyle.Up; 
var currentStyle = 0;
var x = -1;
var y = -1;
var dx = 1;
var dy = 1;

terminal.Repeat((t) =>
{
    glyph.Glyph(new(0, 0), glyphStyle, Drawing.GlyphSize.Normal, Drawing.FillStyle.Black, styles[currentStyle]);

    glyphStyle++;
    if (glyphStyle > Drawing.TriangleGlyphStyle.Right)
    {
        glyphStyle = Drawing.TriangleGlyphStyle.Up;
    }

    x += dx;
    y += dy;

    if (x <= 0)
    {
        x = 0;
        dx = 1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (x >= t.Screen.Size.Width)
    {
        x = t.Screen.Size.Width - 1;
        dx = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (y <= 0)
    {
        y = 0;
        dy = 1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    if (y >= t.Screen.Size.Height)
    {
        y = t.Screen.Size.Height - 1;
        dy = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }

    t.Screen.Clear();
    t.Screen.Draw(new(x, y), glyph);
    t.Screen.Refresh();
    
    return Task.CompletedTask;
}, 50);

await terminal.RunAsync(_ => Task.FromResult(true));