namespace Sharpie;

/// <summary>
///     Describes a text with mixed in styles. Can be used by <see cref="ISurface.WriteText(StyledText,bool)" />.
/// </summary>
[PublicAPI]
public readonly struct StyledText
{
    internal (string text, Style style)[]? Parts { get; }

    private StyledText((string text, Style style)[] parts) => Parts = parts;

    /// <summary>
    ///     Creates a new styled text with the given <paramref name="text" /> and <paramref name="style" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="style">The text style.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text" /> is <c>null</c>.</exception>
    public StyledText(string text, Style style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        Parts = new[] { (text, style) };
    }

    /// <summary>
    ///     Combines this styled text with another styled text.
    /// </summary>
    /// <param name="text">The other text to combine with.</param>
    /// <param name="style">The style of the text to combine with.</param>
    /// <returns>The combined styled text.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text" /> is <c>null</c>.</exception>
    public StyledText Plus(string text, Style style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        return Plus(new(text, style));
    }

    /// <summary>
    ///     Combines this styled text with another styled text.
    /// </summary>
    /// <param name="rhs">The other styled text to combine with.</param>
    /// <returns>The combined styled text.</returns>
    public StyledText Plus(StyledText rhs)
    {
        if (Parts == null)
        {
            return rhs;
        }

        if (rhs.Parts == null)
        {
            return this;
        }

        var combined = new (string text, Style style)[Parts.Length + rhs.Parts.Length];
        Array.Copy(Parts, combined, Parts.Length);
        Array.Copy(rhs.Parts, 0, combined, Parts.Length, Parts.Length);

        return new(combined);
    }

    /// <summary>
    ///     Combines two styled texts together.
    /// </summary>
    /// <param name="lhs">The left hand side styled text to combine.</param>
    /// <param name="rhs">The left right side styled text to combine.</param>
    /// <returns>The combined styled text.</returns>
    public static StyledText operator +(StyledText lhs, StyledText rhs) => lhs.Plus(rhs);

    /// <inheritdoc cref="object.Equals(object?)" />
    public override bool Equals(object? obj)
    {
        if (obj?.GetType() != GetType())
        {
            return false;
        }

        var op = ((StyledText) obj).Parts;

        return (op, Parts) switch
        {
            (null, null) => true,
            (null, not null) => false,
            (not null, null) => false,
            (var l and not null, var r and not null) when l.Length != r.Length => false,
            (var l and not null, var r and not null) => !r.Where((t, i) => l[i] != t)
                                                          .Any()
        };
    }

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode()
    {
        var h = new HashCode();
        if (Parts != null)
        {
            foreach (var p in Parts)
            {
                h.Add(p);
            }
        }

        return h.ToHashCode();
    }

    /// <inheritdoc cref="object.ToString" />
    public override string? ToString()
    {
        return Parts == null ? null : string.Join(", ", Parts.Select(p => $"\"{p.text}\" @ {p.style}"));
    }

    /// <summary>
    ///     Checks if two styled texts are equal.
    /// </summary>
    /// <param name="lhs">The left hand side styled text.</param>
    /// <param name="rhs">The left right side styled ..</param>
    /// <returns>The result of the check.</returns>
    public static bool operator ==(StyledText lhs, StyledText rhs) => lhs.Equals(rhs);

    /// <summary>
    ///     Checks if two styled texts are not equal.
    /// </summary>
    /// <param name="lhs">The left hand side styled text.</param>
    /// <param name="rhs">The left right side styled ..</param>
    /// <returns>The result of the check.</returns>
    public static bool operator !=(StyledText lhs, StyledText rhs) => !(lhs == rhs);
}
