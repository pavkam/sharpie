namespace Sharpie;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct MouseEvent
{
    [MarshalAs(UnmanagedType.I2)] public short id;
    [MarshalAs(UnmanagedType.I4)] public int x;
    [MarshalAs(UnmanagedType.I4)] public int y;
    [MarshalAs(UnmanagedType.I4)] public int z;
    [MarshalAs(UnmanagedType.I8)] public ulong bstate;
}
