using System.Collections.Generic;
using System.IO;

namespace Baseline
{
    public class FlatFileWriter : IFlatFileWriter
    {
        private readonly List<string> _list;

        public FlatFileWriter(List<string> list)
        {
            _list = list;
        }

        public void WriteProperty(string name, string value)
        {
            _list.RemoveAll(x => x.StartsWith(name + "="));
            _list.Add("{0}={1}".ToFormat(name, value));
        }

        public void WriteLine(string line)
        {
            _list.Fill(line);
        }

        public void Sort()
        {
            _list.Sort();
        }


        public List<string> List => _list;

        public override string ToString()
        {
            var writer = new StringWriter();
            _list.Each(x => writer.WriteLine(x));

            return writer.ToString();
        }
    }
}