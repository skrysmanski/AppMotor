// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// Converts between <see cref="ConsoleColor"/>s and their ANSI color codes.
/// </summary>
public static class AnsiColorConverter
{
    /// <summary>
    /// Add this offset to a ANSI foreground/text color value to get the background color value.
    /// </summary>
    public const int BACKGROUND_COLOR_OFFSET = 10;

    /// <summary>
    /// Converts a <see cref="ConsoleColor"/> to its ANSI color codes. Note that the returned
    /// number is for the foreground/text color. For the respective background color, add
    /// <see cref="BACKGROUND_COLOR_OFFSET"/> to the returned value.
    /// </summary>
    [MustUseReturnValue]
    public static int ConvertToAnsiColor(ConsoleColor color)
    {
        // See: https://en.wikipedia.org/wiki/ANSI_escape_code#3-bit_and_4-bit
        return color switch
        {
            ConsoleColor.Black => 30,
            ConsoleColor.DarkBlue => 34,
            ConsoleColor.DarkGreen => 32,
            ConsoleColor.DarkCyan => 36,
            ConsoleColor.DarkRed => 31,
            ConsoleColor.DarkMagenta => 35,
            ConsoleColor.DarkYellow => 33,
            ConsoleColor.Gray => 37,
            ConsoleColor.DarkGray => 90,
            ConsoleColor.Blue => 94,
            ConsoleColor.Green => 92,
            ConsoleColor.Cyan => 96,
            ConsoleColor.Red => 91,
            ConsoleColor.Magenta => 95,
            ConsoleColor.Yellow => 93,
            ConsoleColor.White => 97,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, message: null),
        };
    }

    /// <summary>
    /// Converts an ANSI color code into its <see cref="ConsoleColor"/> equivalent.
    /// </summary>
    /// <param name="ansiColor">The ANSI color code; must be a foreground/text color code; if you
    /// have a background color code, subtract <see cref="BACKGROUND_COLOR_OFFSET"/>.</param>#
    /// <param name="bright">Whether the "bright" flag is set. Only affects the color codes 30 to 37.</param>
    [MustUseReturnValue]
    public static ConsoleColor ConvertToConsoleColor(int ansiColor, bool bright)
    {
        // See: https://en.wikipedia.org/wiki/ANSI_escape_code#3-bit_and_4-bit
        return ansiColor switch
        {
            30 => bright ? ConsoleColor.DarkGray    : ConsoleColor.Black,
            31 => bright ? ConsoleColor.Red         : ConsoleColor.DarkRed,
            32 => bright ? ConsoleColor.Green       : ConsoleColor.DarkGreen,
            33 => bright ? ConsoleColor.Yellow      : ConsoleColor.DarkYellow,
            34 => bright ? ConsoleColor.Blue        : ConsoleColor.DarkBlue,
            35 => bright ? ConsoleColor.Magenta     : ConsoleColor.DarkMagenta,
            36 => bright ? ConsoleColor.Cyan        : ConsoleColor.DarkCyan,
            37 => bright ? ConsoleColor.White       : ConsoleColor.Gray,

            90 => ConsoleColor.DarkGray,
            91 => ConsoleColor.Red,
            92 => ConsoleColor.Green,
            93 => ConsoleColor.Yellow,
            94 => ConsoleColor.Blue,
            95 => ConsoleColor.Magenta,
            96 => ConsoleColor.Cyan,
            97 => ConsoleColor.White,

            _ => throw new ArgumentOutOfRangeException(nameof(ansiColor), ansiColor, message: null),
        };
    }

    [MustUseReturnValue]
    public static bool Is4BitForegroundColor(int number)
    {
        return number is >= 30 and <= 37 or >= 90 and <= 97;
    }

    [MustUseReturnValue]
    public static bool Is4BitBackgroundColor(int number)
    {
        return number is >= 40 and <= 47 or >= 100 and <= 107;
    }

    /// <summary>
    /// Converts an 8 bit ANSI color index to its <see cref="ConsoleColor"/> equivalent.
    /// Only works for color index values from 0 to 15. Any other index will return <c>null</c>.
    /// </summary>
    [MustUseReturnValue]
    public static ConsoleColor? Convert8BitColorToConsoleColor(int colorIndex)
    {
        return colorIndex switch
        {
            0 => ConsoleColor.Black,
            1 => ConsoleColor.DarkRed,
            2 => ConsoleColor.DarkGreen,
            3 => ConsoleColor.DarkYellow,
            4 => ConsoleColor.DarkBlue,
            5 => ConsoleColor.DarkMagenta,
            6 => ConsoleColor.DarkCyan,
            7 => ConsoleColor.Gray,
            8 => ConsoleColor.DarkGray,
            9 => ConsoleColor.Red,
            10 => ConsoleColor.Green,
            11 => ConsoleColor.Yellow,
            12 => ConsoleColor.Blue,
            13 => ConsoleColor.Magenta,
            14 => ConsoleColor.Cyan,
            15 => ConsoleColor.White,
            _ => null,
        };
    }
}
