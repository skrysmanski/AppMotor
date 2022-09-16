// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="StringBuilderExtensions"/>.
/// </summary>
public sealed class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData(LineTerminators.SystemDefault)]
    [InlineData(LineTerminators.CrLf)]
    [InlineData(LineTerminators.Lf)]
    public void Test_AppendLine(LineTerminators lineTerminator)
    {
        new StringBuilder().AppendLine(lineTerminator).ToString().ShouldBe(lineTerminator.AsString());
    }

    [Fact]
    public void Test_AppendLineLf()
    {
        new StringBuilder().AppendLineLf().ToString().ShouldBe("\n");
    }

    [Fact]
    public void Test_AppendLineCrLf()
    {
        new StringBuilder().AppendLineCrLf().ToString().ShouldBe("\r\n");
    }
}
