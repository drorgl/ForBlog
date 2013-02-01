using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Microsoft.SqlServer.Server;
using MultipleParametersSQL.QueryTypes;
using System.IO;

namespace MultipleParametersSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");

            var log = File.CreateText("log.txt");
            log.WriteLine("Operation,Mode,BlockSize,Records,ExecutionTime,RecordsPerSecond");

            int iterations =10;
            int bulkOperationCount = 1000;

            List<AbstractQueryExecution> executions = new List<AbstractQueryExecution>();
            executions.Add(new DynamicExecution { BulkOperationsCount = bulkOperationCount});
            executions.Add(new NoOptimizationsDynamicExecution { BulkOperationsCount = bulkOperationCount });
            executions.Add(new UDTTExecution { BulkOperationsCount = bulkOperationCount });
            executions.Add(new PrerenderExecution { BulkOperationsCount = bulkOperationCount });
            executions.Add(new XMLExecution { BulkOperationsCount = bulkOperationCount });
            executions.Add(new BulkExecution { BulkOperationsCount = bulkOperationCount });
            executions.Add(new SplitExecution {BulkOperationsCount = bulkOperationCount });

            //initialize selects/merges, for initial loading so it won't affect the benchmarks.
            foreach (var exc in executions)
            {
                exc.Select(false);
                exc.Merge(false);
            }

            int[] iterationvalues = new int[] {1,10,50,100,150,200,250,300,350,400,450,500,1000,2000,3000,4000,5000,10000,20000 };

            for (var itc = 0; itc < iterationvalues.Length; itc ++)
            {
                var it = iterationvalues[itc];
                Console.WriteLine("Initializing...");
                executions = new List<AbstractQueryExecution>();
                executions.Add(new DynamicExecution { BulkOperationsCount = it });
                executions.Add(new NoOptimizationsDynamicExecution { BulkOperationsCount = it });
                executions.Add(new UDTTExecution { BulkOperationsCount = it });
                executions.Add(new PrerenderExecution { BulkOperationsCount = it });
                executions.Add(new XMLExecution { BulkOperationsCount = it });
                executions.Add(new BulkExecution { BulkOperationsCount = it });
                executions.Add(new SplitExecution { BulkOperationsCount = it });


                //initialize selects/merges, for initial loading.
                foreach (var exc in executions)
                {
                    exc.Select(false);
                    exc.Merge(false);
                }

                Console.WriteLine("Select:");

                //select
                foreach (var exc in executions)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    for (var i = 0; i < iterations; i++)
                    {
                        exc.Select(false);
                    }
                    bool useElapsed = true;
                    var elapsed = sw.ElapsedMilliseconds;
                    if (elapsed == 0)
                    {
                        elapsed = 1;
                        useElapsed = false;
                    }
                    Console.WriteLine("Bulk: {0}, {1} items - {2}: {3}ms {4}rps", it, (it * iterations), exc.GetType().Name, elapsed, (useElapsed) ? (1000f / elapsed * (it * iterations)) : 0);
                    log.WriteLine("Select,{0},{1},{2},{3},{4}", exc.GetType().Name,it ,(it * iterations), elapsed, (useElapsed) ? (1000f / elapsed * (it * iterations)) : 0);
                }

                //merge/insert/update
                Console.WriteLine("Merge:");

                foreach (var exc in executions)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    for (var i = 0; i < iterations; i++)
                    {
                        exc.Merge(false);
                    }
                    bool useElapsed = true;
                    var elapsed = sw.ElapsedMilliseconds;
                    if (elapsed == 0)
                    {
                        elapsed = 1;
                        useElapsed = false;
                    }
                    Console.WriteLine("Bulk: {0}, {1} items - {2}: {3}ms {4}rps", it, (it * iterations), exc.GetType().Name, elapsed, (useElapsed) ? (1000f / elapsed * (it * iterations)) : 0);
                    log.WriteLine("Insert,{0},{1},{2},{3},{4}", exc.GetType().Name, it, (it * iterations), elapsed, (useElapsed) ? (1000f / elapsed * (it * iterations)) : 0);
                }
            }
            //close log
            log.Close();
        }

    }
}

