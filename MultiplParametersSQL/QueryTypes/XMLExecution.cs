using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// XML Execution Demo
    /// </summary>
    class XMLExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            string testcommand = @"
            select * from Entities
            where EntityId in (
	            SELECT T.val.value('(@Id)','int') as Id
	            FROM @xmlval.nodes('/root/item') T(val)
            )
            ";

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(testcommand, connection))
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;

                    var param = command.CreateParameter();
                    param.SqlDbType = SqlDbType.Xml;
                    param.ParameterName = "@xmlval";

                    XElement root = new XElement("root");
                    foreach (var r in this.SelectRecords)
                    {
                        root.Add(new XElement("item",
                            new XAttribute("Id", r[0])
                            ));
                    }

                    param.Value = root.ToString();

                    command.Parameters.Add(param);

                    using (var reader = command.ExecuteReader())
                    {
                        DisplayReader(display, reader);
                    }
                }
            }
        }

        public override void Merge(bool display)
        {
            string testcommand = @"
            with upusers (EntityId, Name, Phone, Email, Address, City, Zip, State, Country, BirthDate)
            as
            (	
	            SELECT 
		            T.val.value('(@EntityId)','int') as EntityId,
		            T.val.value('(@Name)','nvarchar(max)') as Name,
		            T.val.value('(@Phone)','nvarchar(max)') as Phone,
		            T.val.value('(@Email)','nvarchar(max)') as Email,
		            T.val.value('(@Address)','nvarchar(max)') as Address,
		            T.val.value('(@City)','nvarchar(max)') as City,
		            T.val.value('(@Zip)','nvarchar(max)') as Zip,
		            T.val.value('(@State)','nvarchar(max)') as State,
		            T.val.value('(@Country)','nvarchar(max)') as Country,
		            T.val.value('(@BirthDate)','nvarchar(max)') as BirthDate
	
	            FROM @xmlval.nodes('/root/item') T(val)
            )

            merge Entities as target
            using (select * from upusers) as source (EntityId, Name, Phone, Email, Address, City, Zip, State, Country, BirthDate)
            on (target.EntityId = source.EntityId)
            when matched then
                update 
                    set Name = source.Name,
                     Phone = source.Phone,
                     Email = source.Email,
                     Address = source.Address,
                     City = source.City,
                     Zip = source.Zip,
                     State = source.State,
                     Country = source.Country,
                     BirthDate = source.Birthdate
            when Not Matched Then
                insert (Name, Phone, Email, Address, City, Zip, State, Country, Birthdate)
                values (source.Name, source.Phone, source.Email, source.Address, source.City, source.Zip, source.State, source.Country, source.Birthdate)
            ;
            ";

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(testcommand, connection))
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;

                    var param = command.CreateParameter();
                    param.SqlDbType = SqlDbType.Xml;
                    param.ParameterName = "@xmlval";

                    XElement root = new XElement("root");
                    foreach (DataRow r in this.MergeRecords(this.BulkOperationsCount).Rows)
                    {
                        root.Add(new XElement("item",
                            new XAttribute("EntityId", r["EntityId"]),
                            new XAttribute("Name", r["Name"]),
                            new XAttribute("Phone", r["Phone"]),
                            new XAttribute("Email", r["Email"]),
                            new XAttribute("Address", r["Address"]),
                            new XAttribute("City", r["City"]),
                            new XAttribute("Zip", r["Zip"]),
                            new XAttribute("State", r["State"]),
                            new XAttribute("Country", r["Country"]),
                            new XAttribute("BirthDate", r["BirthDate"])
                            ));
                    }

                    param.Value = root.ToString();

                    command.Parameters.Add(param);

                    using (var reader = command.ExecuteReader())
                    {
                        DisplayReader(display, reader);
                    }
                }
            }
        }

    }

}
