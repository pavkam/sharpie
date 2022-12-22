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

namespace Sharpie.Backend;

/// <summary>
///     Provides functionality to load native libraries and quickly extract the needed symbols.
/// </summary>
/// <typeparam name="TFunctions">
///     The type of class that contains delegate definitions for extracted symbols. Each delegate inside this class
///     represents a function in the library and is expected to have the same name.
/// </typeparam>
[PublicAPI]
internal sealed class NativeLibraryWrapper<TFunctions>: INativeSymbolResolver, IDisposable
{
    private readonly IDotNetSystemAdapter _dotNetSystemAdapter;
    private readonly IReadOnlyDictionary<Type, Delegate> _methodTable;
    private IntPtr _libraryHandle;

    /// <summary>
    ///     Creates a new instance of this class and extracts all required symbols.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="libraryHandle">The handle of the native library.</param>
    /// <exception cref="MissingMethodException">Thrown if one or more function are not found in the library.</exception>
    internal NativeLibraryWrapper(IDotNetSystemAdapter dotNetSystemAdapter, IntPtr libraryHandle)
    {
        Debug.Assert(dotNetSystemAdapter != null);

        _dotNetSystemAdapter = dotNetSystemAdapter;
        _libraryHandle = libraryHandle;

        _methodTable = GetExportedMethodTable();
    }

    /// <summary>
    ///     Disposes of the library and unloads it.
    /// </summary>
    public void Dispose()
    {
        if (_libraryHandle != IntPtr.Zero)
        {
            var h = Interlocked.Exchange(ref _libraryHandle, IntPtr.Zero);
            if (h != IntPtr.Zero)
            {
                _dotNetSystemAdapter.FreeNativeLibrary(h);
                GC.SuppressFinalize(this);
            }
        }
    }

    /// <summary>
    ///     Tries to resolve a given delegate type to the actual function exported by the library.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the function to resolve.</typeparam>
    /// <returns></returns>
    /// <exception cref="MissingMethodException">Thrown if the requested function is not loaded from the library.</exception>
    public TDelegate Resolve<TDelegate>() where TDelegate: MulticastDelegate
    {
        if (!_methodTable.TryGetValue(typeof(TDelegate), out var r))
        {
            throw new MissingMethodException($"The function of type {typeof(TDelegate).Name} is has not been loaded.");
        }

        return (TDelegate) r;
    }

    private static NativeLibraryWrapper<TFunctions>? TryLoad(IDotNetSystemAdapter dotNetSystemAdapter,
        string libraryNameOrPath)
    {
        Debug.Assert(dotNetSystemAdapter != null);

        if (string.IsNullOrEmpty(Path.GetDirectoryName(libraryNameOrPath)) &&
            dotNetSystemAdapter.TryLoadNativeLibrary(libraryNameOrPath, Assembly.GetCallingAssembly(), null, out var libHandle) ||
            dotNetSystemAdapter.TryLoadNativeLibrary(libraryNameOrPath, out libHandle))
        {
            return new(dotNetSystemAdapter, libHandle);
        }

        return null;
    }

    /// <summary>
    ///     Tries to load a library using a list of potential candidate locations.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="libraryNameOrPaths">The list of library name/paths to try.</param>
    /// <returns><c>null</c> is returned if the library could not be loaded; <c>true</c> otherwise.</returns>
    public static NativeLibraryWrapper<TFunctions>? TryLoad(IDotNetSystemAdapter dotNetSystemAdapter,
        IEnumerable<string> libraryNameOrPaths)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(libraryNameOrPaths != null);

        return libraryNameOrPaths.Select(p => TryLoad(dotNetSystemAdapter, p))
                                 .FirstOrDefault(r => r != null);
    }

    private Delegate GetExportedMethod(string name, Type delegateType)
    {
        if (_libraryHandle == IntPtr.Zero)
        {
            throw new ObjectDisposedException("This library has been unloaded.");
        }

        if (_dotNetSystemAdapter.TryGetNativeLibraryExport(_libraryHandle, name, out var handle))
        {
            return _dotNetSystemAdapter.NativeLibraryFunctionToDelegate(handle, delegateType);
        }

        throw new MissingMethodException($"Could not find {name} within the library.");
    }

    private static IEnumerable<TypeInfo> GetRequiredDelegates()
    {
        return typeof(TFunctions).GetTypeInfo()
                                 .DeclaredMembers.Where(m => m.MemberType == MemberTypes.NestedType)
                                 .Select(s => (TypeInfo) s)
                                 .Where(t => !t.IsGenericType &&
                                     t.BaseType == typeof(MulticastDelegate) &&
                                     t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>() != null)
                                 .ToArray();
    }

    private IReadOnlyDictionary<Type, Delegate> GetExportedMethodTable()
    {
        return GetRequiredDelegates()
            .ToDictionary(import => import.AsType(), import => GetExportedMethod(import.Name, import));
    }

    /// <summary>
    ///     Calls the <see cref="Dispose" /> method.
    /// </summary>
    ~NativeLibraryWrapper() { Dispose(); }
}
