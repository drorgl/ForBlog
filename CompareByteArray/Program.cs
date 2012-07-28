using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace CompareByteArray
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            var sr = new StreamWriter("Results.txt");
            sr.WriteLine("Compare,Size,Time");


            int iterations = 10000;

            long result = 0;
            for (int i = 1; i < 10; i++)
            {
                int arraysize = (int)Math.Pow(2, 5 + i);
                //int arraysize = 1024 * i;

                result = Payload(iterations, arraysize, new SimpleCompare());
                sr.WriteLine("{0},{1},{2}", typeof(SimpleCompare).Name, arraysize, result);
                Console.WriteLine("SimpleCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new SequenceEqualCompare());
                sr.WriteLine("{0},{1},{2}", typeof(SequenceEqualCompare).Name, arraysize, result);
                Console.WriteLine("SequenceEqualCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new ArrayEqualCompare());
                sr.WriteLine("{0},{1},{2}", typeof(ArrayEqualCompare).Name, arraysize, result);
                Console.WriteLine("ArrayEqualCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new memcmpCompare());
                sr.WriteLine("{0},{1},{2}", typeof(memcmpCompare).Name, arraysize, result);
                Console.WriteLine("memcmpCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new unsafeCompare());
                sr.WriteLine("{0},{1},{2}", typeof(unsafeCompare).Name, arraysize, result);
                Console.WriteLine("unsafeCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new unsafeSvensonCompare());
                sr.WriteLine("{0},{1},{2}", typeof(unsafeSvensonCompare).Name, arraysize, result);
                Console.WriteLine("unsafeSvensonCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new unsafeByteCompare());
                sr.WriteLine("{0},{1},{2}", typeof(unsafeByteCompare).Name, arraysize, result);
                Console.WriteLine("unsafeByteCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new unsafeIntCompare());
                sr.WriteLine("{0},{1},{2}", typeof(unsafeIntCompare).Name, arraysize, result);
                Console.WriteLine("unsafeIntCompare size {0} took {1}ms", arraysize, result);

                result = Payload(iterations, arraysize, new unsafeLongCompare());
                sr.WriteLine("{0},{1},{2}", typeof(unsafeLongCompare).Name, arraysize, result);
                Console.WriteLine("unsafeLongCompare size {0} took {1}ms", arraysize, result);

                
            }

            sr.Close();
        }

        static long Payload(int iterations, int length, IComparing comparer)
        {
            byte[] array1 = new byte[length];
            byte[] array2 = new byte[length];

            //check that compare is actually working.
            if (comparer.Compare(array1, array2) == false)
                throw new ApplicationException(comparer.GetType().Name + " returned false where arrays are equal");

            array2[length - 1] = 1;

            if (comparer.Compare(array1, array2) == true)
                throw new ApplicationException(comparer.GetType().Name + " returned true where arrays are not equal");

            array2[length - 1] = 0;


            Stopwatch sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations;i++)
                comparer.Compare(array1, array2);

            return sw.ElapsedMilliseconds;
        }

        

    }
}
