#pragma warning disable CS1591

namespace Sharpie.Backend;

using System.Text.RegularExpressions;

[PublicAPI]
internal class NCursesBackend: ICursesBackend
{
    private readonly INativeSymbolResolver _nCursesSymbolResolver;
    private int? _mouseAbiVersion;

    internal NCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver nCursesSymbolResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(nCursesSymbolResolver != null);

        DotNetSystemAdapter = dotNetSystemAdapter;
        _nCursesSymbolResolver = nCursesSymbolResolver;
    }

    public IDotNetSystemAdapter DotNetSystemAdapter { get; }

    private uint EncodeCursesAttribute(VideoAttribute attributes, short colorPair) => ((uint)attributes << 16) | (((uint)colorPair & 0xFF) << 8);

    private (VideoAttribute attributtes, short colorPair) DecodeCursesAttributes(uint attrs) => ((VideoAttribute) (attrs >> 16), (short) ((attrs >> 8) & 0xFF));

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public bool is_immedok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_immedok>()(window);

    public bool is_leaveok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_leaveok>()(window);

    public bool is_scrollok(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_scrollok>()(window);

    public int baudrate() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.baudrate>()();

    public int beep() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.beep>()();

    public bool can_change_color() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.can_change_color>()();

    public int cbreak() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.cbreak>()();

    public int color_content(short color, out short red, out short green, out short blue) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.color_content>()(color, out red, out green, out blue);

    public int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.copywin>()(fromWindow, toWindow, srcStartLine, srcStartCol,
            destStartLine, destStartCol, destEndLine, destEndCol, overlay);

    public int curs_set(int level) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.curs_set>()(level);

    public int delwin(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.delwin>()(window);

    public IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.derwin>()(window, lines, cols, beginLine, beginCol);

    public int doupdate() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.doupdate>()();

    public IntPtr dupwin(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.dupwin>()(window);

    public int echo() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.echo>()();

    public int endwin() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.endwin>()();

    public int erasewchar(out uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.erasewchar>()(out @char);

    public int flash() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.flash>()();

    public int getcurx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcurx>()(window);

    public int getcury(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcury>()(window);

    public int getbegx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getbegx>()(window);

    public int getbegy(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getbegy>()(window);

    public int getmaxx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmaxx>()(window);

    public int getmaxy(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmaxy>()(window);

    public int getparx(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getparx>()(window);

    public int getpary(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getpary>()(window);

    public bool has_colors() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.has_colors>()();


    public void immedok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.immedok>()(window, set);

    public IntPtr initscr() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.initscr>()();

    public int init_color(short color, short red, short green, short blue) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.init_color>()(color, red, green, blue);

    public int init_pair(short colorPair, short fgColor, short bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.init_pair>()(colorPair, fgColor, bgColor);

    public int intrflush(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.intrflush>()(window, set);

    public bool is_linetouched(IntPtr window, int line) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_linetouched>()(window, line);

    public bool is_wintouched(IntPtr window) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.is_wintouched>()(window);

    public int keypad(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.keypad>()(window, set);

    public int leaveok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.leaveok>()(window, set);

    public string? longname() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            _nCursesSymbolResolver.Resolve<NCursesFunctionMap.longname>()());

    public int meta(IntPtr window, bool set) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.meta>()(window, set);

    public int mvderwin(IntPtr window, int parentLine, int parentCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mvderwin>()(window, parentLine, parentCol);

    public int mvwin(IntPtr window, int toLine, int toCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mvwin>()(window, toLine, toCol);

    public IntPtr newpad(int lines, int cols) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.newpad>()(lines, cols);

    public IntPtr newwin(int lines, int cols, int atLine, int atCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.newwin>()(lines, cols, atLine, atCol);

    public int nl() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nl>()();

    public int nocbreak() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nocbreak>()();

    public int nodelay(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nodelay>()(window, set);

    public int noecho() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noecho>()();

    public int nonl() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.nonl>()();

    public void noqiflush() { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noqiflush>()(); }

    public int noraw() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.noraw>()();

    public int notimeout(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.notimeout>()(window, set);

    public int overlay(IntPtr srcWindow, IntPtr destWindow) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.overlay>()(srcWindow, destWindow);

    public int overwrite(IntPtr srcWindow, IntPtr destWindow) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.overwrite>()(srcWindow, destWindow);

    public int pair_content(short colorPair, out short fgColor, out short bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pair_content>()(colorPair, out fgColor, out bgColor);

    public int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.pnoutrefresh>()(pad, padMinLine, padMinCol, scrMinLine,
            scrMinCol, scrMaxLine, scrMaxCol);

    public int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.prefresh>()(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    public void qiflush() { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.qiflush>()(); }

    public int raw() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.raw>()();

    public int ripoffline(int lines, ICursesBackend.ripoffline_callback callback) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.ripoffline>()(lines, callback);

    public int scrollok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.scrollok>()(window, set);

    public int slk_attr_off(VideoAttribute attributes, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_off>()(EncodeCursesAttribute(attributes, 0), reserved);

    public int slk_attr_on(VideoAttribute attributes, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_on>()(EncodeCursesAttribute(attributes, 0), reserved);

    public int slk_attr(out VideoAttribute attributes, out short colorPair)
    {
        var ret = _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            colorPair = 0;

            return ret;
        }

        (attributes, colorPair) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_attr_set>()(EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int slk_clear() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_clear>()();

    public int slk_color(short colorPair) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_color>()(colorPair);

    public int slk_init(int format) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_init>()(format);

    public int slk_noutrefresh() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_noutrefresh>()();

    public int slk_refresh() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_refresh>()();

    public int slk_restore() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_restore>()();

    public int slk_touch() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_touch>()();

    public int start_color() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.start_color>()();

    public IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.subpad>()(pad, lines, cols, atLine, atCol);

    public int syncok(IntPtr window, bool set) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.syncok>()(window, set);

    public string? termname() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            _nCursesSymbolResolver.Resolve<NCursesFunctionMap.termname>()());

    public void use_env(bool set) { _nCursesSymbolResolver.Resolve<NCursesFunctionMap.use_env>()(set); }

    public int wattr_get(IntPtr window, out VideoAttribute attributes, out short colorPair, IntPtr reserved)
    {
        var ret = _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_get>()(window, out var attrs, out colorPair,
            reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public int wattr_set(IntPtr window, VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_set>()(window, EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int wattr_on(IntPtr window, VideoAttribute attributes, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_on>()(window, EncodeCursesAttribute(attributes, 0), reserved);

    public int wattr_off(IntPtr window, VideoAttribute attributes, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wattr_off>()(window, EncodeCursesAttribute(attributes, 0), reserved);

    public int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wborder>()(window, leftSide, rightSide, topSide, bottomSide,
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);

    public int wchgat(IntPtr window, int count, VideoAttribute attributes, short colorPair,
        IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wchgat>()(window, count, EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int wclrtobot(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wclrtobot>()(window);

    public int wclrtoeol(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wclrtoeol>()(window);

    public int wcolor_set(IntPtr window, short colorPair, IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wcolor_set>()(window, colorPair, reserved);

    public int wdelch(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wdelch>()(window);

    public int werase(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.werase>()(window);

    public int whline(IntPtr window, uint @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.whline>()(window, @char, count);

    public int winsdelln(IntPtr window, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.winsdelln>()(window, count);

    public int wmove(IntPtr window, int newLine, int newCol) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wmove>()(window, newLine, newCol);

    public int wnoutrefresh(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wnoutrefresh>()(window);

    public int wredrawln(IntPtr window, int startLine, int lineCount) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wredrawln>()(window, startLine, lineCount);

    public int wrefresh(IntPtr window) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wrefresh>()(window);

    public int wscrl(IntPtr window, int lines) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wscrl>()(window, lines);

    public void wtimeout(IntPtr window, int delay) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wtimeout>()(window, delay);

    public int wtouchln(IntPtr window, int line, int count, int changed) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wtouchln>()(window, line, count, changed);

    public int wvline(IntPtr window, uint @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wvline>()(window, @char, count);

    public string? curses_version() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(_nCursesSymbolResolver
            .Resolve<NCursesFunctionMap.curses_version>()());

    public int assume_default_colors(int fgColor, int bgColor) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.assume_default_colors>()(fgColor, bgColor);

    public int use_default_colors() => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.use_default_colors>()();

    public int wresize(IntPtr window, int lines, int columns) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wresize>()(window, lines, columns);

    public int getcchar(CursesComplexChar @char, StringBuilder dest, out VideoAttribute attributes, out short colorPair,
        IntPtr reserved)
    {
        var ret = _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getcchar>()(ref @char, dest, out var attrs,
            out colorPair, reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public string? key_name(uint @char) =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            _nCursesSymbolResolver.Resolve<NCursesFunctionMap.key_name>()(@char));

    public int killwchar(out uint @char) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.killwchar>()(out @char);

    public int setcchar(out CursesComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.setcchar>()(out @char, text, EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int slk_set(int labelIndex, string title, int align) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.slk_set>()(labelIndex, title, align);

    public int term_attrs(out VideoAttribute attributes)
    {
        var ret = _nCursesSymbolResolver.Resolve<NCursesFunctionMap.term_attrs>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            return ret;
        }
        
        (attributes, _) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public int wadd_wch(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wadd_wch>()(window, ref @char);

    public int wbkgrnd(IntPtr window, CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wbkgrnd>()(window, ref @char);

    public int wborder_set(IntPtr window, CursesComplexChar leftSide, CursesComplexChar rightSide,
        CursesComplexChar topSide, CursesComplexChar bottomSide, CursesComplexChar topLeftCorner,
        CursesComplexChar topRightCorner, CursesComplexChar bottomLeftCorner, CursesComplexChar bottomRightCorner) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wborder_set>()(window, ref leftSide, ref rightSide,
            ref topSide, ref bottomSide, ref topLeftCorner, ref topRightCorner, ref bottomLeftCorner,
            ref bottomRightCorner);

    public int wget_wch(IntPtr window, out uint @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wget_wch>()(window, out @char);

    public int wgetbkgrnd(IntPtr window, out CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wgetbkgrnd>()(window, out @char);

    public int whline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.whline_set>()(window, ref @char, count);

    public int win_wch(IntPtr window, out CursesComplexChar @char) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.win_wch>()(window, out @char);

    public int wvline_set(IntPtr window, CursesComplexChar @char, int count) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.wvline_set>()(window, ref @char, count);

    public int getmouse(out CursesMouseEvent @event) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.getmouse>()(out @event);

    public virtual int mousemask(uint newMask, out uint oldMask) =>
        _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mousemask>()(newMask, out oldMask);

    public virtual int mouse_version()
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

    public int mouseinterval(int millis) => _nCursesSymbolResolver.Resolve<NCursesFunctionMap.mouseinterval>()(millis);

    public void set_title(string title) { DotNetSystemAdapter.SetConsoleTitle(title); }

    public virtual void set_unicode_locale() { }

    public virtual bool monitor_pending_resize(Action action, [NotNullWhen(true)] out IDisposable? handle)
    {
        handle = null;
        return false;
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
