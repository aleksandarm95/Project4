using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace SyslogClient
{
    public class Services : IServices
    {
        public bool Send(byte[] message)
        {
            var m = System.Text.Encoding.UTF8.GetString(message);
            bool retVal = false;

            //DEKRIPTUJ



            return retVal;
        }
    }
}
