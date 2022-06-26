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

using System.ComponentModel;
using System.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Provides methods that format things with <see cref="CultureInfo.InvariantCulture"/>.
/// </summary>
public static class InvariantCultureExtensions
{
    /// <summary>
    /// Convenience method for calling <c>string.Format()</c> with <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <seealso cref="StringExtensions.With"/>
    [StringFormatMethod("message")]
    [MustUseReturnValue]
    public static string WithIC([Localizable(false)] this string message, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, message, args);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [MustUseReturnValue]
    public static string ToStringIC(this float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [MustUseReturnValue]
    public static string ToStringIC(this double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [MustUseReturnValue]
    public static string ToStringIC(this decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    /// <remarks>
    /// Try not to use this method as the returned date format may be confusing to read for
    /// non-English speakers.
    /// </remarks>
    [MustUseReturnValue]
    public static string ToStringIC(this DateTime value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [MustUseReturnValue]
    public static string ToStringIC<T>(this T value, string format) where T : IFormattable
    {
        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}
