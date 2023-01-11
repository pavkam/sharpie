namespace Sharpie.Backend;

/// <summary>
///     Lists the supported backend Curses libraries.
/// </summary>
[PublicAPI]
public enum CursesBackendType
{
    /// <summary>
    ///     The NCurses back-end.
    ///     Available by default on most POSIX-compliant operating systems such as
    ///     Linux, FreeBSD an MacOS. Not available by default on Windows.
    /// </summary>
    NCurses = 1,

    /// <summary>
    ///     The PDCurses back-end. Not available by default on operating systems.
    ///     For some Linuxes and FreeBSD a prebuilt library can be installed from package
    ///     managers.
    ///
    ///     Does not support <see cref="SoftLabelKeyManager"/>.
    ///     Not recommended for use. <see cref="PdCursesMod"/> for a better alternative.
    /// </summary>
    PdCurses,
    
    /// <summary>
    ///     The PDCursesMod back-end. This is an advanced version of <see cref="PdCurses"/> with more
    ///     platform availability and support. Not available by default on any operating systems.
    /// </summary>
    PdCursesMod,
}
