/*
Copyright (c) 2022-2025, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Sharpie.Backend;

/// <summary>
///     Provides functionality for obtaining <see cref="ICursesBackend" /> instances.
/// </summary>
[PublicAPI]
public static class CursesBackend
{
    private static readonly string[] _libraryNameOrPaths = ["libc"];

    /// <summary>
    ///     Internal method that tries to load Curses backend, and, optionally libc.
    /// </summary>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="type">The backend type.</param>
    /// <param name="paths">The list of paths for the library.</param>
    /// <returns>The loaded backend or <c>null</c> if the load failed.</returns>
    internal static BaseCursesBackend? TryLoad(IDotNetSystemAdapter dotNetSystemAdapter, CursesBackendType type,
        IEnumerable<string> paths)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(paths != null);

        INativeSymbolResolver? sym = type switch
        {
            CursesBackendType.NCurses => NativeLibraryWrapper<NCursesFunctionMap>.TryLoad(dotNetSystemAdapter, paths),
            CursesBackendType.PdCurses => NativeLibraryWrapper<PdCursesFunctionMap>.TryLoad(dotNetSystemAdapter, paths),
            CursesBackendType.PdCursesMod => NativeLibraryWrapper<PdCursesMod32FunctionMap>.TryLoad(dotNetSystemAdapter,
                paths),
            var _ => null
        };

        if (sym == null)
        {
            return null;
        }

        INativeSymbolResolver? cSym = dotNetSystemAdapter.IsUnixLike
            ? NativeLibraryWrapper<LibCFunctionMap>.TryLoad(dotNetSystemAdapter, _libraryNameOrPaths)
            : null;
#pragma warning disable IDE0072 // Add missing cases -- this is intentional
        return type switch
        {
            CursesBackendType.PdCurses => new PdCursesBackend(dotNetSystemAdapter, sym, cSym),
            CursesBackendType.PdCursesMod => new PdCursesMod32Backend(dotNetSystemAdapter, sym, cSym),
            var _ => new NCursesBackend(dotNetSystemAdapter, sym, cSym)
        };
#pragma warning restore IDE0072 // Add missing cases
    }

    /// <summary>
    ///     Looks up and loads the Curses backend based on the specified <paramref name="flavors" />.
    /// </summary>
    /// <param name="flavors">The desired Curses flavors.</param>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    [SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    internal static ICursesBackend Load(IDotNetSystemAdapter dotNetSystemAdapter,
        IEnumerable<CursesBackendFlavor> flavors)
    {
        Debug.Assert(flavors != null);

        if (dotNetSystemAdapter is { IsLinux: false, IsFreeBsd: false, IsMacOs: false, IsWindows: false })
        {
            throw new PlatformNotSupportedException("Current platform is not supported.");
        }

        var selector = new CursesBackendFlavorSelector(dotNetSystemAdapter);
        foreach (var f in flavors)
        {
            foreach (var (path, type) in selector.GetLibraryPaths(f))
            {
                var backend = TryLoad(dotNetSystemAdapter, type, new[] { path });
                if (backend != null)
                {
                    return backend;
                }
            }
        }

        throw new CursesInitializationException();
    }

    /// <summary>
    ///     Looks up and loads the Curses backend based on the specified <paramref name="flavor" />.
    /// </summary>
    /// <param name="flavor">The desired Curses flavor.</param>
    /// <param name="otherFlavors">Additional desired Curses flavors.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested."),
     SupportedOSPlatform("macos"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
     SupportedOSPlatform("windows")]
    public static ICursesBackend Load(CursesBackendFlavor flavor = CursesBackendFlavor.Any,
        params CursesBackendFlavor[] otherFlavors)
    {
        return otherFlavors == null
            ? throw new ArgumentNullException(nameof(otherFlavors))
            : Load(IDotNetSystemAdapter.Instance, new[] { flavor }.Concat(otherFlavors));
    }

    /// <summary>
    ///     Loads the Curses backend based on the specified <paramref name="type" />.
    /// </summary>
    /// <param name="type">The backend type.</param>
    /// <param name="dotNetSystemAdapter">Adapter for .NET functionality.</param>
    /// <param name="paths">The list of paths for the library.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    internal static ICursesBackend Load(IDotNetSystemAdapter dotNetSystemAdapter, CursesBackendType type,
        IEnumerable<string> paths)
    {
        var res = TryLoad(dotNetSystemAdapter, type, paths);
        return res == null ? throw new CursesInitializationException() : (ICursesBackend) res;
    }

    /// <summary>
    ///     Loads the Curses backend based on the specified <paramref name="type" />.
    /// </summary>
    /// <param name="type">The backend type.</param>
    /// <param name="path">The path for the library.</param>
    /// <param name="otherPaths">The list of additional paths for the library.</param>
    /// <returns>The loaded Curses backend.</returns>
    /// <exception cref="CursesInitializationException">Thrown if no suitable provider was found.</exception>
    [ExcludeFromCodeCoverage(Justification = "References a singleton .NET object and cannot be tested.")]
    public static ICursesBackend Load(CursesBackendType type, string path, params string[] otherPaths)
    {
        return path == null
            ? throw new ArgumentNullException(nameof(path))
            : otherPaths == null
            ? throw new ArgumentNullException(nameof(otherPaths))
            : Load(IDotNetSystemAdapter.Instance, type, new[] { path }.Concat(otherPaths));
    }
}
