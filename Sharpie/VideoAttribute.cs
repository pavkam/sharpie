namespace Sharpie;

using JetBrains.Annotations;

/// <summary>
/// Defines the possible video attributes.
/// </summary>
[PublicAPI, Flags]
public enum VideoAttribute: uint
{
    /// <summary>
    /// No attributes, default text style.
    /// </summary>
    None = 0U,

    /// <summary>
    /// The best highlighting mode available.
    /// </summary>
    StandOut = 1U << 16,

    /// <summary>
    /// Underlined text.
    /// </summary>
    Underline = StandOut << 1,

    /// <summary>
    /// Reverse-video text.
    /// </summary>
    Reverse = Underline << 1,

    /// <summary>
    /// Blinking text
    /// </summary>
    Blink = Reverse << 1,

    /// <summary>
    /// Half bright text.
    /// </summary>
    Dim = Blink << 1,

    /// <summary>
    /// Extra bright or bold text.
    /// </summary>
    Bold = Dim << 1,

    /// <summary>
    /// Alternate character set.
    /// </summary>
    AltCharset = Bold << 1,

    /// <summary>
    /// Invisible or blank mode.
    /// </summary>
    Invisible = AltCharset << 1,

    /// <summary>
    /// Protected mode.
    /// </summary>
    Protect = Invisible << 1,

    /// <summary>
    /// Horizontal highlight.
    /// </summary>
    HorizontalHighlight = Protect << 1,

    /// <summary>
    /// Left highlight.
    /// </summary>
    LeftHighlight = HorizontalHighlight << 1,

    /// <summary>
    /// Low highlight.
    /// </summary>
    LowHighlight = LeftHighlight << 1,

    /// <summary>
    /// Right highlight.
    /// </summary>
    RightHighlight = LowHighlight << 1,

    /// <summary>
    /// Top highlight.
    /// </summary>
    TopHighlight = RightHighlight << 1,

    /// <summary>
    /// Vertical highlight.
    /// </summary>
    VerticalHighlight = TopHighlight << 1,

    /// <summary>
    /// Italic highlight.
    /// </summary>
    Italic = VerticalHighlight << 1,
}
