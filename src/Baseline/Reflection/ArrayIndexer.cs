﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Baseline.Reflection
{
    public class ArrayIndexer : Accessor
    {
        private readonly IndexerValueGetter _getter;

        public ArrayIndexer(IndexerValueGetter getter)
        {
            _getter = getter;
        }

        public string FieldName => _getter.Name;
        public Type PropertyType => _getter.ValueType;
        public PropertyInfo? InnerProperty => null;
        public Type DeclaringType => _getter.DeclaringType;
        public string Name => _getter.Name;
        public Type OwnerType => DeclaringType;

        public void SetValue(object target, object propertyValue)
        {
            _getter.SetValue(target, propertyValue);
        }

        public object? GetValue(object target)
        {
            return _getter.GetValue(target);
        }

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            throw new NotSupportedException("Not supported in arrays");
        }

        public string[] PropertyNames { get { return new[] {Name}; } }

        public Expression<Func<T, object>> ToExpression<T>()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body = Expression.ArrayIndex(parameter, Expression.Constant(_getter.Index, typeof(int)));
            var getterValueTypeInfo = _getter.ValueType.GetTypeInfo();
            if (getterValueTypeInfo.IsValueType)
            {
                body = Expression.Convert(body, typeof(Object));
            }


            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(object));
            return (Expression<Func<T, object>>)Expression.Lambda(delegateType, body, parameter);

        }

        public Accessor Prepend(PropertyInfo property)
        {
            return
                new PropertyChain(new IValueGetter[] { new PropertyValueGetter(property), _getter });
        }

        public IEnumerable<IValueGetter> Getters()
        {
            yield return _getter;
        }
    }
}