using System;
using System.Diagnostics;

namespace Baseline.Testing
{
    public static class NumberExtensions
    {
        [DebuggerStepThrough]
        public static int Times(this int maxCount, Action<int> eachAction)
        {
            for (int idx = 1; idx <= maxCount; idx++)
            {
                eachAction(idx);
            }

            return maxCount;
        }
    }
}