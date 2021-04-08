using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Baseline.Reflection;

namespace Baseline
{
    /// <summary>
    /// Create Func<T> objects to build types with no-arg constructors regardless of whether
    /// the constructor is public or private
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class New<T>
    {
        public static readonly Func<T> Instance = Creator();

        private static Func<T> Creator()
        {
            var t = typeof(T);
            if (t == typeof(string))
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

            if (t.HasDefaultConstructor())
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

            return () => (T)FormatterServices.GetUninitializedObject(t);
        }
    }

}
