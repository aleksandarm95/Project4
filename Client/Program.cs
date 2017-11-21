using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string component = "";
            string message = "";
            string messageToSend = "";
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
                } while (severity < 0 || severity > 7);

                Console.WriteLine("Unesite poruku:");
                message = Console.ReadLine();

                messageToSend = severity + "`" + component + "`" + message;

                if (component == "1")
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    string address = "net.tcp://10.1.212.116:55555/SecurityService";

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                    }
                }
                else
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    string address = "net.tcp://10.1.212.116:44444/SecurityService";

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                    }
                }
            }
        }
    }
}
