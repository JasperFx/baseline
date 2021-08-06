using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Baseline.Reflection
{
    public class SingleProperty : Accessor
    {
        private readonly PropertyInfo _property;
        private readonly Type? _ownerType;

        public SingleProperty(PropertyInfo property)
        {
            _property = property;
        }

        public SingleProperty(PropertyInfo property, Type ownerType)
        {
            _property = property;
            _ownerType = ownerType;
        }


        public string FieldName => _property.Name;

        public Type PropertyType => _property.PropertyType;

        public Type DeclaringType => _property.DeclaringType!;


        public PropertyInfo InnerProperty => _property;

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = ReflectionHelper.GetProperty(expression);
            return new PropertyChain(new[] {new PropertyValueGetter(_property), new PropertyValueGetter(property)});
        }

        public string[] PropertyNames
        {
            get { return new[] { _property.Name }; }
        }

        public Expression<Func<T, object>> ToExpression<T>()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body = Expression.Property(parameter, _property);
            if (_property.PropertyType.GetTypeInfo().IsValueType)
            {
                body = Expression.Convert(body, typeof (Object));
            }


            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(object));
            return (Expression<Func<T, object>>) Expression.Lambda(delegateType, body, parameter);
        }

        public Accessor Prepend(PropertyInfo property)
        {
            return
                new PropertyChain(new IValueGetter[]
                                  {new PropertyValueGetter(property), new PropertyValueGetter(_property)});
        }

        public IEnumerable<IValueGetter> Getters()
        {
            yield return new PropertyValueGetter(_property);
        }

        public string Name => _property.Name;

        public virtual void SetValue(object target, object propertyValue)
        {
            if (_property.CanWrite)
            {
                _property.SetValue(target, propertyValue, null);
            }
        }

        public object? GetValue(object target)
        {
            return _property.GetValue(target, null);
        }

        public Type OwnerType => _ownerType ?? DeclaringType;


        public static SingleProperty Build<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = ReflectionHelper.GetProperty(expression);
            return new SingleProperty(property);
        }

        public static SingleProperty Build<T>(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName)!;
            return new SingleProperty(property);
        }

        public bool Equals(SingleProperty? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _property.PropertyMatches(other._property);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SingleProperty)) return false;
            return Equals((SingleProperty) obj);
        }

        public override int GetHashCode()
        {
            return (_property != null ? (_property.DeclaringType?.FullName + "." + _property.Name).GetHashCode() : 0);
        }
    }
}