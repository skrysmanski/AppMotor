// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using AppMotor.Core.Collections;
using AppMotor.Core.DataModel;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents a string that contains (potentially) colored substrings. Each colored substring is represented
/// by a <see cref="ColoredSubstring"/> instance - created through one of the <c>TextIn...</c> classes. The primary
/// goal of this class is to be able to make color console output (via <see cref="Terminal"/>).
/// </summary>
public sealed class ColoredString : IReadOnlyList<ColoredSubstring>, ICloneable<ColoredString>
{
    private readonly AppendOnlyList<ColoredSubstring> _substrings;

    /// <inheritdoc />
    public int Count => this._substrings.Count;

    /// <inheritdoc />
    public ColoredSubstring this[int index] => this._substrings[index];

    private ColoredString(ColoredString? other)
    {
        if (other is not null)
        {
            this._substrings = other._substrings.CloneShallow();
        }
        else
        {
            this._substrings = new AppendOnlyList<ColoredSubstring>();
        }
    }

    /// <summary>
    /// Creates a new empty <see cref="ColoredString"/>.
    /// </summary>
    [PublicAPI, Pure]
    public static ColoredString New()
    {
        return new(null);
    }

    /// <summary>
    /// Appends the specified colored string (<paramref name="value"/>) to this colored
    /// string.
    /// </summary>
    /// <param name="color">The color to be used for uncolored substrings in <paramref name="value"/>.
    /// If <c>null</c>, they will remain uncolored.</param>
    /// <param name="value">The color string to append.</param>
    /// <returns>Returns this instance (useful for chaining calls).</returns>
    [PublicAPI]
    public ColoredString Append(ConsoleColor? color, ColoredString? value)
    {
        if (value is null || value.Count == 0)
        {
            return this;
        }

        if (color is null)
        {
            //
            // No change in color. We can use the more efficient "AppendRange()" method.
            //
            this._substrings.AppendRange(value._substrings);
        }
        else
        {
            //
            // Change color of uncolored substrings.
            //
            IReadOnlyList<ColoredSubstring> substringsToAdd;

            if (ReferenceEquals(this._substrings, value._substrings))
            {
                // Create copy because we can't iterate and modify the
                // same collection at the same time.
                substringsToAdd = value._substrings.ToList();
            }
            else
            {
                substringsToAdd = value._substrings;
            }

            foreach (var coloredSubstring in substringsToAdd)
            {
                if (coloredSubstring.Color is null)
                {
                    // Set color.
                    this._substrings.Append(new ColoredSubstring(color, coloredSubstring.Text));
                }
                else
                {
                    this._substrings.Append(coloredSubstring);
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Appends the string value with the specified color.
    /// </summary>
    /// <param name="color">The color to use for the string. Can be <c>null</c>. See
    /// <see cref="ColoredSubstring.Color"/> for more details.</param>
    /// <param name="value">The value to append</param>
    /// <returns>Returns this instance (useful for chaining calls).</returns>
    /// <remarks>
    /// This overload exists because when appending Substrings, the compiler would first
    /// convert them into a <see cref="ColoredString"/> instance and then use
    /// <see cref="Append(ConsoleColor?,ColoredString)"/> instead of
    /// <see cref="Append(ConsoleColor?,object)"/> (resulting in unnecessary conversions).
    /// </remarks>
    [PublicAPI]
    public ColoredString Append(ConsoleColor? color, ColoredSubstring? value)
    {
        if (value is null)
        {
            return this;
        }

        if (color is null)
        {
            return Append(value);
        }
        else
        {
            return Append(color, value.Text);
        }
    }

    /// <summary>
    /// Appends the substring to this string.
    /// </summary>
    /// <returns>Returns this instance (useful for chaining calls).</returns>
    [PublicAPI]
    public ColoredString Append(ColoredSubstring? substring)
    {
        if (substring is null || string.IsNullOrEmpty(substring.Text))
        {
            // If it's empty, don't add it (wouldn't be visible anyways).
            return this;
        }

        this._substrings.Append(substring);

        return this;
    }

    /// <summary>
    /// Appends the string value with the specified color.
    /// </summary>
    /// <param name="color">The color to use for the string. Can be <c>null</c>. See
    /// <see cref="ColoredSubstring.Color"/> for more details.</param>
    /// <param name="value">The value to append</param>
    /// <returns>Returns this instance (useful for chaining calls).</returns>
    /// <remarks>
    /// This overload exists because when appending strings, the compiler would first
    /// convert them into a <see cref="ColoredString"/> instance and then use
    /// <see cref="Append(ConsoleColor?,ColoredString)"/> instead of
    /// <see cref="Append(ConsoleColor?,object)"/> (resulting in unnecessary conversions).
    /// </remarks>
    [PublicAPI]
    public ColoredString Append(ConsoleColor? color, [Localizable(true)] string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            // If it's null or empty, don't add it (wouldn't be visible anyways).
            return this;
        }

        this._substrings.Append(new ColoredSubstring(color, value));

        return this;
    }

    /// <summary>
    /// Appends the value (converted to a string via <c>ToString()</c>) with the specified color.
    /// </summary>
    /// <param name="color">The color to use for the string. Can be <c>null</c>. See
    /// <see cref="ColoredSubstring.Color"/> for more details.</param>
    /// <param name="value">The value to append</param>
    /// <returns>Returns this instance (useful for chaining calls).</returns>
    [PublicAPI]
    public ColoredString Append(ConsoleColor? color, [Localizable(true)] object? value)
    {
        switch (value)
        {
            case ColoredString coloredString:
                return Append(color, coloredString);

            case ColoredSubstring substring:
                return Append(color, substring);

            case null:
                // If it's null, don't add it (wouldn't be visible anyways).
                return this;
        }

        var valueAsString = value.ToString();
        if (string.IsNullOrEmpty(valueAsString))
        {
            // If it's empty, don't add it (wouldn't be visible anyways).
            return this;
        }

        this._substrings.Append(new ColoredSubstring(color, valueAsString));

        return this;
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +(ColoredString? left, [Localizable(true)] string? right)
    {
        return new ColoredString(left).Append(color: null, right);
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +(ColoredString? left, ColoredSubstring? right)
    {
        return new ColoredString(left).Append(right);
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +(ColoredString? left, ColoredString? right)
    {
        return new ColoredString(left).Append(color: null, right);
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +([Localizable(true)] string? left, ColoredString? right)
    {
        return New().Append(color: null, left).Append(color: null, right);
    }

    /// <summary>
    /// Implicitly converts <see cref="ColoredSubstring"/> into <see cref="ColoredString"/>.
    /// </summary>
    public static implicit operator ColoredString(ColoredSubstring? text)
    {
        return New().Append(text);
    }

    /// <summary>
    /// Implicitly converts <see cref="string"/> into <see cref="ColoredString"/>.
    /// </summary>
    public static implicit operator ColoredString([Localizable(true)] string? text)
    {
        return New().Append(color: null, (object?)text);
    }

    #region ICollection<T>

    /// <inheritdoc />
    public IEnumerator<ColoredSubstring> GetEnumerator()
    {
        return this._substrings.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion ICollection<T>

    /// <inheritdoc />
    public ColoredString Clone()
    {
        return new(this);
    }

    /// <summary>
    /// Returns this colored string as plain text.
    /// </summary>
    public override string ToString()
    {
        return string.Join("", this._substrings.Select(sub => sub.Text));
    }
}

/// <summary>
/// Represents a (potentially) colored string. Note that unlike <see cref="ColoredString"/>,
/// instances of this class represent strings where the whole(!) string has the same color.
///
/// <para>Note: To create instances of this class, use the various <c>TextIn...</c> classes.</para>
/// </summary>
public class ColoredSubstring : IEquatable<ColoredSubstring>
{
    /// <summary>
    /// The text value of this colored string. Is never null but may be empty (when
    /// created via one of the <c>TextIn...</c> classes).
    /// </summary>
    [PublicAPI]
    public string Text { get; }

    /// <summary>
    /// The color of this string. Can be <c>null</c> in which case the color is
    /// "inherited" (e.g. it's the console's default color).
    /// </summary>
    [PublicAPI]
    public ConsoleColor? Color { get; }

    /// <summary>
    /// Constructor to create colored text dynamically.
    /// </summary>
    /// <param name="color">The color of the text. Use <c>null</c> if the text should use the default color.</param>
    /// <param name="text">The text.</param>
    public ColoredSubstring(ConsoleColor? color, string? text)
    {
        this.Text = text ?? "";
        this.Color = color;
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +(ColoredSubstring left, [Localizable(true)] string? right)
    {
        return ColoredString.New().Append(left).Append(null, right);
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +([Localizable(true)] string? left, ColoredSubstring right)
    {
        return ColoredString.New().Append(null, left).Append(right);
    }

    /// <summary>
    /// Concatenates both strings into a <see cref="ColoredString"/>.
    /// </summary>
    public static ColoredString operator +(ColoredSubstring left, ColoredSubstring right)
    {
        return ColoredString.New().Append(left).Append(right);
    }

    /// <inheritdoc />
    public bool Equals(ColoredSubstring? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Text == other.Text && this.Color == other.Color;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as ColoredSubstring);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Text, this.Color);
    }

    /// <summary>
    /// Returns the text value of this substring - i.e. <see cref="Text"/>.
    /// </summary>
    public override string ToString()
    {
        return this.Text;
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Black"/>.
///
/// <para>Use it like this: <c>(TextInBlack)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInBlack : ColoredSubstring
{
    private TextInBlack(string text)
        : base(ConsoleColor.Black, text)
    {
    }

    /// <summary>
    /// Converts this text into a black text.
    /// </summary>
    public static explicit operator TextInBlack([Localizable(true)] string? text)
    {
        return new TextInBlack(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkGray"/>.
///
/// <para>Use it like this: <c>(TextInDarkGray)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkGray : ColoredSubstring
{
    private TextInDarkGray(string text)
        : base(ConsoleColor.DarkGray, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark gray text.
    /// </summary>
    public static explicit operator TextInDarkGray([Localizable(true)] string? text)
    {
        return new TextInDarkGray(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Gray"/>.
///
/// <para>Use it like this: <c>(TextInGray)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInGray : ColoredSubstring
{
    private TextInGray(string text)
        : base(ConsoleColor.Gray, text)
    {
    }

    /// <summary>
    /// Converts this text into a gray text.
    /// </summary>
    public static explicit operator TextInGray([Localizable(true)] string? text)
    {
        return new TextInGray(text ?? "");
    }
}


/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.White"/>.
///
/// <para>Use it like this: <c>(TextInWhite)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInWhite : ColoredSubstring
{
    private TextInWhite(string text)
        : base(ConsoleColor.White, text)
    {
    }

    /// <summary>
    /// Converts this text into a white text.
    /// </summary>
    public static explicit operator TextInWhite([Localizable(true)] string? text)
    {
        return new TextInWhite(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkBlue"/>.
///
/// <para>Use it like this: <c>(TextInDarkBlue)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkBlue : ColoredSubstring
{
    private TextInDarkBlue(string text)
        : base(ConsoleColor.DarkBlue, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark blue text.
    /// </summary>
    public static explicit operator TextInDarkBlue([Localizable(true)] string? text)
    {
        return new TextInDarkBlue(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Blue"/>.
///
/// <para>Use it like this: <c>(TextInBlue)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInBlue : ColoredSubstring
{
    private TextInBlue(string text)
        : base(ConsoleColor.Blue, text)
    {
    }

    /// <summary>
    /// Converts this text into a blue text.
    /// </summary>
    public static explicit operator TextInBlue([Localizable(true)] string? text)
    {
        return new TextInBlue(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkGreen"/>.
///
/// <para>Use it like this: <c>(TextInDarkGreen)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkGreen : ColoredSubstring
{
    private TextInDarkGreen(string text)
        : base(ConsoleColor.DarkGreen, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark green text.
    /// </summary>
    public static explicit operator TextInDarkGreen([Localizable(true)] string? text)
    {
        return new TextInDarkGreen(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Green"/>.
///
/// <para>Use it like this: <c>(TextInGreen)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInGreen : ColoredSubstring
{
    private TextInGreen(string text)
        : base(ConsoleColor.Green, text)
    {
    }

    /// <summary>
    /// Converts this text into a green text.
    /// </summary>
    public static explicit operator TextInGreen([Localizable(true)] string? text)
    {
        return new TextInGreen(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkCyan"/>.
///
/// <para>Use it like this: <c>(TextInDarkCyan)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkCyan : ColoredSubstring
{
    private TextInDarkCyan(string text)
        : base(ConsoleColor.DarkCyan, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark cyan text.
    /// </summary>
    public static explicit operator TextInDarkCyan([Localizable(true)] string? text)
    {
        return new TextInDarkCyan(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Cyan"/>.
///
/// <para>Use it like this: <c>(TextInCyan)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInCyan : ColoredSubstring
{
    private TextInCyan(string text)
        : base(ConsoleColor.Cyan, text)
    {
    }

    /// <summary>
    /// Converts this text into a cyan text.
    /// </summary>
    public static explicit operator TextInCyan([Localizable(true)] string? text)
    {
        return new TextInCyan(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkRed"/>.
///
/// <para>Use it like this: <c>(TextInDarkRed)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkRed : ColoredSubstring
{
    private TextInDarkRed(string text)
        : base(ConsoleColor.DarkRed, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark red text.
    /// </summary>
    public static explicit operator TextInDarkRed([Localizable(true)] string? text)
    {
        return new TextInDarkRed(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Red"/>.
///
/// <para>Use it like this: <c>(TextInRed)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInRed : ColoredSubstring
{
    private TextInRed(string text)
        : base(ConsoleColor.Red, text)
    {
    }

    /// <summary>
    /// Converts this text into a red text.
    /// </summary>
    public static explicit operator TextInRed([Localizable(true)] string? text)
    {
        return new TextInRed(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkMagenta"/>.
///
/// <para>Use it like this: <c>(TextInDarkMagenta)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkMagenta : ColoredSubstring
{
    private TextInDarkMagenta(string text)
        : base(ConsoleColor.DarkMagenta, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark magenta text.
    /// </summary>
    public static explicit operator TextInDarkMagenta([Localizable(true)] string? text)
    {
        return new TextInDarkMagenta(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Magenta"/>.
///
/// <para>Use it like this: <c>(TextInMagenta)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInMagenta : ColoredSubstring
{
    private TextInMagenta(string text)
        : base(ConsoleColor.Magenta, text)
    {
    }

    /// <summary>
    /// Converts this text into a magenta text.
    /// </summary>
    public static explicit operator TextInMagenta([Localizable(true)] string? text)
    {
        return new TextInMagenta(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.DarkYellow"/>.
///
/// <para>Use it like this: <c>(TextInDarkYellow)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInDarkYellow : ColoredSubstring
{
    private TextInDarkYellow(string text)
        : base(ConsoleColor.DarkYellow, text)
    {
    }

    /// <summary>
    /// Converts this text into a dark yellow text.
    /// </summary>
    public static explicit operator TextInDarkYellow([Localizable(true)] string? text)
    {
        return new TextInDarkYellow(text ?? "");
    }
}

/// <summary>
/// Represents a text with text color <see cref="ConsoleColor.Yellow"/>.
///
/// <para>Use it like this: <c>(TextInYellow)"some text here"</c></para>
/// </summary>
[PublicAPI]
public sealed class TextInYellow : ColoredSubstring
{
    private TextInYellow(string text)
        : base(ConsoleColor.Yellow, text)
    {
    }

    /// <summary>
    /// Converts this text into a yellow text.
    /// </summary>
    public static explicit operator TextInYellow([Localizable(true)] string? text)
    {
        return new TextInYellow(text ?? "");
    }
}
