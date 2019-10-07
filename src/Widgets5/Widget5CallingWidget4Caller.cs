using System.Reflection;
using Widgets4;

namespace Widgets5
{
    public class Widget5CallingWidget4Caller
    {
        public static Assembly Calling()
        {
            return Widget4Caller.Calling();
        }
    }
}
