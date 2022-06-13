// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

public static class AnsiEscapeSequence
{
    public const char END_OF_ESCAPE_SEQUENCE_CHAR = 'm';

    public const string ESCAPE_SEQUENCE = "\u001B[";

    public const string RESET_SEQUENCE = ESCAPE_SEQUENCE + "0m";

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
