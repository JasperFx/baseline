using System.Reflection;
using BaselineTypeDiscovery;

namespace Widgets1
{
    public class WidgetCallingAssemblyFinder
    {
        public static Assembly Calling()
        {
            return CallingAssembly.Find();
        }
    }
}