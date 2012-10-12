using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.Security.Principal;
using WCFServer.DAL;

namespace WCFServer.Security.Simple
{
    /// <summary>
    /// Authorization Policy for Security by Principal
    /// </summary>
    class SimpleAuthorizationPolicy : IAuthorizationPolicy
    {
        #region IAuthorizationPolicy Members

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            if (evaluationContext.Properties.ContainsKey("Identities"))
            {
                List<IIdentity> identities = evaluationContext.Properties["Identities"] as List<IIdentity>;
                //get the custom username/password validator identity
                IIdentity identity = identities.FirstOrDefault(i => i.AuthenticationType == "CustomUsernameValidator");

                //find the user this identity belongs to
                var user = UserStore.GetUserByUsername(identity.Name);

                //create a custom principal with the roles in it.
                SimplePrincipal simpleprincipal = new SimplePrincipal(identity,user.Roles );

                //populate the evaluation context's principal.
                evaluationContext.Properties["Principal"] = simpleprincipal;

                return true;
            }
            else
                return false;
        }

        public System.IdentityModel.Claims.ClaimSet Issuer { get; private set; }

        #endregion

        #region IAuthorizationComponent Members

        public string Id { get; private set; }

        #endregion
    }
}
