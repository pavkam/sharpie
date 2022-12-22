namespace Sharpie.Abstractions;

using System.Reflection;
using System.Runtime.Versioning;

/// <summary>
/// An internal interface used to help test the functionality or native library loader.
/// </summary>

internal interface IDotNetSystemAdapter
{
    private sealed class DotNetSystemAdapter: IDotNetSystemAdapter
    {
    }

    /// <summary>
    /// The actual instance that connects this interface to the .NET runtime.
    /// </summary>
    public static IDotNetSystemAdapter Instance = new DotNetSystemAdapter();

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryLoad(string,Assembly,DllImportSearchPath?,out System.IntPtr)"/>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath,
        out IntPtr handle) =>
        NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryLoad(string,out System.IntPtr)"/>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryLoadNativeLibrary(string libraryPath, out IntPtr handle) => NativeLibrary.TryLoad(libraryPath, out handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryGetExport(IntPtr,string,out System.IntPtr)"/>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool TryGetNativeLibraryExport(IntPtr handle, string name, out IntPtr address) =>
        NativeLibrary.TryGetExport(handle, name, out address);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.Free(IntPtr)"/>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void FreeNativeLibrary(IntPtr handle) => NativeLibrary.Free(handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(IntPtr, Type)"/>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    Delegate NativeLibraryFunctionToDelegate(IntPtr ptr, Type t) => Marshal.GetDelegateForFunctionPointer(ptr, t);

    /// <summary>
    /// Checks if the operating system is Linux.
    /// </summary>
    /// <returns><c>true</c> if the operating system is Linux; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("linux"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsLinux => OperatingSystem.IsLinux();

    /// <summary>
    /// Checks if the operating system is FreeBSD.
    /// </summary>
    /// <returns><c>true</c> if the operating system is FreeBSD; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("freebsd"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsFreeBsd => OperatingSystem.IsFreeBSD();

    /// <summary>
    /// Checks if the operating system is MacOS.
    /// </summary>
    /// <returns><c>true</c> if the operating system is MacOS; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("macos"), ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsMacOs => OperatingSystem.IsMacOS();

    /// <summary>
    /// Checks if the operating system is Unix-like.
    /// </summary>
    /// <returns><c>true</c> if the operating system is Unix-like; <c>false</c> otherwise.</returns>
    [SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("macos"),
     ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    bool IsUnixLike => IsLinux || IsFreeBsd || IsMacOs;

    /// <summary>
    /// Sets the console title.
    /// </summary>
    /// <param name="title"></param>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void SetConsoleTitle(string title) => Console.Title = title;

    /// <summary>
    /// Writes to out and flushes immediately.
    /// </summary>
    /// <param name="what">What to write.</param>
    [ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    void OutAndFlush(string what)
    {
        Console.Out.Write(what);
        Console.Out.Flush();
    }

    /// <inheritdoc cref="System.Runtime.InteropServices.PosixSignalRegistration.Create"/>
    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"), SupportedOSPlatform("macos"),
     ExcludeFromCodeCoverage(Justification = ".NET runtime interop method.")]
    IDisposable MonitorTerminalResizeSignal(Action action) =>
        PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ => { action(); });
}
