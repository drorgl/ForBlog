using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data;

namespace ToTraceString
{
    class Program
    {
        static void Main(string[] args)
        {
            SchoolEntities se = new SchoolEntities();
            
            

            //var person = se.People.FirstOrDefault();
            var person = new Person();

            person.decimalfield = 12.345m;
            person.BigIntfield = 1;
            person.FirstName = "O'hara";
            person.bitfield = true;
            person.charfield = "c";
            person.datefield = DateTime.Now;
            person.datetime2field = DateTime.Now;
            person.datetimeoffsetfield = new DateTimeOffset(DateTime.Now);
            person.EnrollmentDate = DateTime.Now;
            person.Floatfield = 10;
            person.guidfield = Guid.NewGuid();
            person.Image = new byte[10];
            person.moneyfield = 10;
            person.ncharfield = "nc";
            person.numericfield = 10;
            person.realfield = 10;
            person.smalldatetimefield = DateTime.Now;
            person.smallintfield = 10;
            person.smallmoneyfield = 10;
            person.xmlfield = "<r></r>";

            var query = se.People.Where(i => 
                (
                    i.decimalfield == person.decimalfield ||
                    i.BigIntfield == null ||
                    i.FirstName == null ||
                    i.FirstName == person.FirstName ||
                    i.BigIntfield == person.BigIntfield ||
                    i.bitfield == person.bitfield ||
                    i.charfield == person.charfield ||
                    i.datefield == person.datefield ||
                    i.datetime2field == person.datetime2field ||
                    //i.datetimeoffsetfield == person.datetimeoffsetfield ||
                    i.EnrollmentDate == person.EnrollmentDate ||
                    i.Floatfield == person.Floatfield ||
                    i.guidfield == person.guidfield ||
                    i.Image == person.Image ||
                    i.moneyfield == person.moneyfield ||
                    i.ncharfield == person.ncharfield ||
                    i.numericfield == person.numericfield ||
                    i.realfield == person.realfield ||
                    i.smalldatetimefield == person.smalldatetimefield ||
                    i.smallintfield == person.smallintfield ||
                    i.smallmoneyfield == person.smallmoneyfield //||
                    //i.xmlfield == person.xmlfield
                ));

            //var qr = query.ToList();

            Console.WriteLine("ToSqlString()");
            Console.WriteLine(query.ToSqlString());

            Console.WriteLine("ToTraceString()");
            Console.WriteLine(query.ToTraceString());

            Console.WriteLine("No Parameters ToSqlString()");
            Console.WriteLine(se.People.ToSqlString());
        }

     


    }
}
