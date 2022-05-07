using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2
{
    class TimeUtil
    {
        public static long get_time_stamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
            // C:\Users\topo\source\repos\WebApplication2\WebApplication2\bin\Debug\net5.0\publish
            // C:\Users\topo\source\repos\WebApplication2\WebApplication2\bin\Debug\net5.0
        }
    }
}
