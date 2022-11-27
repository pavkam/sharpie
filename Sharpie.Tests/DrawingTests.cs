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
public class DrawingTests
{
    private Mock<IDrawSurface> _drawSurfaceMock = null!;
    private Drawing _drawing1X1 = null!;
    private Drawing _drawing2X2 = null!;
    private Drawing _drawing3X3 = null!;
    private readonly Style _style1 = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 } };

    private static (Rune, Style)[,] ContentsOf(Drawing drawing)
    {
        var mock = new Mock<IDrawSurface>();
        mock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
             .Returns(true);

        var collector = new (Rune, Style)[drawing.Size.Width, drawing.Size.Height];
        mock.Setup(s => s.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()))
             .Callback<Point, Rune, Style>((location, rune, textStyle) =>
             {
                 collector[location.X, location.Y] = (rune, textStyle);
             });

        drawing.DrawTo(mock.Object, new(0, 0, drawing.Size.Width, drawing.Size.Height), new(0, 0));
        return collector;
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _drawSurfaceMock = new();
        _drawing1X1 = new(new(1, 1));
        _drawing2X2 = new(new(2, 2));
        _drawing3X3 = new(new(3, 3));
    }
    
    [TestMethod, 
     DataRow(0, 1),
     DataRow(1, 0),SuppressMessage("ReSharper", "ObjectCreationAsStatement")
    ]
    public void Ctor_Throws_IfSizeIsInvalid(int width, int height)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            new Drawing(new(width, height));
        });
    }

    [TestMethod]
    public void Ctor_InitializesTheSize()
    {
        var size = new Size(10, 20);
        var d = new Drawing(size);
        d.Size.ShouldBe(size);
    }

    [TestMethod]
    public void DrawTo_Throws_IfTheDestinationINull()
    {
        Should.Throw<ArgumentNullException>(() =>
        {
            _drawing1X1.DrawTo(null!, Rectangle.Empty, Point.Empty);
        });
    }
    
    [TestMethod]
    public void DrawTo_DoesNothing_IfSrcAreaIsEmpty()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(true);
        
        _drawing1X1.DrawTo(_drawSurfaceMock.Object, Rectangle.Empty, Point.Empty);
        
        _drawSurfaceMock.Verify(v => v.CoversArea(It.IsAny<Rectangle>()), Times.Never);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()), Times.Never);
    }
    
    [TestMethod]
    public void DrawTo_Throws_IfSrcAreaIsInvalid_2()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(true);
        
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.DrawTo(_drawSurfaceMock.Object, new(1, 1, 2, 2), Point.Empty);
        });
    }
    
    [TestMethod]
    public void DrawTo_Throws_IfDestLocationIsInvalid()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(false);
        
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.DrawTo(_drawSurfaceMock.Object, new(0, 0, 1, 2), new(3, 4));
        });
        
        _drawSurfaceMock.Verify(v => v.CoversArea(new (3, 4, 1, 2)), Times.Once);
    }

    [TestMethod]
    public void DrawTo_DrawsOntoSurface()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(true);

        _drawing1X1.Glyph(new(0, 0), new('A'), _style1);
        _drawing1X1.DrawTo(_drawSurfaceMock.Object, new(0, 0, 1, 1), new(0, 0));
        
        _drawSurfaceMock.Verify(v => v.CoversArea(new(0, 0, 1, 1)), Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(new(0, 0), new('A'), _style1), Times.Once);
    }
    
    [TestMethod]
    public void DrawTo_SkipsEmptyCells()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(true);

        _drawing2X2.Glyph(new(1, 1), new('A'), _style1);

        _drawing2X2.DrawTo(_drawSurfaceMock.Object, new(0, 0, 2, 2), new(0, 0));
        _drawSurfaceMock.Verify(v => v.DrawCell(new(1, 1), new('A'), _style1), Times.Once);
        _drawSurfaceMock.Verify(v => v.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()), Times.Once);
    }
    
    [TestMethod]
    public void Glyph1_Throws_IfLocationIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Glyph(new(1, 1), new('A'), _style1);
        });
    }

    [TestMethod]
    public void Glyph1_DrawsGlyph()
    {
        _drawing1X1.Glyph(new(0, 0), new('Z'), _style1);
        ContentsOf(_drawing1X1)[0, 0]
            .ShouldBe((new('Z'), _style1));
    }

    [TestMethod]
    public void Glyph1_ReplacesSpecialChars_WithWhitespace()
    {
        for (var ch = 0; ch <= ControlCharacter.Escape; ch++)
        {
            _drawing1X1.Glyph(new(0, 0), new(ch), _style1);
            ContentsOf(_drawing1X1)[0, 0]
                .ShouldBe((new(' '), _style1));
        }
    }
    
    [TestMethod]
    public void Glyph2_Throws_IfStyleIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _drawing1X1.Glyph(new(1, 1), (Drawing.CheckGlyphStyle)100, Drawing.FillStyle.Black, _style1);
        });
    }

    [TestMethod]
    public void Glyph2_DrawsGlyph()
    {
        _drawing1X1.Glyph(new(0, 0), Drawing.CheckGlyphStyle.Diamond, Drawing.FillStyle.Black, _style1);
        ContentsOf(_drawing1X1)[0, 0]
            .ShouldBe((new('◇'), _style1));
    }

    [TestMethod]
    public void Glyph3_Throws_IfStyleIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _drawing1X1.Glyph(new(1, 1), (Drawing.TriangleGlyphStyle)100, Drawing.GlyphSize.Normal, Drawing.FillStyle.Black, _style1);
        });
    }

    [TestMethod]
    public void Glyph3_DrawsGlyph()
    {
        _drawing1X1.Glyph(new(0, 0), Drawing.TriangleGlyphStyle.Down, Drawing.GlyphSize.Normal, Drawing.FillStyle.Black, _style1);
        ContentsOf(_drawing1X1)[0, 0]
            .ShouldBe((new('▽'), _style1));
    }
    
    [TestMethod, DataRow(-1), DataRow(9)]
    public void Glyph4_Throws_IfFillIsInvalid(int fill)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Glyph(new(1, 1), Drawing.GradientGlyphStyle.LeftToRight, fill, _style1);
        });
    }
    
    [TestMethod]
    public void Glyph4_DrawsGlyph()
    {
        _drawing1X1.Glyph(new(0, 0), Drawing.GradientGlyphStyle.LeftToRight, 8, _style1);
        ContentsOf(_drawing1X1)[0, 0]
            .ShouldBe((new('█'), _style1));
    }

    [TestMethod, 
     DataRow(-1F, 0F), 
     DataRow(2F, 0F), 
     DataRow(2.1F, 0F), 
     DataRow(0F, -1F), 
     DataRow(0F, 2F),
     DataRow(0F, 2.1F)
    ]
    public void Validate1_Throws_IfLocationInvalid(float x, float y)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { _drawing2X2.Validate(new PointF(x, y)); });
    }
    
    [TestMethod, 
     DataRow(-1F, 0F, 1F, 1F), 
     DataRow(0F, -1F, 1F, 1F), 
     DataRow(0F, 0F, -1F, 1F), 
     DataRow(0F, 0F, 1F, -1F), 
     DataRow(2F, 0F, 1F, 1F), 
     DataRow(0F, 2F, 1F, 1F), 
     DataRow(0F, 0F, 3F, 1F), 
     DataRow(0F, 0F, 1F, 3F),
     DataRow(1F, 1F, 2F, 2F),
     DataRow(0F, 0F, 1.4F, 2.1F),
     DataRow(0F, 0F, 2.1F, 1.9F),
     DataRow(1F, 1F, 1F, 1.1F),
     DataRow(1F, 1F, 1.1F, 1F),
    ]
    public void Validate2_Throws_IfAreaInvalid(float x, float y, float w, float h)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => { _drawing2X2.Validate(new RectangleF(x, y, w, h)); });
    }
    
    [TestMethod]
    public void Fill1_Throws_IfAreaIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.Fill(new(0, 0, 2, 3), new Rune('A'), _style1);
        });
    }
    
    [TestMethod]
    public void Fill1_FillsArea()
    {
        _drawing2X2.Fill(new(0, 0, 2, 1), new Rune('A'), _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('A'), _style1));
        c[1, 0]
            .ShouldBe((new('A'), _style1));
        c[0, 1].Item1.ShouldBe(new(0));
        c[1, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Fill1_FillsNothing_IfAreaIsEmpty()
    {
        _drawing2X2.Fill(new(0, 0, 2, 0), new Rune('A'), _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0].Item1.ShouldBe(new(0));
        c[1, 0].Item1.ShouldBe(new(0));
        c[0, 1].Item1.ShouldBe(new(0));
        c[1, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Fill2_Throws_IfShadeIsInvalid()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _drawing2X2.Fill(new(0, 0, 2, 3), (Drawing.ShadeGlyphStyle)100, _style1);
        });
    }
    
    [TestMethod]
    public void Fill2_FillsArea()
    {
        _drawing2X2.Fill(new(0, 0, 2, 1), Drawing.ShadeGlyphStyle.Dark, _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('▓'), _style1));
        c[1, 0]
            .ShouldBe((new('▓'), _style1));
        c[0, 1].Item1.ShouldBe(new(0));
        c[1, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Text_Throws_IfLocationIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.Text(new(0, 2), "text", _style1);
        });
    }
    
    [TestMethod]
    public void Text_Throws_IfTextIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
        {
            _drawing2X2.Text(new(0, 0), null!, _style1);
        });
    }
    
    [TestMethod]
    public void Text_DoesNothingIfTextIsEmpty()
    {
        _drawing1X1.Text(new(0, 0), "", _style1);
        ContentsOf(_drawing1X1)[0,0].Item1.ShouldBe(new(0));
    }


    [TestMethod]
    public void Text_DrawsEmoji()
    {
        const string emoji = "❤️";
        Rune.TryGetRuneAt(emoji, 0, out var rune);
            
        _drawing1X1.Text(new(0, 0), emoji, _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((rune, _style1));
    }
    
    [TestMethod]
    public void Text_TextThatFits_Horizontal()
    {
        _drawing2X2.Text(new(0, 0), "text", _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('t'), _style1));
        c[1, 0]
            .ShouldBe((new('e'), _style1));
        c[0, 1].Item1.ShouldBe(new(0));
        c[0, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Text_TextThatFits_Vertical()
    {
        _drawing2X2.Text(new(0, 0), "text", _style1, Drawing.Orientation.Vertical);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('t'), _style1));
        c[0, 1]
            .ShouldBe((new('e'), _style1));
        c[1, 0].Item1.ShouldBe(new(0));
        c[1, 0].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Point_Throws_IfLocationIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.Point(new(0F, 2F), _style1);
        });
    }

    [TestMethod, 
     DataRow(0.4F, 0.4F, '▘'),
     DataRow(0.5F, 0.4F, '▝'),
     DataRow(0.4F, 0.5F, '▖'),
     DataRow(0.5F, 0.5F, '▗'),
    ]
    public void Point_DrawsSinglePoint(float x, float y, char c)
    {
        _drawing1X1.Point(new(x, y), _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new(c), _style1));
    }
    
    [TestMethod]
    public void Point_CombinesPoints()
    {
        _drawing1X1.Point(new(0, 0), _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('▘'), _style1));
        
        _drawing1X1.Point(new(0.6F, 0), _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('▀'), _style1));
        
        _drawing1X1.Point(new(0.6F, 0.9F), _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('▜'), _style1));
        
        _drawing1X1.Point(new(0.2F, 0.5F), _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('█'), _style1));
    }
    
    [TestMethod]
    public void Rectangle_Throws_IfAreaIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.Rectangle(new(0, 0, 1.9F, 2.1F), _style1);
        });
    }

    [TestMethod]
    public void Rectangle_DrawsRectangle()
    {
        _drawing2X2.Rectangle(new(0.5F, 0.5F, 1.2F, 1.4F), _style1);
        var c = ContentsOf(_drawing2X2);
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
    public void Box_Throws_IfAreaIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing2X2.Box(new(0, 1, 1, 2), Drawing.LineStyle.Double, _style1);
        });
    }
    
    [TestMethod]
    public void Box_DrawsCross_When1X1()
    {
        _drawing1X1.Box(new(0, 0, 1, 1), Drawing.LineStyle.Double, _style1);
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╔'), _style1));
    }
      
    
    [TestMethod]
    public void Box_DrawsBox_When2X1()
    {
        _drawing2X2.Box(new(0, 0, 2, 1), Drawing.LineStyle.Double, _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('╔'), _style1));
        c[1, 0]
            .ShouldBe((new('╗'), _style1));
        c[0, 1].Item1.ShouldBe(new(0));
        c[1, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Box_DrawsBox_When1X2()
    {
        _drawing2X2.Box(new(0, 0, 1, 2), Drawing.LineStyle.Double, _style1);
        var c = ContentsOf(_drawing2X2);
        c[0, 0]
            .ShouldBe((new('╔'), _style1));
        c[1, 0].Item1.ShouldBe(new(0));
        c[0, 1]
            .ShouldBe((new('╚'), _style1));
        c[1, 1].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Box_DrawsBox_When2X2()
    {
        _drawing2X2.Box(new(0, 0, 2, 2), Drawing.LineStyle.Double, _style1);
        var c = ContentsOf(_drawing2X2);
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
        _drawing3X3.Box(new(0, 0, 3, 3), Drawing.LineStyle.Double, _style1);
        var c = ContentsOf(_drawing3X3);
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
    public void Line_Throws_IfLocationIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Line(new(0, 1), 1, Drawing.Orientation.Horizontal, Drawing.LineStyle.Double, _style1);
        });
    }
    
    [TestMethod]
    public void Line_Throws_IfLengthIsInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Line(new(0, 0), -1, Drawing.Orientation.Horizontal, Drawing.LineStyle.Double, _style1);
        });
    }
    
    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line_DrawsHalfLine_Horizontal_AtStart(float y)
    {
        _drawing1X1.Line(new(0, y), 0.5F, 
            Drawing.Orientation.Horizontal, 
            Drawing.LineStyle.Light, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╴'), _style1));
    }
    
    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line_DrawsHalfLine_Horizontal_AtEnd(float y)
    {
        _drawing1X1.Line(new(0.5F, y), 0.5F, 
            Drawing.Orientation.Horizontal, 
            Drawing.LineStyle.Light, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╶'), _style1));
    }
    
    [TestMethod, 
     DataRow(Drawing.LineStyle.Light, '─'),
     DataRow(Drawing.LineStyle.Heavy, '━'),
     DataRow(Drawing.LineStyle.LightDashed, '┄'),
     DataRow(Drawing.LineStyle.HeavyDashed, '┅'),
     DataRow(Drawing.LineStyle.Double, '═'),
    ]
    public void Line_DrawsFullLine_Horizontal(Drawing.LineStyle style, char exp)
    {
        _drawing1X1.Line(new(0, 0), 1, 
            Drawing.Orientation.Horizontal, 
            style, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new(exp), _style1));
    }
    
    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line_DrawsHalfLine_Vertical_AtStart(float x)
    {
        _drawing1X1.Line(new(x, 0), 0.5F, 
            Drawing.Orientation.Vertical, 
            Drawing.LineStyle.Light, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╵'), _style1));
    }
    
    [TestMethod, DataRow(0F), DataRow(0.5F), DataRow(0.9F)]
    public void Line_DrawsHalfLine_Vertical_AtEnd(float x)
    {
        _drawing1X1.Line(new(x, 0.5F), 0.5F, 
            Drawing.Orientation.Vertical, 
            Drawing.LineStyle.Light, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╷'), _style1));
    }
    
    [TestMethod, 
     DataRow(Drawing.LineStyle.Light, '│'),
     DataRow(Drawing.LineStyle.Heavy, '┃'),
     DataRow(Drawing.LineStyle.LightDashed, '┆'),
     DataRow(Drawing.LineStyle.HeavyDashed, '┇'),
     DataRow(Drawing.LineStyle.Double, '║'),
    ]
    public void Line_DrawsFullLine_Vertical(Drawing.LineStyle style, char exp)
    {
        _drawing1X1.Line(new(0, 0), 1, 
            Drawing.Orientation.Vertical, 
            style, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new(exp), _style1));
    }
    
    [TestMethod]
    public void Line_Throws_IfLengthLongerThanArea_Y()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Line(new(0, 0), 1.5F, Drawing.Orientation.Vertical, 
                Drawing.LineStyle.Light, _style1);
        });
    }
    
    [TestMethod]
    public void Line_Throws_IfLengthLongerThanArea_X()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Line(new(0, 0), 1.5F, Drawing.Orientation.Horizontal, 
                Drawing.LineStyle.Light, _style1);
        });
    }
    
    [TestMethod]
    public void Line_DoesNotDraw_IfLengthNotHalf()
    {
        _drawing1X1.Line(new(0, 0), 0.4F, 
            Drawing.Orientation.Vertical, 
            Drawing.LineStyle.Light, _style1);
        
        ContentsOf(_drawing1X1)[0,0].Item1.ShouldBe(new(0));
    }
    
    [TestMethod]
    public void Line_CombinesLineRunes()
    {
        _drawing1X1.Line(new(0, 0), 1, 
            Drawing.Orientation.Vertical, 
            Drawing.LineStyle.Light, _style1);
        _drawing1X1.Line(new(0, 0), 1, 
            Drawing.Orientation.Horizontal, 
            Drawing.LineStyle.Double, _style1);
        
        ContentsOf(_drawing1X1)[0,0].ShouldBe((new('╪'), _style1));
    }
    
    [TestMethod, DataRow(Drawing.Orientation.Horizontal), DataRow(Drawing.Orientation.Vertical)]
    public void Line_Throws_IfStyleIsInvalid(Drawing.Orientation orientation)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.Line(new(0, 0), 1, orientation, 
                (Drawing.LineStyle)100, _style1);
        });
    }
    
}
