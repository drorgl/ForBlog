using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer.Services
{
    /// <summary>
    /// Certificate based authentication service demo
    /// </summary>
    [ServiceContract]
    public interface ICertificateServiceDemo
    {
        [OperationContract]
        void DoWork();
    }
}
