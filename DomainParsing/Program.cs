using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DomainParsingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] _tests = new string[] {
                "www.google.com",
                "www.walla.co.il",
                "www.google.co.uk",
                "www.cisco.com",
                "localhost",
                "www.localhost",
                "co.uk"
            };

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < (100000 / _tests.Length); i++)
            {

                foreach (var t in _tests)
                {
                    var dp = DomainParser.GetDomain(t);
                    Console.WriteLine("Full Domain: {0}, Domain: {1}", t, dp);
                }
            }
            Console.WriteLine("Took: {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
