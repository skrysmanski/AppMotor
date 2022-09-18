// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

public sealed class ParamsUtilsTests
{
    [Fact]
    public void TestCombineOne()
    {
        ParamsUtils.Combine(42, Array.Empty<int>()).ShouldBe(new[] { 42 });
        ParamsUtils.Combine(42, new[] { 43 }).ShouldBe(new[] { 42, 43 });
        ParamsUtils.Combine(42, new[] { 43, 44 }).ShouldBe(new[] { 42, 43, 44 });
    }

    [Fact]
    public void TestCombineTwo()
    {
        ParamsUtils.Combine(42, 43, Array.Empty<int>()).ShouldBe(new[] { 42, 43 });
        ParamsUtils.Combine(42, 43, new[] { 44 }).ShouldBe(new[] { 42, 43, 44 });
        ParamsUtils.Combine(42, 43, new[] { 44, 45 }).ShouldBe(new[] { 42, 43, 44, 45 });
    }
}
