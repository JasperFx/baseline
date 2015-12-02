using System;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class TimeSpanExtensionsTester
    {
        [Fact]
        public void to_time_from_int()
        {
            700.ToTime().ShouldBe(new TimeSpan(7, 0, 0));
            1700.ToTime().ShouldBe(new TimeSpan(17, 0, 0));
            1850.ToTime().ShouldBe(new TimeSpan(18, 50, 0));
        }

        [Fact]
        public void to_time_from_string()
        {
            "0700".ToTime().ShouldBe(new TimeSpan(7, 0, 0));
            "1700".ToTime().ShouldBe(new TimeSpan(17, 0, 0));
            "1850".ToTime().ShouldBe(new TimeSpan(18, 50, 0));
        }

        [Fact]
        public void Minutes()
        {
            5.Minutes().ShouldBe(new TimeSpan(0, 5, 0));
        }

        [Fact]
        public void hours()
        {
            6.Hours().ShouldBe(new TimeSpan(6, 0, 0));
        }

        [Fact]
        public void days()
        {
            2.Days().ShouldBe(new TimeSpan(2, 0, 0, 0));
        }

        [Fact]
        public void seconds()
        {
            8.Seconds().ShouldBe(new TimeSpan(0, 0, 8));
        }
    }
}