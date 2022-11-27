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

var window = terminal.Screen;
using var timer = new Timer(_ =>
{
    glyph.Glyph(new(0, 0), glyphStyle, Drawing.GlyphSize.Normal, Drawing.FillStyle.Black,
        styles[currentStyle]);

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
    if (x >= window.Size.Width)
    {
        x = window.Size.Width - 1;
        dx = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }
    if (y <= 0)
    {
        y = 0;
        dy = 1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }
    if (y >= window.Size.Height)
    {
        y = window.Size.Height - 1;
        dy = -1;
        currentStyle = (currentStyle + 1) % styles.Length;
    }
    
    window.Clear();
    window.Draw(new(x, y), glyph);
    window.Refresh();
}, null, 0, 50);

// The default event processing.
foreach (var @event in terminal.Screen.ProcessEvents(CancellationToken.None))
{
    // If the user pressed CTRL+C, break the loop.
    if (@event is KeyEvent { Key: Key.Character, Char.IsAscii: true, Char.Value: 'C', Modifiers: ModifierKey.Ctrl })
    {
        break;
    }
}
