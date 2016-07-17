using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public class BoundProperty : BoundMember
    {
        private readonly PropertyInfo _property;

        public BoundProperty(PropertyInfo property) : base(property.PropertyType, property)
        {
            _property = property;
        }

        public static IEnumerable<IBoundMember> FindMembers(Type type, Conversions conversions)
        {
            return type.GetProperties()
                .Where(x => x.CanWrite)
                .Where(x => conversions.Has(x.PropertyType))
                .Select(x => new BoundProperty(x));
        }

        protected override Expression toSetter(ParameterExpression target, Expression value)
        {
            var method = _property.SetMethod;

            return Expression.Call(target, method, value);
        }
    }
}