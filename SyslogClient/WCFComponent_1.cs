using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace SyslogClient
{
    public class WCFComponent_1
    {
        public WCFComponent_1()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:25000/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }

        public static void EventLogSerialize(SyslogMessage syslogMessage)
        {
            bool allowed = false;
            //odraditi CustomPrincipal i videti da li je korisnik Reader
           
            Audit.AuthorizationSuccess("Branci", syslogMessage);
        }
    }
}
