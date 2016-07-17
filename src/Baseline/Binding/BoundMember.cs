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

        public Expression ToBindExpression(Expression target, Expression source, Expression log, Conversions conversions)
        {
            var value = fetchValue(source, conversions, MemberType, Member.Name);
            if (value == null) return null;

            var setter = toSetter(target, value);

            var ex = Expression.Variable(typeof(Exception), "ex");

            var logProblem = Expression.Invoke(log, Expression.Constant(Member), ex);

            var catcher = Expression.Catch(ex, Expression.Block(logProblem, Expression.Default(setter.Type)), null);
            var tryCatch = Expression.TryCatch(setter, catcher);

            var condition = Expression.Call(
                source, 
                BindingExpressions.DataSourceHas, 
                Expression.Constant(Member.Name));



            return Expression.IfThen(condition, tryCatch);
        }

        protected abstract Expression toSetter(Expression target, Expression value);

        private static Expression fetchValue(Expression source, Conversions conversions, Type memberType,
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