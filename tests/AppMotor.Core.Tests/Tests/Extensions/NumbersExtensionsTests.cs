// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="NumbersExtensions"/>.
/// </summary>
public sealed class NumbersExtensionsTests
{
    [Theory]
    [InlineData(0.0f, 0.0f, true)]
    [InlineData(0.0f, 0.00009f, true)]
    [InlineData(0.0f, 0.0001f, false)]
    [InlineData(0.0f, -0.00009f, true)]
    [InlineData(0.0f, -0.0001f, false)]
    public void Test_IsBasicallyEqualTo_Float(float a, float b, bool expectToBeEqual)
    {
        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }

    [Theory]
    [InlineData(0.0, 0.0, true)]
    [InlineData(0.0, 0.00009, true)]
    [InlineData(0.0, 0.0001, false)]
    [InlineData(0.0, -0.00009, true)]
    [InlineData(0.0, -0.0001, false)]
    public void Test_IsBasicallyEqualTo_Double(double a, double b, bool expectToBeEqual)
    {
        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }

    [Theory]
    [InlineData("0.0", "0.0", true)]
    [InlineData("0.0", "0.00009", true)]
    [InlineData("0.0", "0.0001", false)]
    [InlineData("0.0", "-0.00009", true)]
    [InlineData("0.0", "-0.0001", false)]
    public void Test_IsBasicallyEqualTo_Decimal(string aAsString, string bAsString, bool expectToBeEqual)
    {
        var a = decimal.Parse(aAsString, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bAsString, CultureInfo.InvariantCulture);

        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }
}
