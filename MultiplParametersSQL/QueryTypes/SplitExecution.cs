using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// Uses split for multiple values
    /// </summary>
    class SplitExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            string testcommand = @"
            select * from Entities
            where Entities.EntityId in (select data from dbo.split(@p1,','))
            ";

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(testcommand, connection))
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;


                    var param = command.CreateParameter();
                    param.ParameterName = "@p1";

                    param.Value = string.Join(",", this.SelectRecords.Select(i => i.GetValue(0)).ToArray());

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
            
        }
    }
}
