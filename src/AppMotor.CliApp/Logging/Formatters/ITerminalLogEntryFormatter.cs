// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AppMotor.CliApp.Logging.Formatters;

/// <summary>
/// Defines the format for a <see cref="LogEntry{TState}"/> to be logged. Note that the log message
/// itself should/will be formatted via <see cref="LogEntry{TState}.Formatter"/> instead.
/// </summary>
public interface ITerminalLogEntryFormatter
{
    /// <summary>
    /// The name of the formatter. This formatter is used if its name is specified in <see cref="ConsoleLoggerOptions.FormatterName"/>.
    /// </summary>
    /// <seealso cref="ConsoleFormatterNames"/>
    string Name { get; }

    /// <summary>
    /// Writes the log entry to the specified <see cref="TextWriter"/>. Note that no(!) trailing line break should be included.
    /// </summary>
    /// <remarks>
    /// If the formatter wants to write colors to the console, it can do so by embedding <see cref="TermText"/> strings
    /// in the output.
    /// </remarks>
    void Write<TState>(LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
}
