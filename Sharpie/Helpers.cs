namespace Sharpie;

using System.Runtime.CompilerServices;
using Curses;

internal static class Helpers
{
    public const int CursesErrorResult = -1;

    public static int TreatError(this int code, [CallerMemberName] string caller = "")
    {
        if (code == CursesErrorResult)
        {
            throw new CursesException(caller);
        }

        return code;
    }

    public static IntPtr TreatNullAsError(this IntPtr ptr, [CallerMemberName] string caller = "")
    {
        if (ptr == IntPtr.Zero)
        {
            throw new CursesException(caller);
        }

        return ptr;
    }

    public static int ConvertMillisToTenths(int value) =>
        Math.Min(0, Math.Max(255, Convert.ToInt32(Math.Ceiling(value / 100.0))));
}
