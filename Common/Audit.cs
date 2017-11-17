using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        //malo izmenjati ovo
        const string SourceName = "SecurityManager.Audit";
        const string LogName = "MySecTest";

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        static Audit()
        {
            try
            {
                /// create customLog handle
                string machineName = WindowsIdentity.GetCurrent().Name;

                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void AuthorizationSuccess(string userName, SyslogMessage syslogMessage)
        {
            string UserAuthorizationSuccess = AuditEvents.UserAuthorizationSuccess;
            String.Format(UserAuthorizationSuccess, userName, syslogMessage.Facility, syslogMessage.Severity, syslogMessage.Time.ToString(),syslogMessage.HostName, syslogMessage.Message);

            if (customLog != null)
            {
                customLog.WriteEntry(UserAuthorizationSuccess, EventLogEntryType.Information);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthorizationSuccess));
            }
        }
        
        public static void AuthorizationFailed(string userName,SyslogMessage syslogMessage)
        {
            string UserAuthorizationFailed = AuditEvents.UserAuthorizationFailed;
            String.Format(UserAuthorizationFailed, userName, syslogMessage.Facility, syslogMessage.Severity, syslogMessage.Time.ToString(), syslogMessage.HostName, syslogMessage.Message); 

            if (customLog != null)
            {
                customLog.WriteEntry(UserAuthorizationFailed, EventLogEntryType.Error);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthorizationFailed));
            }
        }
    }
}
