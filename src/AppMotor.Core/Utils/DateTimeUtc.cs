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
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
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
        // TODO #24: Switch type to "DateOnly" with .NET 6
        public DateTime Date => this._underlyingDateTime.Date;

        /// <summary>
        /// The time part of this date time.
        /// </summary>
        // TODO #24: Switch type to "TimeOnly" with .NET 6
        public TimeSpan TimeOfDay => this._underlyingDateTime.TimeOfDay;

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
        public DateTimeUtc(int year, int month, int day, int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, 0)
        {
        }

        /// <summary>
        /// Constructs a <see cref="DateTimeUtc"/> instance for the specified date and time.
        /// </summary>
        public DateTimeUtc(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc))
        {
        }

        #endregion Constructors

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

        /// <summary>
        /// Converts this instance into a <see cref="DateTime"/>.
        /// </summary>
        [Pure]
        public DateTime ToDateTime()
        {
            return this._underlyingDateTime;
        }

        /// <summary>
        /// Converts this instance into a <see cref="DateTimeOffset"/>.
        /// </summary>
        [Pure]
        public DateTimeOffset ToDateTimeOffset()
        {
            return new(this._underlyingDateTime);
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
    }
}
