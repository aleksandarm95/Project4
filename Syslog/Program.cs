using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    class Program
    {
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
                proxy.CheckIfAlive();
                _serverKind = serverKind;
                
            }
            catch
            {
                Console.WriteLine(portClient + "-> primarni");
                _serverKind = SyslogService.ServerKind.Primary;
            }

            return _serverKind;
        }
    }
}
