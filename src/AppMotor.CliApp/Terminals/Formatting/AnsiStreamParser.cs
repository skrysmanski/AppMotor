// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

namespace AppMotor.CliApp.Terminals.Formatting;

public abstract class AnsiStreamParser
{
    private readonly StringBuilder _escapeSequenceBuilder = new();

    protected abstract void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence);

    protected abstract void OnText(ReadOnlySpan<char> text);

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
                if (text[0] == AnsiEscapeSequence.ESCAPE_SEQUENCE_CHAR2) // begin of escape sequence
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
            if (text[0] == AnsiEscapeSequence.ESCAPE_CHAR)
            {
                if (text.Length == 1)
                {
                    // Incomplete escape sequence
                    this._escapeSequenceBuilder.Append(text);
                    return;
                }
                else if (text[1] != AnsiEscapeSequence.ESCAPE_SEQUENCE_CHAR2)
                {
                    // invalid escape sequence - interpret it as text
                    var nextEscapeCharIndex = text[1..].IndexOf(AnsiEscapeSequence.ESCAPE_CHAR);
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
                var nextEscapeCharIndex = text.IndexOf(AnsiEscapeSequence.ESCAPE_CHAR);
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
}
