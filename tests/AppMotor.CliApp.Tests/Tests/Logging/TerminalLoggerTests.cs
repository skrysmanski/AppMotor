// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Logging;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Utils;
using AppMotor.TestCore.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Logging;

/// <summary>
/// Tests for <see cref="TerminalLogger"/> and related classes.
/// </summary>
public sealed class TerminalLoggerTests
{
    [Fact]
    public async Task Test_BasicFunctionality()
    {
        // Setup
        var hostBuilder = new HostBuilder();

        hostBuilder.UseServiceProviderFactory(
            new DefaultServiceProviderFactory(new ServiceProviderOptions()
            {
                // Enable all validations
                ValidateScopes = true,
                ValidateOnBuild = true,
            })
        );

        var testTerminal = new TestTerminal();

        hostBuilder.ConfigureServices(services => services.AddSingleton<ITerminalOutput>(testTerminal));

        hostBuilder.ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                loggingBuilder.AddTerminalLogger();
            }
        );

        IHost host = hostBuilder.Build();

        await host.StartAsync();

        var logger = host.Services.GetRequiredService<ILogger<TerminalLoggerTests>>();

        // Test
        logger.LogError("This is just a test.");

        // Since logging occurs asynchronously, give it some time.
        var startDate = DateTimeUtc.Now;
        while (testTerminal.CurrentOutput.Length == 0)
        {
            await Task.Delay(50);
            (DateTimeUtc.Now - startDate).ShouldBeLessThan(TimeSpan.FromSeconds(2), "No log output found.");
        }

        // Cleanup
        await host.StopAsync();

        // Verify
        Should.CompleteIn(
            () =>
            {
                while (!testTerminal.CurrentOutput.Contains("This is just a test."))
                {
                    Thread.Sleep(50);
                }
            },
            TimeSpan.FromSeconds(2)
        );
        testTerminal.CurrentOutput.ShouldContain("This is just a test.");
    }
}
