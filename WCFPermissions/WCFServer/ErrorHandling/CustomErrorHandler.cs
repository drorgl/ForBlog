using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFServer.ErrorHandling
{
    /// <summary>
    /// Custom Error Handler implementation for demo
    /// </summary>
    class CustomErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public bool HandleError(Exception error)
        {
            Helper.Log("Error {0}",error.Message);
            return false;
        }

        /// <summary>
        /// Convert the Exception/Fault to a custom fault for the client
        /// </summary>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="fault"></param>
        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            //check if the error is already a fault, which means it was handled in the operation level.
            if (error != null)
            {
                var errtype = error.GetType();
                if ((errtype == typeof(FaultException)) || (errtype.GetGenericTypeDefinition() == typeof(FaultException<>)))
                    return;
            }

            //If it wasn't, we'll generate a custom fault with the details in the fault.
            Helper.Log("ProvideFault called. Converting Exception to GreetingFault....");
            FaultException<CustomFault> fe = new FaultException<CustomFault>(new CustomFault(error.Message,"additional text"), error.Message,new FaultCode("code123"),"On line 123");
            MessageFault mfault = fe.CreateMessageFault();
            fault = Message.CreateMessage(
              version,
              mfault,
              OperationContext.Current.IncomingMessageHeaders.Action
            );
        }

        #endregion
    }
}
