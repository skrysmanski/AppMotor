// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// A <see cref="TypeConverter"/> that converts from and to <c>string</c>.
/// </summary>
public abstract class StringTypeConverter<T> : TypeConverter where T : notnull
{
    /// <inheritdoc />
    public sealed override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    /// <inheritdoc />
    public sealed override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    /// <inheritdoc />
    public sealed override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var stringValue = (string)value;

        return ConvertFrom(stringValue, context, culture);
    }

    /// <summary>
    /// Converts the specified string into the type of this converter.
    /// </summary>
    [PublicAPI]
    protected abstract T ConvertFrom(string value, ITypeDescriptorContext? context, CultureInfo? culture);

    /// <inheritdoc />
    public sealed override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is null)
        {
            // We should not get here - but just in case.
            return null;
        }

        var valueAsT = (T)value;

        return ConvertTo(valueAsT, context, culture);
    }

    /// <summary>
    /// Converts the specified instance into a string.
    /// </summary>
    [PublicAPI]
    protected abstract string ConvertTo(T value, ITypeDescriptorContext? context, CultureInfo? culture);
}
