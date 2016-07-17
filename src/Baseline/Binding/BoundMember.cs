using System;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public abstract class BoundMember : IBoundMember
    {
        protected BoundMember(Type memberType, MemberInfo member)
        {
            Member = member;
            MemberType = memberType;
        }

        public MemberInfo Member { get; }
        public Type MemberType { get; }

        public Expression ToBindExpression(ParameterExpression target, ParameterExpression source,
            Conversions conversions)
        {
            var value = fetchValue(source, conversions, MemberType, Member.Name);
            if (value == null) return null;

            var setter = toSetter(target, value);

            var condition = Expression.Call(source, BindingExpressions.DataSourceHas, Expression.Constant(Member.Name));

            return Expression.IfThen(condition, setter);
        }

        protected abstract Expression toSetter(ParameterExpression target, Expression value);

        private static Expression fetchValue(ParameterExpression source, Conversions conversions, Type memberType,
            string memberName)
        {
            Expression value = Expression.Call(source, BindingExpressions.DataSourceGet, Expression.Constant(memberName));
            if (memberType == typeof(string))
            {
                return value;
            }

            var func = conversions.FindConverter(memberType);
            if (func == null)
            {
                return null;
            }


            value = Expression.Invoke(Expression.Constant(func), value);
            value = Expression.Convert(value, memberType);

            return value;
        }
    }
}