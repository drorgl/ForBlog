using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFServer.Security.Claims
{
    /// <summary>
    /// Claim Checking behaviors for operations and services
    /// </summary>
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class CheckClaimAttribute : Attribute, IOperationBehavior, IServiceBehavior
    {
        public readonly List<CheckClaim.CheckItem> Checklist = new List<CheckClaim.CheckItem>();

        /// <summary>
        /// Looks for a claimType/right and makes sure it exists or doesn't exists in any claimset
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="right"></param>
        /// <param name="exists"></param>
        public CheckClaimAttribute(string claimType, string right, bool exists)
        {
            Checklist.Add(
                new CheckClaim.CheckItem
                {
                    CheckType = CheckClaim.CheckItemType.Existance,
                     claimType = claimType,
                     exists = exists,
                     right = right
                }
            );
        }

        /// <summary>
        /// Looks for a claimType/right and makes sure the value is the same as the claim's value
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="right"></param>
        /// <param name="value"></param>
        public CheckClaimAttribute(string claimType, string right, object value)
        {
            Checklist.Add(
                new CheckClaim.CheckItem
                {
                    CheckType = CheckClaim.CheckItemType.Existance,
                    claimType = claimType,
                    value = value,
                    right = right
                }
            );
        }

        internal CheckClaimAttribute(List<CheckClaim.CheckItem> checklist)
        {
            this.Checklist.AddRange(checklist);
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
            //add an invoker that checks for the claims before executing the next invoker in the chain
            dispatchOperation.Invoker = new CheckClaimInvoker(dispatchOperation.Invoker, Checklist.ToArray());
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
                    if (operation.Behaviors.Contains(typeof(CheckClaimAttribute)))
                    {
                        var checkclaimbehavior = operation.Behaviors[typeof(CheckClaimAttribute)] as CheckClaimAttribute;
                        checkclaimbehavior.Checklist.AddRange(this.Checklist);
                    }
                    else
                    {
                        operation.Behaviors.Add(new CheckClaimAttribute(Checklist));
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
