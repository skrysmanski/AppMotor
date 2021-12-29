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

namespace AppMotor.CliApp.Tests.CommandLine;

public sealed class CliApplicationWithCommandTests
{
    [Fact]
    public void TestCommand_ViaConstructor()
    {
        // Setup
        var testCommand = new TestCommand();
        var app = new TestApplicationWithCommand(testCommand);

        // Test
        app.Run("--value", "42");

        // Verify
        app.TerminalOutput.ShouldBeEmpty();
        testCommand.Executed.ShouldBe(true);
    }

    [Fact]
    public void TestCommand_ViaProperty()
    {
        // Setup
        var testCommand = new TestCommand();
        var app = new TestApplicationWithCommand()
        {
            Command = testCommand,
        };

        // Test
        app.Run("--value", "42");

        // Verify
        app.TerminalOutput.ShouldBeEmpty();
        testCommand.Executed.ShouldBe(true);
    }

    [Fact]
    public void TestNullCommand_Constructor()
    {
        Should.Throw<ArgumentNullException>(() => new TestApplicationWithCommand(null!));
    }

    [Fact]
    public void TestNullCommand_Property()
    {
        // Setup
        var app = new TestApplicationWithCommand();

        // Test
        var caughtException = app.RunWithExpectedException("abc");

        // Verify
        caughtException.ShouldNotBeNull();
        caughtException.ShouldBeOfType<InvalidOperationException>();
        caughtException.Message.ShouldBe($"The property '{nameof(app.Command)}' has never been set.");
    }

    private sealed class TestCommand : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        public bool Executed { get; private set; }

        private readonly CliParam<int> _value = new("--value");

        private void Execute()
        {
            this.Executed.ShouldBe(false);
            this.Executed = true;
            this._value.Value.ShouldBe(42);
        }
    }

}