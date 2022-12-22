/*
Copyright (c) 2022, Alexandru Ciobanu
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
/// NCurses-backend with unix-specific extensions.
/// </summary>
[SupportedOSPlatform("linux"), SupportedOSPlatform("macos"), SupportedOSPlatform("freebsd")]
internal sealed class UnixNCursesBackend: NCursesBackend
{
    private readonly INativeSymbolResolver _libCSymbolResolver;

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system adapter.</param>
    /// <param name="nCursesSymbolResolver">The NCurses symbol resolver.</param>
    /// <param name="libCSymbolResolver">The LibC symbol resolver.</param>
    public UnixNCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, 
        INativeSymbolResolver nCursesSymbolResolver, 
        INativeSymbolResolver libCSymbolResolver):
        base(dotNetSystemAdapter, nCursesSymbolResolver)
    {
        Debug.Assert(dotNetSystemAdapter.IsUnixLike);
        Debug.Assert(libCSymbolResolver != null);

        _libCSymbolResolver = libCSymbolResolver;
    }
    
    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming
    
    public override int mousemask(int newMask, out int oldMask)
    {
        var result = base.mousemask(newMask, out oldMask);
        if (!result.Failed())
        {
            var csi = "\x1b[?1003l";
            if ((newMask & (int) CursesMouseEvent.EventType.ReportPosition) != 0)
            {
                csi = "\x1b[?1003h";
            } else if ((newMask & (int) CursesMouseEvent.EventType.All) != 0)
            {
                csi = "\x1b[?1000h";
            }

            // Force enable mouse reporting. Curses doesn't always want to do that.
            DotNetSystemAdapter.OutAndFlush(csi);
        }

        return result;
    }

    public override void set_unicode_locale()
    {
        var category = DotNetSystemAdapter.IsMacOs ? 0 : 6;
        _libCSymbolResolver.Resolve<LibCFunctionMap.setlocale>()(category, "");
    }

    public override bool monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        handle = DotNetSystemAdapter.MonitorTerminalResizeSignal(action);
        return true;
    }
    
    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
