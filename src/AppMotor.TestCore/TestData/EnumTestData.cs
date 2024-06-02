// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using AppMotor.Core.Utils;

namespace AppMotor.TestCore.TestData;

/// <summary>
/// Provides all members/values of an enum as test data to be used as <c>[ClassData(typeof(EnumTestData&lt;MyEnum&gt;))]</c>.
/// </summary>
public sealed class EnumTestData<TEnum> : IEnumerable<object[]> where TEnum : Enum
{
    /// <inheritdoc />
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var enumValue in EnumUtils.GetValues<TEnum>())
        {
            yield return [enumValue];
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
