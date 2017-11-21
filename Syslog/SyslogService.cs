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
        public enum ServerKind {
            Unknown = 0,
            Primary,
            Secondary
        }

        public SyslogService() { }
        public SyslogService(string port)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:"+  port +"/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }

        public void CreateServerSide(string port)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:" + port + "/SystemServer";

            ServiceHost host = new ServiceHost(typeof(SystemServer));
            host.AddServiceEndpoint(typeof(ISystemServer), binding, address);
            host.Open();
        }

        public ISystemServer CreateClientSide(string port)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:" + port + "/SystemServer";

            ChannelFactory<ISystemServer> factory = new ChannelFactory<ISystemServer>(binding, address);

            ISystemServer proxy = factory.CreateChannel();

            return proxy;
        }
    }
}
