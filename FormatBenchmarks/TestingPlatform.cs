using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FormatBenchmarks
{
    public class TestingPlatform
    {
        public class TestResults
        {
            public int ParameterCount;
            public long ElapsedMilliseconds;
            public Exception Exception;
        }

       


        public static TestResults TestFormat(int formatCount, int iterations, Action<string,object[]> action)
        {
            object[] valuesdict = new object[formatCount];
            string template = "hello ";
            for (int i = 0; i < formatCount; i++)
            {
                template += "{" + i + "} text112";
                valuesdict[i] = i;
            }

            Stopwatch sw = new Stopwatch();
            //prime the string format, so it will be more realistic
            action(template, valuesdict);

            for (var i = 0; i < iterations; i++)
            {
                sw.Stop();
                for (int fc = 0; fc < formatCount; fc++)
                {
                    valuesdict[fc] = i;
                }
                sw.Start();
                action(template, valuesdict);
                sw.Stop();
            }



            TestResults tr = new TestResults
            {
                 ParameterCount = formatCount,
                 ElapsedMilliseconds = sw.ElapsedMilliseconds
            };

            return tr;
        }
    }
}
