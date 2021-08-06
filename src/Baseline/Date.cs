using System;
using Baseline.Conversion;

namespace Baseline
{
    public class Date
    {
        public const string TimeFormat = "ddMMyyyy";

        // This *has* to be here for serialization
        public Date()
        {
        }

        public Date(DateTime date)
            : this(date.ToString(TimeFormat))
        {
        }

        public Date(int month, int day, int year)
        {
            Day = new DateTime(year, month, day);
        }

        public Date(string ddmmyyyy)
        {
            Day = DateTime.ParseExact(ddmmyyyy, TimeFormat, null);
        }

        public DateTime Day { get; set; }

        public Date NextDay()
        {
            return new Date(Day.AddDays(1));
        }

        public bool Equals(Date? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Day.Equals(Day);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Date)) return false;
            return Equals((Date) obj);
        }

        public static bool operator ==(Date left, Date right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return Day.GetHashCode();
        }

        public override string ToString()
        {
            return Day.ToString(TimeFormat);
        }

        public Date AddDays(int daysFromNow)
        {
            return new Date(Day.AddDays(daysFromNow));
        }

        public DateTime AtTime(TimeSpan time)
        {
            return Day.Date.Add(time);
        }

        public DateTime AtTime(string mmhh)
        {
            return Day.Date.Add(TimeSpanConverter.GetTimeSpan(mmhh));
        }

        public static Date Today()
        {
            return new Date(DateTime.Today);
        }
    }
}