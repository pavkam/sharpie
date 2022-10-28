namespace Sharpie;

using Curses;

/// <summary>
/// A factory object used to build terminals.
/// </summary>
internal class TerminalFactory
{
    /// <summary>
    /// Builds a new <see cref="Terminal"/> instance.
    /// </summary>
    /// <param name="curses">The Curses backend.</param>
    /// <param name="enableLineBuffering">Enable line buffering.</param>
    /// <param name="enableRawMode">Enable raw mode.</param>
    /// <param name="enableReturnToNewLineTranslation">Enable return => NL translation.</param>
    /// <param name="readTimeoutMillis">Read time-out.</param>
    /// <param name="enableInputEchoing">Enable input echoing.</param>
    /// <param name="enableManualFlush">Enable manual flush.</param>
    /// <param name="enableColors">Enable colors.</param>
    /// <param name="hardwareCursorMode">Enable hardware cursor.</param>
    /// <param name="enableEnvironmentOverrides">Enable environment var. overrides.</param>
    /// <param name="softLabelKeyMode">Soft label key mode.</param>
    /// <param name="enableMouse">Enable mouse.</param>
    /// <returns></returns>
    public virtual Terminal Build(ICursesProvider curses, bool enableLineBuffering, bool enableRawMode,
        bool enableReturnToNewLineTranslation, int readTimeoutMillis, bool enableInputEchoing, bool enableManualFlush,
        bool enableColors, CaretMode hardwareCursorMode, bool enableEnvironmentOverrides,
        SoftLabelKeyMode softLabelKeyMode, bool enableMouse) =>
        new(curses, enableLineBuffering, enableRawMode, enableReturnToNewLineTranslation, readTimeoutMillis,
            enableInputEchoing, enableManualFlush, enableColors, hardwareCursorMode, enableEnvironmentOverrides,
            softLabelKeyMode, enableMouse);
}
