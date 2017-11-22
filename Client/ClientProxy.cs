using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using Manager;
using Formatter = Common.Formatter;

namespace Client
{
    public class ClientProxy : ChannelFactory<IServices>, IServices, IDisposable
    {
        IServices factory;

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            //string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string cltCertCN = "client";

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);


            factory = this.CreateChannel();
        }

        public bool SendTry(byte[] message, byte[] signature)
        {
            try
            {
                return factory.SendTry(message, signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool CheckIfPrimary()
        {
            return factory.CheckIfPrimary();
        }

        public bool Send(byte[] message)
        {
            return factory.Send(message);
        }
    }
}
