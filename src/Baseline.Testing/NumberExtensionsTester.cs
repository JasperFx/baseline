using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class NumberExtensionsTester
    {
        [Fact]
        public void Times_runs_an_action_the_specified_number_of_times()
        {
            int maxCount = 5;
            int total = 0;

            maxCount.Times(x => total += x);

            total.ShouldBe(1 + 2 + 3+ 4+5);
        }
    }
}