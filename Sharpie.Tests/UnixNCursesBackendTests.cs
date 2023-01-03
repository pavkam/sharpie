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

using System.Runtime.InteropServices;

[TestClass, SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class UnixNCursesBackendTests
{
    private UnixNCursesBackend _backend = null!;
    private Mock<IDotNetSystemAdapter> _dotNetSystemAdapterMock = null!;
    private Mock<INativeSymbolResolver> _nativeSymbolResolverMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _dotNetSystemAdapterMock = new();
        _nativeSymbolResolverMock = new();

        _dotNetSystemAdapterMock.Setup(s => s.IsUnixLike)
                                .Returns(true);

        _backend = new(_dotNetSystemAdapterMock.Object, _nativeSymbolResolverMock.Object,
            _nativeSymbolResolverMock.Object);
    }

    [TestMethod, DataRow("linux", 6), DataRow("macos", 0), DataRow("freebsd", 6),
     SuppressMessage("ReSharper", "IdentifierTypo")]
    public void initscr_CallsLibc_ToSetupUnicode(string os, int cate)
    {
        _dotNetSystemAdapterMock.Setup(s => s.IsMacOs)
                                .Returns(os == "macos");

        _dotNetSystemAdapterMock.Setup(s => s.IsLinux)
                                .Returns(os == "linux");

        _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(os == "freebsd");

        var m = _nativeSymbolResolverMock.MockResolve<LibCFunctionMap.setlocale>();
        var o = _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.initscr>();

        _backend.initscr();

        m.Verify(v => v(cate, ""));
        o.Verify(x => x());
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void mousemask_CallsCursesButNotNotConsole_IfCursesFails()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version, IntPtr>(s => s(),
            Marshal.StringToHGlobalAnsi("version 1"));
            
        _dotNetSystemAdapterMock.Setup(s => s.IsFreeBsd)
                                .Returns(true);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask>()
                                 .Setup(s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny))
                                 .Returns((uint _, out uint o) =>
                                 {
                                     o = 999;
                                     return -1;
                                 });

        _backend.mousemask(100, out var old)
                .ShouldBe(-1);

        old.ShouldBe(999u);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush(It.IsAny<string>()), Times.Never);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo"), DataRow(1), DataRow(2)]
    public void mousemask_OutsToConsole_WhenReportingPosition(int abi)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        var version = abi == 2 ? "version 6.0.0" : "version 1.0.0";
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version, IntPtr>(s => s(),
            Marshal.StringToHGlobalAnsi(version));

        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();

        var parser = CursesMouseEventParser.Get(abi);
        _backend.mousemask(parser.ReportPosition, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1003h"), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo"), DataRow(1), DataRow(2)]
    public void mousemask_OutsToConsole_WhenAll(int abi)
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        var version = abi == 2 ? "version 6.0.0" : "version 1.0.0";
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version, IntPtr>(s => s(),
            Marshal.StringToHGlobalAnsi(version));

        _dotNetSystemAdapterMock.Setup(s => s.NativeLibraryAnsiStrPtrToString(It.IsAny<IntPtr>()))
                                .CallBase();

        var parser = CursesMouseEventParser.Get(abi);
        _backend.mousemask(parser.All, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1000h"), Times.Once);
    }

    [TestMethod, SuppressMessage("ReSharper", "IdentifierTypo")]
    public void mousemask_OutsToConsole_WhenNothing()
    {
        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.mousemask, int>(
            s => s(It.IsAny<uint>(), out It.Ref<uint>.IsAny), 0);

        _nativeSymbolResolverMock.MockResolve<BaseCursesFunctionMap.curses_version>();
        _backend.mousemask(0, out var _)
                .ShouldBe(0);

        _dotNetSystemAdapterMock.Verify(v => v.OutAndFlush("\x1b[?1003l"), Times.Once);
    }
}
