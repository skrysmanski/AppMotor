// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using AppMotor.Core.Utils;

namespace AppMotor.TestCore.Utils;

/// <summary>
/// Provides all members/values of an enum as test data to be used as <c>[ClassData(typeof(EnumTestDataGenerator&lt;MyEnum&gt;))]</c>.
/// </summary>
public sealed class EnumTestDataGenerator<TEnum> : IEnumerable<object[]> where TEnum : Enum
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
