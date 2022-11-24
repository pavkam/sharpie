namespace Sharpie.Drawing;

/// <summary>
///     Supplies functionality to obtain line drawing characters.
/// </summary>
[PublicAPI]
public static class Line
{
    [Flags]
    public enum SideAndStyle
    {
        RightLight = 1 << 0,
        LeftLight = 1 << 1,
        TopLight = 1 << 2,
        BottomLight = 1 << 3,
        
        RightHeavy = 1 << 4,
        LeftHeavy = 1 << 5,
        TopHeavy = 1 << 6,
        BottomHeavy = 1 << 7,
        
        RightLightDashed = 1 << 8,
        LeftLightDashed = 1 << 9,
        TopLightDashed = 1 << 10,
        BottomLightDashed = 1 << 11,
        
        RightHeavyDashed = 1 << 12,
        LeftHeavyDashed = 1 << 13,
        TopHeavyDashed = 1 << 14,
        BottomHeavyDashed = 1 << 15,

        RightDouble = 1 << 16,
        LeftDouble = 1 << 17,
        TopDouble = 1 << 18,
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
