using System;
using Xunit;
using Baseline;
using Shouldly;

namespace Baseline.Testing
{
    public class SettableClockTester
    {
        [Fact]
        public void set_the_clock()
        {
            var clock = new SettableClock();
            var localNow = DateTime.Today.AddHours(8);

            clock.LocalNow(localNow, TimeZoneInfo.Local);

            clock.LocalTime().Time.ShouldBe(localNow);

            clock.UtcNow().ShouldBe(localNow.ToUniversalTime(TimeZoneInfo.Local));
        }

        [Fact]
        public void set_the_clock_with_a_local_time()
        {
            var local = LocalTime.AtMachineTime("0800");

            var clock = new SettableClock();
            clock.LocalNow(local);


            clock.ShouldNotBeSameAs(local);
            clock.LocalTime().ShouldBe(local);
            clock.UtcNow().ShouldBe(local.UtcTime);
        }
    }
}