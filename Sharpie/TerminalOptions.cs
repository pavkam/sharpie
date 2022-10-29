namespace Sharpie;

/// <summary>
///     Stores the terminal options.
/// </summary>
/// <param name="UseColors">Toggle the use of colors. Default is <c>true</c>.</param>
/// <param name="EchoInput">Toggle the input echoing. Default is <c>false</c>.</param>
/// <param name="UseInputBuffering">Toggles buffering input. Default is <c>false</c>.</param>
/// <param name="UseMouse">Toggles the use of mouse. Default is <c>true</c>.</param>
/// <param name="MouseClickDelay">The mouse click delay. Default is <c>100</c> millis.</param>
/// <param name="SuppressControlKeys">Toggles the suppression of control keys such as CTRL+C. Default is <c>true</c>.</param>
/// <param name="TranslateReturnToNewLineChar">Toggles the translation of RETURN to \n. Default is <c>false</c>.</param>
/// <param name="CaretMode">Specifies the caret mode. Default is <see cref="Sharpie.CaretMode.Visible" />.</param>
/// <param name="ManualFlush">Toggle the ability to manually flush the terminal. Default is <c>false</c>.</param>
/// <param name="SoftLabelKeyMode">Specifies the SLK mode. Default is <see cref="Sharpie.SoftLabelKeyMode.Disabled" />.</param>
/// <param name="UseEnvironmentOverrides">Toggles the use of environment LINE/COL overrides. Default is <c>true</c>.</param>
[PublicAPI]
public record TerminalOptions(bool UseColors = true, bool EchoInput = false, bool UseInputBuffering = false,
    bool UseMouse = true, int MouseClickDelay = 100, bool SuppressControlKeys = true,
    bool TranslateReturnToNewLineChar = false, CaretMode CaretMode = CaretMode.Visible, bool ManualFlush = false,
    SoftLabelKeyMode SoftLabelKeyMode = SoftLabelKeyMode.Disabled, bool UseEnvironmentOverrides = true);
