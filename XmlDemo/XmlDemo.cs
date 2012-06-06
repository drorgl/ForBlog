using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace XmlDemo
{
    public enum TestEnum
    {
        val1,
        val2,
        val3
    }

    /// <summary>
    /// Demo for various methods of reading and writing XML in C#
    /// </summary>
    public static class XmlDemo
    {
        /// <summary>
        /// XmlWriter demo code
        /// </summary>
        /// <returns>xml document</returns>
        public static string XmlWriterDemo()
        {
            using (MemoryStream ms = new MemoryStream())
            using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("root");

                xw.WriteStartElement("Cars");
                xw.WriteStartElement("Car");
                xw.WriteAttributeString("Color", "white");
                xw.WriteAttributeString("Radio", "true");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();

                xw.Flush();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// XMLReader demo code
        /// </summary>
        /// <param name="doc">xml document to read</param>
        /// <returns>parsing output</returns>
        public static string XmlReaderDemo(string doc)
        {
            StringBuilder output = new StringBuilder();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(doc)))
            using (XmlReader xr = XmlReader.Create(ms))
            {
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            output.AppendLine("Found Start Element name: " + xr.Name);
                            break;
                        case XmlNodeType.Attribute:
                            output.AppendLine("Found Attribute name: " + xr.Name + " value: " + xr.Value);
                            break;
                        case XmlNodeType.EndElement:
                            output.AppendLine("Found End Element name: " + xr.Name);
                            break;
                    }
                    if (xr.HasAttributes)
                    {
                        while (xr.MoveToNextAttribute())
                            output.AppendLine("Found Attribute name: " + xr.Name + " value: " + xr.Value);
                    }

                }
            }
            return output.ToString();
        }

        /// <summary>
        /// XmlDocument writing demo
        /// </summary>
        /// <returns>xml document</returns>
        public static string XmlDocumentWrite()
        {
            XmlDocument doc = new XmlDocument();
            var xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            doc.AppendChild(xmldecl);

            var root = doc.AppendChild(doc.CreateElement("root"));
            var cars = root.AppendChild(doc.CreateElement("Cars"));
            var car = cars.AppendChild(doc.CreateElement("Car"));

            var color = doc.CreateAttribute("Color");
            color.Value = "white";

            car.Attributes.Append(color);

            var radio = doc.CreateAttribute("Radio");
            radio.Value = "true";

            car.Attributes.Append(radio);

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// XmlDocument reading demo
        /// </summary>
        /// <param name="xmldoc">xml document</param>
        /// <returns>parsing output</returns>
        public static string XmlDocumentRead(string xmldoc)
        {
            StringBuilder output = new StringBuilder();

            XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xmldoc);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmldoc));
            doc.Load(ms);

            var nodes = doc.SelectNodes("/root/Cars/Car");
            foreach (XmlNode node in nodes)
            {
                output.AppendLine("Element name: " + node.Name + " value: " + node.Value);
                foreach (XmlAttribute att in node.Attributes)
                {
                    output.AppendLine("Attribute name: " + att.Name + " value: " + att.Value);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// XDocument writing demo
        /// </summary>
        /// <returns>xml document</returns>
        public static string XElementWrite()
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes")
                    , new XElement("root",
                          new XElement("Cars",
                            new XElement("Car",
                                new XAttribute("Color", "white"), new XAttribute("Radio", true)
                            )
                        )
                    )
                );

            using (MemoryStream ms = new MemoryStream())
            {
                xdoc.Save(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// XDocument reading demo
        /// </summary>
        /// <param name="xmldoc">xml document</param>
        /// <returns>parsing output</returns>
        public static string XElementRead(string xmldoc)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmldoc));
            XDocument xdoc = XDocument.Load(ms);

            StringBuilder output = new StringBuilder();

            var cars = from c in xdoc.Element("root").Element("Cars").Elements("Car")
                       select c;
            foreach (var car in cars)
            {
                output.AppendLine("Found Car element: " + car.Name);
                foreach (var att in car.Attributes())
                {
                    output.AppendLine("Attribute name: " + att.Name + " value: " + att.Value);
                }
            }

            return output.ToString();
        }


        public static void TestExtensions()
        {
            //test extensions

            //test xelement ToEnum
            XElement xelement = new XElement("TestElementEnum", TestEnum.val2);
            var enumback = xelement.ToEnum<TestEnum>(TestEnum.val1);
            if (enumback != TestEnum.val2)
                throw new Exception("Failed to verify XElement.ToEnum");

            //test xattribute ToEnum
            XAttribute xattribute = new XAttribute("TestAttributeEnum", TestEnum.val3);
            var aenumback = xattribute.ToEnum<TestEnum>(TestEnum.val1);
            if (aenumback != TestEnum.val3)
                throw new Exception("Failed to verify XAttribute.ToEnum");



            //test byte array
            byte[] array = new byte[8];
            var random = new Random();
            random.NextBytes(array);

            //test xelement ToByteArray
            xelement = new XElement("TestByteArray", Convert.ToBase64String(array));
            var arrayback = xelement.ToByteArray();

            if (!arrayback.SequenceEqual(array))
                throw new Exception("Failed to verify XElement.ToByteArray");


            //test xattribute ToByteArray
            xattribute = new XAttribute("TestByteArray", Convert.ToBase64String(array));
            arrayback = xattribute.ToByteArray();

            if (!arrayback.SequenceEqual(array))
                throw new Exception("Failed to verify XAttribute.ToByteArray");

        }
    }
}
