namespace Sharpie;

/// <summary>
/// Describes a Curses exception
/// </summary>
public class CursesException: Exception
{
    public CursesException(string caller): base($"The call to {caller} failed.")
    {
    }
}
