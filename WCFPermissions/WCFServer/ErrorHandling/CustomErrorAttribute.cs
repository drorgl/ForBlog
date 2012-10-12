using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFServer.ErrorHandling
{
    /// <summary>
    /// Custom Error behavior attribute
    /// </summary>
    public class CustomErrorAttribute :  Attribute, IServiceBehavior
    {

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }
        
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            //Add error handling to all channnels.
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                cd.ErrorHandlers.Add(new CustomErrorHandler());
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}
