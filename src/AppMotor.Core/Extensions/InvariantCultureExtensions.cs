// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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
    public static string ToStringIC<T>(this T value) where T : IConvertible
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
