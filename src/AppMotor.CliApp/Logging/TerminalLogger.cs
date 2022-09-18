// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Logging.Formatters;
using AppMotor.CliApp.Terminals;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AppMotor.CliApp.Logging;

/// <summary>
/// <see cref="ILogger"/> implementation that writes to a <see cref="ITerminalOutput"/> instance
/// (via <see cref="TerminalLogOutputProcessor"/>).
/// </summary>
internal sealed class TerminalLogger : ILogger
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/ConsoleLogger.cs

    [ThreadStatic]
    private static StringWriter? s_threadStringWriter;

    /// <summary>
    /// The name of the category for <see cref="LogEntry{TState}.Category"/>.
    /// </summary>
    private readonly string _categoryName;

    private readonly TerminalLogOutputProcessor _outputProcessor;

    private ITerminalLogEntryFormatter _logEntryFormatter;

    private ConsoleLoggerOptions _options;

    private IExternalScopeProvider _scopeProvider;

    public TerminalLogger(
            string categoryName,
            TerminalLogOutputProcessor outputProcessor,
            ITerminalLogEntryFormatter logEntryFormatter,
            ConsoleLoggerOptions options,
            IExternalScopeProvider scopeProvider
        )
    {
        this._categoryName = categoryName;
        this._outputProcessor = outputProcessor;
        this._logEntryFormatter = logEntryFormatter;
        this._options = options;
        this._scopeProvider = scopeProvider;
    }

    internal void UpdateScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this._scopeProvider = scopeProvider;
    }

    internal void UpdateOptions(ITerminalLogEntryFormatter logEntryFormatter, ConsoleLoggerOptions options)
    {
        this._logEntryFormatter = logEntryFormatter;
        this._options = options;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        s_threadStringWriter ??= new StringWriter();

        var logEntry = new LogEntry<TState>(logLevel, this._categoryName, eventId, state, exception, formatter);

        this._logEntryFormatter.Write(logEntry, this._scopeProvider, s_threadStringWriter);

        var sb = s_threadStringWriter.GetStringBuilder();

        if (sb.Length == 0)
        {
            return;
        }

        string stringToLog = sb.ToString();
        sb.Clear();

        // Shrink string builder to keep its size to 1kb.
        if (sb.Capacity > 1024)
        {
            sb.Capacity = 1024;
        }

        this._outputProcessor.EnqueueMessage(stringToLog, logAsError: logLevel >= this._options.LogToStandardErrorThreshold);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
        return this._scopeProvider.Push(state);
    }
}
