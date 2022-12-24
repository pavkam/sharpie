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

[TestClass]
public class CursesBackendTests
{
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;

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

    private void VerifyAttempts(params string[] candidates)
    {
        foreach (var c in candidates)
        {
            _dotNetSystemAdapterMock.Verify(
                s => s.TryLoadNativeLibrary(c, out It.Ref<IntPtr>.IsAny), Times.Once);
        }
        
        _dotNetSystemAdapterMock.Verify(
            s => s.TryLoadNativeLibrary(It.IsAny<string>(), out It.Ref<IntPtr>.IsAny), Times.Exactly(candidates.Length));
    }
    
    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();

        _dotNetSystemAdapterMock
            .Setup(s => s.TryGetNativeLibraryExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
            .Returns(true);
    }

    [TestMethod]
    public void NCurses1_Throws_IfFailedToLoadNCurses()
    {
        Should.Throw<CursesInitializationException>(() =>
            CursesBackend.NCurses(_dotNetSystemAdapterMock.Object, s => new[] { s }));
    }

    [TestMethod]
    public void NCurses1_Throws_IfFailedToLoadLibC()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(true);

        MockLoadResult("ncurses", true);

        Should.Throw<CursesInitializationException>(() =>
            CursesBackend.NCurses(_dotNetSystemAdapterMock.Object, s => new[] { s }));
    }

    [TestMethod]
    public void NCurses1_ForUnixLikeSystems_LoadsNCursesAndLibC()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(true);

        MockLoadResult("ncurses", true);
        MockLoadResult("libc", true);

        var requests = new List<string>();

        if (_dotNetSystemAdapterMock.Object.IsUnixLike)
        {
            CursesBackend.NCurses(_dotNetSystemAdapterMock.Object, s =>
                         {
                             requests.Add(s);
                             return new[] { s };
                         })
                         .ShouldBeOfType<UnixNCursesBackend>();
        }

        requests.ShouldBe(new[] { "ncurses", "libc" });
    }

    [TestMethod]
    public void NCurses1_ForNonUnixLikeSystems_LoadsNCurses()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(false);

        MockLoadResult("ncurses", true);

        var requests = new List<string>();
        CursesBackend.NCurses(_dotNetSystemAdapterMock.Object, s =>
                     {
                         requests.Add(s);
                         return new[] { s };
                     })
                     .ShouldBeOfType<NCursesBackend>();

        requests.ShouldBe(new[] { "ncurses" });
    }

    [TestMethod]
    public void NCurses2_Throws_LibPathResolverIsNull()
    {
        Should.Throw<ArgumentNullException>(() => CursesBackend.NCurses((Func<string, IEnumerable<string>>) null!));
    }

    [TestMethod]
    public void NCurses2_CallsNCurses1()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(true);

        MockLoadResult("ncurses", true);
        MockLoadResult("libc", true);

        CursesBackend.NCurses(_dotNetSystemAdapterMock.Object);

        _dotNetSystemAdapterMock.Verify(
            s => s.TryLoadNativeLibrary("ncurses", It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Once);

        _dotNetSystemAdapterMock.Verify(
            s => s.TryLoadNativeLibrary("libc", It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Once);
    }
    
    [TestMethod, DataRow("linux"), DataRow("freebsd")]
    public void NCurses2_ForLinuxOrFreeBsd_HasSpecificOptions(string op)
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsLinux)
                                .Returns(op == "linux");
        _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(op == "freebsd");
        
        var options = new[]
        {
            "libncursesw.so.6",
            "libncursesw.so.5",
            "libncursesw.so",
            "ncursesw",
            "libncurses.so.6",
            "libncurses.so.5",
            "libncurses.so",
            "ncurses",
        };

        Should.Throw<CursesInitializationException>(() => CursesBackend.NCurses(_dotNetSystemAdapterMock.Object));

        VerifyAttempts(options);
    }

    [TestMethod]
    public void NCurses2_ForMacOs_TriesToLoadDefault_IfNoHomeBrewFound()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(true);

        Should.Throw<CursesInitializationException>(() => CursesBackend.NCurses(_dotNetSystemAdapterMock.Object));

        VerifyAttempts("ncurses");
    }
    
    [TestMethod]
    public void NCurses2_ForMacOs_ScansTheLibraryDirectory_IfNoHomeBrewFound()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));
        _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_PREFIX")).Returns("/h");
        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+lib")).Returns(true);
        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+lib")).Returns(new[]
        {
            "dummy.txt",
            "libncurses.10",
            "libncurses.dylib",
            "libncurses.a.dylib",
            "libncurses.1.dylib",
            "libncurses.10.dylib"
        });
        
        Should.Throw<CursesInitializationException>(() => CursesBackend.NCurses(_dotNetSystemAdapterMock.Object));
        
        VerifyAttempts("/h+lib+libncurses.1.dylib", "/h+lib+libncurses.10.dylib", "ncurses");
    }
    
    [TestMethod]
    public void NCurses2_ForMacOs_ScansTheCellarDirectories_IfNoHomeBrewFound()
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(true);

        _dotNetSystemAdapterMock.Setup(s => s.CombinePaths(It.IsAny<string[]>()))
                                .Returns((string[] ps) => string.Join("+", ps));
        _dotNetSystemAdapterMock.Setup(s => s.GetEnvironmentVariable("HOMEBREW_CELLAR")).Returns("/h");
        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses")).Returns(true);
        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-one+lib")).Returns(true);
        _dotNetSystemAdapterMock.Setup(s => s.DirectoryExists("/h+ncurses-2+lib")).Returns(true);
        _dotNetSystemAdapterMock.Setup(s => s.EnumerateDirectories("/h+ncurses")).Returns(new[]
        {
            "/h+ncurses-one",
            "/h+ncurses-2"
        });
        
        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-one+lib")).Returns(new[]
        {
            "dummy.txt",
            "libncurses.10",
            "libncurses.dylib",
            "libncurses.a.dylib",
            "libncurses.1.dylib",
            "libncurses.10.dylib"
        });
        
        _dotNetSystemAdapterMock.Setup(s => s.EnumerateFiles("/h+ncurses-2+lib")).Returns(new[]
        {
            "dummy.txt",
            "libncurses.2.dylib",
            "libncurses.12.dylib"
        });
        
        Should.Throw<CursesInitializationException>(() => CursesBackend.NCurses(_dotNetSystemAdapterMock.Object));
        
        VerifyAttempts(
            "/h+ncurses-one+lib+libncurses.1.dylib",
            "/h+ncurses-2+lib+libncurses.2.dylib",
            "/h+ncurses-one+lib+libncurses.10.dylib",
            "/h+ncurses-2+lib+libncurses.12.dylib",
            "ncurses");
    }
    
}
