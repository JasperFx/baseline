using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class PropertyChainTester
    {
        public PropertyChainTester()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            PropertyInfo second = ReflectionHelper.GetProperty<ChildTarget>(x => x.GrandChild);
            PropertyInfo birthday = ReflectionHelper.GetProperty<GrandChildTarget>(x => x.BirthDay);

            _chain = new PropertyChain(new[]
                                       {
                                           new PropertyValueGetter(top),
                                           new PropertyValueGetter(second),
                                           new PropertyValueGetter(birthday),

                                       });
        }


        [Fact]
        public void prepend_property()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            var accessor = ReflectionHelper.GetAccessor<ChildTarget>(x => x.GrandChild.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target(){
                Child = new ChildTarget(){
                    GrandChild = new GrandChildTarget(){
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldBe(new DateTime(1974, 1, 1));
        }

        [Fact]
        public void prepend_accessor()
        {
            var top = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            var accessor = ReflectionHelper.GetAccessor<ChildTarget>(x => x.GrandChild.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target()
            {
                Child = new ChildTarget()
                {
                    GrandChild = new GrandChildTarget()
                    {
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldBe(new DateTime(1974, 1, 1));
        }

        [Fact]
        public void prepend_accessor_2()
        {
            var top = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild);
            var accessor = ReflectionHelper.GetAccessor<GrandChildTarget>(x => x.BirthDay);
            var prependedAccessor = accessor.Prepend(top);

            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "GrandChild", "BirthDay");

            prependedAccessor.ShouldBeOfType<PropertyChain>();

            var target = new Target()
            {
                Child = new ChildTarget()
                {
                    GrandChild = new GrandChildTarget()
                    {
                        BirthDay = new DateTime(1974, 1, 1)
                    }
                }
            };

            prependedAccessor.GetValue(target).ShouldBe(new DateTime(1974, 1, 1));
        }

        private PropertyChain _chain;

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
        }

        public class ChildTarget
        {
            public ChildTarget()
            {
                GrandChildren = new List<GrandChildTarget>();
            }

            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }
            public IList<GrandChildTarget> GrandChildren { get; set; }
            public GrandChildTarget[] GrandChildren2 { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
        }

        [Fact]
        public void to_expression_test()
        {
            var target = new Target{
                Child = new ChildTarget(){
                    GrandChild = new GrandChildTarget(){
                        Name = "Jessica"
                    },
                    SecondGrandChild = new GrandChildTarget(){
                        Name = "Natalie"
                    }
                }
            };

            ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.Name)
                .ToExpression<Target>().Compile()(target)
                .ShouldBe("Jessica");

            ReflectionHelper.GetAccessor<Target>(x => x.Child.SecondGrandChild.Name)
                .ToExpression<Target>().Compile()(target)
                .ShouldBe("Natalie");
        }

        [Fact]
        public void to_expression_test_with_a_value_type()
        {
            var target = new Target
                         {
                             Child = new ChildTarget()
                                     {
                                         GrandChild = new GrandChildTarget()
                                                      {
                                                          Number = 1
                                                      },
                                         SecondGrandChild = new GrandChildTarget()
                                                            {
                                                                Number = 2
                                                            }
                                     }
                         };

            ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.Number)
                .ToExpression<Target>().Compile()(target)
                .ShouldBe(1);

            ReflectionHelper.GetAccessor<Target>(x => x.Child.SecondGrandChild.Number)
                .ToExpression<Target>().Compile()(target)
                .ShouldBe(2);
        }

        [Fact]
        public void property_chain_equals()
        {
            Accessor chain1 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain2 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain3 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild);
            Accessor chain4 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            Accessor chain5 = ReflectionHelper.GetAccessor<Target>(t => t.Child.SecondGrandChild.BirthDay);
            
            chain1.ShouldBe(chain2);
            chain1.ShouldNotBe(chain3);
            chain1.ShouldNotBe(chain4);
            chain1.ShouldNotBe(chain5);
            chain1.Equals(null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)chain1).ShouldBeTrue();
            ((PropertyChain)chain1).Equals(1).ShouldBeFalse();
            chain1.ShouldBe(chain1);

        }

        [Fact]
        public void propertyChain_hashcode()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.GetHashCode().ShouldNotBe(0);
        }

        [Fact]
        public void propertyChain_can_get_inner_property()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.InnerProperty.ShouldBeSameAs(typeof(ChildTarget).GetProperty("Age"));
        }

        [Fact]
        public void propertyChain_can_get_child_accessor()
        {
            Accessor expected = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldBe(expected);
        }

        [Fact]
        public void propertyChain_can_get_child_accessor_from_indexer()
        {
            var expected = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChildren[1].Name);
            var child = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChildren[1])
                                        .GetChildAccessor<GrandChildTarget>(x => x.Name);

            child.ShouldBe(expected);
        }

        [Fact]
        public void propertyChain_can_get_child_accessor_from_array_indexer()
        {
            var expected = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChildren2[1].Name);
            var child = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChildren2[1])
                                        .GetChildAccessor<GrandChildTarget>(x => x.Name);

            child.ShouldBe(expected);
        }

        [Fact]
        public void propertyChain_can_get_the_name()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay).Name.ShouldBe(
                "ChildGrandChildBirthDay");
        }

        [Fact]
        public void propertyChain_can_get_owner_type()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).OwnerType.ShouldBe(typeof(ChildTarget));
        }

        [Fact]
        public void propertyChain_can_get_properties_names()
        {
            string[] names = ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).PropertyNames;
            names.Count().ShouldBe(2);
            names.ShouldContain(n => n == "Child");
            names.ShouldContain(n => n == "Age");
        }

        [Fact]
        public void propertyChain_toString_returns_graph()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name.Length).
                ToString().ShouldBe("Baseline.Testing.Reflection.PropertyChainTester+TargetChild.GrandChild.Name");
        }

        [Fact]
        public void PropertyChainCanGetPropertyHappyPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChild = new GrandChildTarget
                                                      {
                                                          BirthDay = DateTime.Today
                                                      }
                                     }
                         };
            _chain.GetValue(target).ShouldBe(DateTime.Today);
        }

        [Fact]
        public void PropertyChainCanSetPRopertyHappyPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChild = new GrandChildTarget
                                                      {
                                                          BirthDay = DateTime.Today
                                                      }
                                     }
                         };
            _chain.SetValue(target, DateTime.Today.AddDays(1));

            target.Child.GrandChild.BirthDay.ShouldBe(DateTime.Today.AddDays(1));
        }

        [Fact]
        public void PropertyChainGetPropertyReturnsNullForSadPath()
        {
            var target = new Target
                         {
                             Child = new ChildTarget()
                         };
            _chain.GetValue(target).ShouldBeNull();
        }

        [Fact]
        public void PropertyChainReturnsInnerMostFieldName()
        {
            _chain.FieldName.ShouldBe("BirthDay");
        }

        [Fact]
        public void PropertyChainReturnsInnerMostPropertyType()
        {
            _chain.PropertyType.ShouldBe(typeof(DateTime));
        }

        [Fact]
        public void CollectionIndexingPropertyAccessWorks()
        {
            Expression<Func<Target, object>> expression = x => x.Child.GrandChildren[0].Name;
            var accessor = expression.ToAccessor();

            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChildren = { new GrandChildTarget { Name = "Bob" } }
                                     }
                         };

            accessor.GetValue(target).ShouldBe("Bob");
            accessor.Name.ShouldBe("ChildGrandChildren[0]Name");
        }

        [Fact]
        public void ArrayIndexingPropertyAccessWorks()
        {
            Expression<Func<Target, object>> expression = x => x.Child.GrandChildren2[0].Name;
            var accessor = expression.ToAccessor();

            var target = new Target
                         {
                             Child = new ChildTarget
                                     {
                                         GrandChildren2 = new[] { new GrandChildTarget { Name = "Bob" } }
                                     }
                         };

            accessor.GetValue(target).ShouldBe("Bob");
            accessor.Name.ShouldBe("ChildGrandChildren2[0]Name");
        }
    }
}