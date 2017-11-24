using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Manager;
using Formatter = System.Runtime.Serialization.Formatter;

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
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string address = "net.tcp://localhost:"+  port +"/SecurityService";
            //string syslogCert = "syslog";
           string syslogCert = Common.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogCert);


            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
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
            string address = "net.tcp://10.1.212.120:" + port + "/SystemServer";

            ChannelFactory<ISystemServer> factory = new ChannelFactory<ISystemServer>(binding, address);

            ISystemServer proxy = factory.CreateChannel();

            return proxy;
        }
    }
}
