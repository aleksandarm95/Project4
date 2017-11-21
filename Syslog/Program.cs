using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    class Program
    {
        public static bool firstConnect = false;

        public static SyslogService.ServerKind serverKind = SyslogService.ServerKind.Secondary;

        static void Main(string[] args)
        {

            string portToSyslogServer = Console.ReadLine();
            string portServer = Console.ReadLine();
            string portClient = Console.ReadLine();

            SyslogService ss = new SyslogService(portToSyslogServer);

            Task t = new Task(() =>
            {
                ss.CreateServerSide(portServer);
                
                while (true)
                {
                    serverKind = SetServerRole(serverKind, portClient, ss);
                }
            });

            t.Start();

            Console.ReadLine();
        }

        public static SyslogService.ServerKind SetServerRole(SyslogService.ServerKind serverKind, string portClient, SyslogService ss)
        {
            SyslogService.ServerKind _serverKind = SyslogService.ServerKind.Unknown;
            try
            {


                ISystemServer proxy = ss.CreateClientSide(portClient);

                if (!firstConnect)
                {
                    string dataBase = GetDataBase();
                    
                    proxy.SendDatabase(dataBase);
                    firstConnect = true;
                }
                else
                {
                    proxy.CheckIsAlive();
                }
                _serverKind = serverKind;
            }
            catch
            {
                Console.WriteLine(portClient + "-> primarni");
                _serverKind = SyslogService.ServerKind.Primary;
                firstConnect = false;
            }

            return _serverKind;
        }

        public static string GetDataBase()
        {
            string dataBase = "";
            string line = "";

            StreamReader sr = new StreamReader(@"..\..\..\SyslogServices.txt");

            line = sr.ReadLine();
            if (line == null)
            {
                sr.Close();
            }

            dataBase+= line;

            while (line != null)
            {
                line = sr.ReadLine();
                dataBase += "~" + line;
            }

            sr.Close();

            return dataBase;
        }
    }
}
