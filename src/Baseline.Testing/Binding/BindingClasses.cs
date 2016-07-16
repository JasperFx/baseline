using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public interface IDataSource
    {
        bool Has(string key);
        string Get(string key);

        IEnumerable<string> Keys();
    }

    public class DictionaryDataSource : IDataSource
    {
        public IDictionary<string, string> Dictionary { get; }

        public DictionaryDataSource(IDictionary<string, string> dictionary)
        {
            Dictionary = dictionary;
        }

        public bool Has(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public string Get(string key)
        {
            return Dictionary[key];
        }

        public IEnumerable<string> Keys()
        {
            return Dictionary.Keys;
        }
    }

    internal class BindingExpressions
    {
        internal static MethodInfo DataSourceGet = typeof(IDataSource).GetMethod(nameof(IDataSource.Get));
        internal static MethodInfo DataSourceHas = typeof(IDataSource).GetMethod(nameof(IDataSource.Has));
    }

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

            var block = Expression.Block(new[] { source, target }, allSetters);

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
            var func = conversions.FindConverter(property.PropertyType);
            if (func == null)
            {
                return null;
            }

            var name = Expression.Constant(property.Name);
            var value = Expression.Call(source, BindingExpressions.DataSourceGet, name);
            var convert = Expression.Invoke(Expression.Constant(func), value);
            var cast = Expression.Convert(convert, property.PropertyType);

            var method = property.SetMethod;

            var callSetMethod = Expression.Call(target, method, cast);

            var condition = Expression.Call(source, BindingExpressions.DataSourceHas, name);

            return Expression.IfThen(condition, callSetMethod);
        }

        public static Expression BindField(ParameterExpression target, ParameterExpression source, Conversions conversions, FieldInfo field)
        {
            var func = conversions.FindConverter(field.FieldType);
            if (func == null)
            {
                return null;
            }

            var name = Expression.Constant(field.Name);
            var value = Expression.Call(source, BindingExpressions.DataSourceGet, name);
            var convert = Expression.Invoke(Expression.Constant(func), value);
            var cast = Expression.Convert(convert, field.FieldType);

            var fieldExpression = Expression.Field(target, field);
            var assign = Expression.Assign(fieldExpression, cast);

            var condition = Expression.Call(source, BindingExpressions.DataSourceHas, name);

            return Expression.IfThen(condition, assign);
        }
    }
}