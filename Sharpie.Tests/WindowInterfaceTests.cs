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
public class WindowInterfaceTests
{
    private Mock<IWindow> _windowMock = null!;

    [TestInitialize] public void TestInitialize() { _windowMock = new(); }

    [TestMethod]
    public void Refresh_CallsActualImplementation()
    {
        _windowMock.Object.Refresh();
        _windowMock.Verify(v => v.Refresh(false, false), Times.Once);
    }

    [TestMethod]
    public void IsPointWithin_ReturnsTrue_IfPointInside()
    {
        _windowMock.Setup(s => s.Size)
                   .Returns(new Size(1, 1));
            
        _windowMock.Object.IsPointWithin(new(0, 0)).ShouldBeTrue();
        _windowMock.Verify(v => v.Size, Times.Once);
    }
    
    [TestMethod]
    public void IsPointWithin_ReturnsFalse_IfPointNotInside()
    {
        _windowMock.Setup(s => s.Size)
                   .Returns(new Size(1, 1));
            
        _windowMock.Object.IsPointWithin(new(1, 1)).ShouldBeFalse();
        _windowMock.Verify(v => v.Size, Times.Once);
    }

    [TestMethod]
    public void IsRectangleWithin_ReturnsTrue_IfInside()
    {
        _windowMock.Setup(s => s.Size)
                   .Returns(new Size(5, 5));
            
        _windowMock.Object.IsRectangleWithin(new(2, 2, 2, 2)).ShouldBeTrue();
        _windowMock.Verify(v => v.Size, Times.Exactly(2));
    }

    [TestMethod]
    public void IsRectangleWithin_ReturnsFalse_IfNotInside()
    {
        _windowMock.Setup(s => s.Size)
                   .Returns(new Size(5, 5));
            
        _windowMock.Object.IsRectangleWithin(new(2, 2, 3, 3)).ShouldBeFalse();
        _windowMock.Verify(v => v.Size, Times.Exactly(2));
    }

    [TestMethod]
    public void WriteText2_CallsCursesAlso()
    {
        _windowMock.Object.WriteText("12345");
        _windowMock.Verify(v => v.WriteText("12345", Style.Default), Times.Once);
    }

    [TestMethod]
    public void Draw1_Throws_IfDrawingIsNull()
    {
        Should.Throw<ArgumentNullException>(
            () => _windowMock.Object.Draw(new(0, 0), new(0, 0, 1, 1), null!));
    }

    [TestMethod]
    public void Draw1_CallsDrawing_DrawTo_ToDraw()
    {
        var drawingMock = new Mock<IDrawable>();

        var area = new Rectangle(1, 2, 100, 200);
        var location = new Point(10, 20);

        _windowMock.Object.Draw(location, area, drawingMock.Object);

        drawingMock.Verify(v => v.DrawTo(_windowMock.Object, area, location), Times.Once);
    }

    [TestMethod]
    public void Draw2_CallsDrawing_DrawTo_ToDraw()
    {
        var drawingMock = new Mock<IDrawable>();
        drawingMock.Setup(s => s.Size)
                   .Returns(new Size(100, 200));

        var area = new Rectangle(0, 0, 100, 200);
        var location = new Point(10, 20);

        _windowMock.Object.Draw(location, area, drawingMock.Object);

        drawingMock.Verify(v => v.DrawTo(_windowMock.Object, area, location), Times.Once);
    }

    [TestMethod]
    public void Invalidate_CallsActualImplementation()
    {
        _windowMock.Setup(s => s.Size)
                   .Returns(new Size(10, 20));
        
        _windowMock.Object.Invalidate();
        _windowMock.Verify(v => v.Invalidate(0, 20), Times.Once);
    }
}
