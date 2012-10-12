using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Principal;
using System.Data;
using System.Security;
using System.IdentityModel.Tokens;
using System.IdentityModel.Claims;
using WCFServer.DAL;

namespace WCFServer.Security
{
    /// <summary>
    /// Custom Authorization Policy for Username/Password validation
    /// <para>
    ///     This class retrieves the identity from the Properties sent from the Authentication Manager
    ///     and stores the Authorized content for the user in the Properties
    /// </para>
    /// </summary>
    public class CustomUsernameAuthorizationPolicy : IAuthorizationPolicy
    {
        public CustomUsernameAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
            //Issuer = ClaimSet.System;
        }

        #region IAuthorizationPolicy Members

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {


            if (evaluationContext.Properties.ContainsKey("Identities"))
            {
                List<IIdentity> identities = evaluationContext.Properties["Identities"] as List<IIdentity>;
                IIdentity identity = identities.FirstOrDefault(i=>i.AuthenticationType == "CustomUsernameValidator");

                GenericPrincipal genprincipal = new GenericPrincipal(identity, null);
                evaluationContext.Properties["Principal"] = genprincipal;

                //find a user for this identity
                var user = UserStore.GetUserByUsername(identity.Name);

                //Add a claim with the user entity
                evaluationContext.AddClaimSet(this, new DefaultClaimSet(new Claim("User", user, Rights.Identity)));

                return true;
            }
            else
                return false;



            //evaluationContext.ClaimSets.
            //var name = ServiceSecurityContext.Current.AuthorizationContext .AuthorizationContext.Properties["name"];
            //OperationContext.Current

            //IPrincipal user = OperationContext.Current.IncomingMessageProperties["Principal"] as IPrincipal;

            //var user = OperationContext.Current.IncomingMessageProperties["Principal"] as DAL.User;//evaluationContext.Properties["Principal"] as DAL.User;
            //if (user == null)
            //    throw new SecurityTokenException("Unauthorized");

            //evaluationContext.AddClaimSet(this, new DefaultClaimSet(new Claim("UserId", user.UserId, Rights.Identity)));


            //const String HttpRequestKey = "httpRequest";
            //const String UsernameHeaderKey = "x-ms-credentials-username";
            //const String PasswordHeaderKey = "x-ms-credentials-password";
            //const String IdentitiesKey = "Identities";
            //const String PrincipalKey = "Principal";

            //// Check if the properties of the context has the identities list 
            //if (evaluationContext.Properties.Count > 0 ||
            //  evaluationContext.Properties.ContainsKey(IdentitiesKey) ||
            //  !OperationContext.Current.IncomingMessageProperties.ContainsKey(HttpRequestKey))
            //    return false;

            //// get http request
            //var httpRequest = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestKey];

            //// extract credentials
            //var username = httpRequest.Headers[UsernameHeaderKey];
            //var password = httpRequest.Headers[PasswordHeaderKey];

            //// verify credentials complete
            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            //    return false;

            //// Get or create the identities list 
            //if (!evaluationContext.Properties.ContainsKey(IdentitiesKey))
            //    evaluationContext.Properties[IdentitiesKey] = new List<IIdentity>();
            //var identities = (List<IIdentity>)evaluationContext.Properties[IdentitiesKey];

            // lookup user
            //using (var con = ServiceLocator.Current.GetInstance<IDbConnection>())
            //{
            //    using (var userDao = ServiceLocator.Current.GetDao<IUserDao>(con))
            //    {
            //        var user = userDao.GetUserByUsernamePassword(username, password);

            //        //evaluationContext.AddClaimSet(this, new DefaultClaimSet(this.Issuer, roleClaims));
            //    }
            //}


            //check that!
            return true;
        }

        public System.IdentityModel.Claims.ClaimSet Issuer {get;private set;}

        #endregion

        #region IAuthorizationComponent Members

        public string Id {get;private set;}

        #endregion
    }
}
