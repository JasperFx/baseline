using System.Collections;
using System.Collections.Generic;

namespace Baseline.Binding
{
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
}