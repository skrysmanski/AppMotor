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

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;
using AppMotor.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

public sealed class CliApplicationWithVerbsTests
{
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
        app.TerminalOutput.Trim().ShouldBe("This is an error message.");
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
}