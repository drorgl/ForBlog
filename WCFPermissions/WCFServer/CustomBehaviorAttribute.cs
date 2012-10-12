using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFServer
{
    /// <summary>
    /// A "Generic" behavior that can add error handlers and message inspectors based on types passed
    /// </summary>
    public class CustomBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type[] behaviorTypes;

        #region IServiceBehavior Members

        public CustomBehaviorAttribute(Type[] behaviorTypes)
        {
            this.behaviorTypes = behaviorTypes;
        }

        public CustomBehaviorAttribute(Type behaviorType)
        {
            if (behaviorType == null)
                throw new ArgumentNullException("Behavior type cannot be null");

            this.behaviorTypes = new Type[] {behaviorType };
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            var errorhandlers = this.behaviorTypes.Where(t => t.GetInterfaces().Any(i=>i == typeof(IErrorHandler))).ToList();
            var messageinspectors = this.behaviorTypes.Where(t => t.GetInterfaces().Any(i => i == typeof(IDispatchMessageInspector))).ToList();

            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                //add error handlers to all channel dispatchers
                foreach (var errorhandler in errorhandlers)
                {
                    var handler = (IErrorHandler)Activator.CreateInstance(errorhandler);
                    cd.ErrorHandlers.Add(handler);
                }

                //add message inspectors to all endpoints
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    foreach (var messageinspector in messageinspectors)
                    {
                        var inspector = (IDispatchMessageInspector)Activator.CreateInstance(messageinspector);
                        ed.DispatchRuntime.MessageInspectors.Add(inspector);
                    }
                }
            }

        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }

        #endregion

       
    }
}
