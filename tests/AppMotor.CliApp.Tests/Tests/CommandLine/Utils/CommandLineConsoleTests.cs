// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.Terminals;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine.Utils;

/// <summary>
/// Tests for <see cref="CommandLineConsole"/>.
/// </summary>
public sealed class CommandLineConsoleTests
{
    [Fact]
    public void Test_FromTerminal_RealTerminal()
    {
        CommandLineConsole.FromTerminal(Terminal.Instance).ShouldBe(null);
    }
}
