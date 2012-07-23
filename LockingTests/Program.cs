using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using LockingTests.Locks;
using System.IO;

namespace LockingTests
{
    class Program
    {
        static void TestPayload(ILocker locker, int threadcount)
        {
            Thread[] threads = new Thread[threadcount];

            Payload payload = new Payload(locker);

            for (var i = 0; i < threadcount; i++)
            {
                var thread = new Thread(new ThreadStart(payload.ThreadX));
                threads[i] = thread;
                thread.Start();
            }

            for (var i = 0; i < threadcount; i++)
                threads[i].Join();
        }

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int maxthreads = 40;

            var sr = new StreamWriter("Results.txt");
            sr.WriteLine("Lock,Threads,Time,CPU");

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            var perfcounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            

            for (var i = 1; i <= maxthreads; i++)
            {
                sw.Restart();
                TestPayload(new Spinlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Spinlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Spinlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds,i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new Yieldlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Yieldlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Yieldlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new SleepZerolock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(SleepZerolock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("SleepZerolock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new SleepOnelock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(SleepOnelock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("SleepOnelock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());


                sw.Restart();
                TestPayload(new FrameworkSpinlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(FrameworkSpinlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("FrameworkSpinlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new Spinwaitlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Spinwaitlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Spinwaitlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new Complexlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Complexlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Complexlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());


                sw.Restart();
                TestPayload(new Monitorlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Monitorlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Monitorlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new Mutexlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Mutexlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Mutexlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new Semaphorelock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(Semaphorelock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("Semaphorelock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());

                sw.Restart();
                TestPayload(new SemaphoreSlimlock(), i);
                sr.WriteLine("{0},{1},{2},{3}", typeof(SemaphoreSlimlock).Name, i, sw.ElapsedMilliseconds, perfcounter.NextValue());
                Console.WriteLine("SemaphoreSlimlock Took {0}ms with {1} threads with {2}% CPU", sw.ElapsedMilliseconds, i, perfcounter.NextValue());
            }

            sr.Close();


            
            Console.ReadKey();
        }
    }
}
