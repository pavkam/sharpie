namespace Sharpie.Backend;


/// <summary>
///     Opaque Curses character with attributes and color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal class NCursesComplexChar
{
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _attrAndColorPair;
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _char0;
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _char1;
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _char2;
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _char3;
    [MarshalAs(UnmanagedType.U4), UsedImplicitly] internal uint _char4;
}
