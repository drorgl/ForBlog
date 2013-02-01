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
    /// Dynamic SQL Execution Demo without Insert optimizations
    /// </summary>
    class NoOptimizationsDynamicExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            //Does not implement select - irrelevant
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
                for (var ii = 0; ii < inserts.Count; ii++)
                {
                    sb.Append("insert into Entities (");
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        sb.Append(dt.Columns[i].ColumnName);
                        if (i < (dt.Columns.Count - 1))
                            sb.Append(",");
                    }
                    sb.Append(") values ");
                    sb.Append(inserts[ii]);
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
