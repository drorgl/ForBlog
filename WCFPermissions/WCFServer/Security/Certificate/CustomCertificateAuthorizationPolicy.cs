using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.Security.Principal;
using WCFServer.DAL;
using System.IdentityModel.Claims;

namespace WCFServer.Security
{
    /// <summary>
    /// Certificate Authorization Policy
    /// <para>Will check to see which user this certificate belongs to and add a claim with the user</para>
    /// </summary>
    public class CustomCertificateAuthorizationPolicy : IAuthorizationPolicy
    {
        #region IAuthorizationPolicy Members

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            if (evaluationContext.Properties.ContainsKey("Identities"))
            {
                List<IIdentity> identities = evaluationContext.Properties["Identities"] as List<IIdentity>;
                IIdentity identity = identities.FirstOrDefault(i => i.AuthenticationType == "X509");
                
                GenericPrincipal genprincipal = new GenericPrincipal(identity, null);
                evaluationContext.Properties["Principal"] = genprincipal;

                var user = UserStore.GetUserByCertificate(identity.Name);
                evaluationContext.AddClaimSet(this, new DefaultClaimSet(new Claim("User", user, Rights.Identity)));

                return true;
            }
            else
                return false;
        }

        public System.IdentityModel.Claims.ClaimSet Issuer {get;private set;}

        #endregion

        #region IAuthorizationComponent Members

        public string Id {get;private set;}

        #endregion
    }
}
