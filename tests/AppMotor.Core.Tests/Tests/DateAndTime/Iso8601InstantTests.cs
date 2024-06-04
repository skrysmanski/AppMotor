// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.Core.DateAndTime;
using AppMotor.Core.Extensions;
using AppMotor.TestCore;
using AppMotor.TestCore.TestData;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.Core.Tests.DateAndTime;

/// <summary>
/// Tests for <see cref="Iso8601Instant"/>.
/// </summary>
public sealed class Iso8601InstantTests
{
    private static readonly TimeSpan s_localTimeZoneOffset = new DateTimeOffset(DateTime.Now).Offset;

    private readonly ITestOutputHelper _testOutputHelper;

    public Iso8601InstantTests(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test_Constructor_Explicit_DateTimeKind()
    {
        Should.Throw<ArgumentException>(() => new Iso8601Instant(DateTime.UtcNow, timeZoneOffset: null));
        Should.Throw<ArgumentException>(() => new Iso8601Instant(DateTime.Now, timeZoneOffset: null));
        Should.NotThrow(() => new Iso8601Instant(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), timeZoneOffset: null));
    }

    [Fact]
    public void Test_Constructor_Explicit_InvalidTimeZoneOffset()
    {
        // Setup
        var dateTimePart = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        Should.NotThrow(() => new Iso8601Instant(dateTimePart, timeZoneOffset: TimeSpan.Zero)); // Verify assumption that "dateTimePart" is valid

        // Test
        Should.Throw<ArgumentOutOfRangeException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: new TimeSpan(24, 0, 0)));
        Should.Throw<ArgumentOutOfRangeException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: -new TimeSpan(24, 0, 0)));
        Should.Throw<ArgumentOutOfRangeException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: new TimeSpan(1, 0, 0, 0)));
        Should.Throw<ArgumentOutOfRangeException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: -new TimeSpan(1, 0, 0, 0)));
        Should.Throw<ArgumentException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: new TimeSpan(2, 0, 30)));
        Should.Throw<ArgumentException>(() => new Iso8601Instant(dateTimePart, timeZoneOffset: -new TimeSpan(2, 0, 30)));
    }

    [Fact]
    public void Test_Constructor_DateTime()
    {
        // Setup
        var baseDate = new DateTime(2024, 6, 3, 11, 22, 33);

        // Test 1: source is Unspecified
        var instant1 = new Iso8601Instant(DateTime.SpecifyKind(baseDate, DateTimeKind.Unspecified));
        instant1.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant1.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant1.TimeZoneOffset.ShouldBeNull();

        // Test 2: source is UTC
        var instant2 = new Iso8601Instant(DateTime.SpecifyKind(baseDate, DateTimeKind.Utc));
        instant2.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant2.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant2.TimeZoneOffset.ShouldBe(TimeSpan.Zero);

        // Test 3: source is Local
        var instant3 = new Iso8601Instant(DateTime.SpecifyKind(baseDate, DateTimeKind.Local));
        instant3.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant3.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant3.TimeZoneOffset.ShouldBe(new DateTimeOffset(DateTime.SpecifyKind(baseDate, DateTimeKind.Local)).Offset);
    }

    [Fact]
    public void Test_Constructor_DateTimeOffset()
    {
        // Test 1: source has non-UTC time zone
        var dateTimeOffsetWithTimeZone = new DateTimeOffset(new DateTime(2024, 6, 3, 11, 22, 33), new TimeSpan(2, 30, 0));
        var instant1 = new Iso8601Instant(dateTimeOffsetWithTimeZone);
        instant1.DateTimePart.Ticks.ShouldBe(dateTimeOffsetWithTimeZone.DateTime.Ticks);
        instant1.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant1.TimeZoneOffset.ShouldBe(dateTimeOffsetWithTimeZone.Offset);

        // Test 2: source has non-UTC time zone
        var dateTimeOffsetUtcTimeZone = new DateTimeOffset(new DateTime(2024, 6, 3, 11, 22, 33), TimeSpan.Zero);
        var instant2 = new Iso8601Instant(dateTimeOffsetUtcTimeZone);
        instant2.DateTimePart.Ticks.ShouldBe(dateTimeOffsetUtcTimeZone.DateTime.Ticks);
        instant2.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant2.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Test_Constructor_DateTimeUtc()
    {
        var now = DateTimeUtc.Now;

        var instant = new Iso8601Instant(now);

        instant.DateTimePart.Ticks.ShouldBe(now.Ticks);
        instant.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Test_ImplicitOperator_FromDateTime()
    {
        // Setup
        var baseDate = new DateTime(2024, 6, 3, 11, 22, 33);

        // Test 1: source is Unspecified
        Iso8601Instant instant1 = DateTime.SpecifyKind(baseDate, DateTimeKind.Unspecified);
        instant1.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant1.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant1.TimeZoneOffset.ShouldBeNull();

        // Test 2: source is UTC
        Iso8601Instant instant2 = DateTime.SpecifyKind(baseDate, DateTimeKind.Utc);
        instant2.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant2.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant2.TimeZoneOffset.ShouldBe(TimeSpan.Zero);

        // Test 3: source is Local
        Iso8601Instant instant3 = DateTime.SpecifyKind(baseDate, DateTimeKind.Local);
        instant3.DateTimePart.Ticks.ShouldBe(baseDate.Ticks);
        instant3.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant3.TimeZoneOffset.ShouldBe(new DateTimeOffset(DateTime.SpecifyKind(baseDate, DateTimeKind.Local)).Offset);
    }

    [Fact]
    public void Test_ImplicitOperator_FromDateTimeOffset()
    {
        // Test 1: source has non-UTC time zone
        var dateTimeOffsetWithTimeZone = new DateTimeOffset(new DateTime(2024, 6, 3, 11, 22, 33), new TimeSpan(2, 30, 0));
        Iso8601Instant instant1 = dateTimeOffsetWithTimeZone;
        instant1.DateTimePart.Ticks.ShouldBe(dateTimeOffsetWithTimeZone.DateTime.Ticks);
        instant1.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant1.TimeZoneOffset.ShouldBe(dateTimeOffsetWithTimeZone.Offset);

        // Test 2: source has non-UTC time zone
        var dateTimeOffsetUtcTimeZone = new DateTimeOffset(new DateTime(2024, 6, 3, 11, 22, 33), TimeSpan.Zero);
        Iso8601Instant instant2 = dateTimeOffsetUtcTimeZone;
        instant2.DateTimePart.Ticks.ShouldBe(dateTimeOffsetUtcTimeZone.DateTime.Ticks);
        instant2.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant2.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Test_ImplicitOperator_FromDateTimeUtc()
    {
        var now = DateTimeUtc.Now;

        Iso8601Instant instant = now;

        instant.DateTimePart.Ticks.ShouldBe(now.Ticks);
        instant.DateTimePart.Kind.ShouldBe(DateTimeKind.Unspecified);
        instant.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    /// Tests parsing various strings. Each string is "iterated" by creating substrings from length 1 to its full length.
    /// Each substring is parsed and whether the parsing should be correct is determined from <paramref name="validLengths"/>
    /// (which contains each length that should result in a successful parsing). If successful, the result is compared against
    /// an element from <paramref name="expectedInstants"/> (has the same item count as <paramref name="validLengths"/>).
    /// </summary>
    [Theory]
    [ClassData(typeof(TryParseTestData))]
    public void Test_TryParse(string input, int[] validLengths, Iso8601Instant[] expectedInstants)
    {
        int nextValidLengthIndex = 0;

        for (var length = 1; length <= input.Length; length++)
        {
            // Setup
            var nextValidLength = validLengths[nextValidLengthIndex];
            bool shouldBeValid = length == nextValidLength;
            var partialInput = input[0..length];

            // Test
            Iso8601Instant.TryParse(partialInput, out var parsedInstant).ShouldBe(shouldBeValid, $"Input: '{partialInput}' (length: {length})");

            // Verify
            if (shouldBeValid)
            {
                parsedInstant.ShouldBe(expectedInstants[nextValidLengthIndex]);
            }
            else
            {
                parsedInstant.ShouldBe(default);
            }

            // After setup
            if (length == nextValidLength && nextValidLengthIndex + 1 < validLengths.Length)
            {
                nextValidLengthIndex++;
            }
        }
    }

    /// <summary>
    /// Provides the test data for <see cref="Test_TryParse"/>.
    /// </summary>
    private sealed class TryParseTestData : TestDataBase
    {
        /// <inheritdoc />
        public override IEnumerator<object[]> GetEnumerator()
        {
            yield return [
                "2004-03-11T11:22:33.1234567+05:30",
                new[] { 4, 7, 10, 13, 16, 19, 21, 22, 23, 24, 25, 26, 27, 30, 33 },
                new Iso8601Instant[]
                {
                    new DateTime(2004, 1, 1),
                    new DateTime(2004, 3, 1),
                    new DateTime(2004, 3, 11),
                    new DateTime(2004, 3, 11, 11, 0, 0),
                    new DateTime(2004, 3, 11, 11, 22, 0),
                    new DateTime(2004, 3, 11, 11, 22, 33),
                    new DateTime(2004, 3, 11, 11, 22, 33, 100),
                    new DateTime(2004, 3, 11, 11, 22, 33, 120),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4000),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4500),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4560),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567),
                    new (new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567), new TimeSpan(5, 0, 0)),
                    new (new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567), new TimeSpan(5, 30, 0)),
                },
            ];
            yield return [
                "20040311T112233.1234567-05:30",
                new[] { 4, 6, 8, 11, 13, 15, 17, 18, 19, 20, 21, 22, 23, 26, 29 },
                new Iso8601Instant[]
                {
                    new DateTime(2004, 1, 1),
                    new DateTime(2004, 3, 1),
                    new DateTime(2004, 3, 11),
                    new DateTime(2004, 3, 11, 11, 0, 0),
                    new DateTime(2004, 3, 11, 11, 22, 0),
                    new DateTime(2004, 3, 11, 11, 22, 33),
                    new DateTime(2004, 3, 11, 11, 22, 33, 100),
                    new DateTime(2004, 3, 11, 11, 22, 33, 120),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4000),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4500),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4560),
                    new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567),
                    new (new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567), -new TimeSpan(5, 0, 0)),
                    new (new DateTime(2004, 3, 11, 11, 22, 33, 123) + TimeSpan.FromTicks(4567), -new TimeSpan(5, 30, 0)),
                },
            ];
            yield return [
                "2004-0311T11:2233Z",
                new[] { 4, 7, 9, 12, 15, 17, 18 },
                new Iso8601Instant[]
                {
                    new DateTime(2004, 1, 1),
                    new DateTime(2004, 3, 1),
                    new DateTime(2004, 3, 11),
                    new DateTime(2004, 3, 11, 11, 0, 0),
                    new DateTime(2004, 3, 11, 11, 22, 0),
                    new DateTime(2004, 3, 11, 11, 22, 33),
                    new (new DateTime(2004, 3, 11, 11, 22, 33), TimeSpan.Zero),
                    new (new DateTime(2004, 3, 11, 11, 22, 33), TimeSpan.Zero),
                },
            ];
            yield return [
                "200403-11T1122:33Z",
                new[] { 4, 6, 9, 12, 14, 17, 18 },
                new Iso8601Instant[]
                {
                    new DateTime(2004, 1, 1),
                    new DateTime(2004, 3, 1),
                    new DateTime(2004, 3, 11),
                    new DateTime(2004, 3, 11, 11, 0, 0),
                    new DateTime(2004, 3, 11, 11, 22, 0),
                    new DateTime(2004, 3, 11, 11, 22, 33),
                    new (new DateTime(2004, 3, 11, 11, 22, 33), TimeSpan.Zero),
                    new (new DateTime(2004, 3, 11, 11, 22, 33), TimeSpan.Zero),
                },
            ];
        }
    }

    [Theory]
    [InlineData(" \t2024-06-03T11:22:33Z")]
    [InlineData("2024-06-03T11:22:33Z \t")]
    [InlineData("\t 2024-06-03T11:22:33Z\t ")]
    public void Test_TryParse_WhiteSpace(string input)
    {
        Iso8601Instant.TryParse(input, out var result).ShouldBe(true);

        result.DateTimePart.ShouldBe(new DateTime(2024, 6, 3, 11, 22, 33));
        result.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    [Theory]
    [InlineData("01")]
    [InlineData("002")]
    [InlineData("0003")]
    [InlineData("00004")]
    [InlineData("000005")]
    [InlineData("0000006")]
    public void Test_TryParse_FractionWithLeadingZeros(string fractionPart)
    {
        Iso8601Instant.TryParse($"2024-06-03T11:22:33.{fractionPart}Z", out var result).ShouldBe(true);

        result.ToString().ShouldBe($"2024-06-03T11:22:33.{fractionPart}Z");
    }

    /// <summary>
    /// Tests invalid formats (that are not already covered by <see cref="Test_TryParse"/>).
    /// </summary>
    [Theory]
    [InlineData("6/15/2009 1:45:30 PM")] // this is why we can't use "DateTime.Parse()" as this is accepted by it.
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" \t")]
    [InlineData("2004T")]
    [InlineData("2004T11:22:33")] // missing month and day
    [InlineData("2004-03T")]
    [InlineData("2004-03T11:22:33")] // missing day
    [InlineData("2004-03-02T11.")]
    [InlineData("2004-03-02T11+")]
    [InlineData("2004-03-02T11-")]
    [InlineData("2004-03-02T1A")]
    [InlineData("2004-03-02T11:22.")]
    [InlineData("2004-03-02T11:22+")]
    [InlineData("2004-03-02T11:22-")]
    [InlineData("2004-03-02T11:2A")]
    [InlineData("2004-03-02T11:22:33.")]
    [InlineData("2004-03-02T11:22:33+")]
    [InlineData("2004-03-02T11:22:33-")]
    [InlineData("2004-03-02T11:22:33ZZ")]
    [InlineData("2004-03-02T11:22:33A")]
    [InlineData("2004-03-11T11:22:33.12345678Z")] // more than 7 fraction digits
    [InlineData("2004-03-11T11:22:33.abcZ")]
    [InlineData("2004-03-11T11:22:33.1abcZ")]
    [InlineData("2004-03-11T11:22:33.0abcZ")]
    [InlineData("2004-03-02T11:22:33+1A")]
    [InlineData("2004-03-02T11:22:33+A1")]
    [InlineData("2004-03-02T11:22:33+02:3B")]
    [InlineData("2004-03-02T11:22:33-02:B3")]
    public void Test_TryParse_Invalid_Format(string input)
    {
        Iso8601Instant.TryParse(input, out var result).ShouldBe(false);
        result.ShouldBe(default);
    }

    /// <summary>
    /// Tests the behavior when various components are out-of-bound (e.g. day of month is 32, hour is 24, ...).
    /// </summary>
    /// <seealso cref="Test_TryParse_DaysOfMonth"/>
    [Theory]
    [InlineData("2004-03-02T24:00:00")]
    [InlineData("2004-03-02T11:59:60")]
    [InlineData("2004-00-02T11:22:33")]
    [InlineData("2004-13-02T11:22:33")]
    [InlineData("2004-03-00T11:22:33")]
    [InlineData("2004-03-32T11:22:33")]
    [InlineData("2004-03-02T25:22:33")]
    [InlineData("2004-03-02T11:60:33")]
    [InlineData("2004-03-02T11:22:61")]
    [InlineData("2004-03-02T11:22:33+24:00")]
    [InlineData("2004-03-02T11:22:33-24:00")]
    [InlineData("2004-03-02T11:22:33+02:60")]
    [InlineData("2004-03-02T11:22:33-02:60")]
    public void Test_TryParse_Invalid_NumberRanges(string input)
    {
        Iso8601Instant.TryParse(input, out var result).ShouldBe(false);
        result.ShouldBe(default);
    }

    [Theory]
    [InlineData(2024, 01, 31)] // January
    [InlineData(2023, 02, 28)] // February (non-leap year)
    [InlineData(2024, 02, 29)] // February (leap year)
    [InlineData(2024, 03, 31)] // March
    [InlineData(2024, 04, 30)] // April
    [InlineData(2024, 05, 31)] // May
    [InlineData(2024, 06, 30)] // June
    [InlineData(2024, 07, 31)] // July
    [InlineData(2024, 08, 31)] // August
    [InlineData(2024, 09, 30)] // September
    [InlineData(2024, 10, 31)] // October
    [InlineData(2024, 11, 30)] // November
    [InlineData(2024, 12, 31)] // December
    public void Test_TryParse_DaysOfMonth(int year, int month, int lastDay)
    {
        // Test last day
        var lastDayIsoString = $"{year:D4}-{month:D2}-{lastDay:D2}T12:00:00";
        this._testOutputHelper.WriteLine($"Last day ISO string: {lastDayIsoString}");

        Iso8601Instant.TryParse(lastDayIsoString, out var lastDayResult).ShouldBe(true);
        lastDayResult.DateTimePart.Year.ShouldBe(year);
        lastDayResult.DateTimePart.Month.ShouldBe(month);
        lastDayResult.DateTimePart.Day.ShouldBe(lastDay);

        // Test day after last day
        var postLastDayIsoString = $"{year:D4}-{month:D2}-{lastDay + 1:D2}T12:00:00";
        this._testOutputHelper.WriteLine($"Post last day ISO string: {lastDayIsoString}");

        Iso8601Instant.TryParse(postLastDayIsoString, out var invalidResult).ShouldBe(false);
        invalidResult.ShouldBe(default);
    }

    [Theory]
    [InlineData("2014-06-03T11:22:33+00:19",    "2014-06-03T11:22:33.0000000", "00:19:00")] // no fractional seconds
    [InlineData("2014-06-03T11:22:33-23:00",    "2014-06-03T11:22:33.0000000", "-23:00:00")]
    [InlineData("2014-06-03T11:22:33Z",         "2014-06-03T11:22:33.0000000", "00:00:00")]
    [InlineData("2014-06-03T11:22:33z",         "2014-06-03T11:22:33.0000000", "00:00:00")]
    [InlineData("2014-06-03T11:22:33.1+00:19",  "2014-06-03T11:22:33.1000000", "00:19:00")] // with fractional seconds
    [InlineData("2014-06-03T11:22:33.12-23:00", "2014-06-03T11:22:33.1200000", "-23:00:00")]
    [InlineData("2014-06-03T11:22:33.123Z",     "2014-06-03T11:22:33.1230000", "00:00:00")]
    [InlineData("2014-06-03T11:22:33.1234z",    "2014-06-03T11:22:33.1234000", "00:00:00")]
    [InlineData("2014-06-03T11:22+02:30",   "2014-06-03T11:22:00.0000000", "02:30:00")] // no seconds
    [InlineData("2014-06-03T11:22-02:30",   "2014-06-03T11:22:00.0000000", "-02:30:00")]
    [InlineData("2014-06-03T11:22Z",        "2014-06-03T11:22:00.0000000", "00:00:00")]
    [InlineData("2014-06-03T11:22z",        "2014-06-03T11:22:00.0000000", "00:00:00")]
    [InlineData("2014-06-03T11+02:30",  "2014-06-03T11:00:00.0000000", "02:30:00")] // no minutes
    [InlineData("2014-06-03T11-02:30",  "2014-06-03T11:00:00.0000000", "-02:30:00")]
    [InlineData("2014-06-03T11Z",       "2014-06-03T11:00:00.0000000", "00:00:00")]
    [InlineData("2014-06-03T11z",       "2014-06-03T11:00:00.0000000", "00:00:00")]
    [InlineData("2014-06-03T11:22:33+23:59", "2014-06-03T11:22:33.0000000", "23:59:00")] // upper and lower limit
    [InlineData("2014-06-03T11:22:33-23:59", "2014-06-03T11:22:33.0000000", "-23:59:00")]
    public void Test_TryParse_TimeZone(string input, string expectedDateTimePart, string expectedTimeZoneOffset)
    {
        // Test
        Iso8601Instant.TryParse(input, out var iso8601Instant).ShouldBe(true);

        // Verify
        iso8601Instant.DateTimePart.ToStringIC("o").ShouldBe(expectedDateTimePart);
        iso8601Instant.TimeZoneOffset.ShouldNotBeNull();
        iso8601Instant.TimeZoneOffset.Value.ToStringIC("c").ShouldBe(expectedTimeZoneOffset);
    }

    [Theory]
    [InlineData("2014-06-03T11:22:33Z")]
    [InlineData("2014-06-03T11:22:33+00")]
    [InlineData("2014-06-03T11:22:33+00:00")]
    [InlineData("2014-06-03T11:22:33-00")]
    [InlineData("2014-06-03T11:22:33-00:00")]
    public void Test_TryParse_TimeZone_Utc(string input)
    {
        Iso8601Instant.TryParse(input, out var utcInstant).ShouldBe(true);

        utcInstant.DateTimePart.ToStringIC("o").ShouldBe("2014-06-03T11:22:33.0000000");
        utcInstant.TimeZoneOffset.ShouldBe(TimeSpan.Zero);
    }

    [Theory]
    [InlineData("2004Z")]
    [InlineData("2004+05")]
    [InlineData("2004+05:30")]
    [InlineData("2004-05:30")]
    [InlineData("2004-03Z")]
    [InlineData("2004-03+05")]
    [InlineData("2004-03+05:30")]
    [InlineData("2004-03-05:30")]
    [InlineData("2004-03-12Z")]
    [InlineData("2004-03-12+05")]
    [InlineData("2004-03-12+05:30")]
    [InlineData("2004-03-12-05:30")]
    [InlineData("2004-03-12TZ")]
    [InlineData("2004-03-12T+05")]
    [InlineData("2004-03-12T+05:30")]
    [InlineData("2004-03-12T-05")]
    [InlineData("2004-03-12T-05:30")]
    public void Test_TryParse_Invalid_TimeZoneWithoutTimeComponent(string input)
    {
        Iso8601Instant.TryParse(input, out var result).ShouldBe(false);
        result.ShouldBe(default);
    }

    [Theory]
    [ClassData(typeof(ToDateTimeTestData))]
    public void Test_ToDateTime(string timeZone, DateTimeKind assumeKind, bool adjustToUtc, TimeSpan? expectedTime, DateTimeKind expectedKind)
    {
        // Setup
        Iso8601Instant.TryParse($"2024-06-03T11:22:33.1234{timeZone}", out var instant).ShouldBe(true);

        // Test
        if (expectedTime is not null)
        {
            var result = instant.ToDateTime(assumeKind, adjustToUtc: adjustToUtc);

            // Verify
            result.TimeOfDay.ShouldBe(expectedTime.Value);
            result.Kind.ShouldBe(expectedKind);
        }
        else
        {
            Should.Throw<InvalidOperationException>(() => instant.ToDateTime(assumeKind, adjustToUtc: adjustToUtc));
        }
    }

    private sealed class ToDateTimeTestData : TestDataBase
    {
        /// <inheritdoc />
        public override IEnumerator<object?[]> GetEnumerator()
        {
            var dateTimePart = new TimeSpan(0, 11, 22, 33, 123, 400);
            yield return ["", DateTimeKind.Unspecified, false, dateTimePart, DateTimeKind.Unspecified];
            yield return ["", DateTimeKind.Utc,         false, dateTimePart, DateTimeKind.Utc];
            yield return ["", DateTimeKind.Local,       false, dateTimePart, DateTimeKind.Local];

            yield return ["Z", DateTimeKind.Unspecified, false, dateTimePart, DateTimeKind.Utc];
            yield return ["Z", DateTimeKind.Utc,         false, dateTimePart, DateTimeKind.Utc];
            yield return ["Z", DateTimeKind.Local,       false, dateTimePart, DateTimeKind.Utc];

            yield return ["+02:30", DateTimeKind.Unspecified, false, dateTimePart - new TimeSpan(2, 30, 0) + s_localTimeZoneOffset, DateTimeKind.Local];
            yield return ["+02:30", DateTimeKind.Utc,         false, dateTimePart - new TimeSpan(2, 30, 0) + s_localTimeZoneOffset, DateTimeKind.Local];
            yield return ["+02:30", DateTimeKind.Local,       false, dateTimePart - new TimeSpan(2, 30, 0) + s_localTimeZoneOffset, DateTimeKind.Local];

            yield return ["", DateTimeKind.Unspecified, true, null,         DateTimeKind.Utc];
            yield return ["", DateTimeKind.Utc,         true, dateTimePart, DateTimeKind.Utc];
            yield return ["", DateTimeKind.Local,       true, dateTimePart - s_localTimeZoneOffset, DateTimeKind.Utc];

            yield return ["Z", DateTimeKind.Unspecified, true, dateTimePart, DateTimeKind.Utc];
            yield return ["Z", DateTimeKind.Utc,         true, dateTimePart, DateTimeKind.Utc];
            yield return ["Z", DateTimeKind.Local,       true, dateTimePart, DateTimeKind.Utc];

            yield return ["+02:30", DateTimeKind.Unspecified, true, dateTimePart - new TimeSpan(2, 30, 0), DateTimeKind.Utc];
            yield return ["+02:30", DateTimeKind.Utc,         true, dateTimePart - new TimeSpan(2, 30, 0), DateTimeKind.Utc];
            yield return ["+02:30", DateTimeKind.Local,       true, dateTimePart - new TimeSpan(2, 30, 0), DateTimeKind.Utc];
        }
    }

    [Fact]
    public void Test_ToDateTimeOffset()
    {
        var baseDate = new DateTime(2024, 6, 4, 11, 22, 33, 123);

        var result1 = new Iso8601Instant(baseDate, new TimeSpan(2, 30, 0)).ToDateTimeOffset();
        result1.ToStringIC("o").ShouldBe("2024-06-04T11:22:33.1230000+02:30");

        var result2 = new Iso8601Instant(baseDate, -new TimeSpan(5, 45, 0)).ToDateTimeOffset();
        result2.ToStringIC("o").ShouldBe("2024-06-04T11:22:33.1230000-05:45");

        var result3 = new Iso8601Instant(baseDate, TimeSpan.Zero).ToDateTimeOffset();
        result3.ToStringIC("o").ShouldBe("2024-06-04T11:22:33.1230000+00:00");

        var instantWithoutTimeZoneOffset = new Iso8601Instant(baseDate, timeZoneOffset: null);

        var result4 = instantWithoutTimeZoneOffset.ToDateTimeOffset(DateTimeKind.Utc);
        result4.ToStringIC("o").ShouldBe("2024-06-04T11:22:33.1230000+00:00");

        var result5 = instantWithoutTimeZoneOffset.ToDateTimeOffset(DateTimeKind.Local);
        result5.DateTime.ToStringIC("o").ShouldBe("2024-06-04T11:22:33.1230000");
        result5.Offset.ShouldBe(s_localTimeZoneOffset);

        Should.Throw<InvalidOperationException>(() => instantWithoutTimeZoneOffset.ToDateTimeOffset(DateTimeKind.Unspecified));
        Should.Throw<InvalidOperationException>(() => instantWithoutTimeZoneOffset.ToDateTimeOffset());
    }

    [Fact]
    public void Test_ToDateTimeUtc()
    {
        var baseDate = new DateTime(2024, 6, 4, 11, 22, 33, 123);
        var baseDateAsDateTimeUtc = new DateTimeUtc(DateTime.SpecifyKind(baseDate, DateTimeKind.Utc));

        var result1 = new Iso8601Instant(baseDate, new TimeSpan(2, 30, 0)).ToDateTimeUtc();
        result1.ShouldBe(baseDateAsDateTimeUtc - new TimeSpan(2, 30, 0));

        var result2 = new Iso8601Instant(baseDate, -new TimeSpan(5, 45, 0)).ToDateTimeUtc();
        result2.ShouldBe(baseDateAsDateTimeUtc + new TimeSpan(5, 45, 0));

        var result3 = new Iso8601Instant(baseDate, TimeSpan.Zero).ToDateTimeUtc();
        result3.ShouldBe(baseDateAsDateTimeUtc);

        var instantWithoutTimeZoneOffset = new Iso8601Instant(baseDate, timeZoneOffset: null);

        var result4 = instantWithoutTimeZoneOffset.ToDateTimeUtc(DateTimeKind.Utc);
        result4.ShouldBe(baseDateAsDateTimeUtc);

        var result5 = instantWithoutTimeZoneOffset.ToDateTimeUtc(DateTimeKind.Local);
        result5.ShouldBe(baseDateAsDateTimeUtc - s_localTimeZoneOffset);

        Should.Throw<InvalidOperationException>(() => instantWithoutTimeZoneOffset.ToDateTimeUtc(DateTimeKind.Unspecified));
        Should.Throw<InvalidOperationException>(() => instantWithoutTimeZoneOffset.ToDateTimeUtc());
    }

    [Fact]
    public void Test_Equals()
    {
        // Setup
        var utcNow = DateTime.UtcNow;

        var utcInstant = new Iso8601Instant(utcNow);
        var localInstant = new Iso8601Instant(utcNow.ToLocalTime());
        var instantWithoutTimezone = new Iso8601Instant(DateTime.SpecifyKind(utcNow, DateTimeKind.Unspecified));

        // Test
        if (utcNow.ToLocalTime() != utcNow)
        {
            // This only works if the local time zone isn't UTC.
            EqualityMembersTests.TestEquals(utcInstant, differentItem: localInstant);
        }
        EqualityMembersTests.TestEquals(utcInstant, differentItem: instantWithoutTimezone);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        EqualityMembersTests.TestGetHashCode(new Iso8601Instant(DateTime.Now));
        EqualityMembersTests.TestGetHashCode(new Iso8601Instant(DateTime.UtcNow));
        EqualityMembersTests.TestGetHashCode(new Iso8601Instant(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)));
    }

    [Theory]
    [InlineData("2004-03-11T11:22:33")]
    [InlineData("2004-03-11T11:22:33.1")]
    [InlineData("2004-03-11T11:22:33.12")]
    [InlineData("2004-03-11T11:22:33.123")]
    [InlineData("2004-03-11T11:22:33.1234")]
    [InlineData("2004-03-11T11:22:33.12345")]
    [InlineData("2004-03-11T11:22:33.123456")]
    [InlineData("2004-03-11T11:22:33.1234567")]
    [InlineData("2004-03-11T11:22:33Z")]
    [InlineData("2004-03-11T11:22:33.1234Z")]
    [InlineData("2004-03-11T11:22:33+05:30")]
    [InlineData("2004-03-11T11:22:33.1234-13:30")]
    public void Test_ToString(string input)
    {
        // Setup
        Iso8601Instant.TryParse(input, out var parsedInput).ShouldBe(true);

        // Test
        parsedInput.ToString().ShouldBe(input);
    }

    /// <summary>
    /// Shows how the regular .NET APIs don't handle ISO-8601 properly (both converting from and to string) - for <see cref="DateTime"/>.
    /// </summary>
    [Fact]
    public void Test_Justification_DateTime()
    {
        // See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

        //
        // TryParseExact() can't handle anything but 7 fraction digits. If you just provide 3, the string can't be parsed.
        // NOTE: We could use "TryParse()" but this would also accept formats other than ISO-8601.
        //

        const string PARSE_INPUT = "2004-03-11T11:22:33.123Z";
        // Round-trip format
        DateTime.TryParseExact(PARSE_INPUT, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTime.TryParseExact(PARSE_INPUT, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);
        DateTime.TryParseExact(PARSE_INPUT, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTime.TryParseExact(PARSE_INPUT, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);

        // Sortable
        DateTime.TryParseExact(PARSE_INPUT, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTime.TryParseExact(PARSE_INPUT, "s", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);

        // "Counter evidence" with "Iso8601Instant" API
        Iso8601Instant.TryParse(PARSE_INPUT, out _).ShouldBe(true);

        //
        // ToString() doesn't print the right number of fraction digits.
        //

        // Prints 7 fraction digits even though there are no fractions.
        var toStringInput1 = new DateTime(2004, 3, 11, 11, 22, 33, DateTimeKind.Utc);
        const string EXPECTED_TO_STRING_OUTPUT1 = "2004-03-11T11:22:33Z";
        this._testOutputHelper.WriteLine("o: " + toStringInput1.ToString("o", CultureInfo.InvariantCulture));
        toStringInput1.ToString("o", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT1);
        this._testOutputHelper.WriteLine("O: " + toStringInput1.ToString("O", CultureInfo.InvariantCulture));
        toStringInput1.ToString("O", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT1);
        // "Counter evidence" with "Iso8601Instant" API
        new Iso8601Instant(toStringInput1).ToString().ShouldBe(EXPECTED_TO_STRING_OUTPUT1);

        // Prints no fraction digits even though there are some.
        var toStringInput2 = new DateTime(2004, 3, 11, 11, 22, 33, 123, DateTimeKind.Utc);
        const string EXPECTED_TO_STRING_OUTPUT2 = "2004-03-11T11:22:33.123Z";
        this._testOutputHelper.WriteLine("s: " + toStringInput2.ToString("s", CultureInfo.InvariantCulture));
        toStringInput2.ToString("s", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT2);
        // "Counter evidence" with "Iso8601Instant" API
        new Iso8601Instant(toStringInput2).ToString().ShouldBe(EXPECTED_TO_STRING_OUTPUT2);
    }

    /// <summary>
    /// Shows how the regular .NET APIs don't handle ISO-8601 properly (both converting from and to string) - for <see cref="DateTimeOffset"/>.
    /// </summary>
    [Fact]
    public void Test_Justification_DateTimeOffset()
    {
        // See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

        //
        // TryParseExact() can't handle anything but 7 fraction digits. If you just provide 3, the string can't be parsed.
        // NOTE: We could use "TryParse()" but this would also accept formats other than ISO-8601.
        //

        const string PARSE_INPUT = "2004-03-11T11:22:33.123Z";
        // Round-trip format
        DateTimeOffset.TryParseExact(PARSE_INPUT, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTimeOffset.TryParseExact(PARSE_INPUT, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);
        DateTimeOffset.TryParseExact(PARSE_INPUT, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTimeOffset.TryParseExact(PARSE_INPUT, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);

        // Sortable
        DateTimeOffset.TryParseExact(PARSE_INPUT, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out _).ShouldBe(false);
        DateTimeOffset.TryParseExact(PARSE_INPUT, "s", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _).ShouldBe(false);

        // "Counter evidence" with "Iso8601Instant" API
        Iso8601Instant.TryParse(PARSE_INPUT, out _).ShouldBe(true);

        //
        // ToString() doesn't print the right number of fraction digits.
        //

        // Prints 7 fraction digits even though there are no fractions.
        var toStringInput1 = new DateTimeOffset(2004, 3, 11, 11, 22, 33, TimeSpan.Zero);
        const string EXPECTED_TO_STRING_OUTPUT1 = "2004-03-11T11:22:33Z";
        this._testOutputHelper.WriteLine("o: " + toStringInput1.ToString("o", CultureInfo.InvariantCulture));
        toStringInput1.ToString("o", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT1);
        this._testOutputHelper.WriteLine("O: " + toStringInput1.ToString("O", CultureInfo.InvariantCulture));
        toStringInput1.ToString("O", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT1);
        // "Counter evidence" with "Iso8601Instant" API
        new Iso8601Instant(toStringInput1).ToString().ShouldBe(EXPECTED_TO_STRING_OUTPUT1);

        // Prints no fraction digits even though there are some.
        var toStringInput2 = new DateTimeOffset(2004, 3, 11, 11, 22, 33, 123, TimeSpan.Zero);
        const string EXPECTED_TO_STRING_OUTPUT2 = "2004-03-11T11:22:33.123Z";
        this._testOutputHelper.WriteLine("s: " + toStringInput2.ToString("s", CultureInfo.InvariantCulture));
        toStringInput2.ToString("s", CultureInfo.InvariantCulture).ShouldNotBe(EXPECTED_TO_STRING_OUTPUT2);
        // "Counter evidence" with "Iso8601Instant" API
        new Iso8601Instant(toStringInput2).ToString().ShouldBe(EXPECTED_TO_STRING_OUTPUT2);
    }
}
