namespace Sharpie.Backend;

using System.Text.RegularExpressions;

/// <summary>
///     Provides functionality for obtaining <see cref="ICursesBackend" /> instances.
/// </summary>
[PublicAPI]
public static class CursesBackend
{
    /// <summary>
    ///     Lists the supported backends.
    /// </summary>
    [PublicAPI, Flags]
    public enum Provider
    {
        /// <summary>
        ///     The NCurses back-end.
        /// </summary>
        NCurses = 1,

        /// <summary>
        ///     The PDCurses back-end.
        /// </summary>
        PdCurses = 2,

        /// <summary>
        ///     The PDCursesMod with 32-bit WIDE support.
        /// </summary>
        PdCursesMod32Wide = 4,

        /// <summary>
        ///     Any supported backend.
        /// </summary>
        Any = NCurses | PdCurses | PdCursesMod32Wide
    }

    /// <summary>
    ///     Internal method that tries to load Curses backend, and, optionally libc.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="provider">The desired provider.</param>
    /// <param name="paths">The list of paths for the library.</param>
    /// <returns>The loaded backend or <c>null</c> if the load failed.</returns>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static BaseCursesBackend? TryLoad(IDotNetSystemAdapter dotNetSystemAdapter, Provider provider,
        string[] paths)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(paths != null);

        INativeSymbolResolver? sym = provider switch
        {
            Provider.NCurses => NativeLibraryWrapper<NCursesFunctionMap>.TryLoad(dotNetSystemAdapter, paths),
            Provider.PdCurses => NativeLibraryWrapper<PdCursesFunctionMap>.TryLoad(dotNetSystemAdapter, paths),
            Provider.PdCursesMod32Wide => NativeLibraryWrapper<PdCursesMod32FunctionMap>.TryLoad(dotNetSystemAdapter,
                paths),
            var _ => null
        };

        if (sym == null)
        {
            return null;
        }

        INativeSymbolResolver? cSym = dotNetSystemAdapter.IsUnixLike
            ? NativeLibraryWrapper<LibCFunctionMap>.TryLoad(dotNetSystemAdapter, new[] { "libc" })
            : null;

        return provider switch
        {
            Provider.PdCurses => new PdCursesBackend(dotNetSystemAdapter, sym, cSym),
            Provider.PdCursesMod32Wide => new PdCursesMod32Backend(dotNetSystemAdapter, sym, cSym),
            var _ => new NCursesBackend(dotNetSystemAdapter, sym, cSym)
        };
    }

    /// <summary>
    ///     Internal method that loads the Curses backend based on the specified <paramref name="providers" />.
    /// </summary>
    /// <param name="providers">The desired providers.</param>
    /// <param name="dotNetSystemAdapter">Adapter for .NET system.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="providers" /> is empty.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend Load(IDotNetSystemAdapter dotNetSystemAdapter, Provider providers)
    {
        ICursesBackend? result = null;

        if (providers.HasFlag(Provider.PdCursesMod32Wide))
        {
            if (dotNetSystemAdapter.IsLinux || dotNetSystemAdapter.IsFreeBsd)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCursesMod32Wide,
                    new[] { "libpdcurses2.so", "libpdcurses.so", "libXCurses.so" });
            } else if (dotNetSystemAdapter.IsMacOs)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCursesMod32Wide, new[] { "libpdcurses.dylib" });
            } else if (dotNetSystemAdapter.IsWindows)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCursesMod32Wide,
                    new[] { "libpdcurses.dll", "pdcurses.dll" });
            }
        }

        if (providers.HasFlag(Provider.PdCurses) && result == null)
        {
            if (dotNetSystemAdapter.IsLinux || dotNetSystemAdapter.IsFreeBsd)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCurses,
                    new[] { "libpdcurses2.so", "libpdcurses.so", "libXCurses.so" });
            } else if (dotNetSystemAdapter.IsMacOs)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCurses,
                    new[] { "libpdcurses.dylib", "libpdcurses2.dylib" });
            } else if (dotNetSystemAdapter.IsWindows)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.PdCurses, new[] { "libpdcurses.dll", "pdcurses.dll" });
            }
        }

        if (providers.HasFlag(Provider.NCurses) && result == null)
        {
            if (dotNetSystemAdapter.IsLinux || dotNetSystemAdapter.IsFreeBsd)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.NCurses,
                    new[]
                    {
                        "libncursesw.so.6",
                        "libncursesw.so.5",
                        "libncursesw.so",
                        "ncursesw",
                        "libncurses.so.6",
                        "libncurses.so.5",
                        "libncurses.so",
                        "ncurses"
                    });
            } else if (dotNetSystemAdapter.IsMacOs)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.NCurses,
                    FindMacOsNCursesCandidates(dotNetSystemAdapter));
            } else if (dotNetSystemAdapter.IsWindows)
            {
                result = TryLoad(dotNetSystemAdapter, Provider.NCurses,
                    new[]
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
                    });
            }
        }

        if (result == null)
        {
            throw new CursesInitializationException();
        }

        return result;
    }

    /// <summary>
    ///     Internal method that loads the Curses backend based on the specified <paramref name="providers" /> and
    ///     <paramref name="paths" />.
    /// </summary>
    /// <param name="providers">The desired providers.</param>
    /// <param name="dotNetSystemAdapter">Adapter for .NET system.</param>
    /// <param name="paths">The list of paths for the library.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="providers" /> is empty.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend Load(IDotNetSystemAdapter dotNetSystemAdapter, Provider providers,
        params string[] paths)
    {
        ICursesBackend? result = null;

        if (providers.HasFlag(Provider.PdCursesMod32Wide))
        {
            result = TryLoad(dotNetSystemAdapter, Provider.PdCursesMod32Wide, paths);
        }

        if (providers.HasFlag(Provider.PdCurses) && result == null)
        {
            result = TryLoad(dotNetSystemAdapter, Provider.PdCurses, paths);
        }

        if (providers.HasFlag(Provider.NCurses) && result == null)
        {
            result = TryLoad(dotNetSystemAdapter, Provider.NCurses, paths);
        }

        if (result == null)
        {
            throw new CursesInitializationException();
        }

        return result;
    }

    /// <summary>
    ///     Loads the Curses backend based on the specified <paramref name="providers" />.
    /// </summary>
    /// <param name="providers">The desired providers.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="providers" /> is empty.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend Load(Provider providers = Provider.Any) =>
        Load(IDotNetSystemAdapter.Instance, providers);

    /// <summary>
    ///     Loads the Curses backend based on the specified <paramref name="providers" /> and <paramref name="paths" />.
    /// </summary>
    /// <param name="providers">The desired providers.</param>
    /// <param name="paths">The list of paths for the library.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="providers" /> is empty.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend Load(Provider providers, params string[] paths) =>
        Load(IDotNetSystemAdapter.Instance, providers, paths);

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

    [SupportedOSPlatform("macos")]
    private static string[] FindMacOsNCursesCandidates(IDotNetSystemAdapter dotNetSystemAdapter)
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
            var ncursesPath = dotNetSystemAdapter.CombinePaths(homeBrewCellar, "ncurses");
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
                         .Concat(new[] { "ncursesw", "ncurses" })
                         .ToArray();
    }
}
