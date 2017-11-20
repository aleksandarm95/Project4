using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    public class SystemServer : ISystemServer
    {
        public bool CheckIfAlive()
        {
            return false;
        }
    }
}
