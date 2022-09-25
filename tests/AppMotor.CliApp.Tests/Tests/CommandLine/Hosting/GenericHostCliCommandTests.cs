// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
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
    public async Task Test_DefaultGenericHost_NotStoppingOnItsOwn()
    {
        // The number of seconds to wait for the stopping event not to happen to "deduce"
        // that it would not fire on its own. (This test is not 100% reliable but it's better
        // than nothing.)
        const int MAX_WAIT_SECONDS_FOR_CONFIRMATION = 2;

        // Setup
        var command = new GenericHostTestCommand(this.TestConsole);
        var testApp = new TestApplicationWithCommand(command);

        using var startedEvent = new ManualResetEventSlim();
        using var stoppingEvent = new ManualResetEventSlim();
        using var stoppedEvent = new ManualResetEventSlim();

        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Started.RegisterEventHandler(() => startedEvent.Set()).ShouldNotBeNull();
        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Stopping.RegisterEventHandler(() => stoppingEvent.Set()).ShouldNotBeNull();
        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Stopped.RegisterEventHandler(() => stoppedEvent.Set()).ShouldNotBeNull();

        var appTask = testApp.RunAsync();

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
            // Cleanup
            command.Stop();

            // Verify
            stoppingEvent.IsSet.ShouldBe(true);
            command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(true);

            await TestTimeout.TimeoutAfter(appTask, TimeSpan.FromSeconds(10));

            // Verify
            stoppedEvent.IsSet.ShouldBe(true);
        }

        loggerStatistics.ShouldHaveNoErrors();
    }

    private class GenericHostTestCommand : GenericHostCliCommand
    {
        /// <inheritdoc />
        protected sealed override IHostBuilderFactory HostBuilderFactory { get; }

        public IServiceProvider ServicesAsPublic => this.Services;

        /// <inheritdoc />
        public GenericHostTestCommand(ITestOutputHelper testOutputHelper)
        {
            this.HostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
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
    public void Test_ExplicitExecutor()
    {
        const int WAIT_SECONDS_INSIDE_COMMAND = 1;

        // Setup
        var command = new GenericHostCommandWithExplicitExecutor(waitInsideExecute: TimeSpan.FromSeconds(WAIT_SECONDS_INSIDE_COMMAND), this.TestConsole);
        var testApp = new TestApplicationWithCommand(command);

        using var startedEvent = new ManualResetEventSlim();
        using var stoppingEvent = new ManualResetEventSlim();
        using var stoppedEvent = new ManualResetEventSlim();

        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Started.RegisterEventHandler(() => startedEvent.Set()).ShouldNotBeNull();
        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Stopping.RegisterEventHandler(() =>
            {
                command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(true);
                stoppingEvent.Set();
            }
        ).ShouldNotBeNull();
        // ReSharper disable once AccessToDisposedClosure
        command.LifetimeEvents.Stopped.RegisterEventHandler(() => stoppedEvent.Set()).ShouldNotBeNull();

        command.LifetimeEvents.CancellationToken.IsCancellationRequested.ShouldBe(false);

        var appTask = testApp.RunAsync();

        startedEvent.Wait(TimeSpan.FromSeconds(10)).ShouldBe(true);

        // Test
        appTask.Wait(TimeSpan.FromSeconds(WAIT_SECONDS_INSIDE_COMMAND * 3)).ShouldBe(true);

        // Verify
        stoppingEvent.IsSet.ShouldBe(true);
        stoppedEvent.IsSet.ShouldBe(true);
    }

    private sealed class GenericHostCommandWithExplicitExecutor : GenericHostTestCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor ExplicitExecutor => new(Execute);

        private readonly TimeSpan _waitInsideExecute;

        public GenericHostCommandWithExplicitExecutor(TimeSpan waitInsideExecute, ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            this._waitInsideExecute = waitInsideExecute;
        }

        private void Execute()
        {
            Thread.Sleep(this._waitInsideExecute);
        }
    }

    [Fact]
    public void TestCustomHostBuilderFactory()
    {
        // Setup
        var command = new CommandWithCustomHostBuilderFactory();
        var testApp = new TestApplicationWithCommand(command);

        command.LifetimeEvents.Started.RegisterEventHandler(() => command.Stop()).ShouldNotBeNull();

        // Test
        testApp.Run();

        // Verify
        command.CustomHostBuilderFactoryCalled.ShouldBe(true);
    }

    private sealed class CommandWithCustomHostBuilderFactory : GenericHostCliCommand
    {
        public bool CustomHostBuilderFactoryCalled { get; private set; }

        /// <inheritdoc />
        protected override IHostBuilderFactory HostBuilderFactory { get; }

        /// <inheritdoc />
        public CommandWithCustomHostBuilderFactory()
        {
            this.HostBuilderFactory = new MethodHostBuilderFactory(
                () =>
                {
                    this.CustomHostBuilderFactoryCalled = true;
                    return new HostBuilder();
                }
            );
        }
    }
}
