using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public class Binder<T>
    {
        private readonly Action<IDataSource, T> _bindAll;
        private readonly Func<T> _create;

        public Binder() : this(new Conversions())
        {
        }

        public Binder(Conversions conversions)
        {
            var source = Expression.Parameter(typeof(IDataSource), "source");
            var target = Expression.Parameter(typeof(T), "target");

            var properties = typeof(T).GetProperties().Where(x => x.CanWrite).Select(x => BindProperty(target, source, conversions, x));
            var fields = typeof(T).GetFields().Where(x => x.IsPublic).Select(x => BindField(target, source, conversions, x));

            var allSetters = properties.Concat(fields).Where(x => x != null).ToArray();

            var block = Expression.Block(allSetters);

            var bindAll = Expression.Lambda<Action<IDataSource, T>>(block, source, target);

            _bindAll = bindAll.Compile();

            var ctor = typeof(T).GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0);
            if (ctor != null)
            {
                var newUp = Expression.New(ctor);
                _create = Expression.Lambda<Func<T>>(newUp).Compile();
            }
        }

        public bool CanBuild => _create != null;


        public void Bind(IDataSource source, T target)
        {
            _bindAll(source, target);
        }

        public T Build(IDataSource source)
        {
            if (!CanBuild) throw new InvalidOperationException($"The binder for {typeof(T).FullName} cannot build new objects");

            var target = _create();

            _bindAll(source, target);

            return target;
        }



        public static Expression BindProperty(ParameterExpression target, ParameterExpression source, Conversions conversions, PropertyInfo property)
        {
            var value = fetchValue(source, conversions, property.PropertyType, property.Name);
            if (value == null) return null;


            var method = property.SetMethod;

            var callSetMethod = Expression.Call(target, method, value);

            var condition = Expression.Call(source, BindingExpressions.DataSourceHas, Expression.Constant(property.Name));

            return Expression.IfThen(condition, callSetMethod);
        }

        private static Expression fetchValue(ParameterExpression source, Conversions conversions, Type memberType, string memberName)
        {
            var func = conversions.FindConverter(memberType);
            if (func == null)
            {
                return null;
            }

            Expression value = Expression.Call(source, BindingExpressions.DataSourceGet, Expression.Constant(memberName));
            value = Expression.Invoke(Expression.Constant(func), value);
            value = Expression.Convert(value, memberType);

            return value;
        }

        public static Expression BindField(ParameterExpression target, ParameterExpression source, Conversions conversions, FieldInfo field)
        {
            var value = fetchValue(source, conversions, field.FieldType, field.Name);
            if (value == null) return null;


            var fieldExpression = Expression.Field(target, field);
            var assign = Expression.Assign(fieldExpression, value);

            var condition = Expression.Call(source, BindingExpressions.DataSourceHas, Expression.Constant(field.Name));

            return Expression.IfThen(condition, assign);
        }
    }
}