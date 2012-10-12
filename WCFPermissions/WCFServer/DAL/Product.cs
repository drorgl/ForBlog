using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WCFServer.DAL
{
    /// <summary>
    /// Sample Data Contract for Product
    /// </summary>
    [DataContract]
    class Product
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
