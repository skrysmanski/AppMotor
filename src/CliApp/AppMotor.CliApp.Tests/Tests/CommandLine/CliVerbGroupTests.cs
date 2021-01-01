#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
    public sealed class CliVerbGroupTests
    {
        [Fact]
        public void TestCommandGroup()
        {
            var testCommand1 = new TestCommand();
            var testCommand2 = new TestCommand();
            var testGroup = new TestVerbGroup(
                "parent",
                new CliVerb("sub1")
                {
                    Command = testCommand1,
                },
                new CliVerb("sub2")
                {
                    Command = testCommand2,
                }
            );

            var testApp = new TestApplicationWithCommands(testGroup);

            testApp.Run("parent", "sub1", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            testCommand1.Executed.ShouldBe(true);
            testCommand1.Executed = false;

            testApp.Run("parent", "sub2", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            testCommand2.Executed.ShouldBe(true);
            testCommand2.Executed = false;

            // The verb group itself must not be executable.
            testApp.Run("parent").ShouldBe(1, testApp.TerminalOutput);
        }

        [Theory]
        [InlineData("--help")]
        [InlineData("--Help")]
        [InlineData("-h")]
        [InlineData("-?")]
        [InlineData("/h")]
        [InlineData("/?")]
        [InlineData("help")]
        [InlineData("Help")]
        public void TestHelpNames_PrimaryName(string name)
        {
            Should.Throw<ArgumentException>(() => new VariableNameVerbGroup(name));
        }

        [Theory]
        [InlineData("--help")]
        [InlineData("--Help")]
        [InlineData("-h")]
        [InlineData("-?")]
        [InlineData("/h")]
        [InlineData("/?")]
        [InlineData("help")]
        [InlineData("Help")]
        public void TestHelpNames_Alias(string name)
        {
            Should.Throw<ArgumentException>(() => new VariableNameVerbGroup("mygroup", name));
        }

        private sealed class TestCommand : CliCommand
        {
            public bool Executed { get; set; }

            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            private readonly CliParam<int> _value = new("--value");

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.Executed = true;
                this._value.Value.ShouldBe(42);
            }
        }

        private sealed class VariableNameVerbGroup : CliVerb
        {
            public VariableNameVerbGroup(string name, params string[] aliases) : base(name, aliases)
            {
            }
        }
    }
}
