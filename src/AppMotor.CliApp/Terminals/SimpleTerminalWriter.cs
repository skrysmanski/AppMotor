// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// A simple implementation of <see cref="ITerminal"/> that passes the string to write
/// to a delegate (specified in the constructor).
/// </summary>
/// <remarks>
/// Respects https://no-color.org/ via <see cref="TerminalWriterBase.EnableColors"/>.
/// </remarks>
public class SimpleTerminalWriter : TerminalWriterBase
{
    private readonly Action<string> _writeFunc;

    private readonly SuppressAnsiColorSequencesStreamParser _noColorStreamParser;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="writeFunc">This delegate is called for every string to write to the terminal.
    /// Note that the terminal must support ANSI escape sequences (see <see cref="AnsiEscapeSequence"/>).</param>
    public SimpleTerminalWriter(Action<string> writeFunc)
    {
        this._writeFunc = writeFunc;
        this._noColorStreamParser = new SuppressAnsiColorSequencesStreamParser(writeFunc);
    }

    /// <inheritdoc />
    protected override void WriteCore(string value)
    {
        if (this.EnableColors)
        {
            this._writeFunc(value);
        }
        else
        {
            this._noColorStreamParser.ParseNext(value);
        }
    }

    /// <summary>
    /// Suppresses all colors but leaves any other formatting untouched.
    /// </summary>
    private sealed class SuppressAnsiColorSequencesStreamParser : SuppressAnsiColorSequencesStreamParserBase
    {
        private readonly Action<string> _writeFunc;

        public SuppressAnsiColorSequencesStreamParser(Action<string> writeFunc)
        {
            this._writeFunc = writeFunc;
        }

        /// <inheritdoc />
        protected override void OnText(ReadOnlySpan<char> text)
        {
            this._writeFunc(text.ToString());
        }

        /// <inheritdoc />
        protected override void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents)
        {
            this._writeFunc(AnsiEscapeSequence.Create(escapeSequenceContents));
        }
    }
}
