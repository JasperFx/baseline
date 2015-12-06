using System.Linq;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class AccessorRulesTester
    {
        [Fact]
        public void add_and_retrieve_rules_by_accessor()
        {
            var rules = new AccessorRules();
            rules.Add<Target1>(x => x.Name, new FooRule());
            rules.Add<Target2>(x => x.Name, new BarRule());
        
            rules.Add<Target1>(x => x.Age, new ColorRule("red"));
            rules.Add<Target2>(x => x.Age, new ColorRule("green"));

            rules.FirstRule<Target1, IRule>(x => x.Name).ShouldBeOfType<FooRule>();
            rules.FirstRule<Target2, IRule>(x => x.Name).ShouldBeOfType<BarRule>();

            rules.FirstRule<Target1, IRule>(x => x.Age).ShouldBeOfType<ColorRule>().Color.ShouldBe("red");
            rules.FirstRule<Target2, IRule>(x => x.Age).ShouldBeOfType<ColorRule>().Color.ShouldBe("green");
        }

        [Fact]
        public void returns_all_rules_of_a_type()
        {
            var rules = new AccessorRules();
            rules.Add<Target1>(x => x.Name, new ColorRule("red"));
            rules.Add<Target1>(x => x.Name, new ColorRule("green"));
            rules.Add<Target1>(x => x.Name, new ColorRule("blue"));
            rules.Add<Target1>(x => x.Name, new FooPolicy());
            rules.Add<Target1>(x => x.Name, new BarPolicy());
        
            rules.AllRulesFor<Target1, IRule>(x => x.Name).ShouldHaveTheSameElementsAs(
                new ColorRule("red"), new ColorRule("green"), new ColorRule("blue"));
        }

        [Fact]
        public void rule_registration_is_idempotent()
        {
            var rules = new AccessorRules();
            rules.Add<Target1>(x => x.Age, new ColorRule("red"));
            rules.Add<Target1>(x => x.Age, new ColorRule("red"));
            rules.Add<Target1>(x => x.Age, new ColorRule("red"));

            rules.AllRulesFor<Target1, IRule>(x => x.Age)
                .Single().ShouldBe(new ColorRule("red"));
        }
    }

    public class Target1
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Target2
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public interface IRule
    {
        
    }

    public class ColorRule : IRule
    {
        private readonly string _color;

        public ColorRule(string color)
        {
            _color = color;
        }

        public string Color
        {
            get { return _color; }
        }

        protected bool Equals(ColorRule other)
        {
            return string.Equals(_color, other._color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColorRule) obj);
        }

        public override int GetHashCode()
        {
            return (_color != null ? _color.GetHashCode() : 0);
        }
    }

    public class FooRule : IRule
    {
        
    }

    public class BarRule : IRule
    {
        
    }

    public interface IPolicy
    {
        
    }

    public class FooPolicy : IPolicy
    {
        
    }

    public class BarPolicy : IPolicy
    {
        
    }
}