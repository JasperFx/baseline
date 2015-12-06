using System.Reflection;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class MethodValueGetterTester
    {
        private readonly MethodInfo TheMethodInfo = ReflectionHelper.GetMethod<TestSubject>(x => x.Value());
        private readonly Accessor TheAccessor = ReflectionHelper.GetAccessor<TestSubject>(x => x.Value());
        private readonly Accessor TheArgAccessor = ReflectionHelper.GetAccessor<TestSubject>(x => x.AnotherMethod("Test"));
        private readonly MethodInfo TheArgMethodInfo = ReflectionHelper.GetMethod<TestSubject>(x => x.AnotherMethod("Test"));

        [Fact]
        public void hashcode_should_not_eq_zero()
        {
            TheAccessor.GetHashCode().ShouldNotBe(0);
        }

        [Fact]
        public void should_return_methodinfo_hash()
        {
            var expectedHash = TheMethodInfo.GetHashCode();
            TheAccessor.GetHashCode().ShouldBe(expectedHash);
        }

        [Fact]
        public void with_arguments_should_get_correct_hashcode()
        {
            var actual = TheArgAccessor.GetHashCode();
            var expectedHash = (TheArgMethodInfo.GetHashCode() * 397) ^ ("Test".GetHashCode());
            actual.ShouldBe(expectedHash);
        }
    }

    public class TestSubject
    {
        public object Value()
        {
            return new object();
        }

        public object AnotherMethod(string arg1)
        {
            return arg1;
        }
    }
}
