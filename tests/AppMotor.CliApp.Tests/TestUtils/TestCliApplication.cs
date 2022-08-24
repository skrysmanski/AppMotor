// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp.TestUtils;

internal class TestCliApplication : TestCliApplicationBase
{
    /// <inheritdoc />
    protected override CliApplicationExecutor MainExecutor { get; }

    /// <inheritdoc />
    public TestCliApplication(Action mainAction)
    {
        this.MainExecutor = new(mainAction);
    }
}