// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.TestCore;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

/// <summary>
/// Tests for <see cref="AnsiTextFormatter"/>. Note that more tests are
/// located in <see cref="TermTextTests"/>.
/// </summary>
public sealed class AnsiTextFormatterTests
{
    [Fact]
    public void Test_Text_FromChar()
    {
        new AnsiTextFormatter().Underline().Text('a').ShouldBe("\u001b[4ma\u001b[0m");
    }

    [Fact]
    public void Test_Equals()
    {
        var formatter1 = new AnsiTextFormatter().Underline();
        var formatter2 = new AnsiTextFormatter().Underline();
        var formatter3 = new AnsiTextFormatter().Inverse();

        EqualityMembersTests.TestEquals(formatter1, formatter2, formatter3);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var formatter1 = new AnsiTextFormatter().Underline();
        var formatter2 = new AnsiTextFormatter().Underline();

        EqualityMembersTests.TestGetHashCode(formatter1, formatter2);
    }
}
