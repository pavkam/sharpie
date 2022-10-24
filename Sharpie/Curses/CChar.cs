namespace Sharpie.Curses;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct CChar
{
    [MarshalAs(UnmanagedType.U4)] public uint attrAndColorPair;
    [MarshalAs(UnmanagedType.U2)] public char char0;
    [MarshalAs(UnmanagedType.U2)] public char char1;
    [MarshalAs(UnmanagedType.U2)] public char chars2;
    [MarshalAs(UnmanagedType.U2)] public char chars3;
    [MarshalAs(UnmanagedType.U2)] public char chars4;
    [MarshalAs(UnmanagedType.I4)] public int extColor;
}
