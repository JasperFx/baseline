using System;
using Shouldly;
using Xunit;
using Baseline;

namespace Baseline.Testing
{
    public class SystemTimeTester
    {
        [Fact]
        public void local_now()
        {
            var now = SystemTime.Default().LocalTime().Time;
            var secondNow = DateTime.Now;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }


        [Fact]
        public void get_today()
        {
            SystemTime.Default().LocalTime().Date.Day.ShouldBe(DateTime.Today);
        }

        [Fact]
        public void current_time()
        {
            var now = SystemTime.Default().LocalTime().TimeOfDay;
            var secondNow = DateTime.Now.TimeOfDay;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }

        [Fact]
        public void stub()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalTime().Time.ShouldBe(now);
            systemTime.LocalTime().TimeOfDay.ShouldBe(800.ToTime());
            systemTime.LocalTime().Date.Day.ShouldBe(DateTime.Today.AddDays(1));
        }

        [Fact]
        public void stub_then_back_to_live()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalTime().Time.ShouldBe(now);

            clock.Live();

            var firstNow = SystemTime.Default().LocalTime().Time;
            var secondNow = DateTime.Now;

            secondNow.Subtract(firstNow).TotalSeconds.ShouldBeLessThan(1);
        }

        [Fact]
        public void time_zone_is_used_to_calculate_local_time()
        {
            TimeZoneInfo.GetSystemTimeZones().Each(zone =>
            {
                var time = new SystemTime(new Clock(), new SimpleTimeZoneContext(zone));
                var first = time.LocalTime().Time;
                var second = DateTime.UtcNow.ToLocalTime(zone);

                second.Subtract(first).TotalMilliseconds.ShouldBeLessThan(100);
            });
        }

        [Fact]
        public void stub_by_using_at_local_time_by_time()
        {
            var systemTime = SystemTime.AtLocalTime("0700".ToTime());
            systemTime.LocalTime().ShouldBe(LocalTime.AtMachineTime("0700"));
        }
    }
}