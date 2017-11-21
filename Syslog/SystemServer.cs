using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    public class SystemServer : ISystemServer
    {
        public bool CheckIsAlive()
        {
            return true;
        }

        public bool Send(byte[] message)
        {
            string messageFrom = Encoding.UTF8.GetString(message);

            Console.WriteLine(messageFrom);

            StreamWriter sw = null;

            string line = "";
            List<string> lines = new List<string>();

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

            return true;
        }

        public void SendDatabase(string message)
        {
            StreamWriter sw = null;

            string[] lines = message.Split('~');

            sw = new StreamWriter(@"..\..\..\SyslogServices.txt");
            lock (lines)
            {
                foreach (var item in lines)
                {
                    sw.WriteLine(item);
                }
            }
            sw.Close();
        }
    }
}
