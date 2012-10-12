using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFServer.ErrorHandling;
using System.Diagnostics;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.ServiceModel.Channels;
using System.Security.Permissions;

namespace WCFServer.Services
{
    /// <summary>
    /// Simple Service Demo
    /// </summary>
    [CustomBehavior(typeof(CustomErrorHandler))]
    public class ServiceDemo : IServiceDemo
    {
        public void DoWork()
        {
            //check for CustomHeaderValue custom header
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            if (headers.FindHeader("CustomHeaderValue", "ns") != -1)
            {
                var customheader = headers.GetHeader<string>("CustomHeaderValue", "ns");
                if (customheader != null)
                {
                    Helper.Log("ServiceDemo.DoWork() was called with custom header CustomHeaderValue: {0}", customheader);
                }
            }

            //check for InspectorHeader custom header
            if (headers.FindHeader("InspectorHeader", "ns") != -1)
            {
                var customheader = headers.GetHeader<string>("InspectorHeader", "ns");
                if (customheader != null)
                {
                    Helper.Log("ServiceDemo.DoWork() was called with header from Inspector InspectorHeader: {0}", customheader);
                }
            }
        }

        /// <summary>
        /// Throw a fault test Operation
        /// </summary>
        /// <param name="message"></param>
        public void ThrowException(string message)
        {
            Helper.Log("ServiceDemo.ThrowException() was called");
            throw new FaultException("fault message", new FaultCode("fault code"));
        }


        /// <summary>
        /// Throw a Custom Fault test Operation
        /// </summary>
        /// <param name="message"></param>
        public void ThrowCustomException(string message)
        {
            Helper.Log("ServiceDemo.ThrowCustomException() was called");
            throw new FaultException<CustomFault>(new CustomFault(message, "additional data"), new FaultReason("fault reason"), new FaultCode("fault code"));
        }

    }
}

