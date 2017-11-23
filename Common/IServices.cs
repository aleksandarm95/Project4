using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IServices
    {
        [OperationContract]
        bool Send(byte[] message);

        [OperationContract]
        bool SendTry(byte[] message, byte[] signature);
        [OperationContract]
        bool CheckIfPrimary();

        [OperationContract]
        void SendKeys(byte[] keys);
    }
}
