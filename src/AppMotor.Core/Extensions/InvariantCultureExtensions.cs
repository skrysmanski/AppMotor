// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Globalization;
using System.Text;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods that format data with <see cref="CultureInfo.InvariantCulture"/>.
/// </summary>
/// <seealso cref="CurrentCultureExtensions"/>
public static class InvariantCultureExtensions
{
    /// <summary>
    /// Convenience method for calling <c>string.Format()</c> with <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <seealso cref="StringExtensions.With"/>
    [PublicAPI, MustUseReturnValue]
    [StringFormatMethod("message")]
    public static string WithIC([Localizable(false)] this string message, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, message, args);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public static string ToStringIC<T>(this T value) where T : IConvertible
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns this values as string with invariant culture formatting.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public static string ToStringIC<T>(this T value, string format) where T : IFormattable
    {
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with invariant culture formatting.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatIC(this StringBuilder stringBuilder, string format, object? arg0)
    {
        return stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, arg0);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with invariant culture formatting.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatIC(this StringBuilder stringBuilder, string format, object? arg0, object? arg1)
    {
        return stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, arg0, arg1);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with invariant culture formatting.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatIC(this StringBuilder stringBuilder, string format, object? arg0, object? arg1, object? arg2)
    {
        return stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with invariant culture formatting.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatIC(this StringBuilder stringBuilder, string format, params object?[] args)
    {
        return stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);
    }
}
