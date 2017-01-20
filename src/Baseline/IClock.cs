using System;

namespace Baseline
{
    public interface IClock
    {
        DateTime UtcNow();
    }
}