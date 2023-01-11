namespace Sharpie.Abstractions;

/// <summary>
/// Defines the traits implemented by ASCII font providers.
/// </summary>
[PublicAPI]
public interface IAsciiFont
{
    /// <summary>
    /// The font's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Checks if the font contains a given glyph.
    /// </summary>
    /// <param name="char">The character.</param>
    /// <returns><c>true</c> if the font contains the given glyph; <c>false</c> otherwise.</returns>
    bool HasGlyph(Rune @char);
    
    /// <summary>
    /// Tries to get a glyph for a given <paramref name="char"/>.
    /// </summary>
    /// <param name="char">The character.</param>
    /// <param name="style">The style to apply to the glyph.</param>
    /// <returns>The output glyph, if found. Otherwise, the font will substitute the glyph with something else.</returns>
    IDrawable GetGlyph(Rune @char, Style style);
}
