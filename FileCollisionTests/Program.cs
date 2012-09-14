using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace FileCollisionTests
{
    class Program
    {
        /// <summary>
        /// payload execution result (for statistics)
        /// </summary>
        public class payloadresult
        {
            public string Method { get; set; }
            public string Action { get; set; }
            public int Threads { get; set; }
            public long Time { get; set; }
        }

        static void Main(string[] args)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;

            var statistics = new List<payloadresult>();

            //number of tests to perform for each payload
            var iterations = 5;

            //number of operations per iteration
            var operations = 100;

            //files in test block (the lower it is, the more collisions there will be).
            var ftb = 100;

            //file sizes
            var filesize = 1024 * 1024 * 20;//20MB //51200;

            //number of threads
            var tclist = new int[] {1,5,10};
            foreach (var tc in tclist)
            {
                for (var it = 0; it < iterations; it++)
                {
                    statistics.AddRange(PayloadLocking(tc, operations, filesize, ftb, int.MaxValue, TimeSpan.FromMilliseconds(1)));
                    statistics.AddRange(PayloadQueued(tc, operations, filesize, ftb));
                    statistics.AddRange(PayloadQueuedFS(tc, operations, filesize, ftb));
                    statistics.AddRange(PayloadCA(tc, operations, filesize, ftb));
                }
            }


            Console.WriteLine("Statistics:");
            Console.Write("Action\tThreads");
            var methods = statistics.Select(i => i.Method).Distinct();
            foreach (var method in methods)
                Console.Write("\t" + method);
            Console.WriteLine();

            var actions = statistics.Select(i => i.Action).Distinct();
            var threads = statistics.Select(i => i.Threads).Distinct();
            foreach (var action in actions)
                foreach (var thread in threads)
                {
                    //Average time it took to perform
                    Console.Write(action + "\t" + thread);
                    foreach (var method in methods)
                    {
                        var stats = statistics.Where(i => i.Method == method && i.Action == action && i.Threads == thread).Select(i => i.Time).Average();
                        Console.Write("\t" + stats);
                    }
                    Console.WriteLine();

                    //Files per second
                    Console.Write(" \t  ");
                    foreach (var method in methods)
                    {
                        var stats = statistics.Where(i => i.Method == method && i.Action == action && i.Threads == thread).Select(i => i.Time).Average();
                        var fps = Math.Truncate((thread * operations) / ((stats / 1000f)));
                        Console.Write("\t" + fps);
                    }
                    Console.WriteLine();
                }

        }

        /// <summary>
        /// Generate random filenames
        /// </summary>
        private static List<string> GenerateFilenames(int filescount)
        {
            List<string> filenames = new List<string>();
            for (var i = 0; i < filescount ; i++)
                filenames.Add(Guid.NewGuid().ToString() + ".db");
            return filenames;
        }

        /// <summary>
        /// Create payload threads, each one has a loop with number of iterations
        /// </summary>
        public static List<Thread> CreatePayloadThreads(int threadcount, int iterations, Action exec)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < threadcount; i++)
            {
                threads.Add(new Thread(() =>
                    {
                        for (var ti = 0; ti < iterations; ti++)
                        {
                            try
                            {
                                exec.Invoke();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                    }));

            }
            return threads;
        }

        /// <summary>
        /// Start all threads in list
        /// </summary>
        private static void StartThreads(List<Thread> threads)
        {
            foreach (var t in threads)
                t.Start();
        }

        /// <summary>
        /// Wait for all threads in list to finish
        /// </summary>
        private static void WaitForThreadsToFinish(List<Thread> threads)
        {
            foreach (var t in threads)
                t.Join();
        }

        /// <summary>
        /// Time the payload execution
        /// </summary>
        private static Stopwatch TimePayload(int threadcount, int iterations, Action exec)
        {
            var threads = CreatePayloadThreads(threadcount, iterations, exec);
            Stopwatch sw = Stopwatch.StartNew();
            StartThreads(threads);
            WaitForThreadsToFinish(threads);
            sw.Stop();
            return sw;
        }

        /// <summary>
        /// Cleanup test files.
        /// </summary>
        /// <param name="filenames"></param>
        private static void DeleteFiles(List<string> filenames)
        {
            foreach (var file in filenames)
            {
                try
                {
                    File.Delete(file);
                }catch{}
            }
        }

        public static List<payloadresult> PayloadLocking(int threadcount, int iterations, int payloadsize, int filescount, int retry, TimeSpan wait)
        {
            List<payloadresult> retval = new List<payloadresult>();

            byte[] testobject = new byte[payloadsize];
            MemoryStream teststream = new MemoryStream(testobject);

            //generate short list of filenames
            List<string> filenames = GenerateFilenames(filescount);

            //write all filenames
            for (int i = 0; i < filenames.Count; i++)
            {
                LockingFileSystem.SaveStream(filenames[i], teststream);
            }

            //check read speed
            var sw = TimePayload(threadcount, iterations, () =>
                {
                    var stream = LockingFileSystem.LoadStream(filenames[GetRandom(0, filenames.Count - 1)], retry, wait);
                });

            retval.Add(new payloadresult
            {
                 Method = "Locking",
                 Action = "R",
                 Threads = threadcount,
                 Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Locking Load took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds,( (threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check write speed
            sw = TimePayload(threadcount, iterations, () =>
            {
                LockingFileSystem.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream, retry, wait);
            });

            retval.Add(new payloadresult
            {
                Method = "Locking",
                Action = "W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Locking Save took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));


            //check readwrite speed
            sw = TimePayload(threadcount, iterations/2, () =>
            {
                var stream = LockingFileSystem.LoadStream(filenames[GetRandom(0, filenames.Count - 1)], retry, wait);
                LockingFileSystem.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream, retry, wait);
            });

            retval.Add(new payloadresult
            {
                Method = "Locking",
                Action = "R/W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Locking Save/Load took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //cleanup
            DeleteFiles(filenames);

            return retval;
        }


        public static List<payloadresult> PayloadQueued(int threadcount, int iterations, int payloadsize, int filescount)
        {
            List<payloadresult> retval = new List<payloadresult>();

            byte[] testobject = new byte[payloadsize];
            MemoryStream teststream = new MemoryStream(testobject);

            QueuedStreamFileSystem qsf = new QueuedStreamFileSystem(5);

            //generate short list of filenames
            List<string> filenames = GenerateFilenames(filescount);

            //write all filenames
            for (int i = 0; i < filenames.Count; i++)
            {
                qsf.SaveStream(filenames[i], teststream);
                //QueuedFileSystem.SaveObject(filenames[i], testobject);
            }

            //check read speed
            var sw = TimePayload(threadcount, iterations , () =>
            {
                var stream = qsf.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS1",
                Action = "R",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Queued Load took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check write speed
            sw = TimePayload(threadcount, iterations , () =>
            {
                qsf.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS1",
                Action = "W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Queued Save took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));


            //check readwrite speed
            sw = TimePayload(threadcount, iterations / 2, () =>
            {
                var stream = qsf.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
                qsf.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS1",
                Action = "R/W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Queued Save/Load took {1}ms fps:{2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //cleanup
            DeleteFiles(filenames);

            qsf.Dispose();

            return retval;
        }


        public static List<payloadresult> PayloadQueuedFS(int threadcount, int iterations, int payloadsize, int filescount)
        {
            List<payloadresult> retval = new List<payloadresult>();

            var qfs = new QueuedFileSystem(5);

            byte[] testobject = new byte[payloadsize];
            MemoryStream teststream = new MemoryStream(testobject);

            //generate short list of filenames
            List<string> filenames = GenerateFilenames(filescount);

            //write all filenames
            for (int i = 0; i < filenames.Count; i++)
            {
                qfs.SaveStream(filenames[i], teststream);
                //QueuedFileSystem.SaveObject(filenames[i], testobject);
            }

            //check read speed
            var sw = TimePayload(threadcount, iterations , () =>
            {
                var stream = qfs.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS2",
                Action = "R",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} QueuedFS Load took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check write speed
            sw = TimePayload(threadcount, iterations, () =>
            {
                qfs.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS2",
                Action = "W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} QueuedFS Save took {1}ms fps: {2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check readwrite speed
            sw = TimePayload(threadcount, iterations / 2, () =>
            {
                var stream = qfs.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
                qfs.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "QFS2",
                Action = "R/W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} QueuedFS Save/Load took {1}ms fps:{2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //cleanup
            DeleteFiles(filenames);

            qfs.Dispose();

            return retval;
        }

        public static List<payloadresult> PayloadCA(int threadcount, int iterations, int payloadsize, int filescount)
        {
            List<payloadresult> retval = new List<payloadresult>();

            byte[] testobject = new byte[payloadsize];
            MemoryStream teststream = new MemoryStream(testobject);

            //generate short list of filenames
            List<string> filenames = GenerateFilenames(filescount);

            List<Thread> threads = new List<Thread>();

            //write all filenames
            for (int i = 0; i < filenames.Count; i++)
            {
                CollisionAvoidanceFileAccess.SaveStream(filenames[i], teststream);
            }

            //check read speed
            var sw = TimePayload(threadcount, iterations , () =>
            {
                var stream = CollisionAvoidanceFileAccess.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
            });

            retval.Add(new payloadresult
            {
                Method = "CA",
                Action = "R",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Collision Avoidance Load took {1}ms fps:{2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check write speed
            sw = TimePayload(threadcount, iterations, () =>
            {
                CollisionAvoidanceFileAccess.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "CA",
                Action = "W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Collision Avoidance Save took {1}ms fps:{2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //check readwrite speed
            sw = TimePayload(threadcount, iterations /2, () =>
            {
                var stream = CollisionAvoidanceFileAccess.LoadStream(filenames[GetRandom(0, filenames.Count - 1)]);
                CollisionAvoidanceFileAccess.SaveStream(filenames[GetRandom(0, filenames.Count - 1)], teststream);
            });

            retval.Add(new payloadresult
            {
                Method = "CA",
                Action = "R/W",
                Threads = threadcount,
                Time = sw.ElapsedMilliseconds
            });

            Console.WriteLine("Threads: {0} Collision Avoidance Save/Load took {1}ms fps:{2}", threadcount, sw.ElapsedMilliseconds, ((threadcount * iterations) / ((sw.ElapsedMilliseconds) / 1000f)));

            //cleanup
            DeleteFiles(filenames);

            return retval;
        }

        /// <summary>
        /// Get Random integer
        /// </summary>
        private static int GetRandom(int min, int max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException("max must be larger than min");

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] intbytes = new byte[4];
            rng.GetBytes(intbytes);
            int rndval = BitConverter.ToInt32(intbytes, 0);
            int range = max - min;
            return Math.Abs((rndval % range)) + min;
        }

    }
}
