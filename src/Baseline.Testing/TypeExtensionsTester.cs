using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public interface IService<T>
    {
    }

    
    public class TypeExtensionsTester
    {
        public class Service1 : IService<string>
        {
        }

        public class Service2
        {
        }

        public class Service2<T> : IService<T>
        {
        }

        public class Service3 : Service3<string>
        {   
        }

        public class Service3<T>
        {
        }

        public interface ServiceInterface : IService<string>
        {
        }

        [Fact]
        public void closes_applies_to_concrete_types()
        {
            typeof(Service2<string>).Closes(typeof(Service2<>)).ShouldBeTrue();
        }

        [Fact]
        public void closes_applies_to_implementing_an_open_interface()
        {
            typeof (ConcreteListener).Closes(typeof (IListener<>)).ShouldBeTrue();
        }

        [Fact]
        public void closes_applies_to_concrete_non_generic_types()
        {
            typeof(Service2).Closes(typeof(Service2<>)).ShouldBeFalse();
        }

        [Fact]
        public void closes_applies_to_concrete_non_generic_types_with_generic_parent()
        {
            typeof(Service3).Closes(typeof(Service3<>)).ShouldBeTrue();
            typeof(Service3).Closes(typeof(Service2<>)).ShouldBeFalse();
        }

        [Fact]
        public void find_interface_that_closes_open_interface_does_not_crater_on_Object()
        {
            // And yes, this was apparently necessary
            typeof(object).FindInterfaceThatCloses(typeof(IService<>))
                .ShouldBeNull();
        }

        [Fact]
        public void find_interface_that_closes_open_interface()
        {
            typeof (Service1).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldBe(typeof (IService<string>));

            typeof (Service2).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldBeNull();

            typeof(IService<>).FindInterfaceThatCloses(Arg.Any<Type>()).ShouldBeNull();
        }

        [Fact]
        public void find_parameter_type_to()
        {
            typeof (Service1).FindParameterTypeTo(typeof (IService<>)).ShouldBe(typeof (string));
            typeof (Service2).FindParameterTypeTo(typeof (IService<>)).ShouldBeNull();


        }

        [Fact]
        public void find_interface_that_closes_open_interface_from_another_interface()
        {
            typeof (TestHandler).FindInterfaceThatCloses(typeof (IMessageHandler<>)).ShouldBe(
                typeof (IMessageHandler<string>));
        }

        public interface IMessageHandler<T>{}
        public interface TestHandler : IMessageHandler<string>{}

        [Fact]
        public void implements_interface_template()
        {
            typeof (Service1).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeTrue();

            typeof (Service2).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();

            typeof (ServiceInterface).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();
        }

        [Fact]
        public void IsPrimitive()
        {
            Assert.IsTrue(typeof (int).IsPrimitive());
            Assert.IsTrue(typeof (bool).IsPrimitive());
            Assert.IsTrue(typeof (double).IsPrimitive());
            Assert.IsFalse(typeof (string).IsPrimitive());
            Assert.IsFalse(typeof (OutputModel).IsPrimitive());
            Assert.IsFalse(typeof (IRouteVisitor).IsPrimitive());
        }

        public class OutputModel
        {
            
        }

        public interface IRouteVisitor{}

        public enum DoNext{}

        [Fact]
        public void IsSimple()
        {
            Assert.IsTrue(typeof (int).IsSimple());
            Assert.IsTrue(typeof (bool).IsSimple());
            Assert.IsTrue(typeof (double).IsSimple());
            Assert.IsTrue(typeof (string).IsSimple());
            Assert.IsTrue(typeof (DoNext).IsSimple());
            Assert.IsFalse(typeof (IRouteVisitor).IsSimple());
        }

        [Fact]
        public void IsString()
        {
            Assert.IsTrue(typeof (string).IsString());
            Assert.IsFalse(typeof (int).IsString());
        }

        [Fact]
        public void is_nullable_of_T()
        {
            typeof(string).IsNullableOfT().ShouldBeFalse();
            typeof(Nullable<int>).IsNullableOfT().ShouldBeTrue();
        }

        [Fact]
        public void is_nullable_of_T_when_type_is_null()
        {
            Type type = null;
            type.IsNullableOfT().ShouldBeFalse();
        }

        [Fact]
        public void is_nullable_of_a_given_type_when_type_is_null()
        {
            Type type = null;
            type.IsNullableOf(typeof(int)).ShouldBeFalse();
        }

        [Fact]
        public void is_nullable_of_a_given_type()
        {
            typeof(string).IsNullableOf(typeof(int)).ShouldBeFalse();
            typeof(Nullable<DateTime>).IsNullableOf(typeof(int)).ShouldBeFalse();
            typeof(Nullable<int>).IsNullableOf(typeof(int)).ShouldBeTrue();
        }

        [Fact]
        public void is_type_or_nullable_of_T()
        {
            typeof(bool).IsTypeOrNullableOf<bool>().ShouldBeTrue();
            typeof(Nullable<bool>).IsTypeOrNullableOf<bool>().ShouldBeTrue();
            typeof(Nullable<DateTime>).IsTypeOrNullableOf<bool>().ShouldBeFalse();
            typeof(string).IsTypeOrNullableOf<bool>().ShouldBeFalse();
        
        }

        [Fact]
        public void can_be_cast_to()
        {
            typeof(Message1).CanBeCastTo<IMessage>().ShouldBeTrue();
            typeof(Message2).CanBeCastTo<IMessage>().ShouldBeTrue();
            typeof(Message2).CanBeCastTo<Message1>().ShouldBeTrue();

            typeof(Message1).CanBeCastTo<Message1>().ShouldBeTrue();
            typeof(Message1).CanBeCastTo<Message2>().ShouldBeFalse();
            ((Type)null).CanBeCastTo<Message1>().ShouldBeFalse();
        }

        [Fact]
        public void is_in_namespace()
        {
            this.GetType().IsInNamespace("wrong").ShouldBeFalse();
            this.GetType().IsInNamespace(this.GetType().Namespace + ".something").ShouldBeFalse();
            this.GetType().IsInNamespace(this.GetType().Namespace).ShouldBeTrue();
            this.GetType().IsInNamespace(this.GetType().GetTypeInfo().Assembly.GetName().Name).ShouldBeTrue();

            Type type = null;
            type.IsInNamespace("anything").ShouldBeFalse();
        }

        [Fact]
        public void is_open_generic()
        {
            typeof(int).IsOpenGeneric().ShouldBeFalse();
            typeof(Nullable<>).IsOpenGeneric().ShouldBeTrue();
            typeof(Nullable<int>).IsOpenGeneric().ShouldBeFalse();

            Type type = null;
            type.IsOpenGeneric().ShouldBeFalse();
        }

        [Fact]
        public void is_concrete_type_of_T()
        {
            typeof(IMessage).IsConcreteTypeOf<IMessage>().ShouldBeFalse();
            typeof(AbstractMessage).IsConcreteTypeOf<IMessage>().ShouldBeFalse();
            typeof(Message1).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            typeof(Message2).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            typeof(Message3).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            this.GetType().IsConcreteTypeOf<IMessage>().ShouldBeFalse();

            Type type = null;
            type.IsConcreteTypeOf<IMessage>().ShouldBeFalse();
        }

        [Fact]
        public void is_nullable()
        {
            typeof(string).IsNullable().ShouldBeFalse();
            typeof(Nullable<int>).IsNullable().ShouldBeTrue();
        }

        [Fact]
        public void get_inner_type_from_nullable()
        {
            typeof (Nullable<int>).GetInnerTypeFromNullable().ShouldBe(typeof (int));
        }

        [Fact]
        public void get_name_from_generic()
        {
            typeof(Service2<int>).GetName().ShouldBe("Service2`1<Int32>");
        }

        [Fact]
        public void get_full_name_from_generic()
        {
            typeof(Service2<int>).GetFullName().ShouldBe("Service2`1<Int32>");
        }

        [Fact]
        public void get_full_name()
        {
            typeof (string).GetFullName().ShouldBe("System.String");
        }

        [Fact]
        public void is_concrete()
        {
            typeof(IMessage).IsConcrete().ShouldBeFalse();
            typeof(AbstractMessage).IsConcrete().ShouldBeFalse();
            typeof(Message2).IsConcrete().ShouldBeTrue();

            Type type = null;
            type.IsConcrete().ShouldBeFalse();
        }

        [Fact]
        public void is_not_concrete()
        {
            typeof(Message2).IsNotConcrete().ShouldBeFalse();
        }

        [Fact]
        public void close_and_build_as()
        {
            var message = typeof (OpenClass<>).CloseAndBuildAs<IMessage>(typeof (string));
            message.ShouldBeOfType<OpenClass<string>>();
            message.ShouldNotBeNull();
        }

        [Fact]
        public void is_concrete_with_default_ctor()
        {
            typeof(Message1).IsConcreteWithDefaultCtor().ShouldBeTrue();
            typeof(IMessage).IsConcreteWithDefaultCtor().ShouldBeFalse();

            typeof(ClassWithGreedyCtor).IsConcreteWithDefaultCtor().ShouldBeFalse();
        }

        [Fact]
        public void is_a_collection_of_should_handle_arrays()
        {
            var arrayType = typeof (string[]);
            arrayType.IsAnEnumerationOf().ShouldBe(typeof (string));
        }

        [Fact]
        public void is_a_collection_of_should_handle_generic_collections()
        {
            var arrayType = typeof (List<string>);
            arrayType.IsAnEnumerationOf().ShouldBe(typeof (string));
            
        }

        public interface IMessage{}
        public abstract class AbstractMessage : IMessage{}
        public class Message3 : AbstractMessage{}
        public class Message1 : IMessage{}
        public class Message2 : Message1{}

        public interface IListener<T>{}
        public class ConcreteListener : IListener<string>{}
    
        public class ClassWithGreedyCtor
        {
            public ClassWithGreedyCtor(string name)
            {
                Debug.WriteLine(name);
            }
        }
        

        public class OpenClass<T> : IMessage{}
    }

    
}