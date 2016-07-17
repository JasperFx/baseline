using System.Collections.Generic;
using Baseline.Binding;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Binding
{
    /*
     * More things:
     * 1.) test numbers
     * 2.) test dates, etc.
     * 3.) Missing data, do nothing
     * 4.) ConvertProblems
     * 5.) Nested properties
     * 6.) Use expressions within conversions to be more efficient
     * 7.) BoundProperty/BoundField/BoundNested model for diagnostics
     * 8.) Optimize strings
     * 
     */

    public class BinderTests
    {
        public BinderTests()
        {
            theSource = new DictionaryDataSource(theData);
        }

        private readonly Binder<Target> binder = new Binder<Target>();
        private readonly Dictionary<string, string> theData = new Dictionary<string, string>();
        private readonly DictionaryDataSource theSource;
        
        [Fact]
        public void can_bind_string_property()
        {
            theData.Add(nameof(Target.String), "somebody");

            binder.Build(theSource)
                .String.ShouldBe("somebody");
        }

        [Fact]
        public void do_nothing_with_a_missing_string_property_value()
        {
            binder.Build(theSource)
                .String.ShouldBeNull();
        }

        [Fact]
        public void can_bind_string_field()
        {
            theData.Add(nameof(Target.StringField), "somebody");

            binder.Build(theSource)
                .StringField.ShouldBe("somebody");
        }


        [Fact]
        public void do_nothing_with_a_missing_string_field_value()
        {
            binder.Build(theSource)
                .StringField.ShouldBeNull();
        }


        [Fact]
        public void can_build_a_binder()
        {
            binder.ShouldNotBeNull();
        }
        

        [Fact]
        public void can_bind_single_property_if_it_exists()
        {
            theData.Add(nameof(SinglePropertyGuy.Name), "Declan");

            var binderForGuy = new Binder<SinglePropertyGuy>();

            var guy = binderForGuy.Build(theSource);

            guy.Name.ShouldBe("Declan");
        }

        [Fact]
        public void can_bind_numbers()
        {
            theData.Add(nameof(Target.Number), "1");
            theData.Add(nameof(Target.Long), "2");
            theData.Add(nameof(Target.Double), "3.4");
            theData.Add(nameof(Target.Float), "5.6");

            var target = binder.Build(theSource);

            target.Number.ShouldBe(1);
            target.Long.ShouldBe(2L);
            target.Double.ShouldBe(3.4);
            target.Float.ShouldBe(5.6F);
        }

        [Fact]
        public void can_bind_booleans()
        {
            theData.Add(nameof(Target.Flag), "true");

            binder.Build(theSource)
                .Flag.ShouldBeTrue();
        }
        
    }

    public class SinglePropertyGuy
    {
        public string Name { get; set; }
    }
}