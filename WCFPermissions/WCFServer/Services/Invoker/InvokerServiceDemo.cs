using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFServer.Security.CustomSecurityCheck;
using System.IdentityModel.Tokens;

namespace WCFServer.Services
{
    /// <summary>
    /// Invoker based security service demo
    /// </summary>
    [CustomSecurityCheck( CustomSecurityCheckAttribute.CheckType.UserLoggedIn)]
    public class InvokerServiceDemo : IInvokerServiceDemo
    {
        [CustomSecurityCheck( CustomSecurityCheckAttribute.CheckType.HasRole,"API")]
        public void DoWork()
        {
            //UserNameSecurityToken securityToken = OperationContext.Current.IncomingMessageProperties.Security.IncomingSupportingTokens[0].SecurityToken as System.IdentityModel.Tokens.UserNameSecurityToken;

            //string username = securityToken.UserName;
            //string password = securityToken.Password;
        }

        [CustomSecurityCheck(CustomSecurityCheckAttribute.CheckType.HasRole, "Admin")]
        public void DoWorkNoPermission()
        {
        }
    }
}
