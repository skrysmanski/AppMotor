// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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