// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="StringExtensions"/>.
/// </summary>
public sealed class StringExtensionsTests
{
    [Fact]
    public void Test_ReplaceLineEndings()
    {
        "abc\ndef\r\nghi".ReplaceLineEndings(LineTerminators.CrLf).ShouldBe("abc\r\ndef\r\nghi");
        "abc\ndef\r\nghi".ReplaceLineEndings(LineTerminators.Lf).ShouldBe("abc\ndef\nghi");
        "abc\ndef\r\nghi".ReplaceLineEndings(LineTerminators.SystemDefault).ShouldBe($"abc{Environment.NewLine}def{Environment.NewLine}ghi");
    }
}
