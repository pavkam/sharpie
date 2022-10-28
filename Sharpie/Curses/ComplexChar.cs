namespace Sharpie.Curses;
using System.Runtime.InteropServices;

/// <summary>
/// Opaque Curses character with attributes and color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ComplexChar
{
    [MarshalAs(UnmanagedType.U4)] private readonly uint attrAndColorPair;
    [MarshalAs(UnmanagedType.U4)] private readonly uint char0;
    [MarshalAs(UnmanagedType.U4)] private readonly uint char1;
    [MarshalAs(UnmanagedType.U4)] private readonly uint char2;
    [MarshalAs(UnmanagedType.U4)] private readonly uint char3;
    [MarshalAs(UnmanagedType.U4)] private readonly uint char4;
}
