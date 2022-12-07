namespace Sharpie.Tests;

using System.Diagnostics;

internal static class TestHelpers
{
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
