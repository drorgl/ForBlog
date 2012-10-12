using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;

namespace WCFClient.Clients
{
    /// <summary>
    /// Client Endpoint behavior and behavior configuration
    /// </summary>
    public class ClientEndpointBehavior : BehaviorExtensionElement, IEndpointBehavior 
    {

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           
        }

        /// <summary>
        /// Endpoint Add client behavior
        /// <para>Will add the client message inspector to this behavior</para>
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new ClientMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        #endregion

        #region behavior configuration implementation

        public override Type BehaviorType
        {
            get { return typeof(ClientEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ClientEndpointBehavior();
        }

        #endregion
    }
}
