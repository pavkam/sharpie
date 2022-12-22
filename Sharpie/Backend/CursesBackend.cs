namespace Sharpie.Backend;

/// <summary>
/// Provides functionality for obtaining <see cref="ICursesBackend"/> instances.
/// </summary>
[PublicAPI]
public static class CursesBackend
{
    /// <summary>
    /// Internal method that loads the Curses backend from native libraries (and any other support library that is required).
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dotNetSystemAdapter"/> or <paramref name="libPathResolver"/> are <c>null</c>.</exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    internal static ICursesBackend NCurses(IDotNetSystemAdapter dotNetSystemAdapter,
        Func<string, IEnumerable<string>> libPathResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(libPathResolver != null);

        var cw = NativeLibraryWrapper<NCursesFunctionMap>.TryLoad(dotNetSystemAdapter, libPathResolver("ncurses"));
        if (cw == null)
        {
            throw new CursesInitializationException();
        }

        if (dotNetSystemAdapter.IsUnixLike)
        {
            var lw = NativeLibraryWrapper<LibCFunctionMap>.TryLoad(dotNetSystemAdapter, libPathResolver("libc"));
            if (lw == null)
            {
                throw new CursesInitializationException();
            }

            return new UnixNCursesBackend(dotNetSystemAdapter, cw, lw);
        }

        return new NCursesBackend(dotNetSystemAdapter, cw);
    }

    /// <summary>
    /// Loads the Curses backend from native libraries (and any other support library that is required).
    /// </summary>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="libPathResolver"/> is <c>null</c>.</exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested.")]
    public static ICursesBackend NCurses(Func<string, IEnumerable<string>> libPathResolver)
    {
        if (libPathResolver == null)
        {
            throw new ArgumentNullException(nameof(libPathResolver));
        }

        return NCurses(IDotNetSystemAdapter.Instance, libPathResolver);
    }
       

    /// <summary>
    /// Internal method that loads the Curses backend from native libraries (and any other support library that is required).
    /// This method uses standard known names for the 'ncurses' and potentially 'libc' libraries.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    internal static ICursesBackend NCurses(IDotNetSystemAdapter dotNetSystemAdapter)
    {
        return NCurses(dotNetSystemAdapter, s => { return new[] { s }; });
    }

    /// <summary>
    /// Loads the Curses backend from native libraries (and any other support library that is required).
    /// This method uses standard known names for the required libraries.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested.")]
    public static ICursesBackend NCurses() => NCurses(IDotNetSystemAdapter.Instance);
}
