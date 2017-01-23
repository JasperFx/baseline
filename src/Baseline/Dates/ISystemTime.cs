using System;

namespace Baseline.Dates
{
    public interface ISystemTime
    {
        DateTime UtcNow();

        LocalTime LocalTime();
    }
}