using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace WCFServer
{
    /// <summary>
    /// Helper methods
    /// </summary>
    class Helper
    {
        /// <summary>
        /// Retrieve's the Client's IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            //var webhttpcurrent = WebOperationContext.Current;
            //if (webhttpcurrent != null)
            //    if (!string.IsNullOrEmpty(webhttpcurrent.Request.UserHostAddress))
            //        return webhttpcurrent.Request.UserHostAddress;

            //Check through OperationContext
            OperationContext context = OperationContext.Current;
            if (context != null)
            {
                MessageProperties messageProperties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpointProperty.Address;
            }

            //If not found, attempt through HttpContext
            var httpcurrent = HttpContext.Current;
            if (httpcurrent != null)
                if (!string.IsNullOrEmpty(httpcurrent.Request.UserHostAddress))
                    return httpcurrent.Request.UserHostAddress;

            return string.Empty;
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        public static void Log(string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }
    }
}
