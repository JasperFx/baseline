using System;
using System.Diagnostics;
using Baseline.Dates;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Dates
{
    public class LocalTimeTester
    {
        [Fact]
        public void dehydrate_and_hydrate()
        {
            var time = LocalTime.AtMachineTime("0800");
            var text = time.Hydrate();

            Debug.WriteLine(text);

            var time2 = new LocalTime(text);

            time2.UtcTime.ShouldBe(time.UtcTime);

            time2.ShouldNotBeSameAs(time);
            time2.ShouldBe(time);
        }

        [Fact]
        public void hydrate_with_only_time()
        {
            new LocalTime("0800").Time.ShouldBe(LocalTime.AtMachineTime("0800").Time);
        }

        [Fact]
        public void hydrate_with_date_and_time()
        {
            var givenLocalTime = new LocalTime("26022012:0800");
            var expectedLocalTime = LocalTime.AtDayAndTime(new Date("26022012"), "0800".ToTime());

            givenLocalTime.Time.ShouldBe(expectedLocalTime.Time);
            givenLocalTime.Date.ShouldBe(expectedLocalTime.Date);
        }

        [Fact]
        public void create_local_time()
        {
            var time = new LocalTime(DateTime.Today.AddHours(8).ToUniversalTime(TimeZoneInfo.Local), TimeZoneInfo.Local);
            time.Time.ShouldBe(DateTime.Today.AddHours(8));
            time.TimeOfDay.ShouldBe("0800".ToTime());
            time.Date.ShouldBe(DateTime.Today.ToDate());

            time.UtcTime.ShouldBe(DateTime.Today.AddHours(8).ToUniversalTime(TimeZoneInfo.Local));
        }

        [Fact]
        public void add()
        {
            var time = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var halfHourLater = time.Add("0800".ToTime());

            halfHourLater.Time.ShouldBe(DateTime.Today.AddHours(16));
        }

        [Fact]
        public void less_than()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 < time2).ShouldBeTrue();
            (time2 < time1).ShouldBeFalse();
        }

        [Fact]
        public void less_than_or_equal()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);
            var time3 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 <= time2).ShouldBeTrue();
            (time2 <= time1).ShouldBeFalse();
            (time2 <= time3).ShouldBeTrue();
            (time3 <= time2).ShouldBeTrue();
        }

        [Fact]
        public void greater_than_operator()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 > time2).ShouldBeFalse();
            (time2 > time1).ShouldBeTrue();
        }

        [Fact]
        public void greater_than_or_equal_operator()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);
            var time3 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 >= time2).ShouldBeFalse();
            (time2 >= time1).ShouldBeTrue();

            (time2 >= time3).ShouldBeTrue();
            (time3 >= time2).ShouldBeTrue();
        }

        [Fact]
        public void beginning_of_day()
        {
            var morningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8)); // 8 in the morning
            morningTime.BeginningOfDay().UtcTime.AddHours(8).ShouldBe(morningTime.UtcTime);
        }

        [Fact]
        public void GuessDayFromTimeOfDay()
        {
            var morningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8)); // 8 in the morning

            LocalTime.GuessDayFromTimeOfDay(morningTime, "0600".ToTime()).ShouldBe(morningTime.Add(-2.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "0900".ToTime()).ShouldBe(morningTime.Add(1.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "1000".ToTime()).ShouldBe(morningTime.Add(2.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "1500".ToTime()).ShouldBe(morningTime.Add(7.Hours()));
        }

        [Fact]
        public void guess_day_should_find_tomorrow()
        {
            var currentTime = LocalTime.AtMachineTime("2300");

            LocalTime.GuessDayFromTimeOfDay(currentTime, 800.ToTime())
                .ShouldBe(currentTime.Add(9.Hours()));
        }

        [Fact]
        public void should_find_yesterday()
        {
            var currentTime = LocalTime.AtMachineTime("0300");

            LocalTime.GuessDayFromTimeOfDay(currentTime, 2100.ToTime())
                .ShouldBe(currentTime.Add(-6.Hours()));
        }

        [Fact]
        public void subtract()
        {
            var firstTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8));
            var secondTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(12));

            secondTime.Subtract(firstTime).ShouldBe(4.Hours());
        }
    }
}