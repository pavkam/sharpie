namespace Sharpie.Abstractions;

using System.Reflection;

/// <summary>
/// An internal interface used to help test the functionality or native library loader.
/// </summary>
internal interface ISystemInteropAdapter
{
    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryLoad(string,Assembly,DllImportSearchPath?,out System.IntPtr)"/>
    bool TryLoad(string libraryName, Assembly assembly, DllImportSearchPath? searchPath,
        out IntPtr handle);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.TryGetExport(IntPtr,string,out System.IntPtr)"/>
   bool TryGetExport(IntPtr handle, string name, out IntPtr address);

    /// <inheritdoc cref="System.Runtime.InteropServices.NativeLibrary.Free(IntPtr)"/>
   void Free(IntPtr handle);
   
    /// <inheritdoc cref="System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(IntPtr, Type)"/>
   Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t);
}
