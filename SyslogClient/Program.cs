using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFComponent_1 wcfc1 = new WCFComponent_1();
            WCFComponent_2 wcfc2 = new WCFComponent_2();
            Console.WriteLine("Branci was here...");
            Console.ReadLine();
        }
    }
}
