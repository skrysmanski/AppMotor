#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.TestUtils
{
    internal sealed class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly string _categoryName;

        public XUnitLogger(ITestOutputHelper testOutputHelper, string categoryName)
        {
            this._testOutputHelper = testOutputHelper;
            this._categoryName = categoryName;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var now = DateTime.Now;
            var message = formatter(state, exception);
            this._testOutputHelper.WriteLine($"[{now:HH:mm:ss.fff}] [{GetLogLevelString(logLevel)}] {this._categoryName}{Environment.NewLine}      {message}");
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
}
