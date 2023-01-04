namespace Sharpie.Backend;

using System.Text.RegularExpressions;

/// <summary>
///     Provides functionality for obtaining <see cref="ICursesBackend" /> instances.
/// </summary>
[PublicAPI]
public static class CursesBackend
{
    private const string PdCursesPrefix = "pdcurses";
    private const string NCursesPrefix = "ncurses";
    private const string LibCPrefix = "libc";

    /// <summary>
    ///     Internal method that loads the NCurses backend from native libraries (and any other support library that is
    ///     required).
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="dotNetSystemAdapter" /> or
    ///     <paramref name="libPathResolver" /> are <c>null</c>.
    /// </exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend NCurses(IDotNetSystemAdapter dotNetSystemAdapter,
        Func<string, IEnumerable<string>> libPathResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(libPathResolver != null);

        var cw = NativeLibraryWrapper<NCursesFunctionMap>.TryLoad(dotNetSystemAdapter, libPathResolver(NCursesPrefix));
        if (cw == null)
        {
            throw new CursesInitializationException();
        }

        if (dotNetSystemAdapter.IsUnixLike)
        {
            var lw = NativeLibraryWrapper<LibCFunctionMap>.TryLoad(dotNetSystemAdapter, libPathResolver(LibCPrefix));
            if (lw == null)
            {
                throw new CursesInitializationException();
            }

            return new UnixNCursesBackend(dotNetSystemAdapter, cw, lw);
        }

        return new NCursesBackend(dotNetSystemAdapter, cw);
    }

    /// <summary>
    ///     Loads the NCurses backend from native libraries (and any other support library that is required).
    /// </summary>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="libPathResolver" /> is <c>null</c>.</exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows"),
     ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested.")]
    public static ICursesBackend NCurses(Func<string, IEnumerable<string>> libPathResolver)
    {
        if (libPathResolver == null)
        {
            throw new ArgumentNullException(nameof(libPathResolver));
        }

        return NCurses(IDotNetSystemAdapter.Instance, libPathResolver);
    }

    /// <summary>
    ///     Internal method that loads the NCurses backend from native libraries (and any other support library that is
    ///     required).
    ///     This method uses standard known names for the 'ncurses' and potentially 'libc' libraries.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend NCurses(IDotNetSystemAdapter dotNetSystemAdapter)
    {
        return NCurses(dotNetSystemAdapter, lib =>
        {
            return lib switch
            {
                NCursesPrefix when dotNetSystemAdapter.IsLinux || dotNetSystemAdapter.IsFreeBsd =>
                    FindLinuxAndFreeBsdNCursesCandidates(),
                NCursesPrefix when dotNetSystemAdapter.IsMacOs => FindMacOsNCursesCandidates(dotNetSystemAdapter),
                NCursesPrefix when dotNetSystemAdapter.IsWindows => FindWindowsNCursesCandidates(),
                var _ => new[] { lib }
            };
        });
    }

    /// <summary>
    ///     Loads the NCurses backend from native libraries (and any other support library that is required).
    ///     This method uses standard known names for the required libraries.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend NCurses() => NCurses(IDotNetSystemAdapter.Instance);

    private static IEnumerable<(string name, int version)> GetCandidatesInDirectory(
        IDotNetSystemAdapter dotNetSystemAdapter, string directory, Regex pattern)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(pattern != null);
        Debug.Assert(!string.IsNullOrEmpty(directory));

        return dotNetSystemAdapter.EnumerateFiles(directory)
                                  .Select(f => pattern.Match(f))
                                  .Where(m => m.Success)
                                  .Select(m => (name: dotNetSystemAdapter.CombinePaths(directory, m.Groups[0]
                                      .Value), version: int.Parse(m.Groups[1]
                                                                   .Value)));
    }

    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd")]
    private static IEnumerable<string> FindLinuxAndFreeBsdNCursesCandidates()
    {
        return new[]
        {
            "libncursesw.so.6",
            "libncursesw.so.5",
            "libncursesw.so",
            "ncursesw",
            "libncurses.so.6",
            "libncurses.so.5",
            "libncurses.so",
            NCursesPrefix
        };
    }

    [SupportedOSPlatform("windows")]
    private static IEnumerable<string> FindWindowsNCursesCandidates()
    {
        return new[]
        {
            "libncursesw.dll",
            "libncursesw6.dll",
            "libncursesw5.dll",
            "ncursesw.dll",
            "ncursesw6.dll",
            "ncursesw5.dll",
            "libncurses.dll",
            "libncurses6.dll",
            "libncurses5.dll",
            "ncurses.dll",
            "ncurses6.dll",
            "ncurses5.dll"
        };
    }

    [SupportedOSPlatform("macos")]
    private static IEnumerable<string> FindMacOsNCursesCandidates(IDotNetSystemAdapter dotNetSystemAdapter)
    {
        Debug.Assert(dotNetSystemAdapter != null);

        var homeBrewPrefix = dotNetSystemAdapter.GetEnvironmentVariable("HOMEBREW_PREFIX");
        var homeBrewCellar = dotNetSystemAdapter.GetEnvironmentVariable("HOMEBREW_CELLAR");

        var candidates = new List<(string name, int version)>();
        var matchRegEx = new Regex(@"libncurses\.(\d+)\.dylib", RegexOptions.Compiled);

        if (!string.IsNullOrEmpty(homeBrewPrefix))
        {
            var libPath = dotNetSystemAdapter.CombinePaths(homeBrewPrefix, "lib");
            if (dotNetSystemAdapter.DirectoryExists(libPath))
            {
                candidates.AddRange(GetCandidatesInDirectory(dotNetSystemAdapter, libPath, matchRegEx));
            }
        }

        if (!string.IsNullOrEmpty(homeBrewCellar))
        {
            var ncursesPath = dotNetSystemAdapter.CombinePaths(homeBrewCellar, NCursesPrefix);
            if (dotNetSystemAdapter.DirectoryExists(ncursesPath))
            {
                foreach (var v in dotNetSystemAdapter.EnumerateDirectories(ncursesPath))
                {
                    var libPath = dotNetSystemAdapter.CombinePaths(v, "lib");
                    if (dotNetSystemAdapter.DirectoryExists(libPath))
                    {
                        candidates.AddRange(GetCandidatesInDirectory(dotNetSystemAdapter, libPath, matchRegEx));
                    }
                }
            }
        }

        return candidates.OrderByDescending(c => c.version)
                         .Select(c => c.name)
                         .Concat(new[] { NCursesPrefix });
    }

    /// <summary>
    ///     Internal method that loads the PDCurses backend from native library.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="dotNetSystemAdapter" /> or
    ///     <paramref name="libPathResolver" /> are <c>null</c>.
    /// </exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend PdCurses(IDotNetSystemAdapter dotNetSystemAdapter,
        Func<string, IEnumerable<string>> libPathResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(libPathResolver != null);

        var cw = NativeLibraryWrapper<PdCursesMod32FunctionMap>.TryLoad(dotNetSystemAdapter,
            libPathResolver(PdCursesPrefix));

        if (cw == null)
        {
            throw new CursesInitializationException();
        }

        return new PdCursesMod32Backend(dotNetSystemAdapter, cw);
    }

    /// <summary>
    ///     Loads the PDCurses backend from native library.
    /// </summary>
    /// <param name="libPathResolver">Function that provides paths/names for the native loader.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="libPathResolver" /> is <c>null</c>.</exception>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend PdCurses(Func<string, IEnumerable<string>> libPathResolver)
    {
        if (libPathResolver == null)
        {
            throw new ArgumentNullException(nameof(libPathResolver));
        }

        return PdCurses(IDotNetSystemAdapter.Instance, libPathResolver);
    }

    /// <summary>
    ///     Internal method that loads the PDCurses backend from native library.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend PdCurses(IDotNetSystemAdapter dotNetSystemAdapter)
    {
        return PdCurses(dotNetSystemAdapter, lib =>
        {
            return lib switch
            {
                PdCursesPrefix when dotNetSystemAdapter.IsLinux || dotNetSystemAdapter.IsFreeBsd =>
                    FindLinuxAndFreeBsdPdCursesCandidates(),
                PdCursesPrefix when dotNetSystemAdapter.IsWindows => FindWindowsPdCursesCandidates(),
                var _ => new[] { lib }
            };
        });
    }

    /// <summary>
    ///     Loads the Curses backend from native libraries (and any other support library that is required).
    ///     This method uses standard known names for the required libraries.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable library was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend PdCurses() => PdCurses(IDotNetSystemAdapter.Instance);

    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd")]
    private static IEnumerable<string> FindLinuxAndFreeBsdPdCursesCandidates()
    {
        return new[] { "libpdcurses2.so", "libpdcurses.so", "libXCurses.so", PdCursesPrefix };
    }

    [SupportedOSPlatform("windows")]
    private static IEnumerable<string> FindWindowsPdCursesCandidates()
    {
        return new[] { "libpdcurses.dll", "pdcurses.dll" };
    }
}
