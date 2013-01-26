using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PluginTwo
{
    /// <summary>
    /// Data Model for Plugin Two
    /// </summary>
    class PluginTwoModel
    {
        public string textbox { get; set; }
        public string radiogroup { get; set; }

        public PluginTwoModel() { }

        /// <summary>
        /// Initialize the model with XML data
        /// </summary>
        /// <param name="xml"></param>
        public PluginTwoModel(string xml)
        {
            XElement xe = XElement.Parse(xml);
            this.textbox = xe.Element("textbox").Value;
            this.radiogroup = xe.Element("radiogroup").Value;
        }

        /// <summary>
        /// return the model in XML form
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            XElement xe = new XElement("PluginTwo",
                new XElement("textbox", this.textbox),
                new XElement("radiogroup", this.radiogroup)
                );
            return xe.ToString();
        }
    }
}
