using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCFServer.Security
{
    /// <summary>
    /// Custom Authorization Manager
    /// <para>An Authorization Manager is responsible for evaluating all Authorization Policies, build 
    /// a ClaimSet and execute CheckAccessCore for each Operation, making sure its allowed to execute</para>
    /// </summary>
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        /// <summary>
        /// Each Operation is checked against CheckAccessCore to see if its allowed to execute.
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            return base.CheckAccessCore(operationContext);
        }
    }
}
