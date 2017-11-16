using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SyslogMessage syslog = new SyslogMessage();

            string odg = "";
            do
            {
                Console.WriteLine("Izaberitre nacin upisivanja:");

                odg = Console.ReadLine();

            } while (odg != "1" && odg != "2");
            string message = "";
            int severity = 0;
            while (message != "q")
            {
                do
                {
                    Console.WriteLine("Unesite tip poruke[0-7]:");
                    severity = Convert.ToInt32(Console.ReadLine());
                } while (severity < 0 || severity > 7);


                Console.WriteLine("Unesite poruku:");
                message = Console.ReadLine();

                syslog.Facility = 1;
                syslog.Severity = severity;
                syslog.SendTime = DateTime.Now;

                syslog.Message  = odg + '`' + message;
                syslog.HostName = "";

                byte[] retVal = new byte[1032 + message.Length];


                // var a = syslog.Facility.ToString().Select(o => Convert.ToInt32(o)).ToArray();

                // Array.Copy(syslog.Facility.ToString().Select(o => Convert.ToInt32(o)).ToArray(), 0, retVal, 0, sizeof(int));

                object o = syslog as object;

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, o);

                var m = ms.ToArray();

                ms.Write(retVal, 0, retVal.Length);
                ms.Seek(0, SeekOrigin.Begin);

                string essage = (string)bf.Deserialize(ms);

                if (odg == "1")
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    string address = "net.tcp://localhost:9999/SecurityService";

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
                else
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    string address = "net.tcp://localhost:25001/SecurityService";

                    using (ClientProxy proxy = new ClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
            }
        }
    }
}
