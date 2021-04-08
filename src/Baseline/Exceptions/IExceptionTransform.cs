using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Baseline.Exceptions
{
    /// <summary>
    /// Intercept exceptions to possibly transform to different, more
    /// explanatory exceptions
    /// </summary>
    public interface IExceptionTransform
    {
        /// <summary>
        /// Tests an incoming Exception, and optionally return another, transformed
        /// Exception if the rule matches the exception
        /// </summary>
        /// <param name="original"></param>
        /// <param name="transformed"></param>
        /// <returns></returns>
        bool TryTransform(Exception original, out Exception transformed);
    }

    /// <summary>
    /// Collection of IExceptionTransform filters to apply more intelligent
    /// exception wrapping in your APIs
    /// </summary>
    public class ExceptionTransforms : IEnumerable<IExceptionTransform>
    {
        private readonly IList<IExceptionTransform> _transforms = new List<IExceptionTransform>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IExceptionTransform> GetEnumerator()
        {
            return _transforms.GetEnumerator();
        }

        /// <summary>
        /// Add a new, custom transform rule by type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddTransform<T>() where T : IExceptionTransform, new()
        {
            AddTransform(new T());
        }

        /// <summary>
        /// Add a new, custom transform rule
        /// </summary>
        /// <param name="transform"></param>
        public void AddTransform(IExceptionTransform transform)
        {
            _transforms.Add(transform);
        }

        /// <summary>
        /// Start defining exception filtering rules for a certain exception type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ExceptionTransform<T> IfExceptionIs<T>() where T : Exception
        {
            var transform = _transforms.OfType<ExceptionTransform<T>>().FirstOrDefault();
            if (transform == null)
            {
                transform = new ExceptionTransform<T>();
                _transforms.Add(transform);
            }

            return transform;
        }

        /// <summary>
        /// Analyze a thrown exception and either transform or wrap the exception and throw
        /// if a rule applies, or re-throw the original exception with the original stack trace
        /// if no rules apply
        /// </summary>
        /// <param name="ex"></param>
        public void TransformAndThrow(Exception ex)
        {
            _transforms.TransformAndThrow(ex);
        }
    }

    /// <summary>
    /// Basic exception transform based on a type of Exception
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExceptionTransform<T> : IExceptionTransform where T : Exception
    {
        private readonly IList<FilterRule> _rules = new List<FilterRule>();
        
        public bool TryTransform(Exception original, out Exception transformed)
        {
            if (original is T exception)
            {
                foreach (var rule in _rules)
                {
                    if (rule.Filter(exception))
                    {
                        transformed = rule.Transform(exception);
                        return true;
                    }
                }
            }

            transformed = null;
            return false;
        }

        /// <summary>
        /// Always transform an Exception of type T using this transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public ExceptionTransform<T> TransformTo(Func<T, Exception> transform)
        {
            return If(e => true).ThenTransformTo(transform);
        }

        public FilterRule If(Func<T, bool> filter)
        {
            return new FilterRule(filter, this);
        }

        public FilterRule IfInnerIs<TInner>(Func<TInner, bool> innerFilter = null)
        {
            if (innerFilter == null)
            {
                return If(x => x.InnerException is TInner);
            }

            return If(x => x.InnerException is TInner inner && innerFilter(inner));
        }

        public class FilterRule
        {
            private readonly ExceptionTransform<T> _parent;

            public FilterRule(Func<T, bool> filter, ExceptionTransform<T> parent)
            {
                Filter = filter;
                _parent = parent;
            }

            public ExceptionTransform<T> ThenTransformTo(Func<T, Exception> transform)
            {
                Transform = transform;
                _parent._rules.Add(this);
                return _parent;
            }

            internal Func<T, bool> Filter { get; }

            internal Func<T, Exception> Transform { get; private set; }
        }
    }
    
    public static class ExceptionTransformExtensions
    {
        public static void TransformAndThrow(this IEnumerable<IExceptionTransform> transforms, Exception ex)
        {
            foreach (var transform in transforms)
            {
                if (transform.TryTransform(ex, out var transformed))
                {
                    throw transformed;
                }
            }
            
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}