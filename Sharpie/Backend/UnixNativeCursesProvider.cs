#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Runtime.Versioning;

[SupportedOSPlatform("linux"), SupportedOSPlatform("macos"), SupportedOSPlatform("freebsd")]
internal sealed class UnixNativeCursesProvider: NativeCursesProvider
{
    private readonly INativeSymbolResolver _libCResolver;

    public UnixNativeCursesProvider(INativeSymbolResolver cursesResolver, INativeSymbolResolver libCResolver):
        base(cursesResolver)
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS() && !OperatingSystem.IsFreeBSD())
        {
            throw new PlatformNotSupportedException("This class can only be used on Unix systems.");
        }
        
        _libCResolver = libCResolver ?? throw new ArgumentNullException(nameof(libCResolver));
    }

    [SuppressMessage("ReSharper", "IdentifierTypo")]
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
            Console.Out.Write(csi);
            Console.Out.Flush();
        }

        return result;
    }

    public override void set_unicode_locale()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            _libCResolver.Resolve<LibCFunctionMap.setlocale>()(6, "");
        } else if (OperatingSystem.IsMacOS())
        {
            _libCResolver.Resolve<LibCFunctionMap.setlocale>()(0, "");
        }
    }

    public override bool monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        handle = PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ => { action(); });
        return true;
    }
}
