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

using System.Collections.Generic;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.TestUtils;
using AppMotor.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    public sealed class DebugParamTests
    {
        public DebugParamTests()
        {
            DebuggerUtils.IsTestRun = true;
        }

        // ReSharper disable MultipleSpaces
        [Theory]
        [InlineData(TestScenarios.BothAliasesAvailable, true, true)]
        [InlineData(TestScenarios.DAliasTaken,          true, true)]
        [InlineData(TestScenarios.DebugAliasTaken,      true, false)]
        [InlineData(TestScenarios.BothAliasesTaken,     true, false)]
        [InlineData(TestScenarios.BothAliasesAvailable, false, true)]
        [InlineData(TestScenarios.DAliasTaken,          false, false)]
        [InlineData(TestScenarios.DebugAliasTaken,      false, true)]
        [InlineData(TestScenarios.BothAliasesTaken,     false, false)]
        // ReSharper restore MultipleSpaces
        public void TestCliApplicationWithCommand_WhenEnabled(TestScenarios scenario, bool longAlias, bool expectedLaunch)
        {
            // Setup
            var testCommand = new TestCommand(scenario);
            var app = new TestApplicationWithCommand(testCommand);

            app.EnableGlobalDebugParam.ShouldBe(true);

            int oldDebuggerLaunchCount = DebuggerUtils.DebuggerLaunchCount;

            // Test
            if (longAlias)
            {
                app.Run("--value", "42", "--debug");
            }
            else
            {
                app.Run("--value", "42", "-d");
            }

            // Verify
            app.TerminalOutput.ShouldBeEmpty();
            testCommand.Executed.ShouldBe(true);
            if (expectedLaunch)
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount + 1);
            }
            else
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount);
            }
        }

        [Fact]
        public void TestCliApplicationWithCommand_WhenDisabled()
        {
            // Setup
            var testCommand = new TestCommand(TestScenarios.BothAliasesAvailable);
            var app = new TestApplicationWithCommand(testCommand);

            // Test
            app.EnableGlobalDebugParam = false;

            app.RunWithExpectedExitCode(expectedExitCode: 1, "--value", "42", "--debug");
            app.RunWithExpectedExitCode(expectedExitCode: 1, "--value", "42", "-d");

            // Verify
            testCommand.Executed.ShouldBe(false);
        }

        // ReSharper disable MultipleSpaces
        [Theory]
        [InlineData(TestScenarios.BothAliasesAvailable, true, true)]
        [InlineData(TestScenarios.DAliasTaken,          true, true)]
        [InlineData(TestScenarios.DebugAliasTaken,      true, false)]
        [InlineData(TestScenarios.BothAliasesTaken,     true, false)]
        [InlineData(TestScenarios.BothAliasesAvailable, false, true)]
        [InlineData(TestScenarios.DAliasTaken,          false, false)]
        [InlineData(TestScenarios.DebugAliasTaken,      false, true)]
        [InlineData(TestScenarios.BothAliasesTaken,     false, false)]
        // ReSharper restore MultipleSpaces
        public void TestCliApplicationWithParams_WhenEnabled(TestScenarios scenario, bool longAlias, bool expectedLaunch)
        {
            // Setup
            var app = new TestApplicationWithParams(scenario);

            app.EnableGlobalDebugParam.ShouldBe(true);

            int oldDebuggerLaunchCount = DebuggerUtils.DebuggerLaunchCount;

            // Test
            if (longAlias)
            {
                app.Run("--value", "42", "--debug");
            }
            else
            {
                app.Run("--value", "42", "-d");
            }

            // Verify
            app.TerminalOutput.ShouldBeEmpty();
            app.Executed.ShouldBe(true);
            if (expectedLaunch)
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount + 1);
            }
            else
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount);
            }
        }

        [Fact]
        public void TestCliApplicationWithParams_WhenDisabled()
        {
            // Setup
            var app = new TestApplicationWithParams(TestScenarios.BothAliasesAvailable);

            // Test
            app.EnableGlobalDebugParam = false;

            app.RunWithExpectedExitCode(expectedExitCode: 1, "--value", "42", "--debug");
            app.RunWithExpectedExitCode(expectedExitCode: 1, "--value", "42", "-d");

            // Verify
            app.Executed.ShouldBe(false);
        }

        // ReSharper disable MultipleSpaces
        [Theory]
        [InlineData(TestScenarios.BothAliasesAvailable, true, true)]
        [InlineData(TestScenarios.DAliasTaken,          true, true)]
        [InlineData(TestScenarios.DebugAliasTaken,      true, false)]
        [InlineData(TestScenarios.BothAliasesTaken,     true, false)]
        [InlineData(TestScenarios.BothAliasesAvailable, false, true)]
        [InlineData(TestScenarios.DAliasTaken,          false, false)]
        [InlineData(TestScenarios.DebugAliasTaken,      false, true)]
        [InlineData(TestScenarios.BothAliasesTaken,     false, false)]
        // ReSharper restore MultipleSpaces
        public void TestCliApplicationWithVerbs_WhenEnabled(TestScenarios scenario, bool longAlias, bool expectedLaunch)
        {
            // Setup
            var command = new TestCommand(scenario);
            var app = new TestApplicationWithVerbs(new CliVerb("doit", command));

            app.EnableGlobalDebugParam.ShouldBe(true);

            int oldDebuggerLaunchCount = DebuggerUtils.DebuggerLaunchCount;

            // Test
            if (longAlias)
            {
                app.Run("doit", "--value", "42", "--debug");
            }
            else
            {
                app.Run("doit", "--value", "42", "-d");
            }

            // Verify
            app.TerminalOutput.ShouldBeEmpty();
            command.Executed.ShouldBe(true);
            if (expectedLaunch)
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount + 1);
            }
            else
            {
                DebuggerUtils.DebuggerLaunchCount.ShouldBe(oldDebuggerLaunchCount);
            }
        }

        [Fact]
        public void TestCliApplicationWithVerbs_WhenDisabled()
        {
            // Setup
            var command = new TestCommand(TestScenarios.BothAliasesAvailable);
            var app = new TestApplicationWithVerbs(new CliVerb("doit", command));

            // Test
            app.EnableGlobalDebugParam = false;

            app.RunWithExpectedExitCode(expectedExitCode: 1, "doit", "--value", "42", "--debug");
            app.RunWithExpectedExitCode(expectedExitCode: 1, "doit", "--value", "42", "-d");

            // Verify
            command.Executed.ShouldBe(false);
        }

        private static List<CliParamBase> CreateTestParams(TestScenarios scenario, out CliParam<int> valueParam)
        {
            var paramList = new List<CliParamBase>();

            valueParam = new CliParam<int>("--value");
            paramList.Add(valueParam);

            switch (scenario)
            {
                case TestScenarios.BothAliasesAvailable:
                    // No other parameters.
                    break;

                case TestScenarios.DAliasTaken:
                    paramList.Add(new CliParam<bool>("-d"));
                    break;

                case TestScenarios.DebugAliasTaken:
                    paramList.Add(new CliParam<bool>("--debug"));
                    break;

                case TestScenarios.BothAliasesTaken:
                    paramList.Add(new CliParam<bool>("--debug", "-d"));
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(scenario), scenario);
            }

            return paramList;
        }

        public enum TestScenarios
        {
            BothAliasesAvailable,
            DAliasTaken,
            DebugAliasTaken,
            BothAliasesTaken,
        }

        private sealed class TestCommand : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            private readonly TestScenarios _scenario;

            private CliParam<int>? _valueParam;

            public bool Executed { get; private set; }

            /// <inheritdoc />
            public TestCommand(TestScenarios scenario)
            {
                this._scenario = scenario;
            }

            /// <inheritdoc />
            protected override IEnumerable<CliParamBase> GetAllParams()
            {
                return CreateTestParams(this._scenario, out this._valueParam);
            }

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.Executed = true;

                this._valueParam.ShouldNotBeNull();
                this._valueParam.Value.ShouldBe(42);
            }
        }

        private sealed class TestApplicationWithParams : TestApplicationWithParamsBase
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            private readonly TestScenarios _scenario;

            private CliParam<int>? _valueParam;

            public bool Executed { get; private set; }

            /// <inheritdoc />
            public TestApplicationWithParams(TestScenarios scenario)
            {
                this._scenario = scenario;
            }

            /// <inheritdoc />
            protected override IEnumerable<CliParamBase> GetAllParams()
            {
                return CreateTestParams(this._scenario, out this._valueParam);
            }

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.Executed = true;

                this._valueParam.ShouldNotBeNull();
                this._valueParam.Value.ShouldBe(42);
            }

        }
    }
}
