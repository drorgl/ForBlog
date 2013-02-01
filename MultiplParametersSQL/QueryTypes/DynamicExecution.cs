using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// Dynamic SQL Execution Demo
    /// </summary>
    class DynamicExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            string testcommand = @"
            select * from Entities
            where Entities.EntityId in ({0})
            ";

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(testcommand, string.Join(",", this.SelectRecords.Select(i => i.GetValue(0)).ToArray())),connection)) 
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        DisplayReader(display, reader);
                    }
                }
            }
        }

        public override void Merge(bool display)
        {
            var dt = this.MergeRecords(this.BulkOperationsCount);

            List<string> inserts = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() == "0")
                {
                    StringBuilder sbinsert = new StringBuilder();

                    sbinsert.Append("(");
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        sbinsert.Append(RenderSQL.GetDbValue(row[i]));
                        if (i < (dt.Columns.Count - 1))
                            sbinsert.Append(",");
                    }
                    sbinsert.Append(")");

                    inserts.Add(sbinsert.ToString());
                    
                }
                else
                {
                    //update
                    sb.Append("update Entities  set ");
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        sb.Append(dt.Columns[i].ColumnName);
                        sb.Append(" = ");
                        sb.Append(RenderSQL.GetDbValue(row[i]));
                        if (i < (dt.Columns.Count - 1))
                            sb.Append(",");
                    }
                    sb.AppendFormat(" where EntityId = {0};",row[0]);
                }
            }

            if (inserts.Count() > 0)
            {
                //insert
                //iterations are for < 1000 rows, which is SQL's limit
                int insertIterations = (int)Math.Ceiling(inserts.Count() / 1000f);
                for (var ii = 0; ii < insertIterations; ii++)
                {
                    var length = 1000;
                    if (length > (inserts.Count() % 1000))
                        length = inserts.Count() % 1000;

                    List<string> subset = inserts.GetRange(ii * 1000, length);
                    sb.Append("insert into Entities (");
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        sb.Append(dt.Columns[i].ColumnName);
                        if (i < (dt.Columns.Count - 1))
                            sb.Append(",");
                    }
                    sb.Append(") values ");
                    sb.Append(string.Join(",", subset.ToArray()));
                    sb.Append(";");
                }
            }

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.CommandTimeout = 5000;
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        DisplayReader(display, reader);
                    }
                }
            }

        }

    }
}
