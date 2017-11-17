using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Common;
using System.Threading;

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

        public static bool EventLogSerialize(SyslogMessage syslogMessage)
        {
            bool allowed = false;
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            /// audit both successfull and failed authorization checks
            if (principal.IsInRole(Permissions.Read.ToString()))
            {
                Audit.AuthorizationSuccess(principal.Identity.Name, syslogMessage);

                Console.WriteLine("ExecuteCommand() passed for user {0}.", principal.Identity.Name);
                allowed = true;
            }
            else
            {
                Audit.AuthorizationFailed(principal.Identity.Name, syslogMessage);
                Console.WriteLine("ExecuteCommand() failed for user {0}.", principal.Identity.Name);
            }

            return allowed;

        }
    }
}
