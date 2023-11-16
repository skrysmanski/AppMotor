// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

using AppMotor.Core.Logging;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging.Formatters;

internal sealed class JsonTerminalFormatter : AbstractTerminalLogEntryFormatter<JsonConsoleFormatterOptions>
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/JsonConsoleFormatter.cs

    [UsedImplicitly]
    public JsonTerminalFormatter(IOptionsMonitor<JsonConsoleFormatterOptions> options)
        : base(ConsoleFormatterNames.Json, options)
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
        const int DEFAULT_BUFFER_SIZE = 1024;

        using var output = new PooledArrayBufferWriter<byte>(DEFAULT_BUFFER_SIZE);
        using var writer = new Utf8JsonWriter(output, this.FormatterOptions.JsonWriterOptions);

        writer.WriteStartObject();

        var timestamp = CreateTimestampString();
        if (timestamp != null)
        {
            writer.WriteString("Timestamp", timestamp);
        }

        writer.WriteNumber("EventId", eventId.Id);
        writer.WriteString("LogLevel", GetLogLevelString(logLevel));
        writer.WriteString("Category", category);
        writer.WriteString("Message", message);

        if (exception != null)
        {
            string exceptionMessage = exception.ToStringExtended();
            if (!this.FormatterOptions.JsonWriterOptions.Indented)
            {
                exceptionMessage = exceptionMessage.Replace(Environment.NewLine, " ");
            }
            writer.WriteString("Exception", exceptionMessage);
        }

        if (state != null)
        {
            writer.WriteStartObject("State");
            writer.WriteString("Message", state.ToString());

            if (state is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
            {
                foreach (KeyValuePair<string, object> item in stateProperties)
                {
                    WriteItem(writer, item);
                }
            }
            writer.WriteEndObject();
        }

        if (scopeProvider != null)
        {
            WriteScopeInformation(writer, scopeProvider);
        }

        writer.WriteEndObject();
        writer.Flush();

        textWriter.Write(Encoding.UTF8.GetString(output.WrittenSpan));
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }

    private static void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider scopeProvider)
    {
        writer.WriteStartArray("Scopes");

        scopeProvider.ForEachScope(
            static (scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    state.WriteStartObject();
                    state.WriteString("Message", scope.ToString());
                    foreach (var item in scopeItems)
                    {
                        WriteItem(state, item);
                    }

                    state.WriteEndObject();
                }
                else
                {
                    state.WriteStringValue(ToInvariantString(scope));
                }
            },
            state: writer
        );

        writer.WriteEndArray();
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);
}
