namespace Sharpie;

/// <summary>
///     Stores the terminal options.
/// </summary>
/// <param name="UseColors">Toggle the use of colors. Default is <c>true</c>.</param>
/// <param name="EchoInput">Toggle the input echoing. Default is <c>false</c>.</param>
/// <param name="UseInputBuffering">Toggles buffering input. Default is <c>false</c>.</param>
/// <param name="UseMouse">Toggles the use of mouse. Default is <c>true</c>.</param>
/// <param name="MouseClickInterval">
///     The mouse click delay. Default is <c>null</c> which disables Curses handling of
///     clicks.
/// </param>
/// <param name="SuppressControlKeys">Toggles the suppression of control keys such as CTRL+C. Default is <c>true</c>.</param>
/// <param name="CaretMode">Specifies the caret mode. Default is <see cref="Sharpie.CaretMode.Visible" />.</param>
/// <param name="ManualFlush">Toggle the ability to manually flush the terminal. Default is <c>false</c>.</param>
/// <param name="ManagedWindows">
///     Specifies whether the <see cref="Screen" /> manages overlapping windows. Default is
///     <c>false</c>.
/// </param>
/// <param name="SoftLabelKeyMode">Specifies the SLK mode. Default is <see cref="Sharpie.SoftLabelKeyMode.Disabled" />.</param>
/// <param name="AllocateHeader">If <c>true</c>, allocates one line at the top as a header.</param>
/// <param name="AllocateFooter">If <c>true</c>, allocates one line at the bottom as a footer.</param>
/// <param name="UseEnvironmentOverrides">Toggles the use of environment LINE/COL overrides. Default is <c>true</c>.</param>
/// <param name="UseStandardKeySequenceResolvers">
///     Registers the standard key sequence resolvers defined in
///     <see cref="KeySequenceResolver" />.
/// </param>
[PublicAPI]
public record TerminalOptions(bool UseColors = true, bool EchoInput = false, bool UseInputBuffering = false,
    bool UseMouse = true, int? MouseClickInterval = null, bool SuppressControlKeys = true,
    CaretMode CaretMode = CaretMode.Visible, bool ManualFlush = false, bool ManagedWindows = false,
    SoftLabelKeyMode SoftLabelKeyMode = SoftLabelKeyMode.Disabled, bool AllocateHeader = false,
    bool AllocateFooter = false, bool UseEnvironmentOverrides = true, bool UseStandardKeySequenceResolvers = true);

