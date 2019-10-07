using System.Reflection;
using BaselineTypeDiscovery;

[assembly:IgnoreAssembly]

namespace Widgets4
{
    public class Widget4Caller
    {
        public static Assembly Calling()
        {
            return CallingAssembly.Find();
        }
    }
}