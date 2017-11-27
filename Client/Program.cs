     using System;
     using System.ServiceModel;
     using Common;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string component = ""; 
            string message = "";

            do
            {
                Console.WriteLine("Izaberitre nacin upisivanja [1/2]:");

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
                string address = "net.tcp://localhost:55555/SecurityService";
                using (ClientProxy proxy = new ClientProxy(binding, address))
                {
                    proxy.SendKeys(keys);
                }
            }
            else
            {
                binding = new NetTcpBinding();

                string address = "net.tcp://localhost:44444/SecurityService";
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
                    Console.WriteLine("Izaberitre nacin upisivanja [1/2]:");
                    try
                    {
                        severity = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Izaberitre nacin upisivanja [1/2]:");
                        severity = -1;
                    }
                    //try za los unos
                } while (severity < 0 || severity > 7);

                Console.WriteLine("Izaberitre nacin upisivanja [1/2]:");
                message = Console.ReadLine();

                var messageToSend = severity + "`" + component + "`" + message;

                if (component == "1")
                {
                    binding = new NetTcpBinding();
                    string address = "net.tcp://localhost:55555/SecurityService";
                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(DES.TripleEncrypt(messageToSend, key1, key2, key3, true));
                    }
                }
                else
                {
                    binding = new NetTcpBinding();

                    string address = "net.tcp://localhost:44444/SecurityService";
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
