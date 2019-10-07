using Shouldly;
using Widgets1;
using Widgets5;
using Xunit;

namespace BaselineTypeDiscovery.Testing
{
    public class CallingAssemblyTests
    {
        [Fact]
        public void use_current_assembly()
        {
            CallingAssembly.Find()
                .ShouldBe(GetType().Assembly);
        }

        [Fact]
        public void from_another_assembly()
        {
            WidgetCallingAssemblyFinder.Calling()
                .ShouldBe(typeof(WidgetCallingAssemblyFinder).Assembly);
        }

        [Fact]
        public void skip_ignore_assembly()
        {
            // Widget4 assembly should be ignored
            Widget5CallingWidget4Caller.Calling()
                .ShouldBe(typeof(Widget5CallingWidget4Caller).Assembly);
        }
    }
}