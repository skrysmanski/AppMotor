// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.Core.Extensions;
using AppMotor.Core.Globalization;
using AppMotor.TestCore;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.Core.Tests.Extensions;

public sealed class InvariantCultureExtensionsTests : TestBase
{
    /// <summary>
    /// This is a culture whose formats do not(!) match the format for <see cref="CultureInfo.InvariantCulture"/>.
    /// This will be used as thread culture - to make it visible if the invariant culture isn't used.
    /// </summary>
    private static readonly CultureInfo NON_ENGLISH_CULTURE = new("de-de");

    public InvariantCultureExtensionsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        UICulture.Language = NON_ENGLISH_CULTURE;
        UICulture.FormatsAndSorting = NON_ENGLISH_CULTURE;
    }

    [Fact]
    public void Test_WithIC()
    {
        "Some number: {0}".WithIC(123.7).ShouldBe("Some number: 123.7");
    }

    [Fact]
    public void Test_ToStringIC_Float()
    {
        // ReSharper disable once ConvertToConstant.Local
        float valueUnderTest = 127.6f;

        valueUnderTest.ToStringIC().ShouldBe("127.6");
    }

    [Fact]
    public void Test_ToStringIC_Double()
    {
        // ReSharper disable once ConvertToConstant.Local
        double valueUnderTest = 127.6;

        valueUnderTest.ToStringIC().ShouldBe("127.6");
    }

    [Fact]
    public void Test_ToStringIC_Decimal()
    {
        // ReSharper disable once ConvertToConstant.Local
        decimal valueUnderTest = 127.6m;

        valueUnderTest.ToStringIC().ShouldBe("127.6");
    }

    [Fact]
    public void Test_ToStringIC_DateTime()
    {
        DateTime valueUnderTest = new DateTime(2022, 6, 26, 10, 3, 45, DateTimeKind.Local);

        valueUnderTest.ToStringIC().ShouldBe("06/26/2022 10:03:45");
    }

    [Fact]
    public void Test_ToStringIC_Formattable()
    {
        // ReSharper disable once ConvertToConstant.Local
        double valueUnderTest = 127.6;

        valueUnderTest.ToStringIC("F4").ShouldBe("127.6000");
    }
}
