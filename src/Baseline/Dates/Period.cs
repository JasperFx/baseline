using System;
using Baseline.Conversion;

namespace Baseline.Dates
{
    public class Period
    {
        // For serialization
#nullable disable
        public Period()
        {
        }
#nullable enable

        public Period(DateTime utcTime) : this(new LocalTime(utcTime, TimeZoneInfo.Utc))
        {
        }

        public Period(LocalTime from)
        {
            From = from;
        }

        public Period(LocalTime from, LocalTime to)
        {
            From = from;
            To = to;
        }


        public LocalTime From { get; set; }
        public LocalTime? To { get; set; }

        public void MarkCompleted(LocalTime completedTime)
        {
            To = completedTime;
        }

        public override string ToString()
        {
            return "{0} - {1}".ToFormat(From, To!);
        }

        public bool IsActiveAt(LocalTime timestamp)
        {
            return IsActiveAt(timestamp.UtcTime);
        }

        public bool IsActiveAt(DateTime utcTime)
        {
            if (utcTime < From.UtcTime) return false;

            return To != null
                ? utcTime < To.UtcTime
                : true;
        }

        public bool Equals(Period? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.From, From) && Equals(other.To, To);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Period)) return false;
            return Equals((Period) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((From != null ? From.GetHashCode() : 0)*397) ^ (To != null ? To.GetHashCode() : 0);
            }
        }

        public LocalTime FindDateTime(TimeSpan time)
        {
            if (To == null)
                throw new InvalidOperationException("FindDateTime can only be used if there is a value for To");

            var date = From.BeginningOfDay();
            while (date <= To)
            {
                var candidate = date.Add(time);
                if (IsActiveAt(candidate)) return candidate;

                date = date.Add(1.Days());
            }

            throw new InvalidOperationException("Unable to find the matching time");
        }

        public LocalTime FindDateTime(string timeString)
        {
            return FindDateTime(TimeSpanConverter.GetTimeSpan(timeString));
        }
    }
}