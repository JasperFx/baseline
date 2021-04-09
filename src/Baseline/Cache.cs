using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Baseline.ImTools;

namespace Baseline
{
    public class Cache<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly object _locker = new object();
        private ImHashMap<TKey, TValue> _values = ImHashMap<TKey, TValue>.Empty;

        private Action<TValue> _onAddition = x => { };

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            var message = $"Key '{key}' could not be found";
            throw new KeyNotFoundException(message);
        };

        public Cache()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public Cache(Func<TKey, TValue> onMissing)
            : this(new Dictionary<TKey, TValue>(), onMissing)
        {
        }

        public Cache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var pair in dictionary)
            {
                _values = _values.AddOrUpdate(pair.Key, pair.Value);
            }
        }

        public Action<TValue> OnAddition
        {
            set => _onAddition = value;
        }

        public Func<TKey, TValue> OnMissing
        {
            set => _onMissing = value;
        }

        public Func<TValue, TKey> GetKey { get; set; } = delegate { throw new NotImplementedException(); };

        public int Count => _values.Enumerate().Count();


        public TValue this[TKey key]
        {
            get
            {
                if (_values.TryFind(key, out var value))
                {
                    return value;
                }

                lock (_locker)
                {
                    value = _onMissing(key);
                    _onAddition(value);
                    _values = _values.AddOrUpdate(key, value);
                }

                return value;
            }
            set
            {
                _values = _values.AddOrUpdate(key, value);
                
                _onAddition(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>) this).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.Enumerate().Select(x => x.Value).GetEnumerator();
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

        public void Fill(TKey key, Func<TKey, TValue> onMissing)
        {
            if (!_values.TryFind(key, out var existing))
            {
                lock (_locker)
                {
                    var value = onMissing(key);
                    _onAddition(value);
                    _values = _values.AddOrUpdate(key, value);
                }
            }
        }

        public void Fill(TKey key, TValue value)
        {
            if (!_values.TryFind(key, out var existing))
            {
                lock (_locker)
                {
                    _onAddition(value);
                    _values = _values.AddOrUpdate(key, value);
                }
            }
        }


        public bool TryFind(TKey key, out TValue value)
        {
            return _values.TryFind(key, out value);
        }

        public TKey[] GetAllKeys()
        {
            return _values.Enumerate().Select(x => x.Key).ToArray();
        }
        
        public void Remove(TKey key)
        {
            _values = _values.Remove(key);
        }

        public void ClearAll()
        {
            _values = ImHashMap<TKey, TValue>.Empty;
        }
        
        public IDictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();

            foreach (var pair in _values.Enumerate())
            {
                dict.Add(pair.Key, pair.Value);
            }

            return dict;
        }
    }
}