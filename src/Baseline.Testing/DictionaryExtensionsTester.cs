using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class DictionaryExtensionsTester
    {
        private Dictionary<string, int> dictionary = new Dictionary<string, int>()
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
            };

        [Fact]
        public void get_a_dictionary_value_that_exists()
        {
            dictionary.Get("a").ShouldBe(1);
        }

        [Fact]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value()
        {
            dictionary.Get("d").ShouldBe(default(int));
        }

        [Fact]
        public void get_a_dictionary_value_that_exists_2()
        {
            dictionary.Get("a", 5).ShouldBe(1);
        }


        [Fact]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value_2()
        {
            dictionary.Get("d", 5).ShouldBe(5);
        }

        [Fact]
        public void child()
        {
            var dict = new Dictionary<string, object>();
            var child = new Dictionary<string, object>();
            child.Add("a1", 1);

            dict.Add("child1", child);

            dict.Child("child1").ShouldBeSameAs(child);


        }

        [Fact]
        public void get_simple()
        {
            var dict = new Dictionary<string, object>{
                {"a", 1},
                {"b", true},
                {"c", false}
            };

            dict.Get<int>("a").ShouldBe(1);
            dict.Get<bool>("b").ShouldBeTrue();
            dict.Get<bool>("c").ShouldBeFalse();
        }

        [Fact]
        public void get_complex()
        {
            var leaf = new Dictionary<string, object>{
                {"a", 1},
                {"b", true},
                {"c", false}
            };

            var node = new Dictionary<string, object>{
                {"leaf", leaf}
            };

            var top = new Dictionary<string, object>{
                {"node", node}
            };

            top.Get<int>("node/leaf/a").ShouldBe(1);
            top.Get<bool>("node/leaf/b").ShouldBeTrue();
            top.Get<bool>("node/leaf/c").ShouldBeFalse();
        }

    }
}