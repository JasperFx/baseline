using System;
using System.Linq.Expressions;

namespace Baseline.Reflection
{
    public interface IValueGetter
    {
        object GetValue(object target);
        string Name { get; }
        Type DeclaringType { get; }

        Type ValueType { get; }

        Expression ChainExpression(Expression body);
        void SetValue(object target, object propertyValue);
    }
}