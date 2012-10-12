using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Reflection;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using WCFServer.DAL;

namespace WCFServer.Security.CustomSecurityCheck
{
    /// <summary>
    /// Invoker Security check behaviors attibute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class CustomSecurityCheckAttribute : Attribute, IOperationBehavior, IServiceBehavior
    {
        public enum CheckType
        {
            UserLoggedIn,
            HasRole,
            FromIP
        }

        public class CheckItem
        {
            public CheckType Type { get; set; }
            public string value { get; set; }
        }

        public readonly List<CheckItem> CheckList = new List<CheckItem>();

        public CustomSecurityCheckAttribute(CheckType type)
        {
            CheckList.Add(new CheckItem
            {
                Type = type
            });
        }

        public CustomSecurityCheckAttribute(CheckType type, string value)
        {
            CheckList.Add(new CheckItem
            {
                Type = type,
                value = value
            });
        }

        public CustomSecurityCheckAttribute(List<CheckItem> checks)
        {
            CheckList.AddRange(checks);
        }

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            //Injecting an invoker to the IOperationBehavior, note the passing of the previous invoker 
            //so they are chained
            dispatchOperation.Invoker = new CustomSecurityCheckInvoker(dispatchOperation.Invoker, this.CheckList.ToArray());
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            //Go over all endpoints of this service
            foreach (var endpoint in serviceHostBase.Description.Endpoints)
            {
                //for each operation (eg. method)
                foreach (var operation in endpoint.Contract.Operations)
                {
                    //if an Invoker is already preset, merge Checklists, otherwise, create new
                    if (operation.Behaviors.Contains(typeof(CustomSecurityCheckAttribute)))
                    {
                        var customsecuritychecksbehavior = operation.Behaviors[typeof(CustomSecurityCheckAttribute)] as CustomSecurityCheckAttribute;
                        customsecuritychecksbehavior.CheckList.AddRange(this.CheckList);
                    }
                    else
                    {
                        operation.Behaviors.Add(new CustomSecurityCheckAttribute(CheckList));
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
