// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.DataModel;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.DataModel;

/// <summary>
/// Tests for <see cref="Optional{T}"/>.
/// </summary>
/// <remarks>
/// NOTE: Since the main use case for <see cref="Optional{T}"/> is to make "null" and "unset" distinguishable,
/// we only test nullable types here.
/// </remarks>
public sealed class OptionalTests
{
    [Fact]
    public void Test_Unset_RefType()
    {
        var optional = Optional<string?>.UNSET;

        optional.HasValue.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void Test_Unset_ValueType()
    {
        var optional = Optional<int?>.UNSET;

        optional.HasValue.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => optional.Value);
        optional.ToString().ShouldBe("<not set>");
    }

    [Fact]
    public void Test_RefType_Value()
    {
        Optional<string?> optional = "abc";

        optional.HasValue.ShouldBe(true);
        optional.Value.ShouldBe("abc");
        optional.ToString().ShouldBe("abc");
    }

    [Fact]
    public void Test_RefType_Null()
    {
        Optional<string?> optional = null;

        optional.HasValue.ShouldBe(true);
        optional.Value.ShouldBe(null);
        optional.ToString().ShouldBe("");
    }

    [Fact]
    public void Test_ValueType_Value()
    {
        Optional<int?> optional = 42;

        optional.HasValue.ShouldBe(true);
        optional.Value.ShouldBe(42);
        optional.ToString().ShouldBe("42");
    }

    [Fact]
    public void Test_ValueType_Null()
    {
        Optional<int?> optional = null;

        optional.HasValue.ShouldBe(true);
        optional.Value.ShouldBe(null);
        optional.ToString().ShouldBe("");
    }

    /// <summary>
    /// Test for <see cref="object.Equals(object?)"/> with reference types.
    /// </summary>
    [Fact]
    public void Test_Equals_Object_RefType()
    {
        new Optional<string?>().Equals((object?)new Optional<string?>()).ShouldBe(true);
        new Optional<string?>().Equals((object?)new Optional<string?>("")).ShouldBe(false);

        new Optional<string?>().Equals((object?)new Optional<string?>(null)).ShouldBe(false);
        new Optional<string?>(null).Equals((object?)new Optional<string?>(null)).ShouldBe(true);
        new Optional<string?>(null).Equals((object?)new Optional<string?>()).ShouldBe(false);

        // ReSharper disable SuspiciousTypeConversion.Global
        new Optional<string?>().Equals(new Optional<int?>()).ShouldBe(false);
        new Optional<string?>(null).Equals(new Optional<int?>(null)).ShouldBe(false);
        // ReSharper restore SuspiciousTypeConversion.Global

        new Optional<string?>("42").Equals((object?)new Optional<string?>()).ShouldBe(false);
        new Optional<string?>().Equals((object?)new Optional<string?>("42")).ShouldBe(false);
        new Optional<string?>("42").Equals((object?)new Optional<string?>("42")).ShouldBe(true);
    }

    /// <summary>
    /// Test for <see cref="object.Equals(object?)"/> with value types.
    /// </summary>
    [Fact]
    public void Test_Equals_Object_ValueType()
    {
        new Optional<int?>().Equals((object?)new Optional<int?>()).ShouldBe(true);
        new Optional<int?>().Equals((object?)new Optional<int?>(0)).ShouldBe(false);

        new Optional<int?>().Equals((object?)new Optional<int?>(null)).ShouldBe(false);
        new Optional<int?>(null).Equals((object?)new Optional<int?>(null)).ShouldBe(true);
        new Optional<int?>(null).Equals((object?)new Optional<int?>()).ShouldBe(false);

        // ReSharper disable SuspiciousTypeConversion.Global
        new Optional<int?>().Equals(new Optional<string?>()).ShouldBe(false);
        new Optional<int?>(null).Equals(new Optional<string?>(null)).ShouldBe(false);
        // ReSharper restore SuspiciousTypeConversion.Global

        new Optional<int?>(42).Equals((object?)new Optional<int?>()).ShouldBe(false);
        new Optional<int?>().Equals((object?)new Optional<int?>(42)).ShouldBe(false);
        new Optional<int?>(42).Equals((object?)new Optional<int?>(42)).ShouldBe(true);
    }

    /// <summary>
    /// Test for <see cref="IEquatable{T}.Equals(T?)"/> for <see cref="Optional{T}"/> with reference types.
    /// </summary>
    [Fact]
    public void Test_Equals_Typed_RefType()
    {
        new Optional<string?>().Equals(new Optional<string?>()).ShouldBe(true);
        new Optional<string?>().Equals(new Optional<string?>("")).ShouldBe(false);

        new Optional<string?>().Equals(new Optional<string?>(null)).ShouldBe(false);
        new Optional<string?>(null).Equals(new Optional<string?>(null)).ShouldBe(true);
        new Optional<string?>(null).Equals(new Optional<string?>()).ShouldBe(false);

        new Optional<string?>("42").Equals(new Optional<string?>()).ShouldBe(false);
        new Optional<string?>().Equals(new Optional<string?>("42")).ShouldBe(false);
        new Optional<string?>("42").Equals(new Optional<string?>("42")).ShouldBe(true);
    }

    /// <summary>
    /// Test for <see cref="IEquatable{T}.Equals(T?)"/> for <see cref="Optional{T}"/> with value types.
    /// </summary>
    [Fact]
    public void Test_Equals_Typed_ValueType()
    {
        new Optional<int?>().Equals(new Optional<int?>()).ShouldBe(true);
        new Optional<int?>().Equals(new Optional<int?>(0)).ShouldBe(false);

        new Optional<int?>().Equals(new Optional<int?>(null)).ShouldBe(false);
        new Optional<int?>(null).Equals(new Optional<int?>(null)).ShouldBe(true);
        new Optional<int?>(null).Equals(new Optional<int?>()).ShouldBe(false);

        new Optional<int?>(42).Equals(new Optional<int?>()).ShouldBe(false);
        new Optional<int?>().Equals(new Optional<int?>(42)).ShouldBe(false);
        new Optional<int?>(42).Equals(new Optional<int?>(42)).ShouldBe(true);
    }

    /// <summary>
    /// Test for <see cref="IEquatable{T}.Equals(T?)"/> for <c>T</c> with value types.
    /// </summary>
    [Fact]
    public void TestEquals_UnderlyingType_RefType()
    {
        new Optional<string?>().Equals("").ShouldBe(false);

        new Optional<string?>().Equals(null).ShouldBe(false);
        new Optional<string?>(null).Equals(null).ShouldBe(true);

        new Optional<string?>("42").Equals(null).ShouldBe(false);
        new Optional<string?>().Equals("42").ShouldBe(false);
        new Optional<string?>("42").Equals("42").ShouldBe(true);
    }

    /// <summary>
    /// Test for <see cref="IEquatable{T}.Equals(T?)"/> for <c>T</c> with value types.
    /// </summary>
    [Fact]
    public void TestEquals_UnderlyingType_ValueType()
    {
        new Optional<int?>().Equals(0).ShouldBe(false);

        new Optional<int?>().Equals(null).ShouldBe(false);
        new Optional<int?>(null).Equals(null).ShouldBe(true);

        new Optional<int?>(42).Equals(null).ShouldBe(false);
        new Optional<int?>().Equals(42).ShouldBe(false);
        new Optional<int?>(42).Equals(42).ShouldBe(true);
    }

    [Theory]
    [InlineData(true,  true)]
    [InlineData(true,  false)]
    [InlineData(true,  null)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    [InlineData(false, null)]
    [InlineData(null,  true)]
    [InlineData(null,  false)]
    [InlineData(null,  null)]
    public void Test_CompareOperators_OptionalType_RefType(bool? setValueA, bool? setValueB)
    {
        Optional<string?> valueA = setValueA switch
        {
            true => "abc",
            false => new Optional<string?>(),
            null => null,
        };
        Optional<string?> valueB = setValueB switch
        {
            true => "abc",
            false => new Optional<string?>(),
            null => null,
        };

        (valueA == valueB).ShouldBe(setValueA == setValueB);
        (valueA != valueB).ShouldBe(setValueA != setValueB);

        (valueB == valueA).ShouldBe(setValueA == setValueB);
        (valueB != valueA).ShouldBe(setValueA != setValueB);
    }

    [Theory]
    [InlineData(true,  true)]
    [InlineData(true,  false)]
    [InlineData(true,  null)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    [InlineData(false, null)]
    [InlineData(null,  true)]
    [InlineData(null,  false)]
    [InlineData(null,  null)]
    public void Test_CompareOperators_OptionalType_ValueType(bool? setValueA, bool? setValueB)
    {
        Optional<int?> valueA = setValueA switch
        {
            true => 42,
            false => new Optional<int?>(),
            null => null,
        };
        Optional<int?> valueB = setValueB switch
        {
            true => 42,
            false => new Optional<int?>(),
            null => null,
        };

        (valueA == valueB).ShouldBe(setValueA == setValueB);
        (valueA != valueB).ShouldBe(setValueA != setValueB);

        (valueB == valueA).ShouldBe(setValueA == setValueB);
        (valueB != valueA).ShouldBe(setValueA != setValueB);
    }

    [Theory]
    [InlineData(true,  "abc", true)]
    [InlineData(false, "abc", false)]
    [InlineData(null,  "abc", false)]
    [InlineData(true,  "",    false)]
    [InlineData(false, "",    false)]
    [InlineData(null,  "",    false)]
    [InlineData(true,  null,  false)]
    [InlineData(false, null,  false)]
    [InlineData(null,  null,  true)]
    public void Test_CompareOperators_UnderlyingType_RefType(bool? setValue, string? comparator, bool expectEquals)
    {
        Optional<string?> optional = setValue switch
        {
            true => "abc",
            false => new Optional<string?>(),
            null => null,
        };

        (optional == comparator).ShouldBe(expectEquals);
        (optional != comparator).ShouldBe(!expectEquals);

        (comparator == optional).ShouldBe(expectEquals);
        (comparator != optional).ShouldBe(!expectEquals);
    }

    [Theory]
    [InlineData(true,  42,   true)]
    [InlineData(false, 42,   false)]
    [InlineData(null,  42,   false)]
    [InlineData(true,  0,    false)]
    [InlineData(false, 0,    false)]
    [InlineData(null,  0,    false)]
    [InlineData(true,  null, false)]
    [InlineData(false, null, false)]
    [InlineData(null,  null, true)]
    public void Test_CompareOperators_UnderlyingType_ValueType(bool? setValue, int? comparator, bool expectEquals)
    {
        Optional<int?> optional = setValue switch
        {
            true => 42,
            false => new Optional<int?>(),
            null => null,
        };

        (optional == comparator).ShouldBe(expectEquals);
        (optional != comparator).ShouldBe(!expectEquals);

        (comparator == optional).ShouldBe(expectEquals);
        (comparator != optional).ShouldBe(!expectEquals);
    }

    [Fact]
    public void TestGetHashCode()
    {
        new Optional<int?>().GetHashCode().ShouldBe(0);
        new Optional<int?>(null).GetHashCode().ShouldBe(1);
        new Optional<int?>(42).GetHashCode().ShouldNotBe(0);

        new Optional<string?>().GetHashCode().ShouldBe(0);
        new Optional<string?>(null).GetHashCode().ShouldBe(1);
        new Optional<string?>("abc").GetHashCode().ShouldNotBe(0);
    }

    [Fact]
    public void TestToString()
    {
        new Optional<int?>().ToString().ShouldBe("<not set>");
        new Optional<int?>(null).ToString().ShouldBe("");
        new Optional<int?>(42).ToString().ShouldBe("42");

        new Optional<string?>().ToString().ShouldBe("<not set>");
        new Optional<string?>(null).ToString().ShouldBe("");
        new Optional<string?>("abc").ToString().ShouldBe("abc");
    }
}
