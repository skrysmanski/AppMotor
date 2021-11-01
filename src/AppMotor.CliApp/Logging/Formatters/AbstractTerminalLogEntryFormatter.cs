// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging.Formatters;

/// <summary>
/// Abstract base class for implementations of <see cref="ITerminalLogEntryFormatter"/>.
/// </summary>
/// <typeparam name="TOptions">The options type to configure the formatter.</typeparam>
public abstract class AbstractTerminalLogEntryFormatter<TOptions> : Disposable, ITerminalLogEntryFormatter where TOptions : ConsoleFormatterOptions
{
    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// The options for this formatter.
    /// </summary>
    protected TOptions FormatterOptions { get; private set; }

    private readonly IDisposable? _optionsReloadToken;

    /// <summary>
    /// Constructor without support for reloading/refreshing the options.
    /// </summary>
    /// <param name="name">The name of the formatter; see <see cref="ITerminalLogEntryFormatter.Name"/>.</param>
    /// <param name="options">The options for this formatter.</param>
    protected AbstractTerminalLogEntryFormatter(string name, TOptions options)
    {
        this.Name = name;
        this.FormatterOptions = options;
    }

    /// <summary>
    /// Constructor with support for reloading/refreshing the options.
    /// </summary>
    /// <param name="name">The name of the formatter; see <see cref="ITerminalLogEntryFormatter.Name"/>.</param>
    /// <param name="options">The options monitor for this formatter.</param>
    protected AbstractTerminalLogEntryFormatter(string name, IOptionsMonitor<TOptions> options)
    {
        this.Name = name;
        this.FormatterOptions = options.CurrentValue;
        this._optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        this._optionsReloadToken?.Dispose();
    }

    private void ReloadLoggerOptions(TOptions options)
    {
        this.FormatterOptions = options;
    }

    /// <inheritdoc />
    public void Write<TState>(LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        // NOTE: In our implementation, "Formatter" is always set. But just in case...
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (logEntry.Exception == null && message == null)
        {
            // Nothing to log.
            return;
        }

        Write(
            textWriter,
            logEntry.LogLevel,
            logEntry.Category,
            logEntry.EventId,
            message,
            logEntry.State,
            logEntry.Exception,
            this.FormatterOptions.IncludeScopes ? scopeProvider : null
        );
    }

    /// <summary>
    /// Writes the log entry to the specified <see cref="TextWriter"/>. Note that no(!) trailing line break should be included.
    /// </summary>
    /// <remarks>
    /// If the formatter wants to write colors to the console, it can do so by embedding <see cref="TermText"/> strings
    /// in the output.
    /// </remarks>
    protected abstract void Write<TState>(
        TextWriter textWriter,
        LogLevel logLevel,
        string category,
        EventId eventId,
        string? message,
        TState? state,
        Exception? exception,
        IExternalScopeProvider? scopeProvider
    );

    /// <summary>
    /// Creates a timestamp string for "now" based on <see cref="Options"/>. Returns <c>null</c> if
    /// no timestamp format has been specified in the options. Note that the timestamp will always
    /// be formatted with <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <seealso cref="ConsoleFormatterOptions.TimestampFormat"/>
    /// <seealso cref="ConsoleFormatterOptions.UseUtcTimestamp"/>
    [MustUseReturnValue]
    protected string? CreateTimestampString()
    {
        var timestampFormat = this.FormatterOptions.TimestampFormat;

        if (string.IsNullOrEmpty(timestampFormat))
        {
            return null;
        }

        var timestamp = this.FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
        return timestamp.ToStringIC(timestampFormat);
    }

    /// <summary>
    /// Replaces line breaks in the specified message with the specified replacement
    /// and returns the result.
    /// </summary>
    [MustUseReturnValue]
    protected static string ReplaceNewLineWith(string message, string replacement)
    {
        return message.Replace(Environment.NewLine, replacement);
    }
}
