using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Reflection
{
    public class ReflectionHelperTester
    {
        public ReflectionHelperTester()
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
            _expression = (t => t.Child);
        }


        private PropertyChain _chain;
        private Expression<Func<Target, ChildTarget>> _expression;

        [Theory]
        [InlineData(typeof(Target), "Target")]
        [InlineData(typeof(Dictionary<Target, ChildTarget>), "Dictionary<Target,ChildTarget>")]
        public void GetPrettyName(Type type, string expected)
        {
            type.GetPrettyName().ShouldBe(expected);
        }

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
        }

        public class ChildTarget
        {
            public ChildTarget()
            {
                Grandchildren = new List<GrandChildTarget>();
            }

            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }

            public IList<GrandChildTarget> Grandchildren { get; set; }
            public GrandChildTarget[] Grandchildren2 { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
            public DeepTarget Deep { get; set; }
        }

        public class DeepTarget
        {
            public string Color { get; set; }
        }

        public class SomeClass
        {
            public object DoSomething()
            {
                return null;
            }

            public object DoSomething(int i, int j)
            {
                return null;
            }
        }

        public class ClassConstraintHolder<T> where T : class {}
        public class StructConstraintHolder<T> where T : struct {}
        public class NewConstraintHolder<T> where T : new() {}
        public class NoConstraintHolder<T> {}
        public class NoEmptyCtorHolder { public NoEmptyCtorHolder(bool ctorArg) {} }

        [Fact]
        public void tell_if_type_meets_generic_constraints()
        {
            Type[] arguments = typeof (ClassConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(int)).ShouldBeFalse();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeTrue();
            arguments = typeof (StructConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeFalse();
            arguments = typeof(NewConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(NoEmptyCtorHolder)).ShouldBeFalse();
            arguments = typeof(NoConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeTrue();
        }

        [Fact]
        public void CreateAPropertyChainFromReflectionHelper()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.BirthDay);
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

            accessor.GetValue(target).ShouldBe(DateTime.Today);
        }

        [Fact]
        public void can_get_accessor_from_lambda_expression()
        {
            Accessor accessor = ReflectionHelper.GetAccessor((LambdaExpression)_expression);
            accessor.Name.ShouldBe("Child");
            accessor.DeclaringType.ShouldBe(typeof (Target));
        }

        [Fact]
        public void can_get_member_expression_from_lambda()
        {
            MemberExpression memberExpression = ((LambdaExpression) _expression).GetMemberExpression(false);
            memberExpression.ToString().ShouldBe("t.Child");
        }

        [Fact]
        public void can_get_member_expression_from_convert()
        {
            Expression<Func<Target, object>> convertExpression = t => (object)t.Child;
            convertExpression.GetMemberExpression(false).ToString().ShouldBe("t.Child");
        }

        [Fact]
        public void getMemberExpression_should_throw_when_not_a_member_access()
        {
            Expression<Func<Target, object>> typeAsExpression = t => t.Child as object;
            Exception<ArgumentException>.ShouldBeThrownBy(() => typeAsExpression.GetMemberExpression(true)).Message.ShouldContain("Not a member access");
        }

        [Fact]
        public void DeclaringType_of_a_property_chain_is_the_type_of_the_leftmost_object()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.BirthDay);
            accessor.ShouldBeOfType<PropertyChain>().DeclaringType.ShouldBe(typeof (Target));
        }

        [Fact]
        public void Try_to_fetch_a_method()
        {
            MethodInfo method = ReflectionHelper.GetMethod<SomeClass>(s => s.DoSomething());
            const string expected = "DoSomething";
            method.Name.ShouldBe(expected);

            Expression<Func<object>> doSomething = () => new SomeClass().DoSomething();
            ReflectionHelper.GetMethod(doSomething).Name.ShouldBe(expected);

            Expression doSomethingExpression = Expression.Call(Expression.Parameter(typeof (SomeClass), "s"), method);
            ReflectionHelper.GetMethod(doSomethingExpression).Name.ShouldBe(expected);

            Expression<Func<object>> dlgt = () => new SomeClass().DoSomething();
            ReflectionHelper.GetMethod<Func<object>>(dlgt).Name.ShouldBe(expected);

            Expression<Func<int,int,object>> twoTypeParamDlgt = (n1,n2) => new SomeClass().DoSomething(n1,n2);
            ReflectionHelper.GetMethod(twoTypeParamDlgt).Name.ShouldBe(expected);
        }

        [Fact]
        public void can_get_property()
        {
            Expression<Func<Target, ChildTarget>> expression = t => t.Child;
            const string expected = "Child";
            ReflectionHelper.GetProperty(expression).Name.ShouldBe(expected);

            LambdaExpression lambdaExpression = expression;
            ReflectionHelper.GetProperty(lambdaExpression).Name.ShouldBe(expected);
        }

        [Fact]
        public void GetProperty_should_throw_if_not_property_expression()
        {
            Expression<Func<SomeClass, object>> expression = c => c.DoSomething();


            Exception<ArgumentException>.ShouldBeThrownBy(() => ReflectionHelper.GetProperty(expression)).
                Message.ShouldContain("Not a member access");
        }

        [Fact]
        public void should_tell_if_is_member_expression()
        {
            Expression<Func<Target, ChildTarget>> expression = t => t.Child;
            Expression<Func<Target, object>> memberExpression = t => t.Child;
            ReflectionHelper.IsMemberExpression(expression).ShouldBeTrue();
            ReflectionHelper.IsMemberExpression(memberExpression).ShouldBeTrue();
        }

        [Fact]
        public void TryingToCallSetDoesNotBlowUpIfTheIntermediateChildrenAreNotThere()
        {
            var target = new Target
            {
                Child = new ChildTarget()
            };
            _chain.SetValue(target, DateTime.Today.AddDays(4));
        }




        [Fact]
        public void get_value_by_indexer_when_the_indexer_is_variable_reference()
        {
            var target = new Target{
                Child = new ChildTarget{
                    Grandchildren = new List<GrandChildTarget>{
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Red"
                            }
                        },
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Green"
                            }
                        },
                        new GrandChildTarget{
                            Name = "Third"
                        },
                        new GrandChildTarget{
                            Name = "Fourth"
                        },
                    }
                }
            };

            var i = 0;
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[i].Deep.Color)
                .GetValue(target).ShouldBe("Red");

            i = 2;
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[i].Deep.Color)
                .GetValue(target).ShouldBeNull();


            for (int j = 0; j < target.Child.Grandchildren.Count; j++)
            {
                ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[j].Name)
                    .GetValue(target).ShouldBe(target.Child.Grandchildren[j].Name);
            }
        }

        public class Index
        {
            public int I { get; set; }
            public Index2Info Index2 { get; set; }

            public class Index2Info
            {
                public int J { get; set; }
            }
        }

        [Fact]
        public void get_value_by_indexer_when_the_indexer_is_variable_reference_of_a_complex_object()
        {
            var target = new Target
            {
                Child = new ChildTarget
                {
                    Grandchildren = new List<GrandChildTarget>{
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Red"
                            }
                        },
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Green"
                            }
                        },
                        new GrandChildTarget{
                            Name = "Third"
                        }
                    }
                }
            };

            var index = new Index();
            index.I = 0;

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[index.I].Deep.Color)
                .GetValue(target).ShouldBe("Red");

            index.I = 2;
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[index.I].Deep.Color)
                .GetValue(target).ShouldBeNull();

            for (index.I = 0; index.I < target.Child.Grandchildren.Count; index.I++)
            {
                ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[index.I].Name)
                    .GetValue(target).ShouldBe(target.Child.Grandchildren[index.I].Name);
            }

            index.Index2 = new Index.Index2Info();
            index.Index2.J = 1;

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[index.Index2.J].Deep.Color)
               .GetValue(target).ShouldBe("Green");

        }

        [Fact]
        public void get_value_by_array_indexer_when_the_indexer_is_variable_reference_of_a_complex_object()
        {
            var target = new Target
            {
                Child = new ChildTarget
                {
                    Grandchildren2 = new [] {
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Red"
                            }
                        },
                        new GrandChildTarget{
                            Deep = new DeepTarget{
                                Color = "Green"
                            }
                        },
                        new GrandChildTarget{
                            Name = "Third"
                        }
                    }
                }
            };

            var index = new Index();
            index.I = 0;

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[index.I].Deep.Color)
                .GetValue(target).ShouldBe("Red");

            index.I = 2;
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[index.I].Deep.Color)
                .GetValue(target).ShouldBeNull();

            for (index.I = 0; index.I < target.Child.Grandchildren.Count; index.I++)
            {
                ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[index.I].Name)
                    .GetValue(target).ShouldBe(target.Child.Grandchildren2[index.I].Name);
            }

            index.Index2 = new Index.Index2Info();
            index.Index2.J = 1;

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[index.Index2.J].Deep.Color)
               .GetValue(target).ShouldBe("Green");

        }

        [Fact]
        public void get_owner_type_by_indexer()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[1].Deep.Color);
            accessor.OwnerType.ShouldBe(typeof(DeepTarget));

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[1]).OwnerType.ShouldBe(typeof(ChildTarget));
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[1].Name).OwnerType.ShouldBe(typeof(GrandChildTarget));
        }

        [Fact]
        public void get_owner_type_by_array_indexer()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[1].Deep.Color);
            accessor.OwnerType.ShouldBe(typeof(DeepTarget));

            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[1]).OwnerType.ShouldBe(typeof(ChildTarget));
            ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[1].Name).OwnerType.ShouldBe(typeof(GrandChildTarget));
        }

        [Fact]
        public void get_inner_property_with_method_accessor()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[1]);
            accessor.PropertyType.ShouldBe(typeof (GrandChildTarget));
        }

        [Fact]
        public void get_field_name_by_method_accessor()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren[1]);
            accessor.FieldName.ShouldBe("Grandchildren[1]");
            accessor.Name.ShouldBe("ChildGrandchildren[1]");
        }

        [Fact]
        public void get_field_name_by_index_accessor()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.Grandchildren2[1]);
            accessor.FieldName.ShouldBe("Grandchildren2[1]");
            accessor.Name.ShouldBe("ChildGrandchildren2[1]");
        }
    }

    public class when_using_an_accessor_for_a_method
    {
        [Fact]
        public void can_get_The_name()
        {
            ReflectionHelper.GetAccessor<TargetHolder>(x => x.GetTarget())
                .Name.ShouldBe("GetTarget");
        }

        [Fact]
        public void get_value_simple()
        {
            var accessor = ReflectionHelper.GetAccessor<MethodTarget>(x => x.GetName());

            accessor.GetValue(new MethodTarget("red")).ShouldBe("red");
            accessor.GetValue(new MethodTarget(null)).ShouldBe(null);
        }

        [Fact]
        public void get_value_at_the_endof_a_chain()
        {
            var accessor = ReflectionHelper.GetAccessor<MethodHolder>(x => x.MethodGuy.GetName());

            accessor.GetValue(new MethodHolder()).ShouldBe(null);
            accessor.GetValue(new MethodHolder{
                MethodGuy = new MethodTarget("red")
            }).ShouldBe("red");


        }

        [Fact]
        public void method_is_at_beginning_of_a_chain()
        {
            var accessor = ReflectionHelper.GetAccessor<TargetHolder>(x => x.GetTarget().GetName());
            accessor.GetValue(new TargetHolder(null)).ShouldBe(null);
            accessor.GetValue(new TargetHolder(new MethodTarget(null))).ShouldBeNull();
            accessor.GetValue(new TargetHolder(new MethodTarget("red"))).ShouldBe("red");
        }


        public class TargetHolder
        {
            private readonly MethodTarget _target;

            public TargetHolder(MethodTarget target)
            {
                _target = target;
            }

            public MethodTarget GetTarget()
            {
                return _target;
            }
        }

        public class MethodHolder
        {
            public MethodTarget MethodGuy { get; set; }
        }

        public class MethodTarget
        {
            private readonly string _name;

            public MethodTarget(string name)
            {
                _name = name;
            }

            public string GetName()
            {
                return _name;
            }
        }
    }

}