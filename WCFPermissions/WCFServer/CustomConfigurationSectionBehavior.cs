using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace WCFServer
{
    /*
     * we need this type so the CustomConfigurationSection can return a type before its created on CreateBehavior()
     */


    /// <summary>
    /// A "Generic" behavior
    /// </summary>
    class CustomConfigurationSectionBehavior : IServiceBehavior
    {
        private IServiceBehavior _instance;
        public CustomConfigurationSectionBehavior(IServiceBehavior instance)
        {
            this._instance = instance;
        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            _instance.AddBindingParameters(serviceDescription, serviceHostBase, endpoints, bindingParameters);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            _instance.ApplyDispatchBehavior(serviceDescription, serviceHostBase);
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            _instance.Validate(serviceDescription, serviceHostBase);
        }

        #endregion
    }
}
