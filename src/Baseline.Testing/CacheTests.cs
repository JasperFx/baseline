using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public class CacheTests
    {
        private Cache<string, int> cache = new Cache<string, int>();
        private const string Key = "someKey";
        public interface ICallback
        {
            string GetKeyCallback(int value);
            void OnAdditionCallback(int value);
        }

        [Fact]
        public void non_default_fill()
        {
            var count = 0;

            cache.OnMissing = key => ++count;

            cache.FillDefault("number");
            cache["number"].ShouldBe(1);

            cache.Fill("diff", key => 1000);

            cache["diff"].ShouldBe(1000);
        }

        [Fact]
        public void on_addition_should_fire_when_a_cache_adds_something_from_its_on_missing_catch()
        {
            var list = new List<int>();
            int x = 0;

            cache.OnMissing = key => ++x;

            cache.OnAddition = number => list.Add(number);

            cache["a"] = 100;
            cache["b"].ShouldBe(1);
            cache["c"].ShouldBe(2);
        
            list.ShouldHaveTheSameElementsAs(100, 1, 2);
        }

        [Fact]
        public void when_GetKey_not_set_should_throw()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => cache.GetKey(2));
        }

        [Fact]
        public void when_key_not_found_should_throw_by_default()
        {
            const string key = "nonexisting key";

            Exception<KeyNotFoundException>.ShouldBeThrownBy(() => cache[key].ShouldBe(0)).
                Message.ShouldBe("Key '{0}' could not be found".ToFormat(key));
        }

        [Fact]
        public void get_all_keys()
        {
            cache.Fill(Key, 42);
            cache.GetAllKeys().Count().ShouldBe(1);
            cache.GetAllKeys().ShouldContain(Key);
        }

        [Fact]
        public void get_enumerator()
        {
            cache.Fill(Key, 42);
            cache.GetEnumerator().ShouldBeAssignableTo<IEnumerator<int>>();
            IEnumerable enumerable = cache;
            enumerable.GetEnumerator().ShouldBeAssignableTo<IEnumerator>();
            cache.Count().ShouldBe(1);
            cache.ShouldContain(42);
        }

        [Fact]
        public void set_GetKey()
        {
            ICallback callback = Substitute.For<ICallback>();
            cache.GetKey = callback.GetKeyCallback;
            cache.GetKey(42);
            callback.Received().GetKeyCallback(42);
        }

        [Fact]
        public void set_OnAddition()
        {
            ICallback callback = Substitute.For<ICallback>();
            cache["firstKey"] = 0;
            callback.DidNotReceive().OnAdditionCallback(42);
            //callback.AssertWasNotCalled(c => c.OnAdditionCallback(42));
            cache.OnAddition = callback.OnAdditionCallback;
            cache[Key] = 42;
            callback.Received().OnAdditionCallback(42);
            //callback.AssertWasCalled(c=>c.OnAdditionCallback(42));
        }

        [Fact]
        public void can_remove()
        {
            cache[Key] = 42;
            cache.GetAllKeys().ShouldContain(Key);
            cache.Remove(Key);
            cache.GetAllKeys().ShouldNotContain(Key);
        }

        [Fact]
        public void store_and_fetch()
        {
            cache["a"] = 1;
            cache["a"].ShouldBe(1);

            cache["a"] = 2;
            cache["a"].ShouldBe(2);
        }

        [Fact]
        public void test_the_on_missing()
        {
            int count = 0;
            cache.OnMissing = key => ++count;


            cache["a"].ShouldBe(1);
            cache["b"].ShouldBe(2);
            cache["c"].ShouldBe(3);

            cache["a"].ShouldBe(1);
            cache["b"].ShouldBe(2);
            cache["c"].ShouldBe(3);

            cache.Count.ShouldBe(3);
        }

        [Fact]
        public void fill_only_writes_if_there_is_not_previous_value()
        {
            cache.Fill("a", 1);
            cache["a"].ShouldBe(1);

            cache.Fill("a", 2);
            cache["a"].ShouldBe(1); // did not overwrite
        }


    }
}