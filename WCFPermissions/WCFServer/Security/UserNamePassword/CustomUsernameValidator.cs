using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using WCFServer.DAL;

namespace WCFServer.Security
{
    /// <summary>
    /// Custom Username/password validator, the reason that there is no access to any Context is that there should be no logic here,
    /// this class should first check if there is permission to the user to even access this service
    /// </summary>
    public class CustomUsernameValidator :  UserNamePasswordValidator
    {
        public override void
         Validate(string userName, string password)
        {
            if (!UserStore.ValidateUser(userName, password))
            {
                throw new SecurityTokenException("Unauthorized");
            }
        }
    }
}
