using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace BaselineTypeDiscovery.Testing
{
    public class AssemblyFinderTests
    {
        [Fact]
        public void find_assemblies_in_dependency_order()
        {
            var widgets = AssemblyFinder.FindAssemblies(a => a.GetName().Name.StartsWith("Widget"),txt => Console.WriteLine(txt), false);

            var names = widgets.Select(x => x.GetName().Name).ToArray();

            var index3 = Array.IndexOf(names, "Widgets3");
            var index5 = Array.IndexOf(names, "Widgets5");
            
            index3.ShouldBeGreaterThan(index5);
            
        }
    }
}