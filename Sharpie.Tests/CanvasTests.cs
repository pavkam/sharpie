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

namespace Sharpie.Tests;

[TestClass]
public class CanvasTests
{
    private readonly Style _style1 = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 } };
    private Canvas _canvas1X1 = null!;
    private Canvas _canvas2X2 = null!;
    private Canvas _canvas3X3 = null!;
    private Mock<IDrawSurface> _drawSurfaceMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _drawSurfaceMock = new();
        _canvas1X1 = new(new(1, 1));
        _canvas2X2 = new(new(2, 2));
        _canvas3X3 = new(new(3, 3));
    }

    [TestMethod, DataRow(0, 1), DataRow(1, 0), SuppressMessage("ReSharper", "ObjectCreationAsStatement"),
     SuppressMessage("Performance", "CA1806:Do not ignore method results")]
    public void Ctor_Throws_IfSizeIsInvalid(int width, int height)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { new Canvas(new(width, height)); });
    }

    [TestMethod]
    public void Ctor_InitializesTheSize()
    {
        var size = new Size(10, 20);
        var d = new Canvas(size);
        d.Size.ShouldBe(size);
    }

    [TestMethod]
    public void DrawOnto_Throws_IfTheDestinationINull()
    {
        Should.Throw<ArgumentNullException>(() => { _canvas1X1.DrawOnto(null!, Rectangle.Empty, Point.Empty); });
    }

    [TestMethod]
    public void DrawOnto_DoesNothing_IfAreaOutside()
    {
        _canvas1X1.DrawOnto(_drawSurfaceMock.Object, new(1, 1, 2, 3), new(10, 10));

        _drawSurfaceMock.Verify(v => v.Size, Times.Never);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()), Times.Never);
    }

    [TestMethod]
    public void DrawOnto_DoesNothing_IfAreaOutsideDestination()
    {
        _drawSurfaceMock.Setup(s => s.Size)
                        .Returns(new Size(10, 10));

        _canvas1X1.DrawOnto(_drawSurfaceMock.Object, new(0, 0, 2, 3), new(10, 10));

        _drawSurfaceMock.Verify(v => v.Size, Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()), Times.Never);
    }

    [TestMethod]
    public void DrawOnto_DrawsTheAdjustedArea()
    {
        _drawSurfaceMock.Setup(s => s.Size)
                        .Returns(new Size(10, 10));

        _canvas2X2.Glyph(new(0, 0), new('A'), _style1);
        _canvas2X2.Glyph(new(1, 0), new('B'), _style1);
        _canvas2X2.Glyph(new(0, 1), new('C'), _style1);
        _canvas2X2.Glyph(new(1, 1), new('D'), _style1);

        _canvas2X2.DrawOnto(_drawSurfaceMock.Object, new(0, 0, 2, 2), new(9, 8));

        _drawSurfaceMock.Verify(v => v.DrawCell(new(9, 8), new('A'), _style1), Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(new(9, 9), new('C'), _style1), Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()),
            Times.Exactly(2));
    }

    [TestMethod]
    public void DrawOnto_SkipsEmptyCells()
    {
        _drawSurfaceMock.Setup(s => s.Size)
                        .Returns(new Size(10, 10));

        _canvas2X2.Glyph(new(1, 1), new('A'), _style1);

        _canvas2X2.DrawOnto(_drawSurfaceMock.Object, new(0, 0, 2, 2), new(5, 6));
        _drawSurfaceMock.Verify(v => v.DrawCell(new(6, 7), new('A'), _style1), Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()), Times.Once);
    }

    [TestMethod]
    public void DrawCell_DoesNothing_IfOutside()
    {
        ((IDrawSurface) _canvas1X1).DrawCell(new(1, 1), new('Z'), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void DrawCell_DrawsGlyph()
    {
        ((IDrawSurface) _canvas1X1).DrawCell(new(0, 0), new('Z'), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('Z'), _style1));
    }

    [TestMethod]
    public void Glyph1_DoesNothing_IfOutside()
    {
        _canvas1X1.Glyph(new(1, 1), new('Z'), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Glyph1_DrawsGlyph()
    {
        _canvas1X1.Glyph(new(0, 0), new('Z'), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('Z'), _style1));
    }

    [TestMethod]
    public void Glyph1_ReplacesSpecialChars_WithWhitespace()
    {
        for (var ch = 0; ch <= ControlCharacter.Escape; ch++)
        {
            _canvas1X1.Glyph(new(0, 0), new(ch), _style1);
            _canvas1X1.GetContents()[0, 0]
                      .ShouldBe((new(' '), _style1));
        }
    }

    [TestMethod]
    public void Glyph2_Throws_IfStyleIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _canvas1X1.Glyph(new(1, 1), (Canvas.CheckGlyphStyle) 100, Canvas.FillStyle.Black, _style1);
        });
    }

    [TestMethod]
    public void Glyph2_DrawsGlyph()
    {
        _canvas1X1.Glyph(new(0, 0), Canvas.CheckGlyphStyle.Diamond, Canvas.FillStyle.Black, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('◇'), _style1));
    }

    [TestMethod]
    public void Glyph3_Throws_IfStyleIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _canvas1X1.Glyph(new(1, 1), (Canvas.TriangleGlyphStyle) 100, Canvas.GlyphSize.Normal,
                Canvas.FillStyle.Black, _style1);
        });
    }

    [TestMethod]
    public void Glyph3_DrawsGlyph()
    {
        _canvas1X1.Glyph(new(0, 0), Canvas.TriangleGlyphStyle.Down, Canvas.GlyphSize.Normal, Canvas.FillStyle.Black,
            _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('▽'), _style1));
    }

    [TestMethod, DataRow(-1), DataRow(9)]
    public void Glyph4_Throws_IfFillIsInvalid(int fill)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _canvas1X1.Glyph(new(1, 1), Canvas.GradientGlyphStyle.LeftToRight, fill, _style1);
        });
    }

    [TestMethod]
    public void Glyph4_DrawsGlyph()
    {
        _canvas1X1.Glyph(new(0, 0), Canvas.GradientGlyphStyle.LeftToRight, 8, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('█'), _style1));
    }

    [TestMethod]
    public void Fill1_FillsArea()
    {
        _canvas2X2.Fill(new(0, 0, 2, 1), new Rune('A'), _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('A'), _style1));

        c[1, 0]
            .ShouldBe((new('A'), _style1));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Fill1_FillsNothing_IfAreaIsEmpty()
    {
        _canvas2X2.Fill(new(0, 0, 2, 0), new Rune('A'), _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .Item1.ShouldBe(new(0));

        c[1, 0]
            .Item1.ShouldBe(new(0));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Fill2_Throws_IfShadeIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _canvas2X2.Fill(new(0, 0, 2, 3), (Canvas.ShadeGlyphStyle) 100, _style1);
        });
    }

    [TestMethod]
    public void Fill2_FillsArea()
    {
        _canvas2X2.Fill(new(0, 0, 2, 1), Canvas.ShadeGlyphStyle.Dark, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('▓'), _style1));

        c[1, 0]
            .ShouldBe((new('▓'), _style1));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Text_Throws_IfTextIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
        {
            _canvas2X2.Text(new(0, 0), null!, Canvas.Orientation.Horizontal, _style1);
        });
    }

    [TestMethod]
    public void Text_DoesNothingIfTextIsEmpty()
    {
        _canvas1X1.Text(new(0, 0), "", Canvas.Orientation.Horizontal, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Text_DrawsEmoji()
    {
        const string emoji = "❤️";
        Rune.TryGetRuneAt(emoji, 0, out var rune);

        _canvas1X1.Text(new(0, 0), emoji, Canvas.Orientation.Horizontal, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((rune, _style1));
    }

    [TestMethod]
    public void Text_TextThatFits_Horizontal()
    {
        _canvas2X2.Text(new(0, 0), "text", Canvas.Orientation.Horizontal, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('t'), _style1));

        c[1, 0]
            .ShouldBe((new('e'), _style1));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[0, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Text_TextThatFits_Vertical()
    {
        _canvas2X2.Text(new(0, 0), "text", Canvas.Orientation.Vertical, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('t'), _style1));

        c[0, 1]
            .ShouldBe((new('e'), _style1));

        c[1, 0]
            .Item1.ShouldBe(new(0));

        c[1, 0]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Point_DoesNothing_IfOutside()
    {
        _canvas1X1.Point(new(1, 1), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod, DataRow(0.4F, 0.4F, '▘'), DataRow(0.5F, 0.4F, '▝'), DataRow(0.4F, 0.5F, '▖'), DataRow(0.5F, 0.5F, '▗')]
    public void Point_DrawsSinglePoint(float x, float y, char c)
    {
        _canvas1X1.Point(new(x, y), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new(c), _style1));
    }

    [TestMethod]
    public void Point_CombinesPoints()
    {
        _canvas1X1.Point(new(0, 0), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('▘'), _style1));

        _canvas1X1.Point(new(0.6F, 0), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('▀'), _style1));

        _canvas1X1.Point(new(0.6F, 0.9F), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('▜'), _style1));

        _canvas1X1.Point(new(0.2F, 0.5F), _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('█'), _style1));
    }

    [TestMethod]
    public void Rectangle_DoesNothing_IfAreaOutside()
    {
        _canvas2X2.Rectangle(new(2, 2, 2, 2), _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .Item1.ShouldBe(new(0));

        c[1, 0]
            .Item1.ShouldBe(new(0));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Rectangle_DrawsRectangle()
    {
        _canvas2X2.Rectangle(new(0.5F, 0.5F, 1.2F, 1.4F), _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('▗'), _style1));

        c[1, 0]
            .ShouldBe((new('▖'), _style1));

        c[0, 1]
            .ShouldBe((new('▝'), _style1));

        c[1, 1]
            .ShouldBe((new('▘'), _style1));
    }

    [TestMethod]
    public void Box_DoesNothing_WhenOutsideArea()
    {
        _canvas1X1.Box(new(1, 1, 1, 1), Canvas.LineStyle.Double, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Box_DrawsCross_When1X1()
    {
        _canvas1X1.Box(new(0, 0, 1, 1), Canvas.LineStyle.Double, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╔'), _style1));
    }

    [TestMethod]
    public void Box_DrawsBox_When2X1()
    {
        _canvas2X2.Box(new(0, 0, 2, 1), Canvas.LineStyle.Double, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('╔'), _style1));

        c[1, 0]
            .ShouldBe((new('╗'), _style1));

        c[0, 1]
            .Item1.ShouldBe(new(0));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Box_DrawsBox_When1X2()
    {
        _canvas2X2.Box(new(0, 0, 1, 2), Canvas.LineStyle.Double, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('╔'), _style1));

        c[1, 0]
            .Item1.ShouldBe(new(0));

        c[0, 1]
            .ShouldBe((new('╚'), _style1));

        c[1, 1]
            .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Box_DrawsBox_When2X2()
    {
        _canvas2X2.Box(new(0, 0, 2, 2), Canvas.LineStyle.Double, _style1);
        var c = _canvas2X2.GetContents();
        c[0, 0]
            .ShouldBe((new('╔'), _style1));

        c[1, 0]
            .ShouldBe((new('╗'), _style1));

        c[0, 1]
            .ShouldBe((new('╚'), _style1));

        c[1, 1]
            .ShouldBe((new('╝'), _style1));
    }

    [TestMethod]
    public void Box_DrawsBox_When3X3()
    {
        _canvas3X3.Box(new(0, 0, 3, 3), Canvas.LineStyle.Double, _style1);
        var c = _canvas3X3.GetContents();
        c[0, 0]
            .ShouldBe((new('╔'), _style1));

        c[1, 0]
            .ShouldBe((new('═'), _style1));

        c[2, 0]
            .ShouldBe((new('╗'), _style1));

        c[0, 1]
            .ShouldBe((new('║'), _style1));

        c[1, 1]
            .Item1.ShouldBe(new(0));

        c[0, 2]
            .ShouldBe((new('╚'), _style1));

        c[1, 2]
            .ShouldBe((new('═'), _style1));

        c[2, 2]
            .ShouldBe((new('╝'), _style1));
    }

    [TestMethod]
    public void Line1_DoesNothing_IfLengthLessThanZero()
    {
        _canvas1X1.Line(new(0, 0), -0.1f, Canvas.Orientation.Horizontal, Canvas.LineStyle.Light, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Line1_DoesNothing_IfXPlusLengthIsLessThanZero()
    {
        _canvas1X1.Line(new(-1, 0), 0.5f, Canvas.Orientation.Horizontal, Canvas.LineStyle.Light, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Line1_DoesNothing_IfYPlusLengthIsLessThanZero()
    {
        _canvas1X1.Line(new(0, -1), 0.5f, Canvas.Orientation.Vertical, Canvas.LineStyle.Light, _style1);
        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Line1_StopsAtWidth()
    {
        _canvas1X1.Line(new(0, 0), 2, Canvas.Orientation.Horizontal, Canvas.LineStyle.Double, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('═'), _style1));
    }

    [TestMethod]
    public void Line1_StopsAtHeight()
    {
        _canvas1X1.Line(new(0, 0), 2, Canvas.Orientation.Vertical, Canvas.LineStyle.Double, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('║'), _style1));
    }

    [TestMethod]
    public void Line1_ExitsNegative_OnX()
    {
        _canvas1X1.Line(new(-1, 0), 2, Canvas.Orientation.Horizontal, Canvas.LineStyle.Double, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('═'), _style1));
    }

    [TestMethod]
    public void Line1_ExitsNegative_OnY()
    {
        _canvas1X1.Line(new(0, -1), 2, Canvas.Orientation.Vertical, Canvas.LineStyle.Double, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('║'), _style1));
    }

    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line1_DrawsHalfLine1_Horizontal_AtStart(float y)
    {
        _canvas1X1.Line(new(0, y), 0.5F, Canvas.Orientation.Horizontal, Canvas.LineStyle.Light, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╴'), _style1));
    }

    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line1_DrawsHalfLine1_Horizontal_AtEnd(float y)
    {
        _canvas1X1.Line(new(0.5F, y), 0.5F, Canvas.Orientation.Horizontal, Canvas.LineStyle.Light, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╶'), _style1));
    }

    [TestMethod, DataRow(Canvas.LineStyle.Light, '─'), DataRow(Canvas.LineStyle.Heavy, '━'),
     DataRow(Canvas.LineStyle.LightDashed, '┄'), DataRow(Canvas.LineStyle.HeavyDashed, '┅'),
     DataRow(Canvas.LineStyle.Double, '═')]
    public void Line1_DrawsFullLine1_Horizontal(Canvas.LineStyle style, char exp)
    {
        _canvas1X1.Line(new(0, 0), 1, Canvas.Orientation.Horizontal, style, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new(exp), _style1));
    }

    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line1_DrawsHalfLine1_Vertical_AtStart(float x)
    {
        _canvas1X1.Line(new(x, 0), 0.5F, Canvas.Orientation.Vertical, Canvas.LineStyle.Light, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╵'), _style1));
    }

    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line1_DrawsHalfLine1_Vertical_AtEnd(float x)
    {
        _canvas1X1.Line(new(x, 0.5F), 0.5F, Canvas.Orientation.Vertical, Canvas.LineStyle.Light, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╷'), _style1));
    }

    [TestMethod, DataRow(Canvas.LineStyle.Light, '│'), DataRow(Canvas.LineStyle.Heavy, '┃'),
     DataRow(Canvas.LineStyle.LightDashed, '┆'), DataRow(Canvas.LineStyle.HeavyDashed, '┇'),
     DataRow(Canvas.LineStyle.Double, '║')]
    public void Line1_DrawsFullLine1_Vertical(Canvas.LineStyle style, char exp)
    {
        _canvas1X1.Line(new(0, 0), 1, Canvas.Orientation.Vertical, style, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new(exp), _style1));
    }

    [TestMethod]
    public void Line1_DoesNotDraw_IfLengthNotHalf()
    {
        _canvas1X1.Line(new(0, 0), 0.4F, Canvas.Orientation.Vertical, Canvas.LineStyle.Light, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .Item1.ShouldBe(new(0));
    }

    [TestMethod]
    public void Line1_CombinesLineRunes()
    {
        _canvas1X1.Line(new(0, 0), 1, Canvas.Orientation.Vertical, Canvas.LineStyle.Light, _style1);
        _canvas1X1.Line(new(0, 0), 1, Canvas.Orientation.Horizontal, Canvas.LineStyle.Double, _style1);

        _canvas1X1.GetContents()[0, 0]
                  .ShouldBe((new('╪'), _style1));
    }

    [TestMethod, DataRow(Canvas.Orientation.Horizontal), DataRow(Canvas.Orientation.Vertical)]
    public void Line1_Throws_IfStyleIsInvalid(Canvas.Orientation orientation)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _canvas1X1.Line(new(0, 0), 1, orientation, (Canvas.LineStyle) 100, _style1);
        });
    }

    [TestMethod]
    public void Line2_DrawsLine()
    {
        _canvas3X3.Line(new(0, 0), new(_canvas3X3.Size.Width - 0.5F, _canvas3X3.Size.Height - 0.5F), _style1);
        var contents = _canvas3X3.GetContents();

        contents[0, 0]
            .ShouldBe((new('▚'), _style1));

        contents[1, 1]
            .ShouldBe((new('▚'), _style1));

        contents[0, 0]
            .ShouldBe((new('▚'), _style1));
    }
}
