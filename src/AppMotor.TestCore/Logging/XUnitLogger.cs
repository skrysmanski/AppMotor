// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Exceptions;
using AppMotor.Core.Extensions;
using AppMotor.Core.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore.Logging;

/// <summary>
/// Implements <see cref="ILogger"/> and writes output to XUnit's <see cref="ITestOutputHelper"/>.
/// </summary>
internal sealed class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly string _categoryName;

    private readonly TestLoggerStatistics _loggerStatistics;

    public XUnitLogger(ITestOutputHelper testOutputHelper, string categoryName, TestLoggerStatistics loggerStatistics)
    {
        this._testOutputHelper = testOutputHelper;
        this._categoryName = categoryName;
        this._loggerStatistics = loggerStatistics;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        const string INDENTATION = "      ";

        var now = DateTime.Now;
        var message = formatter(state, exception);
        this._testOutputHelper.WriteLine($"[{now:HH:mm:ss.fff}] [{GetLogLevelString(logLevel)}] {this._categoryName}{Environment.NewLine}{INDENTATION}{message}");

        if (exception is not null)
        {
            this._testOutputHelper.WriteLine(
                Environment.NewLine + INDENTATION
              + string.Join(Environment.NewLine + INDENTATION, exception.ToStringExtended().SplitLines())
            );
        }

        this._loggerStatistics.OnLogMessage(logLevel);
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
            _ => throw new UnexpectedSwitchValueException(nameof(logLevel), logLevel),
        };
    }

    /// <summary>
    /// An empty scope without any logic
    /// </summary>
    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
