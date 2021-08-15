using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Baseline
{
    /// <summary>
    /// Convenience wrapper around ConcurrentDictionary to auto-fill
    /// in missing values and simplify usage
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentCache<TKey, TValue> : IEnumerable<TValue> where TKey: notnull
    {
        private readonly ConcurrentDictionary<TKey, TValue> _values;

        private Action<TValue> _onAddition = x => { };

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            var message = $"Key '{key}' could not be found";
            throw new KeyNotFoundException(message);
        };

        public ConcurrentCache()
            : this(new ConcurrentDictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Build a cache that can fill in missing values automatically
        /// </summary>
        /// <param name="onMissing"></param>
        public ConcurrentCache(Func<TKey, TValue> onMissing)
            : this(new ConcurrentDictionary<TKey, TValue>(), onMissing)
        {
        }

        public ConcurrentCache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public ConcurrentCache(IDictionary<TKey, TValue> dictionary)
        {
            _values = new ConcurrentDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Perform an action on an item when it is first added to the cache
        /// </summary>
        public Action<TValue> OnAddition
        {
            set { _onAddition = value; }
        }

        public Func<TKey, TValue> OnMissing
        {
            set { _onMissing = value; }
        }

        public Func<TValue, TKey> GetKey { get; set; } = delegate { throw new NotImplementedException(); };

        public int Count => _values.Count;

        [Obsolete("Use First() or FirstOrDefault().")]
        public TValue First
        {
            get
            {
                throw new NotSupportedException("This property is Obsolete.  Use First() or FirstOrDefault().");
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                FillDefault(key);

                return _values[key];
            }
            set
            {
                _onAddition(value);
                _values[key] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>)this).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        /// <summary>
        ///   Guarantees that the Cache has the default value for a given key.
        ///   If it does not already exist, it's created.
        /// </summary>
        /// <param name = "key"></param>
        public void FillDefault(TKey key)
        {
            Fill(key, _onMissing);
        }

        /// <summary>
        /// Delegates to GetOrAdd() on ConcurrentDictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onMissing"></param>
        public void Fill(TKey key, Func<TKey, TValue> onMissing)
        {
            bool newValue = false;
            var value = _values.GetOrAdd(key,
                k =>
                {
                    newValue = true;
                    return onMissing(k);
                });

            if (newValue)
                _onAddition(value);
        }

        public void Fill(TKey key, TValue value)
        {
            _values.TryAdd(key, value);
        }

        public void Each(Action<TValue> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Value);
            }
        }

        public void Each(Action<TKey, TValue> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<TValue> predicate)
        {
            return _values.Any(pair => predicate(pair.Value));
        }

        public TValue Find(Predicate<TValue> predicate)
        {
            return _values.FirstOrDefault(x => predicate(x.Value)).Value;
        }

        public TKey[] GetAllKeys()
        {
            return _values.Keys.ToArray();
        }

        public TValue[] GetAll()
        {
            return _values.Values.ToArray();
        }

        public void Remove(TKey key)
        {
            TValue? _;
            _values.TryRemove(key, out _);
        }

        public void ClearAll()
        {
            _values.Clear();
        }

        public bool WithValue(TKey key, Action<TValue> callback)
        {
            if (_values.TryGetValue(key, out TValue? value))
            {
                callback(value);
                return true;
            }

            return false;
        }

        public IDictionary<TKey, TValue> ToDictionary()
        {
            return new Dictionary<TKey, TValue>(_values);
        }
    }
}