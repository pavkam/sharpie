#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Text.RegularExpressions;

[PublicAPI]
internal class NCursesBackend: BaseCursesBackend
{
    private int? _mouseAbiVersion;

    internal NCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver nCursesSymbolResolver) : base(dotNetSystemAdapter, nCursesSymbolResolver)
    {
    }

    protected internal override uint EncodeCursesAttribute(VideoAttribute attributes, short colorPair) => ((uint)attributes << 16) | (((uint)colorPair & 0xFF) << 8);

    protected internal override (VideoAttribute attributtes, short colorPair) DecodeCursesAttributes(uint attrs) => ((VideoAttribute) (attrs >> 16), (short) ((attrs >> 8) & 0xFF));

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public override int slk_attr_off(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_off>()(EncodeCursesAttribute(attributes, 0), reserved);

    public override int slk_attr_on(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_on>()(EncodeCursesAttribute(attributes, 0), reserved);

    public override int slk_attr(out VideoAttribute attributes, out short colorPair)
    {
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            colorPair = 0;

            return ret;
        }

        (attributes, colorPair) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public override int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_set>()(EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public override int slk_clear() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_clear>()();

    public override int slk_color(short colorPair) => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_color>()(colorPair);

    public override int slk_set(int labelIndex, string title, int align) =>
        CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_set>()(labelIndex, title, align);
    
    public override int slk_init(int format) => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_init>()(format);

    public override int slk_noutrefresh() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_noutrefresh>()();

    public override int slk_refresh() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_refresh>()();

    public override int slk_restore() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_restore>()();

    public override int slk_touch() => CursesSymbolResolver.Resolve<NCursesFunctionMap.slk_touch>()();

    public override int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes, out short colorPair,
        IntPtr reserved)
    {
        var c = @char.GetRawValue<NCursesComplexChar>();
        
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.getcchar>()(ref c, dest, out var attrs,
            out colorPair, reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public override int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved)
    {
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.setcchar>()(out var c, text,
            EncodeCursesAttribute(attributes, 0), colorPair, reserved);
        @char = new(c);
        return ret;
    }

    public override int wadd_wch(IntPtr window, ComplexChar @char)
    {
        var c = @char.GetRawValue<NCursesComplexChar>();
        
        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wadd_wch>()(window, ref c);
    }

    public override int wbkgrnd(IntPtr window, ComplexChar @char)
    {
        var c = @char.GetRawValue<NCursesComplexChar>();

        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgrnd>()(window, ref c);
    }

    public override int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide,
        ComplexChar topSide, ComplexChar bottomSide, ComplexChar topLeftCorner,
        ComplexChar topRightCorner, ComplexChar bottomLeftCorner, ComplexChar bottomRightCorner)
    {
        var lc = leftSide.GetRawValue<NCursesComplexChar>();
        var rc = rightSide.GetRawValue<NCursesComplexChar>();
        var tc = topSide.GetRawValue<NCursesComplexChar>();
        var bc = bottomSide.GetRawValue<NCursesComplexChar>();
        var tlc = topLeftCorner.GetRawValue<NCursesComplexChar>();
        var trc = topRightCorner.GetRawValue<NCursesComplexChar>();
        var blc = bottomLeftCorner.GetRawValue<NCursesComplexChar>();
        var brc = bottomRightCorner.GetRawValue<NCursesComplexChar>();
        
        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wborder_set>()(window, ref lc, ref rc,
            ref tc, ref bc, ref tlc, ref trc, ref blc,
            ref brc);
    }

    public override int wgetbkgrnd(IntPtr window, out ComplexChar @char)
    {
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.wgetbkgrnd>()(window, out var c);
        @char = new(c);

        return ret;
    }

    public override int whline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = @char.GetRawValue<NCursesComplexChar>();
        
        return CursesSymbolResolver.Resolve<NCursesFunctionMap.whline_set>()(window, ref c, count);
    }

    public override int win_wch(IntPtr window, out ComplexChar @char)
    {
        var ret = CursesSymbolResolver.Resolve<NCursesFunctionMap.win_wch>()(window, out var c);
        @char = new(c);
        return ret;
    }

    public override int wvline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = @char.GetRawValue<NCursesComplexChar>();
        
        return CursesSymbolResolver.Resolve<NCursesFunctionMap.wvline_set>()(window, ref c, count);
    }

    public override int mouse_version()
    {
        if (_mouseAbiVersion == null)
        {
            var ver = curses_version();
            var abi = -1;
            if (ver != null)
            {
                var versionParser = new Regex(@".*(\d+)\.(\d+)\.(\d+)");
                var match = versionParser.Match(ver);
                if (match.Success)
                {
                    var major = int.Parse(match.Groups[1]
                                               .Value);

                    abi = major switch
                    {
                        >= 6 => 2,
                        5 => 1,
                        var _ => abi
                    };
                }
            }

            _mouseAbiVersion = abi;
        }

        return _mouseAbiVersion.Value;
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
