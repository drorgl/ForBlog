using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace XmlDemo
{
    class Program
    {

        

        static void Main(string[] args)
        {
            int iterations = 10000;
            

            Console.WriteLine("XmlWriter/XmlReader demos:");

            

            var xmlwriterdemo = XmlDemo.XmlWriterDemo();
            Console.WriteLine("XmlWriter demo:");
            Console.WriteLine(xmlwriterdemo);
            var xmlreaderdemo = XmlDemo.XmlReaderDemo(xmlwriterdemo);
            Console.WriteLine("XmlReader demo:");
            Console.WriteLine(xmlreaderdemo);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                xmlwriterdemo = XmlDemo.XmlWriterDemo();
                xmlreaderdemo = XmlDemo.XmlReaderDemo(xmlwriterdemo);
            }
            sw.Stop();
            Console.WriteLine("{0} iterations took {1}ms", iterations, sw.ElapsedMilliseconds);
            


            Console.WriteLine("XmlDocument demos:");

            Console.WriteLine("XmlDocument write demo:");
            var xmldocwrite = XmlDemo.XmlDocumentWrite();
            Console.WriteLine(xmldocwrite);
            var xmldocread = XmlDemo.XmlDocumentRead(xmldocwrite);
            Console.WriteLine(xmldocread);

            sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                xmldocwrite = XmlDemo.XmlDocumentWrite();
                xmldocread = XmlDemo.XmlDocumentRead(xmldocwrite);
            }
            sw.Stop();
            Console.WriteLine("{0} iterations took {1}ms", iterations, sw.ElapsedMilliseconds);


            Console.WriteLine("XDocument/XElement demos:");

            Console.WriteLine("X Write:");
            var xdocwrite = XmlDemo.XElementWrite();
            Console.WriteLine(xdocwrite);
            var xdocread = XmlDemo.XElementRead(xdocwrite);
            Console.WriteLine(xdocread);

            sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                xdocwrite = XmlDemo.XElementWrite();
                xdocread = XmlDemo.XElementRead(xdocwrite);
            }
            sw.Stop();
            Console.WriteLine("{0} iterations took {1}ms", iterations, sw.ElapsedMilliseconds);



            XmlDemo.TestExtensions();

            
        }
    }
}
