using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace WCFServer.Security.Simple
{
    /// <summary>
    /// Simple Principal containing the User's Roles
    /// </summary>
    class SimplePrincipal : IPrincipal
    {
        private IIdentity _identity;
        private string[] _roles;

        public SimplePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }

        #endregion
    }
}
