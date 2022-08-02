// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Colors;
using AppMotor.Core.Globalization;

namespace AppMotor.CliApp.Terminals.Formatting;

public abstract class TerminalWriterBase : ITerminalWriter
{
    /// <inheritdoc />
    public NewLineTypes NewLine
    {
        get => this._newLine;
        set
        {
            this._newLine = value;
            this._newLineAsString = value.GetNewLineCharacters();
        }
    }

    private NewLineTypes _newLine = NewLineTypes.SystemDefault;

    private string _newLineAsString = NewLineTypes.SystemDefault.GetNewLineCharacters();

    /// <inheritdoc />
    public IFormatProvider Culture { get; set; } = UICulture.FormatsAndSorting;

    /// <inheritdoc />
    public bool EnableColors { get; set; } = true;

    private readonly object _lock = new();

    protected TerminalWriterBase()
    {
        // See: https://no-color.org/
        if (Environment.GetEnvironmentVariable("NO_COLOR")?.Length > 0)
        {
            this.EnableColors = false;
        }
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        lock (this._lock)
        {
            WriteCore(value);
        }
    }

    /// <inheritdoc />
    public void WriteLine(string? value)
    {
        lock (this._lock)
        {
            if (!string.IsNullOrEmpty(value))
            {
                WriteCore(value);
            }

            WriteCore(this._newLineAsString);
        }
    }

    protected abstract void WriteCore(string value);

    protected abstract class SuppressAllAnsiSequencesStreamParserBase : AnsiStreamParser
    {
        /// <inheritdoc />
        protected sealed override void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence)
        {
            // Suppress
        }
    }

    protected abstract class SuppressAnsiColorSequencesStreamParserBase : AnsiColorStreamParser
    {
        /// <inheritdoc />
        protected override void OnTextColor(ConsoleColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnTextColor(int colorIndex)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnTextColor(RgbColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(ConsoleColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(int colorIndex)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(RgbColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        protected override void OnResetColors(bool resetForegroundColor, bool resetBackgroundColor)
        {
            // Suppress
        }
    }
}
