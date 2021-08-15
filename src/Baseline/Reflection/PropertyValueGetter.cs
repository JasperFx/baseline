﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Baseline.Reflection
{
    public class PropertyValueGetter : IValueGetter
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyValueGetter(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo => _propertyInfo;

        public object? GetValue(object target)
        {
            return _propertyInfo.GetValue(target, null);
        }

        public string Name => _propertyInfo.Name;

        public Type DeclaringType => _propertyInfo.DeclaringType!;

        public Type ValueType => _propertyInfo.PropertyType;

        public Expression ChainExpression(Expression body)
        {
            var memberExpression = Expression.Property(body, _propertyInfo);
            if (!_propertyInfo.PropertyType.GetTypeInfo().IsValueType)
            {
                return memberExpression;
            }

            return Expression.Convert(memberExpression, typeof (object));
        }

        public void SetValue(object target, object propertyValue)
        {
            _propertyInfo.SetValue(target, propertyValue, null);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(PropertyValueGetter)) return false;
            return Equals((PropertyValueGetter) obj);
        }

        public bool Equals(PropertyValueGetter? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._propertyInfo.PropertyMatches(_propertyInfo);
        }

        public override int GetHashCode()
        {
            return (_propertyInfo != null ? _propertyInfo.GetHashCode() : 0);
        }
    }
}