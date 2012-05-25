using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace DictionaryBenchmarks
{
    public class TestingPlatform
    {
        public class TestResults
        {
            public int ThreadNumber;
            public long ElapsedMilliseconds;
            public Exception Exception;
        }

        public static TestResults Test(int threadCount, int itemsCount, Action<int> testFunction)
        {
            List<TestResults> results = new List<TestResults>();
            for (int i = 0; i < threadCount; i++)
                results.Add(new TestResults());

            List<Thread> threads = new List<Thread>();
            int batchsize = itemsCount / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(new ParameterizedThreadStart(delegate(object obj)
                    {
                        int threadnumber = (int)obj;

                        int batchstart = (batchsize * threadnumber);
                        Stopwatch sw = new Stopwatch();
                        try
                        {
                            sw.Start();

                            for (int itemnumber = 0; itemnumber < batchsize; itemnumber++)
                                testFunction(itemnumber + batchstart);

                            sw.Stop();
                        }
                        catch (Exception ex)
                        {
                            results[threadnumber].Exception = ex;
                        }

                        results[threadnumber].ElapsedMilliseconds = sw.ElapsedMilliseconds;
                        results[threadnumber].ThreadNumber = threadnumber;

                    }));
                threads.Add(thread);
            }
            for (int i = 0; i < threadCount; i++)
                threads[i].Start(i);

            for (int i = 0; i < threadCount; i++)
                threads[i].Join();

            TestResults tr = new TestResults();
            for (int i = 0; i < threadCount; i++)
            {
                tr.ElapsedMilliseconds += results[i].ElapsedMilliseconds;

                if (results[i].Exception != null)
                    tr.Exception = results[i].Exception;
            }
            tr.ThreadNumber = threadCount;

            return tr;
        }
        
    }
}
