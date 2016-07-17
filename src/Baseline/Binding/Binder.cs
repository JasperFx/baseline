using System;
using System.Collections.Generic;
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

        private readonly IList<IBoundMember> _members = new List<IBoundMember>(); 

        public Binder() : this(new Conversions())
        {
        }

        public Binder(Conversions conversions)
        {
            var source = Expression.Parameter(typeof(IDataSource), "source");
            var target = Expression.Parameter(typeof(T), "target");

            _members.AddRange(BoundProperty.FindMembers(typeof(T), conversions));
            _members.AddRange(BoundField.FindMembers(typeof(T), conversions));


            var allSetters = _members.Select(x => x.ToBindExpression(target, source, conversions)).ToArray();

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

        public IEnumerable<IBoundMember> Members => _members;

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

    }
}