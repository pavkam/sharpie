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
    private readonly Style _style1 = new() { Attributes = VideoAttribute.Bold, ColorMixture = new() { Handle = 99 } };
    
    [TestInitialize]
    public void TestInitialize()
    {
        _drawSurfaceMock = new();
        _drawing1X1 = new(new(1, 1));
        _drawing2X2 = new(new(2, 2));
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
    public void DrawTo_Throws_IfSrcAreaIsInvalid_1()
    {
        _drawSurfaceMock.Setup(s => s.CoversArea(It.IsAny<Rectangle>()))
                        .Returns(true);
        
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            _drawing1X1.DrawTo(_drawSurfaceMock.Object, Rectangle.Empty, Point.Empty);
        });
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
}
