using System;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public interface IBoundMember
    {
        MemberInfo Member { get; }

        Type MemberType { get; }

        Expression ToBindExpression(ParameterExpression target, ParameterExpression source, ParameterExpression log, Conversions conversions);
    }
}