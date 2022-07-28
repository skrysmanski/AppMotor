// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

public static class AnsiEscapeSequence
{
    public const char ESCAPE_CHAR = '\u001B';

    public const char ESCAPE_SEQUENCE_CHAR2 = '[';

    public const char END_OF_ESCAPE_SEQUENCE_CHAR = 'm';

    public const string ESCAPE_SEQUENCE = "\u001B[";

    public const string RESET_SEQUENCE = ESCAPE_SEQUENCE + "0m";

    /// <summary>
    /// The minimum length of the smallest valid escape sequence: <c>\x1B[Nm</c> with "N" being a character/digit.
    /// </summary>
    public const int ESCAPE_SEQUENCE_MIN_LENGTH = 4;

    [MustUseReturnValue]
    public static string Create(int content)
    {
        return $"{ESCAPE_SEQUENCE}{content}{END_OF_ESCAPE_SEQUENCE_CHAR}";
    }

    [MustUseReturnValue]
    public static string Create(string content)
    {
        return $"{ESCAPE_SEQUENCE}{content}{END_OF_ESCAPE_SEQUENCE_CHAR}";
    }
}
