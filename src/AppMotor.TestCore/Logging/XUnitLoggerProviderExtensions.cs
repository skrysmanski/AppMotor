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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore.Logging
{
    /// <summary>
    /// XUnit logging extension methods for .NET's host builder.
    /// </summary>
    public static class XUnitLoggerProviderExtensions
    {
        /// <summary>
        /// Adds a logger that writes to <see cref="ITestOutputHelper"/>. Also adds the <paramref name="testOutputHelper"/>
        /// instance and <see cref="TestLoggerStatistics"/> to the service collection.
        /// </summary>
        public static void AddXUnitLogger(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper)
        {
            builder.Services.AddSingleton(testOutputHelper);
            builder.Services.AddSingleton<TestLoggerStatistics>();
            builder.Services.AddSingleton<ILoggerProvider, XUnitLoggerProvider>();
        }
    }
}
