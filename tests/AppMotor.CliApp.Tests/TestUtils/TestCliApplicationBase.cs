// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.TestUtils;

internal abstract class TestCliApplicationBase : CliApplication, ITestApplication
{
    public readonly TestApplicationHelper AppHelper;

    /// <inheritdoc />
    public string TerminalOutput => this.AppHelper.TerminalOutput;

    protected TestCliApplicationBase()
    {
        this.AppHelper = new TestApplicationHelper(this);
    }

    public new void Run(params string[] args)
    {
        this.AppHelper.Run(args, expectedExitCode: 0);
    }

    public void RunWithExpectedExitCode(int expectedExitCode, params string[] args)
    {
        this.AppHelper.Run(args, expectedExitCode: expectedExitCode);
    }

    [MustUseReturnValue]
    public Exception RunWithExpectedException(params string[] args)
    {
        return this.AppHelper.RunWithExpectedException(args, expectedExitCode: this.ExitCodeOnException);
    }

    [MustUseReturnValue]
    public Exception RunWithExpectedException(int expectedExitCode, params string[] args)
    {
        return this.AppHelper.RunWithExpectedException(args, expectedExitCode: expectedExitCode);
    }

    /// <inheritdoc />
    protected override void OnUnhandledException(Exception exception, ref int exitCode)
    {
        this.AppHelper.OnUnhandledException(exception);

        base.OnUnhandledException(exception, ref exitCode);
    }
}
