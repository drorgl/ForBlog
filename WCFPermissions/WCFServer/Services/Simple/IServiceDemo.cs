using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFServer.ErrorHandling;

namespace WCFServer.Services
{
    /// <summary>
    /// Simple Service Demo
    /// </summary>
    [ServiceContract]
    public interface IServiceDemo
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        void ThrowException(string message);

        [OperationContract]
        [FaultContract(typeof(CustomFault))]
        void ThrowCustomException(string message);
    }
}
