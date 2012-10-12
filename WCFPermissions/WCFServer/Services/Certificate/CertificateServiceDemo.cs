using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IdentityModel.Policy;
using WCFServer.DAL;
using System.IdentityModel.Claims;
using WCFServer.Security;
using WCFServer.Security.Claims;

namespace WCFServer.Services
{
    /// <summary>
    /// Certificate based authentication service demo
    /// </summary>
    public class CertificateServiceDemo : ICertificateServiceDemo
    {
        public void DoWork()
        {
            var user = CheckClaim.GetClaim<User>("User", Rights.Identity);

            Helper.Log("CertificateServiceDemo.DoWork() was called for user: {0}", user.FullName);
        }


    }
}
