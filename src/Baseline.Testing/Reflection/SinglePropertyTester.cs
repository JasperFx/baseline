using System;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class SinglePropertyTester
    {
        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
            public int Number { get; set; }
        }

        public class ChildTarget
        {
            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
        }

        public class HierarchicalTarget
        {
            public Target Child { get; set; }
        }

        [Fact]
        public void DeclaringType_of_a_single_property_is_type_of_the_object_containing_the_property()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            accessor.ShouldBeOfType<SingleProperty>().DeclaringType.ShouldBe(typeof (Target));
        }

        [Fact]
        public void GetFieldNameFromSingleProperty()
        {
            var property = SingleProperty.Build<Target>(x => x.Name);
            ((object) property.FieldName).ShouldBe("Name");
        }

        [Fact]
        public void GetNameFromSingleProperty()
        {
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.Name.ShouldBe("Name");
        }

        [Fact]
        public void GetValueFromSingleProperty()
        {
            var target = new Target{
                Name = "Jeremy"
            };
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.GetValue(target).ShouldBe("Jeremy");
        }

        [Fact]
        public void SetValueFromSingleProperty()
        {
            var target = new Target{
                Name = "Jeremy"
            };
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.SetValue(target, "Justin");

            target.Name.ShouldBe("Justin");
        }

        [Fact]
        public void build_single_property()
        {
            var prop1 = SingleProperty.Build<Target>("Child");
            var prop2 = SingleProperty.Build<Target>(x => x.Child);
            prop1.ShouldBe(prop2);
            prop1.Name.ShouldBe("Child");
            prop1.OwnerType.ShouldBe(typeof (Target));
        }

        [Fact]
        public void equals_for_a_single_property()
        {
            var prop1 = SingleProperty.Build<Target>(x => x.Name);
            var prop2 = SingleProperty.Build<Target>(x => x.Name);
            var prop3 = SingleProperty.Build<Target>(x => x.Child);

            prop1.ShouldBe(prop2);
            prop1.ShouldNotBe(prop3);
            prop1.Equals(null).ShouldBeFalse();
            prop1.Equals(prop1).ShouldBeTrue();
            prop1.ShouldBe(prop1);
            prop1.Equals((object) null).ShouldBeFalse();
            prop1.Equals(42).ShouldBeFalse();
        }

        [Fact]
        public void get_expression_from_accessor()
        {
            var target = new Target{
                Name = "Chad"
            };

            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Name);

            var expression = accessor.ToExpression<Target>();
            expression.Compile()(target).ShouldBe("Chad");
        }


        [Fact]
        public void get_expression_from_accessor_for_a_number()
        {
            var target = new Target{
                Number = 3
            };

            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Number);

            var expression = accessor.ToExpression<Target>();
            expression.Compile()(target).ShouldBe(3);
        }

        [Fact]
        public void prepend_should_return_a_property_chain()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Name);
            var property = ReflectionHelper.GetProperty<HierarchicalTarget>(x => x.Child);

            var prependedAccessor = accessor.Prepend(property);
            prependedAccessor.ShouldBeOfType<PropertyChain>();
            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "Name");

            var target = new HierarchicalTarget{
                Child = new Target{
                    Name = "Jeremy"
                }
            };

            prependedAccessor.GetValue(target).ShouldBe("Jeremy");
        }

        [Fact]
        public void singleProperty_can_get_child_accessor()
        {
            var expected = ReflectionHelper.GetAccessor<ChildTarget>(c => c.GrandChild.Name);
            SingleProperty.Build<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldBe(expected);
        }

        [Fact]
        public void singleProperty_property_names_contains_single_value()
        {
            var propertyNames = SingleProperty.Build<Target>(t => t.Child.GrandChild.Name).PropertyNames;
            propertyNames.Length.ShouldBe(1);
            
            propertyNames.ShouldContain("Name");
        }
    }
}