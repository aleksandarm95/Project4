using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;

namespace SyslogClient
{
    public class Services : IServices
    {
        public bool Send(byte[] message)
        {
            SyslogMessage syslogMessage = new SyslogMessage();
            var arrayRecieved = System.Text.Encoding.UTF8.GetString(message).Split('`');

            int component = Convert.ToInt32(arrayRecieved[1]);
            syslogMessage.Severity = Convert.ToInt32(arrayRecieved[0]);
            syslogMessage.Facility = 1;
            syslogMessage.Time = DateTime.Now;
            syslogMessage.Message = arrayRecieved[2];
            syslogMessage.HostName = Thread.CurrentPrincipal.Identity.Name;

            if(component == 1)
            {
                WCFComponent_1.EventLogSerialize(syslogMessage);
            }
            else
            {
                WCFComponent_2.XmlSerialize(syslogMessage);
            }

            


            //DEKRIPTUJ



            return false;
        }
    }
}
