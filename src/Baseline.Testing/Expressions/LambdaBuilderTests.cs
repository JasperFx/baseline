using System;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Expressions;
using Baseline.Reflection;
using Baseline.Testing.Reflection;
using Shouldly;
using Xunit;
using System.Linq;

namespace Baseline.Testing.Expressions
{
    public class LambdaBuilderTests
    {
        [Fact]
        public void can_build_getter_for_property()
        {
            var target = new Target() {Number = 5};
            var prop = ReflectionHelper.GetProperty<Target>(x => x.Number);

            var getter = LambdaBuilder.GetProperty<Target, int>(prop);

            getter(target).ShouldBe(target.Number);
        }

        public class GuyWithField
        {
            public Guid Id = Guid.NewGuid();
        }

        [Fact]
        public void can_build_getter_for_field()
        {
            var guy = new GuyWithField();

            var field = typeof(GuyWithField).GetField("Id");

            var getter = LambdaBuilder.GetField<GuyWithField, Guid>(field);

            getter(guy).ShouldBe(guy.Id);
        }

        [Fact]
        public void can_build_setter_for_property()
        {
            var target = new Target { Number = 5 };
            var prop = ReflectionHelper.GetProperty<Target>(x => x.Number);

            var setter = LambdaBuilder.SetProperty<Target, int>(prop);

            setter(target, 11);


            target.Number.ShouldBe(11);
        }


        [Fact]
        public void can_build_setter_for_field()
        {
            var guy = new GuyWithField();

            var field = typeof(GuyWithField).GetField("Id");

            var setter = LambdaBuilder.SetField<GuyWithField, Guid>(field);


            var newGuid = Guid.NewGuid();

            setter(guy, newGuid);

            guy.Id.ShouldBe(newGuid);
        }


        [Fact]
        public void can_set_a_private_id()
        {
            var member = ReflectionHelper.GetProperty<UserWithPrivateId>(x => x.Id);
            var setter = LambdaBuilder.Setter<UserWithPrivateId, Guid>(member);

            var newGuid = Guid.NewGuid();
            var userWithPrivateId = new UserWithPrivateId();

            setter(userWithPrivateId, newGuid);

            userWithPrivateId.Id.ShouldBe(newGuid);
        }

        public class UserWithPrivateId
        {
            public Guid Id { get; private set; }

            public string UserName { get; set; }
        }

    }
}