using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog
{
    public class Services : IServices
    {
        public bool Send(byte[] message)
        {
            //StreamReader sr = new StreamReader()

            string messageFrom = Encoding.UTF8.GetString(message);

            Console.WriteLine(messageFrom);

            StreamWriter sw = null;

            string line = "";
            List<string> lines = new List<string>();

            try
            {
                StreamReader sr = new StreamReader(@"..\..\SyslogServices.txt");

                line = sr.ReadLine();

                if (line == null)
                {
                    sr.Close();
                }

                lines.Add(line);
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window
                    //Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                    lines.Add(line);
                }

                //close the file
                sr.Close();
            }
            catch
            {
                sw = new StreamWriter(@"..\..\SyslogServices.txt");

                sw.Close();
            }

            lines.Add(messageFrom);

            sw = new StreamWriter(@"..\..\SyslogServices.txt");

            foreach (var item in lines)
            {
                sw.WriteLine(item);
            }

            sw.Close();

            return true;
        }
    }
}
