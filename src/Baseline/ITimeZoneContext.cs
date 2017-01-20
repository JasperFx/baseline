using System;

namespace Baseline
{
    public interface ITimeZoneContext
    {
        TimeZoneInfo GetTimeZone();
    }
}