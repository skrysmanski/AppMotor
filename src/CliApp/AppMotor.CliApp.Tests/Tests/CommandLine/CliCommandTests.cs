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
using System.Collections.Generic;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    public sealed class CliCommandTests
    {
        [Fact]
        public void TestProperties()
        {
            var testCommand = new TestCommand();
            var testApp = new TestApplicationWithCommands(testCommand);

            testApp.Run("test", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            testCommand.Executed.ShouldBe(true);
            testCommand.Executed = false;

            testApp.Run("test1", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            testCommand.Executed.ShouldBe(true);
            testCommand.Executed = false;

            testApp.Run("test2", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            testCommand.Executed.ShouldBe(true);
            testCommand.Executed = false;
        }

        [Fact]
        public void TestSubCommands()
        {
            var subCommand1 = new TestCommand("sub1");
            var subCommand2 = new TestCommand("sub2");
            var parentCommand = new CommandWithSubVerbs("parent", subCommand1, subCommand2);
            var testApp = new TestApplicationWithCommands(parentCommand);

            testApp.Run("parent", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(true);
            subCommand1.Executed.ShouldBe(false);
            subCommand2.Executed.ShouldBe(false);
            parentCommand.Executed = false;

            testApp.Run("parent", "sub1", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(false);
            subCommand1.Executed.ShouldBe(true);
            subCommand2.Executed.ShouldBe(false);
            subCommand1.Executed = false;

            testApp.Run("parent", "sub2", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(false);
            subCommand1.Executed.ShouldBe(false);
            subCommand2.Executed.ShouldBe(true);
            subCommand2.Executed = false;
        }

        [Fact]
        public void TestSubGroup()
        {
            var subCommand1 = new TestCommand("sub1");
            var subCommand2 = new TestCommand("sub2");
            var subGroup = new TestVerbGroup("subgroup", subCommand1, subCommand2);
            var parentCommand = new CommandWithSubVerbs("parent", subGroup);
            var testApp = new TestApplicationWithCommands(parentCommand);

            testApp.Run("parent", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(true);
            subCommand1.Executed.ShouldBe(false);
            subCommand2.Executed.ShouldBe(false);
            parentCommand.Executed = false;

            testApp.Run("parent", "subgroup", "sub1", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(false);
            subCommand1.Executed.ShouldBe(true);
            subCommand2.Executed.ShouldBe(false);
            subCommand1.Executed = false;

            testApp.Run("parent", "subgroup", "sub2", "--value", "42").ShouldBe(0, testApp.TerminalOutput);
            testApp.ShouldHaveNoOutput();
            parentCommand.Executed.ShouldBe(false);
            subCommand1.Executed.ShouldBe(false);
            subCommand2.Executed.ShouldBe(true);
            subCommand2.Executed = false;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void TestInvalidName(string? name)
        {
            Should.Throw<ArgumentException>(() => new VariableNameCommand(name!));
            Should.Throw<ArgumentException>(() => new VariableNameCommand("primaryname", name!));
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
            Should.Throw<ArgumentException>(() => new VariableNameCommand(name));
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
            Should.Throw<ArgumentException>(() => new VariableNameCommand("mycommand", name));
        }

        private sealed class TestCommand : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            public bool Executed { get; set; }

            private readonly CliParam<int> m_value = new("--value");

            public TestCommand() : base("test", "test1", "test2")
            {
            }

            public TestCommand(string name) : base(name)
            {
            }

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.Executed = true;
                this.m_value.Value.ShouldBe(42);
            }
        }

        private sealed class VariableNameCommand : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            /// <inheritdoc />
            public VariableNameCommand(string name, params string[] aliases) : base(name, aliases)
            {
            }

            private static void Execute()
            {
                throw new InvalidOperationException("We should not get here.");
            }
        }

        private sealed class CommandWithSubVerbs : CliCommand
        {
            private readonly CliVerb[] m_subVerbs;

            public bool Executed { get; set; }

            private readonly CliParam<int> m_value = new("--value");

            /// <inheritdoc />
            public CommandWithSubVerbs(string name, params CliVerb[] subVerbs) : base(name)
            {
                this.m_subVerbs = subVerbs;
            }

            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            /// <inheritdoc />
            protected override IEnumerable<CliVerb> GetSubVerbs()
            {
                return this.m_subVerbs;
            }

            private void Execute()
            {
                this.Executed.ShouldBe(false);
                this.Executed = true;
                this.m_value.Value.ShouldBe(42);
            }
        }
    }
}
