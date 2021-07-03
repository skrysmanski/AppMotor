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
using System.Text.Json;
using System.Threading;

using AppMotor.Core.TestUtils;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class DateTimeUtcTests
    {
        [Fact]
        public void Test_Now()
        {
            DateTimeUtc.Now.ShouldBe(DateTimeUtc.Now, tolerance: TimeSpan.FromMilliseconds(2));

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            // Check that is has changed
            DateTimeUtc.Now.ToDateTime().ShouldBe(DateTime.UtcNow, tolerance: TimeSpan.FromMilliseconds(2));
        }

        [Fact]
        public void Test_Constructor_DateTime_Utc()
        {
            var now = DateTime.UtcNow;
            var dateTimeUtc = new DateTimeUtc(now);

            dateTimeUtc.Year.ShouldBe(now.Year);
            dateTimeUtc.Month.ShouldBe(now.Month);
            dateTimeUtc.Day.ShouldBe(now.Day);
            dateTimeUtc.Hour.ShouldBe(now.Hour);
            dateTimeUtc.Minute.ShouldBe(now.Minute);
            dateTimeUtc.Second.ShouldBe(now.Second);
            dateTimeUtc.Millisecond.ShouldBe(now.Millisecond);

            dateTimeUtc.Ticks.ShouldBe(now.Ticks);
        }

        [Fact]
        public void Test_Constructor_DateTime_Local()
        {
            var now = DateTime.Now;
            var dateTimeUtc = new DateTimeUtc(now);

            dateTimeUtc.Ticks.ShouldBe(now.ToUniversalTime().Ticks);
        }

        [Fact]
        public void Test_Constructor_DateTime_Unspecified()
        {
            var dateTimeWithUnspecifiedKind = new DateTime(2020, 1, 2);
            Should.Throw<ArgumentException>(() => new DateTimeUtc(dateTimeWithUnspecifiedKind));
        }

        [Fact]
        public void Test_Constructor_DateTimeOffset()
        {
            var now = new DateTimeOffset(2020, 2, 3, 6, 5, 6, 7, offset: TimeSpan.FromHours(2));
            var dateTimeUtc = new DateTimeUtc(now);

            dateTimeUtc.Year.ShouldBe(2020);
            dateTimeUtc.Month.ShouldBe(2);
            dateTimeUtc.Day.ShouldBe(3);
            dateTimeUtc.Hour.ShouldBe(4);
            dateTimeUtc.Minute.ShouldBe(5);
            dateTimeUtc.Second.ShouldBe(6);
            dateTimeUtc.Millisecond.ShouldBe(7);
        }

        [Fact]
        public void Test_Constructor_Ticks()
        {
            var now = DateTime.UtcNow;
            var dateTimeUtc = new DateTimeUtc(now.Ticks);

            dateTimeUtc.Year.ShouldBe(now.Year);
            dateTimeUtc.Month.ShouldBe(now.Month);
            dateTimeUtc.Day.ShouldBe(now.Day);
            dateTimeUtc.Hour.ShouldBe(now.Hour);
            dateTimeUtc.Minute.ShouldBe(now.Minute);
            dateTimeUtc.Second.ShouldBe(now.Second);
            dateTimeUtc.Millisecond.ShouldBe(now.Millisecond);

            dateTimeUtc.Ticks.ShouldBe(now.Ticks);
        }

        [Fact]
        public void Test_Constructor_WithoutMilliseconds()
        {
            var dateTimeUtc = new DateTimeUtc(2020, 2, 3, 4, 5, 6);

            dateTimeUtc.Year.ShouldBe(2020);
            dateTimeUtc.Month.ShouldBe(2);
            dateTimeUtc.Day.ShouldBe(3);
            dateTimeUtc.Hour.ShouldBe(4);
            dateTimeUtc.Minute.ShouldBe(5);
            dateTimeUtc.Second.ShouldBe(6);
            dateTimeUtc.Millisecond.ShouldBe(0);
        }

        [Fact]
        public void Test_Constructor_WithMilliseconds()
        {
            var dateTimeUtc = new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7);

            dateTimeUtc.Year.ShouldBe(2020);
            dateTimeUtc.Month.ShouldBe(2);
            dateTimeUtc.Day.ShouldBe(3);
            dateTimeUtc.Hour.ShouldBe(4);
            dateTimeUtc.Minute.ShouldBe(5);
            dateTimeUtc.Second.ShouldBe(6);
            dateTimeUtc.Millisecond.ShouldBe(7);
        }

        [Fact]
        public void Test_Date()
        {
            var dateTimeUtc = new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7);
            var date = dateTimeUtc.Date;

            date.Year.ShouldBe(2020);
            date.Month.ShouldBe(2);
            date.Day.ShouldBe(3);
            date.Hour.ShouldBe(0);
            date.Minute.ShouldBe(0);
            date.Second.ShouldBe(0);
            date.Millisecond.ShouldBe(0);
        }

        [Fact]
        public void Test_TimeOfDay()
        {
            var dateTimeUtc = new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7);
            var timeOfDay = dateTimeUtc.TimeOfDay;

            timeOfDay.Days.ShouldBe(0);
            timeOfDay.Hours.ShouldBe(4);
            timeOfDay.Minutes.ShouldBe(5);
            timeOfDay.Seconds.ShouldBe(6);
            timeOfDay.Milliseconds.ShouldBe(7);
        }

        [Fact]
        public void Test_DayOfWeek()
        {
            var dateTimeUtc = new DateTimeUtc(2021, 7, 1, 4, 5, 6);
            dateTimeUtc.DayOfWeek.ShouldBe(DayOfWeek.Thursday);
        }

        [Fact]
        public void Test_DayOfYear()
        {
            var today = DateTime.Today;
            var dateTimeUtc = new DateTimeUtc(today.Year, today.Month, today.Day, 4, 5, 6);
            dateTimeUtc.DayOfYear.ShouldBe(today.DayOfYear);
        }

        [Fact]
        public void Test_AddMonths()
        {
            var now = new DateTimeUtc(2021, 7, 1, 4, 5, 6);
            var later = now.AddMonths(2);
            later.Month.ShouldBe(9);
        }

        [Fact]
        public void Test_AddYears()
        {
            var now = new DateTimeUtc(2021, 7, 1, 4, 5, 6);
            var later = now.AddYears(3);
            later.Year.ShouldBe(2024);
        }

        [Fact]
        public void Test_ToDateTime()
        {
            var now = DateTime.UtcNow;
            var dateTimeUtc = new DateTimeUtc(now);

            dateTimeUtc.ToDateTime().Ticks.ShouldBe(now.Ticks);
        }

        [Fact]
        public void Test_ToLocalTime()
        {
            var now = DateTime.Now;
            var dateTimeUtc = new DateTimeUtc(now);

            dateTimeUtc.ToLocalTime().ShouldBe(now);
        }

        [Fact]
        public void Test_ToDateTimeOffset()
        {
            var now = DateTime.UtcNow;
            var dateTimeUtc = new DateTimeUtc(now);

            var dateTimeOffset = dateTimeUtc.ToDateTimeOffset();
            dateTimeOffset.Offset.ShouldBe(TimeSpan.Zero);
            dateTimeOffset.Ticks.ShouldBe(now.Ticks);
        }

        [Fact]
        public void Test_Add_TimeSpan()
        {
            var now = new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7);
            DateTimeUtc later = now + TimeSpan.FromHours(10);

            later.Year.ShouldBe(2020);
            later.Month.ShouldBe(2);
            later.Day.ShouldBe(3);
            later.Hour.ShouldBe(14);
            later.Minute.ShouldBe(5);
            later.Second.ShouldBe(6);
            later.Millisecond.ShouldBe(7);
        }

        [Fact]
        public void Test_Subtract_TimeSpan()
        {
            var now = new DateTimeUtc(2020, 2, 3, 14, 5, 6, 7);
            DateTimeUtc later = now - TimeSpan.FromHours(10);

            later.Year.ShouldBe(2020);
            later.Month.ShouldBe(2);
            later.Day.ShouldBe(3);
            later.Hour.ShouldBe(4);
            later.Minute.ShouldBe(5);
            later.Second.ShouldBe(6);
            later.Millisecond.ShouldBe(7);
        }

        [Fact]
        public void Test_Subtract_DateTimeUtc()
        {
            var a = new DateTimeUtc(2020, 2, 3, 14, 5, 6, 7);
            var b = new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7);

            var diff = a - b;

            diff.TotalHours.ShouldBe(10);
        }

        [Fact]
        public void Test_ToString()
        {
            var now = DateTime.UtcNow;
            var dateTimeUtc = new DateTimeUtc(now);

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            dateTimeUtc.ToString().ShouldBe(now.ToString());
            dateTimeUtc.ToString(null).ShouldBe(now.ToString((string?)null));
            dateTimeUtc.ToString(null, null).ShouldBe(now.ToString(null, null));
        }

        [Fact]
        public void Test_Equals()
        {
            var now = DateTimeUtc.Now;
            var later = now + TimeSpan.FromMilliseconds(1);

            now.Equals(now).ShouldBe(true);
            now.Equals(later).ShouldBe(false);
            (now == later).ShouldBe(false);
            (now != later).ShouldBe(true);

            now.Equals((object)now).ShouldBe(true);
            now.Equals((object)later).ShouldBe(false);
            now.Equals(null).ShouldBe(false);
        }

        [Fact]
        public void Test_GetHashCode()
        {
            var nowDateTime = DateTime.Now;

            var a = new DateTimeUtc(nowDateTime);
            var b = new DateTimeUtc(nowDateTime.ToUniversalTime());

            a.GetHashCode().ShouldBe(b.GetHashCode());
        }

        [Fact]
        public void Test_Compare()
        {
            var now = DateTimeUtc.Now;
            var later = now + TimeSpan.FromMilliseconds(1);

            now.CompareTo(now).ShouldBe(0);
            now.CompareTo(later).ShouldBe(-1);
            later.CompareTo(now).ShouldBe(1);

            now.CompareTo((object)now).ShouldBe(0);
            now.CompareTo((object)later).ShouldBe(-1);
            later.CompareTo((object)now).ShouldBe(1);
            Should.Throw<ArgumentException>(() => now.CompareTo(new object()));
            now.CompareTo(null).ShouldBe(1);

            (now < later).ShouldBe(true);
            (later < now).ShouldBe(false);
            (now > later).ShouldBe(false);
            (later > now).ShouldBe(true);

            (now <= later).ShouldBe(true);
            (later <= now).ShouldBe(false);
            (now <= later - TimeSpan.FromMilliseconds(1)).ShouldBe(true);
            (now >= later).ShouldBe(false);
            (later >= now).ShouldBe(true);
            (later - TimeSpan.FromMilliseconds(1) >= now).ShouldBe(true);
        }

        [Fact]
        public void Test_JsonSerialization()
        {
            var testData = new JsonTestData(new DateTimeUtc(2020, 2, 3, 4, 5, 6, 7));

            var json = JsonSerializer.Serialize(testData);

            json.ShouldBe("{\"SomeTime\":\"2020-02-03T04:05:06.007Z\"}");

            var deserializedTestData = JsonSerializer.Deserialize<JsonTestData>(json);

            deserializedTestData!.SomeTime.ShouldBe(testData.SomeTime);
        }

        private record JsonTestData(DateTimeUtc SomeTime);

        [Fact]
        public void Test_JsonSerialization_Null()
        {
            var testData = new JsonTestDataNullable(null);

            var json = JsonSerializer.Serialize(testData);

            json.ShouldBe("{\"SomeTime\":null}");

            var deserializedTestData = JsonSerializer.Deserialize<JsonTestDataNullable>(json);

            deserializedTestData!.SomeTime.ShouldBe(null);
        }

        private record JsonTestDataNullable(DateTimeUtc? SomeTime);
    }
}
