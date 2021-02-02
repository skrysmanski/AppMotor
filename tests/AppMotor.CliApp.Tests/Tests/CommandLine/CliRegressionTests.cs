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

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    /// <summary>
    /// Tests various scenarios that did not work as expected.
    /// </summary>
    public sealed class CliRegressionTests
    {
        [Fact]
        public void TestArrayParamWithPositionalParams()
        {
            // Setup
            var command = new Command1("abc", "def", "val=1", "eol=lf");
            var app = new TestApplicationWithCommand(command);

            // Test: named parameters first
            command.Executed = false;
            app.Run("-c", "val=1", "-c", "eol=lf", "abc", "def");
            app.ShouldHaveNoOutput();
            command.Executed.ShouldBe(true);

            // Test: named parameters last
            command.Executed = false;
            app.Run("abc", "def", "--config", "val=1", "--config", "eol=lf");
            app.ShouldHaveNoOutput();
            command.Executed.ShouldBe(true);
        }

        private sealed class Command1 : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            private CliParam<string> SourceParam { get; } = new(name: "source", positionIndex: 0);

            private CliParam<string?> TargetParam { get; } = new(name: "target", positionIndex: 1)
            {
                DefaultValue = null,
            };

            private CliParam<string[]> ConfigParam { get; } = new("--config", "-c")
            {
                DefaultValue = Array.Empty<string>(),
            };

            private readonly string _expectedSource;

            private readonly string _expectedTarget;

            private readonly string[] _expectedConfigs;

            public bool Executed { get; set; }

            /// <inheritdoc />
            public Command1(string expectedSource, string expectedTarget, params string[] expectedConfigs)
            {
                this._expectedSource = expectedSource;
                this._expectedTarget = expectedTarget;
                this._expectedConfigs = expectedConfigs;
            }

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.SourceParam.Value.ShouldBe(this._expectedSource);
                this.TargetParam.Value.ShouldBe(this._expectedTarget);
                this.ConfigParam.Value.ShouldBe(this._expectedConfigs);
                this.Executed = true;
            }
        }
    }
}
