using System;
using Xunit;
using Baseline;
using Shouldly;

namespace Baseline.Testing
{
    public class DateTester
    {
        [Fact]
        public void convert_by_constructor()
        {
            var date = new Date("22022012");
            date.Day.ShouldBe(new DateTime(2012, 2, 22));
        }

        [Fact]
        public void equals_method()
        {
            var date1 = new Date("22022012");
            var date2 = new Date("22022012");
            var date3 = new Date("22022013");

            date1.ShouldBe(date2);
            date2.ShouldBe(date1);

            date3.ShouldNotBe(date1);
        }

        [Fact]
        public void to_string_uses_the_ugly_ddMMyyyy_format()
        {
            var date = new Date(2, 22, 2012);
            date.ToString().ShouldBe("22022012");
        }

        [Fact]
        public void ctor_by_date_loses_the_time()
        {
            var date = new Date(DateTime.Now);
            date.Day.ShouldBe(DateTime.Today);
        }
    }
}