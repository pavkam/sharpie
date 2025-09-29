/*
Copyright (c) 2022-2025, Alexandru Ciobanu, Jordan Hemming
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
public class CursesMouseEventParserTests
{
    [TestMethod]
    public void Get_ReturnsObject_ForAbi1()
    {
        var r = CursesMouseEventParser.Get(CursesAbiVersion.NCurses5);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(8u << 24);
        r.All.ShouldBe((8u << 24) - 1);
    }

    [TestMethod]
    public void Get_ReturnsObject_ForAbi1_OldApi()
    {
        var r = CursesMouseEventParser.Get(1);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(8u << 24);
        r.All.ShouldBe((8u << 24) - 1);
    }

    [TestMethod]
    public void Get_ReturnsObject_ForAbi2()
    {
        var r = CursesMouseEventParser.Get(CursesAbiVersion.NCurses6);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(8u << 25);
        r.All.ShouldBe((8u << 25) - 1);
    }

    [TestMethod]
    public void Get_ReturnsObject_ForAbi2_OldApi()
    {
        var r = CursesMouseEventParser.Get(2);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(8u << 25);
        r.All.ShouldBe((8u << 25) - 1);
    }

    [TestMethod]
    public void Get_ReturnsObject_ForPdCursesAbi()
    {
        var r = CursesMouseEventParser.Get(CursesAbiVersion.PdCurses);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(1u << 29);
        r.All.ShouldBe((1u << 29) - 1);
    }

    [TestMethod]
    public void Get_ReturnsObject_ForPdCursesAbi_OldApi()
    {
        var r = CursesMouseEventParser.Get(3);
        r.ShouldNotBeNull();

        r.ReportPosition.ShouldBe(1u << 29);
        r.All.ShouldBe((1u << 29) - 1);
    }

    [TestMethod]
    public void Get_ReturnsAbi1_ForUnknownAbi()
    {
        CursesMouseEventParser.Get(CursesAbiVersion.Unknown)
                              .ShouldBe(CursesMouseEventParser.Get(CursesAbiVersion.Unknown));
    }

    [TestMethod, DataRow(1u << ((1 - 1) * 6), MouseButton.Button1, MouseButtonState.Released),
     DataRow(2u << ((1 - 1) * 6), MouseButton.Button1, MouseButtonState.Pressed),
     DataRow(4u << ((1 - 1) * 6), MouseButton.Button1, MouseButtonState.Clicked),
     DataRow(8u << ((1 - 1) * 6), MouseButton.Button1, MouseButtonState.DoubleClicked),
     DataRow(16u << ((1 - 1) * 6), MouseButton.Button1, MouseButtonState.TripleClicked),
     DataRow(1u << ((2 - 1) * 6), MouseButton.Button2, MouseButtonState.Released),
     DataRow(2u << ((2 - 1) * 6), MouseButton.Button2, MouseButtonState.Pressed),
     DataRow(4u << ((2 - 1) * 6), MouseButton.Button2, MouseButtonState.Clicked),
     DataRow(8u << ((2 - 1) * 6), MouseButton.Button2, MouseButtonState.DoubleClicked),
     DataRow(16u << ((2 - 1) * 6), MouseButton.Button2, MouseButtonState.TripleClicked),
     DataRow(1u << ((3 - 1) * 6), MouseButton.Button3, MouseButtonState.Released),
     DataRow(2u << ((3 - 1) * 6), MouseButton.Button3, MouseButtonState.Pressed),
     DataRow(4u << ((3 - 1) * 6), MouseButton.Button3, MouseButtonState.Clicked),
     DataRow(8u << ((3 - 1) * 6), MouseButton.Button3, MouseButtonState.DoubleClicked),
     DataRow(16u << ((3 - 1) * 6), MouseButton.Button3, MouseButtonState.TripleClicked),
     DataRow(1u << ((4 - 1) * 6), MouseButton.Button4, MouseButtonState.Released),
     DataRow(2u << ((4 - 1) * 6), MouseButton.Button4, MouseButtonState.Pressed),
     DataRow(4u << ((4 - 1) * 6), MouseButton.Button4, MouseButtonState.Clicked),
     DataRow(8u << ((4 - 1) * 6), MouseButton.Button4, MouseButtonState.DoubleClicked),
     DataRow(16u << ((4 - 1) * 6), MouseButton.Button4, MouseButtonState.TripleClicked)]
    public void Parse_ParsesTheButtonAndState_ForAbi1(uint raw, MouseButton expButton, MouseButtonState expState)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses5);
        var p = parser.Parse(raw);

        p.ShouldBe((expButton, expState, ModifierKey.None));
    }

    [TestMethod, DataRow(1u << ((1 - 1) * 5), MouseButton.Button1, MouseButtonState.Released),
     DataRow(2u << ((1 - 1) * 5), MouseButton.Button1, MouseButtonState.Pressed),
     DataRow(4u << ((1 - 1) * 5), MouseButton.Button1, MouseButtonState.Clicked),
     DataRow(8u << ((1 - 1) * 5), MouseButton.Button1, MouseButtonState.DoubleClicked),
     DataRow(16u << ((1 - 1) * 5), MouseButton.Button1, MouseButtonState.TripleClicked),
     DataRow(1u << ((2 - 1) * 5), MouseButton.Button2, MouseButtonState.Released),
     DataRow(2u << ((2 - 1) * 5), MouseButton.Button2, MouseButtonState.Pressed),
     DataRow(4u << ((2 - 1) * 5), MouseButton.Button2, MouseButtonState.Clicked),
     DataRow(8u << ((2 - 1) * 5), MouseButton.Button2, MouseButtonState.DoubleClicked),
     DataRow(16u << ((2 - 1) * 5), MouseButton.Button2, MouseButtonState.TripleClicked),
     DataRow(1u << ((3 - 1) * 5), MouseButton.Button3, MouseButtonState.Released),
     DataRow(2u << ((3 - 1) * 5), MouseButton.Button3, MouseButtonState.Pressed),
     DataRow(4u << ((3 - 1) * 5), MouseButton.Button3, MouseButtonState.Clicked),
     DataRow(8u << ((3 - 1) * 5), MouseButton.Button3, MouseButtonState.DoubleClicked),
     DataRow(16u << ((3 - 1) * 5), MouseButton.Button3, MouseButtonState.TripleClicked),
     DataRow(1u << ((4 - 1) * 5), MouseButton.Button4, MouseButtonState.Released),
     DataRow(2u << ((4 - 1) * 5), MouseButton.Button4, MouseButtonState.Pressed),
     DataRow(4u << ((4 - 1) * 5), MouseButton.Button4, MouseButtonState.Clicked),
     DataRow(8u << ((4 - 1) * 5), MouseButton.Button4, MouseButtonState.DoubleClicked),
     DataRow(16u << ((4 - 1) * 5), MouseButton.Button4, MouseButtonState.TripleClicked),
     DataRow(1u << ((5 - 1) * 5), MouseButton.Button5, MouseButtonState.Released),
     DataRow(2u << ((5 - 1) * 5), MouseButton.Button5, MouseButtonState.Pressed),
     DataRow(4u << ((5 - 1) * 5), MouseButton.Button5, MouseButtonState.Clicked),
     DataRow(8u << ((5 - 1) * 5), MouseButton.Button5, MouseButtonState.DoubleClicked),
     DataRow(16u << ((5 - 1) * 5), MouseButton.Button5, MouseButtonState.TripleClicked)]
    public void Parse_ParsesTheButtonAndState_ForAbi2(uint raw, MouseButton expButton, MouseButtonState expState)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses6);
        var p = parser.Parse(raw);

        p.ShouldBe((expButton, expState, ModifierKey.None));
    }

    [TestMethod, DataRow(0u, ModifierKey.None), DataRow(1u << 24, ModifierKey.Ctrl),
     DataRow(2u << 24, ModifierKey.Shift), DataRow(4u << 24, ModifierKey.Alt),
     DataRow((4u << 24) | (1u << 24), ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((2u << 24) | (1u << 24), ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow((4u << 24) | (2u << 24), ModifierKey.Alt | ModifierKey.Shift),
     DataRow((1u << 24) | (2u << 24), ModifierKey.Ctrl | ModifierKey.Shift),
     DataRow((4u << 24) | (1u << 24) | (2u << 24), ModifierKey.Alt | ModifierKey.Shift | ModifierKey.Ctrl)]
    public void Parse_ParsesTheModifiers_ForAbi1(uint raw, ModifierKey expMod)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses5);
        var p = parser.Parse(raw | 1u);

        p.ShouldBe((MouseButton.Button1, MouseButtonState.Released, expMod));
    }

    [TestMethod, DataRow(0u, ModifierKey.None), DataRow(1u << 25, ModifierKey.Ctrl),
     DataRow(2u << 25, ModifierKey.Shift), DataRow(4u << 25, ModifierKey.Alt),
     DataRow((4u << 25) | (1u << 25), ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((2u << 25) | (1u << 25), ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow((4u << 25) | (2u << 25), ModifierKey.Alt | ModifierKey.Shift),
     DataRow((1u << 25) | (2u << 25), ModifierKey.Ctrl | ModifierKey.Shift),
     DataRow((4u << 25) | (1u << 25) | (2u << 25), ModifierKey.Alt | ModifierKey.Shift | ModifierKey.Ctrl)]
    public void Parse_ParsesTheModifiers_ForAbi2(uint raw, ModifierKey expMod)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses6);
        var p = parser.Parse(raw | 1u);

        p.ShouldBe((MouseButton.Button1, MouseButtonState.Released, expMod));
    }

    [TestMethod, DataRow(0u, ModifierKey.None), DataRow(2u << 26, ModifierKey.Ctrl),
     DataRow(1u << 26, ModifierKey.Shift), DataRow(4u << 26, ModifierKey.Alt),
     DataRow((4u << 26) | (2u << 26), ModifierKey.Alt | ModifierKey.Ctrl),
     DataRow((1u << 26) | (2u << 26), ModifierKey.Shift | ModifierKey.Ctrl),
     DataRow((4u << 26) | (1u << 26), ModifierKey.Alt | ModifierKey.Shift),
     DataRow((2u << 26) | (1u << 26), ModifierKey.Ctrl | ModifierKey.Shift),
     DataRow((4u << 26) | (1u << 26) | (2u << 26), ModifierKey.Alt | ModifierKey.Shift | ModifierKey.Ctrl)]
    public void Parse_ParsesTheModifiers_ForPdCursesAbi(uint raw, ModifierKey expMod)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.PdCurses);
        var p = parser.Parse(raw | 1u);

        p.ShouldBe((MouseButton.Button1, MouseButtonState.Released, expMod));
    }

    [TestMethod, DataRow(0u), DataRow(8u << 24), DataRow(1u << 24)]
    public void Parse_ReturnsNullIfNoButtonPresent_ForAbi1(uint raw)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses5);
        var p = parser.Parse(raw);

        p.ShouldBeNull();
    }

    [TestMethod, DataRow(0u), DataRow(8u << 25), DataRow(1u << 25)]
    public void Parse_ReturnsNullIfNoButtonPresent_ForAbi2(uint raw)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses6);
        var p = parser.Parse(raw);

        p.ShouldBeNull();
    }

    [TestMethod, DataRow(0u), DataRow(8u << 26), DataRow(1u << 26)]
    public void Parse_ReturnsNullIfNoButtonPresent_ForPdCursesAbi(uint raw)
    {
        var parser = CursesMouseEventParser.Get(CursesAbiVersion.NCurses6);
        var p = parser.Parse(raw);

        p.ShouldBeNull();
    }
}
