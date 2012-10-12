using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace WCFServer.Logging
{
    /// <summary>
    /// Logging Message Inspector
    /// <para>Will log every message received and sent</para>
    /// </summary>
    class LoggingMessageInspector : IDispatchMessageInspector
    {
        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            //create a copy of the message since a message can be read only once
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            Message dupeRequest = buffer.CreateMessage();

            //log the action
            Helper.Log(dupeRequest.Headers.Action);

            buffer.Close();

            Helper.Log("IDispatchMessageInspector.AfterReceiveRequest() called from " + Helper.GetClientIP());
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            //log all replies
            Helper.Log("IDispatchMessageInspector.BeforeSendReply() called from " + Helper.GetClientIP());
        }

        #endregion
    }
}
