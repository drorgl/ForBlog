using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer.Services
{
    /// <summary>
    /// Claims based security authorization demo service
    /// </summary>
    [ServiceContract]
    public interface IAAServiceDemo
    {
        [OperationContract]
        void DoWork();
    }
}
