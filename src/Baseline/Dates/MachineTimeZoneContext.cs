using System;

namespace Baseline.Dates
{
    public class MachineTimeZoneContext : ITimeZoneContext
    {
        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.Local;
        }
    }
}