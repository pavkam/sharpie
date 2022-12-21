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
public class NativeLibraryWrapperTests
{
    [SuppressMessage("ReSharper", "UnusedType.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    private abstract class DummyFunctionMap
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int Test1();

        public delegate int Test2();

        public void Test3() { }

        public delegate int Test4<T>();
    }

    private Mock<ISystemInteropAdapter> _interopMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _interopMock = new();

        _interopMock.Setup(s => s.TryGetExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
                    .Returns(true);

        _interopMock.Setup(s => s.GetDelegateForFunctionPointer(It.IsAny<IntPtr>(), It.IsAny<Type>()))
                    .Returns(new DummyFunctionMap.Test1(() => 123));
    }

    [TestMethod] public void Adapter_IsNotNull() { NativeLibraryWrapper<DummyFunctionMap>.Adapter.ShouldNotBeNull(); }

    [TestMethod]
    public void TryLoad_Throws_IfInteropAdapterIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
            NativeLibraryWrapper<DummyFunctionMap>.TryLoad(null!, Array.Empty<string>()));
    }

    [TestMethod]
    public void TryLoad_Throws_IfLibraryNameOrPathsIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
            NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, null!));
    }

    [TestMethod]
    public void TryLoad_ReturnsNull_IfLibraryNameOrPathsIsEmpty()
    {
        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, Array.Empty<string>())
                                              .ShouldBeNull();
    }

    [TestMethod]
    public void TryLoad_ReturnsNull_IfFailedToLoadAnything()
    {
        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, new[] { "hello" })
                                              .ShouldBeNull();
    }

    [TestMethod]
    public void TryLoad_TriesToLoadByName_IfHasNoDirectory_ThenByPath()
    {
        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, new[] { "hello" });

        _interopMock.Verify(v => v.TryLoad("hello", It.IsNotNull<Assembly>(), null, out It.Ref<IntPtr>.IsAny),
            Times.Once);

        _interopMock.Verify(v => v.TryLoad("hello", out It.Ref<IntPtr>.IsAny), Times.Once);
    }

    [TestMethod]
    public void TryLoad_TriesToLoadByPath_IfHasDirectory()
    {
        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, new[] { "hello/world" });

        _interopMock.Verify(
            v => v.TryLoad(It.IsAny<string>(), It.IsNotNull<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                out It.Ref<IntPtr>.IsAny), Times.Never);

        _interopMock.Verify(v => v.TryLoad("hello/world", out It.Ref<IntPtr>.IsAny), Times.Once);
    }

    [TestMethod]
    public void TryLoad_ReturnsAnInstance_IfLoadedByName()
    {
        _interopMock.Setup(s => s.TryLoad(It.IsAny<string>(), It.IsAny<Assembly>(), It.IsAny<DllImportSearchPath?>(),
                        out It.Ref<IntPtr>.IsAny))
                    .Returns((string _, Assembly _, DllImportSearchPath? _, out IntPtr handle) =>
                    {
                        handle = new(1);
                        return true;
                    });

        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, new[] { "hello" })
                                              .ShouldNotBeNull();
    }

    [TestMethod]
    public void TryLoad_ReturnsAnInstance_IfLoadedByPath()
    {
        _interopMock.Setup(s => s.TryLoad(It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
                    .Returns((string _, out IntPtr handle) =>
                    {
                        handle = new(1);
                        return true;
                    });

        NativeLibraryWrapper<DummyFunctionMap>.TryLoad(_interopMock.Object, new[] { "hello/world" })
                                              .ShouldNotBeNull();
    }

    [TestMethod]
    public void Ctor_Throws_IfInteropAdapter_IsNull()
    {
        Should.Throw<ArgumentNullException>(() => new NativeLibraryWrapper<DummyFunctionMap>(null!, new(100)));
    }

    [TestMethod]
    public void Ctor_StoresTheHandleValue()
    {
        new NativeLibraryWrapper<DummyFunctionMap>(_interopMock.Object, new(100)).ShouldNotBeNull();

        _interopMock.Verify(v => v.TryGetExport(new(100), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny));
    }

    [TestMethod]
    public void Ctor_ExtractsOnlyTheSupportedSymbols()
    {
        _interopMock.Setup(s => s.TryGetExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
                    .Returns((IntPtr _, string _, out IntPtr address) =>
                    {
                        address = new(111);
                        return true;
                    });

        new NativeLibraryWrapper<DummyFunctionMap>(_interopMock.Object, new(100)).ShouldNotBeNull();

        _interopMock.Verify(v => v.GetDelegateForFunctionPointer(new(111), typeof(DummyFunctionMap.Test1)), Times.Once);
        _interopMock.Verify(v => v.TryGetExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny),
            Times.Once);
    }

    [TestMethod]
    public void Ctor_Throws_IfFailedToExtractOneSymbol()
    {
        _interopMock.Setup(s => s.TryGetExport(It.IsAny<IntPtr>(), It.IsAny<string>(), out It.Ref<IntPtr>.IsAny))
                    .Returns(false);

        Should.Throw<MissingMethodException>(() =>
            new NativeLibraryWrapper<DummyFunctionMap>(_interopMock.Object, new(100)));
    }

    [TestMethod]
    public void Resolve_Throws_IfUnknownDelegate()
    {
        var w = new NativeLibraryWrapper<DummyFunctionMap>(_interopMock.Object, new(100));

        Should.Throw<MissingMethodException>(() => w.Resolve<LibCFunctionMap.setlocale>());
    }

    [TestMethod]
    public void Resolve_ExtractsTheKnownSymbol()
    {
        _interopMock.Setup(s => s.GetDelegateForFunctionPointer(It.IsAny<IntPtr>(), It.IsAny<Type>()))
                    .Returns(new DummyFunctionMap.Test1(() => 123));

        var w = new NativeLibraryWrapper<DummyFunctionMap>(_interopMock.Object, new(100)).ShouldNotBeNull();

        w.Resolve<DummyFunctionMap.Test1>()()
         .ShouldBe(123);
    }
}
