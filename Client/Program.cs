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

            Random r = new Random();

            string key1 = RandomKey(r);
            string key2 = RandomKey(r);
            string key3 = RandomKey(r);

            Console.WriteLine(key1);

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

                    string address = "net.tcp://10.1.212.155:44444/SecurityService";
                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(DES.TripleEncrypt(messageToSend, key1, key2, key3, true));
                    }
                }
            }
        }

        public static string RandomKey(Random r)
        {
            string key = string.Empty;

            key += (r.Next(10000000, 99999999)).ToString();

            return key;
        }
    }
}
