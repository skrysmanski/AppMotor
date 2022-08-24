// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

public sealed class ColoredSubStringTests
{
    [Fact]
    public void TestEquals_Equatable()
    {
        IEquatable<ColoredSubstring> text = (TextInBlack)"abc";

        text.Equals(null).ShouldBe(false);
        text!.Equals(text).ShouldBe(true);
        text.Equals((TextInBlack)"abc").ShouldBe(true);
        text.Equals((TextInWhite)"abc").ShouldBe(false);
        text.Equals((TextInBlack)"ab").ShouldBe(false);
    }

    [Fact]
    public void TestEquals_Object()
    {
        object text = (TextInBlack)"abc";

        text.Equals(null).ShouldBe(false);
        text!.Equals(text).ShouldBe(true);
        text.Equals((TextInBlack)"abc").ShouldBe(true);
        text.Equals((TextInWhite)"abc").ShouldBe(false);
        text.Equals((TextInBlack)"ab").ShouldBe(false);
    }

    [Fact]
    public void TestGetHashCode()
    {
        ((TextInBlack)"abc").GetHashCode().ShouldBe(((TextInBlack)"abc").GetHashCode());
        ((TextInBlack)"abc").GetHashCode().ShouldNotBe(((TextInWhite)"abc").GetHashCode());
    }
}