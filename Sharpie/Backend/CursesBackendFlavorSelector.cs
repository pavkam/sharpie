/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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

using System.Text.RegularExpressions;

/// <summary>
///     Allows for obtaining paths for given Curses backend types. This is an internal
///     class.
/// </summary>
[SupportedOSPlatform("windows"), SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"),
 SupportedOSPlatform("macos")]
internal class CursesBackendFlavorSelector
{
    private readonly IDotNetSystemAdapter _dotNetSystemAdapter;

    public CursesBackendFlavorSelector(IDotNetSystemAdapter dotNetSystemAdapter)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        _dotNetSystemAdapter = dotNetSystemAdapter;
    }

    private string PlatformDllName(string name, int ver = 0)
    {
        if (_dotNetSystemAdapter.IsLinux || _dotNetSystemAdapter.IsFreeBsd)
        {
            var v1 = ver > 0 ? $".{ver}" : string.Empty;
            return $"lib{name}.so{v1}";
        }

        if (_dotNetSystemAdapter.IsMacOs)
        {
            var v2 = ver > 0 ? ver.ToString() : string.Empty;
            return $"lib{name}{v2}.dylib";
        }

        var v3 = ver > 0 ? ver.ToString() : string.Empty;
        return $"{name}{v3}.dll";
    }

    public IEnumerable<(string path, CursesBackendType type)> GetLibraryPaths(CursesBackendFlavor flavor)
    {
        if (_dotNetSystemAdapter is { IsLinux: false, IsFreeBsd: false, IsMacOs: false, IsWindows: false })
        {
            throw new PlatformNotSupportedException("Current platform is not supported.");
        }

        return flavor switch
        {
            CursesBackendFlavor.PdCursesModVirtualTerminal => new[]
            {
                PlatformDllName("pdcursesmod-vt"), PlatformDllName("pdcursesmod")
            }.Select(p => (p, CursesBackendType.PdCursesMod)),
            CursesBackendFlavor.PdCursesModGui when _dotNetSystemAdapter.IsWindows => new[]
            {
                PlatformDllName("pdcursesmod-wingui")
            }.Select(p => (p, CursesBackendType.PdCursesMod)),
            CursesBackendFlavor.PdCursesModGui => new[]
            {
                PlatformDllName("pdcursesmod-sdl1"), PlatformDllName("pdcursesmod-sdl2")
            }.Select(p => (p, CursesBackendType.PdCursesMod)),
            CursesBackendFlavor.PdCursesModWindowsConsole when _dotNetSystemAdapter.IsWindows => new[]
            {
                PlatformDllName("pdcursesmod-wincon")
            }.Select(p => (p, CursesBackendType.PdCursesMod)),
            CursesBackendFlavor.PdCursesMod => GetLibraryPaths(CursesBackendFlavor.PdCursesModWindowsConsole)
                                               .Concat(GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal))
                                               .Concat(GetLibraryPaths(CursesBackendFlavor.PdCursesModGui)),
            CursesBackendFlavor.PdCursesWindowsConsole when _dotNetSystemAdapter.IsWindows => new[]
            {
                PlatformDllName("pdcurses-wincon"), PlatformDllName("pdcurses")
            }.Select(p => (p, CursesBackendType.PdCurses)),
            CursesBackendFlavor.PdCurses => GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole),
            CursesBackendFlavor.NCurses when _dotNetSystemAdapter.IsMacOs => FindMacOsNCursesCandidates()
                                                                             .Concat(new[]
                                                                             {
                                                                                 PlatformDllName("ncurses")
                                                                             })
                                                                             .Select(p =>
                                                                                 (p, CursesBackendType.NCurses)),
            CursesBackendFlavor.NCurses => new[]
            {
                PlatformDllName("ncursesw", 6), PlatformDllName("ncursesw"), PlatformDllName("ncursesw", 5)
            }.Select(p => (p, CursesBackendType.NCurses)),
            CursesBackendFlavor.AnyWindowsConsole when _dotNetSystemAdapter.IsWindows => GetLibraryPaths(
                    CursesBackendFlavor.PdCursesModWindowsConsole)
                .Concat(GetLibraryPaths(CursesBackendFlavor.PdCursesWindowsConsole)),
            CursesBackendFlavor.AnyVirtualTerminal => GetLibraryPaths(CursesBackendFlavor.NCurses)
                .Concat(GetLibraryPaths(CursesBackendFlavor.PdCursesModVirtualTerminal)),
            CursesBackendFlavor.AnyGui => GetLibraryPaths(CursesBackendFlavor.PdCursesModGui),
            CursesBackendFlavor.Any when _dotNetSystemAdapter.IsLinux ||
                _dotNetSystemAdapter.IsFreeBsd ||
                _dotNetSystemAdapter.IsMacOs => GetLibraryPaths(CursesBackendFlavor.AnyVirtualTerminal)
                    .Concat(GetLibraryPaths(CursesBackendFlavor.AnyGui)),
            CursesBackendFlavor.Any when _dotNetSystemAdapter.IsWindows => GetLibraryPaths(CursesBackendFlavor
                                                                               .AnyWindowsConsole)
                                                                           .Concat(GetLibraryPaths(CursesBackendFlavor
                                                                               .AnyVirtualTerminal))
                                                                           .Concat(GetLibraryPaths(CursesBackendFlavor
                                                                               .AnyGui)),
            var _ => Enumerable.Empty<(string, CursesBackendType)>()
        };
    }

    private IEnumerable<(string name, int version)> GetCandidatesInDirectory(string directory, Regex pattern)
    {
        Debug.Assert(pattern != null);
        Debug.Assert(!string.IsNullOrEmpty(directory));

        return _dotNetSystemAdapter.EnumerateFiles(directory)
                                   .Select(f => pattern.Match(f))
                                   .Where(m => m.Success)
                                   .Select(m => (name: _dotNetSystemAdapter.CombinePaths(directory, m.Groups[0]
                                       .Value), version: int.Parse(m.Groups[1]
                                                                    .Value)));
    }

    private IEnumerable<string> FindMacOsNCursesCandidates()
    {
        var homeBrewPrefix = _dotNetSystemAdapter.GetEnvironmentVariable("HOMEBREW_PREFIX");
        var homeBrewCellar = _dotNetSystemAdapter.GetEnvironmentVariable("HOMEBREW_CELLAR");

        var candidates = new List<(string name, int version)>();
        var matchRegEx = new Regex(@"libncurses\.(\d+)\.dylib", RegexOptions.Compiled);

        if (!string.IsNullOrEmpty(homeBrewPrefix))
        {
            var libPath = _dotNetSystemAdapter.CombinePaths(homeBrewPrefix, "lib");
            if (_dotNetSystemAdapter.DirectoryExists(libPath))
            {
                candidates.AddRange(GetCandidatesInDirectory(libPath, matchRegEx));
            }
        }

        if (!string.IsNullOrEmpty(homeBrewCellar))
        {
            var ncursesPath = _dotNetSystemAdapter.CombinePaths(homeBrewCellar, "ncurses");
            if (_dotNetSystemAdapter.DirectoryExists(ncursesPath))
            {
                foreach (var v in _dotNetSystemAdapter.EnumerateDirectories(ncursesPath))
                {
                    var libPath = _dotNetSystemAdapter.CombinePaths(v, "lib");
                    if (_dotNetSystemAdapter.DirectoryExists(libPath))
                    {
                        candidates.AddRange(GetCandidatesInDirectory(libPath, matchRegEx));
                    }
                }
            }
        }

        return candidates.OrderByDescending(c => c.version)
                         .Select(c => c.name);
    }
}
