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
            _dotNetSystemAdapterMock.Verify(s => s.TryLoadNativeLibrary(c, It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Once);
        }

        _dotNetSystemAdapterMock.Verify(s => s.TryLoadNativeLibrary(It.IsAny<string>(), It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
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
    public void TryLoad_ReturnsNull_IfProviderIsInvalid()
    {
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, 0, Array.Empty<string>()).ShouldBeNull();
        
        _dotNetSystemAdapterMock.Verify(v => v.TryLoadNativeLibrary(It.IsAny<string>(), out It.Ref<IntPtr>.IsAny), Times.Never);
        _dotNetSystemAdapterMock.Verify(v => v.TryLoadNativeLibrary(It.IsAny<string>(), 
            It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(), out It.Ref<IntPtr>.IsAny), Times.Never);
    }
    
    [TestMethod]
    public void TryLoad_ReturnsNull_IfNoPathSupplied()
    {
        MockLoadResult("library", false);
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses, Array.Empty<string>()).ShouldBeNull();
    }
    
    [TestMethod]
    public void TryLoad_ReturnsNull_IfFailedToLoadProvider()
    {
        MockLoadResult("library", false);
        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses, new[] { "library" }).ShouldBeNull();
        
        VerifyLoadLibraryAttempts("library");
    }
    
    [TestMethod]
    public void TryLoad_WhenUnix_ReturnsBackend_IfFailedToLoadLibC()
    {
        MockLoadResult("library", true);
        MockLoadResult("libc", false);

        CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses, new[] { "library" }).ShouldNotBeNull();

        VerifyLoadLibraryAttempts("library", "libc");
    }
    
    [TestMethod, DataRow(CursesBackend.Provider.NCurses), DataRow(CursesBackend.Provider.PdCurses), 
     DataRow(CursesBackend.Provider.PdCursesMod32Wide)]
    public void TryLoad_ReturnsAppropriateBackEnd(CursesBackend.Provider provider)
    {
        MockLoadResult("library", true);

        var backend = CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, provider, new[] { "library" });
        switch (provider)
        {
            case CursesBackend.Provider.NCurses:
                backend.ShouldBeOfType<NCursesBackend>();
                break;
            case CursesBackend.Provider.PdCurses:
                backend.ShouldBeOfType<PdCursesBackend>();
                break;
            case CursesBackend.Provider.PdCursesMod32Wide:
                backend.ShouldBeOfType<PdCursesMod32Backend>();
                break;
        }
        
        backend?.LibCSymbolResolver.ShouldBeNull();
    }
    
    [TestMethod, DataRow(CursesBackend.Provider.NCurses), DataRow(CursesBackend.Provider.PdCurses), 
     DataRow(CursesBackend.Provider.PdCursesMod32Wide)]
    public void TryLoad_WhenUnix_ReturnsAppropriateBackEnd(CursesBackend.Provider provider)
    {
        MockLoadResult("library", true);
        MockLoadResult("libc", true);

        var backend = CursesBackend.TryLoad(_dotNetSystemAdapterMock.Object, provider, new[] { "library" });
        switch (provider)
        {
            case CursesBackend.Provider.NCurses:
                backend.ShouldBeOfType<NCursesBackend>();
                break;
            case CursesBackend.Provider.PdCurses:
                backend.ShouldBeOfType<PdCursesBackend>();
                break;
            case CursesBackend.Provider.PdCursesMod32Wide:
                backend.ShouldBeOfType<PdCursesMod32Backend>();
                break;
        }
        
        backend?.LibCSymbolResolver.ShouldNotBeNull();
    }
    
    [TestMethod]
    public void Load1_WhenUnknownOs_Fails()
    {
        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() =>  CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any));

        actual.ShouldBe(Array.Empty<string>());
    }
    
    [TestMethod]
    public void Load1_WhenWindows_ReturnsValidBackend()
    {
        MockLoadResult("ncurses.dll", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any)
                     .ShouldNotBeNull();
    }
      
    [TestMethod]
    public void Load1_WhenLinux_ReturnsValidBackend()
    {
        MockLoadResult("ncurses", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any)
                     .ShouldNotBeNull();
    }
          
    [TestMethod]
    public void Load1_WhenFreeBsd_ReturnsValidBackend()
    {
        MockLoadResult("ncurses", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any)
                     .ShouldNotBeNull();
    }
    
    [TestMethod]
    public void Load1_WhenMacOs_ReturnsValidBackend()
    {
        MockLoadResult("ncurses", true);

        CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any)
                     .ShouldNotBeNull();
    }
    
    [TestMethod]
    public void Load1_WhenLinux_TriesToLoadManyOptions()
    {
        var options = new[]
        {
            "libpdcurses2.so",
            "libpdcurses.so",
            "libXCurses.so",
            "libpdcurses2.so",
            "libpdcurses.so",
            "libXCurses.so",
            "libncursesw.so.6",
            "libncursesw.so.5",
            "libncursesw.so",
            "ncursesw",
            "libncurses.so.6",
            "libncurses.so.5",
            "libncurses.so",
            "ncurses"
        };
          
        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() =>  CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any));

        actual.ShouldBe(options);
    }
    
    [TestMethod]
    public void Load1_WhenFreeBsd_TriesToLoadManyOptions()
    {
        var options = new[]
        {
            "libpdcurses2.so",
            "libpdcurses.so",
            "libXCurses.so",
            "libpdcurses2.so",
            "libpdcurses.so",
            "libXCurses.so",
            "libncursesw.so.6",
            "libncursesw.so.5",
            "libncursesw.so",
            "ncursesw",
            "libncurses.so.6",
            "libncurses.so.5",
            "libncurses.so",
            "ncurses"
        };

        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() =>  CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any));

        actual.ShouldBe(options);
    }
    
    [TestMethod]
    public void Load1_WhenMacOs_TriesToLoadManyOptions()
    {
        var options = new[] { "libpdcurses.dylib", "libpdcurses.dylib", "libpdcurses2.dylib", "ncursesw", "ncurses" };

        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() =>  CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any));

        actual.ShouldBe(options);
    }
    
    [TestMethod]
    public void Load1_WhenWindows_TriesToLoadManyOptions()
    {
        var options = new[]
        {
            "libpdcurses.dll",
            "pdcurses.dll",
            "libpdcurses.dll",
            "pdcurses.dll",
            "libncursesw.dll",
            "libncursesw6.dll",
            "libncursesw5.dll",
            "ncursesw.dll",
            "ncursesw6.dll",
            "ncursesw5.dll",
            "libncurses.dll",
            "libncurses6.dll",
            "libncurses5.dll",
            "ncurses.dll",
            "ncurses6.dll",
            "ncurses5.dll"
        };

        var actual = CaptureLoadAttempts();
        Should.Throw<CursesInitializationException>(() => CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.Any));

        actual.ShouldBe(options);
    }
    
    [TestMethod]
    public void Load1_WhenMacOs_ForNCurses_TriesToLoadDefault_IfNoHomeBrewFound()
    {
        Should.Throw<CursesInitializationException>(() => CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses));

        VerifyLoadLibraryAttempts("ncurses", "ncursesw");
    }

    [TestMethod]
    public void Load1_WhenMacOs_ScansTheLibraryDirectory_IfNoHomeBrewFound()
    {
        _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));

        _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_PREFIX"))
                                .Returns("/h");

        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+lib"))
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+lib"))
                                .Returns(new[]
                                {
                                    "dummy.txt",
                                    "libncurses.10",
                                    "libncurses.dylib",
                                    "libncurses.a.dylib",
                                    "libncurses.1.dylib",
                                    "libncurses.10.dylib"
                                });

        Should.Throw<CursesInitializationException>(() => CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses));

        VerifyLoadLibraryAttempts("/h+lib+libncurses.1.dylib", "/h+lib+libncurses.10.dylib", "ncurses", "ncursesw");
    }

    [TestMethod]
    public void Load1_WhenMacOs_ScansTheCellarDirectories_IfNoHomeBrewFound()
    {
        _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));

        _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_CELLAR"))
                                .Returns("/h");

        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses"))
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-one+lib"))
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-2+lib"))
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.EnumerateDirectories("/h+ncurses"))
                                .Returns(new[] { "/h+ncurses-one", "/h+ncurses-2" });

        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-one+lib"))
                                .Returns(new[]
                                {
                                    "dummy.txt",
                                    "libncurses.10",
                                    "libncurses.dylib",
                                    "libncurses.a.dylib",
                                    "libncurses.1.dylib",
                                    "libncurses.10.dylib"
                                });

        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-2+lib"))
                                .Returns(new[] { "dummy.txt", "libncurses.2.dylib", "libncurses.12.dylib" });

        Should.Throw<CursesInitializationException>(() => CursesBackend.Load(_dotNetSystemAdapterMock.Object, CursesBackend.Provider.NCurses));

        VerifyLoadLibraryAttempts("/h+ncurses-one+lib+libncurses.1.dylib", "/h+ncurses-2+lib+libncurses.2.dylib",
            "/h+ncurses-one+lib+libncurses.10.dylib", "/h+ncurses-2+lib+libncurses.12.dylib", "ncurses", "ncursesw");
    }
}
