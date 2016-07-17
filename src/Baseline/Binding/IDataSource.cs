using System.Collections.Generic;

namespace Baseline.Binding
{
    public interface IDataSource
    {
        bool Has(string key);
        string Get(string key);

        IEnumerable<string> Keys();
    }
}