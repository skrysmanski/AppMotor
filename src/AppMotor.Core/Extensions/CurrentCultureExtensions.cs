// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Text;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods that format data with the current UI culture (<see cref="UICulture.FormatsAndSorting"/>).
/// </summary>
/// <seealso cref="InvariantCultureExtensions"/>
/// <seealso cref="UICulture"/>
public static class CurrentCultureExtensions
{
    /// <summary>
    /// Converts this value into a string with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public static string ToStringCC<T>(this T value) where T : IConvertible
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts this value into a string with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public static string ToStringCC<T>(this T value, string format) where T : IFormattable
    {
        return value.ToString(format, UICulture.FormatsAndSorting);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatCC(this StringBuilder stringBuilder, string format, object? arg0)
    {
        return stringBuilder.AppendFormat(UICulture.FormatsAndSorting, format, arg0);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatCC(this StringBuilder stringBuilder, string format, object? arg0, object? arg1)
    {
        return stringBuilder.AppendFormat(UICulture.FormatsAndSorting, format, arg0, arg1);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatCC(this StringBuilder stringBuilder, string format, object? arg0, object? arg1, object? arg2)
    {
        return stringBuilder.AppendFormat(UICulture.FormatsAndSorting, format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Convenience method for calling <c>StringBuilder.AppendFormat()</c> with <see cref="UICulture.FormatsAndSorting"/>.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static StringBuilder AppendFormatCC(this StringBuilder stringBuilder, string format, params object?[] args)
    {
        return stringBuilder.AppendFormat(UICulture.FormatsAndSorting, format, args);
    }
}
