using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunWithTuples
{
    class Program
    {
        public static List<string> GenerateStringList(int number)
        {
            List<string> retval = new List<string>();
            for (int i = 0; i < number; i++)
                retval.Add("Item " + i.ToString());

            return retval;
        }


        public static Tuple<int, List<string>> GetPagedItems(int from, int len)
        {
            var list = GenerateStringList(1000);

            var listcount  = list.Count();
            var subset = list.OrderBy(i => i).Skip(from).Take(len).ToList();

            return new Tuple<int, List<string>>(listcount, subset);
        }

        static void Main(string[] args)
        {
            var pageditems = GetPagedItems(4, 5);
            Console.WriteLine("Items: " + pageditems.Item1);
            foreach (var item in pageditems.Item2)
                Console.WriteLine(item);




        }
    }
}
