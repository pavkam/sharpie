namespace Sharpie.Tests;

using System.Diagnostics;

internal static class TestHelpers
{
    public static (Rune, Style)[,] GetContents(this IDrawable drawing)
    {
        var mock = new Mock<IDrawSurface>();
        mock.Setup(s => s.Size)
            .Returns(drawing.Size);

        var collector = new (Rune, Style)[drawing.Size.Width, drawing.Size.Height];
        mock.Setup(s => s.DrawCell(It.IsAny<Point>(), It.IsAny<Rune>(), It.IsAny<Style>()))
            .Callback<Point, Rune, Style>((location, rune, textStyle) =>
            {
                collector[location.X, location.Y] = (rune, textStyle);
            });

        drawing.DrawOnto(mock.Object, new(0, 0, drawing.Size.Width, drawing.Size.Height), new(0, 0));
        return collector;
    }

    public static void MockArea(this Mock<ICursesProvider> cursesMock, ISurface surface, Size size)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        cursesMock.Setup(s => s.getmaxx(surface.Handle))
                  .Returns(size.Width);

        cursesMock.Setup(s => s.getmaxy(surface.Handle))
                  .Returns(size.Height);
    }

    public static void MockArea(this Mock<ICursesProvider> cursesMock, IntPtr handle, Rectangle area)
    {
        Debug.Assert(cursesMock != null);

        cursesMock.Setup(s => s.getbegy(handle))
                  .Returns(area.Top);

        cursesMock.Setup(s => s.getbegx(handle))
                  .Returns(area.Left);

        cursesMock.Setup(s => s.getmaxx(handle))
                  .Returns(area.Width);

        cursesMock.Setup(s => s.getmaxy(handle))
                  .Returns(area.Height);
    }

    public static void MockArea(this Mock<ICursesProvider> cursesMock, ISurface surface, Rectangle area)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        MockArea(cursesMock, surface.Handle, area);
    }
}
