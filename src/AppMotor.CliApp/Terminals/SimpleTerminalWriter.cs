// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

namespace AppMotor.CliApp.Terminals;

public class SimpleTerminalWriter : TerminalWriterBase
{
    private readonly Action<string> _writeFunc;

    private readonly SuppressAnsiColorSequencesStreamParser _noColorStreamParser;

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
