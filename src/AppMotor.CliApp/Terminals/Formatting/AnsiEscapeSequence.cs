// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// Utility class for working with ANSI escape sequences (see:
/// https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_(Select_Graphic_Rendition)_parameters ).
/// </summary>
public static class AnsiEscapeSequence
{
    /// <summary>
    /// The first character (of two) that starts an ANSI escape sequence. The second is <see cref="START_OF_ESCAPE_SEQUENCE_CHAR2"/>.
    /// </summary>
    /// <remarks>
    /// To avoid any confusion: Both characters must be present to start an escape sequence (like in <see cref="START_OF_ESCAPE_SEQUENCE"/>).
    /// It's not(!) that you can choose one or the other.
    /// </remarks>
    /// <seealso cref="END_OF_ESCAPE_SEQUENCE_CHAR"/>
    [PublicAPI]
    public const char START_OF_ESCAPE_SEQUENCE_CHAR1 = '\u001B';

    /// <summary>
    /// The second character (of two) that starts an ANSI escape sequence. The first is <see cref="START_OF_ESCAPE_SEQUENCE_CHAR1"/>.
    /// </summary>
    /// <remarks>
    /// To avoid any confusion: Both characters must be present to start an escape sequence (like in <see cref="START_OF_ESCAPE_SEQUENCE"/>).
    /// It's not(!) that you can choose one or the other.
    /// </remarks>
    /// <seealso cref="END_OF_ESCAPE_SEQUENCE_CHAR"/>
    [PublicAPI]
    public const char START_OF_ESCAPE_SEQUENCE_CHAR2 = '[';

    /// <summary>
    /// This character ends an ANSI escape sequence.
    /// </summary>
    /// <seealso cref="START_OF_ESCAPE_SEQUENCE_CHAR1"/>
    /// <seealso cref="START_OF_ESCAPE_SEQUENCE_CHAR2"/>
    [PublicAPI]
    public const char END_OF_ESCAPE_SEQUENCE_CHAR = 'm';

    /// <summary>
    /// This string is required to start an ANSI escape sequence.
    /// </summary>
    [PublicAPI]
    public const string START_OF_ESCAPE_SEQUENCE = "\u001B[";

    /// <summary>
    /// This escape sequence resets any effects/formatting that previous escape sequences enabled.
    /// </summary>
    [PublicAPI]
    public const string RESET_SEQUENCE = START_OF_ESCAPE_SEQUENCE + "0m";

    /// <summary>
    /// The minimum length of the smallest valid escape sequence: <c>\x1B[Nm</c> with "N" being a character/digit.
    /// </summary>
    [PublicAPI]
    public const int ESCAPE_SEQUENCE_MIN_LENGTH = 4;

    /// <summary>
    /// Creates an ANSI escape sequence with the specified content.
    /// </summary>
    [MustUseReturnValue]
    public static string Create(int content)
    {
        return $"{START_OF_ESCAPE_SEQUENCE}{content}{END_OF_ESCAPE_SEQUENCE_CHAR}";
    }

    /// <summary>
    /// Creates an ANSI escape sequence with the specified content.
    /// </summary>
    [MustUseReturnValue]
    public static string Create(string content)
    {
        return $"{START_OF_ESCAPE_SEQUENCE}{content}{END_OF_ESCAPE_SEQUENCE_CHAR}";
    }

    /// <summary>
    /// Creates an ANSI escape sequence with the specified content.
    /// </summary>
    [MustUseReturnValue]
    public static string Create(ReadOnlySpan<char> content)
    {
        return $"{START_OF_ESCAPE_SEQUENCE}{content}{END_OF_ESCAPE_SEQUENCE_CHAR}";
    }

    /// <summary>
    /// Extracts the content from the specified escape sequence.
    /// </summary>
    /// <remarks>
    /// This method does not check whether the passed value is actually an escape sequence. It simply
    /// assumes that it is one. It also assumes the sequence is long enough (otherwise there will
    /// be an <see cref="ArgumentOutOfRangeException"/>).
    /// </remarks>
    [MustUseReturnValue]
    public static ReadOnlySpan<char> ExtractSequenceContent(ReadOnlySpan<char> escapeSequence)
    {
        return escapeSequence[2..^1];
    }
}
