using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace FormatBenchmarks
{
    class Program
    {
        public class TestResult
        {
            public string FormatMethod;
            public int ParameterCount;
            public long ElapsedMilliseconds;
        }


        static void Main(string[] args)
        {
            int iterations = 1000000;

            int parameterCount = 11;

            List<TestResult> testresults = new List<TestResult>();        
        

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = string.Format(template, parameters);
                });

                Console.WriteLine("string.Format tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "string.Format",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.Format(template, parameters);
                });

                Console.WriteLine("CompiledString.Format tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.Format",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatStringConcat(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatStringConcat tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatStringConcat",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatConcat(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatConcat tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatConcat",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }


            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatList(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatList tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatList",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatInsertRemove(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatInsertRemove tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatInsertRemove",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
          
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatReplace(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatReplace tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatReplace",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }


            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatFly(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatFly tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatFly",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            for (int i = 1; i < parameterCount; i++)
            {
                var results = TestingPlatform.TestFormat(i, iterations, delegate(string template, object[] parameters)
                {
                    var output = CompiledString.FormatByteCopy(template, parameters);
                });

                Console.WriteLine("CompiledString.FormatByteCopy tested " + results.ParameterCount + " took " + results.ElapsedMilliseconds);
                testresults.Add(new TestResult
                {
                    FormatMethod = "CompiledString.FormatByteCopy",
                    ParameterCount = results.ParameterCount,
                    ElapsedMilliseconds = results.ElapsedMilliseconds
                });
            }

            TextWriter tw = new StreamWriter("results.csv");
            tw.WriteLine("FormatMethod,ParameterCount,Time");
            foreach (var result in testresults)
            {
                tw.WriteLine("{0},{1},{2}", result.FormatMethod, result.ParameterCount, result.ElapsedMilliseconds);
            }
            tw.Close();
 
            Console.ReadKey();
        }
        
    }
}
