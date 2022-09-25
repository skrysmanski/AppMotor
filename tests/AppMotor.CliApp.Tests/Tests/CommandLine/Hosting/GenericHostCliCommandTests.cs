// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.AppBuilding;
using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.CliApp.TestUtils;
using AppMotor.TestCore;
using AppMotor.TestCore.Logging;
using AppMotor.TestCore.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.Tests.CommandLine.Hosting;

public sealed class GenericHostCliCommandTests : TestBase
{
    public GenericHostCliCommandTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Test_Stop_ViaStopMethod()
    {
        // Setup
        var command = new GenericHostTestCommand(this.TestConsole);

        // Test
        await Test_Stop(command, command.Stop);
    }

    [Fact]
    public async Task Test_Stop_ViaCancellationToken()
    {
        // Setup
        using var cts = new CancellationTokenSource();

        var command = new GenericHostTestCommand(this.TestConsole);

        // Test
        await Test_Stop(command, cts.Cancel, cts.Token);
    }

    private static async Task Test_Stop(GenericHostTestCommand command, Action stopAction, CancellationToken cancellationToken = default)
    {
        // The number of seconds to wait for the stopping event not to happen to "deduce"
        // that it would not fire on its own. (This test is not 100% reliable but it's better
        // than nothing.)
        const double MAX_WAIT_SECONDS_FOR_CONFIRMATION = 1.2;

        // Setup
        var testApp = new TestApplicationWithCommand(command);

        using var startedEvent = new ManualResetEventSlim();
        using var stoppingEvent = new ManualResetEventSlim();
        using var stoppedEvent = new ManualResetEventSlim();

        // ReSharper disable AccessToDisposedClosure
        command.LifetimeEvents.Started.RegisterEventHandler(() => startedEvent.Set()).ShouldNotBeNull();
        command.LifetimeEvents.Stopping.RegisterEventHandler(() => stoppingEvent.Set()).ShouldNotBeNull();
        command.LifetimeEvents.Stopped.RegisterEventHandler(() => stoppedEvent.Set()).ShouldNotBeNull();
        // ReSharper restore AccessToDisposedClosure

        var appTask = testApp.RunAsync(cancellationToken);

        TestLoggerStatistics loggerStatistics;
        try
        {
            command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(false);

            startedEvent.Wait(TimeSpan.FromSeconds(10)).ShouldBe(true);

            loggerStatistics = command.ServicesAsPublic.GetRequiredService<TestLoggerStatistics>();

            command.LifetimeEvents.Stopping.RegisterEventHandler(() => command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(true)).ShouldNotBeNull();

            // Test
            stoppingEvent.Wait(TimeSpan.FromSeconds(MAX_WAIT_SECONDS_FOR_CONFIRMATION)).ShouldBe(false); // Stopping event was not triggered within 2 seconds
        }
        finally
        {
            // Test stop
            stopAction();

            // Verify
            stoppingEvent.Wait(TimeSpan.FromSeconds(10)).ShouldBe(true);

            await TestTimeout.TimeoutAfter(appTask, TimeSpan.FromSeconds(10));

            // Verify
            stoppedEvent.IsSet.ShouldBe(true);
        }

        loggerStatistics.ShouldHaveNoErrors();
    }

    private class GenericHostTestCommand : GenericHostCliCommand
    {
        public IServiceProvider ServicesAsPublic => this.Services;

        private readonly DefaultHostBuilderFactory _hostBuilderFactory;

        public GenericHostTestCommand(ITestOutputHelper testOutputHelper)
        {
            this._hostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
        }

        /// <inheritdoc />
        protected override IHostBuilder CreateHostBuilder()
        {
            return this._hostBuilderFactory.CreateHostBuilder();
        }
    }

    [Fact]
    public async Task Test_ServiceProvider()
    {
        // Setup
        var command = new GenericHostCommandWithServiceProvider(this.TestConsole);
        var testApp = new TestApplicationWithCommand(command);

        using var startedEvent = new ManualResetEventSlim();

        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Started.RegisterEventHandler(() => startedEvent.Set()).ShouldNotBeNull();

        var appTask = testApp.RunAsync();

        command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(false);

        startedEvent.Wait(TimeSpan.FromSeconds(10)).ShouldBe(true);

        var loggerStatistics = command.ServicesAsPublic.GetRequiredService<TestLoggerStatistics>();

        command.LifetimeEvents.Stopping.RegisterEventHandler(() => command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(true)).ShouldNotBeNull();

        // Test
        var testService = command.ServicesAsPublic.GetRequiredService<GenericHostCommandWithServiceProvider.ITestService>();
        testService.DoSomething(42).ShouldBe(42);

        var logger = command.ServicesAsPublic.GetRequiredService<ILogger<GenericHostCommandWithServiceProvider>>();
        logger.LogInformation("abc");

        command.ServicesAsPublic.GetRequiredService<IGenericHostCliCommandLifetimeEvents>().ShouldBeSameAs(command.LifetimeEvents);

        // Shutdown
        command.Stop();
        await TestTimeout.TimeoutAfter(appTask, TimeSpan.FromSeconds(10));

        loggerStatistics.ShouldHaveNoErrors();
    }

    private sealed class GenericHostCommandWithServiceProvider : GenericHostTestCommand
    {
        public GenericHostCommandWithServiceProvider(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            base.ConfigureServices(context, services);

            services.AddSingleton<ITestService, TestService>();
        }

        public interface ITestService
        {
            int DoSomething(int value);
        }

        private sealed class TestService : ITestService
        {
            /// <inheritdoc />
            public int DoSomething(int value)
            {
                return value;
            }
        }
    }

    [Fact]
    public void Test_CustomHostBuilder()
    {
        // Setup
        var command = new CommandWithCustomHostBuilder();
        var testApp = new TestApplicationWithCommand(command);

        command.LifetimeEvents.Started.RegisterEventHandler(() => command.Stop()).ShouldNotBeNull();

        // Test
        testApp.Run();

        // Verify
        command.CreateHostBuilderCalled.ShouldBe(true);
    }

    private sealed class CommandWithCustomHostBuilder : GenericHostCliCommand
    {
        public bool CreateHostBuilderCalled { get; private set; }

        /// <inheritdoc />
        protected override IHostBuilder CreateHostBuilder()
        {
            this.CreateHostBuilderCalled = true;
            return new HostBuilder();
        }
    }
}
