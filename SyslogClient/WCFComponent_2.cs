using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Xml.Serialization;
using System.IO;

namespace SyslogClient
{
    public class WCFComponent_2
    {
        public WCFComponent_2()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:25001/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }

        public static void XmlSerialize(SyslogMessage syslogMessage)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SyslogMessage>));
            StreamReader streamReader = null;
            StreamWriter streamWriter = null;
            List<SyslogMessage> syslogList = new List<SyslogMessage>();
            try
            {
                streamReader = new StreamReader(@"..\..\SyslogFile.xml");
                syslogList = (List<SyslogMessage>)xmlSerializer.Deserialize(streamReader);
                streamReader.Close();
            }
            catch 
            {
                //slucaj ako ne postoji fajl
                streamReader.Close();
                streamWriter = new StreamWriter(@"..\..\SyslogFile.xml");
                xmlSerializer.Serialize(streamWriter, syslogList);
                streamWriter.Close();
            }

            syslogList.Add(syslogMessage);
            streamWriter = new StreamWriter(@"..\..\SyslogFile.xml");
            xmlSerializer.Serialize(streamWriter, syslogList);
            streamWriter.Close();


        }
    }
}
