using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Security.Permissions;

namespace WCFServer.Services
{
    /// <summary>
    /// Simple Username/Password validation service demo
    /// </summary>
    public class SimpleUsernameServiceDemo : ISimpleUsernameServiceDemo
    {
        public void DoWork()
        {
            var username1 = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            Helper.Log("Username: {0}",username1);
        }

        /// <summary>
        /// Demo Operation that tests for role Admin
        /// </summary>
        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void TestRoleAdmin()
        {
        }

        /// <summary>
        /// Demo Operation that tests for role API
        /// </summary>
        [PrincipalPermission(SecurityAction.Demand, Role = "API")]
        public void TestRoleAPI()
        {
        }
    }
}
