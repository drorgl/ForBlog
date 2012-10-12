using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WCFServer.ErrorHandling
{
    /// <summary>
    /// Custom Fault Data Contract
    /// </summary>
    [DataContract]
    public class CustomFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string AdditionalData { get; set; }

        public CustomFault(string message, string additionalData)
        {
            this.Message = message;
            this.AdditionalData = additionalData;
        }
    }
}
