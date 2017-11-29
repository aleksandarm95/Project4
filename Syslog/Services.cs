using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;

namespace Syslog
{
    public class Services : IServices
    {
        public bool SendTry(byte[] message, byte[] signature)
        {
            string messageFrom = Encoding.UTF8.GetString(message);
            Console.WriteLine(messageFrom);
            StreamWriter sw = null;
            string line = "";
            List<string> lines = new List<string>();

           // var m = Encoding.ASCII.GetString(message);
            X509Certificate2 clientCertificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "syslogclient_sign");
            if (DigitalSignature.Verify(messageFrom, "SHA1", signature, clientCertificate))
            {
                Console.WriteLine("Digital Signature is valid.");
                try
                {
                    StreamReader sr = new StreamReader(@"..\..\..\SyslogServices.txt");

                    line = sr.ReadLine();

                    if (line == null)
                    {
                        sr.Close();
                    }

                    lines.Add(line);

                    while (line != null)
                    {
                        line = sr.ReadLine();
                        lines.Add(line);
                    }

                    sr.Close();
                }
                catch
                {
                    sw = new StreamWriter(@"..\..\..\SyslogServices.txt");

                    sw.Close();
                }

                lines.Add(messageFrom);

                sw = new StreamWriter(@"..\..\..\SyslogServices.txt");
                lock (lines)
                {
                    foreach (var item in lines)
                    {
                        sw.WriteLine(item);
                    }
                }
                sw.Close();

                try
                {
                    SyslogService ss = new SyslogService();
                    ISystemServer proxy = ss.CreateClientSide("58001");
                    proxy.Send(message);
                }
                catch
                {
                }

                return true;
            }
            Console.WriteLine("Digital Signature is invalid.");
            return false;
        }

        public bool CheckIfPrimary()
        {
            if (Program.serverKind == SyslogService.ServerKind.Primary)
            {
                return true;
            }
            return false;
        }

        public bool Send(byte[] message)
        {
            return true;
        }

        public void SendKeys(byte[] message)
        { 
        }
    }
}
