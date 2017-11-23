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

            NetTcpBinding binding = null;

            string key1 = "12345678";
            string key2 = "87654321";
            string key3 = "11111111";

            byte[] keys = DES.Encrypt(key1 + key2 + key3, DES.ReadKeyFromFile("psw1.txt"), true);

            if (component == "1")
            {
                binding = new NetTcpBinding();
                string address = "net.tcp://10.1.212.155:55555/SecurityService";
                using (ClientProxy proxy = new ClientProxy(binding, address))
                {

                    proxy.SendKeys(keys);
                }
            }
            else
            {
                binding = new NetTcpBinding();
                //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string address = "net.tcp://10.1.212.155:44444/SecurityService";
                using (ClientProxy proxy = new ClientProxy(binding, address))
                {
                    proxy.SendKeys(keys);
                }
            }

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

                if (component == "1")
                {
                    binding = new NetTcpBinding();
                    string address = "net.tcp://10.1.212.155:55555/SecurityService";
                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(DES.TripleEncrypt(messageToSend, key1, key2, key3, true));
                    }
                }
                else
                {
                    binding = new NetTcpBinding();
                    //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                    string address = "net.tcp://10.1.212.155:44444/SecurityService";
                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(DES.TripleEncrypt(messageToSend, key1, key2, key3, true));
                    }
                }
            }
        }
    }
}
