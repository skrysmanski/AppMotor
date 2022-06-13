// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics;

using AppMotor.Core.Colors;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// Contains a certain ANSI style (certain color, underlined, ...) and allows you to create strings that are styled in this
/// style (e.g. color a text, make it underlined). You can either use one of the <c>Text()</c> methods or use of one of the
/// other methods that accept a string a parameter.
///
/// <para>You can use the methods without "text" parameter to add chain styles; see <see cref="TermText"/> for details.</para>
///
/// <para>To get a instance of this type, you use the <see cref="TermText"/> class.</para>
/// </summary>
/// <remarks>
/// For more details, see: https://en.wikipedia.org/wiki/ANSI_escape_code
/// </remarks>
[DebuggerDisplay(nameof(_format))]
public readonly partial struct AnsiTextFormatter : IEquatable<AnsiTextFormatter>
{
    private readonly string _format;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <remarks>
    /// Instances of this type should not be created directly. Instead, use <see cref="TermText"/>.
    /// </remarks>
    public AnsiTextFormatter()
        : this(format: "")
    {
    }

    private AnsiTextFormatter(string format)
    {
        this._format = format;
    }

    /// <summary>
    /// Returns the specified character with the current style.
    /// </summary>
    [MustUseReturnValue]
    public string Text(char ch)
    {
        return this._format + ch + AnsiEscapeSequence.RESET_SEQUENCE;
    }

    /// <summary>
    /// Returns the specified text with the current style.
    /// </summary>
    [MustUseReturnValue]
    public string Text(string text)
    {
        if (this._format.Length != 0)
        {
            // If the text already contains ANSI color/formatting sequences, make sure we reapply
            // our own formatting after them. Otherwise, something like "Green("green" + Blue("blue") + "also green")
            // wouldn't work.
            text = text.Replace(AnsiEscapeSequence.RESET_SEQUENCE, AnsiEscapeSequence.RESET_SEQUENCE + this._format);
        }

        return this._format + text + AnsiEscapeSequence.RESET_SEQUENCE;
    }

    /// <summary>
    /// Chains style: underlined
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Underline() => new(this._format + AnsiEscapeSequence.Create(content: "4"));

    /// <summary>
    /// Returns the specified text with the current style plus: underlined
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public string Underline(string text) => Underline().Text(text);

    /// <summary>
    /// Chains style: inverse
    /// </summary>
    /// <remarks>
    /// Inverse switches background and foreground color. Note that this also works
    /// if foreground and background color are "unknown".
    /// </remarks>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Inverse() => new(this._format + AnsiEscapeSequence.Create(content: "7"));

    /// <summary>
    /// Returns the specified text with the current style plus: inverse
    /// </summary>
    /// <remarks>
    /// Inverse switches background and foreground color. Note that this also works
    /// if foreground and background color are "unknown".
    /// </remarks>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public string Inverse(string text) => Inverse().Text(text);

    #region Text Color

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter In(ConsoleColor color)
    {
        int ansiColor = AnsiColorConverter.ConvertToAnsiColor(color);
        return new AnsiTextFormatter(this._format + AnsiEscapeSequence.Create(content: ansiColor));
    }

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Rgb(int r, int g, int b) => new(this._format + AnsiEscapeSequence.Create(content: $"38;2;{r};{g};{b}"));

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Rgb(RgbColor color) => Rgb(color.R, color.G, color.B);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Rgb<TColor>(TColor color) where TColor : IColor => Rgb(color.ToRgb());

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Hex(string hexColor) => Rgb(new HexColor(hexColor));

    #endregion Text Color

    #region Background Color

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Bg(ConsoleColor color)
    {
        int ansiColor = AnsiColorConverter.ConvertToAnsiColor(color) + AnsiColorConverter.BACKGROUND_COLOR_OFFSET;

        return new AnsiTextFormatter(this._format + AnsiEscapeSequence.Create(content: ansiColor));
    }

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgRgb(int r, int g, int b) => new(this._format + AnsiEscapeSequence.Create(content: $"48;2;{r};{g};{b}"));

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgRgb(RgbColor color) => BgRgb(color.R, color.G, color.B);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgRgb<TColor>(TColor color) where TColor : IColor => BgRgb(color.ToRgb());

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgHex(string hexColor) => BgRgb(new HexColor(hexColor));

    #endregion Background Color

    /// <inheritdoc />
    public bool Equals(AnsiTextFormatter other)
    {
        return this._format == other._format;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is AnsiTextFormatter other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._format.GetHashCode();
    }

    /// <summary>
    /// Operator ==
    /// </summary>
    public static bool operator ==(AnsiTextFormatter left, AnsiTextFormatter right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Operator !=
    /// </summary>
    public static bool operator !=(AnsiTextFormatter left, AnsiTextFormatter right)
    {
        return !left.Equals(right);
    }
}
