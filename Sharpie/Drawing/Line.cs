namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain line drawing characters.
/// </summary>
[PublicAPI]
public static class Line
{
    /// <summary>
    /// Defines the lines and their styles. Combine multiple of these to create a box drawing character.
    /// </summary>
    [Flags]
    public enum SideAndStyle
    {
        /// <summary>
        /// Light right line.
        /// </summary>
        RightLight = 1 << 0,

        /// <summary>
        /// Light left line.
        /// </summary>
        LeftLight = 1 << 1,

        /// <summary>
        /// Light top line.
        /// </summary>
        TopLight = 1 << 2,

        /// <summary>
        /// Light bottom line.
        /// </summary>
        BottomLight = 1 << 3,

        /// <summary>
        /// Heavy right line.
        /// </summary>
        RightHeavy = 1 << 4,

        /// <summary>
        /// Heavy left line.
        /// </summary>
        LeftHeavy = 1 << 5,

        /// <summary>
        /// Heavy top line.
        /// </summary>
        TopHeavy = 1 << 6,

        /// <summary>
        /// Heavy bottom line.
        /// </summary>
        BottomHeavy = 1 << 7,

        /// <summary>
        /// Light dashed right line.
        /// </summary>
        RightLightDashed = 1 << 8,

        /// <summary>
        /// Light dashed left line.
        /// </summary>
        LeftLightDashed = 1 << 9,

        /// <summary>
        /// Light dashed top line.
        /// </summary>
        TopLightDashed = 1 << 10,

        /// <summary>
        /// Light dashed bottom line.
        /// </summary>
        BottomLightDashed = 1 << 11,

        /// <summary>
        /// Heavy dashed right line.
        /// </summary>
        RightHeavyDashed = 1 << 12,

        /// <summary>
        /// Heavy dashed left line.
        /// </summary>
        LeftHeavyDashed = 1 << 13,

        /// <summary>
        /// Heavy dashed top line.
        /// </summary>
        TopHeavyDashed = 1 << 14,

        /// <summary>
        /// Heavy dashed bottom line.
        /// </summary>
        BottomHeavyDashed = 1 << 15,

        /// <summary>
        /// Double right line.
        /// </summary>
        RightDouble = 1 << 16,

        /// <summary>
        /// Double left line.
        /// </summary>
        LeftDouble = 1 << 17,

        /// <summary>
        /// Double top line.
        /// </summary>
        TopDouble = 1 << 18,

        /// <summary>
        /// Double bottom line.
        /// </summary>
        BottomDouble = 1 << 19,
    }

    private static readonly Dictionary<SideAndStyle, Rune> BoxCharacters = new()
    {
        // LIGHT
        { SideAndStyle.RightLight, new('╶') },
        { SideAndStyle.LeftLight, new('╴') },
        { SideAndStyle.TopLight, new('╵') },
        { SideAndStyle.BottomLight, new('╷') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight , new('─') },
        { SideAndStyle.LeftLight | SideAndStyle.RightHeavy, new('╼') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight, new('│') },
        { SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('╽') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight, new('┘') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight, new('└') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight, new('┐') },
        { SideAndStyle.RightLight | SideAndStyle.BottomLight, new('┌') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.TopLight, new('┴') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight |  SideAndStyle.BottomLight, new('┬') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomLight, new('┤') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.RightHeavy, new('┶') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('┧') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.RightHeavy, new('┮') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.TopHeavy, new('┦') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.TopHeavy, new('┸') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.TopDouble, new('╨') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.BottomHeavy, new('┰') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.BottomDouble, new('╥') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.BottomLight, new('├') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.LeftHeavy, new('┵') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('┟') },
        { SideAndStyle.RightLight | SideAndStyle.BottomLight | SideAndStyle.LeftHeavy, new('┭') },
        { SideAndStyle.RightLight | SideAndStyle.BottomLight | SideAndStyle.TopHeavy, new('┞') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.LeftHeavy, new('┥') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.LeftDouble, new('╡') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.RightHeavy, new('┝') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.RightDouble, new('╞') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.BottomLight, new('┼') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('╁') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLight | SideAndStyle.BottomLight | SideAndStyle.TopHeavy, new('╀') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.RightHeavy, new('┾') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.LeftHeavy, new('┽') },
        
        // HEAVY
        { SideAndStyle.RightHeavy, new('╺') },
        { SideAndStyle.LeftHeavy, new('╸') },
        { SideAndStyle.TopHeavy, new('╹') },
        { SideAndStyle.BottomHeavy, new('╻') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy, new('━') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightLight, new('╾') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy, new('┃') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('╿') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy, new('┛') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy, new('┓') },
        { SideAndStyle.RightHeavy | SideAndStyle.TopHeavy, new('┗') },
        { SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy, new('┏') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftHeavy, new('┫') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightHeavy, new('┣') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy  | SideAndStyle.TopHeavy, new('┻') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy, new('┳') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.RightLight, new('┹') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('┩') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightLight, new('┱') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.TopLight, new('┪') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.TopLight, new('┷') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.TopDouble, new('╨') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.BottomLight, new('┯') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.BottomDouble, new('╥') },
        { SideAndStyle.RightHeavy | SideAndStyle.TopHeavy | SideAndStyle.LeftLight, new('┺') },
        { SideAndStyle.RightHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('┡') },
        { SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftLight, new('┲') },
        { SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy | SideAndStyle.TopLight, new('┢') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftLight, new('┨') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftDouble, new('╡') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightLight, new('┠') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightDouble, new('╞') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy, new('╋') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('╇') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy | SideAndStyle.TopLight, new('╈') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightLight, new('╉') },
        { SideAndStyle.RightHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftLight, new('╊') },
        
        // LIGHT DASHED
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLightDashed, new('┄') },
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLight, new('┄') },
        { SideAndStyle.RightLight | SideAndStyle.LeftLightDashed, new('┄') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLightDashed, new('┆') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLight, new('┆') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLightDashed, new('┆') },

        // HEAVY DASHED
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavyDashed, new('┅') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavy , new('┅') },
        { SideAndStyle.RightHeavy | SideAndStyle.LeftHeavyDashed , new('┅') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavyDashed, new('┇') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavy, new('┇') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavyDashed, new('┇') },
        
        // DOUBLE
        { SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.LeftDouble, new('═') },
        { SideAndStyle.TopDouble, new('║') },
        { SideAndStyle.BottomDouble, new('║') },
        
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightLight, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightHeavy, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble, new('╝') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble, new('╗') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomLight, new('║') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomHeavy, new('║') },
        { SideAndStyle.RightDouble | SideAndStyle.TopDouble, new('╚') },
        { SideAndStyle.RightDouble | SideAndStyle.BottomDouble, new('╔') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopDouble, new('╩') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.BottomDouble, new('╦') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopLight, new('╧') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopHeavy, new('╧') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.BottomLight, new('╤') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.BottomHeavy, new('╤') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftDouble, new('╣') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightDouble, new('╠') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftLight, new('╢') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftHeavy, new('╢') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightLight, new('╟') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightHeavy, new('╟') },
        
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopDouble | SideAndStyle.BottomLight, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.TopDouble | SideAndStyle.BottomHeavy, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.BottomDouble | SideAndStyle.TopLight, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightDouble | SideAndStyle.BottomDouble | SideAndStyle.TopHeavy, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightLight, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightHeavy, new('╬') },
        { SideAndStyle.RightDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftLight, new('╬') },
        { SideAndStyle.RightDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftHeavy, new('╬') },
    };

    /// <summary>
    ///     Gets the character representing the line drawing resembling the combination specified by <paramref name="lines"/>.
    /// </summary>
    /// <param name="lines">The line combination.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="lines" /> contains an invalid value.</exception>
    public static Rune Get(SideAndStyle lines)
    {
        if (TryGet(lines, 0, 0, out var result) ||
            TryGet(lines, SideAndStyle.RightLightDashed, SideAndStyle.RightLight, out result) ||
            TryGet(lines, SideAndStyle.LeftLightDashed, SideAndStyle.LeftLight, out result) ||
            TryGet(lines, SideAndStyle.TopLightDashed, SideAndStyle.TopLight, out result) ||
            TryGet(lines, SideAndStyle.BottomLightDashed, SideAndStyle.BottomLight, out result) ||
            TryGet(lines, SideAndStyle.RightHeavyDashed, SideAndStyle.RightHeavy, out result) ||
            TryGet(lines, SideAndStyle.LeftHeavyDashed, SideAndStyle.LeftHeavy, out result) ||
            TryGet(lines, SideAndStyle.TopHeavyDashed, SideAndStyle.TopHeavy, out result) ||
            TryGet(lines, SideAndStyle.BottomHeavyDashed, SideAndStyle.BottomHeavy, out result) ||
            TryGet(lines, SideAndStyle.RightDouble, SideAndStyle.RightHeavy, out result) ||
            TryGet(lines, SideAndStyle.LeftDouble, SideAndStyle.LeftHeavy, out result) ||
            TryGet(lines, SideAndStyle.TopDouble, SideAndStyle.TopHeavy, out result) ||
            TryGet(lines, SideAndStyle.BottomDouble, SideAndStyle.BottomHeavy, out result))
        {
            return result;
        }

        throw new ArgumentException("Invalid line combination.", nameof(lines));
    }

    private static bool TryGet(SideAndStyle lines, SideAndStyle replaceWhat, SideAndStyle replaceWith, out Rune rune)
    {
        if (replaceWhat != 0 && lines.HasFlag(replaceWhat))
        {
            lines = (lines & ~replaceWhat) | replaceWith;
        }

        return BoxCharacters.TryGetValue(lines, out rune);
    }
}
