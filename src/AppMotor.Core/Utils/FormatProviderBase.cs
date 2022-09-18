// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Convenience base class for <see cref="IFormatProvider"/> implementations.
/// </summary>
[PublicAPI]
public abstract class FormatProviderBase : IFormatProvider
{
    private readonly Formatter _formatter;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected FormatProviderBase()
    {
        this._formatter = new Formatter(this);
    }

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(ICustomFormatter))
        {
            return this._formatter;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Formats the specified value with the specified format string.
    /// </summary>
    protected abstract string Format(string? format, object? value);

    private sealed class Formatter : ICustomFormatter
    {
        private readonly FormatProviderBase _outerFormatProvider;

        public Formatter(FormatProviderBase outerFormatProvider)
        {
            this._outerFormatProvider = outerFormatProvider;
        }

        /// <inheritdoc />
        public string Format(string? format, object? value, IFormatProvider? formatProvider)
        {
            return this._outerFormatProvider.Format(format, value);
        }
    }
}
