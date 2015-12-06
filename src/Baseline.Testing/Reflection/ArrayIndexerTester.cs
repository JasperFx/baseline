using System;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class ArrayIndexerTester
    {
        public class Target
        {
            public string Name { get; set; }
        }

        [Fact]
        public void DeclaringType_of_a_array_is_the_array_type()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);
            accessor.ShouldBeOfType<ArrayIndexer>();
            accessor.OwnerType.ShouldBe(typeof (Target[]));
            accessor.DeclaringType.ShouldBe(typeof (Target[]));
            accessor.FieldName.ShouldBe("[1]");
            accessor.InnerProperty.ShouldBeNull();
            accessor.PropertyNames.ShouldContain("[1]");
            accessor.PropertyType.ShouldBe(typeof (Target));
        }

        [Fact]
        public void GetValueFromArray()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var target = new[] {new Target(), new Target() };

            accessor.GetValue(target).ShouldBe(target[1]);
            accessor.GetValue(target).ShouldNotBe(target[0]);
        }

        [Fact]
        public void SetValueOnArray()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var original = new Target();
            var replacement = new Target();
            var target = new[] {new Target(), original };

            accessor.SetValue(target, replacement);
            target[1].ShouldNotBe(original);
            target[1].ShouldBe(replacement);
        }

        [Fact]
        public void ExpressionCreation()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var target = new[] {new Target(), new Target() };
            accessor.ToExpression<Target[]>().Compile()(target).ShouldBe(target[1]);
        }

        [Fact]
        public void ExpressionCreationWithValueType()
        {
            var accessor = ReflectionHelper.GetAccessor<DateTime[]>(x => x[1]);

            var target = new[] { new DateTime(2015, 01, 01), new DateTime(2015, 01, 02) };
            accessor.ToExpression<DateTime[]>().Compile()(target).ShouldBe(target[1]);
        }

        [Fact]
        public void SetValueOnArrayWithValueType()
        {
            var accessor = ReflectionHelper.GetAccessor<DateTime[]>(x => x[1]);

            var original = new DateTime(2015, 01, 01);
            var replacement = new DateTime(2015, 01, 02);
            var target = new[] { new DateTime(2015, 01, 03), original };

            accessor.SetValue(target, replacement);
            target[1].ShouldNotBe(original);
            target[1].ShouldBe(replacement);
        }
   }
}