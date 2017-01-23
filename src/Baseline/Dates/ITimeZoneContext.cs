using System;

namespace Baseline.Dates
{
    public interface ITimeZoneContext
    {
        TimeZoneInfo GetTimeZone();
    }
}