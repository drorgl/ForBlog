using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using WCFServer.DAL;
using System.ServiceModel.Channels;
using System.IdentityModel.Policy;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace WCFServer.Security
{
    /// <summary>
    /// Custom Authentication Manager
    /// <para>This class authenticates the message, either valid or from a trustworthy source</para>
    /// <para>for example, say you added a custom header to all your messages, this is where you make sure the message contains that header</para>
    /// </summary>
    class CustomAuthenticationManager : ServiceAuthenticationManager
    {
        public override ReadOnlyCollection<IAuthorizationPolicy> Authenticate(ReadOnlyCollection<IAuthorizationPolicy> authPolicy, 
            Uri listenUri, 
            ref System.ServiceModel.Channels.Message message)
        {
            return authPolicy;
        }
    }
}
