using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer.Services
{
    /// <summary>
    /// Invoker based security service demo
    /// </summary>
    [ServiceContract]
    public interface IInvokerServiceDemo
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        void DoWorkNoPermission();
    }
}
