using System;
using Xunit;
using Baseline;
using Shouldly;

namespace Baseline.Testing
{
    public class PeriodTester
    {
        private readonly Period thePeriod;

        public PeriodTester()
        {
            thePeriod = new Period(DateTime.Now.ToLocal());
        }


        [Fact]
        public void mark_completed()
        {
            var completedTime = DateTime.Now.ToLocal();
            thePeriod.MarkCompleted(completedTime);
            thePeriod.To.ShouldBe(completedTime);
        }

        [Fact]
        public void is_active_at_with_open_to()
        {
            thePeriod.To.ShouldBeNull();
            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();

            thePeriod.IsActiveAt(thePeriod.From.Add(3.Days())).ShouldBeTrue();


            thePeriod.IsActiveAt(thePeriod.From.Add(-1.Days())).ShouldBeFalse();
        }

        [Fact]
        public void is_active_when_the_boundary_is_closed()
        {
            thePeriod.MarkCompleted(thePeriod.From.Add(2.Days()));

            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.Add(1.Minutes())).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.Add(-1.Minutes())).ShouldBeFalse();

            // NOT inclusive
            thePeriod.IsActiveAt(thePeriod.To).ShouldBeFalse();
            thePeriod.IsActiveAt(thePeriod.To.Add(1.Minutes())).ShouldBeFalse();
        }

        [Fact]
        public void find_date_time_within()
        {
            var today = DateTime.Today.ToLocal();
            var from = today.Add(7.Hours());
            var to = today.Add(31.Hours());

            var period = new Period(from, to);

            period.FindDateTime("0700").ShouldBe(today.Add(7.Hours()));
            period.FindDateTime("0800").ShouldBe(today.Add(8.Hours()));
            period.FindDateTime("2300").ShouldBe(today.Add(23.Hours()));
            period.FindDateTime("0500").ShouldBe(today.Add(29.Hours())); // early morning the next day
            period.FindDateTime("0300").ShouldBe(today.Add(27.Hours())); // early morning the next day
        }
    }
}