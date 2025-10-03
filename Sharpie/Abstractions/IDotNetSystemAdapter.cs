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

namespace Sharpie.Abstractions;

/// <summary>
///     An internal interface used to help test the functionality or native library loader.
/// </summary>
internal interface IDotNetSystemAdapter
{
    /// <summary>
    ///     The actual instance that connects this interface to the .NET runtime.
    /// </summary>
    public static readonly IDotNetSystemAdapter Instance = new DotNetSystemAdapter();

    /// <summary>
    ///     Checks if the operating system is Windows.
    /// </summary>
    /// <returns><c>true</c> if the operating system is Windows; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("windows"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsWindows => OperatingSystem.IsWindows();

    /// <summary>
    ///     Checks if the operating system is Linux.
    /// </summary>
    /// <returns><c>true</c> if the operating system is Linux; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("linux"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsLinux => OperatingSystem.IsLinux();

    /// <summary>
    ///     Checks if the operating system is FreeBSD.
    /// </summary>
    /// <returns><c>true</c> if the operating system is FreeBSD; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("freebsd"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsFreeBsd => OperatingSystem.IsFreeBSD();

    /// <summary>
    ///     Checks if the operating system is MacOS.
    /// </summary>
    /// <returns><c>true</c> if the operating system is MacOS; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("macos"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsMacOs => OperatingSystem.IsMacOS();

    /// <summary>
    ///     Checks if the operating system is Unix-like.
    /// </summary>
    /// <returns><c>true</c> if the operating system is Unix-like; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("macos"),
     ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsUnixLike => IsLinux || IsFreeBsd || IsMacOs;

    /// <inheritdoc
    ///     cref="NativeLibrary.TryLoad(string,Assembly,DllImportSearchPath?,out IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath,
        out IntPtr handle) =>
        NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle);

    /// <inheritdoc cref="NativeLibrary.TryLoad(string,out IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryPath, out IntPtr handle) => NativeLibrary.TryLoad(libraryPath, out handle);

    /// <inheritdoc cref="NativeLibrary.TryGetExport(IntPtr,string,out IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryGetNativeLibraryExport(IntPtr handle, string name, out IntPtr address) =>
        NativeLibrary.TryGetExport(handle, name, out address);

    /// <inheritdoc cref="NativeLibrary.Free(IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void FreeNativeLibrary(IntPtr handle) => NativeLibrary.Free(handle);

    /// <inheritdoc cref="Marshal.GetDelegateForFunctionPointer(IntPtr, Type)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    Delegate NativeLibraryFunctionToDelegate(IntPtr ptr, Type t) => Marshal.GetDelegateForFunctionPointer(ptr, t);

    /// <inheritdoc cref="Marshal.PtrToStringUni(IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? NativeLibraryUnicodeStrPtrToString(IntPtr ptr) => Marshal.PtrToStringUni(ptr);

    /// <inheritdoc cref="Marshal.PtrToStringAnsi(IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? NativeLibraryAnsiStrPtrToString(IntPtr ptr) => Marshal.PtrToStringAnsi(ptr);

    /// <summary>
    ///     Sets the console title.
    /// </summary>
    /// <param name="title"></param>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void SetConsoleTitle(string title) => Console.Title = title;

    /// <summary>
    ///     Writes to out and flushes immediately.
    /// </summary>
    /// <param name="what">What to write.</param>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void OutAndFlush(string what)
    {
        Console.Out.Write(what);
        Console.Out.Flush();
    }

    /// <inheritdoc cref="Environment.GetEnvironmentVariable(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    /// <inheritdoc cref=" Directory.Exists(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool DirectoryExists(string name) => Directory.Exists(name);

    /// <inheritdoc cref=" Path.GetDirectoryName(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? GetDirectoryName(string path) => Path.GetDirectoryName(path);

    /// <inheritdoc cref=" Path.GetFileName(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? GetFileName(string path) => Path.GetFileName(path);

    /// <inheritdoc cref=" Path.Combine(string[])" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string CombinePaths(params string[] paths) => Path.Combine(paths);

    /// <inheritdoc cref=" Directory.EnumerateDirectories(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    IEnumerable<string> EnumerateDirectories(string directory) => Directory.EnumerateDirectories(directory);

    /// <inheritdoc cref=" Directory.EnumerateFiles(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    IEnumerable<string> EnumerateFiles(string directory) => Directory.EnumerateFiles(directory);

    /// <inheritdoc cref=" File.OpenText(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    TextReader OpenFileAsText(string path) => File.OpenText(path);

    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop implementation.")]
    private sealed class DotNetSystemAdapter: IDotNetSystemAdapter
    {
    }
}
