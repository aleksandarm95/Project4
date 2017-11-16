using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Client
{
    public class ClientProxy : ChannelFactory<IServices>, IServices, IDisposable
    {
        IServices factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public bool Send(byte[] message)
        {
            return factory.Send(message);
        }
    }
}
