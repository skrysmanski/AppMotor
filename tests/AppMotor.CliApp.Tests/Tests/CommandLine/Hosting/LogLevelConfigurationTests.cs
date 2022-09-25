// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.AppBuilding;
using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;
using AppMotor.TestCore;
using AppMotor.TestCore.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.Tests.CommandLine.Hosting;

public sealed class LogLevelConfigurationTests : TestBase
{
    public LogLevelConfigurationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void Test_Constructor()
    {
        var config = new LogLevelConfiguration(defaultLogLevel: LogLevel.Warning);
        config.DefaultLogLevel.ShouldBe(LogLevel.Warning);
        config.LogCategoryLevels.ShouldNotBeNull();
        config.LogCategoryLevels.ShouldBeEmpty();

        // Test: "None" is allowed
        var config2 = new LogLevelConfiguration(defaultLogLevel: LogLevel.None);
        config2.DefaultLogLevel.ShouldBe(LogLevel.None);
    }

    [Fact]
    public void Test_ApplyToHostBuilder()
    {
        static void LogAndCheck(ILogger logger, TestLoggerStatistics loggerStatistics, LogLevel level, bool shouldLog)
        {
            var oldLogCount = loggerStatistics.GetLogCount(level);

            logger.Log(level, shouldLog ? "This should be logged" : "This should NOT be logged!!!");

            if (shouldLog)
            {
                loggerStatistics.GetLogCount(level).ShouldBe(oldLogCount + 1);
            }
            else
            {
                loggerStatistics.GetLogCount(level).ShouldBe(oldLogCount);
            }
        }

        var logLevelConfig = new LogLevelConfiguration(LogLevel.Warning)
        {
            LogCategoryLevels =
            {
                ["Test1"] = LogLevel.Information,
                ["Test2"] = LogLevel.Critical,
            },
        };

        var mainCommand = new TestCommand(
            logLevelConfig,
            this.TestConsole,
            (loggerFactory, testLoggerStatistics) =>
            {
                var logger1 = loggerFactory.CreateLogger("Test1");
                var logger2 = loggerFactory.CreateLogger("Test2");
                var otherLogger = loggerFactory.CreateLogger("SomeOtherCategory");

                LogAndCheck(otherLogger, testLoggerStatistics, LogLevel.Error, shouldLog: true);
                LogAndCheck(otherLogger, testLoggerStatistics, LogLevel.Warning, shouldLog: true);
                LogAndCheck(otherLogger, testLoggerStatistics, LogLevel.Information, shouldLog: false);

                LogAndCheck(logger1, testLoggerStatistics, LogLevel.Warning, shouldLog: true);
                LogAndCheck(logger1, testLoggerStatistics, LogLevel.Information, shouldLog: true);
                LogAndCheck(logger1, testLoggerStatistics, LogLevel.Debug, shouldLog: false);

                LogAndCheck(logger2, testLoggerStatistics, LogLevel.Critical, shouldLog: true);
                LogAndCheck(logger2, testLoggerStatistics, LogLevel.Error, shouldLog: false);
                LogAndCheck(logger2, testLoggerStatistics, LogLevel.Warning, shouldLog: false);
            }
        );

        var app = new TestApplicationWithCommand(mainCommand);
        app.Run();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Default")]
    [InlineData("default")]
    public void Test_ApplyToHostBuilder_BadLogCategories(string logCategory)
    {
        var logLevelConfig = new LogLevelConfiguration(LogLevel.Warning)
        {
            LogCategoryLevels =
            {
                [logCategory] = LogLevel.Information,
            },
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection();

        var hostBuildContext = new HostBuilderContext(new Dictionary<object, object>())
        {
            Configuration = configBuilder.Build(),
        };

        var ex = Should.Throw<InvalidOperationException>(() => logLevelConfig.ApplyToHostBuilder(hostBuildContext));
        ex.Message.ShouldContain("log category");
    }

    private sealed class TestCommand : CliCommand
    {
        private readonly Action<ILoggerFactory, TestLoggerStatistics> _mainAction;

        private readonly DefaultHostBuilderFactory _hostBuilderFactory;

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Run);

        /// <inheritdoc />
        public TestCommand(LogLevelConfiguration logLevelConfiguration, ITestOutputHelper testOutputHelper, Action<ILoggerFactory, TestLoggerStatistics> mainAction)
        {
            this._mainAction = mainAction;

            this._hostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LogLevelConfiguration = logLevelConfiguration,
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

        private void Run()
        {
            var loggerFactory = this.Services.GetRequiredService<ILoggerFactory>();
            var testLoggerStatistics = this.Services.GetRequiredService<TestLoggerStatistics>();
            this._mainAction(loggerFactory, testLoggerStatistics);
        }
    }
}
