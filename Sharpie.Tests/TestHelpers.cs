namespace Sharpie.Tests;

using System.Diagnostics;

internal static class TestHelpers
{
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

    public static void MockLargeArea(this Mock<ICursesProvider> cursesMock, ISurface surface)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        cursesMock.Setup(s => s.getmaxx(surface.Handle))
                  .Returns(1000);

        cursesMock.Setup(s => s.getmaxy(surface.Handle))
                  .Returns(1000);
    }

    public static void MockSmallArea(this Mock<ICursesProvider> cursesMock, ISurface surface)
    {
        Debug.Assert(cursesMock != null);
        Debug.Assert(surface != null);

        cursesMock.Setup(s => s.getmaxx(surface.Handle))
                  .Returns(1);

        cursesMock.Setup(s => s.getmaxy(surface.Handle))
                  .Returns(1);
    }
}

