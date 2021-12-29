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