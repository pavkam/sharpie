#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Reflection;

[SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class NativeLibrary: IDisposable
{
    private sealed class SystemInteropAdapter: ISystemInteropAdapter
    {
        public bool
            TryLoad(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out IntPtr handle) =>
            System.Runtime.InteropServices.NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle);

        public bool TryGetExport(IntPtr handle, string name, out IntPtr address) =>
            System.Runtime.InteropServices.NativeLibrary.TryGetExport(handle, name, out address);

        public void Free(IntPtr handle) => System.Runtime.InteropServices.NativeLibrary.Free(handle);

        public Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t) =>
            Marshal.GetDelegateForFunctionPointer(ptr, t);
    }

    private static readonly SystemInteropAdapter _adapter = new();

    private readonly ISystemInteropAdapter _interopAdapter;
    private IntPtr _libraryHandle;

    private NativeLibrary(ISystemInteropAdapter interopAdapter, IntPtr libraryHandle)
    {
        Debug.Assert(interopAdapter != null);
        Debug.Assert(libraryHandle != IntPtr.Zero);
        
        _interopAdapter = interopAdapter;
        _libraryHandle = libraryHandle;
    }

    internal static NativeLibrary? TryLoad(ISystemInteropAdapter interopAdapter, string libraryName, DllImportSearchPath? searchPath)
    {
        if (interopAdapter == null)
        {
            throw new ArgumentNullException(nameof(interopAdapter));
        }

        if (interopAdapter.TryLoad(libraryName, Assembly.GetCallingAssembly(), searchPath,
                out var libHandle))
        {
            return new(interopAdapter, libHandle);
        }

        return null;
    }

    public static NativeLibrary? TryLoad(string libraryName, DllImportSearchPath? searchPath) =>
        TryLoad(_adapter, libraryName, searchPath);

    protected Delegate GetExportedMethod(string name, Type delegateType)
    {
        if (_libraryHandle == IntPtr.Zero)
        {
            throw new ObjectDisposedException("This library has been unloaded.");
        }

        if (_interopAdapter.TryGetExport(_libraryHandle, name, out var handle))
        {
            return Marshal.GetDelegateForFunctionPointer(handle, delegateType);
        }

        throw new MissingMethodException($"Could not find {name} within the library.");
    }

    public void Dispose()
    {
        if (_libraryHandle != IntPtr.Zero)
        {
            var h = Interlocked.Exchange(ref _libraryHandle, IntPtr.Zero);
            if (h != IntPtr.Zero)
            {
                _interopAdapter.Free(_libraryHandle);
                GC.SuppressFinalize(this);
            }
        }
    }

    ~NativeLibrary()
    {
        Dispose();
    }
}
