using System.Collections.Generic;
using System.IO;

namespace Baseline
{
    public class FlatFileWriter : IFlatFileWriter
    {
        public FlatFileWriter(List<string> list)
        {
            List = list;
        }


        public void WriteProperty(string name, string value)
        {
            List.RemoveAll(x => x.StartsWith(name + "="));
            List.Add("{0}={1}".ToFormat(name, value));
        }


        public void WriteLine(string line)
        {
            List.Fill(line);
        }

        public void Sort()
        {
            List.Sort();
        }


        public List<string> List { get; }

        public override string ToString()
        {
            var writer = new StringWriter();
            List.Each(x => writer.WriteLine(x));

            return writer.ToString();
        }
    }
}