// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging.Formatters;

internal sealed class SimpleTerminalFormatter : AbstractTerminalLogEntryFormatter<SimpleConsoleFormatterOptions>
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/SimpleConsoleFormatter.cs

    private const string LOG_LEVEL_PADDING = ": ";
    private static readonly string MESSAGE_PADDING = new(' ', GetLogLevelString(LogLevel.Information).Length + LOG_LEVEL_PADDING.Length);
    private static readonly string NEW_LINE_WITH_MESSAGE_PADDING = Environment.NewLine + MESSAGE_PADDING;

    private readonly ITerminalOutput _terminalOutput;

    internal SimpleTerminalFormatter(SimpleConsoleFormatterOptions options, ITerminalOutput terminalOutput)
        : base(ConsoleFormatterNames.Simple, options)
    {
        this._terminalOutput = terminalOutput;
    }

    [UsedImplicitly]
    public SimpleTerminalFormatter(IOptionsMonitor<SimpleConsoleFormatterOptions> options, ITerminalOutput terminalOutput)
        : base(ConsoleFormatterNames.Simple, options)
    {
        this._terminalOutput = terminalOutput;
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
        ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
        string logLevelString = GetLogLevelString(logLevel);

        var timestamp = CreateTimestampString();
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }

        textWriter.Write(TermText.Bg(logLevelColors.Background).In(logLevelColors.Foreground).Text(logLevelString));

        bool singleLine = this.FormatterOptions.SingleLine;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LOG_LEVEL_PADDING);
        textWriter.Write(category);
        textWriter.Write('[');
        textWriter.Write(eventId.Id);
        textWriter.Write(']');

        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        // scope information
        if (scopeProvider != null)
        {
            WriteScopeInformation(textWriter, scopeProvider, singleLine);
        }
        WriteMessage(textWriter, message, singleLine);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception.ToStringExtended(), singleLine);
        }
    }

    private static void WriteMessage(TextWriter textWriter, string? message, bool singleLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                textWriter.Write(ReplaceNewLineWith(message, " "));
            }
            else
            {
                textWriter.Write(MESSAGE_PADDING);
                textWriter.Write(ReplaceNewLineWith(message, NEW_LINE_WITH_MESSAGE_PADDING));
            }
        }
    }

    [MustUseReturnValue]
    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }

    private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        bool disableColors = this.FormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled
                          || (this.FormatterOptions.ColorBehavior == LoggerColorBehavior.Default && this._terminalOutput.IsOutputRedirected);
        if (disableColors)
        {
            return new ConsoleColors();
        }

        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
            _ => new ConsoleColors(),
        };
    }

    private static void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider, bool singleLine)
    {
        bool paddingNeeded = !singleLine;

        scopeProvider.ForEachScope(
            (scope, _) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    textWriter.Write(MESSAGE_PADDING);
                    textWriter.Write("=> ");
                }
                else
                {
                    textWriter.Write(" => ");
                }

                textWriter.Write(scope);
            },
            (object?)null
        );

        if (!paddingNeeded && !singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }
    }

    private readonly struct ConsoleColors
    {
        public ConsoleColor? Foreground { get; }

        public ConsoleColor? Background { get; }

        public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            this.Foreground = foreground;
            this.Background = background;
        }
    }
}
