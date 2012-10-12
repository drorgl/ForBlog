using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer.Services
{
    /// <summary>
    /// Simple Username/Password validation service demo
    /// </summary>
    [ServiceContract]
    public interface ISimpleUsernameServiceDemo
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        void TestRoleAdmin();

        [OperationContract]
        void TestRoleAPI();
    }
}
