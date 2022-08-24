// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine.Utils;

using JetBrains.Annotations;

using Shouldly;

namespace AppMotor.CliApp.TestUtils;

internal sealed class TestApplicationHelper
{
    private readonly object _exceptionLock = new();

    private readonly TestTerminal _testTerminal = new();

    public string TerminalOutput => this._testTerminal.CurrentOutput;

    private Exception? _caughtException;

    private readonly CliApplication _application;

    public TestApplicationHelper(CliApplication application)
    {
        DebuggerUtils.IsTestRun = true;

        application.Terminal = this._testTerminal;

        this._application = application;
    }

    public void Run(string[] args, int expectedExitCode, CancellationToken cancellationToken = default)
    {
        this._testTerminal.ResetOutput();
        this._caughtException = null;

        int exitCode = this._application.Run(args, cancellationToken);

        var caughtException = this._caughtException;
        if (caughtException is not null)
        {
            throw new Exception($"Encountered unexpected exception: {caughtException.Message}", caughtException);
        }

        exitCode.ShouldBe(expectedExitCode, this.TerminalOutput);
    }

    [MustUseReturnValue]
    public Exception RunWithExpectedException(string[] args, int expectedExitCode, CancellationToken cancellationToken = default)
    {
        this._testTerminal.ResetOutput();
        this._caughtException = null;

        int exitCode = this._application.Run(args, cancellationToken);

        var caughtException = this._caughtException;
        caughtException.ShouldNotBeNull("Expected exception but got none.");

        exitCode.ShouldBe(expectedExitCode);
        this.TerminalOutput.ShouldNotBeEmpty();

        return caughtException;
    }

    public void OnUnhandledException(Exception exception)
    {
        lock (this._exceptionLock)
        {
            if (this._caughtException is not null)
            {
                this._caughtException = new AggregateException(this._caughtException, exception);
            }
            else
            {
                this._caughtException = exception;
            }
        }
    }
}