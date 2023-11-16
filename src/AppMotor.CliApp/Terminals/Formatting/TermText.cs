// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Colors;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// Creates colored text for console applications.
/// </summary>
/// <example>
/// You can either create text with a single style via <c>TermText.Red("some text")</c>
/// or chain multiple styles like <c>TermText.Red().Underline().BgBlue("some text")</c>.
/// </example>
/// <remarks>
/// Under the hood, this class uses ANSI escape sequences. To make them available on
/// Windows, see <see cref="AnsiSupportOnWindows"/>.
/// </remarks>
/// <remarks>
/// This implementation doesn't support bold (<c>ESC[1m</c>) because: There are different interpretations on whether this means
/// actually "bold" as old(er) terminals simply interpreted this as "bright" (i.e. giving you the bright variant of a color).
/// Also, it seems not to be supported (out-of-the-box) in Windows Terminal.
///
/// <para>This implementation also doesn't support "strikethrough" (<c>ESC[9m</c>) because it's not supported on
/// Windows Server 2016 (which is basically Windows 10 1607 and minimum requirement on Windows).</para>
/// </remarks>
public static partial class TermText
{
    /// <summary>
    /// Chains style: underlined
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Underline() => new AnsiTextFormatter().Underline();

    /// <summary>
    /// Returns the specified text with style: underlined
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static string Underline(string text) => new AnsiTextFormatter().Underline(text);

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
    public static AnsiTextFormatter Inverse() => new AnsiTextFormatter().Inverse();

    /// <summary>
    /// Returns the specified text with style: inverse
    /// </summary>
    /// <remarks>
    /// Inverse switches background and foreground color. Note that this also works
    /// if foreground and background color are "unknown".
    /// </remarks>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static string Inverse(string text) => new AnsiTextFormatter().Inverse(text);

    #region Text Color

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter In(ConsoleColor? color) => new AnsiTextFormatter().In(color);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Rgb(int r, int g, int b) => new AnsiTextFormatter().Rgb(r, g, b);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Rgb(RgbColor? color) => new AnsiTextFormatter().Rgb(color);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Rgb<TColor>(TColor? color) where TColor : IColor => new AnsiTextFormatter().Rgb(color);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Rgb<TColor>(TColor? color) where TColor : struct, IColor => new AnsiTextFormatter().Rgb(color);

    /// <summary>
    /// Chains style: text color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Hex(string? hexColor) => new AnsiTextFormatter().Hex(hexColor);

    #endregion Text Color

    #region Background Color

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter Bg(ConsoleColor? color) => new AnsiTextFormatter().Bg(color);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter BgRgb(int r, int g, int b) => new AnsiTextFormatter().BgRgb(r, g, b);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter BgRgb(RgbColor? color) => new AnsiTextFormatter().BgRgb(color);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter BgRgb<TColor>(TColor? color) where TColor : IColor => new AnsiTextFormatter().BgRgb(color);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter BgRgb<TColor>(TColor? color) where TColor : struct, IColor => new AnsiTextFormatter().BgRgb(color);

    /// <summary>
    /// Chains style: background color
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    [MustUseReturnValue]
    public static AnsiTextFormatter BgHex(string? hexColor) => new AnsiTextFormatter().BgHex(hexColor);

    #endregion Background Color
}
