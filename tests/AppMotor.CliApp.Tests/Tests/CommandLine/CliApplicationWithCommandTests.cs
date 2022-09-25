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
    public void Test_Command_ViaConstructor()
    {
        // Setup
        var testCommand = new TestCommand();
        var app = new TestApplicationWithCommand(testCommand);

        // Test
        app.RunWithExpectedExitCode(expectedExitCode: 42, "--value", "42");

        // Verify
        app.TerminalOutput.ShouldBeEmpty();
        testCommand.Executed.ShouldBe(true);
    }

    [Fact]
    public void Test_Command_ViaProperty()
    {
        // Setup
        var testCommand = new TestCommand();
        var app = new TestApplicationWithCommand()
        {
            Command = testCommand,
        };

        // Test
        app.RunWithExpectedExitCode(expectedExitCode: 42, "--value", "42");

        // Verify
        app.TerminalOutput.ShouldBeEmpty();
        testCommand.Executed.ShouldBe(true);
    }

    [Fact]
    public void Test_ViaCliApplication_Run()
    {
        // Setup
        var testCommand = new TestCommand();

        // Test
        int exitCode = CliApplication.Run(new[] { "--value", "42" }, testCommand);

        // Verify
        testCommand.Executed.ShouldBe(true);
        exitCode.ShouldBe(42);
    }

    [Fact]
    public async Task Test_ViaCliApplication_RunAsync()
    {
        // Setup
        var testCommand = new TestCommand();

        // Test
        int exitCode = await CliApplication.RunAsync(new[] { "--value", "42" }, testCommand);

        // Verify
        testCommand.Executed.ShouldBe(true);
        exitCode.ShouldBe(42);
    }

    [Fact]
    public void Test_NullCommand_Constructor()
    {
        Should.Throw<ArgumentNullException>(() => new TestApplicationWithCommand(null!));
    }

    [Fact]
    public void Test_NullCommand_Property()
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

        private int Execute()
        {
            this.Executed.ShouldBe(false);
            this.Executed = true;
            this._value.Value.ShouldBe(42);

            return 42;
        }
    }
}
