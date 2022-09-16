// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Colors;
using AppMotor.Core.Globalization;
using AppMotor.Core.Utils;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Abstract base implementation for <see cref="ITerminalWriter"/> - i.e. makes implementing <see cref="ITerminalWriter"/>
/// easier. Also has basic support for https://no-color.org/ (i.e. sets <see cref="EnableColors"/> to <c>false</c>, if the
/// "NO_COLOR" environment variable is set).
/// </summary>
public abstract class TerminalWriterBase : ITerminalWriter
{
    /// <inheritdoc />
    public NewLineTypes NewLine { get; set; } = NewLineTypes.SystemDefault;

    /// <inheritdoc />
    public IFormatProvider Culture { get; set; } = UICulture.FormatsAndSorting;

    /// <inheritdoc />
    public bool EnableColors { get; set; } = true;

    private readonly object _lock = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    [ExcludeFromCodeCoverage]
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

    /// <summary>
    /// Writes the specified string to the terminal. Note that this implementation should
    /// respect <see cref="EnableColors"/> to disable colors if this property is <c>false</c>.
    /// </summary>
    /// <param name="value">The value to string. Is never null or empty.</param>
    protected abstract void WriteCore(string value);

    /// <summary>
    /// An <see cref="AnsiStreamParser"/> that discards all ANSI escape sequences (both colors and formatting).
    /// This is useful for terminals that neither support ANSI escape sequences nor colors (e.g. unit test output).
    /// If you still want/need escape sequences but no colors (e.g. for: https://no-color.org/), use
    /// <see cref="SuppressAnsiColorSequencesStreamParserBase"/> instead.
    /// </summary>
    protected abstract class SuppressAllAnsiSequencesStreamParserBase : AnsiStreamParser
    {
        /// <inheritdoc />
        protected sealed override void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence)
        {
            // Suppress
        }
    }

    /// <summary>
    /// An <see cref="AnsiStreamParser"/> that discards all colors but keeps any other ANSI
    /// escape sequence (i.e. formatting like underlined). The primary use case for this class
    /// is to support https://no-color.org/ .
    /// </summary>
    protected abstract class SuppressAnsiColorSequencesStreamParserBase : AnsiColorStreamParser
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnTextColor(ConsoleColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnTextColor(int colorIndex)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnTextColor(RgbColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnBackgroundColor(ConsoleColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnBackgroundColor(int colorIndex)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnBackgroundColor(RgbColor color)
        {
            // Suppress
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void OnResetColors(bool resetForegroundColor, bool resetBackgroundColor)
        {
            // Suppress
        }
    }
}
