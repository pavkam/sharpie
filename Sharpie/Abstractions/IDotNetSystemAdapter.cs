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
    ///     cref="System.Runtime.InteropServices.NativeLibrary.TryLoad(string,Assembly,DllImportSearchPath?,out System.IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath,
        out IntPtr handle) =>
        NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryLoad(string,out System.IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryPath, out IntPtr handle) => NativeLibrary.TryLoad(libraryPath, out handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryGetExport(IntPtr,string,out System.IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryGetNativeLibraryExport(IntPtr handle, string name, out IntPtr address) =>
        NativeLibrary.TryGetExport(handle, name, out address);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.Free(IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void FreeNativeLibrary(IntPtr handle) => NativeLibrary.Free(handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(IntPtr, Type)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    Delegate NativeLibraryFunctionToDelegate(IntPtr ptr, Type t) => Marshal.GetDelegateForFunctionPointer(ptr, t);

    /// <inheritdoc cref="System.Runtime.InteropServices.Marshal.PtrToStringUni(IntPtr)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? NativeLibraryUnicodeStrPtrToString(IntPtr ptr) => Marshal.PtrToStringUni(ptr);

    /// <inheritdoc cref="System.Runtime.InteropServices.Marshal.PtrToStringAnsi(IntPtr)" />
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

    /// <inheritdoc cref="System.Environment.GetEnvironmentVariable(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    /// <inheritdoc cref=" System.IO.Directory.Exists(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool DirectoryExists(string name) => Directory.Exists(name);

    /// <inheritdoc cref=" System.IO.Path.GetDirectoryName(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string? GetDirectoryName(string path) => Path.GetDirectoryName(path);

    /// <inheritdoc cref=" System.IO.Path.Combine(string[])" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    string CombinePaths(params string[] paths) => Path.Combine(paths);

    /// <inheritdoc cref=" System.IO.Directory.EnumerateDirectories(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    IEnumerable<string> EnumerateDirectories(string directory) => Directory.EnumerateDirectories(directory);

    /// <inheritdoc cref=" System.IO.Directory.EnumerateFiles(string)" />
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    IEnumerable<string> EnumerateFiles(string directory) => Directory.EnumerateFiles(directory);

    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop implementation.")]
    private sealed class DotNetSystemAdapter: IDotNetSystemAdapter
    {
    }
}
