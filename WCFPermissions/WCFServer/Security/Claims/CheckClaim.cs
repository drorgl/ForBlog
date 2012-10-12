using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.ServiceModel;

namespace WCFServer.Security.Claims
{
    /// <summary>
    /// Claims Checking Helpers
    /// </summary>
    public class CheckClaim
    {
        /// <summary>
        /// Which check to perform
        /// </summary>
        public enum CheckItemType
        {
            Existance,
            Value
        }

        /// <summary>
        /// A check item for each check
        /// </summary>
        public class CheckItem
        {
            public CheckItemType CheckType { get; set; }
            public string claimType { get; set; }
            public string right { get; set; }
            public bool exists { get; set; }
            public object value { get; set; }
        }

        /// <summary>
        /// Perform the actual check 
        /// </summary>
        public static bool Check(CheckItem[] checks)
        {
            AuthorizationContext authContext = ServiceSecurityContext.Current.AuthorizationContext;

            foreach (var check in checks)
            {
                switch (check.CheckType)
                {
                    //checking for an existance of a claim
                    case CheckItemType.Existance:
                        var claimexists = authContext.ClaimSets.SelectMany(i => i.FindClaims(check.claimType, check.right)).Any();
                        if (check.exists != claimexists)
                            return false;
                        break;
                    //comparing claim value with requested check
                    case CheckItemType.Value:
                        var claim = authContext.ClaimSets.SelectMany(i => i.FindClaims(check.claimType, check.right)).FirstOrDefault();
                        if (claim != check.value)
                            return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return true;
            }

            return true;
        }

        /// <summary>
        /// Retrieves a claim value if exists or null
        /// </summary>
        public static object GetClaim(string claimType, string right)
        {
            AuthorizationContext authContext = ServiceSecurityContext.Current.AuthorizationContext;
            var claim = authContext.ClaimSets.SelectMany(i => i.FindClaims(claimType, right)).FirstOrDefault();
            if (claim != null)
                return claim.Resource;
            return null;
        }

        /// <summary>
        /// Retrieves a claim value if exists or null
        /// </summary>
        public static T GetClaim<T>(string claimType, string right)
        {
            var claim = GetClaim(claimType, right);
            if (claim != null)
                return (T)claim;

            return default(T);
        }


    }
}
