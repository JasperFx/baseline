using System;
using System.Linq;
using System.Linq.Expressions;
using Baseline.Expressions;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Expressions
{
    public class FindMembersTester
    {
        [Fact]
        public void find_a_single_property()
        {
            Expression<Func<Target, object>> expression = x => x.Color;
            FindMembers.Determine(expression).Single().Name.ShouldBe(nameof(Target.Color));
        }
        
        [Fact]
        public void find_a_single_field()
        {
            Expression<Func<Target, object>> expression = x => x.StringField;
            FindMembers.Determine(expression).Single().Name.ShouldBe(nameof(Target.StringField));
        }

        [Fact]
        public void find_multiple_properties()
        {
            Expression<Func<Target, object>> expression = x => x.Inner.Color;
            var members = FindMembers.Determine(expression);
            members.Select(x => x.Name).ShouldHaveTheSameElementsAs("Inner", "Color");
        }
    }
}