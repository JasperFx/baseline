using System;

namespace Baseline
{
    public class MachineTimeZoneContext : ITimeZoneContext
    {
        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.Local;
        }
    }
}