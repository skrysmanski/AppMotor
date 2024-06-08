// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.DateAndTime;

/// <summary>
/// Represents data of an ISO-8601 date/time (i.e. date/time strings à la "2024-05-30T20:11:21Z").
/// Use <see cref="TryParse"/> to create instances of this type and then use <see cref="ToDateTime"/>
/// or <see cref="ToDateTimeOffset"/> to convert it into the associated .NET type.
/// </summary>
///
/// <remarks>
/// This type is similar to <see cref="DateTimeOffset"/> but with <see cref="TimeZoneOffset"/> being optional.
/// </remarks>
///
/// <remarks>
/// <para>This type is required because the .NET BCL doesn't contain a proper ISO-8601 implementation.</para>
///
/// <para>On the parsing side, you could use <c>DateTime.TryParse()</c> which accepts ISO-8601 but also
/// other date time formats (so you couldn't easily limit valid input strings to ISO-8601). On the other
/// hand, you could use <c>DateTime.TryParseExact()</c> with either "s" or "o" but they're both very strict.
/// The "s" format doesn't allow for any fractional seconds, while "o" always requires exactly 7 fractional
/// digits - but not, say, 3.</para>
///
/// <para>On the <c>ToString()</c> side, you can use either "s" or "o" - but those either print no fractional
/// seconds ("s") or exactly 7 digits ("o") - but not, say 3, if you only have milliseconds but no microseconds.</para>
///
/// <para>You could fix the "required fractional seconds digits" problem by using "F" instead of "f" with a custom format:
/// <c>yyyy-MM-ddTHH:mm:ss.FFFFFFF</c> However, this format could not handle ISO-8601 strings that contain
/// no fractional seconds (as it would need to end with a "." to be accepted - which not a valid ISO-8601 string).</para>
///
/// <para>There is also the problem that ISO-8601 has a shortened form (e.g. "20240603T104822") which
/// is not accepted by either "o" or "s".</para>
///
/// <para>You could try to use <c>DateTime.TryParseExact()</c> with multiple formats but this gets out of
/// hand pretty quickly (see: https://stackoverflow.com/a/17752389/614177).</para>
///
/// <para>All of this also applies to <see cref="DateTimeOffset"/>.</para>
///
/// <para>This type aims to solve all of these problems by providing a proper ISO-8601 implementation.</para>
/// </remarks>
///
/// <remarks>
/// Unfortunately, the actual ISO 8601 specification is only available behind a paywall (it costs about 190$)
/// so this implementation may not follow ISO 8601 in all of its details. However, the broad strokes are publicly
/// available through Wikipedia and via RFC 3339 (https://datatracker.ietf.org/doc/html/rfc3339).
/// </remarks>
public readonly struct Iso8601Instant : IEquatable<Iso8601Instant>
{
    private static readonly int[] DAYS_IN_MONTH_365 = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    private static readonly int[] DAYS_IN_MONTH_366 = [31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    private static readonly TimeSpan MIN_TIME_ZONE_OFFSET = new(-24, 0, 0);
    private static readonly TimeSpan MAX_TIME_ZONE_OFFSET = new(24, 0, 0);

    /// <summary>
    /// The date and time part of this instant.
    /// </summary>
    /// <remarks>
    /// The kind is always set to <see cref="DateTimeKind.Unspecified"/>, no matter what the <see cref="TimeZoneOffset"/> is.
    /// </remarks>
    public DateTime DateTimePart { get; }

    /// <summary>
    /// The time zone offset of this instant. Can be positive or negative. UTC is represented by <see cref="TimeSpan.Zero"/>.
    /// If no time zone was specified in the ISO-8601 string, this property will be <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Only the properties <see cref="TimeSpan.Hours"/> and <see cref="TimeSpan.Minutes"/> will be set. Especially
    /// <see cref="TimeSpan.Days"/> and <see cref="TimeSpan.Seconds"/> will always be zero.
    /// </remarks>
    /// <remarks>
    /// This implementation only checks that this value is greater than "-24:00" and less than "+24:00". Other than that,
    /// no limitations are applied to the offset. Note that <see cref="DateTimeOffset"/> only accepts offsets that are
    /// between "-14:00" and "+14:00" - so having a larger offset here will result in an exception in <see cref="ToDateTimeOffset"/>.
    /// </remarks>
    public TimeSpan? TimeZoneOffset { get; }

    /// <summary>
    /// Constructor that takes the date/time part and the time zone offset separately.
    /// </summary>
    /// <param name="dateTimePart">The date/time part of this ISO-8601 instant. The <see cref="DateTime.Kind"/>
    /// must be <see cref="DateTimeKind.Unspecified"/>.</param>
    /// <param name="timeZoneOffset">The time zone offset of this ISO-8601 instant. See <see cref="TimeZoneOffset"/>
    /// for more details.</param>
    /// <exception cref="ArgumentException">Thrown if the kind of <paramref name="dateTimePart"/> is not <see cref="DateTimeKind.Unspecified"/>,
    /// or if <paramref name="timeZoneOffset"/> is too small or too large, or if it uses seconds, milliseconds, ...</exception>
    public Iso8601Instant(DateTime dateTimePart, TimeSpan? timeZoneOffset)
    {
        if (dateTimePart.Kind != DateTimeKind.Unspecified)
        {
            throw new ArgumentException($"The date/time part must have 'DateTimeKind.Unspecified' but is '{dateTimePart.Kind}'.", nameof(dateTimePart));
        }
        if (timeZoneOffset is not null)
        {
            if (timeZoneOffset <= MIN_TIME_ZONE_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(timeZoneOffset), "The specified time zone offset is -24:00 or less which is not supported.");
            }
            if (timeZoneOffset >= MAX_TIME_ZONE_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(timeZoneOffset), "The specified time zone offset is +24:00 or greater which is not supported.");
            }
            if (timeZoneOffset.Value.Ticks % TimeSpan.TicksPerMinute != 0)
            {
                throw new ArgumentException("The time zone offset only supports minute precision.", nameof(timeZoneOffset));
            }
        }

        this.DateTimePart = dateTimePart;
        this.TimeZoneOffset = timeZoneOffset;
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTime"/>.
    /// </summary>
    public Iso8601Instant(DateTime source)
    {
        switch (source.Kind)
        {
            case DateTimeKind.Unspecified:
                this.DateTimePart = source;
                this.TimeZoneOffset = null;
                break;

            case DateTimeKind.Utc:
                this.DateTimePart = DateTime.SpecifyKind(source, DateTimeKind.Unspecified);
                this.TimeZoneOffset = TimeSpan.Zero;
                break;

            case DateTimeKind.Local:
                this.DateTimePart = DateTime.SpecifyKind(source, DateTimeKind.Unspecified);
                this.TimeZoneOffset = new DateTimeOffset(source).Offset;
                break;

            default:
                // CODE COVERAGE: can't be reached
                throw new ArgumentException($"Source has unsupported DateTimeKind: {source.Kind}", nameof(source));
        }
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTimeOffset"/>.
    /// </summary>
    public Iso8601Instant(DateTimeOffset source)
    {
        this.DateTimePart = DateTime.SpecifyKind(source.DateTime, DateTimeKind.Unspecified);
        this.TimeZoneOffset = source.Offset;
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTimeUtc"/>.
    /// </summary>
    public Iso8601Instant(DateTimeUtc source)
    {
        this.DateTimePart = DateTime.SpecifyKind(source.ToDateTime(), DateTimeKind.Unspecified);
        this.TimeZoneOffset = TimeSpan.Zero;
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTime"/>.
    /// </summary>
    public static implicit operator Iso8601Instant(DateTime source)
    {
        return new Iso8601Instant(source);
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTimeOffset"/>.
    /// </summary>
    public static implicit operator Iso8601Instant(DateTimeOffset source)
    {
        return new Iso8601Instant(source);
    }

    /// <summary>
    /// Constructs an ISO-8601 instant from a <see cref="DateTimeUtc"/>.
    /// </summary>
    public static implicit operator Iso8601Instant(DateTimeUtc source)
    {
        return new Iso8601Instant(source);
    }

    /// <summary>
    /// Parses the specified string as ISO-8601.
    /// </summary>
    /// <remarks>
    /// For details on the implementation, see <see cref="TryParse"/>.
    /// </remarks>
    /// <exception cref="FormatException">Thrown if the specified string is not a valid/supported ISO-8601 string.</exception>
    [MustUseReturnValue]
    public static Iso8601Instant Parse(ReadOnlySpan<char> iso8601String)
    {
        if (!TryParse(iso8601String, out var result))
        {
            throw new FormatException("The specified value is not a valid/supported ISO-8601 string.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse the specified string as ISO-8601.
    /// </summary>
    ///
    /// <remarks>
    /// <para>This implementation mostly follows RFC 3339's ABNF (https://datatracker.ietf.org/doc/html/rfc3339#appendix-A):</para>
    ///
    /// <para>As such it allows a mixture of basic and extended format (basic format: no <c>-</c> or <c>:</c> separators;
    /// extended format: with <c>-</c> and <c>:</c> separators), it requires <c>T</c> as separator, allows both
    /// <c>T</c> and <c>Z</c> to also be lower-case, and allows both <c>.</c> and <c>,</c> as time fraction separator.</para>
    ///
    /// <para>However, this implementation doesn't support "day of week" or "day of year" syntax, and years must be 4 digits
    /// and can't be negative.</para>
    ///
    /// <para>Due to the limitation of <see cref="DateTime"/>, this implementation doesn't support <c>24</c> as hours nor
    /// leap seconds (i.e. seconds = 60).</para>
    /// </remarks>
    [MustUseReturnValue]
    public static bool TryParse(ReadOnlySpan<char> iso8601String, out Iso8601Instant result)
    {
        //
        // IMPLEMENTERS NOTE: This method could have been a regex but unfortunately, .NET doesn't support allocation-free
        //   regexs with capture groups (only matches without capture groups are supported as allocation-free). So this
        //   implementation is not a regex to be allocation-free (at least as much as possible).
        //

        result = default;

        //
        // Remove leading and trailing white-space.
        //
        iso8601String = TrimWhiteSpace(iso8601String);

        //
        // Year (4 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 4, optionalLeadingChar: null, out var year))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year);
        }

        //
        // Month (2 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 2, optionalLeadingChar: '-', out var month))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month);
        }

        //
        // Day (2 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 2, optionalLeadingChar: '-', out var day))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month, day);
        }

        //
        // Date/time separator ('T')
        //
        if (!TryConsumeDateTimeSeparator(ref iso8601String))
        {
            return false;
        }

        //
        // Hour (2 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 2, optionalLeadingChar: null, out var hour))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month, day, hour);
        }

        switch (iso8601String[0])
        {
            case '+':
            case '-':
            case 'Z':
            case 'z':
                if (!TryConsumeTimeZone(iso8601String, out var timeZoneOffset))
                {
                    return false;
                }

                return TryCreateInstant(out result, year, month, day, hour, timeZoneOffset: timeZoneOffset);
        }

        //
        // Minute (2 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 2, optionalLeadingChar: ':', out var minute))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month, day, hour, minute);
        }

        switch (iso8601String[0])
        {
            case '+':
            case '-':
            case 'Z':
            case 'z':
                if (!TryConsumeTimeZone(iso8601String, out var timeZoneOffset))
                {
                    return false;
                }

                return TryCreateInstant(out result, year, month, day, hour, minute, timeZoneOffset: timeZoneOffset);
        }

        //
        // Second (2 digits)
        //
        if (!TryConsumeFixedWidthInt(ref iso8601String, digitCount: 2, optionalLeadingChar: ':', out var second))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month, day, hour, minute, second);
        }

        switch (iso8601String[0])
        {
            case '+':
            case '-':
            case 'Z':
            case 'z':
                if (!TryConsumeTimeZone(iso8601String, out var timeZoneOffset))
                {
                    return false;
                }

                return TryCreateInstant(out result, year, month, day, hour, minute, second, timeZoneOffset: timeZoneOffset);
        }

        if (!TryConsumeFraction(ref iso8601String, out var fraction))
        {
            return false;
        }

        if (iso8601String.IsEmpty)
        {
            return TryCreateInstant(out result, year, month, day, hour, minute, second, fraction);
        }

        switch (iso8601String[0])
        {
            case '+':
            case '-':
            case 'Z':
            case 'z':
                if (!TryConsumeTimeZone(iso8601String, out var timeZoneOffset))
                {
                    return false;
                }

                return TryCreateInstant(out result, year, month, day, hour, minute, second, fraction, timeZoneOffset: timeZoneOffset);
        }

        return false;

        [MustUseReturnValue]
        static ReadOnlySpan<char> TrimWhiteSpace(ReadOnlySpan<char> span)
        {
            if (span.Length == 0)
            {
                return span;
            }

            int start = 0;
            for (; start < span.Length; start++)
            {
                if (!IsWhiteSpace(span[start]))
                {
                    break;
                }
            }

            int end = span.Length - 1;
            for (; end > start; end--)
            {
                if (!IsWhiteSpace(span[end]))
                {
                    break;
                }
            }

            if (start == 0 && end == span.Length - 1)
            {
                // No whitespace on either end.
                return span;
            }

            return span.Slice(start, end - start + 1);
        }

        // For performance reasons and because of common sense we only treat space and tabs as whitespace.
        [MustUseReturnValue]
        static bool IsWhiteSpace(char ch)
        {
            return ch switch
            {
                ' ' or '\t' => true,
                _ => false,
            };
        }

        [MustUseReturnValue]
        static bool TryConsumeFixedWidthInt(ref ReadOnlySpan<char> @string, int digitCount, char? optionalLeadingChar, out int result)
        {
            result = 0;

            if (optionalLeadingChar is not null && !@string.IsEmpty && @string[0] == optionalLeadingChar.Value)
            {
                @string = @string[1..];
            }

            if (@string.Length < digitCount)
            {
                return false;
            }

            for (int index = 0; index < digitCount; index++)
            {
                var ch = @string[index];
                if (ch == '0' && result == 0) // "result == 0" - this is a leading 0
                {
                    continue;
                }

                if (ch is >= '0' and <= '9')
                {
                    int digitValue = ch - '0';

                    result = result * 10 + digitValue;
                }
                else
                {
                    // Non digit character
                    return false;
                }
            }

            @string = @string[digitCount..];
            return true;
        }

        [MustUseReturnValue]
        static bool TryConsumeDateTimeSeparator(ref ReadOnlySpan<char> @string)
        {
            if (@string[0] is 'T' or 't')
            {
                @string = @string[1..];
                return true;
            }

            return false;
        }

        [MustUseReturnValue]
        static bool TryConsumeFraction(ref ReadOnlySpan<char> @string, out TimeSpan result)
        {
            if (@string.Length > 1 && @string[0] is '.' or ',')
            {
                @string = @string[1..];

                // NOTE: 1 tick = 100 nanoseconds; this is why we support only up to 7 fraction digits
                //   as the 7th digit represents 100ns. (The 8th would be 10ns but this can't be expressed
                //   anymore.)
                ConsumeFraction(ref @string, expectedDigitCount: 7, out var ticks);

                result = TimeSpan.FromTicks(ticks);
                return true;
            }

            result = default;
            return false;
        }

        static void ConsumeFraction(ref ReadOnlySpan<char> @string, int expectedDigitCount, out int result)
        {
            result = 0;

            var maxIndex = Math.Min(expectedDigitCount, @string.Length);

            int readDigitCount = 0;

            for (int index = 0; index < maxIndex; index++)
            {
                var ch = @string[index];
                if (ch == '0' && result == 0) // "result == 0" - this is a leading 0
                {
                    readDigitCount++;
                    continue;
                }

                if (ch is >= '0' and <= '9')
                {
                    int digitValue = ch - '0';

                    result = result * 10 + digitValue;
                    readDigitCount++;
                }
                else
                {
                    // Non digit character
                    break;
                }
            }

            if (readDigitCount == 0)
            {
                return;
            }

            // Expand result to the number of requested digits.
            for (int unmappedDigits = readDigitCount; unmappedDigits < expectedDigitCount; unmappedDigits++)
            {
                result *= 10;
            }

            @string = @string[readDigitCount..];
        }

        [MustUseReturnValue]
        static bool TryConsumeTimeZone(ReadOnlySpan<char> @string, out TimeSpan timeZoneOffset)
        {
            bool negativeOffset;

            switch (@string[0])
            {
                case 'Z':
                case 'z':
                    if (@string.Length != 1)
                    {
                        timeZoneOffset = default;
                        return false;
                    }
                    else
                    {
                        timeZoneOffset = TimeSpan.Zero;
                        return true;
                    }

                case '+':
                    negativeOffset = false;
                    break;
                case '-':
                    negativeOffset = true;
                    break;

                default:
                    // CODE COVERAGE: can't be reached
                    timeZoneOffset = default;
                    return false;
            }

            @string = @string[1..];
            if (!TryConsumeFixedWidthInt(ref @string, digitCount: 2, optionalLeadingChar: null, out var hourOffset))
            {
                timeZoneOffset = default;
                return false;
            }

            if (hourOffset >= 24)
            {
                timeZoneOffset = default;
                return false;
            }

            if (@string.IsEmpty)
            {
                timeZoneOffset = new TimeSpan(hourOffset, 0, 0);
                if (negativeOffset)
                {
                    timeZoneOffset = -timeZoneOffset;
                }
                return true;
            }

            if (@string.Length != 3 || @string[0] != ':')
            {
                timeZoneOffset = default;
                return false;
            }

            @string = @string[1..];
            if (!TryConsumeFixedWidthInt(ref @string, digitCount: 2, optionalLeadingChar: null, out var minuteOffset))
            {
                timeZoneOffset = default;
                return false;
            }

            if (minuteOffset > 59)
            {
                timeZoneOffset = default;
                return false;
            }

            timeZoneOffset = new TimeSpan(hourOffset, minuteOffset, 0);
            if (negativeOffset)
            {
                timeZoneOffset = -timeZoneOffset;
            }
            return true;
        }

        [MustUseReturnValue]
        static bool TryCreateInstant(
                out Iso8601Instant result,
                int year,
                int month = 1,
                int day = 1,
                int hour = 0,
                int minute = 0,
                int second = 0,
                TimeSpan? fraction = null,
                TimeSpan? timeZoneOffset = null
            )
        {
            if (   year is < 0 or > 9999
                || month is < 1 or > 12
                || day is < 1 or > 31
                || hour is < 0 or > 23
                || minute is < 0 or > 59
                || second is < 0 or > 59
                || fraction?.Ticks is < 0 or > 1_000_000_0
                )
            {
                result = default;
                return false;
            }

            var daysInMonth = DateTime.IsLeapYear(year) ? DAYS_IN_MONTH_366 : DAYS_IN_MONTH_365;
            if (day > daysInMonth[month - 1])
            {
                result = default;
                return false;
            }

            var dateTimePart = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            if (fraction is not null)
            {
                dateTimePart += fraction.Value;
            }

            result = new Iso8601Instant(dateTimePart, timeZoneOffset: timeZoneOffset);
            return true;
        }
    }

    /// <summary>
    /// Converts this ISO-8601 instant into a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="assumeKind">If <see cref="TimeZoneOffset"/> is <c>null</c>, this specifies what <see cref="DateTimeKind"/>
    /// should be assumed for <see cref="DateTimePart"/>. Defaults to <see cref="DateTimeKind.Unspecified"/>. Pass, for example,
    /// <see cref="DateTimeKind.Utc"/> if you know that the ISO-8601 is always UTC.</param>
    /// <param name="adjustToUtc">Whether to convert the result to UTC before returning it. Defaults to <c>false</c>. If this is
    /// <c>false</c> and <see cref="TimeZoneOffset"/> is set (but not to <see cref="TimeSpan.Zero"/>), this instant is converted
    /// into local time before returning it (i.e. it will have <see cref="DateTimeKind.Local"/>).</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TimeZoneOffset"/> is <c>null</c>, <paramref name="assumeKind"/>
    /// is <see cref="DateTimeKind.Unspecified"/> and <paramref name="adjustToUtc"/> is <c>true</c> - because in this case
    /// it's unknown how much time to add or subtract from <see cref="DateTimePart"/> to "reach" UTC.</exception>
    [MustUseReturnValue]
    public DateTime ToDateTime(DateTimeKind assumeKind = DateTimeKind.Unspecified, bool adjustToUtc = false)
    {
        if (this.TimeZoneOffset is null)
        {
            if (assumeKind == DateTimeKind.Unspecified && adjustToUtc)
            {
                throw new InvalidOperationException("Can't adjust an instant to UTC when no time zone offset is present and 'assumeKind' is 'Unspecified'.");
            }

            var result = DateTime.SpecifyKind(this.DateTimePart, assumeKind);

            return adjustToUtc ? result.ToUniversalTime() : result;
        }

        if (this.TimeZoneOffset == TimeSpan.Zero)
        {
            return DateTime.SpecifyKind(this.DateTimePart, DateTimeKind.Utc);
        }
        else
        {
            if (adjustToUtc)
            {
                return new DateTimeOffset(this.DateTimePart, this.TimeZoneOffset.Value).UtcDateTime;
            }
            else
            {
                return new DateTimeOffset(this.DateTimePart, this.TimeZoneOffset.Value).LocalDateTime;
            }
        }
    }

    /// <summary>
    /// Converts this ISO-8601 instant into a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="assumeKind">If <see cref="TimeZoneOffset"/> is <c>null</c>, this specifies what <see cref="DateTimeKind"/>
    /// should be assumed for <see cref="DateTimePart"/>. Defaults to <see cref="DateTimeKind.Unspecified"/>. Pass, for example,
    /// <see cref="DateTimeKind.Utc"/> if you know that the ISO-8601 is always UTC.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TimeZoneOffset"/> is <c>null</c> and <paramref name="assumeKind"/>
    /// is <see cref="DateTimeKind.Unspecified"/> (the default).</exception>
    [MustUseReturnValue]
    public DateTimeOffset ToDateTimeOffset(DateTimeKind assumeKind = DateTimeKind.Unspecified)
    {
        if (this.TimeZoneOffset is null)
        {
            switch (assumeKind)
            {
                case DateTimeKind.Utc:
                case DateTimeKind.Local:
                    return DateTime.SpecifyKind(this.DateTimePart, assumeKind);

                default:
                    throw new InvalidOperationException("Can't create an instance of DateTimeOffset without a time zone offset.");
            }
        }

        return new DateTimeOffset(this.DateTimePart, this.TimeZoneOffset.Value);
    }

    /// <summary>
    /// Converts this ISO-8601 instant into a <see cref="DateTimeUtc"/>.
    /// </summary>
    /// <param name="assumeKind">If <see cref="TimeZoneOffset"/> is <c>null</c>, this specifies what <see cref="DateTimeKind"/>
    /// should be assumed for <see cref="DateTimePart"/>. Defaults to <see cref="DateTimeKind.Unspecified"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TimeZoneOffset"/> is <c>null</c> and <paramref name="assumeKind"/>
    /// is <see cref="DateTimeKind.Unspecified"/> (the default).</exception>
    [MustUseReturnValue]
    public DateTimeUtc ToDateTimeUtc(DateTimeKind assumeKind = DateTimeKind.Unspecified)
    {
        if (this.TimeZoneOffset is null)
        {
            switch (assumeKind)
            {
                case DateTimeKind.Utc:
                case DateTimeKind.Local:
                    return DateTime.SpecifyKind(this.DateTimePart, assumeKind);

                default:
                    throw new InvalidOperationException("Can't create an instance of DateTimeUtc without a time zone offset.");
            }
        }

        return new DateTimeOffset(this.DateTimePart, this.TimeZoneOffset.Value);
    }

    /// <inheritdoc />
    public bool Equals(Iso8601Instant other)
    {
        return this.DateTimePart == other.DateTimePart && this.TimeZoneOffset == other.TimeZoneOffset;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Iso8601Instant other && Equals(other);
    }

    /// <summary>
    /// <c>==</c> operator.
    /// </summary>
    public static bool operator ==(Iso8601Instant left, Iso8601Instant right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// <c>!=</c> operator.
    /// </summary>
    public static bool operator !=(Iso8601Instant left, Iso8601Instant right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.DateTimePart, this.TimeZoneOffset);
    }

    /// <summary>
    /// Prints this instant as ISO-8601. Note that seconds are always included in the returned value
    /// but second fractions (milliseconds, microseconds, nanoseconds) will only be included if not
    /// zero (and trailing zeros will be removed).
    /// </summary>
    public override string ToString()
    {
        var resultBuilder = new StringBuilder(capacity: "yyyy-MM-ddTHH:mm:ss.fffffff+XX:YY".Length);

        if (this.DateTimePart.Nanosecond != 0)
        {
            // 7 fractional digits
            resultBuilder.AppendFormatIC("{0:o}", this.DateTimePart);
        }
        else if (this.DateTimePart.Microsecond != 0 || this.DateTimePart.Millisecond != 0)
        {
            // 1-6 fractional digits
            resultBuilder.AppendFormatIC("{0:yyyy-MM-ddTHH:mm:ss.FFFFFF}", this.DateTimePart);
        }
        else
        {
            resultBuilder.AppendFormatIC("{0:yyyy-MM-ddTHH:mm:ss}", this.DateTimePart);
        }

        if (this.TimeZoneOffset is null)
        {
            return resultBuilder.ToString();
        }

        var timeZoneOffset = this.TimeZoneOffset.Value;

        if (timeZoneOffset == TimeSpan.Zero)
        {
            return resultBuilder.Append('Z').ToString();
        }
        else
        {
            if (timeZoneOffset > TimeSpan.Zero)
            {
                resultBuilder.AppendFormatIC("+{0:D2}:{1:D2}", timeZoneOffset.Hours, timeZoneOffset.Minutes);
            }
            else
            {
                resultBuilder.AppendFormatIC("{0:D2}:{1:D2}", timeZoneOffset.Hours, -timeZoneOffset.Minutes);
            }

            return resultBuilder.ToString();
        }
    }
}
