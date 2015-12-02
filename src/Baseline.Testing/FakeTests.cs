using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public class FakeTests
    {
        [Fact]
        public void do_something()
        {
            1.ShouldBe(1);
        }
    }
}
