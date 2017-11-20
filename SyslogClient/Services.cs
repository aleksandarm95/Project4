using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;
using System.ServiceModel;

namespace SyslogClient
{
    public class Services : IServices
    {
        public bool CheckIfPrimary()
        {
            return false;
        }

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

            string messageToSend = syslogMessage.Time.ToString() + "\t"+ syslogMessage.HostName.ToString() + "\t" + syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() + "\t" + syslogMessage.Message;

            bool first = false;

            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string address = "net.tcp://localhost:26000/SecurityService";

                using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                {
                    first = proxy.CheckIfPrimary();
                }

                if (first)
                {
                    using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                    {
                        proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                    }
                }
                else
                {
                    try
                    {
                        NetTcpBinding binding1 = new NetTcpBinding();
                        string address1 = "net.tcp://localhost:27000/SecurityService";


                        using (SyslogClientProxy proxy = new SyslogClientProxy(binding1, address1))
                        {
                            proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                        }
                    }
                    catch
                    {
                        using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                        {
                            proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                        }
                    }
                } 
            }
            catch
            {
                NetTcpBinding binding = new NetTcpBinding();
                string address = "net.tcp://localhost:27000/SecurityService";


                using (SyslogClientProxy proxy = new SyslogClientProxy(binding, address))
                {
                    proxy.Send(Encoding.ASCII.GetBytes(messageToSend));
                }      
            }

            return true;
        }
    }
}
