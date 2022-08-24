// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Default implementation of <see cref="IValueFormatter"/>. Supports <see cref="IConvertible"/>
/// and <see cref="IFormattable"/>. Does not provide any custom format strings in <see cref="FormatValue{T}"/>.
/// </summary>
public class DefaultValueFormatter : IValueFormatter
{
    private readonly IFormatProvider? _formatProvider;

    /// <summary>
    /// The format provider to be used for formatting values.
    /// </summary>
    [PublicAPI]
    public IFormatProvider FormatProvider => this._formatProvider ?? UICulture.FormatsAndSorting;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="formatProvider">The format provider to be used for formatting values. If <c>null</c>,
    /// <see cref="UICulture.FormatsAndSorting"/> will be used.</param>
    public DefaultValueFormatter(IFormatProvider? formatProvider)
    {
        this._formatProvider = formatProvider;
    }

    /// <inheritdoc />
    public virtual string? FormatValue<T>(T value)
    {
        switch (value)
        {
            case null:
                return null;

            case IFormattable formattable:
                return formattable.ToString(format: null, this.FormatProvider);

            case IConvertible convertible:
                return convertible.ToString(this.FormatProvider);

            default:
                return value.ToString();
        }
    }
}