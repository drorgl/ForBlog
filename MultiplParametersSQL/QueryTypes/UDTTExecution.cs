using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// User Defined Table Type Execution Demo
    /// </summary>
    class UDTTExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            string testcommand = @"
            select * from Entities
            join @intt 
	            on [@intt].Id = Entities.EntityId
            ";

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(testcommand, connection))
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;

                    var param = command.CreateParameter();
                    param.SqlDbType = SqlDbType.Structured;
                    param.ParameterName = "@intt";
                    param.TypeName = "udt_inttable";

                    param.Value = this.SelectRecords;

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
            merge Entities as target
            using (select * from @upusers) as source (EntityId, Name, Phone, Email, Address, City, Zip, State, Country, BirthDate)
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
                    param.SqlDbType = SqlDbType.Structured;
                    param.ParameterName = "@upusers";
                    param.TypeName = "udt_entities";

                    param.Value = this.MergeRecords(this.BulkOperationsCount);

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
