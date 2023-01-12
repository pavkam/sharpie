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

using System.Reflection;
using System.Runtime.InteropServices;

[TestClass, SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class CursesBackendTests
{
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    private void MockLoadResult(string lib, bool result)
    {
        _dotNetSystemAdapterMock.Setup(s => s.TryLoadNativeLibrary(lib, It.IsAny<Assembly>(),
                                    It.IsAny<DllImportSearchPath?>(), out It.Ref<IntPtr>.IsAny))
                                .Returns((string _, Assembly _, DllImportSearchPath? _, out IntPtr handle) =>
                                {
                                    handle = new(100);
                                    return result;
                                });
    }

    private IEnumerable<string> CaptureLoadAttempts()
    {
        var att = new List<string>();
        _dotNetSystemAdapterMock.Setup(s => s.TryLoadNativeLibrary(It.IsAny<string>(), It.IsAny<Assembly>(),
                                    It.IsAny<DllImportSearchPath?>(), out It.Ref<IntPtr>.IsAny))
                                .Returns((string lib, Assembly _, DllImportSearchPath? _, out IntPtr handle) =>
                                {
                                    att.Add(lib);
                                    handle = new(0);
                                    return false;
                                });

        return att;
    }

    private void VerifyLoadLibraryAttempts(params string[] candidates)
    {
        foreach (var c in candidates)
        {
            _dotNetSystemAdapterMock.Verify(
                s => s.TryLoadNativeLibrary(c, It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                    out It.Ref<IntPtr>.IsAny), Times.Once);
        }

        _dotNetSystemAdapterMock.Verify(
            s => s.TryLoadNativeLibrary(It.IsAny<string>(), It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Exactly(candidates.Length));
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();

        var isLinux = TestContext.TestName!.Contains("_WhenLinux_");
        var isFreeBsd = TestContext.TestName!.Contains("_WhenFreeBsd_");
        var isMacOs = TestContext.TestName!.Contains("_WhenMacOs_");
        var isWindows = TestContext.TestName!.Contains("_WhenWindows_");
        var isUnix = TestContext.TestName!.Contains("_WhenUnix_") || isLinux || isFreeBsd || isMacOs;

        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(isUnix);

        _dotNetSystemAdapterMock.Setup(s => s.IsLinux)
                                .Returns(isLinux);

        _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(isFreeBsd);

        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(isMacOs);

        _dotNetSystemAdapterMock.Setup(s => s.IsWindows)
                                .Returns(isWindows);


        _dotNetSystemAdapterMock
            .Setup(s => s.TryGetNativeLibraryExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
            .Returns(true);
    }

    [TestMethod]
    public void TryLoad_ReturnsNull_IfTypeIsInvalid()
    {
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, 0, Array.Empty<string>())
                     .ShouldBeNull();

        _dotNetSystemAdapterMock.Verify(v => v.TryLoadNativeLibrary(It.IsAny<string>(), out It.Ref<IntPtr>.IsAny),
            Times.Never);

        _dotNetSystemAdapterMock.Verify(
            v => v.TryLoadNativeLibrary(It.IsAny<string>(), It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Never);
    }

    [TestMethod]
    public void TryLoad_ReturnsNull_IfNoPathSupplied()
    {
        MockLoadResult("library", false);
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackendType.NCurses, Array.Empty<string>())
                     .ShouldBeNull();
    }

    [TestMethod]
    public void TryLoad_ReturnsNull_IfFailedToLoadType()
    {
        MockLoadResult("library", false);
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackendType.NCurses, new[] { "library" })
                     .ShouldBeNull();

        VerifyLoadLibraryAttempts("library");
    }

    [TestMethod]
    public void TryLoad_WhenUnix_ReturnsBackend_IfFailedToLoadLibC()
    {
        MockLoadResult("library", true);
        MockLoadResult("libc", false);

        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackendType.NCurses, new[] { "library" })
                     .ShouldNotBeNull();

        VerifyLoadLibraryAttempts("library", "libc");
    }

    [TestMethod, DataRow(CursesBackendType.NCurses), DataRow(CursesBackendType.PdCurses),
     DataRow(CursesBackendType.PdCursesMod)]
    public void TryLoad_ReturnsAppropriateBackEnd(CursesBackendType type)
    {
        MockLoadResult("library", true);

        var backend = CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, type, new[] { "library" });
        switch (type)
        {
            case CursesBackendType.NCurses:
                backend.ShouldBeOfType<NCursesBackend>();
                break;
            case CursesBackendType.PdCurses:
                backend.ShouldBeOfType<PdCursesBackend>();
                break;
            case CursesBackendType.PdCursesMod:
                backend.ShouldBeOfType<PdCursesMod32Backend>();
                break;
        }

        backend?.LibCSymbolResolver.ShouldBeNull();
    }

    [TestMethod, DataRow(CursesBackendType.NCurses), DataRow(CursesBackendType.PdCurses),
     DataRow(CursesBackendType.PdCursesMod)]
    public void TryLoad_WhenUnix_ReturnsAppropriateBackEnd(CursesBackendType type)
    {
        MockLoadResult("library", true);
        MockLoadResult("libc", true);

        var backend = CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, type, new[] { "library" });
        switch (type)
        {
            case CursesBackendType.NCurses:
                backend.ShouldBeOfType<NCursesBackend>();
                break;
            case CursesBackendType.PdCurses:
                backend.ShouldBeOfType<PdCursesBackend>();
                break;
            case CursesBackendType.PdCursesMod:
                backend.ShouldBeOfType<PdCursesMod32Backend>();
                break;
        }

        backend?.LibCSymbolResolver.ShouldNotBeNull();
    }

    [TestMethod]
    public void Load1_WhenLinux_Fails_IfNoFlavor()
    {
        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() =>
            CursesBackend.Load(_dotNetSystemAdapterMock.Object, Array.Empty<CursesBackendFlavor>()));

        actual.ShouldBe(Array.Empty<string>());
    }

    [TestMethod]
    public void Load1_WhenLinux_Fails_IfNoFlavorCouldBeLoaded()
    {
        Should.Throw<CursesInitializationException>(() =>
            CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any }));
    }

    [TestMethod]
    public void Load1_Throws_WhenUnknownOs()
    {
        Should.Throw<PlatformNotSupportedException>(() =>
            CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any }));
    }

    [TestMethod]
    public void Load1_WhenWindows_ReturnsValidBackend()
    {
        MockLoadResult("ncursesw.dll", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any })
                     .ShouldNotBeNull();
    }

    [TestMethod]
    public void Load1_WhenLinux_ReturnsValidBackend()
    {
        MockLoadResult("libncursesw.so", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any })
                     .ShouldNotBeNull();
    }

    [TestMethod]
    public void Load1_WhenFreeBsd_ReturnsValidBackend()
    {
        MockLoadResult("libncursesw.so", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any })
                     .ShouldNotBeNull();
    }

    [TestMethod]
    public void Load1_WhenMacOs_ReturnsValidBackend()
    {
        MockLoadResult("libncurses.dylib", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, new[] { CursesBackendFlavor.Any })
                     .ShouldNotBeNull();
    }

    [TestMethod]
    public void Load2_Throws_WhenFlavorsIsNull()
    {
        Should.Throw<ArgumentNullException>(() => CursesBackend.Load(CursesBackendFlavor.AnyGui, null!));
    }

    [TestMethod]
    public void Load3_TriesToLoadGivenPaths()
    {
        var actual = CaptureLoadAttempts();

        Should.Throw<CursesInitializationException>(() =>
            CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackendType.PdCurses,
                new[] { "path1", "path2" }));

        actual.ShouldBe(new[] { "path1", "path2" });
    }

    [TestMethod]
    public void Load3_WhenLinux_StopsAtFirstGoodLoad()
    {
        MockLoadResult("path2", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackendType.NCurses, new[] { "path1", "path2" })
                     .ShouldNotBeNull();

        VerifyLoadLibraryAttempts("path1", "path2", "libc");
    }

    [TestMethod]
    public void Load3_Throws_WhenPathIsNull()
    {
        Should.Throw<ArgumentNullException>(() => CursesBackend.Load(CursesBackendType.NCurses, null!, "hello"));
    }

    [TestMethod]
    public void Load3_Throws_WhenOtherPathsIsNull()
    {
        Should.Throw<ArgumentNullException>(() => CursesBackend.Load(CursesBackendType.NCurses, "hello", null!));
    }
}
