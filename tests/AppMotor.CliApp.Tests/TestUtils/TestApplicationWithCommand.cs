// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;

using JetBrains.Annotations;

namespace AppMotor.CliApp.TestUtils;

internal class TestApplicationWithCommand : CliApplicationWithCommand, ITestApplication
{
    private readonly TestApplicationHelper _appHelper;

    /// <inheritdoc />
    public string TerminalOutput => this._appHelper.TerminalOutput;

    public TestApplicationWithCommand()
    {
        this._appHelper = new TestApplicationHelper(this);
    }

    public TestApplicationWithCommand(CliCommand command) : base(command)
    {
        this._appHelper = new TestApplicationHelper(this);
    }

    public new void Run(params string[] args)
    {
        this._appHelper.Run(args, expectedExitCode: 0);
    }

    public void RunWithExpectedExitCode(int expectedExitCode, params string[] args)
    {
        this._appHelper.Run(args, expectedExitCode: expectedExitCode);
    }

    [MustUseReturnValue]
    public Exception RunWithExpectedException(params string[] args)
    {
        return this._appHelper.RunWithExpectedException(args, expectedExitCode: this.ExitCodeOnException);
    }

    [MustUseReturnValue]
    public Exception RunWithExpectedException(int expectedExitCode, params string[] args)
    {
        return this._appHelper.RunWithExpectedException(args, expectedExitCode: expectedExitCode);
    }

    /// <inheritdoc />
    protected override void OnUnhandledException(Exception exception, ref int exitCode)
    {
        this._appHelper.OnUnhandledException(exception);

        base.OnUnhandledException(exception, ref exitCode);
    }
}
