using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SyslogClient
{
    public class WCFComponent_2
    {
        public WCFComponent_2()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:25001/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }


    }
}
