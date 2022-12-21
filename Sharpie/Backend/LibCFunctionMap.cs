#pragma warning disable CS1591

namespace Sharpie.Backend;

[SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "InconsistentNaming")]
internal abstract class LibCFunctionMap
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int setlocale(int cate, [MarshalAs(UnmanagedType.LPStr)] string locale);
}
