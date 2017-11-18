using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    public class SyslogService
    {
        public SyslogService()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:26000/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }
    }
}
