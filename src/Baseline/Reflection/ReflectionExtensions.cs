using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Baseline.Reflection
{
    public static class ReflectionExtensions
    {
        public static U? ValueOrDefault<T, U>(this T? root, Expression<Func<T, U>> expression)
            where T : class
        {
            if (root == null)
            {
                return default(U);
            }

            var accessor = ReflectionHelper.GetAccessor(expression);

            var result = accessor.GetValue(root);

            return (U) (result ?? default(U))!;
        }

        public static T? GetAttribute<T>(this MemberInfo provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof (T), true);
            return atts.FirstOrDefault() as T;
        }

        public static T? GetAttribute<T>(this Assembly provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T));
            return atts.FirstOrDefault() as T;
        }

        public static T? GetAttribute<T>(this Module provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T));
            return atts.FirstOrDefault() as T;
        }

        public static T? GetAttribute<T>(this ParameterInfo provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T), true);
            return atts.FirstOrDefault() as T;
        }

        public static IEnumerable<T> GetAllAttributes<T>(this Assembly provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T)).OfType<T>();
        }

        public static IEnumerable<T> GetAllAttributes<T>(this MemberInfo provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), true).OfType<T>();
        }

        public static IEnumerable<T> GetAllAttributes<T>(this Module provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T)).OfType<T>();
        }

        public static IEnumerable<T> GetAllAttributes<T>(this ParameterInfo provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), true).OfType<T>();
        }

        public static IEnumerable<T>? GetAllAttributes<T>(this Accessor accessor) where T : Attribute
        {
            return accessor.InnerProperty?.GetAllAttributes<T>();
        }

        public static bool HasAttribute<T>(this Assembly provider) where T : Attribute
        {
            return provider.IsDefined(typeof(T));
        }

        public static bool HasAttribute<T>(this MemberInfo provider) where T : Attribute
        {
            return provider.IsDefined(typeof(T), true);
        }

        public static bool HasAttribute<T>(this Module provider) where T : Attribute
        {
            return provider.IsDefined(typeof(T));
        }

        public static bool HasAttribute<T>(this ParameterInfo provider) where T : Attribute
        {
            return provider.IsDefined(typeof(T), true);
        }

        public static void ForAttribute<T>(this Assembly provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this MemberInfo provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this Module provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this ParameterInfo provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this Assembly provider, Action<T> action, Action elseDo)
            where T : Attribute
        {
            var found = false;
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
                found = true;
            }

            if (!found) elseDo();
        }

        public static void ForAttribute<T>(this MemberInfo provider, Action<T> action, Action elseDo)
            where T : Attribute
        {
            var found = false;
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
                found = true;
            }

            if (!found) elseDo();
        }

        public static void ForAttribute<T>(this Module provider, Action<T> action, Action elseDo)
            where T : Attribute
        {
            var found = false;
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
                found = true;
            }

            if (!found) elseDo();
        }

        public static void ForAttribute<T>(this ParameterInfo provider, Action<T> action, Action elseDo)
            where T : Attribute
        {
            var found = false;
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
                found = true;
            }

            if (!found) elseDo();
        }

        public static void ForAttribute<T>(this Accessor accessor, Action<T> action) where T : Attribute
        {
            foreach (T attribute in accessor.InnerProperty!.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static T? GetAttribute<T>(this Accessor provider) where T : Attribute
        {
            return provider.InnerProperty?.GetAttribute<T>();
        }

        public static bool HasAttribute<T>(this Accessor provider) where T : Attribute
        {
            return provider.InnerProperty?.HasAttribute<T>() ?? false;
        }

        public static Accessor ToAccessor<T>(this Expression<Func<T, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression);
        }

        public static string GetName<T>(this Expression<Func<T, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression).Name;
        }


        public static void IfPropertyTypeIs<T>(this Accessor accessor, Action action)
        {
            if (accessor.PropertyType == typeof (T))
            {
                action();
            }
        }

        public static bool IsInteger(this Accessor accessor)
        {
            return accessor.PropertyType.IsTypeOrNullableOf<int>() || accessor.PropertyType.IsTypeOrNullableOf<long>();
        }

        
        /// <summary>
        /// Does a .Net type have a default, no arg constructor
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null;
        }
        
        // http://stackoverflow.com/a/15273117/426840
        /// <summary>
        /// Is the object an anonymous type that is not within a .Net
        /// namespace. See http://stackoverflow.com/a/15273117/426840
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this object? instance)
        {
            if (instance == null)
                return false;

            return instance.GetType().Namespace == null;
        }
        
        /// <summary>
        /// Get a user readable, "pretty" type name for a given type. Corrects for
        /// generics and inner classes
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetPrettyName(this Type t)
        {
            if (!t.GetTypeInfo().IsGenericType)
                return t.Name;

            var sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.Ordinal)));
            sb.Append(t.GetGenericArguments().Aggregate("<",
                (aggregate, type) => aggregate + (aggregate == "<" ? "" : ",") + GetPrettyName(type)));
            sb.Append('>');

            return sb.ToString();
        }
    }
}