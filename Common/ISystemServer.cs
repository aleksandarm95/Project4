using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ISystemServer
    {
        [OperationContract]
        bool Send(byte[] message);

        [OperationContract]
        void SendDatabase(string message);

        [OperationContract]
        bool CheckIsAlive();
    }
}
