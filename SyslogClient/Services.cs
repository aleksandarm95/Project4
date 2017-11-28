using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;
using System.ServiceModel;
using Manager;

namespace SyslogClient
{
    public class Services : IServices
    {
        static Dictionary<string, string[]> clientsKeys = new Dictionary<string, string[]>();
        public bool SendTry(byte[] message, byte[] signature)
        {
            return false;
        }

        public bool CheckIfPrimary()
        {
            return false;
        }

        public bool Send(byte[] message)
        {
            WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;

            string decriptMessage = string.Empty;

            if (clientsKeys.ContainsKey(principal.Identity.Name))
            {
                decriptMessage = DES.TripleDecrypt(message, clientsKeys[principal.Identity.Name][0], 
                    clientsKeys[principal.Identity.Name][1], clientsKeys[principal.Identity.Name][2], true);
            }
            else
            {
                Console.WriteLine("NEMA GA U LISTI-> " + principal.Identity.Name);
                return false;
            }

            SyslogMessage syslogMessage = new SyslogMessage();
            var arrayRecieved = decriptMessage.Split('`');

            int component = Convert.ToInt32(arrayRecieved[1]);
            syslogMessage.Severity = Convert.ToInt32(arrayRecieved[0]);
            syslogMessage.Facility = 1;
            syslogMessage.Time = DateTime.Now;
            syslogMessage.Message = arrayRecieved[2];
            syslogMessage.ClientName = Thread.CurrentPrincipal.Identity.Name;

            Console.WriteLine("PRIMLJENA PORUKA-> " + syslogMessage.Message);

            bool successfullyAccessed = false;
            if (component == 1)
            {
                successfullyAccessed = WCFComponent_1.EventLogSerialize(syslogMessage);
            }
            else
            {
                successfullyAccessed = WCFComponent_2.XmlSerialize(syslogMessage);
            }

            string messageToSend = "";
            if (successfullyAccessed)
            {
                messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.ClientName.ToString() + "\t" +
                                "SUCCESSFULLY accessed" +
                                syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() +
                                "\t" + syslogMessage.Message;
            }
            else
            {
                messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.ClientName.ToString() + "\t" +
                                "FAILED accessed" +
                                syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() +
                                "\t" + syslogMessage.Message;
            }

            bool first = true;

            string syslogServerCert1 = "syslog";
            string syslogServerCert2 = "syslog2";
            string syslogClient_sign = "syslogclient_sign";

            string portServer1 = "26000";
            string portServer2 = "27000";

            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                X509Certificate2 srvCert =
                    CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, syslogServerCert1);
                
                EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:" + portServer1 + "/SecurityService"),
                    new X509CertificateEndpointIdentity(srvCert));

                using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                {
                    first = proxy.CheckIfPrimary();
                }

                if (first)
                {
                    using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                    {
                        X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My,
                            StoreLocation.LocalMachine, syslogClient_sign);
                        byte[] signatureServerBytes = DigitalSignature.Create(messageToSend, "SHA1", signCert);
                        var arr = Encoding.ASCII.GetBytes(messageToSend);
                        proxy.SendTry(Encoding.ASCII.GetBytes(messageToSend), signatureServerBytes);
                    }
                }
                else
                {
                    try
                    {
                        NetTcpBinding binding2 = new NetTcpBinding();
                        binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                        X509Certificate2 srvCert2 = CertManager.GetCertificateFromStorage(StoreName.My,
                            StoreLocation.LocalMachine, syslogServerCert2);
                        EndpointAddress address2 = new EndpointAddress(
                            new Uri("net.tcp://localhost:" + portServer2 + "/SecurityService"),
                            new X509CertificateEndpointIdentity(srvCert2));


                        using (SyslogClientProxy proxy = new SyslogClientProxy(binding2, address2))
                        {
                            X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My,
                                StoreLocation.LocalMachine, syslogClient_sign);
                            byte[] signatureServerBytes = DigitalSignature.Create(messageToSend, "SHA1", signCert);
                            var arr = Encoding.ASCII.GetBytes(messageToSend);
                            proxy.SendTry(Encoding.ASCII.GetBytes(messageToSend), signatureServerBytes);
                        }
                    }
                    catch
                    {
                        using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                        {
                            X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My,
                                StoreLocation.LocalMachine, syslogClient_sign);
                            byte[] signatureServerBytes = DigitalSignature.Create(messageToSend, "SHA1", signCert);
                            var arr = Encoding.ASCII.GetBytes(messageToSend);
                            proxy.SendTry(Encoding.ASCII.GetBytes(messageToSend), signatureServerBytes);
                        }
                    }
                }
            }
            catch
            {
                NetTcpBinding binding2 = new NetTcpBinding();
                binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                X509Certificate2 srvCert2 =
                    CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogServerCert2);
                EndpointAddress address2 = new EndpointAddress(new Uri("net.tcp://localhost:" + portServer2 + "/SecurityService"),
                    new X509CertificateEndpointIdentity(srvCert2));

                using (SyslogClientProxy proxy = new SyslogClientProxy(binding2, address2))
                {
                    X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My,
                        StoreLocation.LocalMachine, syslogClient_sign);
                    byte[] signatureServerBytes = DigitalSignature.Create(messageToSend, "SHA1", signCert);
                    var arr = Encoding.ASCII.GetBytes(messageToSend);
                    proxy.SendTry(Encoding.ASCII.GetBytes(messageToSend), signatureServerBytes);
                }
            }

            return true;
        }

        public void SendKeys(byte[] message)
        {
            string keys = DES.Decrypt(message, DES.ReadKeyFromFile("psw1.txt"), true);

            string[] keysArray = new string[3];

            keysArray[0] = keys.Substring(0, 8);
            keysArray[1] = keys.Substring(8, 8);
            keysArray[2] = keys.Substring(16, 8);

            WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;
            if (!clientsKeys.ContainsKey(principal.Identity.Name))
            {
                clientsKeys.Add(principal.Identity.Name, keysArray);
            }
            else
            {
                clientsKeys[principal.Identity.Name] = keysArray;
            }
        }
    }
}
