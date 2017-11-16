using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SyslogMessage
    {
        private int facility;
        private int severity;

        private DateTime sendTime;
        private string hostName;

        private string message;

        public SyslogMessage()
        {

        }

        public int Facility
        {
            get
            {
                return facility;
            }

            set
            {
                facility = value;
            }
        }

        public int Severity
        {
            get
            {
                return severity;
            }

            set
            {
                severity = value;
            }
        }

        public DateTime SendTime
        {
            get
            {
                return sendTime;
            }

            set
            {
                sendTime = value;
            }
        }

        public string HostName
        {
            get
            {
                return hostName;
            }

            set
            {
                hostName = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }
    }
}
