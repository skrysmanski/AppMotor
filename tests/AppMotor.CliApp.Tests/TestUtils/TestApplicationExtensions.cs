// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using Shouldly;

namespace AppMotor.CliApp.TestUtils;

internal static class TestApplicationExtensions
{
    public static void ShouldHaveNoOutput(this ITestApplication app)
    {
        app.TerminalOutput.ShouldBeEmpty();
    }
}
