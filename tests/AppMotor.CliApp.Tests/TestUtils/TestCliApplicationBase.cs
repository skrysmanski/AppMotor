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

using System;

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