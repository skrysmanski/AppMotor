// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.CliApp.Terminals;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

/// <summary>
/// Tests for <see cref="SimpleTerminalWriter"/>.
/// </summary>
public sealed class SimpleTerminalWriterTests
{
    [Fact]
    public void Test_Write_WithColors()
    {
        // Setup
        var outputBuilder = new StringBuilder();
        var terminalWriter = new SimpleTerminalWriter(val => outputBuilder.Append(val))
        {
            EnableColors = true,
        };

        // Test
        terminalWriter.Write("abc \u001b[4;6mmy_test\u001b[0m \u001b[33mdef\u001b[0m");

        // Verify
        outputBuilder.ToString().ShouldBe("abc \u001b[4;6mmy_test\u001b[0m \u001b[33mdef\u001b[0m");
    }

    [Fact]
    public void Test_Write_NoColors()
    {
        // Setup
        var outputBuilder = new StringBuilder();
        var terminalWriter = new SimpleTerminalWriter(val => outputBuilder.Append(val))
        {
            EnableColors = false,
        };

        // Test
        terminalWriter.Write("abc \u001b[4;6mmy_test\u001b[0m \u001b[33mdef\u001b[0m");

        // Verify
        outputBuilder.ToString().ShouldBe("abc \u001b[4m\u001b[6mmy_test\u001b[0m def\u001b[0m");
    }
}
