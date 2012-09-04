using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace XmlIllegalCharacterTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //Write XML 1.1 file
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Encoding = Encoding.UTF8;
            xws.Indent = true;
            //Disable character checking
            xws.CheckCharacters = false;

            System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb, xws);
            //write your own header
            xw.WriteProcessingInstruction("xml", "version='1.1'");
            XElement doc = new XElement("root");
            doc.Add(
                    new XElement("test", 
                            new XAttribute("val", "\x03")));
            //use WriteTo instead of Save
            doc.WriteTo(xw);
            xw.Close();

            //Print XML contents to console
            Console.WriteLine(sb.ToString());
            

            //Read XML 1.1 file
            TextReader tr = new StringReader(sb.ToString());
            tr.ReadLine(); //skip Version number '1.1' is invalid. exception

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.CheckCharacters = false;
            XmlReader xr = XmlReader.Create(tr, xrs);

            var xmldoc = XElement.Load(xr);
            foreach (var e in xmldoc.Elements())
            {
                Console.Write("Element: {0}", e.Name);
                foreach (var a in e.Attributes())
                    Console.Write(" Attribute: {0}={1}", a.Name, a.Value);
                Console.WriteLine();
            }
                
        }
    }
}
