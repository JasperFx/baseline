using System;
using System.Linq.Expressions;
using Baseline.Reflection;
using NSubstitute;
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
            _callback = Substitute.For<ICallback>();
            _uncalledCallback = Substitute.For<ICallback>();
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
            _callback.Received().Callback();
            accessor.IfPropertyTypeIs<PropertyHolder>(_uncalledCallback.Callback);
            _uncalledCallback.DidNotReceive().Callback();
        }

        [Fact]
        public void isInteger_returns_if_accessor_property_type_is_int()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IsInteger().ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(SimpleClass), true)]
        [InlineData(typeof(Simple2Class), true)]
        [InlineData(typeof(SimpleClass3), false)]
        public void has_default_constructor(Type target, bool hasCtor)
        {
            target.HasDefaultConstructor().ShouldBe(hasCtor);
        }
        
        
        
        public class SimpleClass{}

        public class Simple2Class
        {
            public Simple2Class(string name)
            {
                
                
            }

            public Simple2Class()
            {
            }
        }

        public class SimpleClass3
        {
            public SimpleClass3(string name)
            {
            }
        }

        [Fact]
        public void is_anonymous_type()
        {
            var o = new {Name = "Miller"};
            o.IsAnonymousType().ShouldBeTrue();
            
            "who?".IsAnonymousType().ShouldBeFalse();
            string data = null;
            data.IsAnonymousType().ShouldBeFalse();
        }
    }
}