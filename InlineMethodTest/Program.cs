using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace InlineMethodTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Small:");
            SmallMethodsTest.Execute();
            Console.WriteLine("Large:");
            LargeMethodsTest.Execute();
        }
    }
}
