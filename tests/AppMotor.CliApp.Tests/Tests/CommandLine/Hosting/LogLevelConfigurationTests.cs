﻿#region License
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

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;
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

    private sealed class TestCommand : GenericHostCliCommand
    {
        private readonly Action<ILoggerFactory, TestLoggerStatistics> _mainAction;

        /// <inheritdoc />
        protected override IHostBuilderFactory HostBuilderFactory { get; }

        /// <inheritdoc />
        protected override CliCommandExecutor ExplicitExecutor => new(Run);

        /// <inheritdoc />
        public TestCommand(LogLevelConfiguration logLevelConfiguration, ITestOutputHelper testOutputHelper, Action<ILoggerFactory, TestLoggerStatistics> mainAction)
        {
            this._mainAction = mainAction;

            this.HostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LogLevelConfiguration = logLevelConfiguration,
                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
        }

        private void Run()
        {
            var loggerFactory = this.Services.GetRequiredService<ILoggerFactory>();
            var testLoggerStatistics = this.Services.GetRequiredService<TestLoggerStatistics>();
            this._mainAction(loggerFactory, testLoggerStatistics);
        }
    }
}