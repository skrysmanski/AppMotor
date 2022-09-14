// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Runtime.CompilerServices;

using AppMotor.Core.Colors;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

/// <summary>
/// An <see cref="AnsiStreamParser"/> that converts all color sequences (text color and background color)
/// into method calls (like <see cref="OnTextColor(ConsoleColor)"/> but leaves any other escape sequence
/// untouched (reported via <see cref="OnNonColorAnsiEscapeSequence"/>). The primary use cases for class
/// are the support of https://no-color.org/ (see <see cref="TerminalWriterBase.SuppressAnsiColorSequencesStreamParserBase"/>)
/// and to support colors on terminals that don't support ANSI escape sequences.
/// </summary>
public abstract class AnsiColorStreamParser : AnsiStreamParser
{
    private bool _useBrightColor;

    /// <summary>
    /// Note that this is implemented as member variable rather than local variable
    /// to save on the allocation.
    /// </summary>
    private readonly List<Range> _curParamRanges = new();

    /// <summary>
    /// Called when the text/foreground color should be set the specified 4 bit color.
    /// </summary>
    protected abstract void OnTextColor(ConsoleColor color);

    /// <summary>
    /// Called when the text/foreground color should be set the specified 8 bit color.
    /// Note that for the values 0 - 15, <see cref="OnTextColor(System.ConsoleColor)"/>
    /// is called instead.
    /// </summary>
    /// <param name="colorIndex">The index (16 - 255) in the color table. For an explanation what
    /// each color index means, see: https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit </param>
    protected abstract void OnTextColor(int colorIndex);

    /// <summary>
    /// Called when the text/foreground color should be set the specified 24 bit color.
    /// </summary>
    protected abstract void OnTextColor(RgbColor color);

    /// <summary>
    /// Called when the background color should be set the specified 4 bit color.
    /// </summary>
    protected abstract void OnBackgroundColor(ConsoleColor color);

    /// <summary>
    /// Called when the background color should be set the specified 8 bit color.
    /// Note that for the values 0 - 15, <see cref="OnBackgroundColor(System.ConsoleColor)"/>
    /// is called instead.
    /// </summary>
    /// <param name="colorIndex">The index (16 - 255) in the color table. For an explanation what
    /// each color index means, see: https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit </param>
    protected abstract void OnBackgroundColor(int colorIndex);

    /// <summary>
    /// Called when the background color should be set the specified 24 bit color.
    /// </summary>
    protected abstract void OnBackgroundColor(RgbColor color);

    /// <summary>
    /// Called when the text/foreground and/or background color should be reset to its default.
    /// </summary>
    /// <remarks>
    /// If the ANSI escape sequence is "ESC[0m" (reset all colors and formatting), <see cref="OnNonColorAnsiEscapeSequence"/>
    /// will be called additionally to this method.
    /// </remarks>
    protected abstract void OnResetColors(bool resetForegroundColor, bool resetBackgroundColor);

    /// <summary>
    /// Called for any ANSI escape sequence that's not a color sequence. If an escape sequence consists
    /// of multiple parameters (separated by <c>;</c>), this method will be called for each parameter.
    /// </summary>
    /// <param name="escapeSequenceContents">The content/parameter of the ANSI sequence, i.e. without the escape
    /// sequence prefix (<c>\x1B[</c>) and postfix (<c>m</c>).</param>
    /// <remarks>
    /// This method will be called for the sequence "ESC[0m" (reset all colors and formatting) additionally to
    /// <see cref="OnResetColors"/>.
    /// </remarks>
    protected abstract void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents);

    /// <inheritdoc />
    protected sealed override void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence)
    {
        var escapeSequenceContents = AnsiEscapeSequence.ExtractSequenceContent(escapeSequence);

        switch (escapeSequenceContents.Length)
        {
            case 1:
            case 2:
            {
                var curParamNumber = TryParseNumber(escapeSequenceContents);
                if (curParamNumber is null || !ProcessWellKnownFixedLengthSequenceNumbers(curParamNumber.Value))
                {
                    switch (curParamNumber)
                    {
                        case 38:
                        case 48:
                            // These escape sequences require additional parameters. They're not provided here,
                            // so we ignore them completely.
                            return;
                    }

                    OnNonColorAnsiEscapeSequence(escapeSequenceContents);
                }
                break;
            }

            default: // other length; most likely parameter list
                FindParameters(escapeSequenceContents);

                for (var index = 0; index < this._curParamRanges.Count; index++)
                {
                    var curParamRange = this._curParamRanges[index];
                    var curParam = escapeSequenceContents[curParamRange];
                    var curParamNumber = TryParseNumber(curParam);
                    if (curParamNumber is null)
                    {
                        OnNonColorAnsiEscapeSequence(curParam);
                        continue;
                    }

                    if (ProcessWellKnownFixedLengthSequenceNumbers(curParamNumber.Value))
                    {
                        continue;
                    }

                    switch (curParamNumber)
                    {
                        case 38: // 8 bit or 24 bit text color
                        {
                            var colorType = TryParseColorTypeParameter(escapeSequenceContents, ref index);
                            switch (colorType)
                            {
                                case 5: // 8 bit color
                                    var colorIndex = TryParse8BitColor(escapeSequenceContents, ref index);
                                    if (colorIndex is null)
                                    {
                                        // Unexpected. Ignore the whole remaining escape sequence.
                                        return;
                                    }

                                    if (colorIndex.Value <= 15)
                                    {
                                        OnTextColor(AnsiColorConverter.Convert8BitColorToConsoleColor(colorIndex.Value)!.Value);
                                    }
                                    else
                                    {
                                        OnTextColor(colorIndex.Value);
                                    }
                                    break;

                                case 2: // 24 bit color
                                    var rgbColor = TryParse24BitColor(escapeSequenceContents, ref index);
                                    if (rgbColor is null)
                                    {
                                        // Unexpected. Ignore the whole remaining escape sequence.
                                        return;
                                    }

                                    OnTextColor(rgbColor.Value);
                                    break;

                                default:
                                    // Unexpected. Ignore the whole remaining escape sequence.
                                    return;
                            }

                            break;
                        }

                        case 48: // 8 bit or 24 bit background color
                        {
                            var colorType = TryParseColorTypeParameter(escapeSequenceContents, ref index);
                            switch (colorType)
                            {
                                case 5: // 8 bit color
                                    var colorIndex = TryParse8BitColor(escapeSequenceContents, ref index);
                                    if (colorIndex is null)
                                    {
                                        // Unexpected. Ignore the whole remaining escape sequence.
                                        return;
                                    }

                                    if (colorIndex.Value <= 15)
                                    {
                                        OnBackgroundColor(AnsiColorConverter.Convert8BitColorToConsoleColor(colorIndex.Value)!.Value);
                                    }
                                    else
                                    {
                                        OnBackgroundColor(colorIndex.Value);
                                    }
                                    break;

                                case 2: // 24 bit color
                                    var rgbColor = TryParse24BitColor(escapeSequenceContents, ref index);
                                    if (rgbColor is null)
                                    {
                                        // Unexpected. Ignore the whole remaining escape sequence.
                                        return;
                                    }

                                    OnBackgroundColor(rgbColor.Value);
                                    break;

                                default:
                                    // Unexpected. Ignore the whole remaining escape sequence.
                                    return;
                            }

                            break;
                        }

                        default:
                            OnNonColorAnsiEscapeSequence(curParam);
                            break;
                    }
                }
                break;
        }
    }

    [MustUseReturnValue]
    private bool ProcessWellKnownFixedLengthSequenceNumbers(int number)
    {
        switch (number)
        {
            case 0:
                OnResetColors(resetForegroundColor: true, resetBackgroundColor: true);
                OnNonColorAnsiEscapeSequence("0");
                return true;

            case 39: // default text color
                OnResetColors(resetForegroundColor: true, resetBackgroundColor: false);
                return true;

            case 49: // default background color
                OnResetColors(resetForegroundColor: false, resetBackgroundColor: true);
                return true;

            case 1:
                this._useBrightColor = true;
                return true;

            default:
                if (AnsiColorConverter.Is4BitForegroundColor(number))
                {
                    var color = AnsiColorConverter.ConvertToConsoleColor(number, this._useBrightColor);
                    OnTextColor(color);
                    return true;
                }
                else if (AnsiColorConverter.Is4BitBackgroundColor(number))
                {
                    // NOTE: "_useBrightColor" only affects the text color - not the background color (tested with Windows 11).
                    var color = AnsiColorConverter.ConvertToConsoleColor(number - AnsiColorConverter.BACKGROUND_COLOR_OFFSET, bright: false);
                    OnBackgroundColor(color);
                    return true;
                }
                else
                {
                    return false;
                }
        }
    }

    private static int? TryParseNumber(ReadOnlySpan<char> text)
    {
        switch (text.Length)
        {
            case 1:
                return TryParseDigit(text[0]);

            case 2:
            {
                var num1 = TryParseDigit(text[0]);
                if (num1 is null)
                {
                    return null;
                }
                var num2 = TryParseDigit(text[1]);
                if (num2 is null)
                {
                    return null;
                }
                return num1 * 10 + num2;
            }

            case 3: // primarily for RGB colors
            {
                var num1 = TryParseDigit(text[0]);
                if (num1 is null)
                {
                    return null;
                }
                var num2 = TryParseDigit(text[1]);
                if (num2 is null)
                {
                    return null;
                }
                var num3 = TryParseDigit(text[2]);
                if (num3 is null)
                {
                    return null;
                }
                return num1 * 100 + num2 * 10 + num3;
            }

            default:
                if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
                {
                    return number;
                }
                else
                {
                    return null;
                }
        }
    }

    private void FindParameters(ReadOnlySpan<char> escapeSequenceContents)
    {
        this._curParamRanges.Clear();

        var sequenceToSearch = escapeSequenceContents;
        int curParameterStart = 0;

        while (true)
        {
            int curParameterEnd = sequenceToSearch.IndexOf(';');
            if (curParameterEnd == -1)
            {
                curParameterEnd = sequenceToSearch.Length;
            }

            if (curParameterEnd != 0)
            {
                var curParamRange = new Range(curParameterStart, curParameterStart + curParameterEnd);
                this._curParamRanges.Add(curParamRange);
            }

            if (curParameterEnd >= sequenceToSearch.Length)
            {
                break;
            }
            else
            {
                curParameterStart += curParameterEnd + 1;
                sequenceToSearch = escapeSequenceContents[curParameterStart..];
            }
        }
    }

    private int? TryParseColorTypeParameter(ReadOnlySpan<char> escapeSequenceContents, ref int paramIndex)
    {
        if (paramIndex + 1 >= this._curParamRanges.Count)
        {
            return null;
        }

        paramIndex++;
        var colorTypeParameter = escapeSequenceContents[this._curParamRanges[paramIndex]];
        return TryParseNumber(colorTypeParameter);
    }

    [MustUseReturnValue]
    private int? TryParse8BitColor(ReadOnlySpan<char> escapeSequenceContents, ref int paramIndex)
    {
        if (paramIndex + 1 >= this._curParamRanges.Count)
        {
            return null;
        }

        paramIndex++;
        var colorTypeParameter = escapeSequenceContents[this._curParamRanges[paramIndex]];
        var colorIndex = TryParseNumber(colorTypeParameter);

        if (colorIndex is null or < 0 or > 255)
        {
            return null;
        }

        return colorIndex;
    }

    [MustUseReturnValue]
    private RgbColor? TryParse24BitColor(ReadOnlySpan<char> escapeSequenceContents, ref int paramIndex)
    {
        if (paramIndex + 3 >= this._curParamRanges.Count)
        {
            return null;
        }

        paramIndex++;
        var red = TryParseNumber(escapeSequenceContents[this._curParamRanges[paramIndex]]);
        if (red is null or < 0 or > 255)
        {
            return null;
        }

        paramIndex++;
        var green = TryParseNumber(escapeSequenceContents[this._curParamRanges[paramIndex]]);
        if (green is null or < 0 or > 255)
        {
            return null;
        }

        paramIndex++;
        var blue = TryParseNumber(escapeSequenceContents[this._curParamRanges[paramIndex]]);
        if (blue is null or < 0 or > 255)
        {
            return null;
        }

        return new RgbColor((byte)red, (byte)green, (byte)blue);
    }

    /// <summary>
    /// Returns the digit (0 - 9) for the specified character, if the character is a digit.
    /// Returns <c>null</c> otherwise.
    /// </summary>
    /// <remarks>
    /// This method is a tad faster than <see cref="char.IsDigit(char)"/>. Also it returns
    /// <c>true</c> only for the digits 0 - 9 - not for any other digit character (unlike
    /// <see cref="char.IsDigit(char)"/>).
    /// </remarks>
    [MustUseReturnValue]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int? TryParseDigit(char ch)
    {
        int potentialDigit = ch - '0';

        // NOTE: The "uint" conversion makes a negative int positive and thus
        //   the comparison false.
        return (uint)potentialDigit <= '9' - '0' ? potentialDigit : null;
    }
}
