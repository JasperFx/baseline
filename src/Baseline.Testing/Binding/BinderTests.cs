using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline.Binding;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Binding
{
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
        public void can_bind_guid()
        {
            var id = Guid.NewGuid();
            theData.Add(nameof(Target.Id), id.ToString());

            binder.Build(theSource).Id.ShouldBe(id);
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
        public void can_capture_problems_without_blowing_up()
        {
            MemberInfo member = null;
            Exception ex = null;

            theData.Add(nameof(Target.Number), "1");

            // Will cause a conversion problem
            theData.Add(nameof(Target.Long), "abc");

            var target = binder.Build(theSource, (m, e) =>
            {
                member = m;
                ex = e;
            });

            target.Number.ShouldBe(1);

            member.Name.ShouldBe(nameof(Target.Long));

            ex.ShouldBeOfType<FormatException>();
        }

        [Fact]
        public void can_bind_booleans()
        {
            theData.Add(nameof(Target.Flag), "true");

            binder.Build(theSource)
                .Flag.ShouldBeTrue();
        }

        [Fact]
        public void happily_ignores_nested_property_when_no_data_is_available()
        {
            //var child = theSource.AddChild(nameof(Target.Inner));
            //child.Dictionary.Add(nameof(Child.Name), "Logan");

            var target = binder.Build(theSource);

            target.Inner.ShouldBeNull();
        }

        [Fact]
        public void can_bind_nested_property()
        {
            var child = theSource.AddChild(nameof(Target.Inner));
            child.Dictionary.Add(nameof(Child.Name), "Logan");

            var target = binder.Build(theSource);

            target.Inner.ShouldNotBeNull();
            target.Inner.Name.ShouldBe("Logan");
        }

        [Fact]
        public void happily_ignores_nested_field_when_no_data_is_available()
        {
            //var child = theSource.AddChild(nameof(Target.Inner));
            //child.Dictionary.Add(nameof(Child.Name), "Logan");

            var target = binder.Build(theSource);

            target.InnerField.ShouldBeNull();
        }

        [Fact]
        public void can_bind_nested_field()
        {
            var child = theSource.AddChild(nameof(Target.InnerField));
            child.Dictionary.Add(nameof(Child.Name), "Ororo");

            var target = binder.Build(theSource);

            target.InnerField.ShouldNotBeNull();
            target.InnerField.Name.ShouldBe("Ororo");
        }

    }

    public class SinglePropertyGuy
    {
        public string Name { get; set; }
    }
}