// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

using AppMotor.Core.DataModel;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.DataModel;

public sealed class OptionalTests
{
    [Fact]
    public void TestEmpty_NullableRefType()
    {
        var optional = Optional<string?>.UNSET;

        optional.IsSet.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void TestEmpty_NonNullableRefType()
    {
        var optional = new Optional<string>();

        optional.IsSet.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void TestEmpty_NullableValueType()
    {
        var optional = Optional<int?>.UNSET;

        optional.IsSet.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void TestEmpty_NonNullableValueType()
    {
        var optional = new Optional<int>();

        optional.IsSet.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void Test_NullableRefType()
    {
        Optional<string?> optional = "abc";

        optional.IsSet.ShouldBe(true);
        optional.Value.ShouldBe("abc");
        optional.ToString().ShouldBe("abc");
    }

    [Fact]
    public void Test_NonNullableRefType()
    {
        Optional<string> optional = "abc";

        optional.IsSet.ShouldBe(true);
        optional.Value.ShouldBe("abc");
        optional.ToString().ShouldBe("abc");
    }

    [Fact]
    public void Test_NullableValueType()
    {
        Optional<int?> optional = 42;

        optional.IsSet.ShouldBe(true);
        optional.Value.ShouldBe(42);
        optional.ToString().ShouldBe("42");
    }

    [Fact]
    public void Test_NonNullableValueType()
    {
        Optional<int> optional = 42;

        optional.IsSet.ShouldBe(true);
        optional.Value.ShouldBe(42);
        optional.ToString().ShouldBe("42");
    }

    [Fact]
    public void TestEquals_ValueTypes()
    {
        new Optional<int>().Equals((object?)new Optional<int>()).ShouldBe(true);
        new Optional<int>().Equals((object?)new Optional<int>(0)).ShouldBe(false);
        // ReSharper disable once SuspiciousTypeConversion.Global
        new Optional<int>().Equals(new Optional<string>()).ShouldBe(false);

        new Optional<int>(42).Equals((object?)new Optional<int>()).ShouldBe(false);
        new Optional<int>().Equals((object?)new Optional<int>(42)).ShouldBe(false);
        new Optional<int>(42).Equals((object?)new Optional<int>(42)).ShouldBe(true);
    }

    [Fact]
    public void TestEquals_RefTypes()
    {
        new Optional<string>().Equals((object?)new Optional<string>()).ShouldBe(true);
        new Optional<string>().Equals((object?)new Optional<string?>(null)).ShouldBe(false);
        // ReSharper disable once SuspiciousTypeConversion.Global
        new Optional<string>().Equals(new Optional<int>()).ShouldBe(false);

        new Optional<string>("abc").Equals((object?)new Optional<string>()).ShouldBe(false);
        new Optional<string>("abc").Equals((object?)new Optional<string?>(null)).ShouldBe(false);
        new Optional<string>().Equals((object?)new Optional<string>("abc")).ShouldBe(false);
        new Optional<string?>(null).Equals((object?)new Optional<string>("abc")).ShouldBe(false);
        new Optional<string>("abc").Equals((object?)new Optional<string>("abc")).ShouldBe(true);
    }

    [Fact]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public void TestCompareOperators_ValueTypes()
    {
        (new Optional<int>() == new Optional<int>()).ShouldBe(true);
        (new Optional<int>() != new Optional<int>()).ShouldBe(false);
        (new Optional<int>() == new Optional<int>(0)).ShouldBe(false);
        (new Optional<int>() != new Optional<int>(0)).ShouldBe(true);

        (new Optional<int>(42) == new Optional<int>()).ShouldBe(false);
        (new Optional<int>(42) != new Optional<int>()).ShouldBe(true);
        (new Optional<int>() == new Optional<int>(42)).ShouldBe(false);
        (new Optional<int>() != new Optional<int>(42)).ShouldBe(true);
        (new Optional<int>(42) == new Optional<int>(42)).ShouldBe(true);
        (new Optional<int>(42) != new Optional<int>(42)).ShouldBe(false);
    }

    [Fact]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public void TestCompareOperators_RefTypes()
    {
        (new Optional<string>() == new Optional<string>()).ShouldBe(true);
        (new Optional<string>() != new Optional<string>()).ShouldBe(false);
        (new Optional<string?>() == new Optional<string?>(null)).ShouldBe(false);
        (new Optional<string?>() != new Optional<string?>(null)).ShouldBe(true);

        (new Optional<string>("abc") == new Optional<string>()).ShouldBe(false);
        (new Optional<string>("abc") != new Optional<string>()).ShouldBe(true);
        (new Optional<string?>("abc") == new Optional<string?>(null)).ShouldBe(false);
        (new Optional<string?>("abc") != new Optional<string?>(null)).ShouldBe(true);
        (new Optional<string>() == new Optional<string>("abc")).ShouldBe(false);
        (new Optional<string>() != new Optional<string>("abc")).ShouldBe(true);
        (new Optional<string?>(null) == new Optional<string?>("abc")).ShouldBe(false);
        (new Optional<string?>(null) != new Optional<string?>("abc")).ShouldBe(true);
        (new Optional<string>("abc") == new Optional<string>("abc")).ShouldBe(true);
        (new Optional<string>("abc") != new Optional<string>("abc")).ShouldBe(false);
    }

    [Fact]
    public void TestGetHashCode()
    {
        new Optional<int>().GetHashCode().ShouldBe(0);
        new Optional<int>(42).GetHashCode().ShouldNotBe(0);
        new Optional<string>().GetHashCode().ShouldBe(0);
        new Optional<string?>(null).GetHashCode().ShouldNotBe(0);
        new Optional<string>("abc").GetHashCode().ShouldNotBe(0);
    }

    [Fact]
    public void TestToString()
    {
        new Optional<int>().ToString().ShouldBe(Optional<int>.NOT_SET_TO_STRING_RESULT);
        new Optional<int>(42).ToString().ShouldBe("42");
        new Optional<string>().ToString().ShouldBe(Optional<string>.NOT_SET_TO_STRING_RESULT);
        new Optional<string?>(null).ToString().ShouldBe("");
        new Optional<string>("abc").ToString().ShouldBe("abc");
    }
}
