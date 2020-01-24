using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Widgets5;

namespace Widgets3
{
    public class Widget3CallingWidget5Caller
    {
        public static Assembly Calling()
        {
            return Widget5Caller.Calling();
        }
    }
}
