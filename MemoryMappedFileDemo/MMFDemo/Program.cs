using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileStore;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace MMFDemo
{
    class Program
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        private static void TestGuids()
        {
            Console.WriteLine("Basic tests start");
            string filename = "test.db";

            using (FileStoreManager fsm = new FileStoreManager(filename))
            {

                Guid id1 = new Guid("f8c88a04-97b3-445b-a62c-eeeac7b40959");
                Guid id2 = new Guid("5dfee98f-476a-4e1f-bea4-2b455577b699");
                Guid id3 = new Guid("b15bb1dd-d565-4f11-aa47-ff3fe68d6c99");

                fsm.PushEntry(id1, "id1 string");
                fsm.PushEntry(id2, "id2 string");
                fsm.PushEntry(id3, "id3 string");

                Console.WriteLine("Entries found: {0}", fsm.GetAllEntries().Count());

                Console.WriteLine("id1: {0}", fsm.GetEntry<string>(id1));
                Console.WriteLine("id2: {0}", fsm.GetEntry<string>(id2));
                Console.WriteLine("id3: {0}", fsm.GetEntry<string>(id3));

                fsm.Delete();
            }
            Console.WriteLine("Basic tests end\r\n");
        }

        private static void TestManyRecords()
        {
            Console.WriteLine("Many records test start");
            string filename = "testmany.db";
            FileStoreManager fsm = new FileStoreManager(filename);

            Console.WriteLine("Entries found: {0}", fsm.GetAllEntries().Count());

            //writing first entry so initialization will be done
            fsm.PushEntry(Guid.NewGuid(), Guid.NewGuid());

            Random r = new Random();

            List<Tuple<Guid, byte[]>> biglist = new List<Tuple<Guid, byte[]>>();

            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Preparing test data...");
            for (var i = 0; i < 70000; i++)
            {
                byte[] data = new byte[100];
                r.NextBytes(data);

                biglist.Add(new Tuple<Guid, byte[]>(Guid.NewGuid(), data));
            }

            Console.WriteLine("Done in {0}ms", sw.ElapsedMilliseconds);

            sw.Restart();

            Console.Write("Writing entries to disk...");
            
            for (var i = 0; i < biglist.Count(); i++)
            {
                fsm.PushEntry(biglist[i].Item1, biglist[i].Item2);
            }

            Console.WriteLine("Saved {0} objects in {1}ms", biglist.Count(), sw.ElapsedMilliseconds);
            sw.Restart();

            Console.Write("Reading back data... ");

            var entries = fsm.GetAllEntries();

            Console.WriteLine("Entries read after insert: {0} in {1} ms", entries.Count(),sw.ElapsedMilliseconds);
            sw.Restart();

            Console.Write("Verifying entries... ");

            foreach (var item in biglist)
            {
                var data = fsm.GetEntry<byte[]>(item.Item1);
                if (memcmp(data, item.Item2, item.Item2.Length) != 0)
                    Console.WriteLine("ERROR!");
            }

            Console.WriteLine("Verified {0} entries in {1}ms", biglist.Count(), sw.ElapsedMilliseconds);

            Console.WriteLine(fsm.GetStatistics());

            fsm.Delete();

            Console.WriteLine("Many records test end\r\n");
        }


        static void Main(string[] args)
        {
            TestMinimum();
            TestGuids();
            TestManyRecords();
            

        }

        private static void TestMinimum()
        {
            Console.Write("Sanity test...");
            var testfilename = "xxtest.db";

            using (FileStore.Mapper.MemoryMapper mm = new FileStore.Mapper.MemoryMapper(testfilename, 10, 10))
            {
                using (var stream = mm.CreateStream(0, 1))
                {
                    stream.WriteByte(1);
                }
                using (var view = mm.CreateView(1, 1))
                {
                    view.Write(0, 1);
                }
                mm.Flush();
            }
            if (File.Exists(testfilename))
                File.Delete(testfilename);

            Console.WriteLine("done.\r\n");
        }
    }
}
