// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Concurrent;

using AppMotor.CliApp.Logging.Formatters;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging;

/// <summary>
/// Creates <see cref="TerminalLogger"/> instances. Register this provider via <see cref="TerminalLoggerExtensions.AddTerminalLogger"/>.
/// </summary>
[ProviderAlias("Console")] // <-- defines the name for the configuration settings; we use "Console" here so that existing settings don't need to be changed
internal sealed class TerminalLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/ConsoleLoggerProvider.cs

    /// <summary>
    /// The loggers created by this provider. Items are added via <see cref="CreateLogger"/>. Key is
    /// the logger category.
    /// </summary>
    private readonly ConcurrentDictionary<string, TerminalLogger> _loggers;

    /// <summary>
    /// The available <see cref="ITerminalLogEntryFormatter"/>. Only one of these is used at the same time
    /// (see <see cref="ReloadLoggerOptions"/>) and it's based on <see cref="ConsoleLoggerOptions.FormatterName"/>.
    /// </summary>
    private readonly ConcurrentDictionary<string, ITerminalLogEntryFormatter> _logEntryFormatters;

    private readonly TerminalLogOutputProcessor _outputProcessor;

    private readonly IOptionsMonitor<ConsoleLoggerOptions> _options;

    private readonly IDisposable _optionsReloadToken;

    private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Constructor.
    /// </summary>
    public TerminalLoggerProvider(
            ITerminalOutput terminal,
            IOptionsMonitor<ConsoleLoggerOptions> options,
            IEnumerable<ITerminalLogEntryFormatter> formatters
        )
    {
        this._options = options;
        this._loggers = new ConcurrentDictionary<string, TerminalLogger>();
        this._logEntryFormatters = CreateFormattersDictionary(formatters, terminal);

        ReloadLoggerOptions(options.CurrentValue);
        this._optionsReloadToken = this._options.OnChange(ReloadLoggerOptions)!;

        this._outputProcessor = new TerminalLogOutputProcessor(terminal);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._optionsReloadToken.Dispose();
        this._outputProcessor.Dispose();
    }

    /// <summary>
    /// Converts the specified list of log entry formatters into a dictionary.
    /// </summary>
    [MustUseReturnValue]
    private static ConcurrentDictionary<string, ITerminalLogEntryFormatter> CreateFormattersDictionary(
            IEnumerable<ITerminalLogEntryFormatter> formatters,
            ITerminalOutput terminal
        )
    {
        var formattersDict = new ConcurrentDictionary<string, ITerminalLogEntryFormatter>(StringComparer.OrdinalIgnoreCase);

        foreach (var formatter in formatters)
        {
            formattersDict.TryAdd(formatter.Name, formatter);
        }

        // NOTE: This is our fallback formatter. We need to make sure it's always present.
        if (!formattersDict.ContainsKey(ConsoleFormatterNames.Simple))
        {
            formattersDict.TryAdd(ConsoleFormatterNames.Simple, new SimpleTerminalFormatter(new SimpleConsoleFormatterOptions(), terminal));
        }

        return formattersDict;
    }

    /// <summary>
    /// Returns the <see cref="ITerminalLogEntryFormatter"/> to be used to format each logged log entry.
    /// </summary>
    [MustUseReturnValue]
    private ITerminalLogEntryFormatter GetLogEntryFormatter(string? formatterName)
    {
        if (formatterName is null || !this._logEntryFormatters.TryGetValue(formatterName, out var logFormatter))
        {
            logFormatter = this._logEntryFormatters[ConsoleFormatterNames.Simple];
        }

        return logFormatter;
    }

    private void ReloadLoggerOptions(ConsoleLoggerOptions options)
    {
        var logEntryFormatter = GetLogEntryFormatter(options.FormatterName);

        foreach (var logger in this._loggers.Values)
        {
            logger.UpdateOptions(logEntryFormatter, options);
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        Validate.ArgumentWithName(nameof(categoryName)).IsNotNull(categoryName);

        var logFormatter = GetLogEntryFormatter(this._options.CurrentValue.FormatterName);

        // NOTE: We use "TryGetValue()" before "GetOrAdd()" below so that we don't need to allocate
        //   the memory for the factory delegate if it's not really needed.
        if (this._loggers.TryGetValue(categoryName, out var logger))
        {
            return logger;
        }

        return this._loggers.GetOrAdd(
            categoryName,
            _ => new TerminalLogger(categoryName, this._outputProcessor, logFormatter, this._options.CurrentValue, this._scopeProvider)
        );
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        Validate.ArgumentWithName(nameof(scopeProvider)).IsNotNull(scopeProvider);

        this._scopeProvider = scopeProvider;

        // Update existing loggers with the new scope provider.
        foreach (var logger in this._loggers.Values)
        {
            logger.UpdateScopeProvider(this._scopeProvider);
        }
    }
}
