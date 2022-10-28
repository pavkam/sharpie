namespace Sharpie;

using System.Text;
using JetBrains.Annotations;

/// <summary>
///     Defines a key event.
/// </summary>
[PublicAPI]
public sealed class KeyEvent: Event
{
    /// <summary>
    ///     Creates a new instance of the class.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    /// <param name="char">The char that was pressed if it was not a key.</param>
    /// <param name="keyName">The name of the key.</param>
    /// <param name="modifiers">The modifiers at the time of the press.</param>
    internal KeyEvent(Key key, Rune @char, string? keyName, ModifierKey modifiers): base(EventType.KeyPress)
    {
        Key = key;
        Char = @char;
        Modifiers = modifiers;
        Name = keyName;
    }

    /// <summary>
    ///     The key that was pressed. If the value is <see cref="Sharpie.Key.Character" /> then <see cref="Char" /> contains
    ///     the actual code of the key.
    /// </summary>
    public Key Key { get; internal init; }

    /// <summary>
    ///     The character that was pressed. Only valid if <see cref="Key" /> is <see cref="Sharpie.Key.Character" />.
    /// </summary>
    public Rune Char { get; internal init; }

    /// <summary>
    ///     The printable name of the key.
    /// </summary>
    public string? Name { get; internal init; }

    /// <summary>
    ///     The modifier keys that were present at the time.
    /// </summary>
    public ModifierKey Modifiers { get; internal init; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString()
    {
        var modifiers = new StringBuilder();
        if (Modifiers.HasFlag(ModifierKey.Ctrl))
        {
            modifiers.Append("CTRL-");
        }

        if (Modifiers.HasFlag(ModifierKey.Shift))
        {
            modifiers.Append("SHIFT-");
        }

        if (Modifiers.HasFlag(ModifierKey.Alt))
        {
            modifiers.Append("ALT-");
        }

        return Key == Key.Character
            ? $"Key [{modifiers}'{Name ?? Char.ToString()}']"
            : $"Key [{modifiers}{Name ?? Key.ToString()}]";
    }

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) =>
        obj is KeyEvent ke &&
        ke.Char == Char &&
        ke.Key == Key &&
        ke.Modifiers == Modifiers &&
        ke.Name == Name &&
        ke.Type == Type &&
        obj.GetType() == GetType();

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => HashCode.Combine(Char, Key, Modifiers, Name, Type);
}
