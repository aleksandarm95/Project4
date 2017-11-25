using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Security.Principal;

namespace SyslogClient
{
    public class WCFComponent_2
    {
        public WCFComponent_2()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:44444/SecurityService";

            ServiceHost host = new ServiceHost(typeof(Services));
            host.AddServiceEndpoint(typeof(IServices), binding, address);
            host.Open();
        }

        public static bool XmlSerialize(SyslogMessage syslogMessage)
        {
            WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;

            syslogMessage.successfullyAccessed = "FAILED accessed";
            /*if (principal.IsInRole(Permissions.Read.ToString()))
            {
                syslogMessage.successfullyAccessed = "SUCCESSFULLY accessed";
                allowed = true;
            }*/

            bool groupExists = false;
            
            var groups = ((WindowsIdentity)(principal.Identity)).Groups;
            if (groups != null)
                foreach (IdentityReference group in groups)
                {
                    SecurityIdentifier sid = (SecurityIdentifier)@group.Translate(typeof(SecurityIdentifier));
                    var fullName = sid.Translate(typeof(NTAccount));
                    if (fullName.ToString().Contains("Reader"))
                    {
                        syslogMessage.successfullyAccessed = "SUCCESSFULLY accessed";
                        groupExists = true;
                        break;
                    }
                }

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


            return groupExists;
        }
    }
}
