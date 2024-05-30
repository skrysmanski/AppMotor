// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Security;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Utils;

namespace AppMotor.Core.Logging;

/// <summary>
/// Like <see cref="DefaultValueFormatter"/> - just for the purpose of logging. Unlike <see cref="DefaultValueFormatter"/>,
/// it provides concrete format strings for various data types, hides sensitive information and its culture
/// defaults to <see cref="CultureInfo.InvariantCulture"/> instead of the current UI culture.
/// </summary>
public class DefaultLoggableValueFormatter : DefaultValueFormatter
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="formatProvider">The format provide to use for formatting values; defaults
    /// to <see cref="CultureInfo.InvariantCulture"/>.</param>
    public DefaultLoggableValueFormatter(IFormatProvider? formatProvider = null)
        // NOTE: For logging purposes it's better to use "InvariantCulture" than "CurrentUICulture".
        : base(formatProvider ?? CultureInfo.InvariantCulture)
    {
    }

    /// <inheritdoc />
    public override string? FormatValue<T>(T value)
    {
        switch (value)
        {
            case DateTime dateTime:
                // Output is similar to RFC 1123 ("r") but without day name and with timezone information.
                return dateTime.ToString("dd MMM yyy HH:mm:ss zzz", this.FormatProvider);

            case DateTimeOffset dateTime:
                // Output is similar to RFC 1123 ("r") but without day name and with timezone information.
                return dateTime.ToString("dd MMM yyy HH:mm:ss zzz", this.FormatProvider);

            // NOTE: TimeSpan is not listed here as it's default format is not globalizable.

            case SecureString:
            case ISensitiveValue:
                return "<hidden>";

            default:
                return base.FormatValue(value);
        }
    }
}
