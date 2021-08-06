using System;

namespace Baseline.Dates
{
    public interface ISettableClock : ISystemTime
    {
        ISettableClock LocalNow(DateTime now, TimeZoneInfo? timeZone = null);
        ISettableClock LocalNow(LocalTime time);
    }

    public class SettableClock : ISettableClock
    {
        private DateTime _time = DateTime.UtcNow;
        private TimeZoneInfo _timeZone = TimeZoneInfo.Local;

        public DateTime UtcNow()
        {
            return _time;
        }

        public LocalTime LocalTime()
        {
            return new LocalTime(_time, _timeZone);
        }

        public ISettableClock LocalNow(LocalTime time)
        {
            _timeZone = time.TimeZone;
            _time = time.UtcTime;

            return this;
        }

        public ISettableClock LocalNow(DateTime now, TimeZoneInfo? timeZone = null)
        {
            _timeZone = timeZone ?? TimeZoneInfo.Local;
            _time = now.ToUniversalTime(_timeZone);

            return this;
        }
    }
}