﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;
using System.ServiceModel;
using Manager;

namespace SyslogClient
{
    public class Services : IServices
    {
        public bool SendTry(byte[] message, byte[] signature)
        {
            var m = Encoding.ASCII.GetString(message);
            X509Certificate2 clientCertificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "client_sign");
            if (DigitalSignature.Verify(m, "SHA1", signature, clientCertificate))
            {
                Console.WriteLine("Digital Signature is valid.");
                SyslogMessage syslogMessage = new SyslogMessage();
                var arrayRecieved = System.Text.Encoding.UTF8.GetString(message).Split('`');

                int component = Convert.ToInt32(arrayRecieved[1]);
                syslogMessage.Severity = Convert.ToInt32(arrayRecieved[0]);
                syslogMessage.Facility = 1;
                syslogMessage.Time = DateTime.Now;
                syslogMessage.Message = arrayRecieved[2];
                syslogMessage.HostName = Thread.CurrentPrincipal.Identity.Name;

                bool successfullyAccessed = false;
                bool groupExists = false;
                if (component == 1)
                {
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;
                    var groups = ((WindowsIdentity) (principal.Identity)).Groups;
                    if (groups != null)
                        foreach (IdentityReference group in groups)
                        {
                            SecurityIdentifier sid = (SecurityIdentifier) @group.Translate(typeof(SecurityIdentifier));
                            var name = sid.Translate(typeof(NTAccount));
                            if (name.Value == "Reader")
                            {
                                groupExists = true;
                                break;
                            }
                        }
                    if (groupExists)
                    {
                        Audit.AuthorizationSuccess(principal.Identity.Name, syslogMessage);

                        Console.WriteLine("ExecuteCommand() passed for user {0}.", principal.Identity.Name);
                        successfullyAccessed = true;
                    }
                    else
                    {
                        Audit.AuthorizationFailed(principal.Identity.Name, syslogMessage);
                        Console.WriteLine("ExecuteCommand() failed for user {0}.", principal.Identity.Name);
                        successfullyAccessed = false;
                    }
                    //successfullyAccessed = WCFComponent_1.EventLogSerialize(syslogMessage);
                }
                else
                {
                    successfullyAccessed = WCFComponent_2.XmlSerialize(syslogMessage);
                }

                string messageToSend = "";
                if (successfullyAccessed)
                {
                    messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.HostName.ToString() + "\t" +
                                    "SUCCESSFULLY accessed" +
                                    syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() +
                                    "\t" + syslogMessage.Message;
                }
                else
                {
                    messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.HostName.ToString() + "\t" +
                                    "FAILED accessed" +
                                    syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() +
                                    "\t" + syslogMessage.Message;
                }

                bool first = false;
                //string syslogServerCert1 = "syslog";
                //string syslogServerCert2 = "syslog2";
                //string syslogClient_sign = "syslogclient_sign";
                try
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    //X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, syslogServerCert1);
                    //EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:26000/SecurityService"),
                    //    new X509CertificateEndpointIdentity(srvCert));
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
            Console.WriteLine("Digital Signature is invalid.");
            return false;
        }

        public bool CheckIfPrimary()
        {
            return false;
        }

        public bool Send(byte[] message)
        {
            X509Certificate2 clientCertificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "client_sign");


            SyslogMessage syslogMessage = new SyslogMessage();
            var arrayRecieved = System.Text.Encoding.UTF8.GetString(message).Split('`');

            int component = Convert.ToInt32(arrayRecieved[1]);
            syslogMessage.Severity = Convert.ToInt32(arrayRecieved[0]);
            syslogMessage.Facility = 1;
            syslogMessage.Time = DateTime.Now;
            syslogMessage.Message = arrayRecieved[2];
            syslogMessage.HostName = Thread.CurrentPrincipal.Identity.Name;

            bool successfullyAccessed = false;

            if(component == 1)
            {
               // successfullyAccessed = WCFComponent_1.EventLogSerialize(syslogMessage);
            }
            else
            {
                successfullyAccessed = WCFComponent_2.XmlSerialize(syslogMessage);
            }

            string messageToSend = "";
            if (successfullyAccessed)
            {
                messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.HostName.ToString() + "\t" + "SUCCESSFULLY accessed" + 
                    syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() + "\t" + syslogMessage.Message;
            }
            else
            {
                messageToSend = syslogMessage.Time.ToString() + "\t" + syslogMessage.HostName.ToString() + "\t" + "FAILED accessed" + 
                    syslogMessage.Facility.ToString() + "\t" + syslogMessage.Severity.ToString() + "\t" + syslogMessage.Message;
            }

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
