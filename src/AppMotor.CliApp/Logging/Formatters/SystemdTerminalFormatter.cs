// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging.Formatters;

// ReSharper disable once IdentifierTypo
internal sealed class SystemdTerminalFormatter : AbstractTerminalLogEntryFormatter<ConsoleFormatterOptions>
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/SystemdConsoleFormatter.cs

    // ReSharper disable once IdentifierTypo
    [UsedImplicitly]
    public SystemdTerminalFormatter(IOptionsMonitor<ConsoleFormatterOptions> options)
        : base(ConsoleFormatterNames.Systemd, options)
    {
    }

    /// <inheritdoc />
    protected override void Write<TState>(
            TextWriter textWriter,
            LogLevel logLevel,
            string category,
            EventId eventId,
            string? message,
            TState? state,
            Exception? exception,
            IExternalScopeProvider? scopeProvider
        )
            where TState : default
    {
        // systemd reads messages from standard out line-by-line in a '<pri>message' format.
        // newline characters are treated as message delimiters, so we must replace them.
        // Messages longer than the journal LineMax setting (default: 48KB) are cropped.
        // Example:
        // <6>ConsoleApp.Program[10] Request received

        // loglevel
        string logLevelString = GetSyslogSeverityString(logLevel);
        textWriter.Write(logLevelString);

        // timestamp
        var timestamp = CreateTimestampString();
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }

        // category and event id
        textWriter.Write(category);
        textWriter.Write('[');
        textWriter.Write(eventId.Id);
        textWriter.Write(']');

        // scope information
        if (scopeProvider != null)
        {
            WriteScopeInformation(textWriter, scopeProvider);
        }

        // message
        if (!string.IsNullOrEmpty(message))
        {
            textWriter.Write(' ');
            // message
            textWriter.Write(ReplaceNewLineWith(message, " "));
        }

        // exception
        // System.InvalidOperationException at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            textWriter.Write(' ');
            textWriter.Write(ReplaceNewLineWith(exception.ToStringExtended(), " "));
        }
    }

    private static string GetSyslogSeverityString(LogLevel logLevel)
    {
        // 'Syslog Message Severities' from https://tools.ietf.org/html/rfc5424.
        return logLevel switch
        {
            LogLevel.Trace => "<7>",
            LogLevel.Debug => "<7>",        // debug-level messages
            LogLevel.Information => "<6>",  // informational messages
            LogLevel.Warning => "<4>",     // warning conditions
            LogLevel.Error => "<3>",       // error conditions
            LogLevel.Critical => "<2>",    // critical conditions
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }

    private static void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider)
    {
        scopeProvider.ForEachScope(
            static (scope, state) =>
            {
                state.Write(" => ");
                state.Write(scope);
            },
            state: textWriter
        );
    }
}
