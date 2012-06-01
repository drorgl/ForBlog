using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSVParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CSVParser parser = new CSVParser("test.csv");


            //display unordered records from file.
            Console.WriteLine("Unordered");
            Console.WriteLine("Row Number\tFullName\t Number \t Date");
            foreach (var row in parser.Rows)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", row.RowNumber, row.Cell[0].Value, row.Cell[1].Value, row.Cell[2].Value);
            }

            Console.WriteLine("press enter to continue");
            Console.ReadLine();


            //order by column 1 which is a string
            var orderedbycolumn0 = parser.Rows.OrderBy(i => (string)i.Cell[0]).ToList();
            Console.WriteLine("Order by Name");
            Console.WriteLine("Row Number\tFullName\t Number \t Date");
            foreach (var row in orderedbycolumn0)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", row.RowNumber, row.Cell[0].Value, row.Cell[1].Value, row.Cell[2].Value);
            }

            Console.WriteLine("press enter to continue");
            Console.ReadLine();

            //order by column 2 which might be an int
            var orderedbycolumn2 = parser.Rows.OrderBy(i => (int?)i.Cell[1]).ToList();
            Console.WriteLine("Order by Number");
            Console.WriteLine("Row Number\tFullName\t Number \t Date");
            foreach (var row in orderedbycolumn2)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", row.RowNumber, row.Cell[0].Value, row.Cell[1].Value, row.Cell[2].Value);
            }

            Console.WriteLine("press enter to continue");
            Console.ReadLine();

            //order by column 3 which might be an datetime
            var orderedbycolumn3 = parser.Rows.OrderBy(i => (DateTime?)i.Cell[2]).ToList();
            Console.WriteLine("Order by Date");
            Console.WriteLine("Row Number\tFullName\t Number \t Date");
            foreach (var row in orderedbycolumn3)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", row.RowNumber, row.Cell[0].Value, row.Cell[1].Value, row.Cell[2].Value);
            }

            Console.WriteLine("press enter to continue");
            Console.ReadLine();
        }
    }
}
