using System;

namespace Baseline
{
    public interface ISystemTime
    {
        DateTime UtcNow();

        LocalTime LocalTime();
    }
}