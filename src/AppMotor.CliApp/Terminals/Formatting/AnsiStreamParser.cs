// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// A parser for strings with ANSI escape sequences (see <see cref="AnsiEscapeSequence"/>).
/// See <see cref="ParseNext"/> for more details.
/// </summary>
public abstract class AnsiStreamParser
{
    private readonly StringBuilder _escapeSequenceBuilder = new();

    /// <summary>
    /// Parses the specified text. Note that the text is interpreted as continuation of any
    /// previously parsed text (i.e. this parser is stateful and can't be shared among different
    /// "text streams").
    ///
    /// <para>For any detected escape sequence, <see cref="OnAnsiEscapeSequence"/> is called. For any
    /// regular text, <see cref="OnText"/> is called.</para>
    /// </summary>
    /// <param name="text">The next part in the "text stream"</param>
    /// <remarks>
    /// This method supports incomplete escape sequences at the end of <paramref name="text"/>. In this
    /// case, the escape sequence can/must be complete with the next call(s) to this method.
    /// </remarks>
    public void ParseNext(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return;
        }

        if (this._escapeSequenceBuilder.Length != 0)
        {
            if (this._escapeSequenceBuilder.Length == 1) // just the escape character
            {
                if (text[0] == AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE_CHAR2) // begin of escape sequence
                {
                    int endOfEscapeSequence = text.IndexOf(AnsiEscapeSequence.END_OF_ESCAPE_SEQUENCE_CHAR);
                    if (endOfEscapeSequence == -1)
                    {
                        this._escapeSequenceBuilder.Append(text);
                        return;
                    }

                    this._escapeSequenceBuilder.Append(text[0..(endOfEscapeSequence + 1)]);
                    OnAnsiEscapeSequence(this._escapeSequenceBuilder.ToString());
                    this._escapeSequenceBuilder.Clear();

                    if (endOfEscapeSequence + 1 < text.Length)
                    {
                        text = text[(endOfEscapeSequence + 1)..];
                    }
                    else
                    {
                        // End of text
                        return;
                    }
                }
                else // invalid escape sequence - interpret it as text
                {
                    OnText(this._escapeSequenceBuilder.ToString());
                    this._escapeSequenceBuilder.Clear();
                }
            }
            else // started escape sequence
            {
                int endOfEscapeSequence = text.IndexOf(AnsiEscapeSequence.END_OF_ESCAPE_SEQUENCE_CHAR);
                if (endOfEscapeSequence == -1)
                {
                    this._escapeSequenceBuilder.Append(text);
                    return;
                }

                this._escapeSequenceBuilder.Append(text[0..(endOfEscapeSequence + 1)]);
                OnAnsiEscapeSequence(this._escapeSequenceBuilder.ToString());
                this._escapeSequenceBuilder.Clear();

                if (endOfEscapeSequence + 1 < text.Length)
                {
                    text = text[(endOfEscapeSequence + 1)..];
                }
                else
                {
                    // End of text
                    return;
                }
            }
        }

        while (true)
        {
            if (text[0] == AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE_CHAR1)
            {
                if (text.Length == 1)
                {
                    // Incomplete escape sequence
                    this._escapeSequenceBuilder.Append(text);
                    return;
                }
                else if (text[1] != AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE_CHAR2)
                {
                    // invalid escape sequence - interpret it as text
                    var nextEscapeCharIndex = text[1..].IndexOf(AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE_CHAR1);
                    if (nextEscapeCharIndex == -1)
                    {
                        // No more escape characters found
                        OnText(text);
                        return;
                    }

                    OnText(text[0..nextEscapeCharIndex]);
                    text = text[nextEscapeCharIndex..];
                }
                else
                {
                    if (text.Length >= AnsiEscapeSequence.ESCAPE_SEQUENCE_MIN_LENGTH)
                    {
                        int endOfEscapeSequence = text.IndexOf(AnsiEscapeSequence.END_OF_ESCAPE_SEQUENCE_CHAR);
                        if (endOfEscapeSequence != -1)
                        {
                            // Complete sequence in text
                            OnAnsiEscapeSequence(text[0..(endOfEscapeSequence + 1)]);
                            if (endOfEscapeSequence + 1 >= text.Length)
                            {
                                // End of text
                                return;
                            }

                            text = text[(endOfEscapeSequence + 1)..];
                            continue;
                        }
                    }

                    // Incomplete escape sequence
                    this._escapeSequenceBuilder.Append(text);
                    return;
                }
            }
            else
            {
                var nextEscapeCharIndex = text.IndexOf(AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE_CHAR1);
                if (nextEscapeCharIndex == -1)
                {
                    // No more escape characters found
                    OnText(text);
                    return;
                }

                OnText(text[0..nextEscapeCharIndex]);
                text = text[nextEscapeCharIndex..];
            }
        }
    }

    /// <summary>
    /// Called for every detected ANSI escape sequence.
    /// </summary>
    /// <param name="escapeSequence">The whole escape sequence (including <see cref="AnsiEscapeSequence.START_OF_ESCAPE_SEQUENCE"/>
    /// and <see cref="AnsiEscapeSequence.END_OF_ESCAPE_SEQUENCE_CHAR"/>).</param>
    protected abstract void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence);

    /// <summary>
    /// Called for every text that's not an ANSI escape sequence.
    /// </summary>
    protected abstract void OnText(ReadOnlySpan<char> text);

}
