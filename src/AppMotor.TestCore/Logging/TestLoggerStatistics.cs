// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Shouldly;

namespace AppMotor.TestCore.Logging;

/// <summary>
/// Provides statistics about the messages logged with the XUnit test logger (see <see cref="XUnitLoggerProviderExtensions.AddXUnitLogger"/>).
/// </summary>
public sealed class TestLoggerStatistics
{
    private readonly Dictionary<LogLevel, int> _logEntryCounts = new();

    /// <summary>
    /// Returns the number of log entries for the given log level.
    /// </summary>
    [Pure]
    public int GetLogCount(LogLevel logLevel)
    {
        lock (this._logEntryCounts)
        {
            if (this._logEntryCounts.TryGetValue(logLevel, out var count))
            {
                return count;
            }

            return 0;
        }
    }

    internal void OnLogMessage(LogLevel logLevel)
    {
        lock (this._logEntryCounts)
        {
            this._logEntryCounts[logLevel] = GetLogCount(logLevel) + 1;
        }
    }

    /// <summary>
    /// Ensures that no entries with log level <see cref="LogLevel.Error"/> or <see cref="LogLevel.Critical"/> have
    /// been logged.
    /// </summary>
    public void ShouldHaveNoErrors()
    {
        GetLogCount(LogLevel.Error).ShouldBe(0);
        GetLogCount(LogLevel.Critical).ShouldBe(0);
    }

    /// <summary>
    /// Resets this instance (i.e. the count for all log levels is reset to 0).
    /// </summary>
    public void Reset()
    {
        lock (this._logEntryCounts)
        {
            this._logEntryCounts.Clear();
        }
    }
}