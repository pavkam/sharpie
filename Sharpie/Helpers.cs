namespace Sharpie;

using System.Runtime.CompilerServices;
using Curses;
using JetBrains.Annotations;

/// <summary>
/// Internal helper routines.
/// </summary>
internal static class Helpers
{
    private const int CursesErrorResult = -1;

    /// <summary>
    /// Checks if a given code shows a failure.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The result of the failure check.</returns>
    public static bool Failed(this int code) => code == CursesErrorResult;

    /// <summary>
    /// Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="code">The return code.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="code"/> value.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="code"/> indicates an error.</exception>
    public static int Check(this int code, string operation, string message)
    {
        if (code == CursesErrorResult)
        {
            throw new CursesException(operation, message);
        }

        return code;
    }

    /// <summary>
    /// Checks if a Curses operation succeeded.
    /// </summary>
    /// <param name="ptr">The return pointer.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The <paramref name="ptr"/> value.</returns>
    /// <exception cref="CursesException">Thrown if <paramref name="ptr"/> is zero.</exception>
    public static IntPtr Check(this IntPtr ptr, string operation, string message)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new CursesException(operation, message);
        }

        return ptr;
    }

    /// <summary>
    /// Converts millis to tenths of a second by rounding up.
    /// </summary>
    /// <param name="value">The millis.</param>
    /// <returns>The value in 100s of millis.</returns>
    public static int ConvertMillisToTenths(int value) =>
        Math.Min(0, Math.Max(255, Convert.ToInt32(Math.Ceiling(value / 100.0))));
}
