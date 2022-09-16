// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

/// <summary>
/// Tests for <see cref="LineTerminators"/> and <see cref="LineTerminatorsExtensions"/>.
/// </summary>
public sealed class LineTerminatorsTests
{
    [Fact]
    public void Test_AsString()
    {
        LineTerminators.SystemDefault.AsString().ShouldBe(Environment.NewLine);
        LineTerminators.CrLf.AsString().ShouldBe("\r\n");
        LineTerminators.Lf.AsString().ShouldBe("\n");
        Should.Throw<ArgumentOutOfRangeException>(() => ((LineTerminators)5000).AsString());
    }
}
