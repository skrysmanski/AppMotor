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

using Microsoft.Extensions.Logging;

using Shouldly;

namespace AppMotor.TestCore.Logging
{
    /// <summary>
    /// Provides statistics about the messages logged with the XUnit test logger (see <see cref="XUnitLoggerProviderExtensions.AddXUnitLogger"/>).
    /// </summary>
    public sealed class TestLoggerStatistics
    {
        private readonly object _lock = new();

        /// <summary>
        /// The number of warnings that were logged.
        /// </summary>
        public int WarningCount { get; private set; }

        /// <summary>
        /// The number of errors that were logged.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// The number of critical messages that were logged.
        /// </summary>
        public int CriticalCount { get; private set; }

        internal void OnLogMessage(LogLevel logLevel)
        {
            lock (this._lock)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        this.CriticalCount++;
                        break;

                    case LogLevel.Error:
                        this.ErrorCount++;
                        break;

                    case LogLevel.Warning:
                        this.WarningCount++;
                        break;
                }
            }
        }

        /// <summary>
        /// Ensures that both <see cref="ErrorCount"/> and <seealso cref="CriticalCount"/> are 0.
        /// </summary>
        public void ShouldHaveNoErrors()
        {
            this.ErrorCount.ShouldBe(0);
            this.CriticalCount.ShouldBe(0);
        }
    }
}
