// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

/// <summary>
/// Tests for <see cref="CliCommand"/>.
/// </summary>
public sealed class CliCommandTests
{
    [Fact]
    public void TestProperties()
    {
        var testVerb = new TestVerb();
        var testApp = new TestApplicationWithVerbs(testVerb);

        testApp.Run("test", "--value", "42");
        testApp.ShouldHaveNoOutput();
        testVerb.TestCommand.Executed.ShouldBe(true);
        testVerb.TestCommand.Executed = false;

        testApp.Run("test1", "--value", "42");
        testApp.ShouldHaveNoOutput();
        testVerb.TestCommand.Executed.ShouldBe(true);
        testVerb.TestCommand.Executed = false;

        testApp.Run("test2", "--value", "42");
        testApp.ShouldHaveNoOutput();
        testVerb.TestCommand.Executed.ShouldBe(true);
        testVerb.TestCommand.Executed = false;
    }

    [Fact]
    public void TestSubCommands()
    {
        var subVerb1 = new TestVerb("sub1");
        var subVerb2 = new TestVerb("sub2");
        var parentVerb = new VerbWithSubVerbs("parent", subVerb1, subVerb2);
        var testApp = new TestApplicationWithVerbs(parentVerb);

        testApp.Run("parent", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(true);
        subVerb1.TestCommand.Executed.ShouldBe(false);
        subVerb2.TestCommand.Executed.ShouldBe(false);
        parentVerb.TestCommand.Executed = false;

        testApp.Run("parent", "sub1", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed.ShouldBe(true);
        subVerb2.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed = false;

        testApp.Run("parent", "sub2", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed.ShouldBe(false);
        subVerb2.TestCommand.Executed.ShouldBe(true);
        subVerb2.TestCommand.Executed = false;
    }

    [Fact]
    public void TestSubGroup()
    {
        var subVerb1 = new TestVerb("sub1");
        var subVerb2 = new TestVerb("sub2");
        var subGroup = new TestVerbGroup("subgroup", subVerb1, subVerb2);
        var parentVerb = new VerbWithSubVerbs("parent", subGroup);
        var testApp = new TestApplicationWithVerbs(parentVerb);

        testApp.Run("parent", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(true);
        subVerb1.TestCommand.Executed.ShouldBe(false);
        subVerb2.TestCommand.Executed.ShouldBe(false);
        parentVerb.TestCommand.Executed = false;

        testApp.Run("parent", "subgroup", "sub1", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed.ShouldBe(true);
        subVerb2.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed = false;

        testApp.Run("parent", "subgroup", "sub2", "--value", "42");
        testApp.ShouldHaveNoOutput();
        parentVerb.TestCommand.Executed.ShouldBe(false);
        subVerb1.TestCommand.Executed.ShouldBe(false);
        subVerb2.TestCommand.Executed.ShouldBe(true);
        subVerb2.TestCommand.Executed = false;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void TestInvalidName(string? name)
    {
        Should.Throw<ArgumentException>(() => new VariableNameVerb(name!));
        Should.Throw<ArgumentException>(() => new VariableNameVerb("primaryname", name!));
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
        Should.Throw<ArgumentException>(() => new VariableNameVerb(name));
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
        Should.Throw<ArgumentException>(() => new VariableNameVerb("mycommand", name));
    }

    [Fact]
    public void Test_Terminal()
    {
        // Setup
        var testVerb = new TestVerb();
        var testApp = new TestApplicationWithVerbs(testVerb);

        // Test
        Should.Throw<InvalidOperationException>(() => testVerb.TestCommand.TerminalAsPublic); // not yet initialized

        testApp.Run("test", "--value", "42");

        Should.Throw<InvalidOperationException>(() => testVerb.TestCommand.TerminalAsPublic); // uninitialized

        // Verify
        testVerb.TestCommand.TerminalDuringExecution.ShouldBeSameAs(testApp.Terminal);
    }

    private sealed class TestCommand : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        public bool Executed { get; set; }

        public ITerminal TerminalAsPublic => this.Terminal;

        public ITerminal? TerminalDuringExecution { get; private set; }

        private readonly CliParam<int> _value = new("--value");

        private void Execute()
        {
            this.Executed.ShouldBe(false);
            this.Executed = true;
            this._value.Value.ShouldBe(42);
            this.TerminalDuringExecution = this.Terminal;
        }
    }

    private sealed class TestVerb : CliVerb
    {
        public TestCommand TestCommand { get; } = new();

        public TestVerb() : base("test", "test1", "test2")
        {
            this.Command = this.TestCommand;
        }

        public TestVerb(string name) : base(name)
        {
            this.Command = this.TestCommand;
        }
    }

    private sealed class NonExecutableCommand : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private static void Execute()
        {
            throw new InvalidOperationException("We should not get here.");
        }
    }

    private sealed class VariableNameVerb : CliVerb
    {
        /// <inheritdoc />
        public VariableNameVerb(string name, params string[] aliases) : base(name, aliases)
        {
            this.Command = new NonExecutableCommand();
        }
    }

    private sealed class VerbWithSubVerbs : CliVerb
    {
        public TestCommand TestCommand { get; } = new();

        public VerbWithSubVerbs(string name, params CliVerb[] subVerbs) : base(name)
        {
            this.Command = this.TestCommand;
            this.SubVerbs = subVerbs;
        }
    }
}
