using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SyslogMessage
    {
        public int Facility { get; set; }

        public int Severity { get; set; }

        public DateTime Time { get; set; }

        public string ClientName { get; set; }

        public string Message { get; set; }

        public string SuccessfullyAccessed { get; set; }
    }
}
