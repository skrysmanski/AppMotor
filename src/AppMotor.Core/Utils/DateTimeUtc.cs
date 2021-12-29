#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Like <see cref="DateTime"/> but always in UTC (i.e. <see cref="DateTimeKind.Utc"/>).
/// </summary>
/// <remarks>
/// This type makes it easier/less error prone to operate with date times in the backend - in that there is
/// no confusion whether the instance is in UTC or local time (like with <see cref="DateTime"/>).
/// </remarks>
[JsonConverter(typeof(DateTimeUtcJsonConverter))]
public readonly struct DateTimeUtc : IEquatable<DateTimeUtc>, IComparable<DateTimeUtc>, IComparable, IFormattable
{
    private readonly DateTime _underlyingDateTime;

    /// <summary>
    /// The current date and time.
    /// </summary>
    public static DateTimeUtc Now => new(DateTime.UtcNow);

    /// <summary>
    /// The date part of this date time.
    /// </summary>
    public DateOnly Date => DateOnly.FromDateTime(this._underlyingDateTime);

    /// <summary>
    /// The time part of this date time.
    /// </summary>
    public TimeOnly TimeOfDay => TimeOnly.FromDateTime(this._underlyingDateTime);

    /// <summary>
    /// The year component of this instance.
    /// </summary>
    public int Year => this._underlyingDateTime.Year;

    /// <summary>
    /// The month component of this instance.
    /// </summary>
    public int Month => this._underlyingDateTime.Month;

    /// <summary>
    /// The day (of the month) component of this instance.
    /// </summary>
    public int Day => this._underlyingDateTime.Day;

    /// <summary>
    /// The day of the week of this instance.
    /// </summary>
    public DayOfWeek DayOfWeek => this._underlyingDateTime.DayOfWeek;

    /// <summary>
    /// The day of the year of this instance.
    /// </summary>
    public int DayOfYear => this._underlyingDateTime.DayOfYear;

    /// <summary>
    /// The hour component (0 - 23) of this instance.
    /// </summary>
    public int Hour => this._underlyingDateTime.Hour;

    /// <summary>
    /// The minute component (0 - 59) of this instance.
    /// </summary>
    public int Minute => this._underlyingDateTime.Minute;

    /// <summary>
    /// The second component (0 - 59) of this instance.
    /// </summary>
    public int Second => this._underlyingDateTime.Second;

    /// <summary>
    /// The millisecond component (0 - 999) of this instance.
    /// </summary>
    public int Millisecond => this._underlyingDateTime.Millisecond;

    /// <inheritdoc cref="DateTime.Ticks"/>
    public long Ticks => this._underlyingDateTime.Ticks;

    #region Constructors

    /// <summary>
    /// Constructs a <see cref="DateTimeUtc"/> instance from an existing <see cref="DateTime"/>.
    /// </summary>
    /// <param name="source">The date time source; must be either <see cref="DateTimeKind.Utc"/>
    /// or <see cref="DateTimeKind.Local"/>.</param>
    /// <exception cref="ArgumentException">Thrown when the source has the <see cref="DateTimeKind.Unspecified"/>.</exception>
    public DateTimeUtc(DateTime source)
    {
        switch (source.Kind)
        {
            case DateTimeKind.Utc:
                this._underlyingDateTime = source;
                break;

            case DateTimeKind.Local:
                this._underlyingDateTime = source.ToUniversalTime();
                break;

            default:
                throw new ArgumentException($"Unsupported DateTimeKind: {source.Kind}", nameof(source));
        }
    }

    /// <summary>
    /// Constructs a <see cref="DateTimeUtc"/> instance from an existing <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeUtc(DateTimeOffset source)
        : this(source.UtcDateTime)
    {
    }

    /// <summary>
    /// Constructs a <see cref="DateTimeUtc"/> instance from the specified ticks.
    /// </summary>
    public DateTimeUtc(long ticks)
        : this(new DateTime(ticks, DateTimeKind.Utc))
    {
    }

    /// <summary>
    /// Constructs a <see cref="DateTimeUtc"/> instance for the specified date and time.
    /// </summary>
    public DateTimeUtc(int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
        : this(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc))
    {
    }

    #endregion Constructors

    #region Parse Methods

    /// <summary>
    /// Parses <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <exception cref="FormatException">Thrown if the string could not be parsed</exception>
    [MustUseReturnValue]
    public static DateTimeUtc Parse(ReadOnlySpan<char> dateTimeAsString, IFormatProvider formatProvider, DateTimeStyles styles = DateTimeStyles.None)
    {
        if (TryParse(dateTimeAsString, formatProvider, styles, out var result))
        {
            return result;
        }

        var exception = new FormatException("The string could not be parsed.");
        exception.AddData("InputString", dateTimeAsString.ToString());
        throw exception;
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="result">The parse result</param>
    /// <remarks>
    /// This method uses <see cref="DateTimeStyles.None"/>.
    /// </remarks>
    [MustUseReturnValue]
    public static bool TryParse(ReadOnlySpan<char> dateTimeAsString, IFormatProvider formatProvider, out DateTimeUtc result)
    {
        return TryParse(dateTimeAsString, formatProvider, DateTimeStyles.None, out result);
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <param name="result">The parse result</param>
    [MustUseReturnValue]
    public static bool TryParse(ReadOnlySpan<char> dateTimeAsString, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeUtc result)
    {
        if (!DateTimeOffset.TryParse(dateTimeAsString, formatProvider, styles, out var dateTimeOffset))
        {
            result = default;
            return false;
        }

        result = new DateTimeUtc(dateTimeOffset);
        return true;
    }

    /// <summary>
    /// Parses <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="format">The format to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <exception cref="FormatException">Thrown if the string could not be parsed</exception>
    [MustUseReturnValue]
    public static DateTimeUtc ParseExact(ReadOnlySpan<char> dateTimeAsString, ReadOnlySpan<char> format, IFormatProvider formatProvider, DateTimeStyles styles = DateTimeStyles.None)
    {
        if (TryParseExact(dateTimeAsString, format, formatProvider, styles, out var result))
        {
            return result;
        }

        var exception = new FormatException("The string could not be parsed.");
        exception.AddData("InputString", dateTimeAsString.ToString());
        throw exception;
    }

    /// <summary>
    /// Parses <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formats">The formats to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <exception cref="FormatException">Thrown if the string could not be parsed</exception>
    [MustUseReturnValue]
    public static DateTimeUtc ParseExact(ReadOnlySpan<char> dateTimeAsString, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles = DateTimeStyles.None)
    {
        if (TryParseExact(dateTimeAsString, formats, formatProvider, styles, out var result))
        {
            return result;
        }

        var exception = new FormatException("The string could not be parsed.");
        exception.AddData("InputString", dateTimeAsString.ToString());
        throw exception;
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="format">The format to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="result">The parse result</param>
    /// <remarks>
    /// This method uses <see cref="DateTimeStyles.None"/>.
    /// </remarks>
    [MustUseReturnValue]
    public static bool TryParseExact(ReadOnlySpan<char> dateTimeAsString, ReadOnlySpan<char> format, IFormatProvider formatProvider, out DateTimeUtc result)
    {
        return TryParseExact(dateTimeAsString, format, formatProvider, DateTimeStyles.None, out result);
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="format">The format to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <param name="result">The parse result</param>
    [MustUseReturnValue]
    public static bool TryParseExact(ReadOnlySpan<char> dateTimeAsString, ReadOnlySpan<char> format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeUtc result)
    {
        if (!DateTimeOffset.TryParseExact(dateTimeAsString, format, formatProvider, styles, out var dateTimeOffset))
        {
            result = default;
            return false;
        }

        result = new DateTimeUtc(dateTimeOffset);
        return true;
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formats">The formats to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="result">The parse result</param>
    /// <remarks>
    /// This method uses <see cref="DateTimeStyles.None"/>.
    /// </remarks>
    [MustUseReturnValue]
    public static bool TryParseExact(ReadOnlySpan<char> dateTimeAsString, [NotNullWhen(true)] string?[]? formats, IFormatProvider formatProvider, out DateTimeUtc result)
    {
        return TryParseExact(dateTimeAsString, formats, formatProvider, DateTimeStyles.None, out result);
    }

    /// <summary>
    /// Tries to parse <paramref name="dateTimeAsString"/> into an instance of <see cref="DateTimeUtc"/>. If successful, the
    /// result will be stored in <paramref name="result"/> and <c>true</c> will be returned. If not successful, <c>false</c>
    /// will be returned.
    /// </summary>
    /// <param name="dateTimeAsString">The string to convert</param>
    /// <param name="formats">The formats to use; can use any format supported by <see cref="DateTimeOffset"/>.</param>
    /// <param name="formatProvider">The format provider to use</param>
    /// <param name="styles">The parse styles to use</param>
    /// <param name="result">The parse result</param>
    [MustUseReturnValue]
    public static bool TryParseExact(ReadOnlySpan<char> dateTimeAsString, [NotNullWhen(true)] string?[]? formats, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeUtc result)
    {
        if (!DateTimeOffset.TryParseExact(dateTimeAsString, formats, formatProvider, styles, out var dateTimeOffset))
        {
            result = default;
            return false;
        }

        result = new DateTimeUtc(dateTimeOffset);
        return true;
    }

    #endregion Parse Methods

    #region Calculation Members

    /// <summary>
    /// Adds the number of months to this date time and returns the result.
    /// </summary>
    /// <param name="months">The months; can be positive or negative.</param>
    /// <remarks>
    /// Most "add" functionality is implemented by using "dateTime + TimeSpan...". However, months can not
    /// be expressed precisely with a <see cref="TimeSpan"/> (since the number of days per month is not always
    /// the same). This is why this method exists.
    /// </remarks>
    [Pure]
    public DateTimeUtc AddMonths(int months)
    {
        return new(this._underlyingDateTime.AddMonths(months));
    }

    /// <summary>
    /// Adds the number of years to this date time and returns the result.
    /// </summary>
    /// <param name="years">The years; can be positive or negative.</param>
    /// <remarks>
    /// Most "add" functionality is implemented by using "dateTime + TimeSpan...". However, years can not
    /// be expressed precisely with a <see cref="TimeSpan"/> (since the number of days per year is not always
    /// the same). This is why this method exists.
    /// </remarks>
    [Pure]
    public DateTimeUtc AddYears(int years)
    {
        return new(this._underlyingDateTime.AddYears(years));
    }

    /// <summary>
    /// Adds operator
    /// </summary>
    public static DateTimeUtc operator +(DateTimeUtc d, TimeSpan t)
    {
        return new(d._underlyingDateTime + t);
    }

    /// <summary>
    /// Subtract operator
    /// </summary>
    /// <returns></returns>
    public static DateTimeUtc operator -(DateTimeUtc d, TimeSpan t)
    {
        return new(d._underlyingDateTime - t);
    }

    /// <summary>
    /// Subtract operator
    /// </summary>
    public static TimeSpan operator -(DateTimeUtc d1, DateTimeUtc d2)
    {
        return d1._underlyingDateTime - d2._underlyingDateTime;
    }

    #endregion Calculation Members

    #region Equality Members

    /// <inheritdoc />
    public bool Equals(DateTimeUtc other)
    {
        return this._underlyingDateTime.Equals(other._underlyingDateTime);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is DateTimeUtc other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._underlyingDateTime.GetHashCode();
    }

    /// <summary>
    /// Equals operator
    /// </summary>
    public static bool operator ==(DateTimeUtc left, DateTimeUtc right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Not equals operator
    /// </summary>
    public static bool operator !=(DateTimeUtc left, DateTimeUtc right)
    {
        return !left.Equals(right);
    }

    #endregion Equality Members

    #region Comparison Members

    /// <inheritdoc />
    public int CompareTo(DateTimeUtc other)
    {
        return this._underlyingDateTime.CompareTo(other._underlyingDateTime);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return 1;
        }

        return obj is DateTimeUtc other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(DateTimeUtc)}");
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    public static bool operator <(DateTimeUtc left, DateTimeUtc right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    public static bool operator >(DateTimeUtc left, DateTimeUtc right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Less than or equal operator
    /// </summary>
    public static bool operator <=(DateTimeUtc left, DateTimeUtc right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Greater than or equal operator
    /// </summary>
    public static bool operator >=(DateTimeUtc left, DateTimeUtc right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion Comparison Members

    #region Conversion to and from other types

    /// <summary>
    /// Converts this instance into a <see cref="DateTime"/>.
    /// </summary>
    /// <seealso cref="ToDateTimeOffset"/>
    /// <seealso cref="ToLocalTime"/>
    [Pure]
    public DateTime ToDateTime()
    {
        return this._underlyingDateTime;
    }

    /// <summary>
    /// Implicit conversion to <see cref="DateTime"/>.
    /// </summary>
    /// <seealso cref="ToDateTime"/>
    public static implicit operator DateTime(DateTimeUtc dateTime)
    {
        return dateTime.ToDateTime();
    }

    /// <summary>
    /// Implicit conversion from <see cref="DateTime"/>.
    /// </summary>
    public static implicit operator DateTimeUtc(DateTime dateTime)
    {
        return new DateTimeUtc(dateTime);
    }

    /// <summary>
    /// Converts this instance into a <see cref="DateTime"/> in the local timezone.
    /// </summary>
    /// <seealso cref="ToDateTime"/>
    [Pure]
    public DateTime ToLocalTime()
    {
        return this._underlyingDateTime.ToLocalTime();
    }

    /// <summary>
    /// Converts this instance into a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <seealso cref="ToDateTime"/>
    /// <seealso cref="ToLocalTime"/>
    [Pure]
    public DateTimeOffset ToDateTimeOffset()
    {
        return new(this._underlyingDateTime);
    }

    /// <summary>
    /// Implicit conversion to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <seealso cref="ToDateTimeOffset"/>
    public static implicit operator DateTimeOffset(DateTimeUtc dateTime)
    {
        return dateTime.ToDateTimeOffset();
    }

    /// <summary>
    /// Implicit conversion from <see cref="DateTimeOffset"/>.
    /// </summary>
    public static implicit operator DateTimeUtc(DateTimeOffset dateTime)
    {
        return new DateTimeUtc(dateTime);
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return this._underlyingDateTime.ToString(format, formatProvider);
    }

    /// <inheritdoc cref="DateTime.ToString(string)"/>
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Same as DateTime.ToString()")]
    [Pure]
    public string ToString(string? format)
    {
        return this._underlyingDateTime.ToString(format);
    }

    /// <inheritdoc />
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Same as DateTime.ToString()")]
    public override string ToString()
    {
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        return this._underlyingDateTime.ToString();
    }

    #endregion Conversion to and from other types
}