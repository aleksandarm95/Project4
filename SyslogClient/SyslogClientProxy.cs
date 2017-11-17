using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SyslogClient
{
    public class SyslogClientProxy : ChannelFactory<IServices>, IServices, IDisposable
    {
        IServices factory;

        public SyslogClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public bool Send(byte[] message)
        {
            return factory.Send(message);
        }
    }
}
