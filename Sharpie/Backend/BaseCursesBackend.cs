#pragma warning disable CS1591

namespace Sharpie.Backend;

/// <summary>
/// Implements the <see cref="ICursesBackend"/> interface and serves as base class for specific Curses backends.
/// </summary>
[PublicAPI]
internal abstract class BaseCursesBackend: ICursesBackend
{
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system interop adapter.</param>
    /// <param name="cursesSymbolResolver">The Curses symbol resolver.</param>
    protected BaseCursesBackend(IDotNetSystemAdapter dotNetSystemAdapter, INativeSymbolResolver cursesSymbolResolver)
    {
        Debug.Assert(dotNetSystemAdapter != null);
        Debug.Assert(cursesSymbolResolver != null);

        DotNetSystemAdapter = dotNetSystemAdapter;
        CursesSymbolResolver = cursesSymbolResolver;
    }

    /// <summary>
    /// The Curses symbol resolver.
    /// </summary>
    protected internal INativeSymbolResolver CursesSymbolResolver { get; }
    
    /// <summary>
    /// The .NET system interop adapter.
    /// </summary>
    protected internal IDotNetSystemAdapter DotNetSystemAdapter { get; }

    /// <summary>
    /// Encodes the given video attributes and the color pair into a Curses-specific value.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="colorPair">The color pair.</param>
    /// <returns>The combined value.</returns>
    protected internal abstract uint EncodeCursesAttribute(VideoAttribute attributes, short colorPair);

    /// <summary>
    /// Decodes the Curses-specific value back into vide attributes and the color pair.
    /// </summary>
    /// <param name="attrs">The backend specific value.</param>
    /// <returns>The tuple containing the decoded values.</returns>
    protected internal abstract (VideoAttribute attributtes, short colorPair) DecodeCursesAttributes(uint attrs);

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public bool is_immedok(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.is_immedok>()(window);

    public bool is_leaveok(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.is_leaveok>()(window);

    public bool is_scrollok(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.is_scrollok>()(window);

    public int baudrate() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.baudrate>()();

    public int beep() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.beep>()();

    public bool can_change_color() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.can_change_color>()();

    public int cbreak() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.cbreak>()();

    public int color_content(short color, out short red, out short green, out short blue) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.color_content>()(color, out red, out green, out blue);

    public int copywin(IntPtr fromWindow, IntPtr toWindow, int srcStartLine, int srcStartCol,
        int destStartLine, int destStartCol, int destEndLine, int destEndCol,
        int overlay) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.copywin>()(fromWindow, toWindow, srcStartLine, srcStartCol,
            destStartLine, destStartCol, destEndLine, destEndCol, overlay);

    public int curs_set(int level) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.curs_set>()(level);

    public int delwin(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.delwin>()(window);

    public IntPtr derwin(IntPtr window, int lines, int cols, int beginLine,
        int beginCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.derwin>()(window, lines, cols, beginLine, beginCol);

    public int doupdate() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.doupdate>()();

    public IntPtr dupwin(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.dupwin>()(window);

    public int echo() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.echo>()();

    public int endwin() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.endwin>()();

    public int erasewchar(out uint @char) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.erasewchar>()(out @char);

    public int flash() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.flash>()();

    public int getcurx(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getcurx>()(window);

    public int getcury(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getcury>()(window);

    public int getbegx(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getbegx>()(window);

    public int getbegy(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getbegy>()(window);

    public int getmaxx(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getmaxx>()(window);

    public int getmaxy(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getmaxy>()(window);

    public int getparx(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getparx>()(window);

    public int getpary(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getpary>()(window);

    public bool has_colors() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.has_colors>()();

    public void immedok(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.immedok>()(window, set);

    public virtual IntPtr initscr() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.initscr>()();

    public int init_color(short color, short red, short green, short blue) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.init_color>()(color, red, green, blue);

    public int init_pair(short colorPair, short fgColor, short bgColor) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.init_pair>()(colorPair, fgColor, bgColor);

    public int intrflush(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.intrflush>()(window, set);

    public bool is_linetouched(IntPtr window, int line) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.is_linetouched>()(window, line);

    public bool is_wintouched(IntPtr window) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.is_wintouched>()(window);

    public int keypad(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.keypad>()(window, set);

    public int leaveok(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.leaveok>()(window, set);

    public string? longname() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            CursesSymbolResolver.Resolve<BaseCursesFunctionMap.longname>()());

    public int meta(IntPtr window, bool set) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.meta>()(window, set);

    public int mvderwin(IntPtr window, int parentLine, int parentCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.mvderwin>()(window, parentLine, parentCol);

    public int mvwin(IntPtr window, int toLine, int toCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.mvwin>()(window, toLine, toCol);

    public IntPtr newpad(int lines, int cols) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.newpad>()(lines, cols);

    public IntPtr newwin(int lines, int cols, int atLine, int atCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.newwin>()(lines, cols, atLine, atCol);

    public int nl() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.nl>()();

    public int nocbreak() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.nocbreak>()();

    public int nodelay(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.nodelay>()(window, set);

    public int noecho() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.noecho>()();

    public int nonl() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.nonl>()();

    public void noqiflush() { CursesSymbolResolver.Resolve<BaseCursesFunctionMap.noqiflush>()(); }

    public int noraw() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.noraw>()();

    public int notimeout(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.notimeout>()(window, set);

    public int overlay(IntPtr srcWindow, IntPtr destWindow) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.overlay>()(srcWindow, destWindow);

    public int overwrite(IntPtr srcWindow, IntPtr destWindow) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.overwrite>()(srcWindow, destWindow);

    public int pair_content(short colorPair, out short fgColor, out short bgColor) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.pair_content>()(colorPair, out fgColor, out bgColor);

    public int pnoutrefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.pnoutrefresh>()(pad, padMinLine, padMinCol, scrMinLine,
            scrMinCol, scrMaxLine, scrMaxCol);

    public int prefresh(IntPtr pad, int padMinLine, int padMinCol, int scrMinLine,
        int scrMinCol, int scrMaxLine, int scrMaxCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.prefresh>()(pad, padMinLine, padMinCol, scrMinLine, scrMinCol,
            scrMaxLine, scrMaxCol);

    public void qiflush() { CursesSymbolResolver.Resolve<BaseCursesFunctionMap.qiflush>()(); }

    public int raw() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.raw>()();

    public int ripoffline(int lines, ICursesBackend.ripoffline_callback callback) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.ripoffline>()(lines, callback);

    public int scrollok(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.scrollok>()(window, set);

    public abstract int slk_attr_off(VideoAttribute attributes, IntPtr reserved);

    public abstract int slk_attr_on(VideoAttribute attributes, IntPtr reserved);

    public abstract int slk_attr(out VideoAttribute attributes, out short colorPair);

    public abstract int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved);

    public abstract int slk_clear();

    public abstract int slk_set(int labelIndex, string title, int align);
    
    public abstract int slk_color(short colorPair);

    public abstract int slk_init(int format);

    public abstract int slk_noutrefresh();

    public abstract int slk_refresh();

    public abstract int slk_restore();

    public abstract int slk_touch();

    public int start_color() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.start_color>()();

    public IntPtr subpad(IntPtr pad, int lines, int cols, int atLine,
        int atCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.subpad>()(pad, lines, cols, atLine, atCol);

    public int syncok(IntPtr window, bool set) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.syncok>()(window, set);

    public string? termname() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            CursesSymbolResolver.Resolve<BaseCursesFunctionMap.termname>()());

    public void use_env(bool set) { CursesSymbolResolver.Resolve<BaseCursesFunctionMap.use_env>()(set); }

    public int wattr_get(IntPtr window, out VideoAttribute attributes, out short colorPair, IntPtr reserved)
    {
        var ret = CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wattr_get>()(window, out var attrs, out colorPair,
            reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public int wattr_set(IntPtr window, VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wattr_set>()(window, EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int wattr_on(IntPtr window, VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wattr_on>()(window, EncodeCursesAttribute(attributes, 0), reserved);

    public int wattr_off(IntPtr window, VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wattr_off>()(window, EncodeCursesAttribute(attributes, 0), reserved);

    public int wborder(IntPtr window, uint leftSide, uint rightSide, uint topSide,
        uint bottomSide, uint topLeftCorner, uint topRightCorner, uint bottomLeftCorner,
        uint bottomRightCorner) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wborder>()(window, leftSide, rightSide, topSide, bottomSide,
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);

    public int wchgat(IntPtr window, int count, VideoAttribute attributes, short colorPair,
        IntPtr reserved) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wchgat>()(window, count, EncodeCursesAttribute(attributes, 0), colorPair, reserved);

    public int wclrtobot(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wclrtobot>()(window);

    public int wclrtoeol(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wclrtoeol>()(window);

    public int wcolor_set(IntPtr window, short colorPair, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wcolor_set>()(window, colorPair, reserved);

    public int wdelch(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wdelch>()(window);

    public int werase(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.werase>()(window);

    public int whline(IntPtr window, uint @char, int count) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.whline>()(window, @char, count);

    public int winsdelln(IntPtr window, int count) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.winsdelln>()(window, count);

    public int wmove(IntPtr window, int newLine, int newCol) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wmove>()(window, newLine, newCol);

    public int wnoutrefresh(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wnoutrefresh>()(window);

    public int wredrawln(IntPtr window, int startLine, int lineCount) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wredrawln>()(window, startLine, lineCount);

    public int wrefresh(IntPtr window) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wrefresh>()(window);

    public int wscrl(IntPtr window, int lines) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wscrl>()(window, lines);

    public void wtimeout(IntPtr window, int delay) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wtimeout>()(window, delay);

    public int wtouchln(IntPtr window, int line, int count, int changed) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wtouchln>()(window, line, count, changed);

    public int wvline(IntPtr window, uint @char, int count) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wvline>()(window, @char, count);

    public string? curses_version() =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(CursesSymbolResolver
            .Resolve<BaseCursesFunctionMap.curses_version>()());

    public int assume_default_colors(int fgColor, int bgColor) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.assume_default_colors>()(fgColor, bgColor);

    public int use_default_colors() => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.use_default_colors>()();

    public int wresize(IntPtr window, int lines, int columns) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wresize>()(window, lines, columns);

    public abstract int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes,
        out short colorPair, IntPtr reserved);

    public string? key_name(uint @char) =>
        DotNetSystemAdapter.NativeLibraryAnsiStrPtrToString(
            CursesSymbolResolver.Resolve<BaseCursesFunctionMap.key_name>()(@char));

    public int killwchar(out uint @char) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.killwchar>()(out @char);

    public abstract int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved);

    public int term_attrs(out VideoAttribute attributes)
    {
        var ret = CursesSymbolResolver.Resolve<BaseCursesFunctionMap.term_attrs>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            return ret;
        }
        
        (attributes, _) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public abstract int wadd_wch(IntPtr window, ComplexChar @char);

    public abstract int wbkgrnd(IntPtr window, ComplexChar @char);

    public abstract int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide,
        ComplexChar topSide, ComplexChar bottomSide, ComplexChar topLeftCorner,
        ComplexChar topRightCorner, ComplexChar bottomLeftCorner, ComplexChar bottomRightCorner);

    public int wget_wch(IntPtr window, out uint @char) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.wget_wch>()(window, out @char);

    public abstract int wgetbkgrnd(IntPtr window, out ComplexChar @char);

    public abstract int whline_set(IntPtr window, ComplexChar @char, int count);

    public abstract int win_wch(IntPtr window, out ComplexChar @char);

    public abstract int wvline_set(IntPtr window, ComplexChar @char, int count);

    public int getmouse(out CursesMouseEvent @event) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.getmouse>()(out @event);

    public virtual int mousemask(uint newMask, out uint oldMask) =>
        CursesSymbolResolver.Resolve<BaseCursesFunctionMap.mousemask>()(newMask, out oldMask);

    public abstract int mouse_version();

    public int mouseinterval(int millis) => CursesSymbolResolver.Resolve<BaseCursesFunctionMap.mouseinterval>()(millis);

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
