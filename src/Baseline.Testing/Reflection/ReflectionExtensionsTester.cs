using System;
using System.Linq.Expressions;
using Baseline.Reflection;
using Rhino.Mocks;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class ReflectionExtensionsTester
    {
        public class PropertyHolder{
            public int Age { get; set; }}
        public interface ICallback{void Callback();}

        private ICallback _callback;
        private Expression<Func<PropertyHolder, object>> _expression;
        private ICallback _uncalledCallback;

        public ReflectionExtensionsTester()
        {
            _expression = ph => ph.Age;
            _callback = MockRepository.GenerateStub<ICallback>();
            _uncalledCallback = MockRepository.GenerateStub<ICallback>();
        }


        [Fact]
        public void get_name_returns_expression_property_name()
        {
            _expression.GetName().ShouldBe("Age");
        }

        [Fact]
        public void ifPropertyTypeIs_invokes_method()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IfPropertyTypeIs<int>(_callback.Callback);
            _callback.AssertWasCalled(c=>c.Callback());
            accessor.IfPropertyTypeIs<PropertyHolder>(_uncalledCallback.Callback);
            _uncalledCallback.AssertWasNotCalled(c=>c.Callback());
        }

        [Fact]
        public void isInteger_returns_if_accessor_property_type_is_int()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IsInteger().ShouldBeTrue();
        }
    }
}