using System;

namespace Baseline
{
    public class Clock : IClock
    {
        private Func<DateTime> _now = () => DateTime.UtcNow;

        public DateTime UtcNow()
        {
            return _now();
        }

        public void Live()
        {
            _now = () => DateTime.Now;
        }

        public Clock LocalNow(DateTime localTime, TimeZoneInfo localZone = null)
        {
            var zone = localZone ?? TimeZoneInfo.Local;
            var now = localTime.ToUniversalTime(zone);

            _now = () => now;
            return this;
        }

        public Clock RestartAtLocal(DateTime desiredLocalTime, TimeZoneInfo localZone = null)
        {
            var zone = localZone ?? TimeZoneInfo.Local;
            var desired = desiredLocalTime.ToUniversalTime(zone);

            var delta = desired.Subtract(DateTime.UtcNow);
            _now = () => DateTime.UtcNow.Add(delta);

            return this;
        }
    }
}