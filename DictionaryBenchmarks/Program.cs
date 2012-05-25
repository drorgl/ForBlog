using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using Ariadne.Collections;
using Babil.Inject;
using System.IO;

namespace DictionaryBenchmarks
{
    class Program
    {
        public class TestResult
        {
            public string DictionaryType;
            public int Threads;
            public bool Read;
            public bool Write;
            public long ElapsedMilliseconds;
            public bool Error;
        }


        static List<TestResult> TestDictionary(IDictionary<string, string> dictionary, List<string> keys, List<string> values, int iterations, int[] threadCounts)
        {
            if (iterations > keys.Count)
                iterations = keys.Count;
            if (iterations > values.Count)
                iterations = values.Count;

            //test with 1 thread
            List<TestResult> testresults = new List<TestResult>();

            foreach (var threadCount in threadCounts)
            {

                try
                {
                    //test write
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    var results = TestingPlatform.Test(threadCount, iterations, delegate(int i)
                    {
                        dictionary[keys[i]] = values[i];
                    });

                    testresults.Add(new TestResult
                    {
                        DictionaryType = dictionary.GetType().Name,
                        ElapsedMilliseconds = results.ElapsedMilliseconds,
                        Error = (results.Exception != null),
                        Read = false,
                        Threads = threadCount,
                        Write = true
                    });

                    Console.WriteLine("Time took to write dictionary {0} with {1} threads: {2} ms", dictionary.GetType().Name, results.ThreadNumber, results.ElapsedMilliseconds);

                    if (results.Exception != null)
                        Console.WriteLine("Dictionary {0} threw an exception during the write test with {1} threads", dictionary.GetType().Name,results.ThreadNumber);

                    //test read
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    results = TestingPlatform.Test(threadCount, iterations, delegate(int i)
                    {
                        var retval = dictionary[keys[i]];
                    });

                    testresults.Add(new TestResult
                    {
                        DictionaryType = dictionary.GetType().Name,
                        ElapsedMilliseconds = results.ElapsedMilliseconds,
                        Error = (results.Exception != null),
                        Read = true,
                        Threads = threadCount,
                        Write = false
                    });

                    Console.WriteLine("Time took to read dictionary {0} with {1} threads: {2} ms", dictionary.GetType().Name, results.ThreadNumber, results.ElapsedMilliseconds);

                    if (results.Exception != null)
                        Console.WriteLine("Dictionary {0} threw an exception during the read test with {1} threads", dictionary.GetType().Name, results.ThreadNumber);

                    //test read
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    results = TestingPlatform.Test(threadCount, iterations, delegate(int i)
                    {
                        dictionary[keys[i]] = values[i];
                        var retval = dictionary[keys[i]];
                    });

                    testresults.Add(new TestResult
                    {
                        DictionaryType = dictionary.GetType().Name,
                        ElapsedMilliseconds = results.ElapsedMilliseconds,
                        Error = (results.Exception != null),
                        Read = true,
                        Threads = threadCount,
                        Write = true
                    });

                    Console.WriteLine("Time took to read/write dictionary {0} with {1} threads: {2} ms", dictionary.GetType().Name, results.ThreadNumber, results.ElapsedMilliseconds);

                    if (results.Exception != null)
                        Console.WriteLine("Dictionary {0} threw an exception during the read test with {1} threads", dictionary.GetType().Name, results.ThreadNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn't test dictionary {0} with {1} threads, error", dictionary.GetType().Name, threadCount);
                }

            }
            return testresults;

        }

        static void Main(string[] args)
        {
            int iterations = 1000000;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //generate a list of random strings
            List<string> keys = new List<string>();
            for (var i = 0; i < iterations; i++)
                keys.Add(Guid.NewGuid().ToString());

            //generate a list of random values
            List<string> values = new List<string>();
            for (var i = 0; i < iterations; i++)
                values.Add(Guid.NewGuid().ToString());

            sw.Stop();
            Console.WriteLine("Time took to generate {0} random keys: {1} ms", iterations * 2, sw.ElapsedMilliseconds);

            List<TestResult> results = new List<TestResult>();
            List<int> threadcounts = new List<int>();
            for (var i = 1; i <=20;i++)
                threadcounts.Add(i);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            results.AddRange( TestDictionary(new Dictionary<string, string>(), keys, values, iterations, threadcounts.ToArray()));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            results.AddRange( TestDictionary(new SafeDictionary<string, string>(), keys, values, iterations, threadcounts.ToArray()));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            results.AddRange(TestDictionary(new SpinlockDictionary<string, string>(), keys, values, iterations, threadcounts.ToArray()));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            results.AddRange( TestDictionary(new ConcurrentDictionary<string, string>(), keys, values, iterations, threadcounts.ToArray()));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            results.AddRange( TestDictionary(new ThreadSafeDictionary<string, string>(), keys, values, iterations, threadcounts.ToArray()));

            TextWriter tw = new StreamWriter("results.csv");
            tw.WriteLine("Dictionary,Threads,Read,Write,Time,Error");
            foreach (var result in results)
            {
                tw.WriteLine("{0},{1},{2},{3},{4},{5}", result.DictionaryType, result.Threads, result.Read, result.Write, result.ElapsedMilliseconds, result.Error);
            }
            tw.Close();
           


            Console.ReadKey();
        }
    }
}
