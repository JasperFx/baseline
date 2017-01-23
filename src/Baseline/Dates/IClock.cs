using System;

namespace Baseline.Dates
{
    public interface IClock
    {
        DateTime UtcNow();
    }
}