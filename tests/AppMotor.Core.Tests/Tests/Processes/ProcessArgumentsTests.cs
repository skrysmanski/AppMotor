// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics;

using AppMotor.Core.Processes;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Processes;

/// <summary>
/// Tests for <see cref="ProcessArguments"/>.
/// </summary>
public sealed class ProcessArgumentsTests
{
    [Fact]
    public void Test_Constructor_Array()
    {
        new ProcessArguments().Count.ShouldBe(0);
        new ProcessArguments(Array.Empty<string>()).Count.ShouldBe(0);
        // ReSharper disable once RedundantCast
        new ProcessArguments((string[]?)null).Count.ShouldBe(0);

        new ProcessArguments("abc", "def").ShouldBe(new[] { "abc", "def" });
    }

    [Fact]
    public void Test_Constructor_Enumerable()
    {
        new ProcessArguments(new List<string>()).Count.ShouldBe(0);
        // ReSharper disable once RedundantCast
        new ProcessArguments((List<string>?)null).Count.ShouldBe(0);

        new ProcessArguments(new List<string>() { "abc", "def" }).ShouldBe(new[] { "abc", "def" });
    }

    [Fact]
    public void Test_CopyTo()
    {
        // Setup
        var processArgs = new ProcessArguments("abc", "def");
        var startInfo = new ProcessStartInfo()
        {
            Arguments = "will-not-be-used",
        };

        // Test
        processArgs.CopyTo(startInfo);

        // Verify
        startInfo.Arguments.ShouldBe("");
        startInfo.ArgumentList.ShouldBe(new[] { "abc", "def" });
    }

    [Fact]
    public void Test_ToString()
    {
        new ProcessArguments().ToString().ShouldBe("");
        new ProcessArguments("abc", "def").ToString().ShouldBe("abc def");
        new ProcessArguments("abc xyz", "def").ToString().ShouldBe("\"abc xyz\" def");
    }
}
