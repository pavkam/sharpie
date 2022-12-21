#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Reflection;

[PublicAPI]
internal sealed class NativeLibraryWrapper<TFunctions>: INativeSymbolResolver, IDisposable
{
    private sealed class SystemInteropAdapter: ISystemInteropAdapter
    {
        public bool TryLoad(string libraryName, Assembly assembly, DllImportSearchPath? searchPath,
            out IntPtr handle) =>
            NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle);

        public bool TryLoad(string libraryPath, out IntPtr handle) => NativeLibrary.TryLoad(libraryPath, out handle);

        public bool TryGetExport(IntPtr handle, string name, out IntPtr address) =>
            NativeLibrary.TryGetExport(handle, name, out address);

        public void Free(IntPtr handle) => NativeLibrary.Free(handle);

        public Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t) =>
            Marshal.GetDelegateForFunctionPointer(ptr, t);
    }

    private static readonly SystemInteropAdapter Adapter = new();

    private readonly ISystemInteropAdapter _interopAdapter;
    private IntPtr _libraryHandle;
    private readonly IReadOnlyDictionary<Type, Delegate> _methodTable;

    private NativeLibraryWrapper(ISystemInteropAdapter interopAdapter, IntPtr libraryHandle)
    {
        Debug.Assert(interopAdapter != null);
        Debug.Assert(libraryHandle != IntPtr.Zero);

        _interopAdapter = interopAdapter;
        _libraryHandle = libraryHandle;

        _methodTable = GetExportedMethodTable();
    }

    private static NativeLibraryWrapper<TFunctions>? TryLoad(ISystemInteropAdapter interopAdapter,
        string libraryNameOrPath)
    {
        if (interopAdapter == null)
        {
            throw new ArgumentNullException(nameof(interopAdapter));
        }

        if (string.IsNullOrEmpty(Path.GetDirectoryName(libraryNameOrPath)) &&
            interopAdapter.TryLoad(libraryNameOrPath, Assembly.GetCallingAssembly(), null, out var libHandle) ||
            interopAdapter.TryLoad(libraryNameOrPath, out libHandle))
        {
            return new(interopAdapter, libHandle);
        }

        return null;
    }

    internal static NativeLibraryWrapper<TFunctions>? TryLoad(ISystemInteropAdapter interopAdapter,
        IEnumerable<string> libraryNameOrPaths)
    {
        if (interopAdapter == null)
        {
            throw new ArgumentNullException(nameof(interopAdapter));
        }

        if (libraryNameOrPaths == null)
        {
            throw new ArgumentNullException(nameof(libraryNameOrPaths));
        }

        return libraryNameOrPaths.Select(p => TryLoad(interopAdapter, p))
                                 .FirstOrDefault(r => r != null);
    }

    public static NativeLibraryWrapper<TFunctions>? TryLoad(IEnumerable<string> libraryNameOrPaths) =>
        TryLoad(Adapter, libraryNameOrPaths);

    private Delegate GetExportedMethod(string name, Type delegateType)
    {
        if (_libraryHandle == IntPtr.Zero)
        {
            throw new ObjectDisposedException("This library has been unloaded.");
        }

        if (_interopAdapter.TryGetExport(_libraryHandle, name, out var handle))
        {
            return _interopAdapter.GetDelegateForFunctionPointer(handle, delegateType);
        }

        throw new MissingMethodException($"Could not find {name} within the library.");
    }

    private static IEnumerable<TypeInfo> GetRequiredDelegates()
    {
        return typeof(TFunctions).GetTypeInfo()
                                 .DeclaredMembers.Where(m => m.MemberType == MemberTypes.NestedType)
                                 .Select(s => (TypeInfo) s)
                                 .Where(t => !t.IsGenericType && t.BaseType == typeof(MulticastDelegate) && 
                                     t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>() != null)
                                 .ToArray();
    }

    private IReadOnlyDictionary<Type, Delegate> GetExportedMethodTable()
    {
        return GetRequiredDelegates()
            .ToDictionary(import => import.AsType(), import => GetExportedMethod(import.Name, import));
    }

    public TDelegate Resolve<TDelegate>() where TDelegate: MulticastDelegate
    {
        if (!_methodTable.TryGetValue(typeof(TDelegate), out var r))
        {
            throw new MissingMethodException($"The function of type {typeof(TDelegate).Name} is has not been loaded.");
        }

        return (TDelegate) r;
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

    ~NativeLibraryWrapper() { Dispose(); }
}
