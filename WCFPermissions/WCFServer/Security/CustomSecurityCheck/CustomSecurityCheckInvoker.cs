using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.ServiceModel;
using WCFServer.DAL;

namespace WCFServer.Security.CustomSecurityCheck
{
    /// <summary>
    /// Security by Invoker
    /// </summary>
    public class CustomSecurityCheckInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _invoker;
        private readonly CustomSecurityCheckAttribute.CheckItem[] _checks;

        public CustomSecurityCheckInvoker(IOperationInvoker invoker, CustomSecurityCheckAttribute.CheckItem[] checkfunctions)
        {
            this._invoker = invoker;
            this._checks = checkfunctions;
        }

        /// <summary>
        /// Retrieves the method name for the check.
        /// </summary>
        /// <returns></returns>
        private string GetInvokerMethod()
        {
            PropertyInfo pi = _invoker.GetType().GetProperty("MethodName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null)
            {
                var val = pi.GetValue(_invoker, null);
                return val.ToString();
            }

            return string.Empty;
        }

        private void Validate()
        {
            foreach (var check in _checks)
            {
                switch (check.Type)
                {
                    case CustomSecurityCheckAttribute.CheckType.FromIP:
                            {
                                var clientip = Helper.GetClientIP();
                                if (clientip != check.value)
                                    throw new SecurityTokenException("Unauthorized");
                            }
                        break;
                    case CustomSecurityCheckAttribute.CheckType.HasRole:
                            {
                                UserNameSecurityToken securityToken = OperationContext.Current.IncomingMessageProperties.Security.IncomingSupportingTokens[0].SecurityToken as System.IdentityModel.Tokens.UserNameSecurityToken;

                                string username = securityToken.UserName;
                                var user = UserStore.GetUserByUsername(username);
                                if ((user == null) || (!user.Roles.Contains(check.value)))
                                    throw new SecurityTokenException("Unauthorized");
                            }
                        break;
                    case CustomSecurityCheckAttribute.CheckType.UserLoggedIn:
                            {
                                UserNameSecurityToken securityToken = OperationContext.Current.IncomingMessageProperties.Security.IncomingSupportingTokens[0].SecurityToken as System.IdentityModel.Tokens.UserNameSecurityToken;

                                string username = securityToken.UserName;
                                string password = securityToken.Password;

                                if (!UserStore.ValidateUser(username, password))
                                    throw new SecurityTokenException("Unauthorized");
                            }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #region IOperationInvoker Members

        public object[] AllocateInputs()
        {
            return _invoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            //execute validate before invoking the previous invoker in the chain
            Validate();

            return _invoker.Invoke(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            //execute validate before invoking the previous invoker in the chain
            Validate();
            return _invoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return _invoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return _invoker.IsSynchronous; }
        }

        #endregion
    }
}
