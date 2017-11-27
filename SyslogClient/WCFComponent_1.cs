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
using System.ServiceModel.Description;
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security;
using Manager;

namespace SyslogClient
{
    public class WCFComponent_1
    {
        public WCFComponent_1()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:55555/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);

            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            host.Open();
        }

        public static bool EventLogSerialize(SyslogMessage syslogMessage)
        {
            WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;
            bool groupExists = false;
            bool successfullyAccessed = false;
            var groups = ((WindowsIdentity) (principal.Identity)).Groups;
            if (groups != null)
                foreach (IdentityReference group in groups)
                {
                    SecurityIdentifier sid = (SecurityIdentifier) @group.Translate(typeof(SecurityIdentifier));
                    var fullName = sid.Translate(typeof(NTAccount));
                    if (fullName.ToString().Contains("Reader"))
                    {
                        groupExists = true;
                        break;
                    }
                }
            if (groupExists)
            {
                Audit.AuthorizationSuccess(principal.Identity.Name, syslogMessage);

                Console.WriteLine("ExecuteCommand() passed for user {0}.", principal.Identity.Name);
                successfullyAccessed = true;
            }
            else
            {
                Audit.AuthorizationFailed(principal.Identity.Name, syslogMessage);
                Console.WriteLine("ExecuteCommand() failed for user {0}.", principal.Identity.Name);
                successfullyAccessed = false;
            }
            return successfullyAccessed;
        }
    }
}
