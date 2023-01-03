namespace Sharpie.Backend;

/// <summary>
///     Opaque Curses character with attributes and color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct NCursesComplexChar
{
    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _attrAndColorPair;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char0;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char1;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char2;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char3;

    [MarshalAs(UnmanagedType.U4), UsedImplicitly]
    internal uint _char4;

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() =>
        $"{_attrAndColorPair:X8}-{_char0:X8}:{_char1:X8}:{_char2:X8}:{_char3:X8}:{_char4:X8}";
}
