using System;
using System.Diagnostics;

namespace Baseline
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Execute the supplied action a number of times
        /// </summary>
        /// <param name="maxCount"></param>
        /// <param name="eachAction"></param>
        /// <returns></returns>
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