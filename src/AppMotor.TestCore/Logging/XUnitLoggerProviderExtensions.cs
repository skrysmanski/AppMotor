// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore.Logging;

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
