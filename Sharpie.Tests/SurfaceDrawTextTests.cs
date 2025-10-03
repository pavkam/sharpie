/*
Copyright (c) 2022-2025, Alexandru Ciobanu
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
public class SurfaceDrawTextTests
{
    private readonly Style _style = new()
    {
        ColorMixture = new()
        {
            Handle = 11
        },
        Attributes = VideoAttribute.Bold
    };

    private Mock<ICursesBackend> _cursesMock = null!;
    private Mock<IAsciiFont> _font2X2 = null!;
    private Mock<IAsciiFont> _font6X6 = null!;
    private Mock<IDrawable> _glyph2X2 = null!;
    private Mock<IDrawable> _glyph6X6 = null!;
    private Surface _sf10X5 = null!;

    private void MockCaretAt(ISurface sf, int x, int y)
    {
        _ = _cursesMock.Setup(s => s.getcurx(sf.Handle))
                   .Returns(x);

        _ = _cursesMock.Setup(s => s.getcury(sf.Handle))
                   .Returns(y);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _sf10X5 = new(_cursesMock.Object, new(1));
        _cursesMock.MockArea(_sf10X5, new Size(10, 5));

        _glyph2X2 = new();
        _ = _glyph2X2.Setup(s => s.Size)
                 .Returns(new Size(2, 2));

        _font2X2 = new();
        _ = _font2X2.Setup(s => s.Height)
                .Returns(2);

        _ = _font2X2.Setup(s => s.GetGlyph(It.IsAny<Rune>(), It.IsAny<Style>()))
                .Returns(_glyph2X2.Object);

        _glyph6X6 = new();
        _ = _glyph6X6.Setup(s => s.Size)
                 .Returns(new Size(6, 6));

        _font6X6 = new();
        _ = _font6X6.Setup(s => s.Height)
                .Returns(6);

        _ = _font6X6.Setup(s => s.GetGlyph(It.IsAny<Rune>(), It.IsAny<Style>()))
                .Returns(_glyph6X6.Object);
    }

    [TestMethod]
    public void DrawText1_Throws_IfFontIsNull() =>
        Should.Throw<ArgumentNullException>(() => _sf10X5.DrawText(null!, "text", Style.Default));

    [TestMethod]
    public void DrawText1_Throws_IfFontIsStringIsNull() =>
        Should.Throw<ArgumentNullException>(() => _sf10X5.DrawText(_font2X2.Object, null!, Style.Default));

    [TestMethod]
    public void DrawText1_DoesNothing_IfTextIsEmpty()
    {
        _sf10X5.DrawText(_font2X2.Object, string.Empty, Style.Default);

        _font2X2.VerifyNoOtherCalls();
        _glyph2X2.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void DrawText1_AsksFontForGlyphs()
    {
        _sf10X5.DrawText(_font2X2.Object, "ABC", _style);

        _font2X2.Verify(v => v.GetGlyph(new('A'), _style), Times.Once);
        _font2X2.Verify(v => v.GetGlyph(new('B'), _style), Times.Once);
        _font2X2.Verify(v => v.GetGlyph(new('C'), _style), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_WhenEnoughSpace_DrawsOnGlyphsAtExpectedLocation()
    {
        _sf10X5.DrawText(_font2X2.Object, "ABC", Style.Default);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(2, 0)), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_WhenNotEnoughSpaceOnXAndNotWrapping_DoesNotDrawGlyphs()
    {
        MockCaretAt(_sf10X5, 9, 0);
        _sf10X5.DrawText(_font2X2.Object, "A", Style.Default, false, false);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, It.IsAny<Rectangle>(), It.IsAny<Point>()), Times.Never);
    }

    [TestMethod]
    public void DrawText1_Ltr_WhenNotEnoughSpaceOnXAndWrapping_MovesToNextLine()
    {
        MockCaretAt(_sf10X5, 7, 0);
        _sf10X5.DrawText(_font2X2.Object, "AB", Style.Default);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(7, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 2)), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_WhenEnoughSpaceOnXForOneCharOnlyAndNotWrapping_DrawsOnlyOne()
    {
        MockCaretAt(_sf10X5, 8, 0);
        _sf10X5.DrawText(_font2X2.Object, "ABC", Style.Default, false, false);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(8, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, It.IsAny<Rectangle>(), It.IsAny<Point>()), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_LeavesTheCaretAfterTheDrawnGlyph_IfFits()
    {
        MockCaretAt(_sf10X5, 7, 0);
        _sf10X5.DrawText(_font2X2.Object, "A", Style.Default);

        _cursesMock.Verify(v => v.wmove(_sf10X5.Handle, 0, 9), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_LeavesTheCaretAtEndOfDrawnGlyph_IfNotFits()
    {
        MockCaretAt(_sf10X5, 8, 0);
        _sf10X5.DrawText(_font2X2.Object, "A", Style.Default);

        _cursesMock.Verify(v => v.wmove(_sf10X5.Handle, 0, 9), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_ProcessesNewLine_IfAsked()
    {
        MockCaretAt(_sf10X5, 7, 0);
        _sf10X5.DrawText(_font2X2.Object, "A\nB", Style.Default);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(7, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 2)), Times.Once);

        _cursesMock.Verify(v => v.wmove(_sf10X5.Handle, 2, 2), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_DoesNotProcessNewLine_IfAsked()
    {
        MockCaretAt(_sf10X5, 0, 0);
        _sf10X5.DrawText(_font2X2.Object, "A\nB", _style, false);

        _font2X2.Verify(v => v.GetGlyph(new('\n'), _style), Times.Once);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(2, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(4, 0)), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_ScrollsUp_IfWrappingAndScrollable()
    {
        _ = _cursesMock.Setup(s => s.is_scrollok(_sf10X5.Handle))
                   .Returns(true);

        MockCaretAt(_sf10X5, 8, 2);
        _sf10X5.DrawText(_font2X2.Object, "A\nB", Style.Default);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(8, 2)), Times.Once);
        _cursesMock.Verify(s => s.wscrl(_sf10X5.Handle, 1), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 3)), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_ScrollsUpToZero_IfScrollableAndNotEnoughSpaceForHeight()
    {
        _ = _cursesMock.Setup(s => s.is_scrollok(_sf10X5.Handle))
                   .Returns(true);

        MockCaretAt(_sf10X5, 0, 2);
        _sf10X5.DrawText(_font6X6.Object, "A", Style.Default);

        _cursesMock.Verify(s => s.wscrl(_sf10X5.Handle, 3), Times.Once);
        _glyph6X6.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 6, 6), new(0, 0)), Times.Once);

        _cursesMock.Verify(s => s.wmove(_sf10X5.Handle, 0, 6), Times.Once);
    }

    [TestMethod]
    public void DrawText1_Ltr_DrawsFromTopSmallerArea_IfNotScrollableAndNotEnoughSpaceForHeight()
    {
        _ = _cursesMock.Setup(s => s.is_scrollok(_sf10X5.Handle))
                   .Returns(false);

        MockCaretAt(_sf10X5, 0, 2);
        _sf10X5.DrawText(_font6X6.Object, "A", Style.Default);

        _cursesMock.Verify(s => s.wscrl(_sf10X5.Handle, It.IsAny<int>()), Times.Never);
        _glyph6X6.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 6, 6), new(0, 0)), Times.Once);

        _cursesMock.Verify(s => s.wmove(_sf10X5.Handle, 0, 6), Times.Once);
    }

    [TestMethod]
    public void DrawText2_Throws_IfFontIsNull() =>
        Should.Throw<ArgumentNullException>(() => _sf10X5.DrawText(null!, "text"));

    [TestMethod]
    public void DrawText2_Throws_IfFontIsStringIsNull() =>
        Should.Throw<ArgumentNullException>(() => _sf10X5.DrawText(_font2X2.Object, null!));

    [TestMethod]
    public void DrawText2_DoesNothing_IfTextIsEmpty()
    {
        _sf10X5.DrawText(_font2X2.Object, string.Empty, Style.Default);

        _font2X2.VerifyNoOtherCalls();
        _glyph2X2.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void DrawText2_CallsDrawText1()
    {
        MockCaretAt(_sf10X5, 0, 0);
        _sf10X5.DrawText(_font2X2.Object, "A\nB", Style.Default, false);

        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(0, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(2, 0)), Times.Once);
        _glyph2X2.Verify(v => v.DrawOnto(_sf10X5, new(0, 0, 2, 2), new(4, 0)), Times.Once);
    }

    private sealed class Surface(ICursesBackend curses, IntPtr handle): Sharpie.Surface(curses, handle)
    {
        protected internal override void AssertSynchronized()
        {
        }
    }
}
