using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public class BoundField : BoundMember
    {
        private readonly FieldInfo _field;

        public BoundField(FieldInfo field) : base(field.FieldType, field)
        {
            _field = field;
        }

        public static IEnumerable<IBoundMember> FindMembers(Type type, Conversions conversions)
        {
            var allFields = type.GetFields().Where(x => x.IsPublic).ToArray();

            var simples = allFields
                .Where(x => conversions.Has(x.FieldType))
                .Select(x => new BoundField(x));

            var nested =
                allFields.Where(x => !conversions.Has(x.FieldType) && x.FieldType.IsConcreteWithDefaultCtor())
                .Select(x => typeof(NestedFieldMember<>).CloseAndBuildAs<IBoundMember>(conversions, x, x.FieldType));

            return simples.Concat(nested);
        }

        protected override Expression toSetter(Expression target, Expression value)
        {
            var fieldExpression = Expression.Field(target, _field);
            return Expression.Assign(fieldExpression, value);
        }
    }
}