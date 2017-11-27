using System;
using System.ServiceModel;
using Common;

namespace Client
{
    public class ClientProxy : ChannelFactory<IServices>, IServices, IDisposable
    {
        IServices factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = CreateChannel();
        }

        public bool SendTry(byte[] message, byte[] signature)
        {
            try
            {
                return factory.SendTry(message, signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool CheckIfPrimary()
        {
            return factory.CheckIfPrimary();
        }

        public bool Send(byte[] message)
        {
            return factory.Send(message);
        }

        public void SendKeys(byte[] keys)
        {
            factory.SendKeys(keys);
        }
    }
}
