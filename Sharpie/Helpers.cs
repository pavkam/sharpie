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

    public static void AssertNotDisposed(this ICursesProvider cursesProvider)
    {
        if (cursesProvider == null)
        {
            throw new ArgumentNullException(nameof(cursesProvider));
        }

        if (cursesProvider.isendwin())
        {
            throw new ObjectDisposedException("The terminal has been disposed and no further operations are allowed.");
        }
    }

    public static int ConvertMillisToTenths(int value) =>
        Math.Min(0, Math.Max(255, Convert.ToInt32(Math.Ceiling(value / 100.0))));
}
