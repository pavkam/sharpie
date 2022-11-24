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
        { SideAndStyle.RightLight, new('╶') },
        { SideAndStyle.LeftLight, new('╴') },
        { SideAndStyle.TopLight, new('╵') },
        { SideAndStyle.BottomLight, new('╷') },
        { SideAndStyle.RightLight | SideAndStyle.LeftLight, new('─') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight, new('│') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight, new('┘') },
        { SideAndStyle.RightLight | SideAndStyle.TopLight, new('└') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight, new('┐') },
        { SideAndStyle.RightLight | SideAndStyle.BottomLight, new('┌') },
        { SideAndStyle.RightLight | SideAndStyle.LeftLight | SideAndStyle.TopLight, new('┴') },
        { SideAndStyle.RightLight | SideAndStyle.LeftLight | SideAndStyle.BottomLight, new('┬') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.LeftLight, new('┤') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLight | SideAndStyle.RightLight, new('├') },
        { SideAndStyle.RightLight | SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomLight, new('┼') },
        { SideAndStyle.LeftLight | SideAndStyle.RightHeavy, new('╼') },
        { SideAndStyle.LeftLight | SideAndStyle.RightLightDashed, new('┄') },
        { SideAndStyle.LeftLight | SideAndStyle.RightHeavyDashed, new('╼') },
        { SideAndStyle.LeftLight | SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('╽') },
        { SideAndStyle.TopLight | SideAndStyle.BottomLightDashed, new('┆') },
        { SideAndStyle.TopLight | SideAndStyle.BottomHeavyDashed, new('╽') },
        { SideAndStyle.TopLight | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.RightHeavy, new('┶') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.RightLightDashed, new('┴') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.RightHeavyDashed, new('┶') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.RightDouble, new('┶') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomHeavy, new('┧') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomLightDashed, new('┤') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomHeavyDashed, new('┧') },
        { SideAndStyle.LeftLight | SideAndStyle.TopLight | SideAndStyle.BottomDouble, new('┧') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.RightHeavy, new('┮') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.RightLightDashed, new('┬') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.RightHeavyDashed, new('┮') },
        { SideAndStyle.LeftLight | SideAndStyle.BottomLight | SideAndStyle.RightDouble, new('┮') },
        

        { SideAndStyle.RightHeavy, new('╺') },
        { SideAndStyle.LeftHeavy, new('╸') },
        { SideAndStyle.TopHeavy, new('╹') },
        { SideAndStyle.BottomHeavy, new('╻') },
        { SideAndStyle.RightHeavy | SideAndStyle.LeftHeavy, new('━') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy, new('┃') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy, new('┛') },
        { SideAndStyle.RightHeavy | SideAndStyle.TopHeavy, new('┗') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy, new('┓') },
        { SideAndStyle.RightHeavy | SideAndStyle.BottomHeavy, new('┏') },
        { SideAndStyle.RightHeavy | SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy, new('┻') },
        { SideAndStyle.RightHeavy | SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy, new('┳') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.LeftHeavy, new('┫') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightHeavy, new('┣') },
        { SideAndStyle.RightHeavy | SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomHeavy, new('╋') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightLight, new('╾') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightLightDashed, new('─') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightHeavyDashed, new('┅') },
        { SideAndStyle.LeftHeavy | SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('╿') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomLightDashed, new('╿') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomHeavyDashed, new('┇') },
        { SideAndStyle.TopHeavy | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.RightLight, new('┹') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.RightLightDashed, new('┹') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.RightHeavyDashed, new('┻') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.RightDouble, new('┻') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomLight, new('┩') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomLightDashed, new('┩') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomHeavyDashed, new('┫') },
        { SideAndStyle.LeftHeavy | SideAndStyle.TopHeavy | SideAndStyle.BottomDouble, new('┫') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightLight, new('┱') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightLightDashed, new('┱') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightHeavyDashed, new('┳') },
        { SideAndStyle.LeftHeavy | SideAndStyle.BottomHeavy | SideAndStyle.RightDouble, new('┳') },
        
        
        { SideAndStyle.RightLightDashed, new('╶') },
        { SideAndStyle.LeftLightDashed, new('╴') },
        { SideAndStyle.TopLightDashed, new('╵') },
        { SideAndStyle.BottomLightDashed, new('╷') },
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLightDashed, new('┄') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLightDashed, new('┆') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed, new('┘') },
        { SideAndStyle.RightLightDashed | SideAndStyle.TopLightDashed, new('└') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed, new('┐') },
        { SideAndStyle.RightLightDashed | SideAndStyle.BottomLightDashed, new('┌') },
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed, new('┴') },
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed, new('┬') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.LeftLightDashed, new('┤') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.RightLightDashed, new('├') },
        { SideAndStyle.RightLightDashed | SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.BottomLightDashed, new('┼') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.RightLight, new('─') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.RightHeavy, new('╼') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.RightHeavyDashed, new('╼') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomLight, new('┆') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomHeavy, new('╽') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomHeavyDashed, new('╽') },
        { SideAndStyle.TopLightDashed | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.RightLight, new('┴') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.RightHeavy, new('┶') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.RightHeavyDashed, new('┶') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.RightDouble, new('┶') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.BottomLight, new('┤') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.BottomHeavy, new('┧') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.BottomHeavyDashed, new('┧') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.TopLightDashed | SideAndStyle.BottomDouble, new('┧') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.RightLight, new('┬') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.RightHeavy, new('┮') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.RightHeavyDashed, new('┮') },
        { SideAndStyle.LeftLightDashed | SideAndStyle.BottomLightDashed | SideAndStyle.RightDouble, new('┮') },
        
        
        { SideAndStyle.RightHeavyDashed, new('╺') },
        { SideAndStyle.LeftHeavyDashed, new('╸') },
        { SideAndStyle.TopHeavyDashed, new('╹') },
        { SideAndStyle.BottomHeavyDashed, new('╻') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavyDashed, new('┅') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavyDashed, new('┇') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed, new('┛') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.TopHeavyDashed, new('┗') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed, new('┓') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.BottomHeavyDashed, new('┏') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed, new('┻') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed, new('┳') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.LeftHeavyDashed, new('┫') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.RightHeavyDashed, new('┣') },
        { SideAndStyle.RightHeavyDashed | SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavyDashed, new('╋') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.RightLight, new('╾') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.RightLightDashed, new('╾') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.RightHeavy, new('━') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomLight, new('╿') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavy, new('┇') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomLightDashed, new('╿') },
        { SideAndStyle.TopHeavyDashed | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.RightLight, new('┹') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.RightHeavy, new('┻') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.RightLightDashed, new('┹') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.RightDouble, new('┻') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.BottomLight, new('┩') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.BottomHeavy, new('┫') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.BottomLightDashed, new('┩') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.TopHeavyDashed | SideAndStyle.BottomDouble, new('┫') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.RightLight, new('┱') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.RightHeavy, new('┳') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.RightLightDashed, new('┱') },
        { SideAndStyle.LeftHeavyDashed | SideAndStyle.BottomHeavyDashed | SideAndStyle.RightDouble, new('┳') },
        
        
        { SideAndStyle.RightDouble, new('═') },
        { SideAndStyle.LeftDouble, new('═') },
        { SideAndStyle.TopDouble, new('║') },
        { SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.RightDouble | SideAndStyle.LeftDouble, new('═') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble, new('║') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble, new('╝') },
        { SideAndStyle.RightDouble | SideAndStyle.TopDouble, new('╚') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble, new('╗') },
        { SideAndStyle.RightDouble | SideAndStyle.BottomDouble, new('╔') },
        { SideAndStyle.RightDouble | SideAndStyle.LeftDouble | SideAndStyle.TopDouble, new('╩') },
        { SideAndStyle.RightDouble | SideAndStyle.LeftDouble | SideAndStyle.BottomDouble, new('╦') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.LeftDouble, new('╣') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomDouble | SideAndStyle.RightDouble, new('╠') },
        { SideAndStyle.RightDouble | SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomDouble, new('╬') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightLight, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightHeavy, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightLightDashed, new('═') },
        { SideAndStyle.LeftDouble | SideAndStyle.RightHeavyDashed, new('═') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomLight, new('║') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomHeavy, new('║') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomLightDashed, new('║') },
        { SideAndStyle.TopDouble | SideAndStyle.BottomHeavyDashed, new('║') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.RightLight, new('┹') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.RightHeavy, new('┻') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.RightLightDashed, new('┹') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.RightHeavyDashed, new('┻') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomLight, new('┩') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomHeavy, new('┫') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomLightDashed, new('┩') },
        { SideAndStyle.LeftDouble | SideAndStyle.TopDouble | SideAndStyle.BottomHeavyDashed, new('┫') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble | SideAndStyle.RightLight, new('┱') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble | SideAndStyle.RightHeavy, new('┳') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble | SideAndStyle.RightLightDashed, new('┱') },
        { SideAndStyle.LeftDouble | SideAndStyle.BottomDouble | SideAndStyle.RightHeavyDashed, new('┳') },
    };

    /// <summary>
    ///     Gets the character representing the line drawing resembling the combination specified by <paramref name="lines"/>.
    /// </summary>
    /// <param name="lines">The line combination.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="lines" /> contains an invalid value.</exception>
    public static Rune Get(SideAndStyle lines)
    {
        if (BoxCharacters.TryGetValue(lines, out var r))
        {
            return r;
        }

        throw new ArgumentException("Invalid line combination.", nameof(lines));
    }
}
