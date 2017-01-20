using System;
using Xunit;
using Baseline;
using Shouldly;

namespace Baseline.Testing
{
    public class DateTimeExtensionTester
    {
        [Fact]
        public void to_date()
        {
            // not everything is hard
            DateTime.Now.ToDate().Day.ShouldBe(DateTime.Today);
        }

        [Fact]
        public void first_day_of_month()
        {
            var date = new DateTime(2012, 6, 7);
            date.FirstDayOfMonth().ShouldBe(new DateTime(2012, 6, 1).ToDate());
        }

        [Fact]
        public void last_day_of_month()
        {
            var date = new DateTime(2012, 6, 7);
            date.LastDayOfMonth().ShouldBe(new DateTime(2012, 6, 30).ToDate());
        }

        [Fact]
        public void end_of_month_calcs_properly_mid_month_input()
        {
            var realLastDay = new DateTime(2012, 02, 29);
            var today = new DateTime(2012, 02, 03);
            today.LastDayOfMonth().ShouldBe(realLastDay.ToDate());
        }

        [Fact]
        public void eom_as_input_calcs_begin_and_end_properly()
        {
            var realLastDay = new DateTime(2012, 02, 29);
            var realFirstDay = new DateTime(2012, 02, 01);
            var today = new DateTime(2012, 02, 29);
            today.LastDayOfMonth().ShouldBe(realLastDay.ToDate());
            today.FirstDayOfMonth().ShouldBe(realFirstDay.ToDate());
        }

        [Fact]
        public void first_day_of_month_for_date_is_same()
        {
            var today = new DateTime(2012, 02, 01);
            today.FirstDayOfMonth().ShouldBe(today.ToDate());
        }

        [Fact]
        public void middle_of_month_input_has_proper_first_day_of_month()
        {
            var realFirstDay = new DateTime(2012, 02, 01);
            var today = new DateTime(2012, 02, 03);
            today.FirstDayOfMonth().ShouldBe(realFirstDay.ToDate());
        }
    }
}