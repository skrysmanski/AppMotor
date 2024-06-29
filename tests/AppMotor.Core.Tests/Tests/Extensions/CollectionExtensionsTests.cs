// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="Core.Extensions.CollectionExtensions"/>.
/// </summary>
public sealed class CollectionExtensionsTests
{
    [Fact]
    public void Test_WhereNotNull_RefType()
    {
        // Test
        // NOTE: We specify the result type here explicitly so that we have a compiler check to verify that the resulting item type is actually non-nullable.
        IEnumerable<string> result = new[] { "abc", null, "def", null, "ghi" }.WhereNotNull();

        // Verify
        result.ShouldBe(["abc", "def", "ghi"]);
    }

    [Fact]
    public void Test_WhereNotNull_ValueType()
    {
        // Test
        // NOTE: We specify the result type here explicitly so that we have a compiler check to verify that the resulting item type is actually non-nullable.
        IEnumerable<int> result = new int?[] { 42, null, 43, null, 44 }.WhereNotNull();

        // Verify
        result.ShouldBe([42, 43, 44]);
    }
}
