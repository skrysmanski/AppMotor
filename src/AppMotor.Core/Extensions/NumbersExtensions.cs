// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for number types.
/// </summary>
/// <seealso cref="InvariantCultureExtensions"/>
public static class NumbersExtensions
{
    /// <summary>
    /// Returns whether this number is basically equal to the specified number. They're considered equal if
    /// their difference is less than the specified precision.
    /// </summary>
    [MustUseReturnValue]
    public static bool IsBasicallyEqualTo(this float thisNumber, float comparisonValue, float precision = 0.0001f)
    {
        return Math.Abs(thisNumber - comparisonValue) < precision;
    }

    /// <summary>
    /// Returns whether this number is basically equal to the specified number. They're considered equal if
    /// their difference is less than the specified precision.
    /// </summary>
    [MustUseReturnValue]
    public static bool IsBasicallyEqualTo(this double thisNumber, double comparisonValue, double precision = 0.0001)
    {
        return Math.Abs(thisNumber - comparisonValue) < precision;
    }

    /// <summary>
    /// Returns whether this number is basically equal to the specified number. They're considered equal if
    /// their difference is less than the specified precision.
    /// </summary>
    [MustUseReturnValue]
    public static bool IsBasicallyEqualTo(this decimal thisNumber, decimal comparisonValue, decimal precision = 0.0001m)
    {
        return Math.Abs(thisNumber - comparisonValue) < precision;
    }
}
