#region License
// Copyright 2022 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
