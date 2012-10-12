using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.IdentityModel.Tokens;

namespace WCFServer.Security.Claims
{
    /// <summary>
    /// Claims checking invoker
    /// <para>Will validate a claim's existance of value</para>
    /// </summary>
    class CheckClaimInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _invoker;
        private readonly CheckClaim.CheckItem[] _checks;

        public CheckClaimInvoker(IOperationInvoker invoker, CheckClaim.CheckItem[] checkitems)
        {
            this._invoker = invoker;
            this._checks = checkitems;
        }

        private void Validate()
        {
            if (!CheckClaim.Check(_checks))
                throw new SecurityTokenException("Unauthorized");
        }

        #region IOperationInvoker Members

        public object[] AllocateInputs()
        {
            return _invoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Validate();

            return _invoker.Invoke(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
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
