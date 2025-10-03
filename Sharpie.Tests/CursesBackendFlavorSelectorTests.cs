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

[TestClass, SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class CursesBackendFlavorSelectorTests
{
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private CursesBackendFlavorSelector _provider = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();

        var isLinux = TestContext.TestName!.Contains("_WhenLinux_");
        var isFreeBsd = TestContext.TestName!.Contains("_WhenFreeBsd_");
        var isMacOs = TestContext.TestName!.Contains("_WhenMacOs_");
        var isWindows = TestContext.TestName!.Contains("_WhenWindows_");
        var isUnix = TestContext.TestName!.Contains("_WhenUnix_") || isLinux || isFreeBsd || isMacOs;

        _ = _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(isUnix);

        _ = _dotNetSystemAdapterMock.Setup(s => s.IsLinux)
                                .Returns(isLinux);

        _ = _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(isFreeBsd);

        _ = _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(isMacOs);

        _ = _dotNetSystemAdapterMock.Setup(s => s.IsWindows)
                                .Returns(isWindows);

        _provider = new(_dotNetSystemAdapterMock.Object);
    }

    [TestMethod]
    public void GetLibraryPaths_WhenUnknownOs_Throws() => Should.Throw<PlatformNotSupportedException>(() => _provider.GetLibraryPaths(CursesBackendFlavor.NCurses));

    [TestMethod]
    public void GetLibraryPaths_WhenLinux_ReturnsExpectedPaths()
    {
        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[]
        {
            ("libncursesw.so.6", CursesBackendType.NCurses),
            ("libncursesw.so", CursesBackendType.NCurses),
            ("libncursesw.so.5", CursesBackendType.NCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCurses);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesMod);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl1.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-sdl1.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.NCurses)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal)));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyGui);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.Any);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.AnyGui)));
    }

    [TestMethod]
    public void GetLibraryPaths_WhenFreeBsd_ReturnsExpectedPaths()
    {
        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[]
        {
            ("libncursesw.so.6", CursesBackendType.NCurses),
            ("libncursesw.so", CursesBackendType.NCurses),
            ("libncursesw.so.5", CursesBackendType.NCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCurses);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesMod);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl1.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-sdl1.so", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.so", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.NCurses)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal)));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyGui);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.Any);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.AnyGui)));
    }

    [TestMethod]
    public void GetLibraryPaths_WhenMacOs_ReturnsExpectedPaths()
    {
        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[] { ("libncurses.dylib", CursesBackendType.NCurses) });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCurses);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesMod);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.dylib", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.dylib", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl1.dylib", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.dylib", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-vt.dylib", CursesBackendType.PdCursesMod),
            ("libpdcursesmod.dylib", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui);
        actual.ShouldBe(new[]
        {
            ("libpdcursesmod-sdl1.dylib", CursesBackendType.PdCursesMod),
            ("libpdcursesmod-sdl2.dylib", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyWindowsConsole);
        actual.ShouldBe(Array.Empty<(string, CursesBackendType)>());

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.NCurses)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal)));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyGui);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.Any);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.AnyGui)));
    }

    [TestMethod]
    public void GetLibraryPaths_WhenWindows_ReturnsExpectedPaths()
    {
        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[]
        {
            ("ncursesw6.dll", CursesBackendType.NCurses),
            ("ncursesw.dll", CursesBackendType.NCurses),
            ("ncursesw5.dll", CursesBackendType.NCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCurses);
        actual.ShouldBe(new[]
        {
            ("pdcurses-wincon.dll", CursesBackendType.PdCurses), ("pdcurses.dll", CursesBackendType.PdCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole);
        actual.ShouldBe(new[]
        {
            ("pdcurses-wincon.dll", CursesBackendType.PdCurses), ("pdcurses.dll", CursesBackendType.PdCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesMod);
        actual.ShouldBe(new[]
        {
            ("pdcursesmod-wincon.dll", CursesBackendType.PdCursesMod),
            ("pdcursesmod-vt.dll", CursesBackendType.PdCursesMod),
            ("pdcursesmod.dll", CursesBackendType.PdCursesMod),
            ("pdcursesmod-wingui.dll", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModWindowsConsole);
        actual.ShouldBe(new[] { ("pdcursesmod-wincon.dll", CursesBackendType.PdCursesMod) });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal);
        actual.ShouldBe(new[]
        {
            ("pdcursesmod-vt.dll", CursesBackendType.PdCursesMod),
            ("pdcursesmod.dll", CursesBackendType.PdCursesMod)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui);
        actual.ShouldBe(new[] { ("pdcursesmod-wingui.dll", CursesBackendType.PdCursesMod) });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyWindowsConsole);
        actual.ShouldBe(new[]
        {
            ("pdcursesmod-wincon.dll", CursesBackendType.PdCursesMod),
            ("pdcurses-wincon.dll", CursesBackendType.PdCurses),
            ("pdcurses.dll", CursesBackendType.PdCurses)
        });

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.NCurses)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal)));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.AnyGui);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.PdCursesModGui));

        actual = _provider.GetLibraryPaths(CursesBackendFlavor.Any);
        actual.ShouldBe(_provider.GetLibraryPaths(CursesBackendFlavor.AnyWindowsConsole)
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal))
                                 .Concat(_provider.GetLibraryPaths(CursesBackendFlavor.AnyGui)));
    }

    [TestMethod]
    public void GetLibraryPaths_WhenMacOs_ForNCurses_TriesToLoadDefault_IfNoHomeBrewFound()
    {
        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[] { ("libncurses.dylib", CursesBackendType.NCurses) });
    }

    [TestMethod]
    public void GetLibraryPaths_WhenMacOs_ForNCurses_ScansTheLibraryDirectory_IfNoHomeBrewFound()
    {
        _ = _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));

        _ = _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_PREFIX"))
                                .Returns("/h");

        _ = _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+lib"))
                                .Returns(true);

        _ = _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+lib"))
                                .Returns(new[]
                                {
                                    "dummy.txt",
                                    "libncurses.10",
                                    "libncurses.dylib",
                                    "libncurses.a.dylib",
                                    "libncurses.1.dylib",
                                    "libncurses.10.dylib"
                                });

        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[]
        {
            ("/h+lib+libncurses.10.dylib", CursesBackendType.NCurses),
            ("/h+lib+libncurses.1.dylib", CursesBackendType.NCurses),
            ("libncurses.dylib", CursesBackendType.NCurses)
        });
    }

    [TestMethod]
    public void GetLibraryPaths_WhenMacOs_ForNCurses_ScansTheCellarDirectories_IfNoHomeBrewFound()
    {
        _ = _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));

        _ = _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_CELLAR"))
                                .Returns("/h");

        _ = _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses"))
                                .Returns(true);

        _ = _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-one+lib"))
                                .Returns(true);

        _ = _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-2+lib"))
                                .Returns(true);

        _ = _dotNetSystemAdapterMock.Setup(s => s.EnumerateDirectories("/h+ncurses"))
                                .Returns(new[] { "/h+ncurses-one", "/h+ncurses-2" });

        _ = _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-one+lib"))
                                .Returns(new[]
                                {
                                    "dummy.txt",
                                    "libncurses.10",
                                    "libncurses.dylib",
                                    "libncurses.a.dylib",
                                    "libncurses.1.dylib",
                                    "libncurses.10.dylib"
                                });

        _ = _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-2+lib"))
                                .Returns(new[] { "dummy.txt", "libncurses.2.dylib", "libncurses.12.dylib" });

        var actual = _provider.GetLibraryPaths(CursesBackendFlavor.NCurses);
        actual.ShouldBe(new[]
        {
            ("/h+ncurses-2+lib+libncurses.12.dylib", CursesBackendType.NCurses),
            ("/h+ncurses-one+lib+libncurses.10.dylib", CursesBackendType.NCurses),
            ("/h+ncurses-2+lib+libncurses.2.dylib", CursesBackendType.NCurses),
            ("/h+ncurses-one+lib+libncurses.1.dylib", CursesBackendType.NCurses),
            ("libncurses.dylib", CursesBackendType.NCurses)
        });
    }
}
