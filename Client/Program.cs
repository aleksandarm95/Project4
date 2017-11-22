using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using Manager;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            string component = "";
            string message = "";
            string messageToSend = "";

            #region Cert

            string syslogClientCert = "syslogclient";
            string clientCert_sign = "client_sign";

            #endregion
            do
            {
                Console.WriteLine("Izaberitre nacin upisivanja:");

                component = Console.ReadLine();

            } while (component != "1" && component != "2");
            
            while (message != "q")
            {
                var severity = 0;
                do
                {
                    Console.WriteLine("Unesite tip poruke[0-7]:");
                    severity = Convert.ToInt32(Console.ReadLine());
                    //try za los unos
                } while (severity < 0 || severity > 7);

                Console.WriteLine("Unesite poruku:");
                message = Console.ReadLine();

                messageToSend = severity + "`" + component + "`" + message;

                NetTcpBinding binding = null;

                if (component == "1")
                {
                    binding = new NetTcpBinding();
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                    X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogClientCert);
                    EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:55555/SecurityService"),
                        new X509CertificateEndpointIdentity(srvCert));

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCert_sign);

                        // Create a signature using SHA1 hash algorithm
                        byte[] signature = DigitalSignature.Create(messageToSend, "SHA1", signCert);
                        var arr = Encoding.ASCII.GetBytes(messageToSend);
                        proxy.SendTry(arr, signature);
                    }
                }
                else
                {
                    binding = new NetTcpBinding();
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                    X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogClientCert);
                    EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:44444/SecurityService"),
                        new X509CertificateEndpointIdentity(srvCert));

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                    }
                }
            }
        }
    }
}
