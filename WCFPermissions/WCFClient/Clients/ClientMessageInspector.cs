using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace WCFClient.Clients
{
    /// <summary>
    /// Client Message Inspector
    /// <para>Simple demo how to add a custom header to all outgoing messages</para>
    /// </summary>
    public class ClientMessageInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader("InspectorHeader", "ns", "Value InspectorHeader"));
            return null;
        }

        #endregion
    }
}
