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
            string syslogClientCert = "syslogclient";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string address = "net.tcp://localhost:55555/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);

            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

           //host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogClientCert);


            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            host.Open();
        }

        //public static bool EventLogSerialize(SyslogMessage syslogMessage)
        //{
        //    bool allowed = false;

        //    WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;

           
        //    WindowsIdentity identity = WindowsIdentity.GetCurrent();
        //    var groups = from sid in identity.Groups select sid.Translate(typeof(NTAccount))
        //    if (principal.IsInRole(Permissions.Read.ToString()))
        //    {
        //        Audit.AuthorizationSuccess(principal.Identity.Name, syslogMessage);

        //        Console.WriteLine("ExecuteCommand() passed for user {0}.", principal.Identity.Name);
        //        allowed = true;
        //    }
        //    else
        //    {
        //        Audit.AuthorizationFailed(principal.Identity.Name, syslogMessage);
        //        Console.WriteLine("ExecuteCommand() failed for user {0}.", principal.Identity.Name);
        //    }

        //    return allowed;

        //}
    }
}
