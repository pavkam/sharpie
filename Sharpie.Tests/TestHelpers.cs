/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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

using System.Diagnostics;
using System.Linq.Expressions;

internal static class TestHelpers
{
    public static (Rune, Style)[,] GetContents(this IDrawable drawing)
    {
        var mock = new Mock<IDrawSurface>();
        _ = mock.Setup(s => s.Size)
            .Returns(drawing.Size);

        var collector = new (Rune, Style)[drawing.Size.Width, drawing.Size.Height];
        _ = mock.Setup(s => s.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()))
            .Callback<Point, Rune, Style>((location, rune, textStyle) =>
            {
                collector[location.X, location.Y] = (rune, textStyle);
            });

        drawing.DrawOnto(mock.Object, new(0, 0, drawing.Size.Width, drawing.Size.Height), new(0, 0));
        return collector;
    }

    public static void MockArea(this Mock<ICursesBackend> cursesMock, ISurface surface, Size size)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        _ = cursesMock.Setup(s => s.getmaxx(surface.Handle))
                  .Returns(size.Width);

        _ = cursesMock.Setup(s => s.getmaxy(surface.Handle))
                  .Returns(size.Height);
    }

    public static void MockArea(this Mock<ICursesBackend> cursesMock, IntPtr handle, Rectangle area)
    {
        Debug.Assert(cursesMock != null);

        _ = cursesMock.Setup(s => s.getbegy(handle))
                  .Returns(area.Top);

        _ = cursesMock.Setup(s => s.getbegx(handle))
                  .Returns(area.Left);

        _ = cursesMock.Setup(s => s.getparx(handle))
                  .Returns(area.Left);

        _ = cursesMock.Setup(s => s.getpary(handle))
                  .Returns(area.Top);

        _ = cursesMock.Setup(s => s.getmaxx(handle))
                  .Returns(area.Width);

        _ = cursesMock.Setup(s => s.getmaxy(handle))
                  .Returns(area.Height);
    }

    public static void MockArea(this Mock<ICursesBackend> cursesMock, ISurface surface, Rectangle area)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        MockArea(cursesMock, surface.Handle, area);
    }

    public static Mock<T> MockResolve<T>(this Mock<INativeSymbolResolver> mock) where T : MulticastDelegate
    {
        var m = new Mock<T>();
        _ = mock.Setup(s => s.Resolve<T>())
            .Returns(m.Object);

        return m;
    }

    public static Mock<T> MockResolve<T, TResult>(this Mock<INativeSymbolResolver> mock,
        Expression<Func<T, TResult>> expression, TResult ret) where T : MulticastDelegate
    {
        var m = new Mock<T>();
        _ = m.Setup(expression)
         .Returns(new InvocationFunc(_ => ret));

        _ = mock.Setup(s => s.Resolve<T>())
            .Returns(m.Object);

        return m;
    }
}
