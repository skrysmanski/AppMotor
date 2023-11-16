// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.CliApp.TestUtils;
using AppMotor.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

public sealed class CliApplicationWithVerbsTests
{
    [Fact]
    public void Test_ViaCliApplication_Run()
    {
        // Test 1
        int exitCode1 = CliApplication.Run(
            new[] { "do1", "--value1", "42" },
            new CliVerb("do1", new Test1Command()),
            new CliVerb("do2", new Test2Command())
        );
        exitCode1.ShouldBe(42);

        // Test 1
        int exitCode2 = CliApplication.Run(
            new[] { "do2", "--value2", "43" },
            new CliVerb("do1", new Test1Command()),
            new CliVerb("do2", new Test2Command())
        );
        exitCode2.ShouldBe(43);
    }

    [Fact]
    public async Task Test_ViaCliApplication_RunAsync()
    {
        // Test 1
        int exitCode1 = await CliApplication.RunAsync(
            new[] { "do1", "--value1", "42" },
            new CliVerb("do1", new Test1Command()),
            new CliVerb("do2", new Test2Command())
        );
        exitCode1.ShouldBe(42);

        // Test 1
        int exitCode2 = await CliApplication.RunAsync(
            new[] { "do2", "--value2", "43" },
            new CliVerb("do1", new Test1Command()),
            new CliVerb("do2", new Test2Command())
        );
        exitCode2.ShouldBe(43);
    }

    [Fact]
    public void TestExceptionHandling_Regular()
    {
        var app = new ExceptionTestApplication(throwErrorMessageException: false);

        var caughtException = app.RunWithExpectedException("error");

        caughtException.ShouldNotBeNull();
        caughtException.ShouldBeOfType<InvalidOperationException>();
        caughtException.Message.ShouldBe("This is a test");

        app.TerminalOutput.ShouldContain("This is a test");
        app.TerminalOutput.ShouldContain($"[{typeof(InvalidOperationException).FullName}] "); // <-- Indicates the full exception report has been printed
    }

    [Fact]
    public void TestExceptionHandling_ErrorMessageException()
    {
        var app = new ExceptionTestApplication(throwErrorMessageException: true);

        // "1" is used automatically for ErrorMessageException
        var caughtException = app.RunWithExpectedException(expectedExitCode: 1, "error");

        caughtException.ShouldNotBeNull();
        caughtException.ShouldBeOfType<ErrorMessageException>();
        caughtException.Message.ShouldBe("This is an error message.");

        // This must be the only output.
        app.TerminalOutput.Trim().ShouldBe(TermText.Red("This is an error message."));
    }

    [Fact]
    public void TestVerbs_Null()
    {
        var app = new TestApplicationWithVerbsBase();

        var caughtException = app.RunWithExpectedException("abc");

        caughtException.ShouldNotBeNull();
        caughtException.ShouldBeOfType<InvalidOperationException>();
        caughtException.Message.ShouldBe($"The property '{nameof(app.Verbs)}' has never been set.");
    }

    [Fact]
    public void TestVerbs_Empty()
    {
        var app = new TestApplicationWithVerbsBase()
        {
            Verbs = Array.Empty<CliVerb>(),
        };

        var caughtException = app.RunWithExpectedException("abc");

        caughtException.ShouldNotBeNull();
        caughtException.ShouldBeOfType<InvalidOperationException>();
        caughtException.Message.ShouldBe($"No verbs have be defined in property '{nameof(app.Verbs)}'.");
    }

    [Fact]
    public void TestVerbs_NullVerb()
    {
        var app = new TestApplicationWithVerbsBase()
        {
            Verbs = new CliVerb[] { null!, },
        };

        var caughtException = app.RunWithExpectedException("abc");

        caughtException.ShouldNotBeNull();
    }

    private sealed class ExceptionTestApplication : TestApplicationWithVerbs
    {
        /// <inheritdoc />
        protected override int ExitCodeOnException => 42;

        /// <inheritdoc />
        public ExceptionTestApplication(bool throwErrorMessageException)
            : base(new CliVerb("error", new ErrorCommand(throwErrorMessageException)))
        {
        }

        private sealed class ErrorCommand : CliCommand
        {
            private readonly bool _throwErrorMessageException;

            /// <inheritdoc />
            public ErrorCommand(bool throwErrorMessageException)
            {
                this._throwErrorMessageException = throwErrorMessageException;
            }

            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            private void Execute()
            {
                if (this._throwErrorMessageException)
                {
                    throw new ErrorMessageException("This is an error message.");
                }
                else
                {
                    throw new InvalidOperationException("This is a test");
                }
            }
        }
    }

    private sealed class Test1Command : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private readonly CliParam<int> _value = new("--value1");

        private int Execute()
        {
            return this._value.Value;
        }
    }

    private sealed class Test2Command : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private readonly CliParam<int> _value = new("--value2");

        private int Execute()
        {
            return this._value.Value;
        }
    }
}
