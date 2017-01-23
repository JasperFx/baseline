using System;

namespace Baseline.Dates
{
    public class Milestone
    {
        private DateTime? _timestamp = null;

        public static implicit operator bool(Milestone m)
        {
            if (m == null) return false;

            return m.IsTrue;
        }

        public Milestone()
        {
        }

        public Milestone(string timeString)
        {
            if (timeString.IsNotEmpty())
            {
                _timestamp = DateTime.Parse(timeString);
            }
        }

        public Milestone(LocalTime time) : this(time.UtcTime) { }

        public Milestone(DateTime timestamp)
        {
            Capture(timestamp);
        }

        public Milestone Capture(DateTime timestamp)
        {
            if (timestamp.Kind != DateTimeKind.Utc)
            {
                _timestamp = timestamp.ToUniversalTime();
            }
            else
            {
                _timestamp = timestamp;
            }


            return this;
        }

        public DateTime? Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }

        public bool IsTrue
        {
            get
            {
                return _timestamp.HasValue;
            }
        }

        public bool IsFalse
        {
            get
            {
                return !IsTrue;
            }
        }


        public bool HappenedBefore(DateTime time)
        {
            return _timestamp.HasValue ? _timestamp.Value < time : false;
        }

        public bool Equals(Milestone other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._timestamp.Equals(_timestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Milestone)) return false;
            return Equals((Milestone)obj);
        }

        public override int GetHashCode()
        {
            return (_timestamp.HasValue ? _timestamp.Value.GetHashCode() : 0);
        }

        public void Clear()
        {
            _timestamp = null;
        }

        public override string ToString()
        {
            return _timestamp.HasValue ? _timestamp.Value.ToString("s") : string.Empty;
        }
    }
}