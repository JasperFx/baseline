using BaselineTypeDiscovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Widgets5
{
    public class Widget5Caller
    {
        public static Assembly Calling()
        {
            return CallingAssembly.Find();
        }
    }
}
