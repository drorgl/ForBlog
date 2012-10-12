using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using WCFServer.DAL;
using System.IdentityModel.Tokens;
using WCFServer.Security.Claims;

namespace WCFServer.Services
{
    /// <summary>
    /// Claims based security authorization demo service
    /// </summary>
    public class AAServiceDemo : IAAServiceDemo
    {
        /// <summary>
        /// Attribute for validating the "User" exists
        /// </summary>
        [CheckClaim("User", "http://schemas.xmlsoap.org/ws/2005/05/identity/right/identity", true)]
        public void DoWork()
        {
            //get the user claim from the claim sets so we can know who we're executing for
            var user = CheckClaim.GetClaim<User>("User", Rights.Identity);

            Helper.Log("AAServiceDemo.DoWork() was called with user: {0}" + user.FullName);

        }
    }
}
