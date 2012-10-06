using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InlineMethodTest
{
    class SmallMethodsTest
    {
        delegate int delegatetest(int a1, int s2, int a3);

        public static void Execute()
        {
            int iterations = 5000000;
            Stopwatch sw = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                var res = MathTest(i, 1, 3);
            }
            Console.WriteLine("No Inline: {0}ms", sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var res = MathTestInline(i, 1, 3);
            }
            Console.WriteLine("Inline: {0}ms", sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var res = MathTestDefault(i, 1, 3);
            }
            Console.WriteLine("Default: {0}ms", sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var res = (i+1) * 3;
            }
            Console.WriteLine("Code: {0}ms", sw.ElapsedMilliseconds);


            sw = Stopwatch.StartNew();

            delegatetest delegatemthod = delegate(int a1, int a2, int a3)
            {
                return (a1 + a2) * a3;
            };

            for (var i = 0; i < iterations; i++)
            {
                var res = delegatemthod(i, 1, 3);
            }
            Console.WriteLine("Delegate: {0}ms", sw.ElapsedMilliseconds);

            Func<int, int, int, int> lambdatest = (a1, a2, a3) =>
            {
                return (a1 + a2) * a3;
            };

            sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var res = lambdatest(i, 1, 3);
            }
            Console.WriteLine("Lambda: {0}ms", sw.ElapsedMilliseconds);
        }


        private static int MathTestDefault(int a1, int a2, int a3)
        {
            return (a1 + a2) * a3;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int MathTest(int a1, int a2, int a3)
        {
            return (a1 + a2) * a3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MathTestInline(int a1, int a2, int a3)
        {
            return (a1 + a2) * a3;
        }
    }
}
